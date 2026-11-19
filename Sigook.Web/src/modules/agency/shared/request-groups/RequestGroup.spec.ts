import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import RequestGroup from '@/modules/agency/shared/request-groups/RequestGroup.vue';
import { useAgencyStore } from '@/modules/agency/store';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestGroupFill, RequestGroupSummary } from '@/modules/agency/shared/request-groups/types';

let pinia: Pinia;

const baseGroup: RequestGroupSummary = {
  requestId: 'r1',
  agencyId: 'a2',
  numberId: 120,
  companyFullName: 'Acme',
  jobTitle: 'Forklift operator',
  city: 'Toronto',
  provinceName: 'Ontario',
  displayShift: '7:00 AM - 3:00 PM',
  startAt: '2026-06-03T12:00:00',
  isAsap: true,
  isDirectHiring: true,
};

function setup(group: Partial<RequestGroupSummary> = {}, fill: RequestGroupFill | null = null, slots: Record<string, string> = {}) {
  return mountWithPlugins(RequestGroup, {
    props: { group: { ...baseGroup, ...group }, countLabel: '3 workers', fill },
    slots: { default: '<div class="group-body">Body</div>', ...slots },
    global: { plugins: [pinia] },
  });
}

function tagTexts(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.request-group-tags .tag').map((t) => t.text());
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  useAgencyStore().setAgencies([{ id: 'a1', name: 'Covenant' }, { id: 'a2', name: 'Sigook Staffing' }]);
  vi.useFakeTimers({ now: new Date('2026-06-01T09:00:00'), toFake: ['Date'] });
});

afterEach(() => {
  vi.useRealTimers();
});

describe('RequestGroup', () => {
  it('shows the request summary and flags', () => {
    const wrapper = setup();
    expect(wrapper.find('.request-group-title').text()).toBe('120 · Acme · Forklift operator');
    const meta = wrapper.find('.request-group-meta').text();
    expect(meta).toContain('3 workers · Toronto, Ontario');
    expect(meta).toContain('· 7:00 AM - 3:00 PM');
    expect(wrapper.findAll('.request-flags .request-flag').map((f) => f.text())).toEqual(['Asap', 'DH']);
    expect(wrapper.find('.agency-badge').text()).toBe('SS');
  });

  it('omits missing city, shift, flags and start date', () => {
    const wrapper = setup({ city: null, displayShift: null, isAsap: false, isDirectHiring: false, startAt: null });
    expect(wrapper.find('.request-group-meta').text()).toBe('3 workers');
    expect(wrapper.find('.request-flags').exists()).toBe(false);
    expect(tagTexts(wrapper)).toEqual([]);
  });

  it('warns when the request starts within two days', () => {
    const start = setup().find('.request-group-tags .tag');
    expect(start.text()).toContain('Starts');
    expect(start.classes()).toContain('is-warning');
  });

  it('does not warn when the start is further away', () => {
    const wrapper = setup({ startAt: '2026-06-10T12:00:00' });
    expect(wrapper.find('.request-group-tags .tag').classes()).not.toContain('is-warning');
  });

  it('colors the fill by completion', () => {
    expect(setup({}, { value: 2, total: 3, label: 'booked' }).find('.request-group-fill').classes()).toContain('is-warning');
    const full = setup({}, { value: 3, total: 3, label: 'booked' }).find('.request-group-fill');
    expect(full.text()).toBe('3/3 booked');
    expect(full.classes()).toContain('is-success');
  });

  it('toggles open from the header', async () => {
    const wrapper = setup();
    await wrapper.find('.request-group-header').trigger('click');
    expect(wrapper.emitted('update:open')).toEqual([[true]]);
  });

  it('renders the menu only when provided', async () => {
    expect(setup().find('.request-group-actions').exists()).toBe(false);
    const wrapper = setup({}, null, { menu: '<b-dropdown-item class="menu-item">Edit</b-dropdown-item>' });
    await wrapper.find('.request-group-actions button').trigger('click');
    await nextTick();
    expect(document.body.querySelector('.menu-item')?.textContent).toBe('Edit');
    expect(wrapper.emitted('update:open')).toBeUndefined();
  });
});
