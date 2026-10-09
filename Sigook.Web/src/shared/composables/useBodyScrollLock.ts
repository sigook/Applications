import { onScopeDispose } from 'vue'

let lockCount = 0
let previousOverflow = ''

export function lockScroll(): void {
  if (typeof document === 'undefined') return
  if (lockCount === 0) {
    previousOverflow = document.body.style.overflow
    document.body.style.overflow = 'hidden'
  }
  lockCount += 1
}

export function unlockScroll(): void {
  if (typeof document === 'undefined') return
  lockCount = Math.max(0, lockCount - 1)
  if (lockCount === 0) {
    document.body.style.overflow = previousOverflow
  }
}

export function useBodyScrollLock() {
  let held = false

  function lock(): void {
    if (held) return
    held = true
    lockScroll()
  }

  function unlock(): void {
    if (!held) return
    held = false
    unlockScroll()
  }

  onScopeDispose(() => unlock())

  return { lock, unlock }
}
