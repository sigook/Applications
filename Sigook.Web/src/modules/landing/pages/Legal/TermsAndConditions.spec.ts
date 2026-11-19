import { describe, expect, it } from 'vitest';
import TermsAndConditions from '@/modules/landing/pages/Legal/TermsAndConditions.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('TermsAndConditions', () => {
  it('renders the terms of service', () => {
    const wrapper = mountWithPlugins(TermsAndConditions);

    expect(wrapper.find('h4').text()).toBe('TERMS AND CONDITIONS OF SERVICE');
  });
});
