import { ref, onMounted, onBeforeUnmount, type Ref } from 'vue'

/**
 * Track a template element's content-box size with a ResizeObserver.
 *
 * Bind the returned `el` as a template ref; `width`/`height` update on every
 * resize (rAF-debounced to dodge the "ResizeObserver loop" warning). Used by
 * the SVG charts that need real pixels (e.g. SalesBarChart's scaleBand range).
 *
 * @param fallbackWidth   width used before the first measure / when clientWidth is 0.
 * @param fallbackHeight  height used before the first measure / when clientHeight is 0.
 */
export interface UseElementSizeReturn {
  el: Ref<HTMLElement | null>
  width: Ref<number>
  height: Ref<number>
}

export function useElementSize(
  fallbackWidth = 0,
  fallbackHeight = 0
): UseElementSizeReturn {
  const el = ref<HTMLElement | null>(null)
  const width = ref(fallbackWidth)
  const height = ref(fallbackHeight)
  let observer: ResizeObserver | null = null
  let frame = 0

  const measure = (): void => {
    const node = el.value
    if (!node) return
    width.value = node.clientWidth || fallbackWidth
    height.value = node.clientHeight || fallbackHeight
  }

  onMounted(() => {
    if (!el.value) return
    measure()
    if (typeof ResizeObserver === 'undefined') return
    observer = new ResizeObserver(() => {
      cancelAnimationFrame(frame)
      frame = requestAnimationFrame(measure)
    })
    observer.observe(el.value)
  })

  onBeforeUnmount(() => {
    cancelAnimationFrame(frame)
    observer?.disconnect()
  })

  return { el, width, height }
}
