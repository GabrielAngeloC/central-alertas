<template>
  <div class="stats">
    <header class="stats__header">
      <h1 class="stats__title">Estatísticas</h1>
      <p class="stats__subtitle">Tendência e distribuição dos alertas (últimos 30 dias).</p>
    </header>

    <p v-if="error" class="stats__err">{{ error }}</p>

    <!-- Alertas por dia -->
    <section class="card">
      <h2 class="card__title">Alertas por dia</h2>
      <div class="bars" v-if="data">
        <svg :viewBox="`0 0 ${chartW} ${chartH}`" class="bars__svg" preserveAspectRatio="none">
          <g v-for="(d, i) in data.alertsPerDay" :key="d.date">
            <rect
              :x="i * barStep + 1"
              :y="chartH - barHeight(d.count)"
              :width="barStep - 2"
              :height="barHeight(d.count)"
              rx="1"
              fill="#6366f1"
            >
              <title>{{ formatDay(d.date) }}: {{ d.count }}</title>
            </rect>
          </g>
        </svg>
        <div class="bars__axis">
          <span>{{ formatDay(data.alertsPerDay[0]?.date) }}</span>
          <span>hoje</span>
        </div>
      </div>
      <p v-else class="card__loading">Carregando…</p>
    </section>

    <div class="stats__grid">
      <!-- Por categoria -->
      <section class="card">
        <h2 class="card__title">Por categoria</h2>
        <ul class="dist" v-if="data">
          <li v-for="c in data.byCategory" :key="c.label" class="dist__row">
            <span class="dist__label">{{ c.label }}</span>
            <span class="dist__track"><span class="dist__fill" :style="{ width: pct(c.count, maxCategory) }" /></span>
            <span class="dist__val">{{ c.count }}</span>
          </li>
          <li v-if="!data.byCategory.length" class="dist__empty">Sem dados no período.</li>
        </ul>
      </section>

      <!-- Por tipo -->
      <section class="card">
        <h2 class="card__title">Por tipo</h2>
        <ul class="dist" v-if="data">
          <li v-for="t in data.byType" :key="t.label" class="dist__row">
            <span class="dist__label">{{ t.label }}</span>
            <span class="dist__track"><span class="dist__fill dist__fill--alt" :style="{ width: pct(t.count, maxType) }" /></span>
            <span class="dist__val">{{ t.count }}</span>
          </li>
          <li v-if="!data.byType.length" class="dist__empty">Sem dados no período.</li>
        </ul>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { dashboardApi, ApiError, type DashboardStatisticsResponse } from '@/api'

const data = ref<DashboardStatisticsResponse | null>(null)
const error = ref<string | null>(null)

const chartW = 600
const chartH = 140

const barStep = computed(() => (data.value ? chartW / Math.max(1, data.value.alertsPerDay.length) : 20))
const maxDay = computed(() => Math.max(1, ...(data.value?.alertsPerDay.map((d) => d.count) ?? [1])))
const maxCategory = computed(() => Math.max(1, ...(data.value?.byCategory.map((c) => c.count) ?? [1])))
const maxType = computed(() => Math.max(1, ...(data.value?.byType.map((t) => t.count) ?? [1])))

function barHeight(count: number): number {
  return (count / maxDay.value) * (chartH - 8)
}
function pct(value: number, max: number): string {
  return `${(value / max) * 100}%`
}
function formatDay(iso?: string): string {
  if (!iso) return ''
  return new Intl.DateTimeFormat('pt-BR', { day: '2-digit', month: '2-digit' }).format(new Date(iso))
}

async function load() {
  error.value = null
  try {
    data.value = await dashboardApi.statistics()
  } catch (err) {
    if (err instanceof ApiError && err.status === 401) return
    error.value = err instanceof ApiError ? err.message : 'Falha ao carregar estatísticas.'
  }
}

onMounted(load)
</script>

<style scoped>
.stats { padding: 32px 32px 60px; max-width: 1000px; margin: 0 auto; }
.stats__header { margin-bottom: 20px; }
.stats__title { font-size: 24px; font-weight: 700; color: var(--text-primary); margin: 0 0 6px; }
.stats__subtitle { font-size: 14px; color: var(--text-muted); margin: 0; }
.stats__err { color: #f87171; font-size: 13px; }

.card { background: var(--card-bg); border: 1px solid var(--border-color); border-radius: 12px; padding: 20px 22px; margin-bottom: 16px; }
.card__title { font-size: 12px; letter-spacing: 0.08em; text-transform: uppercase; color: var(--text-secondary); margin: 0 0 16px; font-weight: 600; }
.card__loading { color: var(--text-muted); font-size: 13px; }

.bars__svg { width: 100%; height: 140px; display: block; }
.bars__axis { display: flex; justify-content: space-between; font-size: 11px; color: var(--text-muted); margin-top: 6px; font-variant-numeric: tabular-nums; }

.stats__grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
@media (max-width: 760px) { .stats__grid { grid-template-columns: 1fr; } }

.dist { list-style: none; margin: 0; padding: 0; display: flex; flex-direction: column; gap: 10px; }
.dist__row { display: grid; grid-template-columns: 130px 1fr 36px; align-items: center; gap: 12px; }
.dist__label { font-size: 12.5px; color: var(--text-secondary); font-family: monospace; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.dist__track { height: 14px; border-radius: 4px; background: rgba(255, 255, 255, 0.05); overflow: hidden; }
.dist__fill { display: block; height: 100%; background: #6366f1; border-radius: 4px; transition: width 0.4s; }
.dist__fill--alt { background: #22c55e; }
.dist__val { font-size: 12px; color: var(--text-muted); text-align: right; font-variant-numeric: tabular-nums; }
.dist__empty { font-size: 13px; color: var(--text-muted); }
</style>
