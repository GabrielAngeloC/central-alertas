<template>
  <div class="hub">
    <header class="hub__header">
      <h1 class="hub__title">Saúde do hub</h1>
      <p class="hub__subtitle">Entregas de notificação e heartbeats das fontes. O hub não pode falhar em silêncio.</p>
    </header>

    <p v-if="error" class="hub__err">{{ error }}</p>

    <!-- Entregas (últimas 24h) -->
    <section class="card">
      <h2 class="card__title">Entregas · últimas {{ health?.windowHours ?? 24 }}h</h2>
      <div class="kpis" v-if="health">
        <div class="kpi kpi--ok"><span class="kpi__v">{{ health.successCount }}</span><span class="kpi__l">Sucesso</span></div>
        <div class="kpi kpi--fail"><span class="kpi__v">{{ health.failedCount }}</span><span class="kpi__l">Falha</span></div>
        <div class="kpi kpi--skip"><span class="kpi__v">{{ health.skippedCount }}</span><span class="kpi__l">Throttle</span></div>
        <div class="kpi"><span class="kpi__v">{{ successRate }}</span><span class="kpi__l">Taxa de sucesso</span></div>
      </div>

      <table class="channels" v-if="health && health.byChannel.length">
        <thead><tr><th>Canal</th><th>Sucesso</th><th>Falha</th><th>Throttle</th></tr></thead>
        <tbody>
          <tr v-for="c in health.byChannel" :key="c.channel">
            <td>{{ c.channel }}</td>
            <td class="ok">{{ c.success }}</td>
            <td class="fail">{{ c.failed }}</td>
            <td class="skip">{{ c.skipped }}</td>
          </tr>
        </tbody>
      </table>
      <p v-else-if="health" class="card__empty">Nenhuma entrega registrada nas últimas 24h.</p>
    </section>

    <!-- Fontes & heartbeats -->
    <section class="card">
      <h2 class="card__title">Fontes &amp; heartbeats</h2>
      <ul class="sources" v-if="sources.length">
        <li v-for="s in sources" :key="s.id" class="source" :class="{ 'source--off': !s.isActive }">
          <div class="source__info">
            <span class="source__name">{{ s.name }}</span>
            <span class="source__sub">intervalo {{ s.expectedIntervalMinutes }} min · últ. {{ formatTime(s.lastReceivedAt) }}</span>
          </div>
          <span class="source__status" :class="`source__status--${statusOf(s)}`">{{ statusLabel(s) }}</span>
        </li>
      </ul>
      <p v-else class="card__empty">Nenhuma fonte cadastrada.</p>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { dashboardApi, sourcesApi, ApiError, type HubHealthResponse, type SourceHealthResponse } from '@/api'

const POLL_MS = Number(import.meta.env.VITE_ALERT_POLL_MS ?? 10000)

const health = ref<HubHealthResponse | null>(null)
const sources = ref<SourceHealthResponse[]>([])
const error = ref<string | null>(null)
let timer: ReturnType<typeof setInterval> | null = null

const successRate = computed(() => {
  if (!health.value) return '—'
  const considered = health.value.successCount + health.value.failedCount
  if (considered === 0) return '—'
  return `${Math.round((health.value.successCount / considered) * 100)}%`
})

function statusOf(s: SourceHealthResponse): string {
  if (!s.isActive) return 'off'
  return s.isSilent ? 'silent' : 'healthy'
}
function statusLabel(s: SourceHealthResponse): string {
  const map: Record<string, string> = { healthy: 'saudável', silent: 'silenciosa', off: 'inativa' }
  return map[statusOf(s)] ?? '—'
}
function formatTime(iso?: string | null): string {
  if (!iso) return 'nunca'
  return new Intl.DateTimeFormat('pt-BR', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' }).format(new Date(iso))
}

async function load() {
  error.value = null
  try {
    const [h, s] = await Promise.all([
      dashboardApi.hubHealth(),
      sourcesApi.health().catch(() => [] as SourceHealthResponse[]),
    ])
    health.value = h
    sources.value = s
  } catch (err) {
    if (err instanceof ApiError && err.status === 401) return
    error.value = err instanceof ApiError ? err.message : 'Falha ao carregar a saúde do hub.'
  }
}

onMounted(() => {
  load()
  timer = setInterval(load, POLL_MS)
})
onUnmounted(() => {
  if (timer) clearInterval(timer)
})
</script>

<style scoped>
.hub { padding: 32px 32px 60px; max-width: 900px; margin: 0 auto; }
.hub__header { margin-bottom: 20px; }
.hub__title { font-size: 24px; font-weight: 700; color: var(--text-primary); margin: 0 0 6px; }
.hub__subtitle { font-size: 14px; color: var(--text-muted); margin: 0; }
.hub__err { color: #f87171; font-size: 13px; }

.card { background: var(--card-bg); border: 1px solid var(--border-color); border-radius: 12px; padding: 20px 22px; margin-bottom: 16px; }
.card__title { font-size: 12px; letter-spacing: 0.08em; text-transform: uppercase; color: var(--text-secondary); margin: 0 0 16px; font-weight: 600; }
.card__empty { font-size: 13px; color: var(--text-muted); margin: 0; }

.kpis { display: grid; grid-template-columns: repeat(auto-fit, minmax(120px, 1fr)); gap: 12px; margin-bottom: 16px; }
.kpi { background: rgba(255, 255, 255, 0.03); border: 1px solid var(--border-color); border-radius: 8px; padding: 12px 14px; display: flex; flex-direction: column; gap: 4px; }
.kpi__v { font-size: 24px; font-weight: 700; font-variant-numeric: tabular-nums; }
.kpi__l { font-size: 10.5px; text-transform: uppercase; letter-spacing: 0.05em; color: var(--text-muted); }
.kpi--ok .kpi__v { color: #4ade80; }
.kpi--fail .kpi__v { color: #f87171; }
.kpi--skip .kpi__v { color: #94a3b8; }

.channels { width: 100%; border-collapse: collapse; font-size: 13px; }
.channels th { text-align: left; font-family: monospace; font-size: 10.5px; text-transform: uppercase; letter-spacing: 0.06em; color: var(--text-muted); padding: 8px 10px; border-bottom: 1px solid var(--border-color); }
.channels td { padding: 9px 10px; border-bottom: 1px solid rgba(255, 255, 255, 0.05); color: var(--text-secondary); }
.channels td.ok { color: #4ade80; }
.channels td.fail { color: #f87171; }
.channels td.skip { color: #94a3b8; }
.channels tr:last-child td { border-bottom: none; }

.sources { list-style: none; margin: 0; padding: 0; display: flex; flex-direction: column; gap: 8px; }
.source { display: flex; align-items: center; justify-content: space-between; gap: 12px; padding: 10px 12px; background: rgba(255, 255, 255, 0.03); border: 1px solid var(--border-color); border-radius: 8px; }
.source--off { opacity: 0.55; }
.source__name { font-size: 13px; color: var(--text-primary); font-weight: 600; }
.source__sub { display: block; font-size: 11.5px; color: var(--text-muted); margin-top: 2px; }
.source__status { font-family: monospace; font-size: 11px; padding: 2px 9px; border-radius: 10px; white-space: nowrap; }
.source__status--healthy { color: #4ade80; background: rgba(76, 175, 141, 0.14); }
.source__status--silent { color: #f59e0b; background: rgba(245, 158, 11, 0.14); }
.source__status--off { color: #94a3b8; background: rgba(148, 163, 184, 0.12); }
</style>
