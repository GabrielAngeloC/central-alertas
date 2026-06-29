<template>
  <div class="search">
    <header class="search__header">
      <h1 class="search__title">Busca por item</h1>
      <p class="search__subtitle">Encontre os alertas em que um pedido, SKU ou termo apareceu.</p>
    </header>

    <form class="search__form" @submit.prevent="run">
      <input
        v-model="query"
        class="search__input"
        placeholder="Ex.: ELET-1042, PED-88201, CD-Extrema…"
        autofocus
      />
      <button class="btn btn--primary" type="submit" :disabled="loading || !query.trim()">
        {{ loading ? 'Buscando…' : 'Buscar' }}
      </button>
    </form>

    <p v-if="error" class="search__err">{{ error }}</p>

    <p v-if="searched && !loading" class="search__count">
      {{ results.length }} alerta(s) encontrado(s)
    </p>

    <ul class="results" v-if="results.length">
      <li
        v-for="a in results"
        :key="a.id"
        class="result"
        :class="`result--${sev(a.severity)}`"
        @click="selectedId = a.id"
      >
        <span class="result__sev" :class="`result__sev--${sev(a.severity)}`">{{ sevLabel(a.severity) }}</span>
        <div class="result__main">
          <span class="result__title">{{ a.title }}</span>
          <span class="result__meta">
            <code>{{ a.source }}</code> · <code>{{ a.category }}</code> · <code>{{ a.type }}</code>
            <template v-if="a.metricValue != null"> · <strong>{{ a.metricValue }} {{ a.metricUnit }}</strong></template>
          </span>
        </div>
        <span class="result__occ">{{ a.occurrenceCount }}×</span>
      </li>
    </ul>

    <AlertDetailDrawer :alert-id="selectedId" @close="selectedId = null" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { alertsApi, ApiError, type AlertSummaryResponse } from '@/api'
import AlertDetailDrawer from '@/components/AlertDetailDrawer.vue'

const query = ref('')
const results = ref<AlertSummaryResponse[]>([])
const loading = ref(false)
const searched = ref(false)
const error = ref<string | null>(null)
const selectedId = ref<string | null>(null)

function sev(s: string): string {
  const v = (s || '').toLowerCase()
  return v === 'critical' || v === 'warning' || v === 'info' ? v : 'info'
}
function sevLabel(s: string): string {
  return { critical: 'CRÍT', warning: 'ATEN', info: 'INFO' }[sev(s)] ?? 'INFO'
}

async function run() {
  if (!query.value.trim()) return
  loading.value = true
  error.value = null
  try {
    results.value = await alertsApi.search(query.value.trim())
    searched.value = true
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Falha na busca.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.search { padding: 32px 32px 60px; max-width: 900px; margin: 0 auto; }
.search__header { margin-bottom: 20px; }
.search__title { font-size: 24px; font-weight: 700; color: var(--text-primary); margin: 0 0 6px; }
.search__subtitle { font-size: 14px; color: var(--text-muted); margin: 0; }

.search__form { display: flex; gap: 10px; margin-bottom: 18px; }
.search__input {
  flex: 1;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 10px 14px;
  font-size: 14px;
  color: var(--text-primary);
  font-family: inherit;
  outline: none;
}
.search__input:focus { border-color: #6366f1; }

.search__err { color: #f87171; font-size: 13px; }
.search__count { font-size: 13px; color: var(--text-muted); margin-bottom: 12px; }

.results { list-style: none; margin: 0; padding: 0; display: flex; flex-direction: column; gap: 8px; }
.result {
  display: grid;
  grid-template-columns: 56px 1fr auto;
  align-items: center;
  gap: 14px;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-left: 3px solid transparent;
  border-radius: 8px;
  padding: 12px 16px;
  cursor: pointer;
}
.result--critical { border-left-color: #ef4444; }
.result--warning { border-left-color: #f59e0b; }
.result--info { border-left-color: #60a5fa; }
.result__sev { font-family: monospace; font-size: 10px; font-weight: 700; padding: 3px 6px; border-radius: 4px; text-align: center; }
.result__sev--critical { color: #ef4444; background: rgba(239, 68, 68, 0.14); }
.result__sev--warning { color: #f59e0b; background: rgba(245, 158, 11, 0.14); }
.result__sev--info { color: #60a5fa; background: rgba(96, 165, 250, 0.16); }
.result__main { min-width: 0; }
.result__title { display: block; font-size: 14px; color: var(--text-primary); font-weight: 600; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.result__meta { font-size: 12px; color: var(--text-muted); }
.result__meta code { font-family: monospace; color: var(--text-secondary); }
.result__occ { font-family: monospace; font-size: 12px; color: var(--text-muted); }

.btn { padding: 9px 18px; border-radius: 8px; font-size: 13px; font-weight: 600; cursor: pointer; border: none; font-family: inherit; }
.btn--primary { background: #6366f1; color: #fff; }
.btn--primary:hover:not(:disabled) { background: #4f46e5; }
.btn:disabled { opacity: 0.5; cursor: not-allowed; }
</style>
