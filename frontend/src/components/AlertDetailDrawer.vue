<template>
  <Teleport to="body">
    <Transition name="drawer">
      <div v-if="alertId" class="drawer-overlay" @click.self="$emit('close')">
        <aside class="drawer">
          <button class="drawer__close" @click="$emit('close')" aria-label="Fechar">✕</button>

          <div v-if="loading" class="drawer__loading">Carregando…</div>
          <div v-else-if="error" class="drawer__error">{{ error }}</div>

          <template v-else-if="detail">
            <header class="drawer__header">
              <span class="drawer__sev" :class="`drawer__sev--${sev}`">{{ sevLabel }}</span>
              <h2 class="drawer__title">{{ detail.title }}</h2>
              <p v-if="detail.message" class="drawer__msg">{{ detail.message }}</p>
              <div class="drawer__meta">
                <code>{{ detail.source }}</code>
                <code>{{ detail.category }}</code>
                <code>{{ detail.type }}</code>
                <span v-if="detail.isEscalating" class="drawer__esc">📈 escalando</span>
              </div>
              <div class="drawer__dedup">dedup_key: <code>{{ detail.dedupKey }}</code></div>
            </header>

            <!-- Metric -->
            <div v-if="detail.metricValue != null" class="drawer__metric">
              <span class="drawer__metric-val">{{ detail.metricValue }} {{ detail.metricUnit }}</span>
              <span v-if="detail.metricThreshold != null" class="drawer__metric-th">
                limite: {{ detail.metricThreshold }}
              </span>
              <span class="drawer__occ">{{ detail.occurrenceCount }} ocorrência(s)</span>
            </div>

            <!-- Resolve -->
            <div v-if="detail.isActive" class="drawer__actions">
              <button class="btn btn--ghost" @click="resolve" :disabled="resolving">
                {{ resolving ? 'Resolvendo…' : 'Marcar como resolvido' }}
              </button>
              <span v-if="resolveMsg" class="drawer__resolvemsg">{{ resolveMsg }}</span>
            </div>

            <!-- Occurrences / metric evolution -->
            <section class="drawer__section">
              <h3 class="drawer__h3">Evolução do metric ({{ occurrences.length }})</h3>
              <ul class="occ-list">
                <li v-for="o in occurrences" :key="o.id" class="occ-item">
                  <span class="occ-time">{{ formatTime(o.receivedAt) }}</span>
                  <span class="occ-val">{{ o.metricValue != null ? `${o.metricValue} ${o.metricUnit ?? ''}` : '—' }}</span>
                </li>
              </ul>
            </section>

            <!-- Items with search -->
            <section v-if="itemsArray.length" class="drawer__section">
              <div class="drawer__section-head">
                <h3 class="drawer__h3">Itens ({{ filteredItems.length }}/{{ itemsArray.length }})</h3>
                <input v-model="itemSearch" class="drawer__search" placeholder="Buscar item (pedido, SKU…)" />
              </div>
              <div class="items-table">
                <table>
                  <thead>
                    <tr><th v-for="col in itemColumns" :key="col">{{ col }}</th></tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, i) in filteredItems" :key="i">
                      <td v-for="col in itemColumns" :key="col">{{ formatCell(item[col]) }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </section>

            <!-- Deliveries -->
            <section v-if="deliveries.length" class="drawer__section">
              <h3 class="drawer__h3">Entregas ({{ deliveries.length }})</h3>
              <ul class="deliv-list">
                <li v-for="d in deliveries" :key="d.id" class="deliv-item">
                  <span class="deliv-status" :class="`deliv-status--${d.status}`">{{ d.status }}</span>
                  <span class="deliv-ch">{{ d.channel }}</span>
                  <span class="deliv-dest">{{ d.notificationDestinationName ?? '—' }}</span>
                  <span class="deliv-time">{{ formatTime(d.attemptedAt) }}</span>
                </li>
              </ul>
            </section>

            <!-- Payload -->
            <section v-if="detail.payload" class="drawer__section">
              <h3 class="drawer__h3">Payload</h3>
              <pre class="drawer__pre">{{ prettyPayload }}</pre>
            </section>
          </template>
        </aside>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import {
  alertsApi,
  ApiError,
  type AlertDetailResponse,
  type AlertOccurrenceResponse,
  type AlertDeliveryResponse,
} from '@/api'

const props = defineProps<{ alertId: string | null }>()
const emit = defineEmits<{ (e: 'close'): void; (e: 'resolved'): void }>()

const loading = ref(false)
const error = ref<string | null>(null)
const detail = ref<AlertDetailResponse | null>(null)
const occurrences = ref<AlertOccurrenceResponse[]>([])
const deliveries = ref<AlertDeliveryResponse[]>([])
const itemSearch = ref('')
const resolving = ref(false)
const resolveMsg = ref<string | null>(null)

const sev = computed(() => {
  const v = (detail.value?.severity ?? '').toLowerCase()
  return v === 'critical' || v === 'warning' || v === 'info' ? v : 'info'
})
const sevLabel = computed(() => ({ critical: 'CRÍTICO', warning: 'ATENÇÃO', info: 'INFO' }[sev.value]))

const itemsArray = computed<Record<string, unknown>[]>(() => {
  const items = detail.value?.items
  return Array.isArray(items) ? (items as Record<string, unknown>[]) : []
})
const itemColumns = computed(() => {
  const cols = new Set<string>()
  for (const it of itemsArray.value) {
    Object.keys(it).forEach((k) => cols.add(k))
  }
  return Array.from(cols)
})
const filteredItems = computed(() => {
  const q = itemSearch.value.trim().toLowerCase()
  if (!q) return itemsArray.value
  return itemsArray.value.filter((it) =>
    Object.values(it).some((v) => String(v).toLowerCase().includes(q)),
  )
})
const prettyPayload = computed(() => {
  try {
    return JSON.stringify(detail.value?.payload, null, 2)
  } catch {
    return String(detail.value?.payload ?? '')
  }
})

function formatCell(v: unknown): string {
  if (v === null || v === undefined) return '—'
  if (typeof v === 'object') return JSON.stringify(v)
  return String(v)
}
function formatTime(iso: string): string {
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit',
  }).format(new Date(iso))
}

async function load(id: string) {
  loading.value = true
  error.value = null
  itemSearch.value = ''
  resolveMsg.value = null
  try {
    const [d, occ, dlv] = await Promise.all([
      alertsApi.byId(id),
      alertsApi.occurrences(id).catch(() => []),
      alertsApi.deliveries(id).catch(() => []),
    ])
    detail.value = d
    occurrences.value = occ
    deliveries.value = dlv
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Falha ao carregar o alerta.'
  } finally {
    loading.value = false
  }
}

async function resolve() {
  if (!props.alertId) return
  resolving.value = true
  resolveMsg.value = null
  try {
    await alertsApi.resolve(props.alertId, 'Resolvido pelo painel.')
    resolveMsg.value = 'Alerta resolvido.'
    emit('resolved')
  } catch (err) {
    resolveMsg.value = err instanceof ApiError ? err.message : 'Falha ao resolver.'
  } finally {
    resolving.value = false
  }
}

watch(
  () => props.alertId,
  (id) => {
    if (id) load(id)
  },
  { immediate: true },
)
</script>

<style scoped>
.drawer-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 300;
  display: flex;
  justify-content: flex-end;
}
.drawer {
  width: min(560px, 100%);
  height: 100%;
  background: var(--bg-surface);
  border-left: 1px solid var(--border-color);
  overflow-y: auto;
  padding: 28px 26px 60px;
  position: relative;
}
.drawer__close {
  position: absolute;
  top: 18px;
  right: 18px;
  background: none;
  border: none;
  color: var(--text-muted);
  font-size: 16px;
  cursor: pointer;
}
.drawer__close:hover { color: var(--text-primary); }
.drawer__loading, .drawer__error { color: var(--text-muted); padding: 40px 0; }
.drawer__error { color: #f87171; }

.drawer__header { margin-bottom: 18px; }
.drawer__sev {
  font-family: monospace;
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.06em;
  padding: 3px 9px;
  border-radius: 5px;
}
.drawer__sev--critical { color: #ef4444; background: rgba(239, 68, 68, 0.14); }
.drawer__sev--warning { color: #f59e0b; background: rgba(245, 158, 11, 0.14); }
.drawer__sev--info { color: #60a5fa; background: rgba(96, 165, 250, 0.16); }
.drawer__title { font-size: 19px; font-weight: 600; color: var(--text-primary); margin: 12px 0 6px; }
.drawer__msg { font-size: 14px; color: var(--text-secondary); margin: 0 0 10px; }
.drawer__meta { display: flex; flex-wrap: wrap; gap: 6px; align-items: center; margin-bottom: 6px; }
.drawer__meta code, .drawer__dedup code {
  font-family: monospace;
  font-size: 11.5px;
  color: var(--text-secondary);
  background: rgba(255, 255, 255, 0.05);
  padding: 1px 6px;
  border-radius: 4px;
}
.drawer__esc { font-size: 12px; color: #f59e0b; }
.drawer__dedup { font-size: 11.5px; color: var(--text-muted); }

.drawer__metric {
  display: flex;
  align-items: baseline;
  gap: 14px;
  flex-wrap: wrap;
  padding: 14px 0;
  border-top: 1px solid var(--border-color);
  border-bottom: 1px solid var(--border-color);
  margin-bottom: 16px;
}
.drawer__metric-val { font-size: 22px; font-weight: 700; color: var(--text-primary); font-variant-numeric: tabular-nums; }
.drawer__metric-th, .drawer__occ { font-size: 12px; color: var(--text-muted); }

.drawer__actions { display: flex; align-items: center; gap: 12px; margin-bottom: 20px; }
.drawer__resolvemsg { font-size: 12px; color: var(--text-secondary); }

.drawer__section { margin-bottom: 22px; }
.drawer__section-head { display: flex; align-items: center; justify-content: space-between; gap: 12px; margin-bottom: 10px; }
.drawer__h3 {
  font-size: 11px;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--text-secondary);
  margin: 0 0 10px;
}
.drawer__search {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid var(--border-color);
  border-radius: 6px;
  padding: 5px 10px;
  font-size: 12px;
  color: var(--text-primary);
  outline: none;
}

.occ-list, .deliv-list { list-style: none; margin: 0; padding: 0; }
.occ-item {
  display: flex;
  justify-content: space-between;
  padding: 6px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  font-size: 13px;
}
.occ-time { color: var(--text-muted); font-variant-numeric: tabular-nums; }
.occ-val { color: var(--text-primary); font-family: monospace; }

.items-table { overflow-x: auto; border: 1px solid var(--border-color); border-radius: 8px; }
.items-table table { width: 100%; border-collapse: collapse; font-size: 12.5px; }
.items-table th {
  text-align: left;
  font-family: monospace;
  font-size: 10.5px;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: var(--text-muted);
  padding: 8px 10px;
  border-bottom: 1px solid var(--border-color);
  background: var(--card-bg);
}
.items-table td {
  padding: 7px 10px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  color: var(--text-secondary);
  white-space: nowrap;
}
.items-table tr:last-child td { border-bottom: none; }

.deliv-item {
  display: grid;
  grid-template-columns: 80px 70px 1fr auto;
  gap: 10px;
  align-items: center;
  padding: 7px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  font-size: 12.5px;
}
.deliv-status { font-family: monospace; font-size: 11px; padding: 1px 7px; border-radius: 8px; text-align: center; }
.deliv-status--success { color: #4ade80; background: rgba(76, 175, 141, 0.14); }
.deliv-status--failed { color: #f87171; background: rgba(239, 68, 68, 0.12); }
.deliv-status--skipped { color: #94a3b8; background: rgba(148, 163, 184, 0.12); }
.deliv-ch { color: var(--text-secondary); }
.deliv-dest { color: var(--text-muted); overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.deliv-time { color: var(--text-muted); font-variant-numeric: tabular-nums; }

.drawer__pre {
  background: var(--bg-page);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 12px 14px;
  overflow-x: auto;
  font-family: monospace;
  font-size: 12px;
  color: var(--text-secondary);
  line-height: 1.55;
}

.btn {
  padding: 7px 16px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  border: none;
  font-family: inherit;
}
.btn--ghost {
  background: rgba(255, 255, 255, 0.07);
  color: var(--text-secondary);
  border: 1px solid var(--border-color);
}
.btn--ghost:hover:not(:disabled) { background: rgba(255, 255, 255, 0.12); }
.btn:disabled { opacity: 0.5; cursor: not-allowed; }

.drawer-enter-active, .drawer-leave-active { transition: opacity 0.2s; }
.drawer-enter-from, .drawer-leave-to { opacity: 0; }
</style>
