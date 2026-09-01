import { computed, ref, type ComputedRef } from 'vue'

const mobileQuery = window.matchMedia('(max-width: 767px)')
const touchQuery = window.matchMedia('(max-width: 1023px)')

const mobile = ref(mobileQuery.matches)
const touch = ref(touchQuery.matches)

mobileQuery.addEventListener('change', (e) => {
  mobile.value = e.matches
})
touchQuery.addEventListener('change', (e) => {
  touch.value = e.matches
})

export interface Breakpoint {
  isMobile: ComputedRef<boolean>
  isTablet: ComputedRef<boolean>
  isDesktop: ComputedRef<boolean>
  isTouch: ComputedRef<boolean>
}

export function useBreakpoint(): Breakpoint {
  return {
    isMobile: computed(() => mobile.value),
    isTablet: computed(() => touch.value && !mobile.value),
    isDesktop: computed(() => !touch.value),
    isTouch: computed(() => touch.value),
  }
}
