<template>
  <div class="login">
    <div class="login__card">
      <div class="login__brand">
        <span class="login__logo">🔔</span>
        <h1 class="login__title">Central de Alertas</h1>
        <p class="login__subtitle">Acesse o painel administrativo</p>
      </div>

      <form class="login__form" @submit.prevent="submit">
        <div class="field">
          <label class="field__label">E-mail</label>
          <input
            class="field__input"
            v-model="email"
            type="email"
            autocomplete="username"
            placeholder="admin@centralalertas.local"
            required
          />
        </div>

        <div class="field">
          <label class="field__label">Senha</label>
          <input
            class="field__input"
            v-model="password"
            type="password"
            autocomplete="current-password"
            placeholder="••••••••"
            required
          />
        </div>

        <p v-if="auth.error" class="login__error">{{ auth.error }}</p>

        <button class="login__submit" type="submit" :disabled="auth.loading">
          {{ auth.loading ? 'Entrando…' : 'Entrar' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const email = ref('')
const password = ref('')

async function submit() {
  const ok = await auth.login(email.value, password.value)
  if (ok) {
    const redirect = (route.query.redirect as string) || '/'
    router.replace(redirect)
  }
}
</script>

<style scoped>
.login {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: radial-gradient(1200px 600px at 50% -10%, #131b2e 0%, #080c14 60%);
  padding: 24px;
}
.login__card {
  width: 100%;
  max-width: 380px;
  background: var(--card-bg, #0d1220);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 16px;
  padding: 36px 32px;
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.4);
}
.login__brand {
  text-align: center;
  margin-bottom: 28px;
}
.login__logo { font-size: 32px; }
.login__title {
  font-size: 22px;
  font-weight: 700;
  color: var(--text-primary, #e2e8f0);
  margin: 10px 0 4px;
}
.login__subtitle {
  font-size: 13px;
  color: var(--text-muted, #94a3b8);
  margin: 0;
}
.login__form { display: flex; flex-direction: column; gap: 16px; }

.field { display: flex; flex-direction: column; gap: 6px; }
.field__label {
  font-size: 12px;
  font-weight: 600;
  color: var(--text-muted, #94a3b8);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.field__input {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  padding: 10px 12px;
  font-size: 14px;
  color: var(--text-primary, #e2e8f0);
  font-family: inherit;
  outline: none;
  transition: border-color 0.15s;
}
.field__input:focus { border-color: #6366f1; }

.login__error {
  color: #f87171;
  font-size: 13px;
  margin: 0;
}
.login__submit {
  margin-top: 8px;
  background: #6366f1;
  color: white;
  border: none;
  border-radius: 8px;
  padding: 11px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.15s, opacity 0.15s;
  font-family: inherit;
}
.login__submit:hover:not(:disabled) { background: #4f46e5; }
.login__submit:disabled { opacity: 0.6; cursor: not-allowed; }
</style>
