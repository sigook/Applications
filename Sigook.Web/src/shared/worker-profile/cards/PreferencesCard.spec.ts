import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import PreferencesCard from '@/shared/worker-profile/cards/PreferencesCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

const { formStub } = vi.hoisted(() => ({
  formStub: (name: string) => ({ name, props: { data: Object }, emits: ['closeModal'], template: `<div class="${name}" />` }),
}));

vi.mock('@/shared/worker-profile/forms/WorkAvailabilitiesForm.vue', () => ({ default: formStub('AvailabilitiesForm') }));
vi.mock('@/shared/worker-profile/forms/WorkAvailabilityTimesForm.vue', () => ({ default: formStub('AvailabilityTimesForm') }));
vi.mock('@/shared/worker-profile/forms/WorkAvailabilityDaysForm.vue', () => ({ default: formStub('AvailabilityDaysForm') }));
vi.mock('@/shared/worker-profile/forms/WorkLocationPreferencesForm.vue', () => ({ default: formStub('LocationPreferencesForm') }));
vi.mock('@/shared/worker-profile/forms/WorkLiftForm.vue', () => ({ default: formStub('LiftForm') }));
vi.mock('@/shared/worker-profile/forms/WorkLanguagesForm.vue', () => ({ default: formStub('LanguagesForm') }));

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  return mountWithPlugins(PreferencesCard, { props: { worker: createWorkerProfileDetail(overrides) } });
}

function field(wrapper: ReturnType<typeof setup>, label: string) {
  const row = wrapper.findAll('.profile-field').find((f) => f.find('dt').text() === label);
  if (!row) throw new Error(`${label} not found`);
  return row.find('dd');
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

describe('PreferencesCard', () => {
  it('shows a placeholder for every empty preference', () => {
    const wrapper = setup();

    expect(wrapper.findAll('.profile-field-label').map((l) => l.text())).toEqual([
      'Availability', 'Shifts', 'Days', 'Preferred locations', 'Can lift up to', 'Languages',
    ]);
    expect(wrapper.findAll('dd.profile-empty')).toHaveLength(6);
  });

  it('lists the selected values as tags', () => {
    const wrapper = setup({
      availabilities: [{ id: 'a1', value: 'Full time' }],
      availabilityTimes: [{ id: 't1', value: 'Morning' }, { id: 't2', value: 'Night' }],
      availabilityDays: [{ id: 'd1', value: 'Monday' }],
      locationPreferences: [{ id: 'l1', value: 'Toronto' }],
      lift: { id: 'lift', value: '50 lbs' },
      languages: [{ id: 'en', value: 'English' }],
    });

    expect(field(wrapper, 'Shifts').findAll('.profile-tag').map((t) => t.text())).toEqual(['Morning', 'Night']);
    expect(field(wrapper, 'Can lift up to').text()).toBe('50 lbs');
    expect(field(wrapper, 'Languages').text()).toBe('English');
    expect(wrapper.findAll('dd.profile-empty')).toHaveLength(0);
  });

  it.each([
    ['Availability', 'AvailabilitiesForm'],
    ['Shifts', 'AvailabilityTimesForm'],
    ['Days', 'AvailabilityDaysForm'],
    ['Preferred locations', 'LocationPreferencesForm'],
    ['Can lift up to', 'LiftForm'],
    ['Languages', 'LanguagesForm'],
  ])('opens the %s form and emits updateProfile once it closes', async (item, name) => {
    const wrapper = setup();
    menuItem(item).click();
    await nextTick();

    const form = wrapper.findComponent({ name });
    expect(form.exists()).toBe(true);
    form.vm.$emit('closeModal');
    expect(wrapper.emitted('updateProfile')).toHaveLength(1);
  });
});
