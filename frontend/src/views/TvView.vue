<template>
  <div class="tv">
    <!-- Header -->
    <header class="tv__header">
      <div class="tv__brand">

        <span class="tv__title">Central de Alertas</span>
      </div>
      <div class="tv__header-right">
        <span class="tv__feed-dot" :class="feedDotClass" title="Feed SSE" />
        <span class="tv__clock">{{ clock }}</span>
      </div>
    </header>

    <!-- Severity counters -->
    <section class="tv__counters">
      <div class="tv__counter tv__counter--critical">
        <span class="tv__counter-num">{{ store.countBySeverity.critical }}</span>
        <span class="tv__counter-label">Críticos</span>
      </div>
      <div class="tv__counter tv__counter--warning">
        <span class="tv__counter-num">{{ store.countBySeverity.warning }}</span>
        <span class="tv__counter-label">Atenção</span>
      </div>
      <div class="tv__counter tv__counter--info">
        <span class="tv__counter-num">{{ store.countBySeverity.info }}</span>
        <span class="tv__counter-label">Info</span>
      </div>
      <div class="tv__counter tv__counter--total">
        <span class="tv__counter-num">{{ store.alerts.length }}</span>
        <span class="tv__counter-label">Total</span>
      </div>
    </section>

    <!-- Main content: category bars + recent alerts -->
    <div class="tv__body">
      <!-- Category bars -->
      <section class="tv__categories">
        <h2 class="tv__section-title">Por Categoria</h2>
        <div class="tv__bars">
          <div
            v-for="cat in categoryStats"
            :key="cat.name"
            class="tv__bar-row"
          >
            <span class="tv__bar-label">{{ cat.name }}</span>
            <div class="tv__bar-track">
              <div
                class="tv__bar-fill tv__bar-fill--critical"
                :style="{ width: pct(cat.critical) }"
              />
              <div
                class="tv__bar-fill tv__bar-fill--warning"
                :style="{ width: pct(cat.warning) }"
              />
              <div
                class="tv__bar-fill tv__bar-fill--info"
                :style="{ width: pct(cat.info) }"
              />
            </div>
            <span class="tv__bar-count">{{ cat.total }}</span>
          </div>
        </div>

        <!-- Legend -->
        <div class="tv__legend">
          <span class="tv__legend-item tv__legend-item--critical">Crítico</span>
          <span class="tv__legend-item tv__legend-item--warning">Atenção</span>
          <span class="tv__legend-item tv__legend-item--info">Info</span>
        </div>
      </section>

      <!-- Recent alerts -->
      <section class="tv__recent">
        <h2 class="tv__section-title">Alertas Recentes</h2>
        <TransitionGroup name="tv-alert" tag="ul" class="tv__alert-list">
          <li
            v-for="alert in recentAlerts"
            :key="alert.id"
            class="tv__alert-item"
            :class="`tv__alert-item--${alert.severity}`"
          >
            <span class="tv__alert-sev" :class="`tv__alert-sev--${alert.severity}`">
              {{ severityLabel(alert.severity) }}
            </span>
            <div class="tv__alert-info">
              <span class="tv__alert-title">{{ alert.title }}</span>
              <span class="tv__alert-meta">
                <code>{{ alert.source }}</code> · <code>{{ alert.category }}</code>
                <template v-if="alert.metric">
                  · <strong>{{ alert.metric.value }} {{ alert.metric.unit }}</strong>
                </template>
              </span>
            </div>
            <span class="tv__alert-time">{{ formatTime(alert.received_at) }}</span>
          </li>
        </TransitionGroup>
      </section>
    </div>

    <!-- Back link -->
    <RouterLink to="/" class="tv__back">← Dashboard</RouterLink>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useAlertsStore } from '@/stores/alerts'
import type { Severity } from '@/types/alert'

// Dados vêm do polling do backend (useAlertFeed em App.vue) via store.
const store = useAlertsStore()

const RECENT_MAX = 12

const recentAlerts = computed(() =>
  [...store.alerts]
    .sort((a, b) => (b.received_at ?? '').localeCompare(a.received_at ?? ''))
    .slice(0, RECENT_MAX),
)

const categoryStats = computed(() => {
  const map = new Map<string, { critical: number; warning: number; info: number; total: number }>()

  for (const alert of store.alerts) {
    if (!map.has(alert.category)) {
      map.set(alert.category, { critical: 0, warning: 0, info: 0, total: 0 })
    }
    const entry = map.get(alert.category)!
    entry[alert.severity]++
    entry.total++
  }

  return Array.from(map.entries())
    .map(([name, counts]) => ({ name, ...counts }))
    .sort((a, b) => b.total - a.total)
})

const maxCategoryTotal = computed(() =>
  Math.max(1, ...categoryStats.value.map((c) => c.total)),
)

function pct(count: number) {
  return `${(count / maxCategoryTotal.value) * 100}%`
}

function severityLabel(sev: Severity) {
  return { critical: 'CRÍTICO', warning: 'ATENÇÃO', info: 'INFO' }[sev]
}

function formatTime(iso?: string) {
  if (!iso) return ''
  return new Intl.DateTimeFormat('pt-BR', { hour: '2-digit', minute: '2-digit', second: '2-digit' }).format(
    new Date(iso),
  )
}

// Clock
const clock = ref('')
let clockTimer: ReturnType<typeof setInterval>

function updateClock() {
  clock.value = new Intl.DateTimeFormat('pt-BR', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  }).format(new Date())
}

// Feed dot blink
const feedDotClass = ref('tv__feed-dot--idle')

function flashFeedDot() {
  feedDotClass.value = 'tv__feed-dot--active'
  setTimeout(() => (feedDotClass.value = 'tv__feed-dot--idle'), 800)
}

// Watch for new alerts to flash dot
let prevCount = store.alerts.length
let watchTimer: ReturnType<typeof setInterval>

onMounted(() => {
  updateClock()
  clockTimer = setInterval(updateClock, 1000)
  watchTimer = setInterval(() => {
    if (store.alerts.length !== prevCount) {
      prevCount = store.alerts.length
      flashFeedDot()
    }
  }, 200)
})

onUnmounted(() => {
  clearInterval(clockTimer)
  clearInterval(watchTimer)
})
</script>

<style scoped>
.tv {
  min-height: 100vh;
  background: #080c14;
  color: #e2e8f0;
  display: flex;
  flex-direction: column;
  font-family: 'Inter', 'Segoe UI', system-ui, sans-serif;
  -webkit-font-smoothing: antialiased;
}

/* Header */
.tv__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 18px 40px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.07);
  background: #0d1220;
}
.tv__brand { display: flex; align-items: center; gap: 12px; }
.tv__bell { font-size: 28px; }
.tv__title { font-size: 24px; font-weight: 700; letter-spacing: -0.02em; }
.tv__header-right { display: flex; align-items: center; gap: 16px; }

.tv__feed-dot {
  width: 10px; height: 10px;
  border-radius: 50%;
  transition: background 0.3s;
}
.tv__feed-dot--idle   { background: #334155; }
.tv__feed-dot--active { background: #22c55e; box-shadow: 0 0 6px #22c55e; }

.tv__clock {
  font-size: 28px;
  font-weight: 300;
  font-variant-numeric: tabular-nums;
  letter-spacing: 0.04em;
  color: #94a3b8;
}

/* Counters */
.tv__counters {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.07);
}
.tv__counter {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 32px 20px;
  border-right: 1px solid rgba(255, 255, 255, 0.07);
}
.tv__counter:last-child { border-right: none; }

.tv__counter-num {
  font-size: 72px;
  font-weight: 800;
  line-height: 1;
  font-variant-numeric: tabular-nums;
}
.tv__counter-label {
  font-size: 14px;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  margin-top: 8px;
  opacity: 0.6;
}

.tv__counter--critical .tv__counter-num { color: #ef4444; }
.tv__counter--warning  .tv__counter-num { color: #f59e0b; }
.tv__counter--info     .tv__counter-num { color: #60a5fa; }
.tv__counter--total    .tv__counter-num { color: #e2e8f0; }

/* Body */
.tv__body {
  display: grid;
  grid-template-columns: 1fr 1.6fr;
  flex: 1;
  min-height: 0;
}

.tv__section-title {
  font-size: 12px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: #475569;
  margin-bottom: 24px;
}

/* Category bars */
.tv__categories {
  padding: 32px 40px;
  border-right: 1px solid rgba(255, 255, 255, 0.07);
  display: flex;
  flex-direction: column;
}

.tv__bars { display: flex; flex-direction: column; gap: 18px; flex: 1; }

.tv__bar-row {
  display: grid;
  grid-template-columns: 130px 1fr 36px;
  align-items: center;
  gap: 16px;
}
.tv__bar-label {
  font-size: 14px;
  font-family: monospace;
  color: #94a3b8;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.tv__bar-track {
  height: 18px;
  border-radius: 4px;
  background: rgba(255, 255, 255, 0.05);
  display: flex;
  overflow: hidden;
}
.tv__bar-fill {
  height: 100%;
  transition: width 0.6s cubic-bezier(0.4, 0, 0.2, 1);
}
.tv__bar-fill--critical { background: #ef4444; }
.tv__bar-fill--warning  { background: #f59e0b; }
.tv__bar-fill--info     { background: #3b82f6; }

.tv__bar-count { font-size: 14px; color: #64748b; text-align: right; }

.tv__legend {
  display: flex;
  gap: 20px;
  margin-top: 24px;
}
.tv__legend-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: #64748b;
}
.tv__legend-item::before {
  content: '';
  width: 10px; height: 10px;
  border-radius: 2px;
  display: inline-block;
}
.tv__legend-item--critical::before { background: #ef4444; }
.tv__legend-item--warning::before  { background: #f59e0b; }
.tv__legend-item--info::before     { background: #3b82f6; }

/* Recent alerts */
.tv__recent {
  padding: 32px 40px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.tv__alert-list {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 8px;
  overflow: hidden;
}

.tv__alert-item {
  display: grid;
  grid-template-columns: 80px 1fr auto;
  align-items: center;
  gap: 16px;
  padding: 14px 18px;
  border-radius: 8px;
  background: rgba(255, 255, 255, 0.03);
  border-left: 3px solid transparent;
}
.tv__alert-item--critical { border-left-color: #ef4444; }
.tv__alert-item--warning  { border-left-color: #f59e0b; }
.tv__alert-item--info     { border-left-color: #3b82f6; }

.tv__alert-sev {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  padding: 3px 8px;
  border-radius: 4px;
  text-align: center;
}
.tv__alert-sev--critical { background: rgba(239,68,68,.18); color: #ef4444; }
.tv__alert-sev--warning  { background: rgba(245,158,11,.18); color: #f59e0b; }
.tv__alert-sev--info     { background: rgba(59,130,246,.18); color: #60a5fa; }

.tv__alert-info { display: flex; flex-direction: column; gap: 3px; min-width: 0; }
.tv__alert-title {
  font-size: 15px;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.tv__alert-meta { font-size: 12px; color: #64748b; }
.tv__alert-meta code { font-family: monospace; color: #475569; }
.tv__alert-meta strong { color: #94a3b8; }

.tv__alert-time {
  font-size: 13px;
  color: #475569;
  font-variant-numeric: tabular-nums;
  white-space: nowrap;
}

/* Transitions */
.tv-alert-enter-active { transition: opacity 0.3s, transform 0.3s; }
.tv-alert-enter-from  { opacity: 0; transform: translateX(-16px); }

/* Back link */
.tv__back {
  position: fixed;
  bottom: 20px;
  right: 24px;
  font-size: 13px;
  color: #475569;
  text-decoration: none;
  padding: 6px 14px;
  border-radius: 8px;
  border: 1px solid rgba(255,255,255,0.07);
  transition: color 0.15s, background 0.15s;
}
.tv__back:hover { color: #94a3b8; background: rgba(255,255,255,0.05); }
</style>
