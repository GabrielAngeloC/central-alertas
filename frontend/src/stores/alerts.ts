import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Alert, Severity } from '@/types/alert'

export const useAlertsStore = defineStore('alerts', () => {
  const alerts = ref<Alert[]>([])
  const filterSeverity = ref<Severity | 'all'>('all')
  const filterCategory = ref<string>('all')

  const categories = computed(() => {
    const cats = new Set(alerts.value.map((a) => a.category))
    return ['all', ...Array.from(cats).sort()]
  })

  const filteredAlerts = computed(() => {
    return alerts.value.filter((alert) => {
      const matchSeverity =
        filterSeverity.value === 'all' || alert.severity === filterSeverity.value
      const matchCategory =
        filterCategory.value === 'all' || alert.category === filterCategory.value
      return matchSeverity && matchCategory
    })
  })

  const countBySeverity = computed(() => ({
    critical: alerts.value.filter((a) => a.severity === 'critical').length,
    warning: alerts.value.filter((a) => a.severity === 'warning').length,
    info: alerts.value.filter((a) => a.severity === 'info').length,
  }))

  function addAlert(alert: Alert) {
    const existing = alerts.value.findIndex((a) => a.dedup_key === alert.dedup_key)
    const entry: Alert = {
      ...alert,
      id: alert.id ?? crypto.randomUUID(),
      received_at: new Date().toISOString(),
    }
    if (existing >= 0) {
      alerts.value.splice(existing, 1, entry)
    } else {
      alerts.value.unshift(entry)
    }
  }

  function removeAlert(id: string) {
    alerts.value = alerts.value.filter((a) => a.id !== id)
  }

  function setAlerts(list: Alert[]) {
    alerts.value = list.map((a) => ({
      ...a,
      id: a.id ?? crypto.randomUUID(),
      received_at: a.received_at ?? new Date().toISOString(),
    }))
  }

  return {
    alerts,
    filteredAlerts,
    categories,
    countBySeverity,
    filterSeverity,
    filterCategory,
    addAlert,
    removeAlert,
    setAlerts,
  }
})
