import { nextTick, onScopeDispose } from 'vue';

export interface DropdownReveal {
  reveal: (anchor: HTMLElement | null | undefined, active: boolean) => Promise<void>;
  release: () => void;
}

const GUTTER = 12;
const SPACER_ATTR = 'data-dropdown-spacer';

function findScroller(el: HTMLElement): HTMLElement | null {
  let node = el.parentElement;
  while (node && node !== document.body) {
    const { overflowY } = getComputedStyle(node);
    if (overflowY === 'auto' || overflowY === 'scroll') return node;
    node = node.parentElement;
  }
  return null;
}

export function useDropdownReveal(): DropdownReveal {
  let spacer: HTMLElement | null = null;

  function release(): void {
    spacer?.remove();
    spacer = null;
  }

  async function reveal(anchor: HTMLElement | null | undefined, active: boolean): Promise<void> {
    release();
    if (!active || !anchor) return;

    const host = findScroller(anchor);
    if (!host) return;

    await nextTick();
    const menu = anchor.querySelector<HTMLElement>('.dropdown-menu');
    if (!menu) return;

    const hostRect = host.getBoundingClientRect();
    const overflow = menu.getBoundingClientRect().bottom + GUTTER - hostRect.bottom;
    if (overflow <= 0) return;

    spacer = document.createElement('div');
    spacer.setAttribute(SPACER_ATTR, '');
    spacer.style.cssText = `flex:none;height:${overflow}px`;
    host.appendChild(spacer);

    const anchorOffset = anchor.getBoundingClientRect().top - hostRect.top;
    host.scrollTo({ top: host.scrollTop + Math.min(overflow, Math.max(anchorOffset, 0)), behavior: 'smooth' });
  }

  onScopeDispose(release);

  return { reveal, release };
}
