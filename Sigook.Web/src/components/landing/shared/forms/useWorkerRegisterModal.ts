import { ref, readonly, type Ref } from 'vue'
import { lockScroll, unlockScroll } from '@/composables/useBodyScrollLock'
export interface WorkerRegisterModalContext {
  readonly jobTitle?: string
  readonly jobNumber?: string
  readonly requestId?: string
}

const isOpen = ref(false)
const context = ref<WorkerRegisterModalContext | null>(null)

function open(ctx?: WorkerRegisterModalContext): void {
  context.value = ctx ?? null
  if (isOpen.value) return
  isOpen.value = true
  lockScroll()
}

function close(): void {
  if (!isOpen.value) return
  isOpen.value = false
  context.value = null
  unlockScroll()
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
