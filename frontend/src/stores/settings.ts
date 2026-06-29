import { defineStore } from 'pinia'
import { ref, watch } from 'vue'

export interface TelegramConfig {
  enabled: boolean
  botToken: string
  chatId: string
  onCritical: boolean
  onWarning: boolean
  onInfo: boolean
}

export interface EmailConfig {
  enabled: boolean
  smtpHost: string
  smtpPort: number
  user: string
  password: string
  recipients: string
  onCritical: boolean
  onWarning: boolean
  onInfo: boolean
}

export interface SoundConfig {
  enabled: boolean
  volume: number
  tone: 'beep' | 'chime' | 'alert'
  onCritical: boolean
  onWarning: boolean
  onInfo: boolean
}

export interface AppConfig {
  enabled: boolean
  onCritical: boolean
  onWarning: boolean
  onInfo: boolean
}

const STORAGE_KEY = 'central-alertas:settings'

const DEFAULTS = {
  telegram: {
    enabled: false,
    botToken: '',
    chatId: '',
    onCritical: true,
    onWarning: true,
    onInfo: false,
  } as TelegramConfig,
  email: {
    enabled: false,
    smtpHost: '',
    smtpPort: 587,
    user: '',
    password: '',
    recipients: '',
    onCritical: true,
    onWarning: false,
    onInfo: false,
  } as EmailConfig,
  sound: {
    enabled: false,
    volume: 70,
    tone: 'beep' as const,
    onCritical: true,
    onWarning: true,
    onInfo: false,
  } as SoundConfig,
  app: {
    enabled: false,
    onCritical: true,
    onWarning: true,
    onInfo: false,
  } as AppConfig,
}

function loadFromStorage() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return DEFAULTS
    const parsed = JSON.parse(raw)
    return {
      telegram: { ...DEFAULTS.telegram, ...parsed.telegram },
      email: { ...DEFAULTS.email, ...parsed.email },
      sound: { ...DEFAULTS.sound, ...parsed.sound },
      app: { ...DEFAULTS.app, ...parsed.app },
    }
  } catch {
    return DEFAULTS
  }
}

export const useSettingsStore = defineStore('settings', () => {
  const saved = loadFromStorage()

  const telegram = ref<TelegramConfig>(saved.telegram)
  const email = ref<EmailConfig>(saved.email)
  const sound = ref<SoundConfig>(saved.sound)
  const app = ref<AppConfig>(saved.app)

  function save() {
    localStorage.setItem(
      STORAGE_KEY,
      JSON.stringify({
        telegram: telegram.value,
        email: email.value,
        sound: sound.value,
        app: app.value,
      }),
    )
  }

  // Auto-save on any change
  watch([telegram, email, sound, app], save, { deep: true })

  return { telegram, email, sound, app, save }
})
