import { describe, expect, it } from 'vitest';
import NeedsAttention from '@/shared/worker-profile/cards/NeedsAttention.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerAttentionItem } from '@/shared/worker-profile/types';

const items: WorkerAttentionItem[] = [
  { key: 'sin', severity: 'danger', title: 'SIN/SSN expired', detail: 'Expired 01-Jan-2026', sectionId: 'personal' },
  { key: 'license', severity: 'warning', title: 'License expires soon', detail: '5 days left', sectionId: 'documents' },
];

describe('NeedsAttention', () => {
  it('shows the empty state without items', () => {
    const wrapper = mountWithPlugins(NeedsAttention, { props: { items: [] } });

    expect(wrapper.text()).toContain('Nothing pending');
    expect(wrapper.find('.worker-count').exists()).toBe(false);
    expect(wrapper.findAll('.worker-attention-item')).toHaveLength(0);
  });

  it('lists every item with its count and severity', () => {
    const wrapper = mountWithPlugins(NeedsAttention, { props: { items } });
    const buttons = wrapper.findAll('.worker-attention-item');

    expect(wrapper.find('.worker-count').text()).toBe('2');
    expect(wrapper.text()).not.toContain('Nothing pending');
    expect(buttons[0].classes()).toContain('is-danger');
    expect(buttons[0].text()).toContain('SIN/SSN expired');
    expect(buttons[0].text()).toContain('Expired 01-Jan-2026');
    expect(buttons[1].classes()).toContain('is-warning');
  });

  it('emits the section of the clicked item', async () => {
    const wrapper = mountWithPlugins(NeedsAttention, { props: { items } });
    await wrapper.findAll('.worker-attention-item')[1].trigger('click');

    expect(wrapper.emitted('select')).toEqual([['documents']]);
  });
});
