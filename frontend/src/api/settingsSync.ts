// Sincroniza os cards de canal (Telegram / E-mail) da tela de Configurações
// com o backend:
//   - cada canal vira um Notification Destination
//   - cada severidade marcada ("Acionar em") vira/ativa uma Routing Rule
//     que aponta para aquele destino; desmarcada => regra desativada.
//
// Observação importante sobre arquitetura do backend:
//   - O Bot Token do Telegram e as credenciais SMTP são GLOBAIS (definidas no
//     servidor via variáveis de ambiente). O destino guarda apenas o que é
//     específico: chatId (Telegram) e recipients (E-mail).

import { destinationsApi, routingRulesApi } from '@/api'
import type {
  NotificationDestinationResponse,
  RoutingRuleResponse,
} from '@/api'

export type ChannelKind = 'telegram' | 'email'
export type SeverityKey = 'critical' | 'warning' | 'info'

export const SEVERITIES: SeverityKey[] = ['critical', 'warning', 'info']

const DESTINATION_NAME: Record<ChannelKind, string> = {
  telegram: 'Painel — Telegram',
  email: 'Painel — E-mail',
}

const CHANNEL_LABEL: Record<ChannelKind, string> = {
  telegram: 'Telegram',
  email: 'E-mail',
}

function ruleName(kind: ChannelKind, sev: SeverityKey): string {
  return `Painel: ${CHANNEL_LABEL[kind]} (${sev})`
}

export interface ChannelServerState {
  destinationId: string | null
  enabled: boolean
  // telegram
  chatId: string
  // email (string separada por vírgula, como na UI)
  recipients: string
  onCritical: boolean
  onWarning: boolean
  onInfo: boolean
}

function emptyState(): ChannelServerState {
  return {
    destinationId: null,
    enabled: false,
    chatId: '',
    recipients: '',
    onCritical: false,
    onWarning: false,
    onInfo: false,
  }
}

function parseConfig(json: string): Record<string, unknown> {
  try {
    return JSON.parse(json || '{}')
  } catch {
    return {}
  }
}

function severityFlags(
  rules: RoutingRuleResponse[],
  destinationId: string | null,
  kind: ChannelKind,
) {
  const flags = { onCritical: false, onWarning: false, onInfo: false }
  if (!destinationId) return flags
  for (const sev of SEVERITIES) {
    const rule = rules.find(
      (r) =>
        r.name === ruleName(kind, sev) &&
        r.destinations.some((d) => d.destinationId === destinationId),
    )
    const active = !!rule && rule.isActive
    if (sev === 'critical') flags.onCritical = active
    if (sev === 'warning') flags.onWarning = active
    if (sev === 'info') flags.onInfo = active
  }
  return flags
}

// Carrega o estado atual dos canais a partir do backend.
export async function loadServerConfig(): Promise<{
  telegram: ChannelServerState
  email: ChannelServerState
}> {
  const [destinations, rules] = await Promise.all([
    destinationsApi.list(),
    routingRulesApi.list(),
  ])

  function build(kind: ChannelKind): ChannelServerState {
    const dest = destinations.find(
      (d: NotificationDestinationResponse) => d.name === DESTINATION_NAME[kind],
    )
    const state = emptyState()
    if (!dest) return state

    state.destinationId = dest.id
    state.enabled = dest.isActive

    const cfg = parseConfig(dest.configurationJson)
    if (kind === 'telegram') {
      state.chatId = (cfg.chatId as string) ?? ''
    } else {
      const recipients = cfg.recipients
      state.recipients = Array.isArray(recipients) ? (recipients as string[]).join(', ') : ''
    }

    const flags = severityFlags(rules, dest.id, kind)
    state.onCritical = flags.onCritical
    state.onWarning = flags.onWarning
    state.onInfo = flags.onInfo
    return state
  }

  return { telegram: build('telegram'), email: build('email') }
}

function buildConfiguration(kind: ChannelKind, state: ChannelServerState): Record<string, unknown> {
  if (kind === 'telegram') {
    return { chatId: state.chatId.trim() }
  }
  const recipients = state.recipients
    .split(',')
    .map((r) => r.trim())
    .filter(Boolean)
  return { recipients }
}

// Cria/atualiza o destino e reconcilia as regras de roteamento por severidade.
// Retorna o id do destino salvo.
export async function saveChannel(
  kind: ChannelKind,
  state: ChannelServerState,
): Promise<string> {
  const configuration = buildConfiguration(kind, state)

  // 1) Upsert do destino
  let destinationId = state.destinationId
  if (destinationId) {
    await destinationsApi.update(destinationId, {
      name: DESTINATION_NAME[kind],
      type: kind,
      configuration,
      isActive: state.enabled,
    })
  } else {
    // pode já existir no backend (criado antes) — procura por nome
    const existing = (await destinationsApi.list()).find((d) => d.name === DESTINATION_NAME[kind])
    if (existing) {
      destinationId = existing.id
      await destinationsApi.update(destinationId, {
        name: DESTINATION_NAME[kind],
        type: kind,
        configuration,
        isActive: state.enabled,
      })
    } else {
      const created = await destinationsApi.create({
        name: DESTINATION_NAME[kind],
        type: kind,
        configuration,
      })
      destinationId = created.id
      if (!state.enabled) await destinationsApi.deactivate(destinationId)
    }
  }

  // 2) Reconcilia as regras de roteamento
  const rules = await routingRulesApi.list()
  const desired: Record<SeverityKey, boolean> = {
    critical: state.enabled && state.onCritical,
    warning: state.enabled && state.onWarning,
    info: state.enabled && state.onInfo,
  }

  let order = rules.length
  for (const sev of SEVERITIES) {
    const name = ruleName(kind, sev)
    const rule = rules.find((r) => r.name === name)

    if (desired[sev]) {
      if (!rule) {
        await routingRulesApi.create({
          name,
          order: order++,
          severity: sev,
          category: null,
          type: null,
          source: null,
          deliveryMode: 'immediate',
          throttleMinutes: null,
          destinationIds: [destinationId],
        })
      } else {
        await routingRulesApi.update(rule.id, {
          name,
          order: rule.order,
          severity: sev,
          category: rule.category ?? null,
          type: rule.type ?? null,
          source: rule.source ?? null,
          deliveryMode: rule.deliveryMode || 'immediate',
          throttleMinutes: rule.throttleMinutes ?? null,
          isActive: true,
          destinationIds: [destinationId],
        })
      }
    } else if (rule && rule.isActive) {
      await routingRulesApi.deactivate(rule.id)
    }
  }

  return destinationId
}

export async function testChannel(destinationId: string) {
  return destinationsApi.test(destinationId)
}
