import { watch, nextTick, onScopeDispose, type Ref } from 'vue'

const FOCUSABLE = [
  'a[href]',
  'button:not([disabled])',
  'input:not([disabled]):not([type="hidden"])',
  'select:not([disabled])',
  'textarea:not([disabled])',
  '[tabindex]:not([tabindex="-1"])',
].join(',')

export function useFocusTrap(active: Readonly<Ref<boolean>>, container: Ref<HTMLElement | null>) {
  let previouslyFocused: HTMLElement | null = null

  function focusable(): HTMLElement[] {
    if (!container.value) return []
    return Array.from(container.value.querySelectorAll<HTMLElement>(FOCUSABLE)).filter(
      (el) => el.offsetParent !== null || el === document.activeElement,
    )
  }

  function onKeydown(e: KeyboardEvent): void {
    if (e.key !== 'Tab' || !container.value) return
    const items = focusable()
    if (items.length === 0) {
      e.preventDefault()
      container.value.focus()
      return
    }
    const first = items[0]
    const last = items[items.length - 1]
    const activeEl = document.activeElement as HTMLElement | null

    if (e.shiftKey && (activeEl === first || !container.value.contains(activeEl))) {
      e.preventDefault()
      last.focus()
    } else if (!e.shiftKey && activeEl === last) {
      e.preventDefault()
      first.focus()
    }
  }

  watch(active, async (open) => {
    if (open) {
      previouslyFocused = document.activeElement as HTMLElement | null
      await nextTick()
      const items = focusable()
      ;(items[0] ?? container.value)?.focus?.()
      document.addEventListener('keydown', onKeydown, true)
    } else {
      document.removeEventListener('keydown', onKeydown, true)
      previouslyFocused?.focus?.()
      previouslyFocused = null
    }
  })

  onScopeDispose(() => document.removeEventListener('keydown', onKeydown, true))
}
