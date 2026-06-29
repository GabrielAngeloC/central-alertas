<template>
  <div class="kpis">
    <div class="kpi kpi--critical">
      <span class="kpi__value">{{ summary?.activeCriticalCount ?? '—' }}</span>
      <span class="kpi__label">Critical ativos</span>
    </div>
    <div class="kpi kpi--warning">
      <span class="kpi__value">{{ summary?.activeWarningCount ?? '—' }}</span>
      <span class="kpi__label">Warnings ativos</span>
    </div>
    <div class="kpi kpi--ok">
      <span class="kpi__value">{{ healthLabel }}</span>
      <span class="kpi__label">Fontes saudáveis</span>
    </div>
    <div class="kpi kpi--info">
      <span class="kpi__value">{{ summary?.activeInfoCount ?? '—' }}</span>
      <span class="kpi__label">Info ativos</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { dashboardApi, ApiError, type DashboardSummaryResponse } from '@/api'

const POLL_MS = Number(import.meta.env.VITE_ALERT_POLL_MS ?? 10000)

const summary = ref<DashboardSummaryResponse | null>(null)
let timer: ReturnType<typeof setInterval> | null = null

const healthLabel = computed(() => {
  if (!summary.value) return '—'
  return `${summary.value.healthySourcesCount}/${summary.value.totalSourcesCount}`
})

async function load() {
  try {
    summary.value = await dashboardApi.summary()
  } catch (err) {
    if (err instanceof ApiError && err.status === 401) return
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
.kpis {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 12px;
  margin-bottom: 20px;
}
.kpi {
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 10px;
  padding: 16px 18px;
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.kpi__value {
  font-size: 30px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  line-height: 1;
}
.kpi__label {
  font-size: 11px;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  color: var(--text-muted);
}
.kpi--critical .kpi__value { color: #ef4444; }
.kpi--warning .kpi__value { color: #f59e0b; }
.kpi--info .kpi__value { color: #60a5fa; }
.kpi--ok .kpi__value { color: #4ade80; }
</style>
