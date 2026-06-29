import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'

// O frontend conversa com o backend .NET (Central de Alertas API).
// Em dev, fazemos proxy de /api -> VITE_API_URL para evitar problemas de CORS.
export default defineConfig(({ mode }) => {
  // envDir = pasta deste arquivo (raiz do projeto), sem depender de `process`.
  const env = loadEnv(mode, fileURLToPath(new URL('.', import.meta.url)), '')
  const apiTarget = env.VITE_API_URL || 'http://localhost:8080'

  return {
    plugins: [vue()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
    },
    server: {
      // Porta fixa para a origem do dev server ser previsível (casar com o CORS do backend).
      port: 5173,
      strictPort: true,
      proxy: {
        '/api': {
          target: apiTarget,
          changeOrigin: true,
          secure: false,
        },
      },
    },
  }
})
