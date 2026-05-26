import { ref, readonly, type Ref } from 'vue'

/**
 * Module-scoped state — acts as a tiny singleton store so any V2 component
 * can open / close the WorkerRegisterModal without dragging in Pinia.
 *
 * `context` is optional metadata (currently just a job slug) that callers
 * can pass in via `open()`; the modal reads it to display "Applying for: X".
 */
export interface WorkerRegisterModalContext {
  /** Slug of the job the user is applying to, if any. */
  readonly jobSlug?: string
  /** Human-readable role title (shown in the modal header). */
  readonly jobTitle?: string
}

const isOpen = ref(false)
const context = ref<WorkerRegisterModalContext | null>(null)

function open(ctx?: WorkerRegisterModalContext): void {
  context.value = ctx ?? null
  isOpen.value = true
  // Lock body scroll while the modal is open
  if (typeof document !== 'undefined') {
    document.body.style.overflow = 'hidden'
  }
}

function close(): void {
  isOpen.value = false
  context.value = null
  if (typeof document !== 'undefined') {
    document.body.style.overflow = ''
  }
}

export function useWorkerRegisterModal(): {
  readonly isOpen: Readonly<Ref<boolean>>
  readonly context: Readonly<Ref<WorkerRegisterModalContext | null>>
  open: (ctx?: WorkerRegisterModalContext) => void
  close: () => void
} {
  return {
    isOpen: readonly(isOpen),
    context: readonly(context),
    open,
    close,
  }
}
