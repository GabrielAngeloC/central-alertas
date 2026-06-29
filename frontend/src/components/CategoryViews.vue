<template>
  <section class="cat-views">
    <div class="cat-views__head">
      <h2 class="cat-views__title">Visões por categoria</h2>
      <span v-if="lastError" class="cat-views__err">{{ lastError }}</span>
    </div>

    <div class="cat-grid">
      <div v-for="view in views" :key="view.category" class="cat-card">
        <h3 class="cat-card__title">{{ view.title }}</h3>

        <ul v-if="rowsFor(view).length" class="cat-rows">
          <li
            v-for="row in rowsFor(view)"
            :key="row.id"
            class="cat-row"
            @click="$emit('select', row.id)"
          >
            <span class="cat-row__name">{{ row.label }}</span>
            <span class="cat-row__badge" :class="`cat-row__badge--${row.severity}`">
              {{ row.metric }}
              <span v-if="row.isEscalating" title="Escalando">📈</span>
            </span>
          </li>
        </ul>
        <p v-else class="cat-empty">
          <span class="cat-empty__dot" /> tudo ok
        </p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { dashboardApi, ApiError, type DashboardViewResponse, type DashboardViewAlertResponse } from '@/api'

defineEmits<{ (e: 'select', alertId: string): void }>()

const POLL_MS = Number(import.meta.env.VITE_ALERT_POLL_MS ?? 10000)

const views = ref<DashboardViewResponse[]>([])
const lastError = ref<string | null>(null)
let timer: ReturnType<typeof setInterval> | null = null

interface Row {
  id: string
  label: string
  severity: string
  metric: string
  isEscalating: boolean
}

function normalizeSeverity(s: string): string {
  const v = (s || '').toLowerCase()
  return v === 'critical' || v === 'warning' || v === 'info' ? v : 'info'
}

function formatMetric(a: DashboardViewAlertResponse): string {
  if (a.metricValue == null) return `${a.occurrenceCount}×`
  const unit = a.metricUnit ? ` ${a.metricUnit}` : ''
  return `${a.metricValue}${unit}`
}

// Para cada categoria, mostra o alerta ativo mais recente de cada `type`.
function rowsFor(view: DashboardViewResponse): Row[] {
  const byType = new Map<string, DashboardViewAlertResponse>()
  for (const alert of view.alerts) {
    const current = byType.get(alert.type)
    if (!current || alert.lastSeenAt > current.lastSeenAt) {
      byType.set(alert.type, alert)
    }
  }
  return Array.from(byType.values())
    .sort((a, b) => b.lastSeenAt.localeCompare(a.lastSeenAt))
    .map((a) => ({
      id: a.id,
      label: a.title,
      severity: normalizeSeverity(a.severity),
      metric: formatMetric(a),
      isEscalating: a.isEscalating,
    }))
}

async function load() {
  try {
    views.value = await dashboardApi.views()
    lastError.value = null
  } catch (err) {
    if (err instanceof ApiError && err.status === 401) return
    lastError.value = err instanceof ApiError ? err.message : 'Falha ao carregar visões.'
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
.cat-views { margin-bottom: 28px; }
.cat-views__head {
  display: flex;
  align-items: baseline;
  gap: 12px;
  margin-bottom: 14px;
}
.cat-views__title {
  font-size: 16px;
  font-weight: 600;
  color: var(--text-primary);
  margin: 0;
}
.cat-views__err { font-size: 12px; color: #f87171; }

.cat-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 14px;
}
.cat-card {
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 12px;
  padding: 16px 18px;
}
.cat-card__title {
  font-size: 11px;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--text-secondary);
  margin: 0 0 12px;
  font-weight: 600;
}
.cat-rows { list-style: none; margin: 0; padding: 0; }
.cat-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
  padding: 8px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  cursor: pointer;
}
.cat-row:last-child { border-bottom: none; }
.cat-row:hover .cat-row__name { color: var(--text-primary); }
.cat-row__name {
  font-size: 13px;
  color: var(--text-secondary);
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.cat-row__badge {
  font-family: monospace;
  font-size: 11.5px;
  padding: 2px 9px;
  border-radius: 10px;
  white-space: nowrap;
  flex-shrink: 0;
}
.cat-row__badge--critical { color: #ef4444; background: rgba(239, 68, 68, 0.12); }
.cat-row__badge--warning { color: #f59e0b; background: rgba(245, 158, 11, 0.12); }
.cat-row__badge--info { color: #60a5fa; background: rgba(96, 165, 250, 0.14); }

.cat-empty {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: var(--text-muted);
  margin: 4px 0 0;
}
.cat-empty__dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #4ade80;
}
</style>
