import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import App from './App.vue'
import './assets/main.css'
import { setToken, setUnauthorizedHandler } from '@/api/http'

const app = createApp(App)
app.use(createPinia())
app.use(router)

// Em qualquer 401 da API, limpa o token e manda para o login.
setUnauthorizedHandler(() => {
  setToken(null)
  localStorage.removeItem('central-alertas:user')
  if (router.currentRoute.value.path !== '/login') {
    router.replace({ path: '/login', query: { redirect: router.currentRoute.value.fullPath } })
  }
})

app.mount('#app')
