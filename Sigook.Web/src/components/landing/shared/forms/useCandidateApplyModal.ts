import { ref, readonly, type Ref } from 'vue'

/**
 * Module-scoped state — a tiny singleton store so any landing component can
 * open / close the CandidateApplyModal without dragging in Pinia.
 *
 * `context` is optional metadata (job title / number / request id) that callers
 * pass via `open()`; the modal reads it to display "Applying for: X" and to
 * attach the application to a specific Request.
 */
export interface CandidateApplyModalContext {
  /** Human-readable role title (shown in the modal header). */
  readonly jobTitle?: string
  /** Human-readable job number (e.g. the request NumberId), if any. */
  readonly jobNumber?: string
  /** Backend Request id the application should attach to, if any. */
  readonly requestId?: string
}

const isOpen = ref(false)
const context = ref<CandidateApplyModalContext | null>(null)

function open(ctx?: CandidateApplyModalContext): void {
  context.value = ctx ?? null
  isOpen.value = true
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

export function useCandidateApplyModal(): {
  readonly isOpen: Readonly<Ref<boolean>>
  readonly context: Readonly<Ref<CandidateApplyModalContext | null>>
  open: (ctx?: CandidateApplyModalContext) => void
  close: () => void
} {
  return {
    isOpen: readonly(isOpen),
    context: readonly(context),
    open,
    close,
  }
}
