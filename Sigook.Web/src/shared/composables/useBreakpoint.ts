import { computed, ref, type ComputedRef } from 'vue'

const mobileQuery = window.matchMedia('(max-width: 767px)')
const touchQuery = window.matchMedia('(max-width: 1023px)')
const compactDesktopQuery = window.matchMedia('(min-width: 1024px) and (max-width: 1439px)')

const mobile = ref(mobileQuery.matches)
const touch = ref(touchQuery.matches)
const compactDesktop = ref(compactDesktopQuery.matches)

mobileQuery.addEventListener('change', (e) => {
  mobile.value = e.matches
})
touchQuery.addEventListener('change', (e) => {
  touch.value = e.matches
})
compactDesktopQuery.addEventListener('change', (e) => {
  compactDesktop.value = e.matches
})

export interface Breakpoint {
  isMobile: ComputedRef<boolean>
  isTablet: ComputedRef<boolean>
  isDesktop: ComputedRef<boolean>
  isCompactDesktop: ComputedRef<boolean>
  isTouch: ComputedRef<boolean>
}

export function useBreakpoint(): Breakpoint {
  return {
    isMobile: computed(() => mobile.value),
    isTablet: computed(() => touch.value && !mobile.value),
    isDesktop: computed(() => !touch.value),
    isCompactDesktop: computed(() => compactDesktop.value),
    isTouch: computed(() => touch.value),
  }
}
