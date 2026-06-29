<template>
  <div class="settings">
    <header class="settings__header">
      <h1 class="settings__title">Configurações</h1>
      <p class="settings__subtitle">Defina como e quando cada canal de notificação será acionado</p>
    </header>

    <!-- Conexão / Conta -->
    <div class="server-card">
      <div class="server-card__info">
        <div class="server-card__row">
          <span class="server-card__dot" :class="`server-card__dot--${connState}`" />
          <span class="server-card__label">Backend</span>
          <code class="server-card__url">{{ apiUrl }}</code>
          <span class="server-card__state">{{ connLabel }}</span>
        </div>
        <div class="server-card__row server-card__row--muted" v-if="auth.user">
          Conectado como <strong>{{ auth.user.name }}</strong> ({{ auth.user.email }})
        </div>
      </div>
      <div class="server-card__actions">
        <button class="btn btn--ghost" @click="reloadServer" :disabled="loadingServer">
          {{ loadingServer ? 'Carregando…' : 'Recarregar' }}
        </button>
        <button class="btn btn--ghost" @click="logout">Sair</button>
      </div>
    </div>

    <p v-if="serverError" class="server-error">{{ serverError }}</p>

    <div class="settings__grid">
      <!-- Telegram -->
      <div class="channel-card" :class="{ 'channel-card--active': s.telegram.enabled }">
        <div class="channel-card__header">
          <div class="channel-card__identity">
            <span class="channel-card__icon channel-card__icon--telegram">
              <svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 0C5.373 0 0 5.373 0 12s5.373 12 12 12 12-5.373 12-12S18.627 0 12 0zm5.894 8.221-1.97 9.28c-.145.658-.537.818-1.084.508l-3-2.21-1.447 1.394c-.16.16-.295.295-.605.295l.213-3.053 5.56-5.023c.242-.213-.054-.333-.373-.12l-6.871 4.326-2.962-.924c-.643-.204-.657-.643.136-.953l11.57-4.461c.537-.194 1.006.131.833.941z"/></svg>
            </span>
            <div>
              <h3 class="channel-card__name">Telegram</h3>
              <p class="channel-card__desc">Mensagens via Bot do Telegram</p>
            </div>
          </div>
          <label class="toggle">
            <input type="checkbox" v-model="s.telegram.enabled" />
            <span class="toggle__track"><span class="toggle__thumb" /></span>
          </label>
        </div>

        <Transition name="expand">
          <div v-if="s.telegram.enabled" class="channel-card__body">
            <p class="server-hint">
              O <strong>Token do Bot</strong> é configurado no servidor (variável <code>Telegram__BotToken</code>).
              Aqui você define apenas o <strong>Chat ID</strong> do destino.
            </p>
            <div class="field">
              <label class="field__label">Chat ID</label>
              <input class="field__input" v-model="s.telegram.chatId" placeholder="-100123456789" />
            </div>
            <div class="field">
              <label class="field__label">Acionar em</label>
              <div class="severity-checks">
                <label class="sev-check sev-check--critical"><input type="checkbox" v-model="s.telegram.onCritical" /> Crítico</label>
                <label class="sev-check sev-check--warning"><input type="checkbox" v-model="s.telegram.onWarning" /> Atenção</label>
                <label class="sev-check sev-check--info"><input type="checkbox" v-model="s.telegram.onInfo" /> Info</label>
              </div>
            </div>
            <div class="channel-card__actions">
              <button class="btn btn--primary" @click="saveTelegram" :disabled="savingTelegram">
                {{ savingTelegram ? 'Salvando…' : 'Salvar no servidor' }}
              </button>
              <button class="btn btn--ghost" @click="testTelegram" :disabled="testingTelegram">
                {{ testingTelegram ? 'Enviando…' : 'Testar envio' }}
              </button>
              <span v-if="telegramFeedback" class="feedback" :class="telegramFeedback.ok ? 'feedback--ok' : 'feedback--err'">
                {{ telegramFeedback.msg }}
              </span>
            </div>
          </div>
        </Transition>
      </div>

      <!-- Email -->
      <div class="channel-card" :class="{ 'channel-card--active': s.email.enabled }">
        <div class="channel-card__header">
          <div class="channel-card__identity">
            <span class="channel-card__icon channel-card__icon--email">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="2" y="4" width="20" height="16" rx="2"/><path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7"/></svg>
            </span>
            <div>
              <h3 class="channel-card__name">E-mail</h3>
              <p class="channel-card__desc">Envio via servidor SMTP</p>
            </div>
          </div>
          <label class="toggle">
            <input type="checkbox" v-model="s.email.enabled" />
            <span class="toggle__track"><span class="toggle__thumb" /></span>
          </label>
        </div>

        <Transition name="expand">
          <div v-if="s.email.enabled" class="channel-card__body">
            <p class="server-hint">
              As credenciais SMTP (host, porta, usuário e senha) são configuradas no servidor
              (variáveis <code>Email__*</code>). No destino salvamos apenas os
              <strong>destinatários</strong>.
            </p>
            <div class="field-row">
              <div class="field">
                <label class="field__label">Host SMTP <span class="field__hint">(servidor)</span></label>
                <input class="field__input" v-model="s.email.smtpHost" placeholder="smtp.empresa.com" />
              </div>
              <div class="field field--small">
                <label class="field__label">Porta</label>
                <input class="field__input" v-model.number="s.email.smtpPort" type="number" placeholder="587" />
              </div>
            </div>
            <div class="field-row">
              <div class="field">
                <label class="field__label">Usuário</label>
                <input class="field__input" v-model="s.email.user" placeholder="alertas@empresa.com" />
              </div>
              <div class="field">
                <label class="field__label">Senha</label>
                <input class="field__input" v-model="s.email.password" type="password" placeholder="••••••••" />
              </div>
            </div>
            <div class="field">
              <label class="field__label">Destinatários <span class="field__hint">(separados por vírgula)</span></label>
              <input class="field__input" v-model="s.email.recipients" placeholder="ti@empresa.com, gestor@empresa.com" />
            </div>
            <div class="field">
              <label class="field__label">Acionar em</label>
              <div class="severity-checks">
                <label class="sev-check sev-check--critical"><input type="checkbox" v-model="s.email.onCritical" /> Crítico</label>
                <label class="sev-check sev-check--warning"><input type="checkbox" v-model="s.email.onWarning" /> Atenção</label>
                <label class="sev-check sev-check--info"><input type="checkbox" v-model="s.email.onInfo" /> Info</label>
              </div>
            </div>
            <div class="channel-card__actions">
              <button class="btn btn--primary" @click="saveEmail" :disabled="savingEmail">
                {{ savingEmail ? 'Salvando…' : 'Salvar no servidor' }}
              </button>
              <button class="btn btn--ghost" @click="testEmail" :disabled="testingEmail">
                {{ testingEmail ? 'Enviando…' : 'Testar envio' }}
              </button>
              <span v-if="emailFeedback" class="feedback" :class="emailFeedback.ok ? 'feedback--ok' : 'feedback--err'">
                {{ emailFeedback.msg }}
              </span>
            </div>
          </div>
        </Transition>
      </div>

      <!-- Sound -->
      <div class="channel-card" :class="{ 'channel-card--active': s.sound.enabled }">
        <div class="channel-card__header">
          <div class="channel-card__identity">
            <span class="channel-card__icon channel-card__icon--sound">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polygon points="11,5 6,9 2,9 2,15 6,15 11,19 11,5"/><path d="M19.07 4.93a10 10 0 0 1 0 14.14"/><path d="M15.54 8.46a5 5 0 0 1 0 7.07"/></svg>
            </span>
            <div>
              <h3 class="channel-card__name">Sonoro</h3>
              <p class="channel-card__desc">Alerta sonoro no browser</p>
            </div>
          </div>
          <label class="toggle">
            <input type="checkbox" v-model="s.sound.enabled" />
            <span class="toggle__track"><span class="toggle__thumb" /></span>
          </label>
        </div>

        <Transition name="expand">
          <div v-if="s.sound.enabled" class="channel-card__body">
            <div class="field">
              <label class="field__label">Volume — {{ s.sound.volume }}%</label>
              <input class="field__range" type="range" v-model.number="s.sound.volume" min="10" max="100" step="5" />
            </div>
            <div class="field">
              <label class="field__label">Tom</label>
              <div class="radio-group">
                <label class="radio-option" v-for="tone in tones" :key="tone.value">
                  <input type="radio" v-model="s.sound.tone" :value="tone.value" />
                  <span>{{ tone.label }}</span>
                </label>
              </div>
            </div>
            <div class="field">
              <label class="field__label">Acionar em</label>
              <div class="severity-checks">
                <label class="sev-check sev-check--critical"><input type="checkbox" v-model="s.sound.onCritical" /> Crítico</label>
                <label class="sev-check sev-check--warning"><input type="checkbox" v-model="s.sound.onWarning" /> Atenção</label>
                <label class="sev-check sev-check--info"><input type="checkbox" v-model="s.sound.onInfo" /> Info</label>
              </div>
            </div>
            <div class="channel-card__actions">
              <button class="btn btn--ghost" @click="testSound">▶ Testar som</button>
            </div>
          </div>
        </Transition>
      </div>

      <!-- App / Push -->
      <div class="channel-card" :class="{ 'channel-card--active': s.app.enabled }">
        <div class="channel-card__header">
          <div class="channel-card__identity">
            <span class="channel-card__icon channel-card__icon--app">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/></svg>
            </span>
            <div>
              <h3 class="channel-card__name">Aplicativo</h3>
              <p class="channel-card__desc">Push Notification no browser</p>
            </div>
          </div>
          <label class="toggle">
            <input type="checkbox" v-model="s.app.enabled" />
            <span class="toggle__track"><span class="toggle__thumb" /></span>
          </label>
        </div>

        <Transition name="expand">
          <div v-if="s.app.enabled" class="channel-card__body">
            <div class="permission-status" :class="`permission-status--${permissionStatus}`">
              <span class="permission-status__dot" />
              <span>{{ permissionLabel }}</span>
            </div>
            <div class="field">
              <label class="field__label">Acionar em</label>
              <div class="severity-checks">
                <label class="sev-check sev-check--critical"><input type="checkbox" v-model="s.app.onCritical" /> Crítico</label>
                <label class="sev-check sev-check--warning"><input type="checkbox" v-model="s.app.onWarning" /> Atenção</label>
                <label class="sev-check sev-check--info"><input type="checkbox" v-model="s.app.onInfo" /> Info</label>
              </div>
            </div>
            <div class="channel-card__actions">
              <button v-if="permissionStatus === 'default'" class="btn btn--primary" @click="requestPermission">
                Permitir notificações
              </button>
              <button v-else-if="permissionStatus === 'granted'" class="btn btn--ghost" @click="testPush">
                Testar notificação
              </button>
              <span v-else class="feedback feedback--err">
                Permissão negada — ajuste nas configurações do browser
              </span>
            </div>
          </div>
        </Transition>
      </div>
    </div>

    <!-- Origens permitidas (CORS) -->
    <div class="origins-card">
      <div class="origins-card__header">
        <div>
          <h3 class="origins-card__title">Origens permitidas (CORS)</h3>
          <p class="origins-card__desc">
            Domínios autorizados a consumir a API. Inclua a URL do painel (ex.: http://localhost:5173).
          </p>
        </div>
        <button class="btn btn--ghost" @click="loadOrigins" :disabled="originsLoading">
          {{ originsLoading ? 'Carregando…' : 'Recarregar' }}
        </button>
      </div>

      <form class="origins-form" @submit.prevent="addOrigin">
        <input class="field__input" v-model="newOrigin" placeholder="https://app.suaempresa.com" />
        <input class="field__input" v-model="newOriginDesc" placeholder="Descrição (opcional)" />
        <button class="btn btn--primary" type="submit" :disabled="originSaving || !newOrigin.trim()">
          {{ originSaving ? 'Adicionando…' : 'Adicionar' }}
        </button>
      </form>

      <p v-if="originsError" class="feedback feedback--err">{{ originsError }}</p>

      <ul class="origins-list" v-if="origins.length">
        <li
          v-for="o in origins"
          :key="o.id"
          class="origins-item"
          :class="{ 'origins-item--off': !o.isActive }"
        >
          <div class="origins-item__info">
            <code class="origins-item__url">{{ o.origin }}</code>
            <span v-if="o.description" class="origins-item__desc">{{ o.description }}</span>
          </div>
          <div class="origins-item__actions">
            <label class="toggle">
              <input type="checkbox" :checked="o.isActive" @change="toggleOrigin(o)" />
              <span class="toggle__track"><span class="toggle__thumb" /></span>
            </label>
            <button class="btn btn--danger" @click="removeOrigin(o)">Remover</button>
          </div>
        </li>
      </ul>
      <p v-else class="origins-empty">
        Nenhuma origem cadastrada. Se a configuração do servidor também estiver vazia, o backend libera todas as origens.
      </p>
    </div>

    <!-- Save feedback -->
    <Transition name="fade">
      <div v-if="saved" class="save-toast">✓ Configurações salvas automaticamente</div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSettingsStore } from '@/stores/settings'
import { useAuthStore } from '@/stores/auth'
import { ApiError, corsOriginsApi, type AllowedOriginResponse } from '@/api'
import {
  loadServerConfig,
  saveChannel,
  testChannel,
  type ChannelServerState,
} from '@/api/settingsSync'

const s = useSettingsStore()
const auth = useAuthStore()
const router = useRouter()

// ---- Conexão / Conta ----
const apiUrl = (import.meta.env.VITE_API_URL as string | undefined) || '(proxy /api)'
const loadingServer = ref(false)
const serverError = ref<string | null>(null)
const connState = ref<'idle' | 'ok' | 'err'>('idle')
const connLabel = computed(() =>
  connState.value === 'ok' ? 'conectado' : connState.value === 'err' ? 'sem conexão' : 'verificando…',
)

// ids dos destinos no backend (para o botão de testar)
const telegramDestinationId = ref<string | null>(null)
const emailDestinationId = ref<string | null>(null)

function buildState(kind: 'telegram' | 'email'): ChannelServerState {
  if (kind === 'telegram') {
    return {
      destinationId: telegramDestinationId.value,
      enabled: s.telegram.enabled,
      chatId: s.telegram.chatId,
      recipients: '',
      onCritical: s.telegram.onCritical,
      onWarning: s.telegram.onWarning,
      onInfo: s.telegram.onInfo,
    }
  }
  return {
    destinationId: emailDestinationId.value,
    enabled: s.email.enabled,
    chatId: '',
    recipients: s.email.recipients,
    onCritical: s.email.onCritical,
    onWarning: s.email.onWarning,
    onInfo: s.email.onInfo,
  }
}

async function reloadServer() {
  loadingServer.value = true
  serverError.value = null
  try {
    const cfg = await loadServerConfig()
    connState.value = 'ok'

    if (cfg.telegram.destinationId) {
      telegramDestinationId.value = cfg.telegram.destinationId
      s.telegram.enabled = cfg.telegram.enabled
      s.telegram.chatId = cfg.telegram.chatId
      s.telegram.onCritical = cfg.telegram.onCritical
      s.telegram.onWarning = cfg.telegram.onWarning
      s.telegram.onInfo = cfg.telegram.onInfo
    }
    if (cfg.email.destinationId) {
      emailDestinationId.value = cfg.email.destinationId
      s.email.enabled = cfg.email.enabled
      s.email.recipients = cfg.email.recipients
      s.email.onCritical = cfg.email.onCritical
      s.email.onWarning = cfg.email.onWarning
      s.email.onInfo = cfg.email.onInfo
    }
  } catch (err) {
    connState.value = 'err'
    serverError.value = err instanceof ApiError ? err.message : 'Falha ao carregar configurações do servidor.'
  } finally {
    loadingServer.value = false
  }
}

function logout() {
  auth.logout()
  router.replace('/login')
}

onMounted(reloadServer)

// ---- CRUD de origens permitidas (CORS) ----
const origins = ref<AllowedOriginResponse[]>([])
const originsLoading = ref(false)
const originsError = ref<string | null>(null)
const newOrigin = ref('')
const newOriginDesc = ref('')
const originSaving = ref(false)

async function loadOrigins() {
  originsLoading.value = true
  originsError.value = null
  try {
    origins.value = await corsOriginsApi.list()
  } catch (err) {
    originsError.value = err instanceof ApiError ? err.message : 'Falha ao carregar origens.'
  } finally {
    originsLoading.value = false
  }
}

async function addOrigin() {
  if (!newOrigin.value.trim()) return
  originSaving.value = true
  originsError.value = null
  try {
    await corsOriginsApi.create({
      origin: newOrigin.value.trim(),
      description: newOriginDesc.value.trim() || null,
    })
    newOrigin.value = ''
    newOriginDesc.value = ''
    await loadOrigins()
  } catch (err) {
    originsError.value = err instanceof ApiError ? err.message : 'Falha ao adicionar origem.'
  } finally {
    originSaving.value = false
  }
}

async function toggleOrigin(o: AllowedOriginResponse) {
  originsError.value = null
  try {
    if (o.isActive) await corsOriginsApi.deactivate(o.id)
    else await corsOriginsApi.activate(o.id)
    await loadOrigins()
  } catch (err) {
    originsError.value = err instanceof ApiError ? err.message : 'Falha ao atualizar origem.'
    await loadOrigins()
  }
}

async function removeOrigin(o: AllowedOriginResponse) {
  originsError.value = null
  try {
    await corsOriginsApi.remove(o.id)
    await loadOrigins()
  } catch (err) {
    originsError.value = err instanceof ApiError ? err.message : 'Falha ao remover origem.'
  }
}

onMounted(loadOrigins)

// ---- Salvar canais no backend ----
const savingTelegram = ref(false)
const savingEmail = ref(false)

async function saveTelegram() {
  savingTelegram.value = true
  telegramFeedback.value = null
  try {
    const id = await saveChannel('telegram', buildState('telegram'))
    telegramDestinationId.value = id
    telegramFeedback.value = { ok: true, msg: 'Destino e regras salvos no servidor.' }
  } catch (err) {
    telegramFeedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao salvar.' }
  } finally {
    savingTelegram.value = false
    setTimeout(() => (telegramFeedback.value = null), 4000)
  }
}

async function saveEmail() {
  savingEmail.value = true
  emailFeedback.value = null
  try {
    const id = await saveChannel('email', buildState('email'))
    emailDestinationId.value = id
    emailFeedback.value = { ok: true, msg: 'Destino e regras salvos no servidor.' }
  } catch (err) {
    emailFeedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao salvar.' }
  } finally {
    savingEmail.value = false
    setTimeout(() => (emailFeedback.value = null), 4000)
  }
}

// Auto-save toast (persistência local de som/app e preferências)
const saved = ref(false)
let saveTimer: ReturnType<typeof setTimeout>
watch([s.telegram, s.email, s.sound, s.app], () => {
  clearTimeout(saveTimer)
  saved.value = true
  saveTimer = setTimeout(() => (saved.value = false), 2000)
}, { deep: true })

// Tone options
const tones = [
  { value: 'beep', label: 'Beep' },
  { value: 'chime', label: 'Chime' },
  { value: 'alert', label: 'Alerta' },
]

// Sound test via Web Audio API
function testSound() {
  const ctx = new AudioContext()
  const gain = ctx.createGain()
  gain.connect(ctx.destination)
  gain.gain.setValueAtTime(s.sound.volume / 100, ctx.currentTime)

  if (s.sound.tone === 'beep') {
    const osc = ctx.createOscillator()
    osc.connect(gain)
    osc.frequency.value = 880
    osc.type = 'square'
    gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.4)
    osc.start(); osc.stop(ctx.currentTime + 0.4)
  } else if (s.sound.tone === 'chime') {
    [1047, 1319, 1568].forEach((freq, i) => {
      const osc = ctx.createOscillator()
      osc.connect(gain)
      osc.frequency.value = freq
      osc.type = 'sine'
      osc.start(ctx.currentTime + i * 0.15)
      osc.stop(ctx.currentTime + i * 0.15 + 0.3)
    })
  } else {
    // alert: two alternating tones
    [0, 0.2, 0.4].forEach((offset) => {
      const osc = ctx.createOscillator()
      osc.connect(gain)
      osc.frequency.value = offset % 0.4 === 0 ? 440 : 880
      osc.type = 'sawtooth'
      osc.start(ctx.currentTime + offset)
      osc.stop(ctx.currentTime + offset + 0.18)
    })
  }
}

// Telegram test — garante que o destino está salvo e chama /test no backend
const testingTelegram = ref(false)
const telegramFeedback = ref<{ ok: boolean; msg: string } | null>(null)
async function testTelegram() {
  testingTelegram.value = true
  telegramFeedback.value = null
  try {
    const id = await saveChannel('telegram', buildState('telegram'))
    telegramDestinationId.value = id
    const res = await testChannel(id)
    telegramFeedback.value = { ok: res.success, msg: res.success ? res.message : res.error || res.message }
  } catch (err) {
    telegramFeedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao testar.' }
  } finally {
    testingTelegram.value = false
    setTimeout(() => (telegramFeedback.value = null), 5000)
  }
}

// Email test — garante que o destino está salvo e chama /test no backend
const testingEmail = ref(false)
const emailFeedback = ref<{ ok: boolean; msg: string } | null>(null)
async function testEmail() {
  testingEmail.value = true
  emailFeedback.value = null
  try {
    const id = await saveChannel('email', buildState('email'))
    emailDestinationId.value = id
    const res = await testChannel(id)
    emailFeedback.value = { ok: res.success, msg: res.success ? res.message : res.error || res.message }
  } catch (err) {
    emailFeedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao testar.' }
  } finally {
    testingEmail.value = false
    setTimeout(() => (emailFeedback.value = null), 5000)
  }
}

// Push / App notification
const permissionStatus = ref<NotificationPermission>(
  'Notification' in window ? Notification.permission : 'denied',
)
const permissionLabel = computed(() => {
  const labels: Record<NotificationPermission, string> = {
    granted: 'Permissão concedida',
    denied: 'Permissão negada',
    default: 'Permissão não solicitada',
  }
  return labels[permissionStatus.value]
})
async function requestPermission() {
  const result = await Notification.requestPermission()
  permissionStatus.value = result
}
function testPush() {
  new Notification('Central de Alertas — Teste', {
    body: 'Notificação de teste funcionando corretamente.',
    icon: '/favicon.svg',
  })
}
</script>

<style scoped>
.settings {
  padding: 32px 40px;
  max-width: 960px;
  margin: 0 auto;
}

/* Header */
.settings__header {
  margin-bottom: 32px;
}
.settings__title {
  font-size: 24px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 6px;
}
.settings__subtitle {
  font-size: 14px;
  color: var(--text-muted);
  margin: 0;
}

/* Grid */
.settings__grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
}
@media (max-width: 720px) {
  .settings__grid { grid-template-columns: 1fr; }
}

/* Channel card */
.channel-card {
  background: var(--card-bg);
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.07);
  overflow: hidden;
  transition: border-color 0.2s;
}
.channel-card--active {
  border-color: rgba(99, 102, 241, 0.4);
}

.channel-card__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 20px 20px 16px;
}
.channel-card__identity {
  display: flex;
  align-items: center;
  gap: 14px;
}
.channel-card__icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.channel-card__icon svg { width: 20px; height: 20px; }

.channel-card__icon--telegram { background: rgba(41, 182, 246, 0.15); color: #29b6f6; }
.channel-card__icon--email    { background: rgba(99, 102, 241, 0.15); color: #818cf8; }
.channel-card__icon--sound    { background: rgba(245, 158, 11, 0.15); color: #fbbf24; }
.channel-card__icon--app      { background: rgba(34, 197, 94, 0.15);  color: #4ade80; }

.channel-card__name {
  font-size: 15px;
  font-weight: 600;
  color: var(--text-primary);
  margin: 0 0 2px;
}
.channel-card__desc {
  font-size: 12px;
  color: var(--text-muted);
  margin: 0;
}

.channel-card__body {
  padding: 0 20px 20px;
  border-top: 1px solid rgba(255, 255, 255, 0.06);
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding-top: 16px;
}

/* Toggle switch */
.toggle {
  position: relative;
  display: inline-block;
  width: 44px;
  height: 24px;
  cursor: pointer;
  flex-shrink: 0;
}
.toggle input { opacity: 0; width: 0; height: 0; position: absolute; }
.toggle__track {
  position: absolute;
  inset: 0;
  border-radius: 12px;
  background: #1e293b;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: background 0.2s, border-color 0.2s;
}
.toggle input:checked + .toggle__track {
  background: #6366f1;
  border-color: #6366f1;
}
.toggle__thumb {
  position: absolute;
  top: 2px;
  left: 2px;
  width: 18px;
  height: 18px;
  border-radius: 50%;
  background: white;
  transition: transform 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.3);
}
.toggle input:checked + .toggle__track .toggle__thumb {
  transform: translateX(20px);
}

/* Fields */
.field { display: flex; flex-direction: column; gap: 6px; }
.field--small { flex: 0 0 80px; }
.field__label {
  font-size: 12px;
  font-weight: 600;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.field__hint { font-weight: 400; text-transform: none; letter-spacing: 0; }
.field__input {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  padding: 8px 12px;
  font-size: 13px;
  color: var(--text-primary);
  font-family: inherit;
  outline: none;
  transition: border-color 0.15s;
  width: 100%;
}
.field__input:focus { border-color: #6366f1; }
.field__input::placeholder { color: var(--text-muted); }

.field-row { display: flex; gap: 12px; }
.field-row .field { flex: 1; }

/* Range slider */
.field__range {
  -webkit-appearance: none;
  width: 100%;
  height: 4px;
  border-radius: 2px;
  background: rgba(255, 255, 255, 0.1);
  outline: none;
  cursor: pointer;
}
.field__range::-webkit-slider-thumb {
  -webkit-appearance: none;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  background: #6366f1;
  cursor: pointer;
  box-shadow: 0 0 0 2px rgba(99, 102, 241, 0.3);
}

/* Radio group */
.radio-group { display: flex; gap: 16px; }
.radio-option {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: var(--text-secondary);
  cursor: pointer;
}
.radio-option input { accent-color: #6366f1; }

/* Severity checks */
.severity-checks { display: flex; gap: 16px; flex-wrap: wrap; }
.sev-check {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  padding: 4px 10px;
  border-radius: 6px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: background 0.15s;
}
.sev-check input { display: none; }
.sev-check--critical { color: #ef4444; }
.sev-check--warning  { color: #f59e0b; }
.sev-check--info     { color: #60a5fa; }

.sev-check:has(input:checked).sev-check--critical { background: rgba(239,68,68,0.12); border-color: rgba(239,68,68,0.3); }
.sev-check:has(input:checked).sev-check--warning  { background: rgba(245,158,11,0.12); border-color: rgba(245,158,11,0.3); }
.sev-check:has(input:checked).sev-check--info     { background: rgba(59,130,246,0.12); border-color: rgba(59,130,246,0.3); }

/* Permission status */
.permission-status {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  padding: 8px 12px;
  border-radius: 8px;
  background: rgba(255,255,255,0.04);
}
.permission-status__dot {
  width: 8px; height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
}
.permission-status--granted { color: #4ade80; }
.permission-status--granted .permission-status__dot { background: #4ade80; }
.permission-status--denied  { color: #f87171; }
.permission-status--denied  .permission-status__dot { background: #f87171; }
.permission-status--default { color: #94a3b8; }
.permission-status--default .permission-status__dot { background: #475569; }

/* Actions */
.channel-card__actions {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

/* Buttons */
.btn {
  padding: 7px 16px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  border: none;
  transition: background 0.15s, opacity 0.15s;
  font-family: inherit;
}
.btn:disabled { opacity: 0.5; cursor: not-allowed; }
.btn--ghost {
  background: rgba(255, 255, 255, 0.07);
  color: var(--text-secondary);
  border: 1px solid rgba(255, 255, 255, 0.1);
}
.btn--ghost:hover:not(:disabled) { background: rgba(255, 255, 255, 0.12); }
.btn--primary {
  background: #6366f1;
  color: white;
}
.btn--primary:hover { background: #4f46e5; }

/* Feedback */
.feedback { font-size: 12px; }
.feedback--ok  { color: #4ade80; }
.feedback--err { color: #f87171; }

/* Save toast */
.save-toast {
  position: fixed;
  bottom: 24px;
  left: 50%;
  transform: translateX(-50%);
  background: #1e293b;
  border: 1px solid rgba(255,255,255,0.12);
  color: #4ade80;
  padding: 10px 20px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  z-index: 200;
  box-shadow: 0 4px 16px rgba(0,0,0,0.4);
}

/* Transitions */
.expand-enter-active,
.expand-leave-active {
  transition: opacity 0.2s, max-height 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  max-height: 600px;
  overflow: hidden;
}
.expand-enter-from,
.expand-leave-to {
  opacity: 0;
  max-height: 0;
}

.fade-enter-active,
.fade-leave-active { transition: opacity 0.3s; }
.fade-enter-from,
.fade-leave-to { opacity: 0; }

/* Server / connection card */
.server-card {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  flex-wrap: wrap;
  background: var(--card-bg);
  border: 1px solid rgba(255, 255, 255, 0.07);
  border-radius: 12px;
  padding: 16px 20px;
  margin-bottom: 16px;
}
.server-card__info { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
.server-card__row {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 13px;
  color: var(--text-secondary);
  flex-wrap: wrap;
}
.server-card__row--muted { color: var(--text-muted); font-size: 12px; }
.server-card__label {
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--text-muted);
}
.server-card__url {
  font-family: monospace;
  font-size: 12px;
  color: var(--text-primary);
  background: rgba(255, 255, 255, 0.05);
  padding: 2px 8px;
  border-radius: 6px;
}
.server-card__state { font-size: 12px; color: var(--text-muted); }
.server-card__dot { width: 9px; height: 9px; border-radius: 50%; flex-shrink: 0; }
.server-card__dot--ok  { background: #4ade80; box-shadow: 0 0 6px #4ade80; }
.server-card__dot--err { background: #f87171; }
.server-card__dot--idle { background: #475569; }
.server-card__actions { display: flex; gap: 8px; flex-shrink: 0; }

.server-error {
  color: #f87171;
  font-size: 13px;
  margin: 0 0 16px;
}

.server-hint {
  font-size: 12px;
  line-height: 1.5;
  color: var(--text-muted);
  background: rgba(99, 102, 241, 0.08);
  border: 1px solid rgba(99, 102, 241, 0.2);
  border-radius: 8px;
  padding: 10px 12px;
  margin: 0;
}
.server-hint code {
  font-family: monospace;
  color: var(--text-secondary);
  background: rgba(255, 255, 255, 0.06);
  padding: 1px 5px;
  border-radius: 4px;
}

/* Origins (CORS) card */
.origins-card {
  margin-top: 16px;
  background: var(--card-bg);
  border: 1px solid rgba(255, 255, 255, 0.07);
  border-radius: 12px;
  padding: 20px;
}
.origins-card__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 16px;
}
.origins-card__title {
  font-size: 15px;
  font-weight: 600;
  color: var(--text-primary);
  margin: 0 0 4px;
}
.origins-card__desc {
  font-size: 12px;
  color: var(--text-muted);
  margin: 0;
}
.origins-form {
  display: flex;
  gap: 10px;
  margin-bottom: 14px;
  flex-wrap: wrap;
}
.origins-form .field__input { flex: 1; min-width: 160px; }

.origins-list {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin: 0;
  padding: 0;
}
.origins-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 10px 14px;
  border-radius: 8px;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.06);
}
.origins-item--off { opacity: 0.55; }
.origins-item__info { display: flex; flex-direction: column; gap: 2px; min-width: 0; }
.origins-item__url {
  font-family: monospace;
  font-size: 13px;
  color: var(--text-primary);
}
.origins-item__desc { font-size: 12px; color: var(--text-muted); }
.origins-item__actions { display: flex; align-items: center; gap: 12px; flex-shrink: 0; }

.origins-empty {
  font-size: 13px;
  color: var(--text-muted);
  margin: 0;
}

.btn--danger {
  background: rgba(239, 68, 68, 0.12);
  color: #f87171;
  border: 1px solid rgba(239, 68, 68, 0.3);
}
.btn--danger:hover { background: rgba(239, 68, 68, 0.2); }
</style>
