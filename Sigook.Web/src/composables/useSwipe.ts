export interface UseSwipeOptions {
  threshold?: number
}

export interface UseSwipeReturn {
  onPointerDown: (event: PointerEvent) => void
  onPointerUp: (event: PointerEvent) => void
}

export function useSwipe(
  onSwipeLeft: () => void,
  onSwipeRight: () => void,
  options: UseSwipeOptions = {}
): UseSwipeReturn {
  const { threshold = 40 } = options

  let startX: number | null = null
  let startY: number | null = null

  function onPointerDown(event: PointerEvent): void {
    startX = event.clientX
    startY = event.clientY
  }

  function onPointerUp(event: PointerEvent): void {
    if (startX === null || startY === null) return
    const deltaX = event.clientX - startX
    const deltaY = event.clientY - startY
    startX = null
    startY = null
    if (Math.abs(deltaX) < threshold || Math.abs(deltaX) <= Math.abs(deltaY)) return
    if (deltaX < 0) {
      onSwipeLeft()
    } else {
      onSwipeRight()
    }
  }

  return { onPointerDown, onPointerUp }
}
