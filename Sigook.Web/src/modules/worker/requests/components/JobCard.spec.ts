import { describe, expect, it } from 'vitest';
import JobCard from '@/modules/worker/requests/components/JobCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerRequestListItem } from '@/modules/worker/requests/types';

const job: WorkerRequestListItem = {
  id: 'r1',
  numberId: 300,
  jobTitle: 'Forklift operator',
  location: '1 King St, Toronto',
  entrance: 'Door 4',
  status: 'Booked',
  isAsap: true,
  workerRate: 21.5,
  workerSalary: null,
  startAt: '2026-06-01T12:00:00',
  finishAt: '2026-06-30T12:00:00',
  durationTerm: 'ShortTerm',
};

function setup(overrides: Partial<WorkerRequestListItem> = {}, clickable = false) {
  return mountWithPlugins(JobCard, { props: { job: { ...job, ...overrides }, clickable } });
}

function texts(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.job-card__text').map((t) => t.text());
}

describe('JobCard', () => {
  it('summarizes the job', () => {
    const wrapper = setup();
    expect(wrapper.find('.job-card__title').text()).toBe('300 · Forklift operator');
    expect(wrapper.find('.job-card__asap').exists()).toBe(true);
    expect(texts(wrapper)).toEqual(['1 King St, Toronto - Door 4', '01-Jun-2026 - 30-Jun-2026']);
    expect(wrapper.find('.job-card__chip.is-rate').text()).toBe('$21.50/hr');
    expect(wrapper.find('.job-card__chip.is-term').text()).toBe('Short Term');
    expect(wrapper.find('.job-card__status').classes()).toContain('is-success');
  });

  it('shows the yearly salary for direct hiring', () => {
    expect(setup({ workerSalary: 52000 }).find('.job-card__chip.is-rate').text()).toBe('$52,000.00/year');
  });

  it('falls back when there is no address and hides an empty status', () => {
    const wrapper = setup({ location: '', entrance: '', status: 'None', isAsap: false });
    expect(texts(wrapper)[0]).toBe('N/A');
    expect(wrapper.find('.job-card__status').exists()).toBe(false);
    expect(wrapper.find('.job-card__asap').exists()).toBe(false);
  });

  it.each([
    ['Applied', 'is-warning'],
    ['Open', 'is-warning'],
    ['Filled', 'is-success'],
    ['Declined', 'is-danger'],
    ['Cancelled', 'is-danger'],
    ['InQueue', ''],
  ])('colors the %s status', (status, color) => {
    const classes = setup({ status }).find('.job-card__status').classes();
    expect(classes.filter((c) => c.startsWith('is-'))).toEqual(color ? [color] : []);
  });

  it('hides the finish date of an open long term job', () => {
    expect(texts(setup({ durationTerm: 'LongTerm', status: 'Open' }))[1]).toBe('01-Jun-2026');
    expect(texts(setup({ durationTerm: 'LongTerm', status: 'Filled' }))[1]).toBe('01-Jun-2026 - 30-Jun-2026');
    expect(texts(setup({ finishAt: null }))[1]).toBe('01-Jun-2026');
  });

  it('emits click', async () => {
    const wrapper = setup({}, true);
    expect(wrapper.find('.job-card').classes()).toContain('is-clickable');
    await wrapper.find('.job-card').trigger('click');
    expect(wrapper.emitted('click')).toHaveLength(1);
  });
});
