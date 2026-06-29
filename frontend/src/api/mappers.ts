import type { Alert, Severity } from '@/types/alert'
import type { AlertSummaryResponse } from './types'

function normalizeSeverity(value: string): Severity {
  const v = (value || '').toLowerCase()
  if (v === 'critical' || v === 'warning' || v === 'info') return v
  // fallbacks comuns vindos do backend
  if (v === 'crit' || v === 'critico' || v === 'crítico') return 'critical'
  if (v === 'warn' || v === 'atencao' || v === 'atenção') return 'warning'
  return 'info'
}

// Converte o DTO do backend (AlertSummaryResponse) para o modelo do frontend.
export function mapAlertSummary(dto: AlertSummaryResponse): Alert {
  return {
    id: dto.id,
    source: dto.source,
    category: dto.category,
    type: dto.type,
    severity: normalizeSeverity(dto.severity),
    title: dto.title,
    message: dto.message ?? undefined,
    dedup_key: dto.dedupKey,
    metric:
      dto.metricValue != null
        ? {
            value: dto.metricValue,
            unit: dto.metricUnit ?? '',
            threshold: dto.metricThreshold ?? undefined,
          }
        : undefined,
    received_at: dto.lastSeenAt,
  }
}
