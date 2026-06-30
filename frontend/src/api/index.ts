import { apiRequest } from './http'
import type {
  LoginRequest,
  LoginResponse,
  AlertSummaryResponse,
  DashboardSummaryResponse,
  NotificationDestinationResponse,
  CreateNotificationDestinationRequest,
  UpdateNotificationDestinationRequest,
  TestNotificationDestinationResponse,
  RoutingRuleResponse,
  CreateRoutingRuleRequest,
  UpdateRoutingRuleRequest,
  AllowedOriginResponse,
  CreateAllowedOriginRequest,
  UpdateAllowedOriginRequest,
  DashboardViewResponse,
  AlertDetailResponse,
  AlertOccurrenceResponse,
  AlertDeliveryResponse,
  SourceResponse,
  SourceHealthResponse,
  CreateSourceRequest,
  UpdateSourceRequest,
  DashboardStatisticsResponse,
  HubHealthResponse,
} from './types'

export type AlertListParams = {
  status?: string
  severity?: string
  category?: string
  source?: string
  from?: string
  to?: string
}

const V1 = '/api/v1'

export const authApi = {
  login: (payload: LoginRequest) =>
    apiRequest<LoginResponse>(`${V1}/auth/login`, { method: 'POST', body: payload, anonymous: true }),
}

function buildQuery(params: Record<string, string | undefined>): string {
  const q = Object.entries(params)
    .filter(([, v]) => v !== undefined && v !== null && v !== '')
    .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(v as string)}`)
    .join('&')
  return q ? `?${q}` : ''
}

export const alertsApi = {
  list: (params: AlertListParams = {}) =>
    apiRequest<AlertSummaryResponse[]>(`${V1}/alerts${buildQuery(params)}`),
  active: () => apiRequest<AlertSummaryResponse[]>(`${V1}/alerts/active`),
  resolved: () => apiRequest<AlertSummaryResponse[]>(`${V1}/alerts/resolved`),
  search: (q: string) =>
    apiRequest<AlertSummaryResponse[]>(`${V1}/alerts/search${buildQuery({ q })}`),
  byId: (id: string) => apiRequest<AlertDetailResponse>(`${V1}/alerts/${id}`),
  occurrences: (id: string) =>
    apiRequest<AlertOccurrenceResponse[]>(`${V1}/alerts/${id}/occurrences`),
  deliveries: (id: string) =>
    apiRequest<AlertDeliveryResponse[]>(`${V1}/alerts/${id}/deliveries`),
  resolve: (id: string, reason?: string) =>
    apiRequest<unknown>(`${V1}/alerts/${id}/resolve`, { method: 'POST', body: { reason } }),
}

export const dashboardApi = {
  summary: () => apiRequest<DashboardSummaryResponse>(`${V1}/dashboard/summary`),
  views: () => apiRequest<DashboardViewResponse[]>(`${V1}/dashboard/views`),
  statistics: () => apiRequest<DashboardStatisticsResponse>(`${V1}/dashboard/statistics`),
  hubHealth: () => apiRequest<HubHealthResponse>(`${V1}/dashboard/hub-health`),
}

export const destinationsApi = {
  list: () => apiRequest<NotificationDestinationResponse[]>(`${V1}/notification-destinations`),
  create: (payload: CreateNotificationDestinationRequest) =>
    apiRequest<NotificationDestinationResponse>(`${V1}/notification-destinations`, {
      method: 'POST',
      body: payload,
    }),
  update: (id: string, payload: UpdateNotificationDestinationRequest) =>
    apiRequest<NotificationDestinationResponse>(`${V1}/notification-destinations/${id}`, {
      method: 'PUT',
      body: payload,
    }),
  remove: (id: string) =>
    apiRequest<void>(`${V1}/notification-destinations/${id}`, { method: 'DELETE' }),
  activate: (id: string) =>
    apiRequest<unknown>(`${V1}/notification-destinations/${id}/activate`, { method: 'POST' }),
  deactivate: (id: string) =>
    apiRequest<unknown>(`${V1}/notification-destinations/${id}/deactivate`, { method: 'POST' }),
  test: (id: string) =>
    apiRequest<TestNotificationDestinationResponse>(`${V1}/notification-destinations/${id}/test`, {
      method: 'POST',
    }),
}

export const sourcesApi = {
  list: () => apiRequest<SourceResponse[]>(`${V1}/sources`),
  create: (payload: CreateSourceRequest) =>
    apiRequest<SourceResponse>(`${V1}/sources`, { method: 'POST', body: payload }),
  update: (id: string, payload: UpdateSourceRequest) =>
    apiRequest<SourceResponse>(`${V1}/sources/${id}`, { method: 'PUT', body: payload }),
  remove: (id: string) => apiRequest<void>(`${V1}/sources/${id}`, { method: 'DELETE' }),
  activate: (id: string) => apiRequest<unknown>(`${V1}/sources/${id}/activate`, { method: 'POST' }),
  deactivate: (id: string) => apiRequest<unknown>(`${V1}/sources/${id}/deactivate`, { method: 'POST' }),
  health: () => apiRequest<SourceHealthResponse[]>(`${V1}/sources/health`),
}

export const routingRulesApi = {
  list: () => apiRequest<RoutingRuleResponse[]>(`${V1}/routing-rules`),
  create: (payload: CreateRoutingRuleRequest) =>
    apiRequest<RoutingRuleResponse>(`${V1}/routing-rules`, { method: 'POST', body: payload }),
  update: (id: string, payload: UpdateRoutingRuleRequest) =>
    apiRequest<RoutingRuleResponse>(`${V1}/routing-rules/${id}`, { method: 'PUT', body: payload }),
  remove: (id: string) => apiRequest<void>(`${V1}/routing-rules/${id}`, { method: 'DELETE' }),
  activate: (id: string) => apiRequest<unknown>(`${V1}/routing-rules/${id}/activate`, { method: 'POST' }),
  deactivate: (id: string) =>
    apiRequest<unknown>(`${V1}/routing-rules/${id}/deactivate`, { method: 'POST' }),
}

export const corsOriginsApi = {
  list: () => apiRequest<AllowedOriginResponse[]>(`${V1}/cors-origins`),
  create: (payload: CreateAllowedOriginRequest) =>
    apiRequest<AllowedOriginResponse>(`${V1}/cors-origins`, { method: 'POST', body: payload }),
  update: (id: string, payload: UpdateAllowedOriginRequest) =>
    apiRequest<AllowedOriginResponse>(`${V1}/cors-origins/${id}`, { method: 'PUT', body: payload }),
  remove: (id: string) =>
    apiRequest<void>(`${V1}/cors-origins/${id}`, { method: 'DELETE' }),
  activate: (id: string) =>
    apiRequest<AllowedOriginResponse>(`${V1}/cors-origins/${id}/activate`, { method: 'POST' }),
  deactivate: (id: string) =>
    apiRequest<AllowedOriginResponse>(`${V1}/cors-origins/${id}/deactivate`, { method: 'POST' }),
}

export * from './types'
export { ApiError } from './http'
