// Tipos espelhando os contratos (DTOs) do backend .NET.

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  userId: string
  name: string
  email: string
  accessToken: string
  expiresAt: string
  tokenType: string
}

// GET /api/v1/alerts e /active e /resolved
export interface AlertSummaryResponse {
  id: string
  source: string
  category: string
  type: string
  severity: string
  title: string
  message?: string | null
  dedupKey: string
  metricValue?: number | null
  metricUnit?: string | null
  metricThreshold?: number | null
  occurrenceCount: number
  firstSeenAt: string
  lastSeenAt: string
  resolvedAt?: string | null
  resolutionReason?: string | null
  isActive: boolean
  isEscalating: boolean
}

// GET /api/v1/dashboard/summary
export interface DashboardSummaryResponse {
  activeCriticalCount: number
  activeWarningCount: number
  activeInfoCount: number
  activeAlertsCount: number
  healthySourcesCount: number
  silentSourcesCount: number
  totalSourcesCount: number
  alertsBySeverity: { severity: string; count: number }[]
  alertsByCategory: { category: string; count: number }[]
  latestAlerts: {
    id: string
    source: string
    category: string
    type: string
    severity: string
    title: string
    metricValue?: number | null
    metricUnit?: string | null
    lastSeenAt: string
    isEscalating: boolean
  }[]
}

// GET /api/v1/dashboard/views (visões por categoria)
export interface DashboardViewAlertResponse {
  id: string
  source: string
  type: string
  severity: string
  title: string
  message?: string | null
  metricValue?: number | null
  metricUnit?: string | null
  metricThreshold?: number | null
  occurrenceCount: number
  isEscalating: boolean
  firstSeenAt: string
  lastSeenAt: string
}

export interface DashboardViewResponse {
  category: string
  title: string
  order: number
  alerts: DashboardViewAlertResponse[]
}

// GET /api/v1/dashboard/statistics
export interface DashboardStatisticsResponse {
  alertsPerDay: { date: string; count: number }[]
  byCategory: { label: string; count: number }[]
  byType: { label: string; count: number }[]
}

// GET /api/v1/dashboard/hub-health
export interface HubHealthResponse {
  windowHours: number
  totalDeliveries: number
  successCount: number
  failedCount: number
  skippedCount: number
  byChannel: { channel: string; success: number; failed: number; skipped: number }[]
}

// GET /api/v1/alerts/{id}
export interface AlertDetailResponse {
  id: string
  source: string
  category: string
  type: string
  severity: string
  title: string
  message?: string | null
  dedupKey: string
  metricValue?: number | null
  metricUnit?: string | null
  metricThreshold?: number | null
  items?: unknown
  payload?: unknown
  occurrenceCount: number
  firstSeenAt: string
  lastSeenAt: string
  lastNotifiedAt?: string | null
  resolvedAt?: string | null
  resolutionReason?: string | null
  isActive: boolean
  isEscalating: boolean
}

// GET /api/v1/alerts/{id}/occurrences
export interface AlertOccurrenceResponse {
  id: string
  alertId: string
  metricValue?: number | null
  metricUnit?: string | null
  metricThreshold?: number | null
  items?: unknown
  payload?: unknown
  receivedAt: string
}

// GET /api/v1/alerts/{alertId}/deliveries
export interface AlertDeliveryResponse {
  id: string
  alertId: string
  routingRuleId?: string | null
  routingRuleName?: string | null
  notificationDestinationId?: string | null
  notificationDestinationName?: string | null
  channel: string
  status: string
  errorMessage?: string | null
  attemptedAt: string
  sentAt?: string | null
}

// GET/POST/PUT /api/v1/notification-destinations
export interface NotificationDestinationResponse {
  id: string
  name: string
  type: string
  configurationJson: string
  isActive: boolean
  createdAt: string
  updatedAt?: string | null
}

export interface CreateNotificationDestinationRequest {
  name: string
  type: string
  configuration: Record<string, unknown>
}

export interface UpdateNotificationDestinationRequest {
  name: string
  type: string
  configuration: Record<string, unknown>
  isActive: boolean
}

export interface TestNotificationDestinationResponse {
  destinationId: string
  destinationName: string
  type: string
  success: boolean
  message: string
  error?: string | null
}

// /api/v1/routing-rules
export interface RoutingRuleResponse {
  id: string
  name: string
  order: number
  severity?: string | null
  category?: string | null
  type?: string | null
  source?: string | null
  deliveryMode: string
  throttleMinutes?: number | null
  isActive: boolean
  createdAt: string
  updatedAt?: string | null
  destinations: { destinationId: string; name: string; type: string }[]
}

export interface CreateRoutingRuleRequest {
  name: string
  order: number
  severity?: string | null
  category?: string | null
  type?: string | null
  source?: string | null
  deliveryMode?: string
  throttleMinutes?: number | null
  destinationIds: string[]
}

export interface UpdateRoutingRuleRequest extends CreateRoutingRuleRequest {
  isActive: boolean
}

// /api/v1/sources
export interface SourceResponse {
  id: string
  name: string
  expectedIntervalMinutes: number
  lastReceivedAt?: string | null
  isActive: boolean
  createdAt: string
  updatedAt?: string | null
}

export interface SourceHealthResponse {
  id: string
  name: string
  expectedIntervalMinutes: number
  lastReceivedAt?: string | null
  nextExpectedAt?: string | null
  status: string
  minutesLate: number
  isSilent: boolean
  isActive: boolean
}

export interface CreateSourceRequest {
  name: string
  expectedIntervalMinutes: number
}

export interface UpdateSourceRequest {
  name: string
  expectedIntervalMinutes: number
}

// /api/v1/cors-origins (CRUD de origens permitidas para CORS)
export interface AllowedOriginResponse {
  id: string
  origin: string
  description?: string | null
  isActive: boolean
  createdAt: string
  updatedAt?: string | null
}

export interface CreateAllowedOriginRequest {
  origin: string
  description?: string | null
}

export interface UpdateAllowedOriginRequest {
  origin: string
  description?: string | null
  isActive: boolean
}
