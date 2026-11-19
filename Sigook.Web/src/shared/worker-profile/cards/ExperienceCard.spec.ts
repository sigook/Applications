import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import ExperienceCard from '@/shared/worker-profile/cards/ExperienceCard.vue';
import { deleteWorkerWorkExperience } from '@/shared/worker-profile/api';
import { showAlertConfirm, showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileJobExperienceDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/api', () => ({ deleteWorkerWorkExperience: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/worker-profile/forms/WorkExperienceForm.vue', () => ({
  default: {
    name: 'WorkExperienceForm',
    props: { workerId: String, data: Object },
    emits: ['updateExperience'],
    template: '<div class="experience-form" />',
  },
}));

const experiences: WorkerProfileJobExperienceDetail[] = [
  { id: 'e1', company: 'Acme', supervisor: 'Bob', duties: 'Packing', startDate: '2024-02-15T12:00:00', endDate: '2025-01-10T12:00:00', isCurrentJobPosition: false },
  { id: 'e2', company: 'Globex', supervisor: null, duties: null, startDate: '2025-03-01T12:00:00', endDate: null, isCurrentJobPosition: true },
];

function setup(jobExperiences = experiences) {
  return mountWithPlugins(ExperienceCard, { props: { worker: createWorkerProfileDetail({ jobExperiences }) } });
}

describe('ExperienceCard', () => {
  it('shows the empty state', () => {
    expect(setup([]).find('.profile-empty').text()).toBe('No experience added');
  });

  it('lists the experiences with their period', () => {
    const rows = setup().findAll('.experience-row');

    expect(rows[0].find('.experience-period').text()).toBe('Feb 2024 — Jan 2025');
    expect(rows[0].find('.experience-title').text()).toBe('Acme · Bob');
    expect(rows[0].find('.experience-duties').text()).toBe('Packing');
    expect(rows[1].find('.experience-period').text()).toBe('Mar 2025 — Present');
    expect(rows[1].find('.experience-title').text()).toBe('Globex');
    expect(rows[1].find('.experience-duties').exists()).toBe(false);
  });

  it('opens an empty form to add an experience', async () => {
    const wrapper = setup();
    await wrapper.find('.profile-link').trigger('click');
    await nextTick();

    const form = wrapper.findComponent({ name: 'WorkExperienceForm' });
    expect(form.props('workerId')).toBe('worker-profile-1');
    expect(form.props('data')).toBeUndefined();
    form.vm.$emit('updateExperience');
    expect(wrapper.emitted('updateProfile')).toHaveLength(1);
  });

  it('opens the form with the experience to edit', async () => {
    const wrapper = setup();
    await wrapper.findAll('button[aria-label="Edit experience"]')[1].trigger('click');
    await nextTick();

    expect(wrapper.findComponent({ name: 'WorkExperienceForm' }).props('data')).toEqual(experiences[1]);
  });

  it('deletes the experience once confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteWorkerWorkExperience).mockResolvedValue();
    const wrapper = setup();
    await wrapper.find('button[aria-label="Delete experience"]').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('updateProfile')).toHaveLength(1); });
    expect(deleteWorkerWorkExperience).toHaveBeenCalledExactlyOnceWith('worker-profile-1', 'e1');
    expect(wrapper.emitted('loading')).toEqual([[true]]);
  });

  it('does nothing when the deletion is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = setup();
    await wrapper.find('button[aria-label="Delete experience"]').trigger('click');

    await vi.waitFor(() => { expect(showAlertConfirm).toHaveBeenCalled(); });
    expect(deleteWorkerWorkExperience).not.toHaveBeenCalled();
    expect(wrapper.emitted('loading')).toBeUndefined();
  });

  it('stops loading and shows the error when the deletion fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteWorkerWorkExperience).mockRejectedValue(error);
    const wrapper = setup();
    await wrapper.find('button[aria-label="Delete experience"]').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('loading')).toEqual([[true], [false]]);
    expect(wrapper.emitted('updateProfile')).toBeUndefined();
  });
});
