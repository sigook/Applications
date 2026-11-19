import { describe, expect, it } from 'vitest';
import ProfileIndex from '@/shared/worker-profile/cards/ProfileIndex.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerProfileSection } from '@/shared/worker-profile/types';

const sections: WorkerProfileSection[] = [
  { id: 'personal', label: 'Personal', isComplete: false, pendingCount: 2 },
  { id: 'contact', label: 'Contact', isComplete: true, pendingCount: 0 },
  { id: 'documents', label: 'Documents', isComplete: false, pendingCount: 0 },
  { id: 'comments', label: 'Comments', isComplete: null, pendingCount: 0 },
];

function setup(missingLabels: string[] = ['Personal', 'Documents']) {
  return mountWithPlugins(ProfileIndex, { props: { sections, activeId: 'contact', completeness: 60, missingLabels } });
}

describe('ProfileIndex', () => {
  it('links every section to its anchor and marks the active one', () => {
    const links = setup().findAll('.worker-profile-index-link');

    expect(links.map((l) => l.attributes('href'))).toEqual([
      '#worker-section-personal',
      '#worker-section-contact',
      '#worker-section-documents',
      '#worker-section-comments',
    ]);
    expect(links[1].classes()).toContain('is-active');
    expect(links[0].classes()).not.toContain('is-active');
  });

  it('shows pending counts, complete and missing markers', () => {
    const links = setup().findAll('.worker-profile-index-link');

    expect(links[0].find('.worker-count').text()).toBe('2');
    expect(links[1].find('.worker-profile-index-check').exists()).toBe(true);
    expect(links[2].find('.worker-profile-index-missing').exists()).toBe(true);
    expect(links[3].find('.icon').exists()).toBe(false);
  });

  it('shows the completeness with the missing sections', () => {
    const wrapper = setup();

    expect(wrapper.find('progress').attributes('value')).toBe('60');
    expect(wrapper.find('.worker-profile-completeness-value').text()).toBe('60% · missing personal, documents');
  });

  it('omits the missing list when nothing is missing', () => {
    expect(setup([]).find('.worker-profile-completeness-value').text()).toBe('60%');
  });

  it('emits the selected section instead of navigating', async () => {
    const wrapper = setup();
    await wrapper.findAll('.worker-profile-index-link')[2].trigger('click');

    expect(wrapper.emitted('select')).toEqual([['documents']]);
  });
});
