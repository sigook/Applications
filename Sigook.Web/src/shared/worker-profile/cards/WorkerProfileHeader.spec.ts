import { describe, expect, it, vi } from 'vitest';
import WorkerProfileHeader from '@/shared/worker-profile/cards/WorkerProfileHeader.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/forms/WorkImageDetail.vue', () => ({
  default: { name: 'ImageDetail', props: { data: Object }, emits: ['updateProfile'], template: '<div class="image-detail" />' },
}));

function setup(overrides: Partial<WorkerProfileDetail> = {}, numberClass?: string, slots: Record<string, string> = {}) {
  return mountWithPlugins(WorkerProfileHeader, {
    props: { worker: createWorkerProfileDetail(overrides), numberClass },
    slots,
  });
}

describe('WorkerProfileHeader', () => {
  it('shows the lowercased full name and the worker number', () => {
    const wrapper = setup({ firstName: 'JOHN', middleName: 'Paul', lastName: 'SMITH', secondLastName: 'Lee' }, 'is-dnu');
    const name = wrapper.find('.worker-header-name');

    expect(name.text().replace(/\s+/g, ' ')).toBe('john paul smith lee #1001');
    expect(name.find('.worker-header-number').classes()).toContain('is-dnu');
  });

  it('shows the contact kpis that have a value', () => {
    const wrapper = setup({
      mobileNumber: '4165550000',
      location: { city: { id: 'c1', value: 'Toronto', province: { id: 'p1', value: 'Ontario', code: 'ON' } } },
    });
    const kpis = wrapper.findAll('.detail-kpi');

    expect(kpis.map((k) => k.find('dt').text())).toEqual(['Phone', 'Location', 'Email']);
    expect(kpis[0].find('a').attributes('href')).toBe('tel:4165550000');
    expect(kpis[1].find('.detail-kpi-value').text()).toBe('Toronto, ON');
    expect(kpis[2].find('a').attributes('href')).toBe('mailto:john.smith@example.com');
  });

  it('shows the city alone when it has no province code', () => {
    const wrapper = setup({ location: { city: { id: 'c1', value: 'Toronto' } } });

    expect(wrapper.findAll('.detail-kpi').map((k) => k.find('dt').text())).toEqual(['Location', 'Email']);
    expect(wrapper.find('.detail-kpi-value').text()).toBe('Toronto');
  });

  it('hides the kpis when there is no contact data', () => {
    expect(setup({ email: '' }).find('.detail-kpis').exists()).toBe(false);
  });

  it('renders the chips and actions slots', () => {
    const wrapper = setup({}, undefined, { chips: '<span class="chip">Approved</span>', actions: '<button>Approve</button>' });

    expect(wrapper.find('.chip').text()).toBe('Approved');
    expect(wrapper.find('.worker-header-actions button').text()).toBe('Approve');
  });

  it('hides the actions container without actions', () => {
    expect(setup().find('.worker-header-actions').exists()).toBe(false);
  });

  it('forwards updateProfile from the photo', () => {
    const wrapper = setup();
    const photo = wrapper.findComponent({ name: 'ImageDetail' });

    expect(photo.props('data')).toMatchObject({ id: 'worker-profile-1' });
    photo.vm.$emit('updateProfile');
    expect(wrapper.emitted('updateProfile')).toHaveLength(1);
  });
});
