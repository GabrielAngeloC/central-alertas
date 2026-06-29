import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi, ApiError } from '@/api'
import { getToken, setToken } from '@/api/http'

interface SessionUser {
  userId: string
  name: string
  email: string
  expiresAt: string
}

const USER_KEY = 'central-alertas:user'

function loadUser(): SessionUser | null {
  try {
    const raw = localStorage.getItem(USER_KEY)
    return raw ? (JSON.parse(raw) as SessionUser) : null
  } catch {
    return null
  }
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<SessionUser | null>(loadUser())
  const token = ref<string | null>(getToken())
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)

  async function login(email: string, password: string) {
    loading.value = true
    error.value = null
    try {
      const res = await authApi.login({ email, password })
      token.value = res.accessToken
      setToken(res.accessToken)
      const session: SessionUser = {
        userId: res.userId,
        name: res.name,
        email: res.email,
        expiresAt: res.expiresAt,
      }
      user.value = session
      localStorage.setItem(USER_KEY, JSON.stringify(session))
      return true
    } catch (err) {
      if (err instanceof ApiError) {
        error.value = err.status === 401 ? 'E-mail ou senha inválidos.' : err.message
      } else {
        error.value = 'Falha inesperada ao entrar.'
      }
      return false
    } finally {
      loading.value = false
    }
  }

  function logout() {
    token.value = null
    user.value = null
    setToken(null)
    localStorage.removeItem(USER_KEY)
  }

  return { user, token, loading, error, isAuthenticated, login, logout }
})
