import { afterEach, describe, expect, it, vi } from 'vitest';
import { effectScope, type EffectScope } from 'vue';
import { useDropdownReveal } from '@/shared/composables/useDropdownReveal';

const scopes: EffectScope[] = [];

function rect(top: number, bottom: number): DOMRect {
  return { top, bottom, left: 0, right: 0, width: 0, height: bottom - top, x: 0, y: top, toJSON: () => ({}) };
}

interface Layout {
  hostTop?: number;
  hostBottom?: number;
  anchorTop?: number;
  menuBottom?: number;
  scrollTop?: number;
  overflowY?: string;
  withMenu?: boolean;
}

function build(layout: Layout = {}) {
  const { hostTop = 0, hostBottom = 400, anchorTop = 300, menuBottom = 450, scrollTop = 20, overflowY = 'auto', withMenu = true } = layout;
  const host = document.createElement('div');
  host.style.overflowY = overflowY;
  const wrapper = document.createElement('section');
  const anchor = document.createElement('div');
  host.appendChild(wrapper);
  wrapper.appendChild(anchor);
  document.body.appendChild(host);

  let menu: HTMLElement | null = null;
  if (withMenu) {
    menu = document.createElement('div');
    menu.className = 'dropdown-menu';
    anchor.appendChild(menu);
    vi.spyOn(menu, 'getBoundingClientRect').mockReturnValue(rect(anchorTop, menuBottom));
  }
  vi.spyOn(host, 'getBoundingClientRect').mockReturnValue(rect(hostTop, hostBottom));
  vi.spyOn(anchor, 'getBoundingClientRect').mockReturnValue(rect(anchorTop, anchorTop + 30));
  host.scrollTop = scrollTop;
  const scrollTo = vi.fn();
  host.scrollTo = scrollTo;
  return { host, anchor, menu, scrollTo };
}

function setup() {
  const scope = effectScope();
  scopes.push(scope);
  const api = scope.run(() => useDropdownReveal());
  if (!api) throw new Error('Scope did not run');
  return { ...api, scope };
}

function spacers(host: HTMLElement): HTMLElement[] {
  return Array.from(host.querySelectorAll<HTMLElement>('[data-dropdown-spacer]'));
}

describe('useDropdownReveal', () => {
  afterEach(() => {
    scopes.splice(0).forEach((scope) => { scope.stop(); });
  });

  it('adds a spacer sized to the overflow plus gutter and scrolls the host', async () => {
    const { host, anchor, scrollTo } = build();
    const { reveal } = setup();
    await reveal(anchor, true);

    const [spacer] = spacers(host);
    expect(spacer.style.height).toBe('62px');
    expect(spacer.style.flexGrow).toBe('0');
    expect(host.lastElementChild).toBe(spacer);
    expect(scrollTo).toHaveBeenCalledWith({ top: host.scrollTop + 62, behavior: 'smooth' });
  });

  it('caps the scroll at the anchor offset inside the host', async () => {
    const { host, anchor, scrollTo } = build({ hostTop: 100, anchorTop: 130, menuBottom: 500 });
    const { reveal } = setup();
    await reveal(anchor, true);
    expect(spacers(host)[0].style.height).toBe('112px');
    expect(scrollTo).toHaveBeenCalledWith({ top: host.scrollTop + 30, behavior: 'smooth' });
  });

  it('never scrolls upward when the anchor sits above the host top', async () => {
    const { host, anchor, scrollTo } = build({ hostTop: 100, anchorTop: 50, menuBottom: 500 });
    const { reveal } = setup();
    await reveal(anchor, true);
    expect(scrollTo).toHaveBeenCalledWith({ top: host.scrollTop, behavior: 'smooth' });
  });

  it('does nothing when the menu already fits', async () => {
    const { host, anchor, scrollTo } = build({ menuBottom: 388 });
    const { reveal } = setup();
    await reveal(anchor, true);
    expect(spacers(host)).toHaveLength(0);
    expect(scrollTo).not.toHaveBeenCalled();
  });

  it('does nothing when inactive or without an anchor', async () => {
    const { host, anchor, scrollTo } = build();
    const { reveal } = setup();
    await reveal(anchor, false);
    await reveal(null, true);
    await reveal(undefined, true);
    expect(spacers(host)).toHaveLength(0);
    expect(scrollTo).not.toHaveBeenCalled();
  });

  it('does nothing when no ancestor scrolls', async () => {
    const { host, anchor, scrollTo } = build({ overflowY: 'visible' });
    const { reveal } = setup();
    await reveal(anchor, true);
    expect(spacers(host)).toHaveLength(0);
    expect(scrollTo).not.toHaveBeenCalled();
  });

  it('finds a scroll container using overflow scroll', async () => {
    const { host, anchor } = build({ overflowY: 'scroll' });
    const { reveal } = setup();
    await reveal(anchor, true);
    expect(spacers(host)).toHaveLength(1);
  });

  it('does nothing when the anchor has no dropdown menu', async () => {
    const { host, anchor, scrollTo } = build({ withMenu: false });
    const { reveal } = setup();
    await reveal(anchor, true);
    expect(spacers(host)).toHaveLength(0);
    expect(scrollTo).not.toHaveBeenCalled();
  });

  it('replaces the previous spacer on each reveal', async () => {
    const { host, anchor } = build();
    const { reveal } = setup();
    await reveal(anchor, true);
    await reveal(anchor, true);
    expect(spacers(host)).toHaveLength(1);
  });

  it('removes the spacer on release, on close and on scope dispose', async () => {
    const { host, anchor } = build();
    const { reveal, release, scope } = setup();

    await reveal(anchor, true);
    release();
    expect(spacers(host)).toHaveLength(0);

    await reveal(anchor, true);
    await reveal(anchor, false);
    expect(spacers(host)).toHaveLength(0);

    await reveal(anchor, true);
    scope.stop();
    expect(spacers(host)).toHaveLength(0);
  });
});
