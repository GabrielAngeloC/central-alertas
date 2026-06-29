import { ref, watch } from 'vue'

const collapsed = ref(true)

watch(
  collapsed,
  (val) => document.documentElement.style.setProperty('--sidebar-w', val ? '64px' : '220px'),
  { immediate: true },
)

export function useSidebar() {
  return { collapsed }
}
