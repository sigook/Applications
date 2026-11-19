import { describe, expect, it, vi } from 'vitest';
import { useSwipe } from '@/shared/composables/useSwipe';

function pointer(clientX: number, clientY: number): PointerEvent {
  return { clientX, clientY } as PointerEvent;
}

function setup(threshold?: number) {
  const left = vi.fn();
  const right = vi.fn();
  const swipe = useSwipe(left, right, threshold === undefined ? undefined : { threshold });
  return { left, right, ...swipe };
}

describe('useSwipe', () => {
  it('fires the left callback on a horizontal swipe to the left', () => {
    const { left, right, onPointerDown, onPointerUp } = setup();
    onPointerDown(pointer(200, 100));
    onPointerUp(pointer(100, 110));
    expect(left).toHaveBeenCalledTimes(1);
    expect(right).not.toHaveBeenCalled();
  });

  it('fires the right callback on a horizontal swipe to the right', () => {
    const { left, right, onPointerDown, onPointerUp } = setup();
    onPointerDown(pointer(100, 100));
    onPointerUp(pointer(200, 90));
    expect(right).toHaveBeenCalledTimes(1);
    expect(left).not.toHaveBeenCalled();
  });

  it('ignores movements shorter than the default threshold of 40px', () => {
    const { left, right, onPointerDown, onPointerUp } = setup();
    onPointerDown(pointer(100, 100));
    onPointerUp(pointer(61, 100));
    expect(left).not.toHaveBeenCalled();
    expect(right).not.toHaveBeenCalled();
  });

  it('treats a movement exactly at the threshold as a swipe', () => {
    const { left, onPointerDown, onPointerUp } = setup();
    onPointerDown(pointer(100, 100));
    onPointerUp(pointer(60, 100));
    expect(left).toHaveBeenCalledTimes(1);
  });

  it('honours a custom threshold', () => {
    const { right, onPointerDown, onPointerUp } = setup(100);
    onPointerDown(pointer(0, 0));
    onPointerUp(pointer(80, 0));
    expect(right).not.toHaveBeenCalled();
    onPointerDown(pointer(0, 0));
    onPointerUp(pointer(120, 0));
    expect(right).toHaveBeenCalledTimes(1);
  });

  it('ignores mostly vertical gestures', () => {
    const { left, right, onPointerDown, onPointerUp } = setup();
    onPointerDown(pointer(100, 100));
    onPointerUp(pointer(40, 200));
    onPointerDown(pointer(100, 100));
    onPointerUp(pointer(160, 160));
    expect(left).not.toHaveBeenCalled();
    expect(right).not.toHaveBeenCalled();
  });

  it('ignores a pointer up without a preceding pointer down', () => {
    const { left, right, onPointerUp } = setup();
    onPointerUp(pointer(500, 0));
    expect(left).not.toHaveBeenCalled();
    expect(right).not.toHaveBeenCalled();
  });

  it('resets the start point after each gesture', () => {
    const { left, onPointerDown, onPointerUp } = setup();
    onPointerDown(pointer(200, 0));
    onPointerUp(pointer(100, 0));
    onPointerUp(pointer(0, 0));
    expect(left).toHaveBeenCalledTimes(1);
  });
});
