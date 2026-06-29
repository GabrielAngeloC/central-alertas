<template>
  <div class="rules">
    <div class="rules__cols">
      <!-- Lista de regras (avaliadas de cima para baixo) -->
      <div class="rules__list">
        <div class="rules__list-head">
          <h3 class="rules__h">Regras <span class="rules__hint">(avaliadas de cima para baixo)</span></h3>
          <button class="btn btn--primary btn--sm" @click="startNew">+ Nova</button>
        </div>

        <p v-if="loadError" class="feedback feedback--err">{{ loadError }}</p>

        <ul class="rule-items">
          <li
            v-for="(rule, i) in rules"
            :key="rule.id"
            class="rule-item"
            :class="{ 'rule-item--sel': selectedId === rule.id, 'rule-item--off': !rule.isActive }"
            @click="select(rule)"
          >
            <div class="rule-item__main">
              <div class="rule-item__cond">{{ conditionSummary(rule) }}</div>
              <div class="rule-item__act">{{ actionSummary(rule) }}</div>
            </div>
            <div class="rule-item__side">
              <button class="reorder" :disabled="i === 0" @click.stop="moveUp(i)" title="Subir">▲</button>
              <button class="reorder" :disabled="i === rules.length - 1" @click.stop="moveDown(i)" title="Descer">▼</button>
              <button
                class="dot"
                :class="rule.isActive ? 'dot--on' : 'dot--off'"
                @click.stop="toggleActive(rule)"
                :title="rule.isActive ? 'Ativa' : 'Inativa'"
              />
            </div>
          </li>
        </ul>
      </div>

      <!-- Editor -->
      <div class="rules__editor" v-if="form">
        <h3 class="rules__h">{{ form.id ? 'Editar regra' : 'Nova regra' }}</h3>

        <div class="field">
          <label class="field__label">Nome</label>
          <input class="field__input" v-model="form.name" placeholder="Ex.: Críticos para operação" />
        </div>

        <label class="field__label">Condições</label>
        <div class="cond-grid">
          <div class="field">
            <label class="field__sub">severity</label>
            <select class="field__input" v-model="form.severity">
              <option value="">(qualquer)</option>
              <option value="critical">critical</option>
              <option value="warning">warning</option>
              <option value="info">info</option>
            </select>
          </div>
          <div class="field">
            <label class="field__sub">category</label>
            <input class="field__input" v-model="form.category" placeholder="(qualquer)" />
          </div>
          <div class="field">
            <label class="field__sub">type</label>
            <input class="field__input" v-model="form.type" placeholder="(qualquer)" />
          </div>
          <div class="field">
            <label class="field__sub">source</label>
            <input class="field__input" v-model="form.source" placeholder="(qualquer)" />
          </div>
        </div>

        <div class="field-row">
          <div class="field">
            <label class="field__label">Entrega</label>
            <select class="field__input" v-model="form.deliveryMode">
              <option value="immediate">imediata</option>
              <option value="throttle">throttle</option>
              <option value="digest">digest</option>
            </select>
          </div>
          <div class="field" v-if="form.deliveryMode !== 'immediate'">
            <label class="field__label">Minutos</label>
            <input class="field__input" type="number" v-model.number="form.throttleMinutes" placeholder="30" />
          </div>
        </div>

        <div class="field">
          <label class="field__label">Destinos</label>
          <div v-if="destinations.length" class="dest-checks">
            <label v-for="d in destinations" :key="d.id" class="dest-check">
              <input type="checkbox" :value="d.id" v-model="form.destinationIds" />
              {{ d.name }} <span class="dest-type">{{ d.type }}</span>
            </label>
          </div>
          <p v-else class="field__hint">Nenhum destino cadastrado — crie na aba Destinos.</p>
        </div>

        <label class="toggle-line">
          <input type="checkbox" v-model="form.isActive" /> Regra ativa
        </label>

        <div class="rules__actions">
          <button class="btn btn--primary" @click="save" :disabled="saving">
            {{ saving ? 'Salvando…' : 'Salvar regra' }}
          </button>
          <button class="btn btn--ghost" @click="sendTest" :disabled="testing || !form.id">
            {{ testing ? 'Enviando…' : 'Enviar teste' }}
          </button>
          <button v-if="form.id" class="btn btn--danger" @click="remove" :disabled="removing">
            {{ removing ? 'Removendo…' : 'Remover' }}
          </button>
          <span v-if="feedback" class="feedback" :class="feedback.ok ? 'feedback--ok' : 'feedback--err'">
            {{ feedback.msg }}
          </span>
        </div>
        <p class="rules__note" v-if="!form.id">A regra recebe a próxima ordem de avaliação ao salvar.</p>
        <p class="rules__note" v-else>"Enviar teste" dispara uma mensagem de teste para cada destino da regra.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  routingRulesApi,
  destinationsApi,
  ApiError,
  type RoutingRuleResponse,
  type NotificationDestinationResponse,
} from '@/api'

interface RuleForm {
  id: string | null
  name: string
  order: number
  severity: string
  category: string
  type: string
  source: string
  deliveryMode: string
  throttleMinutes: number | null
  destinationIds: string[]
  isActive: boolean
}

const rules = ref<RoutingRuleResponse[]>([])
const destinations = ref<NotificationDestinationResponse[]>([])
const selectedId = ref<string | null>(null)
const form = ref<RuleForm | null>(null)
const saving = ref(false)
const testing = ref(false)
const removing = ref(false)
const loadError = ref<string | null>(null)
const feedback = ref<{ ok: boolean; msg: string } | null>(null)

function conditionSummary(r: RoutingRuleResponse): string {
  const parts: string[] = []
  if (r.severity) parts.push(`severity = ${r.severity}`)
  if (r.category) parts.push(`category = ${r.category}`)
  if (r.type) parts.push(`type = ${r.type}`)
  if (r.source) parts.push(`source = ${r.source}`)
  return parts.length ? `SE ${parts.join(' E ')}` : 'PADRÃO (tudo)'
}

function actionSummary(r: RoutingRuleResponse): string {
  const dests = r.destinations.map((d) => d.name).join(', ') || 'sem destino'
  const mode =
    r.deliveryMode === 'immediate'
      ? 'imediata'
      : `${r.deliveryMode}${r.throttleMinutes ? ` ${r.throttleMinutes}min` : ''}`
  return `→ ${dests} · ${mode}`
}

function toForm(r: RoutingRuleResponse): RuleForm {
  return {
    id: r.id,
    name: r.name,
    order: r.order,
    severity: r.severity ?? '',
    category: r.category ?? '',
    type: r.type ?? '',
    source: r.source ?? '',
    deliveryMode: r.deliveryMode || 'immediate',
    throttleMinutes: r.throttleMinutes ?? null,
    destinationIds: r.destinations.map((d) => d.destinationId),
    isActive: r.isActive,
  }
}

function select(r: RoutingRuleResponse) {
  selectedId.value = r.id
  form.value = toForm(r)
  feedback.value = null
}

function startNew() {
  selectedId.value = null
  feedback.value = null
  form.value = {
    id: null,
    name: '',
    order: rules.value.length + 1,
    severity: '',
    category: '',
    type: '',
    source: '',
    deliveryMode: 'immediate',
    throttleMinutes: null,
    destinationIds: [],
    isActive: true,
  }
}

function payload(f: RuleForm) {
  return {
    name: f.name.trim(),
    order: f.order,
    severity: f.severity || null,
    category: f.category.trim() || null,
    type: f.type.trim() || null,
    source: f.source.trim() || null,
    deliveryMode: f.deliveryMode,
    throttleMinutes: f.deliveryMode === 'immediate' ? null : f.throttleMinutes,
    destinationIds: f.destinationIds,
  }
}

async function load() {
  loadError.value = null
  try {
    const [r, d] = await Promise.all([routingRulesApi.list(), destinationsApi.list()])
    rules.value = [...r].sort((a, b) => a.order - b.order)
    destinations.value = d
  } catch (err) {
    if (err instanceof ApiError && err.status === 401) return
    loadError.value = err instanceof ApiError ? err.message : 'Falha ao carregar regras.'
  }
}

async function save() {
  if (!form.value) return
  saving.value = true
  feedback.value = null
  try {
    if (form.value.id) {
      await routingRulesApi.update(form.value.id, { ...payload(form.value), isActive: form.value.isActive })
    } else {
      const created = await routingRulesApi.create(payload(form.value))
      if (!form.value.isActive) await routingRulesApi.deactivate(created.id)
    }
    feedback.value = { ok: true, msg: 'Regra salva.' }
    await load()
  } catch (err) {
    feedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao salvar.' }
  } finally {
    saving.value = false
  }
}

async function toggleActive(r: RoutingRuleResponse) {
  try {
    if (r.isActive) await routingRulesApi.deactivate(r.id)
    else await routingRulesApi.activate(r.id)
    await load()
  } catch {
    // recarrega para refletir o estado real
    await load()
  }
}

async function swapOrder(a: RoutingRuleResponse, b: RoutingRuleResponse) {
  const orderA = a.order
  const orderB = b.order
  await routingRulesApi.update(a.id, {
    ...payload(toForm(a)),
    order: orderB,
    isActive: a.isActive,
  })
  await routingRulesApi.update(b.id, {
    ...payload(toForm(b)),
    order: orderA,
    isActive: b.isActive,
  })
  await load()
}

async function moveUp(i: number) {
  if (i === 0) return
  await swapOrder(rules.value[i], rules.value[i - 1])
}
async function moveDown(i: number) {
  if (i >= rules.value.length - 1) return
  await swapOrder(rules.value[i], rules.value[i + 1])
}

async function remove() {
  if (!form.value?.id) return
  if (!window.confirm('Remover esta regra? O histórico de entregas é preservado.')) return
  removing.value = true
  feedback.value = null
  try {
    await routingRulesApi.remove(form.value.id)
    form.value = null
    selectedId.value = null
    await load()
  } catch (err) {
    feedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao remover.' }
  } finally {
    removing.value = false
  }
}

async function sendTest() {
  if (!form.value?.id) return
  testing.value = true
  feedback.value = null
  try {
    const ids = form.value.destinationIds
    if (ids.length === 0) {
      feedback.value = { ok: false, msg: 'A regra não tem destinos.' }
      return
    }
    const results = await Promise.all(ids.map((id) => destinationsApi.test(id)))
    const ok = results.filter((r) => r.success).length
    feedback.value = {
      ok: ok === results.length,
      msg: `Teste: ${ok}/${results.length} destino(s) com sucesso.`,
    }
  } catch (err) {
    feedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha no teste.' }
  } finally {
    testing.value = false
  }
}

onMounted(load)
</script>

<style scoped>
.rules__cols { display: grid; grid-template-columns: 1.1fr 1fr; gap: 18px; }
@media (max-width: 820px) { .rules__cols { grid-template-columns: 1fr; } }

.rules__list-head { display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; }
.rules__h { font-size: 14px; font-weight: 600; color: var(--text-primary); margin: 0; }
.rules__hint { font-weight: 400; font-size: 12px; color: var(--text-muted); }

.rule-items { list-style: none; margin: 0; padding: 0; display: flex; flex-direction: column; gap: 8px; }
.rule-item {
  display: flex; align-items: center; gap: 10px;
  background: var(--card-bg); border: 1px solid var(--border-color);
  border-radius: 8px; padding: 10px 12px; cursor: pointer;
}
.rule-item--sel { border-color: rgba(99, 102, 241, 0.55); }
.rule-item--off { opacity: 0.55; }
.rule-item__main { flex: 1; min-width: 0; }
.rule-item__cond { font-family: monospace; font-size: 12px; color: #818cf8; }
.rule-item__act { font-size: 12px; color: var(--text-muted); margin-top: 3px; }
.rule-item__side { display: flex; align-items: center; gap: 4px; }
.reorder {
  background: none; border: 1px solid var(--border-color); color: var(--text-muted);
  border-radius: 4px; width: 22px; height: 20px; font-size: 9px; cursor: pointer;
}
.reorder:disabled { opacity: 0.3; cursor: not-allowed; }
.dot { width: 12px; height: 12px; border-radius: 50%; border: none; cursor: pointer; margin-left: 4px; }
.dot--on { background: #4ade80; }
.dot--off { background: #475569; }

.rules__editor { background: var(--card-bg); border: 1px solid var(--border-color); border-radius: 10px; padding: 18px 20px; }
.cond-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 8px; }
.field { display: flex; flex-direction: column; gap: 5px; margin-bottom: 12px; }
.field-row { display: flex; gap: 12px; }
.field-row .field { flex: 1; }
.field__label { font-size: 11px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; color: var(--text-muted); }
.field__sub { font-size: 11px; font-family: monospace; color: var(--text-muted); }
.field__hint { font-size: 12px; color: var(--text-muted); }
.field__input {
  background: rgba(255, 255, 255, 0.05); border: 1px solid var(--border-color);
  border-radius: 7px; padding: 8px 11px; font-size: 13px; color: var(--text-primary);
  font-family: inherit; outline: none; width: 100%;
}
.field__input:focus { border-color: #6366f1; }

.dest-checks { display: flex; flex-direction: column; gap: 6px; }
.dest-check { font-size: 13px; color: var(--text-secondary); display: flex; align-items: center; gap: 8px; }
.dest-type { font-family: monospace; font-size: 11px; color: var(--text-muted); }

.toggle-line { display: flex; align-items: center; gap: 8px; font-size: 13px; color: var(--text-secondary); margin-bottom: 14px; }

.rules__actions { display: flex; align-items: center; gap: 10px; flex-wrap: wrap; }
.rules__note { font-size: 11.5px; color: var(--text-muted); margin: 10px 0 0; }

.btn { padding: 7px 16px; border-radius: 8px; font-size: 13px; font-weight: 600; cursor: pointer; border: none; font-family: inherit; }
.btn--sm { padding: 5px 12px; font-size: 12px; }
.btn--primary { background: #6366f1; color: #fff; }
.btn--primary:hover:not(:disabled) { background: #4f46e5; }
.btn--ghost { background: rgba(255, 255, 255, 0.07); color: var(--text-secondary); border: 1px solid var(--border-color); }
.btn--danger { background: rgba(239, 68, 68, 0.12); color: #f87171; border: 1px solid rgba(239, 68, 68, 0.3); }
.btn--danger:hover:not(:disabled) { background: rgba(239, 68, 68, 0.2); }
.btn:disabled { opacity: 0.5; cursor: not-allowed; }

.feedback { font-size: 12px; }
.feedback--ok { color: #4ade80; }
.feedback--err { color: #f87171; }
</style>
