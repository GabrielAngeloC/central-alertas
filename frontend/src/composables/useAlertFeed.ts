import { onMounted, onUnmounted, ref } from 'vue'
import { useAlertsStore } from '@/stores/alerts'
import { useAuthStore } from '@/stores/auth'
import { alertsApi, ApiError } from '@/api'
import { mapAlertSummary } from '@/api/mappers'

// Intervalo de polling (ms). Pode ser sobrescrito por VITE_ALERT_POLL_MS.
const POLL_MS = Number(import.meta.env.VITE_ALERT_POLL_MS ?? 10000)

// Substitui o antigo SSE/mock: busca os alertas ativos do backend em intervalo
// regular enquanto o usuário estiver autenticado.
export function useAlertFeed() {
  const store = useAlertsStore()
  const auth = useAuthStore()

  const connected = ref(false)
  const lastError = ref<string | null>(null)
  let timer: ReturnType<typeof setInterval> | null = null

  async function poll() {
    if (!auth.isAuthenticated) return
    try {
      const list = await alertsApi.active()
      store.setAlerts(list.map(mapAlertSummary))
      connected.value = true
      lastError.value = null
    } catch (err) {
      connected.value = false
      if (err instanceof ApiError && err.status === 401) return // handler global redireciona
      lastError.value = err instanceof ApiError ? err.message : 'Falha ao buscar alertas.'
    }
  }

  onMounted(() => {
    poll()
    timer = setInterval(poll, POLL_MS)
  })

  onUnmounted(() => {
    if (timer) clearInterval(timer)
  })

  return { isConnected: () => connected.value, lastError, refresh: poll }
}
