import { afterEach, describe, expect, it } from 'vitest';
import { effectScope, nextTick, ref, type EffectScope, type Ref } from 'vue';
import { useFocusTrap } from '@/shared/composables/useFocusTrap';

const FIELDS = `
  <button id="first">first</button>
  <input id="hidden" type="hidden" />
  <button id="disabled" disabled>disabled</button>
  <input id="middle" />
  <a id="last" href="#">last</a>
`;

const scopes: EffectScope[] = [];

function markRendered(root: HTMLElement, hiddenIds: string[]): void {
  root.querySelectorAll<HTMLElement>('*').forEach((el) => {
    Object.defineProperty(el, 'offsetParent', { configurable: true, value: hiddenIds.includes(el.id) ? null : root });
  });
}

function setup(innerHtml: string, hiddenIds: string[] = []) {
  const opener = document.createElement('button');
  const containerEl = document.createElement('div');
  containerEl.tabIndex = -1;
  containerEl.innerHTML = innerHtml;
  markRendered(containerEl, hiddenIds);
  document.body.append(opener, containerEl);
  const active = ref(false);
  const container = ref<HTMLElement | null>(containerEl);
  const scope = effectScope();
  scopes.push(scope);
  scope.run(() => { useFocusTrap(active, container); });
  return { opener, containerEl, active, container, scope };
}

function pressTab(shiftKey = false): KeyboardEvent {
  const event = new KeyboardEvent('keydown', { key: 'Tab', shiftKey, bubbles: true, cancelable: true });
  document.dispatchEvent(event);
  return event;
}

function byId(id: string): HTMLElement {
  const el = document.getElementById(id);
  if (!el) throw new Error(`Missing #${id}`);
  return el;
}

async function open(active: Ref<boolean>): Promise<void> {
  active.value = true;
  await nextTick();
  await nextTick();
}

describe('useFocusTrap', () => {
  afterEach(() => {
    scopes.splice(0).forEach((scope) => { scope.stop(); });
  });

  it('focuses the first focusable element when activated', async () => {
    const { opener, active } = setup(FIELDS);
    opener.focus();
    await open(active);
    expect(document.activeElement).toBe(byId('first'));
  });

  it('wraps forward from the last element to the first', async () => {
    const { active } = setup(FIELDS);
    await open(active);
    byId('last').focus();
    const event = pressTab();
    expect(event.defaultPrevented).toBe(true);
    expect(document.activeElement).toBe(byId('first'));
  });

  it('wraps backward from the first element to the last', async () => {
    const { active } = setup(FIELDS);
    await open(active);
    const event = pressTab(true);
    expect(event.defaultPrevented).toBe(true);
    expect(document.activeElement).toBe(byId('last'));
  });

  it('pulls focus to the last element on shift+tab from outside the container', async () => {
    const { opener, active } = setup(FIELDS);
    await open(active);
    opener.focus();
    pressTab(true);
    expect(document.activeElement).toBe(byId('last'));
  });

  it('lets the browser handle tab between middle elements', async () => {
    const { active } = setup(FIELDS);
    await open(active);
    byId('middle').focus();
    expect(pressTab().defaultPrevented).toBe(false);
    expect(pressTab(true).defaultPrevented).toBe(false);
    expect(document.activeElement).toBe(byId('middle'));
  });

  it('ignores keys other than tab', async () => {
    const { active } = setup(FIELDS);
    await open(active);
    byId('last').focus();
    const event = new KeyboardEvent('keydown', { key: 'Enter', cancelable: true });
    document.dispatchEvent(event);
    expect(event.defaultPrevented).toBe(false);
    expect(document.activeElement).toBe(byId('last'));
  });

  it('keeps focus on the container when it has nothing focusable', async () => {
    const { containerEl, active } = setup('<span>text</span>');
    await open(active);
    expect(document.activeElement).toBe(containerEl);
    const event = pressTab();
    expect(event.defaultPrevented).toBe(true);
    expect(document.activeElement).toBe(containerEl);
  });

  it('skips elements that are not rendered', async () => {
    const { active } = setup(FIELDS, ['first']);
    await open(active);
    expect(document.activeElement).toBe(byId('middle'));
  });

  it('does nothing on tab when the container ref is empty', async () => {
    const { active, container } = setup(FIELDS);
    await open(active);
    container.value = null;
    byId('last').focus();
    expect(pressTab().defaultPrevented).toBe(false);
  });

  it('restores focus to the previously focused element when deactivated', async () => {
    const { opener, active } = setup(FIELDS);
    opener.focus();
    await open(active);
    active.value = false;
    await nextTick();
    expect(document.activeElement).toBe(opener);
    byId('last').focus();
    expect(pressTab().defaultPrevented).toBe(false);
  });

  it('removes the keydown listener when the scope is disposed', async () => {
    const { active, scope } = setup(FIELDS);
    await open(active);
    scope.stop();
    byId('last').focus();
    expect(pressTab().defaultPrevented).toBe(false);
  });
});
