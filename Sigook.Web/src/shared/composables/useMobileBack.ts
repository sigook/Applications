import { computed, onUnmounted, shallowRef, watch, type ComputedRef } from 'vue'
import type { RouteLocationRaw } from 'vue-router'

interface BackEntry {
  owner: symbol
  to: RouteLocationRaw
}

const current = shallowRef<BackEntry | null>(null)

export function useMobileBack(): ComputedRef<RouteLocationRaw | null> {
  return computed(() => current.value?.to ?? null)
}

export function registerMobileBack(source: () => RouteLocationRaw | null): void {
  const owner = Symbol('mobile-back')

  watch(source, (to) => {
    if (to) {
      current.value = { owner, to }
    } else if (current.value?.owner === owner) {
      current.value = null
    }
  }, { immediate: true })

  onUnmounted(() => {
    if (current.value?.owner === owner) current.value = null
  })
}
