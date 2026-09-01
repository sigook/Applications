import { ref, readonly, type Ref } from 'vue';
import { lockScroll, unlockScroll } from '@/composables/useBodyScrollLock';
import type { CandidateApplyModalContext } from '@/types/website';

const isOpen = ref(false);
const context = ref<CandidateApplyModalContext | null>(null);

function open(ctx?: CandidateApplyModalContext): void {
  context.value = ctx ?? null;
  if (isOpen.value) return;
  isOpen.value = true;
  lockScroll();
}

function close(): void {
  if (!isOpen.value) return;
  isOpen.value = false;
  context.value = null;
  unlockScroll();
}

export function useCandidateApplyModal(): {
  readonly isOpen: Readonly<Ref<boolean>>;
  readonly context: Readonly<Ref<CandidateApplyModalContext | null>>;
  open: (ctx?: CandidateApplyModalContext) => void;
  close: () => void;
} {
  return {
    isOpen: readonly(isOpen),
    context: readonly(context),
    open,
    close,
  };
}
