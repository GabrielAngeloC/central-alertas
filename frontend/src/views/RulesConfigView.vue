<template>
  <div class="config">
    <header class="config__header">
      <h1 class="config__title">Configuração</h1>
      <p class="config__subtitle">Regras de roteamento, destinos e fontes — administráveis pela interface.</p>
    </header>

    <nav class="config__tabs">
      <button
        v-for="tab in tabs"
        :key="tab.id"
        class="config__tab"
        :class="{ 'config__tab--active': active === tab.id }"
        @click="active = tab.id"
      >
        {{ tab.label }}
      </button>
    </nav>

    <RulesManager v-if="active === 'rules'" />
    <DestinationsManager v-else-if="active === 'destinations'" />
    <SourcesManager v-else-if="active === 'sources'" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import RulesManager from '@/components/config/RulesManager.vue'
import DestinationsManager from '@/components/config/DestinationsManager.vue'
import SourcesManager from '@/components/config/SourcesManager.vue'

const tabs = [
  { id: 'rules', label: 'Regras' },
  { id: 'destinations', label: 'Destinos' },
  { id: 'sources', label: 'Fontes' },
] as const

const active = ref<'rules' | 'destinations' | 'sources'>('rules')
</script>

<style scoped>
.config { padding: 32px 32px 60px; max-width: 1100px; margin: 0 auto; }
.config__header { margin-bottom: 20px; }
.config__title { font-size: 24px; font-weight: 700; color: var(--text-primary); margin: 0 0 6px; }
.config__subtitle { font-size: 14px; color: var(--text-muted); margin: 0; }

.config__tabs { display: flex; gap: 6px; margin-bottom: 22px; border-bottom: 1px solid var(--border-color); }
.config__tab {
  background: none; border: none; color: var(--text-muted);
  font-size: 14px; font-weight: 600; font-family: inherit;
  padding: 10px 16px; cursor: pointer; border-bottom: 2px solid transparent; margin-bottom: -1px;
}
.config__tab:hover { color: var(--text-secondary); }
.config__tab--active { color: var(--text-primary); border-bottom-color: #6366f1; }
</style>
