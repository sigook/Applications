import { ref, watch, onMounted, onBeforeUnmount, type Ref } from 'vue'

/** Cubic ease-out — fast start, gentle settle. */
const easeOutCubic = (t: number): number => 1 - Math.pow(1 - t, 3)

const prefersReducedMotion = (): boolean =>
  typeof window !== 'undefined' &&
  typeof window.matchMedia === 'function' &&
  window.matchMedia('(prefers-reduced-motion: reduce)').matches

/**
 * Animate a number toward a reactive target with a self-contained rAF tween —
 * no d3-transition, no dependency. Animates on mount (from `initial`) and on
 * every target change. Snaps instantly when the user prefers reduced motion.
 *
 * Used for the SalesGoalDonut sweep, where the arc `d` attribute is not
 * CSS-transitionable.
 *
 * @param target    reactive value to chase.
 * @param duration  tween length in ms (default 600).
 * @param initial   starting value on mount (default 0).
 */
export function useTween(target: Ref<number>, duration = 600, initial = 0): Ref<number> {
  const current = ref(initial)
  let frame = 0

  const animateTo = (to: number): void => {
    cancelAnimationFrame(frame)
    if (duration <= 0 || prefersReducedMotion()) {
      current.value = to
      return
    }
    const from = current.value
    let startTime = 0
    const step = (now: number): void => {
      if (!startTime) startTime = now
      const t = Math.min(1, (now - startTime) / duration)
      current.value = from + (to - from) * easeOutCubic(t)
      if (t < 1) frame = requestAnimationFrame(step)
    }
    frame = requestAnimationFrame(step)
  }

  onMounted(() => animateTo(target.value))
  watch(target, (to) => animateTo(to))
  onBeforeUnmount(() => cancelAnimationFrame(frame))

  return current
}
