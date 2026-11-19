import { afterEach, describe, expect, it } from 'vitest';
import OpenPositionsCtaSection from '@/modules/landing/components/OpenPositions/OpenPositionsCtaSection.vue';
import { useCandidateApplyModal } from '@/modules/landing/composables/useCandidateApplyModal';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('OpenPositionsCtaSection', () => {
  afterEach(() => {
    useCandidateApplyModal().close();
  });

  it('lists what to expect and links to the industries', () => {
    const wrapper = mountWithPlugins(OpenPositionsCtaSection);

    expect(wrapper.findAll('.op-cta__feature-text')).toHaveLength(4);
    expect(wrapper.find('a[href="/industries"]').text()).toContain('Browse industries');
  });

  it('opens the generic apply modal', async () => {
    const wrapper = mountWithPlugins(OpenPositionsCtaSection);
    await wrapper.find('button.arrow-pill').trigger('click');

    expect(useCandidateApplyModal().isOpen.value).toBe(true);
    expect(useCandidateApplyModal().context.value).toBeNull();
  });
});
