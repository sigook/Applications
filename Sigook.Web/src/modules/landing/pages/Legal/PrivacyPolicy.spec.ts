import { describe, expect, it } from 'vitest';
import PrivacyPolicy from '@/modules/landing/pages/Legal/PrivacyPolicy.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('PrivacyPolicy', () => {
  it('renders the privacy policy', () => {
    const wrapper = mountWithPlugins(PrivacyPolicy);

    expect(wrapper.find('h4').text()).toBe('PRIVACY POLICY');
    expect(wrapper.text()).toContain('INTRODUCTION');
  });
});
