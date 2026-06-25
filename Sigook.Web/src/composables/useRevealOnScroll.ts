import { ref, onMounted, onUnmounted, type Ref } from 'vue'

/**
 * Fade-up-on-scroll reveal driven by a single IntersectionObserver.
 *
 * Bind the returned `el` as a template ref and toggle a CSS class off
 * `visible`; the component keeps its own entrance transition/animation.
 *
 * Used across the landing cards (Primary/Secondary/Tertiary) and the
 * Numbers / WhyChooseUs / OurHistory sections.
 *
 * @param options.threshold   IntersectionObserver threshold (default 0.15).
 * @param options.rootMargin  IntersectionObserver rootMargin (default '0px 0px -10% 0px').
 * @param options.once        Reveal a single time and disconnect; when false (default)
 *                            `visible` tracks intersection both ways (fades out on exit).
 */
export interface UseRevealOnScrollOptions {
  threshold?: number
  rootMargin?: string
  once?: boolean
}

export interface UseRevealOnScrollReturn {
  /** Template ref to the element being observed. */
  el: Ref<HTMLElement | null>
  /** True while the element is intersecting (or after first reveal when `once`). */
  visible: Ref<boolean>
}

export function useRevealOnScroll(
  options: UseRevealOnScrollOptions = {}
): UseRevealOnScrollReturn {
  const { threshold = 0.15, rootMargin = '0px 0px -10% 0px', once = false } = options

  const el = ref<HTMLElement | null>(null)
  const visible = ref(false)
  let observer: IntersectionObserver | null = null

  onMounted(() => {
    if (!el.value || typeof IntersectionObserver === 'undefined') return
    observer = new IntersectionObserver(
      (entries) => {
        if (once) {
          if (entries[0].isIntersecting) {
            visible.value = true
            observer?.disconnect()
          }
        } else {
          visible.value = entries[0].isIntersecting
        }
      },
      { threshold, rootMargin }
    )
    observer.observe(el.value)
  })

  onUnmounted(() => observer?.disconnect())

  return { el, visible }
}
