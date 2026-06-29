<template>
  <div class="dashboard">
    <!-- Topbar -->
    <header class="topbar">
      <div class="topbar__brand">
        <h1 class="topbar__title">Central de Alertas</h1>
      </div>
      <div class="topbar__actions">
        <span class="total-count">{{ store.filteredAlerts.length }} alertas</span>
      </div>
    </header>

    <!-- Summary counters -->
    <section class="summary-bar">
      <button
        class="summary-chip summary-chip--critical"
        :class="{ 'summary-chip--active': store.filterSeverity === 'critical' }"
        @click="toggleSeverity('critical')"
      >
        <span class="summary-chip__count">{{ store.countBySeverity.critical }}</span>
        <span class="summary-chip__label">Críticos</span>
      </button>
      <button
        class="summary-chip summary-chip--warning"
        :class="{ 'summary-chip--active': store.filterSeverity === 'warning' }"
        @click="toggleSeverity('warning')"
      >
        <span class="summary-chip__count">{{ store.countBySeverity.warning }}</span>
        <span class="summary-chip__label">Atenção</span>
      </button>
      <button
        class="summary-chip summary-chip--info"
        :class="{ 'summary-chip--active': store.filterSeverity === 'info' }"
        @click="toggleSeverity('info')"
      >
        <span class="summary-chip__count">{{ store.countBySeverity.info }}</span>
        <span class="summary-chip__label">Info</span>
      </button>
    </section>

    <!-- Filters -->
    <section class="filters">
      <div class="filter-group">
        <label class="filter-label">Categoria</label>
        <div class="filter-pills">
          <button
            v-for="cat in store.categories"
            :key="cat"
            class="pill"
            :class="{ 'pill--active': store.filterCategory === cat }"
            @click="store.filterCategory = cat"
          >
            {{ cat === 'all' ? 'Todas' : cat }}
          </button>
        </div>
      </div>
    </section>

    <!-- Alert grid -->
    <main class="alerts-grid">
      <TransitionGroup name="card" tag="div" class="alerts-grid__inner">
        <AlertCard
          v-for="alert in store.filteredAlerts"
          :key="alert.id"
          :alert="alert"
          @dismiss="store.removeAlert($event)"
        />
      </TransitionGroup>

      <div v-if="store.filteredAlerts.length === 0" class="empty-state">
        <span class="empty-state__icon">✅</span>
        <p class="empty-state__text">Nenhum alerta encontrado</p>
        <p class="empty-state__sub">Ajuste os filtros ou aguarde novas ocorrências</p>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { useAlertsStore } from '@/stores/alerts'
import AlertCard from '@/components/AlertCard.vue'
import type { Severity } from '@/types/alert'

const store = useAlertsStore()

function toggleSeverity(sev: Severity) {
  store.filterSeverity = store.filterSeverity === sev ? 'all' : sev
}
</script>

<style scoped>
.dashboard {
  min-height: 100vh;
  background: var(--bg-page);
  display: flex;
  flex-direction: column;
  gap: 0;
}

/* Topbar */
.topbar {
  background: var(--bg-surface);
  border-bottom: 1px solid var(--border-color);
  padding: 14px 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: sticky;
  top: 0;
  z-index: 10;
}
.topbar__brand {
  display: flex;
  align-items: center;
  gap: 10px;
}
.topbar__icon { font-size: 22px; }
.topbar__title {
  margin: 0;
  font-size: 18px;
  font-weight: 700;
  color: var(--text-primary);
  letter-spacing: -0.02em;
}
.total-count {
  font-size: 13px;
  color: var(--text-muted);
}

/* Summary bar */
.summary-bar {
  display: flex;
  gap: 12px;
  padding: 16px 24px;
  background: var(--bg-surface);
  border-bottom: 1px solid var(--border-color);
}

.summary-chip {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 18px;
  border-radius: 10px;
  border: 1px solid transparent;
  cursor: pointer;
  background: rgba(255,255,255,0.04);
  transition: background 0.15s, border-color 0.15s, transform 0.1s;
}
.summary-chip:hover { background: rgba(255,255,255,0.08); }
.summary-chip:active { transform: scale(0.97); }

.summary-chip--critical { border-color: rgba(239,68,68,0.3); }
.summary-chip--warning  { border-color: rgba(245,158,11,0.3); }
.summary-chip--info     { border-color: rgba(59,130,246,0.3); }

.summary-chip--active.summary-chip--critical { background: rgba(239,68,68,0.15); border-color: #ef4444; }
.summary-chip--active.summary-chip--warning  { background: rgba(245,158,11,0.15); border-color: #f59e0b; }
.summary-chip--active.summary-chip--info     { background: rgba(59,130,246,0.15); border-color: #3b82f6; }

.summary-chip__count {
  font-size: 22px;
  font-weight: 800;
  line-height: 1;
}
.summary-chip--critical .summary-chip__count { color: #ef4444; }
.summary-chip--warning  .summary-chip__count { color: #f59e0b; }
.summary-chip--info     .summary-chip__count { color: #60a5fa; }

.summary-chip__label {
  font-size: 12px;
  color: var(--text-secondary);
  font-weight: 500;
}

/* Filters */
.filters {
  padding: 14px 24px;
  background: var(--bg-page);
  border-bottom: 1px solid var(--border-color);
}
.filter-group {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}
.filter-label {
  font-size: 12px;
  color: var(--text-muted);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.filter-pills {
  display: flex;
  gap: 6px;
  flex-wrap: wrap;
}
.pill {
  padding: 4px 12px;
  border-radius: 20px;
  border: 1px solid rgba(255,255,255,0.12);
  background: none;
  font-size: 12px;
  color: var(--text-secondary);
  cursor: pointer;
  font-family: monospace;
  transition: background 0.15s, border-color 0.15s, color 0.15s;
}
.pill:hover { background: rgba(255,255,255,0.07); }
.pill--active {
  background: rgba(99,102,241,0.2);
  border-color: #6366f1;
  color: #a5b4fc;
}

/* Grid */
.alerts-grid {
  padding: 24px;
  flex: 1;
}
.alerts-grid__inner {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
  gap: 16px;
}

/* Transitions */
.card-enter-active,
.card-leave-active {
  transition: opacity 0.25s ease, transform 0.25s ease;
}
.card-enter-from {
  opacity: 0;
  transform: translateY(-12px);
}
.card-leave-to {
  opacity: 0;
  transform: scale(0.95);
}
.card-move {
  transition: transform 0.25s ease;
}

/* Empty state */
.empty-state {
  grid-column: 1 / -1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 24px;
  gap: 8px;
}
.empty-state__icon { font-size: 48px; }
.empty-state__text {
  font-size: 16px;
  font-weight: 600;
  color: var(--text-secondary);
  margin: 0;
}
.empty-state__sub {
  font-size: 13px;
  color: var(--text-muted);
  margin: 0;
}
</style>
