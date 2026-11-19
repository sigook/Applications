import { afterEach, describe, expect, it } from 'vitest';
import { useCandidateApplyModal } from '@/modules/landing/composables/useCandidateApplyModal';

describe('useCandidateApplyModal', () => {
  afterEach(() => {
    useCandidateApplyModal().close();
    document.body.style.overflow = '';
  });

  it('starts closed without context', () => {
    const { isOpen, context } = useCandidateApplyModal();
    expect(isOpen.value).toBe(false);
    expect(context.value).toBeNull();
  });

  it('opens with a context and locks body scroll', () => {
    const { isOpen, context, open } = useCandidateApplyModal();
    open({ jobTitle: 'Driver', jobNumber: '42', requestId: 'r-1' });
    expect(isOpen.value).toBe(true);
    expect(context.value).toEqual({ jobTitle: 'Driver', jobNumber: '42', requestId: 'r-1' });
    expect(document.body.style.overflow).toBe('hidden');
  });

  it('opens without a context', () => {
    const { isOpen, context, open } = useCandidateApplyModal();
    open();
    expect(isOpen.value).toBe(true);
    expect(context.value).toBeNull();
  });

  it('closes, clears the context and restores body scroll', () => {
    document.body.style.overflow = 'auto';
    const { isOpen, context, open, close } = useCandidateApplyModal();
    open({ jobTitle: 'Driver' });
    close();
    expect(isOpen.value).toBe(false);
    expect(context.value).toBeNull();
    expect(document.body.style.overflow).toBe('auto');
  });

  it('updates the context without locking twice when opened again', () => {
    const { context, open, close } = useCandidateApplyModal();
    open({ jobTitle: 'Driver' });
    open({ jobTitle: 'Welder' });
    expect(context.value).toEqual({ jobTitle: 'Welder' });
    close();
    expect(document.body.style.overflow).toBe('');
  });

  it('ignores close when already closed', () => {
    document.body.style.overflow = 'scroll';
    const { isOpen, close } = useCandidateApplyModal();
    close();
    expect(isOpen.value).toBe(false);
    expect(document.body.style.overflow).toBe('scroll');
  });

  it('shares the modal state between callers', () => {
    useCandidateApplyModal().open({ requestId: 'r-9' });
    const other = useCandidateApplyModal();
    expect(other.isOpen.value).toBe(true);
    expect(other.context.value?.requestId).toBe('r-9');
  });
});
