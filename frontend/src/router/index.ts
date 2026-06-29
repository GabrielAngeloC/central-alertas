import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '@/views/DashboardView.vue'
import TvView from '@/views/TvView.vue'
import SettingsView from '@/views/SettingsView.vue'
import RulesConfigView from '@/views/RulesConfigView.vue'
import SearchView from '@/views/SearchView.vue'
import StatsView from '@/views/StatsView.vue'
import HubHealthView from '@/views/HubHealthView.vue'
import LoginView from '@/views/LoginView.vue'
import { getToken } from '@/api/http'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/login', component: LoginView, meta: { public: true, fullscreen: true } },
    { path: '/', component: DashboardView },
    { path: '/tv', component: TvView, meta: { fullscreen: true } },
    { path: '/busca', component: SearchView },
    { path: '/estatisticas', component: StatsView },
    { path: '/saude', component: HubHealthView },
    { path: '/configuracao', component: RulesConfigView },
    { path: '/configuracoes', component: SettingsView },
  ],
})

// Protege as rotas: sem token JWT, redireciona para o login.
router.beforeEach((to) => {
  const authed = !!getToken()
  if (!to.meta.public && !authed) {
    return { path: '/login', query: { redirect: to.fullPath } }
  }
  if (to.path === '/login' && authed) {
    return { path: '/' }
  }
  return true
})

export default router
