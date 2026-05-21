declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<{}, {}, any>
  export default component
}

declare global {
  const defineProps: typeof import('vue')['defineProps']
  const defineEmits: typeof import('vue')['defineEmits']
  const defineExpose: typeof import('vue')['defineExpose']
  const defineOptions: typeof import('vue')['defineOptions']
  const defineSlots: typeof import('vue')['defineSlots']
  const withDefaults: typeof import('vue')['withDefaults']
}
