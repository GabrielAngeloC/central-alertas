<template>
  <div class="dm">
    <div class="dm__head">
      <h3 class="dm__h">Destinos de notificação</h3>
      <button class="btn btn--primary btn--sm" @click="startNew">+ Novo destino</button>
    </div>
    <p v-if="loadError" class="feedback feedback--err">{{ loadError }}</p>

    <div class="dm__grid">
      <!-- Lista -->
      <ul class="dm-list">
        <li
          v-for="d in destinations"
          :key="d.id"
          class="dm-item"
          :class="{ 'dm-item--sel': form?.id === d.id, 'dm-item--off': !d.isActive }"
          @click="select(d)"
        >
          <div>
            <div class="dm-item__name">{{ d.name }}</div>
            <div class="dm-item__type">{{ d.type }} · {{ configSummary(d) }}</div>
          </div>
          <button class="dot" :class="d.isActive ? 'dot--on' : 'dot--off'" @click.stop="toggle(d)" />
        </li>
      </ul>

      <!-- Editor -->
      <div class="dm__editor" v-if="form">
        <div class="field">
          <label class="field__label">Nome</label>
          <input class="field__input" v-model="form.name" placeholder="Ex.: Telegram Operação" />
        </div>
        <div class="field">
          <label class="field__label">Tipo</label>
          <select class="field__input" v-model="form.type">
            <option value="telegram">telegram</option>
            <option value="email">email</option>
          </select>
        </div>
        <div class="field" v-if="form.type === 'telegram'">
          <label class="field__label">Chat ID</label>
          <input class="field__input" v-model="form.chatId" placeholder="-100123456789" />
        </div>
        <div class="field" v-else>
          <label class="field__label">Destinatários <span class="field__hint">(vírgula)</span></label>
          <input class="field__input" v-model="form.recipients" placeholder="ti@x.com, op@x.com" />
        </div>
        <label class="toggle-line"><input type="checkbox" v-model="form.isActive" /> Ativo</label>

        <div class="dm__actions">
          <button class="btn btn--primary" @click="save" :disabled="saving">{{ saving ? 'Salvando…' : 'Salvar' }}</button>
          <button class="btn btn--ghost" @click="test" :disabled="testing || !form.id">{{ testing ? 'Testando…' : 'Enviar teste' }}</button>
          <button v-if="form.id" class="btn btn--danger" @click="remove" :disabled="removing">{{ removing ? 'Removendo…' : 'Remover' }}</button>
          <span v-if="feedback" class="feedback" :class="feedback.ok ? 'feedback--ok' : 'feedback--err'">{{ feedback.msg }}</span>
        </div>
        <p class="dm__note">Bot token (Telegram) e SMTP (e-mail) são configurados no servidor.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { destinationsApi, ApiError, type NotificationDestinationResponse } from '@/api'

interface DestForm {
  id: string | null
  name: string
  type: string
  chatId: string
  recipients: string
  isActive: boolean
}

const destinations = ref<NotificationDestinationResponse[]>([])
const form = ref<DestForm | null>(null)
const saving = ref(false)
const testing = ref(false)
const removing = ref(false)
const loadError = ref<string | null>(null)
const feedback = ref<{ ok: boolean; msg: string } | null>(null)

function parseConfig(json: string): Record<string, unknown> {
  try { return JSON.parse(json || '{}') } catch { return {} }
}

function configSummary(d: NotificationDestinationResponse): string {
  const cfg = parseConfig(d.configurationJson)
  if (d.type === 'telegram') return `chat ${cfg.chatId ?? '?'}`
  const r = cfg.recipients
  return Array.isArray(r) ? `${r.length} dest.` : '0 dest.'
}

function select(d: NotificationDestinationResponse) {
  const cfg = parseConfig(d.configurationJson)
  const recipients = Array.isArray(cfg.recipients) ? (cfg.recipients as string[]).join(', ') : ''
  form.value = {
    id: d.id,
    name: d.name,
    type: d.type,
    chatId: (cfg.chatId as string) ?? '',
    recipients,
    isActive: d.isActive,
  }
  feedback.value = null
}

function startNew() {
  form.value = { id: null, name: '', type: 'telegram', chatId: '', recipients: '', isActive: true }
  feedback.value = null
}

function buildConfig(f: DestForm): Record<string, unknown> {
  if (f.type === 'telegram') return { chatId: f.chatId.trim() }
  return { recipients: f.recipients.split(',').map((r) => r.trim()).filter(Boolean) }
}

async function load() {
  loadError.value = null
  try {
    destinations.value = await destinationsApi.list()
  } catch (err) {
    if (err instanceof ApiError && err.status === 401) return
    loadError.value = err instanceof ApiError ? err.message : 'Falha ao carregar destinos.'
  }
}

async function save() {
  if (!form.value) return
  saving.value = true
  feedback.value = null
  try {
    const cfg = buildConfig(form.value)
    if (form.value.id) {
      await destinationsApi.update(form.value.id, {
        name: form.value.name.trim(),
        type: form.value.type,
        configuration: cfg,
        isActive: form.value.isActive,
      })
    } else {
      const created = await destinationsApi.create({
        name: form.value.name.trim(),
        type: form.value.type,
        configuration: cfg,
      })
      if (!form.value.isActive) await destinationsApi.deactivate(created.id)
    }
    feedback.value = { ok: true, msg: 'Destino salvo.' }
    await load()
  } catch (err) {
    feedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao salvar.' }
  } finally {
    saving.value = false
  }
}

async function toggle(d: NotificationDestinationResponse) {
  try {
    if (d.isActive) await destinationsApi.deactivate(d.id)
    else await destinationsApi.activate(d.id)
    await load()
  } catch { await load() }
}

async function remove() {
  if (!form.value?.id) return
  if (!window.confirm('Remover este destino?')) return
  removing.value = true
  feedback.value = null
  try {
    await destinationsApi.remove(form.value.id)
    form.value = null
    await load()
  } catch (err) {
    feedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha ao remover.' }
  } finally {
    removing.value = false
  }
}

async function test() {
  if (!form.value?.id) return
  testing.value = true
  feedback.value = null
  try {
    const res = await destinationsApi.test(form.value.id)
    feedback.value = { ok: res.success, msg: res.success ? res.message : res.error || res.message }
  } catch (err) {
    feedback.value = { ok: false, msg: err instanceof ApiError ? err.message : 'Falha no teste.' }
  } finally {
    testing.value = false
  }
}

onMounted(load)
</script>

<style scoped>
.dm__head { display: flex; align-items: center; justify-content: space-between; margin-bottom: 14px; }
.dm__h { font-size: 14px; font-weight: 600; color: var(--text-primary); margin: 0; }
.dm__grid { display: grid; grid-template-columns: 1fr 1fr; gap: 18px; }
@media (max-width: 820px) { .dm__grid { grid-template-columns: 1fr; } }

.dm-list { list-style: none; margin: 0; padding: 0; display: flex; flex-direction: column; gap: 8px; }
.dm-item {
  display: flex; align-items: center; justify-content: space-between; gap: 10px;
  background: var(--card-bg); border: 1px solid var(--border-color); border-radius: 8px;
  padding: 10px 12px; cursor: pointer;
}
.dm-item--sel { border-color: rgba(99, 102, 241, 0.55); }
.dm-item--off { opacity: 0.55; }
.dm-item__name { font-size: 13px; color: var(--text-primary); font-weight: 600; }
.dm-item__type { font-size: 11.5px; color: var(--text-muted); font-family: monospace; margin-top: 2px; }
.dot { width: 12px; height: 12px; border-radius: 50%; border: none; cursor: pointer; }
.dot--on { background: #4ade80; }
.dot--off { background: #475569; }

.dm__editor { background: var(--card-bg); border: 1px solid var(--border-color); border-radius: 10px; padding: 18px 20px; }
.field { display: flex; flex-direction: column; gap: 5px; margin-bottom: 12px; }
.field__label { font-size: 11px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; color: var(--text-muted); }
.field__hint { font-weight: 400; text-transform: none; }
.field__input {
  background: rgba(255, 255, 255, 0.05); border: 1px solid var(--border-color); border-radius: 7px;
  padding: 8px 11px; font-size: 13px; color: var(--text-primary); font-family: inherit; outline: none; width: 100%;
}
.field__input:focus { border-color: #6366f1; }
.toggle-line { display: flex; align-items: center; gap: 8px; font-size: 13px; color: var(--text-secondary); margin-bottom: 14px; }
.dm__actions { display: flex; align-items: center; gap: 10px; flex-wrap: wrap; }
.dm__note { font-size: 11.5px; color: var(--text-muted); margin: 10px 0 0; }

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
