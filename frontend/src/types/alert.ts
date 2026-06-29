export type Severity = 'critical' | 'warning' | 'info'

export interface AlertMetric {
  value: number
  unit: string
  threshold?: number
}

export interface Alert {
  id?: string
  source: string
  category: string
  type: string
  severity: Severity
  title: string
  message?: string
  dedup_key: string
  metric?: AlertMetric
  items?: Record<string, unknown>[]
  payload?: Record<string, unknown>
  received_at?: string
}
