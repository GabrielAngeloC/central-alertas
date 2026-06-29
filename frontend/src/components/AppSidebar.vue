<template>
  <aside class="sidebar" :class="{ 'sidebar--collapsed': collapsed }">
    <!-- Toggle -->
    <button class="sidebar__toggle" @click="collapsed = !collapsed" :title="collapsed ? 'Expandir menu' : 'Recolher menu'">
      <svg v-if="collapsed" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <line x1="3" y1="6" x2="21" y2="6"/>
        <line x1="3" y1="12" x2="21" y2="12"/>
        <line x1="3" y1="18" x2="21" y2="18"/>
      </svg>
      <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <polyline points="15,18 9,12 15,6"/>
      </svg>
    </button>

    <!-- Brand -->
    <div class="sidebar__brand">
      <span class="sidebar__brand-name">Central de Alertas</span>
    </div>

    <div class="sidebar__divider" />

    <!-- Nav -->
    <nav class="sidebar__nav">
      <RouterLink
        v-for="item in navItems"
        :key="item.to"
        :to="item.to"
        class="sidebar__item"
        :title="collapsed ? item.label : ''"
        active-class="sidebar__item--active"
        exact-active-class="sidebar__item--active"
      >
        <span class="sidebar__item-icon" v-html="item.icon" />
        <span class="sidebar__item-label">{{ item.label }}</span>
      </RouterLink>
    </nav>

    <!-- Footer: alert counters mini -->
    <div class="sidebar__footer" v-if="!collapsed">
      <div class="mini-counter mini-counter--critical">
        <span class="mini-counter__dot" />
        <span>{{ store.countBySeverity.critical }} críticos</span>
      </div>
      <div class="mini-counter mini-counter--warning">
        <span class="mini-counter__dot" />
        <span>{{ store.countBySeverity.warning }} atenção</span>
      </div>
      <div class="mini-counter mini-counter--info">
        <span class="mini-counter__dot" />
        <span>{{ store.countBySeverity.info }} info</span>
      </div>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { useSidebar } from '@/composables/useSidebar'
import { useAlertsStore } from '@/stores/alerts'

const { collapsed } = useSidebar()
const store = useAlertsStore()

const navItems = [
  {
    to: '/',
    label: 'Dashboard',
    icon: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
      <rect x="3" y="3" width="7" height="7" rx="1"/>
      <rect x="14" y="3" width="7" height="7" rx="1"/>
      <rect x="3" y="14" width="7" height="7" rx="1"/>
      <rect x="14" y="14" width="7" height="7" rx="1"/>
    </svg>`,
  },
  {
    to: '/tv',
    label: 'Modo TV',
    icon: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
      <rect x="2" y="3" width="20" height="14" rx="2"/>
      <line x1="8" y1="21" x2="16" y2="21"/>
      <line x1="12" y1="17" x2="12" y2="21"/>
    </svg>`,
  },
  {
    to: '/busca',
    label: 'Busca',
    icon: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
      <circle cx="11" cy="11" r="7"/>
      <line x1="21" y1="21" x2="16.65" y2="16.65"/>
    </svg>`,
  },
  {
    to: '/estatisticas',
    label: 'Estatísticas',
    icon: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
      <line x1="4" y1="20" x2="4" y2="10"/>
      <line x1="10" y1="20" x2="10" y2="4"/>
      <line x1="16" y1="20" x2="16" y2="14"/>
      <line x1="22" y1="20" x2="2" y2="20"/>
    </svg>`,
  },
  {
    to: '/saude',
    label: 'Saúde do hub',
    icon: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
      <path d="M22 12h-4l-3 9L9 3l-3 9H2"/>
    </svg>`,
  },
  {
    to: '/configuracao',
    label: 'Regras & Cadastros',
    icon: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
      <line x1="4" y1="6" x2="20" y2="6"/>
      <line x1="4" y1="12" x2="20" y2="12"/>
      <line x1="4" y1="18" x2="20" y2="18"/>
      <circle cx="8" cy="6" r="2" fill="currentColor"/>
      <circle cx="16" cy="12" r="2" fill="currentColor"/>
      <circle cx="9" cy="18" r="2" fill="currentColor"/>
    </svg>`,
  },
  {
    to: '/configuracoes',
    label: 'Preferências',
    icon: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
      <circle cx="12" cy="12" r="3"/>
      <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83-2.83l.06-.06A1.65 1.65 0 0 0 4.68 15a1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 2.83-2.83l.06.06A1.65 1.65 0 0 0 9 4.68a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 2.83l-.06.06A1.65 1.65 0 0 0 19.4 9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"/>
    </svg>`,
  },
]
</script>

<style scoped>
.sidebar {
  position: fixed;
  left: 0;
  top: 0;
  height: 100vh;
  width: 220px;
  background: #0d1220;
  border-right: 1px solid rgba(255, 255, 255, 0.07);
  display: flex;
  flex-direction: column;
  gap: 0;
  transition: width 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
  z-index: 50;
}
.sidebar--collapsed {
  width: 64px;
}

/* Toggle */
.sidebar__toggle {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 56px;
  border: none;
  background: none;
  color: #64748b;
  cursor: pointer;
  flex-shrink: 0;
  transition: color 0.15s, background 0.15s;
  padding: 0 20px;
  justify-content: flex-start;
}
.sidebar--collapsed .sidebar__toggle {
  justify-content: center;
  padding: 0;
}
.sidebar__toggle:hover {
  color: #94a3b8;
  background: rgba(255, 255, 255, 0.04);
}
.sidebar__toggle svg {
  width: 20px;
  height: 20px;
  flex-shrink: 0;
}

/* Brand */
.sidebar__brand {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 0 16px 16px;
  overflow: hidden;
}
.sidebar__brand-icon {
  font-size: 20px;
  flex-shrink: 0;
  width: 32px;
  text-align: center;
}
.sidebar--collapsed .sidebar__brand-icon {
  width: auto;
}
.sidebar__brand-name {
  font-size: 13px;
  font-weight: 700;
  color: #e2e8f0;
  white-space: nowrap;
  overflow: hidden;
  opacity: 1;
  transition: opacity 0.15s;
}
.sidebar--collapsed .sidebar__brand-name {
  opacity: 0;
  pointer-events: none;
  width: 0;
}

/* Divider */
.sidebar__divider {
  height: 1px;
  background: rgba(255, 255, 255, 0.07);
  margin: 0 0 8px;
}

/* Nav */
.sidebar__nav {
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 0 8px;
  flex: 1;
}

.sidebar__item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 10px;
  border-radius: 8px;
  text-decoration: none;
  color: #64748b;
  font-size: 14px;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  transition: color 0.15s, background 0.15s;
}
.sidebar__item:hover {
  color: #cbd5e1;
  background: rgba(255, 255, 255, 0.06);
}
.sidebar__item--active {
  color: #a5b4fc;
  background: rgba(99, 102, 241, 0.15);
}

.sidebar__item-icon {
  flex-shrink: 0;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.sidebar__item-icon :deep(svg) {
  width: 20px;
  height: 20px;
}

.sidebar__item-label {
  overflow: hidden;
  opacity: 1;
  transition: opacity 0.15s;
}
.sidebar--collapsed .sidebar__item-label {
  opacity: 0;
  pointer-events: none;
  width: 0;
}

/* Footer counters */
.sidebar__footer {
  padding: 12px 16px;
  border-top: 1px solid rgba(255, 255, 255, 0.07);
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.mini-counter {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: #475569;
}
.mini-counter__dot {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  flex-shrink: 0;
}
.mini-counter--critical .mini-counter__dot { background: #ef4444; }
.mini-counter--warning  .mini-counter__dot { background: #f59e0b; }
.mini-counter--info     .mini-counter__dot { background: #3b82f6; }
</style>
