import { describe, expect, it, vi } from 'vitest';
import Apply from '@/modules/landing/pages/Apply/Apply.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/shared/forms/CandidateApplyForm.vue', () => ({
  default: { name: 'CandidateApplyForm', template: '<form class="apply-form-stub" />' },
}));

describe('Apply', () => {
  it('renders the hero and the standalone application form', () => {
    const wrapper = mountWithPlugins(Apply);

    expect(wrapper.find('.apply-page__eyebrow').text()).toBe('Apply');
    expect(wrapper.find('.apply-page__heading').text()).toContain('Join Sigook.');
    expect(wrapper.find('.apply-page__form .apply-form-stub').exists()).toBe(true);
  });
});
