<template>
  <div class="dashboard-extra">
    <!-- Faixa de status -->
    <DashboardKpis />

    <!-- Visões por categoria (mockup §06) -->
    <CategoryViews @select="openDetail" />
  </div>

  <!-- Lista de alertas ativos (mantida) -->
  <AlertDashboard />

  <!-- Detalhe do alerta -->
  <AlertDetailDrawer :alert-id="selectedAlertId" @close="selectedAlertId = null" @resolved="onResolved" />

  <RouterLink to="/tv" class="tv-link" title="Abrir modo TV">📺</RouterLink>
</template>

<script setup lang="ts">
// Os alertas chegam do backend via polling (useAlertFeed em App.vue),
// que popula a store. KPIs e visões buscam /dashboard/summary e /dashboard/views.
import { ref } from 'vue'
import AlertDashboard from '@/components/AlertDashboard.vue'
import DashboardKpis from '@/components/DashboardKpis.vue'
import CategoryViews from '@/components/CategoryViews.vue'
import AlertDetailDrawer from '@/components/AlertDetailDrawer.vue'

const selectedAlertId = ref<string | null>(null)

function openDetail(id: string) {
  selectedAlertId.value = id
}

function onResolved() {
  // mantém o drawer aberto mostrando o estado resolvido; o polling atualiza o resto.
}
</script>

<style scoped>
.dashboard-extra {
  padding: 28px 24px 4px;
}
.tv-link {
  position: fixed;
  bottom: 20px;
  right: 20px;
  font-size: 22px;
  background: rgba(255, 255, 255, 0.07);
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 10px;
  padding: 8px 12px;
  text-decoration: none;
  transition: background 0.15s;
  z-index: 100;
}
.tv-link:hover {
  background: rgba(255, 255, 255, 0.14);
}
</style>
