// Cliente HTTP central para falar com a API .NET (Central de Alertas).
//
// - Base da URL vem de VITE_API_URL. Se vazio, usa caminho relativo e
//   conta com o proxy do Vite (/api -> backend) em desenvolvimento.
// - Injeta o token JWT (Bearer) automaticamente quando existe.
// - Em 401, dispara o handler global (redireciona para o login).

const API_BASE = (import.meta.env.VITE_API_URL as string | undefined)?.replace(/\/$/, '') ?? ''

const TOKEN_KEY = 'central-alertas:token'

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token: string | null) {
  if (token) localStorage.setItem(TOKEN_KEY, token)
  else localStorage.removeItem(TOKEN_KEY)
}

let onUnauthorized: (() => void) | null = null
export function setUnauthorizedHandler(handler: () => void) {
  onUnauthorized = handler
}

export class ApiError extends Error {
  status: number
  body: unknown
  constructor(status: number, message: string, body?: unknown) {
    super(message)
    this.status = status
    this.body = body
  }
}

interface RequestOptions {
  method?: string
  body?: unknown
  // headers extras (ex.: X-API-KEY na ingestão)
  headers?: Record<string, string>
  // não enviar o token (endpoints públicos como login)
  anonymous?: boolean
}

export async function apiRequest<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { method = 'GET', body, headers = {}, anonymous = false } = options

  const finalHeaders: Record<string, string> = { ...headers }

  if (body !== undefined && !(body instanceof FormData)) {
    finalHeaders['Content-Type'] = 'application/json'
  }

  if (!anonymous) {
    const token = getToken()
    if (token) finalHeaders['Authorization'] = `Bearer ${token}`
  }

  let response: Response
  try {
    response = await fetch(`${API_BASE}${path}`, {
      method,
      headers: finalHeaders,
      body: body !== undefined ? (body instanceof FormData ? body : JSON.stringify(body)) : undefined,
    })
  } catch (err) {
    throw new ApiError(0, 'Não foi possível conectar à API. Verifique se o backend está no ar.', err)
  }

  if (response.status === 401) {
    if (!anonymous && onUnauthorized) onUnauthorized()
    throw new ApiError(401, 'Sessão expirada ou não autorizada.')
  }

  const text = await response.text()
  const data = text ? safeJson(text) : null

  if (!response.ok) {
    const message =
      (data && typeof data === 'object' && 'message' in data && (data as { message?: string }).message) ||
      `Erro ${response.status} ao chamar a API.`
    throw new ApiError(response.status, message as string, data)
  }

  return data as T
}

function safeJson(text: string): unknown {
  try {
    return JSON.parse(text)
  } catch {
    return text
  }
}
