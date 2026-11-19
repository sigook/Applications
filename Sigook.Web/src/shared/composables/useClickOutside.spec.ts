import { afterEach, describe, expect, it, vi } from 'vitest';
import { effectScope, nextTick, ref, type EffectScope } from 'vue';
import { useClickOutside } from '@/shared/composables/useClickOutside';

const scopes: EffectScope[] = [];

function setup() {
  const rootEl = document.createElement('div');
  const inside = document.createElement('span');
  rootEl.appendChild(inside);
  const ignoredEl = document.createElement('div');
  const outside = document.createElement('button');
  document.body.append(rootEl, ignoredEl, outside);

  const root = ref<HTMLElement | null>(rootEl);
  const ignored = ref<HTMLElement | null>(ignoredEl);
  const active = ref(false);
  const onOutside = vi.fn();
  const scope = effectScope();
  scopes.push(scope);
  scope.run(() => { useClickOutside(root, active, onOutside, [ignored]); });
  return { root, ignored, active, onOutside, scope, inside, ignoredEl, outside };
}

describe('useClickOutside', () => {
  afterEach(() => {
    scopes.splice(0).forEach((scope) => { scope.stop(); });
  });

  it('does not listen until it becomes active', () => {
    const { outside, onOutside } = setup();
    outside.click();
    expect(onOutside).not.toHaveBeenCalled();
  });

  it('calls the handler for clicks outside the root once active', async () => {
    const { active, outside, onOutside } = setup();
    active.value = true;
    await nextTick();
    outside.click();
    expect(onOutside).toHaveBeenCalledTimes(1);
  });

  it('ignores clicks inside the root or the ignored elements', async () => {
    const { active, inside, ignoredEl, onOutside } = setup();
    active.value = true;
    await nextTick();
    inside.click();
    ignoredEl.click();
    expect(onOutside).not.toHaveBeenCalled();
  });

  it('still fires when the root and ignored refs are empty', async () => {
    const { active, root, ignored, inside, onOutside } = setup();
    root.value = null;
    ignored.value = null;
    active.value = true;
    await nextTick();
    inside.click();
    expect(onOutside).toHaveBeenCalledTimes(1);
  });

  it('works without an ignore list', async () => {
    const rootEl = document.createElement('div');
    const outside = document.createElement('button');
    document.body.append(rootEl, outside);
    const active = ref(false);
    const onOutside = vi.fn();
    const scope = effectScope();
    scope.run(() => { useClickOutside(ref<HTMLElement | null>(rootEl), active, onOutside); });
    active.value = true;
    await nextTick();
    outside.click();
    expect(onOutside).toHaveBeenCalledTimes(1);
    scope.stop();
  });

  it('stops listening when deactivated', async () => {
    const { active, outside, onOutside } = setup();
    active.value = true;
    await nextTick();
    active.value = false;
    await nextTick();
    outside.click();
    expect(onOutside).not.toHaveBeenCalled();
  });

  it('removes the capture listener when the scope is disposed', async () => {
    const removeSpy = vi.spyOn(document, 'removeEventListener');
    const { active, outside, onOutside, scope } = setup();
    active.value = true;
    await nextTick();
    scope.stop();
    outside.click();
    expect(onOutside).not.toHaveBeenCalled();
    expect(removeSpy).toHaveBeenCalledWith('click', expect.any(Function), true);
  });
});
