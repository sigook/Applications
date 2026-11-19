import { describe, expect, it } from 'vitest';
import ApprovalStatusCard from '@/modules/worker/profile/components/ApprovalStatusCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(approved: boolean) {
  return mountWithPlugins(ApprovalStatusCard, { props: { approved } });
}

describe('ApprovalStatusCard', () => {
  it('links approved workers to the job requests', () => {
    const wrapper = setup(true);
    expect(wrapper.find('h3').text()).toBe('Approved to work');
    expect(wrapper.find('.approval-status-icon').classes()).toContain('is-success');
    expect(wrapper.find('a').attributes('href')).toBe('/worker-requests');
  });

  it('explains the pending review', () => {
    const wrapper = setup(false);
    expect(wrapper.find('h3').text()).toBe('Pending approval');
    expect(wrapper.find('.approval-status-icon').classes()).toContain('is-warning');
    expect(wrapper.text()).toContain('Your agency reviews your profile');
    expect(wrapper.find('a').exists()).toBe(false);
  });
});
