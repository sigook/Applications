import { afterEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import CandidateApplyModal from '@/modules/landing/components/shared/forms/CandidateApplyModal.vue';
import { useCandidateApplyModal } from '@/modules/landing/composables/useCandidateApplyModal';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/shared/forms/CandidateApplyForm.vue', async () => {
  const { defineComponent } = await import('vue');
  return {
    default: defineComponent({
      name: 'CandidateApplyForm',
      props: { jobTitle: { type: String, default: undefined }, requestId: { type: String, default: undefined } },
      emits: ['submitted'],
      template: '<div class="form-stub"><button type="button">field</button></div>',
    }),
  };
});

function setup() {
  return mountWithPlugins(CandidateApplyModal);
}

function dialog() {
  return document.body.querySelector<HTMLElement>('.reg-modal');
}

describe('CandidateApplyModal', () => {
  afterEach(() => {
    useCandidateApplyModal().close();
    vi.useRealTimers();
  });

  it('renders nothing while closed', () => {
    setup();

    expect(dialog()).toBeNull();
  });

  it('shows the job title and forwards the context to the form', async () => {
    const wrapper = setup();
    useCandidateApplyModal().open({ jobTitle: 'Welder', requestId: 'request-1' });
    await nextTick();

    expect(document.body.querySelector('.reg-modal__title')?.textContent).toContain('Apply for this role');
    expect(document.body.querySelector('.reg-modal__subtitle')?.textContent).toBe('Welder');
    const form = wrapper.findComponent({ name: 'CandidateApplyForm' });
    expect(form.props()).toEqual({ jobTitle: 'Welder', requestId: 'request-1' });
    expect(form.attributes()).not.toHaveProperty('redirect-on-success');
  });

  it('uses the register title without a job', async () => {
    setup();
    useCandidateApplyModal().open();
    await nextTick();

    expect(document.body.querySelector('.reg-modal__title')?.textContent.trim()).toBe('Register with us');
    expect(document.body.querySelector('.reg-modal__subtitle')).toBeNull();
  });

  it('closes with the close button', async () => {
    setup();
    useCandidateApplyModal().open();
    await nextTick();
    document.body.querySelector<HTMLButtonElement>('.reg-modal__close')?.click();
    await nextTick();

    expect(dialog()).toBeNull();
  });

  it('closes when clicking the backdrop but not the panel', async () => {
    setup();
    useCandidateApplyModal().open();
    await nextTick();
    document.body.querySelector<HTMLElement>('.reg-modal__panel')?.click();
    await nextTick();
    expect(dialog()).not.toBeNull();

    dialog()?.click();
    await nextTick();
    expect(dialog()).toBeNull();
  });

  it('closes on Escape', async () => {
    setup();
    useCandidateApplyModal().open();
    await nextTick();
    window.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter' }));
    await nextTick();
    expect(dialog()).not.toBeNull();

    window.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape' }));
    await nextTick();
    expect(dialog()).toBeNull();
  });

  it('closes shortly after the form is submitted', async () => {
    vi.useFakeTimers();
    const wrapper = setup();
    useCandidateApplyModal().open();
    await nextTick();
    const form = wrapper.findComponent({ name: 'CandidateApplyForm' });
    form.vm.$emit('submitted');
    form.vm.$emit('submitted');
    vi.advanceTimersByTime(799);
    await nextTick();
    expect(dialog()).not.toBeNull();

    vi.advanceTimersByTime(1);
    await nextTick();
    expect(dialog()).toBeNull();
  });

  it('cancels the pending close on unmount', async () => {
    vi.useFakeTimers();
    const wrapper = setup();
    useCandidateApplyModal().open();
    await nextTick();
    wrapper.findComponent({ name: 'CandidateApplyForm' }).vm.$emit('submitted');
    wrapper.unmount();
    vi.advanceTimersByTime(1000);

    expect(useCandidateApplyModal().isOpen.value).toBe(true);
  });
});
