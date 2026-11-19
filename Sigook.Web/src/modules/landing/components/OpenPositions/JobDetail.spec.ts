import { afterEach, describe, expect, it } from 'vitest';
import JobDetail from '@/modules/landing/components/OpenPositions/JobDetail.vue';
import { useCandidateApplyModal } from '@/modules/landing/composables/useCandidateApplyModal';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { JobViewModel } from '@/modules/landing/types';

function job(overrides: Partial<JobViewModel> = {}): JobViewModel {
  return {
    id: 'job-1',
    requestId: 'request-1',
    numberId: '101',
    title: 'Forklift Operator',
    salary: '$22.50',
    location: 'Miami, FL',
    type: 'Full time',
    createdAt: '2026-11-03T15:00:00Z',
    description: '<p>Move pallets</p><script>alert(1)</script>',
    requirements: '<ul><li>Licence</li></ul>',
    responsibilities: '<p>Load trucks</p>',
    shift: 'Mon-Fri 7am-3pm',
    createdBy: 'agency',
    agencyId: null,
    ...overrides,
  };
}

function sections(wrapper: ReturnType<typeof mountWithPlugins>) {
  return wrapper.findAll('.job-detail__section-title').map((s) => s.text());
}

describe('JobDetail', () => {
  afterEach(() => {
    useCandidateApplyModal().close();
  });

  it('renders the job header and sanitized sections', () => {
    const wrapper = mountWithPlugins(JobDetail, { props: { job: job() } });

    expect(wrapper.find('.job-detail__title').text()).toBe('Forklift Operator #101');
    expect(wrapper.find('.job-detail__meta-item--location').text()).toBe('Miami, FL');
    expect(wrapper.find('.job-detail__meta-item--salary').text()).toBe('$22.50');
    expect(wrapper.find('.job-detail__chip').text()).toBe('Full time');
    expect(sections(wrapper)).toEqual(['Description', 'Schedule', 'Responsibilities', 'Requirements']);
    expect(wrapper.find('.job-detail__html').html()).toContain('<p>Move pallets</p>');
    expect(wrapper.find('.job-detail__html').html()).not.toContain('script');
    expect(wrapper.find('.job-detail__posted').text()).toBe('Posted November 3, 2026 · ref 101');
  });

  it('hides missing details', () => {
    const wrapper = mountWithPlugins(JobDetail, {
      props: { job: job({ salary: '$0.00', type: '', description: '', shift: '', responsibilities: '', requirements: '', createdAt: 'not a date' }) },
    });

    expect(wrapper.find('.job-detail__meta-item--salary').exists()).toBe(false);
    expect(wrapper.find('.job-detail__chip').exists()).toBe(false);
    expect(sections(wrapper)).toEqual([]);
    expect(wrapper.find('.job-detail__posted').text()).toBe('ref 101');
  });

  it('hides an empty salary', () => {
    expect(mountWithPlugins(JobDetail, { props: { job: job({ salary: '' }) } }).find('.job-detail__meta-item--salary').exists()).toBe(false);
  });

  it('opens the apply modal for the job', async () => {
    const wrapper = mountWithPlugins(JobDetail, { props: { job: job() } });
    const apply = wrapper.find('.job-detail__apply');
    await apply.trigger('click');

    expect(apply.attributes('aria-label')).toBe('Apply for Forklift Operator');
    expect(useCandidateApplyModal().context.value).toEqual({ jobTitle: 'Forklift Operator', jobNumber: '101', requestId: 'request-1' });
  });
});
