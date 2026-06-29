<template>
  <div class="app-shell" :class="{ 'app-shell--fullscreen': isFullscreen }">
    <AppSidebar v-if="!isFullscreen" />
    <main class="app-main">
      <RouterView />
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import AppSidebar from '@/components/AppSidebar.vue'
import { useAlertFeed } from '@/composables/useAlertFeed'
import { useAuthStore } from '@/stores/auth'

const feed = useAlertFeed()
const auth = useAuthStore()

// Ao autenticar, atualiza o feed imediatamente (sem esperar o próximo ciclo).
watch(
  () => auth.isAuthenticated,
  (authed) => {
    if (authed) feed.refresh()
  },
)

const route = useRoute()
const isFullscreen = computed(() => !!route.meta.fullscreen)
</script>

<style scoped>
.app-shell {
  display: flex;
  min-height: 100vh;
  padding-left: var(--sidebar-w, 64px);
  transition: padding-left 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}
.app-shell--fullscreen {
  padding-left: 0;
}
.app-main {
  flex: 1;
  min-width: 0;
}
</style>
