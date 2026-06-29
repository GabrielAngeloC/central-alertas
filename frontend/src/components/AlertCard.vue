<template>
  <div class="alert-card" :class="`alert-card--${alert.severity}`">
    <!-- Header -->
    <div class="alert-card__header">
      <div class="alert-card__header-left">
        <span class="severity-badge" :class="`severity-badge--${alert.severity}`">
          <span class="severity-badge__icon">{{ severityIcon }}</span>
          {{ severityLabel }}
        </span>
        <span class="category-tag">{{ alert.category }}</span>
        <span class="type-tag">{{ alert.type }}</span>
      </div>
      <div class="alert-card__header-right">
        <span v-if="alert.received_at" class="timestamp">{{ formattedTime }}</span>
        <button class="dismiss-btn" @click="$emit('dismiss', alert.id!)" title="Descartar">✕</button>
      </div>
    </div>

    <!-- Title & Source -->
    <div class="alert-card__body">
      <h3 class="alert-card__title">{{ alert.title }}</h3>
      <p v-if="alert.message" class="alert-card__message">{{ alert.message }}</p>
      <div class="alert-card__source">
        <span class="source-label">Origem:</span>
        <code class="source-value">{{ alert.source }}</code>
      </div>
    </div>

    <!-- Metric -->
    <div v-if="alert.metric" class="alert-card__metric">
      <span class="metric-value">{{ alert.metric.value }}</span>
      <span class="metric-unit">{{ alert.metric.unit }}</span>
      <span v-if="alert.metric.threshold !== undefined" class="metric-threshold">
        limite: {{ alert.metric.threshold }}
      </span>
    </div>

    <!-- Items -->
    <div v-if="alert.items && alert.items.length > 0" class="alert-card__items">
      <button class="items-toggle" @click="showItems = !showItems">
        <span>{{ alert.items.length }} {{ alert.items.length === 1 ? 'item afetado' : 'itens afetados' }}</span>
        <span class="items-toggle__arrow" :class="{ 'items-toggle__arrow--open': showItems }">▼</span>
      </button>
      <div v-if="showItems" class="items-list">
        <div
          v-for="(item, idx) in visibleItems"
          :key="idx"
          class="item-row"
        >
          <span
            v-for="(val, key) in item"
            :key="key"
            class="item-field"
          >
            <span class="item-field__key">{{ key }}:</span>
            <span class="item-field__val">{{ val }}</span>
          </span>
        </div>
        <p v-if="alert.items.length > ITEMS_PREVIEW" class="items-more">
          + {{ alert.items.length - ITEMS_PREVIEW }} mais...
        </p>
      </div>
    </div>

    <!-- Payload -->
    <div v-if="alert.payload" class="alert-card__payload">
      <button class="items-toggle" @click="showPayload = !showPayload">
        <span>Payload</span>
        <span class="items-toggle__arrow" :class="{ 'items-toggle__arrow--open': showPayload }">▼</span>
      </button>
      <pre v-if="showPayload" class="payload-json">{{ JSON.stringify(alert.payload, null, 2) }}</pre>
    </div>

    <!-- Footer: dedup_key -->
    <div class="alert-card__footer">
      <span class="dedup-key">{{ alert.dedup_key }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import type { Alert } from '@/types/alert'

const props = defineProps<{ alert: Alert }>()
defineEmits<{ dismiss: [id: string] }>()

const ITEMS_PREVIEW = 3
const showItems = ref(false)
const showPayload = ref(false)

const visibleItems = computed(() => props.alert.items?.slice(0, ITEMS_PREVIEW) ?? [])

const severityIcon = computed(() => {
  const icons = { critical: '🔴', warning: '⚠️', info: 'ℹ️' }
  return icons[props.alert.severity]
})

const severityLabel = computed(() => {
  const labels = { critical: 'Crítico', warning: 'Atenção', info: 'Info' }
  return labels[props.alert.severity]
})

const formattedTime = computed(() => {
  if (!props.alert.received_at) return ''
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(props.alert.received_at))
})
</script>

<style scoped>
.alert-card {
  background: var(--card-bg);
  border-radius: 10px;
  border-left: 4px solid var(--severity-color);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.18);
  padding: 16px 20px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  transition: box-shadow 0.2s;
}
.alert-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.28);
}

.alert-card--critical { --severity-color: #ef4444; }
.alert-card--warning  { --severity-color: #f59e0b; }
.alert-card--info     { --severity-color: #3b82f6; }

/* Header */
.alert-card__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 8px;
}
.alert-card__header-left {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}
.alert-card__header-right {
  display: flex;
  align-items: center;
  gap: 10px;
}

.severity-badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.severity-badge--critical { background: rgba(239,68,68,0.18); color: #ef4444; }
.severity-badge--warning  { background: rgba(245,158,11,0.18); color: #f59e0b; }
.severity-badge--info     { background: rgba(59,130,246,0.18); color: #60a5fa; }

.category-tag,
.type-tag {
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 11px;
  font-weight: 600;
  background: rgba(255,255,255,0.08);
  color: var(--text-muted);
  font-family: monospace;
}

.timestamp {
  font-size: 11px;
  color: var(--text-muted);
}

.dismiss-btn {
  background: none;
  border: none;
  color: var(--text-muted);
  cursor: pointer;
  font-size: 14px;
  padding: 2px 6px;
  border-radius: 4px;
  line-height: 1;
  transition: background 0.15s, color 0.15s;
}
.dismiss-btn:hover {
  background: rgba(255,255,255,0.1);
  color: var(--text-primary);
}

/* Body */
.alert-card__title {
  margin: 0;
  font-size: 15px;
  font-weight: 600;
  color: var(--text-primary);
  line-height: 1.4;
}
.alert-card__message {
  margin: 4px 0 0;
  font-size: 13px;
  color: var(--text-secondary);
  line-height: 1.5;
}
.alert-card__source {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-top: 6px;
}
.source-label {
  font-size: 11px;
  color: var(--text-muted);
}
.source-value {
  font-size: 11px;
  background: rgba(255,255,255,0.06);
  padding: 1px 6px;
  border-radius: 3px;
  color: var(--text-secondary);
}

/* Metric */
.alert-card__metric {
  display: flex;
  align-items: baseline;
  gap: 6px;
  padding: 10px 14px;
  background: rgba(255,255,255,0.05);
  border-radius: 8px;
}
.metric-value {
  font-size: 28px;
  font-weight: 700;
  color: var(--severity-color);
  line-height: 1;
}
.metric-unit {
  font-size: 13px;
  color: var(--text-secondary);
}
.metric-threshold {
  margin-left: auto;
  font-size: 11px;
  color: var(--text-muted);
}

/* Items */
.items-toggle {
  background: none;
  border: 1px solid rgba(255,255,255,0.1);
  border-radius: 6px;
  padding: 5px 12px;
  font-size: 12px;
  color: var(--text-secondary);
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
  justify-content: space-between;
  transition: background 0.15s;
}
.items-toggle:hover { background: rgba(255,255,255,0.06); }

.items-toggle__arrow {
  font-size: 10px;
  transition: transform 0.2s;
  display: inline-block;
}
.items-toggle__arrow--open { transform: rotate(180deg); }

.items-list {
  margin-top: 8px;
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.item-row {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  padding: 8px 12px;
  background: rgba(255,255,255,0.04);
  border-radius: 6px;
  font-size: 12px;
}
.item-field {
  display: flex;
  gap: 4px;
}
.item-field__key {
  color: var(--text-muted);
  font-family: monospace;
}
.item-field__val {
  color: var(--text-secondary);
  font-family: monospace;
  font-weight: 600;
}
.items-more {
  font-size: 11px;
  color: var(--text-muted);
  margin: 4px 0 0;
  text-align: center;
}

/* Payload */
.payload-json {
  margin-top: 8px;
  background: rgba(0,0,0,0.3);
  border-radius: 6px;
  padding: 12px;
  font-size: 11px;
  color: var(--text-secondary);
  overflow-x: auto;
  white-space: pre-wrap;
  word-break: break-all;
  line-height: 1.6;
}

/* Footer */
.alert-card__footer {
  padding-top: 8px;
  border-top: 1px solid rgba(255,255,255,0.07);
}
.dedup-key {
  font-size: 10px;
  font-family: monospace;
  color: var(--text-muted);
}
</style>
