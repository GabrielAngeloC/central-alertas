<template>
  <div class="sm">
    <div class="sm__head">
      <h3 class="sm__h">Fontes (sources)</h3>
      <div class="sm__head-actions">
        <button class="btn btn--ghost btn--sm" @click="reload">Recarregar</button>
        <button class="btn btn--primary btn--sm" @click="startNew">+ Nova fonte</button>
      </div>
    </div>
    <p v-if="loadError" class="feedback feedback--err">{{ loadError }}</p>

    <div class="sm__grid">
      <!-- Lista com saúde -->
      <ul class="sm-list">
        <li
          v-for="s in sources"
          :key="s.id"
          class="sm-item"
          :class="{ 'sm-item--sel': form?.id === s.id, 'sm-item--off': !s.isActive }"
          @click="select(s)"
        >
          <div class="sm-item__info">
            <div class="sm-item__name">{{ s.name }}</div>
            <div class="sm-item__sub">intervalo {{ s.expectedIntervalMinutes }} min · últ. {{ formatTime(s.lastReceivedAt) }}</div>
          </div>
          <span class="sm-status" :class="`sm-status--${healthOf(s.id)}`">{{ healthLabel(s.id) }}</span>
        </li>
      </ul>

      <!-- Editor -->
      <div class="sm__editor" v-if="form">
        <div class="field">
          <label class="field__label">Nome (source)</label>
          <input class="field__input" v-model="form.name" placeholder="validador-estoque" />
        </div>
        <div class="field">
          <label class="field__label">Intervalo esperado (min)</label>
          <input class="field__input" type="number" v-model.number="form.expectedIntervalMinutes" placeholder="60" />
        </div>
        <label class="toggle-line" v-if="form.id"><input type="checkbox" v-model="form.isActive" @change="toggleActive" /> Ativa</label>
        <div class="sm__actions">
          <button class="btn btn--primary" @click="save" :disabled="saving">{{ saving ? 'Salvando…' : 'Salvar' }}</button>
          <button v-if="form.id" class="btn btn--danger" @click="remove" :disabled="removing">{{ removing ? 'Removendo…' : 'Remover' }}</button>
          <span v-if="feedback" class="feedback" :class="feedback.ok ? 'feedback--ok' : 'feedback--err'">{{ feedback.msg }}</span>
        </div>
        <p class="sm__note">O heartbeat sinaliza a fonte como silenciosa quando passa do intervalo sem enviar nada.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { sourcesApi, ApiError, type SourceResponse, type SourceHealthResponse } from '@/api'

interface SourceForm {
  id: string | null
  name: string
  expectedIntervalMinutes: number
  isActive: boolean
}

const sources = ref<SourceResponse[]>([])
const health = ref<Record<string, SourceHealthResponse>>({})
const form = ref<SourceForm | null>(null)
const saving = ref(false)
const removing = ref(false)
const loadError = ref<string | null>(null)
const feedback = ref<{ ok: boolean; msg: string } | null>(null)

function healthOf(id: string): string {
  const h = health.value[id]
  if (!h) return 'unknown'
  if (!h.isActive) return 'off'
  return h.isSilent ? 'silent' : 'healthy'
}
function healthLabel(id: string): string {
  return { healthy: 'saudável', silent: 'silenciosa', off: 'inativa', unknown: '—' }[healthOf(id)] ?? '—'
}
function formatTime(iso?: string | null): string {
  if (!iso) return 'nunca'
  return new Intl.DateTimeFormat('pt-BR', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' }).format(new Date(iso))
}

function select(s: SourceResponse) {
  form.value = { id: s.id, name: s.name, expectedIntervalMinutes: s.expectedIntervalMinutes, isActive: s.isActive }
  feedback.value = null
}
function startNew() {
  form.value = { id: null, name: '', expectedIntervalMinutes: 60, isActive: true }
  feedback.value = null
}

async function load() {
  loadError.value = null
  try {
    const [list, healthList] = await Promise.all([
      sourcesApi.list(),
      sourcesApi.health().catch(() => [] as SourceHealthResponse[]),
    ])
    sources.value = list
    health.value = Object.fromEntries(healthList.map((h) => [h.id, h]))
  } catch (err) {
    if (err instanceof ApiError && err.status === 401) return
    loadError.value = err instanceof ApiError ? err.message : 'Falha ao carregar fontes.'
  }
}
const reload = load

async function save() {
  if (!form.value) return
  saving.value = true
  feedback.value = null
  try {
    const body = { name: form.value.name.trim(), expectedIntervalMinutes: form.value.expectedIntervalMinutes }
    if (form.value.id) await sourcesApi.update(form.value.id, body)
    else await sourcesApi.create(body)
    feedback.value = { ok: true, msg: 'Fonte salva.' }
    await load()
  } catch (err) {
    feedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao salvar.' }
  } finally {
    saving.value = false
  }
}

async function remove() {
  if (!form.value?.id) return
  if (!window.confirm('Remover esta fonte?')) return
  removing.value = true
  feedback.value = null
  try {
    await sourcesApi.remove(form.value.id)
    form.value = null
    await load()
  } catch (err) {
    feedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao remover.' }
  } finally {
    removing.value = false
  }
}

async function toggleActive() {
  if (!form.value?.id) return
  try {
    if (form.value.isActive) await sourcesApi.activate(form.value.id)
    else await sourcesApi.deactivate(form.value.id)
    await load()
  } catch { await load() }
}

onMounted(load)
</script>

<style scoped>
.sm__head { display: flex; align-items: center; justify-content: space-between; margin-bottom: 14px; }
.sm__head-actions { display: flex; gap: 8px; }
.sm__h { font-size: 14px; font-weight: 600; color: var(--text-primary); margin: 0; }
.sm__grid { display: grid; grid-template-columns: 1fr 1fr; gap: 18px; }
@media (max-width: 820px) { .sm__grid { grid-template-columns: 1fr; } }

.sm-list { list-style: none; margin: 0; padding: 0; display: flex; flex-direction: column; gap: 8px; }
.sm-item {
  display: flex; align-items: center; justify-content: space-between; gap: 10px;
  background: var(--card-bg); border: 1px solid var(--border-color); border-radius: 8px;
  padding: 10px 12px; cursor: pointer;
}
.sm-item--sel { border-color: rgba(99, 102, 241, 0.55); }
.sm-item--off { opacity: 0.55; }
.sm-item__name { font-size: 13px; color: var(--text-primary); font-weight: 600; }
.sm-item__sub { font-size: 11.5px; color: var(--text-muted); margin-top: 2px; }
.sm-status { font-family: monospace; font-size: 11px; padding: 2px 8px; border-radius: 10px; white-space: nowrap; }
.sm-status--healthy { color: #4ade80; background: rgba(76, 175, 141, 0.14); }
.sm-status--silent { color: #f59e0b; background: rgba(245, 158, 11, 0.14); }
.sm-status--off { color: #94a3b8; background: rgba(148, 163, 184, 0.12); }
.sm-status--unknown { color: var(--text-muted); }

.sm__editor { background: var(--card-bg); border: 1px solid var(--border-color); border-radius: 10px; padding: 18px 20px; }
.field { display: flex; flex-direction: column; gap: 5px; margin-bottom: 12px; }
.field__label { font-size: 11px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; color: var(--text-muted); }
.field__input {
  background: rgba(255, 255, 255, 0.05); border: 1px solid var(--border-color); border-radius: 7px;
  padding: 8px 11px; font-size: 13px; color: var(--text-primary); font-family: inherit; outline: none; width: 100%;
}
.field__input:focus { border-color: #6366f1; }
.toggle-line { display: flex; align-items: center; gap: 8px; font-size: 13px; color: var(--text-secondary); margin-bottom: 14px; }
.sm__actions { display: flex; align-items: center; gap: 10px; flex-wrap: wrap; }
.sm__note { font-size: 11.5px; color: var(--text-muted); margin: 10px 0 0; }

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
