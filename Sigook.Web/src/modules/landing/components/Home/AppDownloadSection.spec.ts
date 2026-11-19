import { describe, expect, it } from 'vitest';
import AppDownloadSection from '@/modules/landing/components/Home/AppDownloadSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('AppDownloadSection', () => {
  it('links to both app stores and lists the features', () => {
    const wrapper = mountWithPlugins(AppDownloadSection);
    const badges = wrapper.findAll('.app__badge');

    expect(badges.map((b) => b.attributes('href'))).toEqual([
      'https://apps.apple.com/us/app/sigook/id1446736193',
      'https://play.google.com/store/apps/details?id=com.sigook.sigook&hl=es_EC',
    ]);
    expect(badges.every((b) => b.attributes('target') === '_blank')).toBe(true);
    expect(wrapper.text()).toContain('One platform. Total workforce visibility.');
  });
});
