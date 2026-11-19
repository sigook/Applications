import { describe, expect, it } from 'vitest';
import TempToPermDetail from '@/modules/landing/components/shared/sections/TempToPermDetail.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('TempToPermDetail', () => {
  it('explains how temp to perm works', () => {
    const wrapper = mountWithPlugins(TempToPermDetail);

    expect(wrapper.findAll('.ttp-detail__heading').map((h) => h.text())).toEqual(['How it works', 'Why employers choose this path', 'What Sigook handles for you']);
    expect(wrapper.findAll('.ttp-detail__steps li')).toHaveLength(3);
  });
});
