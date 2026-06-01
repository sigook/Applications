import { ref, computed, onMounted, onUnmounted, type Ref, type ComputedRef } from 'vue'

/**
 * Carousel state + auto-advance timer for any section with fading slides.
 *
 * Used by HeroSection and TestimonialsSection.
 *
 * @param items   List of slide items (or a reactive ref/getter returning one).
 * @param options Optional config.
 * @param options.intervalMs  Auto-advance interval in ms (default 5000).
 * @param options.autoStart   Start the timer on mount (default true).
 */
export interface UseCarouselOptions {
  intervalMs?: number
  autoStart?: boolean
}

export interface UseCarouselReturn<T> {
  /** Active slide index (0-based). */
  currentIndex: Ref<number>
  /** Currently visible slide item. */
  currentItem: ComputedRef<T>
  /** Total number of slides. */
  count: ComputedRef<number>
  /** Jump to a specific slide index. Resets the auto-advance timer. */
  goTo: (index: number) => void
  /** Advance to the next slide (wraps). */
  next: () => void
  /** Start (or restart) the auto-advance timer. */
  start: () => void
  /** Stop the auto-advance timer. */
  stop: () => void
}

export function useCarousel<T>(
  items: T[] | (() => T[]),
  options: UseCarouselOptions = {}
): UseCarouselReturn<T> {
  const { intervalMs = 5000, autoStart = true } = options

  const list = computed<T[]>(() =>
    typeof items === 'function' ? (items as () => T[])() : items
  )

  const count = computed(() => list.value.length)
  const currentIndex = ref(0)
  const currentItem = computed(() => list.value[currentIndex.value])

  let timer: ReturnType<typeof setInterval> | null = null

  function next() {
    if (count.value === 0) return
    currentIndex.value = (currentIndex.value + 1) % count.value
  }

  function start() {
    stop()
    timer = setInterval(next, intervalMs)
  }

  function stop() {
    if (timer) {
      clearInterval(timer)
      timer = null
    }
  }

  function goTo(index: number) {
    if (index < 0 || index >= count.value) return
    currentIndex.value = index
    start() // reset timer on manual navigation
  }

  onMounted(() => {
    if (autoStart) start()
  })

  onUnmounted(stop)

  return { currentIndex, currentItem, count, goTo, next, start, stop }
}
