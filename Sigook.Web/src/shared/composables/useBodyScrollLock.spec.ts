import { afterEach, describe, expect, it } from 'vitest';
import { effectScope } from 'vue';
import { lockScroll, unlockScroll, useBodyScrollLock } from '@/shared/composables/useBodyScrollLock';

function setup() {
  const scope = effectScope();
  const api = scope.run(() => useBodyScrollLock());
  if (!api) throw new Error('Scope did not run');
  return { ...api, scope };
}

describe('useBodyScrollLock', () => {
  afterEach(() => {
    for (let i = 0; i < 10; i += 1) unlockScroll();
    document.body.style.overflow = '';
  });

  it('hides body overflow while locked and restores the previous value', () => {
    document.body.style.overflow = 'scroll';
    lockScroll();
    expect(document.body.style.overflow).toBe('hidden');
    unlockScroll();
    expect(document.body.style.overflow).toBe('scroll');
  });

  it('keeps the body locked until every lock is released', () => {
    lockScroll();
    lockScroll();
    unlockScroll();
    expect(document.body.style.overflow).toBe('hidden');
    unlockScroll();
    expect(document.body.style.overflow).toBe('');
  });

  it('does not go below zero locks on extra unlocks', () => {
    unlockScroll();
    unlockScroll();
    lockScroll();
    expect(document.body.style.overflow).toBe('hidden');
    unlockScroll();
    expect(document.body.style.overflow).toBe('');
  });

  it('counts a composable lock only once no matter how often it is called', () => {
    const { lock, unlock } = setup();
    lock();
    lock();
    unlock();
    expect(document.body.style.overflow).toBe('');
  });

  it('ignores unlock when the composable does not hold a lock', () => {
    lockScroll();
    const { unlock } = setup();
    unlock();
    expect(document.body.style.overflow).toBe('hidden');
  });

  it('shares the lock count between independent holders', () => {
    const first = setup();
    const second = setup();
    first.lock();
    second.lock();
    first.unlock();
    expect(document.body.style.overflow).toBe('hidden');
    second.unlock();
    expect(document.body.style.overflow).toBe('');
  });

  it('releases its lock when the scope is disposed', () => {
    const { lock, scope } = setup();
    lock();
    expect(document.body.style.overflow).toBe('hidden');
    scope.stop();
    expect(document.body.style.overflow).toBe('');
  });
});
