import { describe, expect, it, vi } from 'vitest';
import { useCreateWorker } from '@/modules/worker/register/useCreateWorker';
import {
  fetchLanguages,
  fetchLifts,
  getAvailability,
  getAvailabilityTimes,
  getDays,
  getGenders,
  getIdentificationTypes,
  getSkills,
} from '@/shared/api/catalogApi';

vi.mock('@/shared/api/catalogApi', () => ({
  getGenders: vi.fn(),
  getIdentificationTypes: vi.fn(),
  getAvailability: vi.fn(),
  getAvailabilityTimes: vi.fn(),
  getDays: vi.fn(),
  fetchLifts: vi.fn(),
  fetchLanguages: vi.fn(),
  getSkills: vi.fn(),
}));

const genders = [{ id: 'g1', value: 'Female' }];
const identificationTypes = [{ id: 'i1', value: 'Passport', code: 1 }];
const availabilities = [{ id: 'a1', value: 'Full time' }];
const availabilityTimes = [{ id: 't1', value: 'Morning' }];
const days = [
  { id: 'd1', value: 'Monday' },
  { id: 'd2', value: 'Tuesday' },
  { id: 'd3', value: 'Wednesday' },
];
const lifts = [{ id: 'l1', value: '50 lbs' }];
const languages = [
  { id: 'en', value: 'English' },
  { id: 'fr', value: 'French' },
  { id: 'es', value: 'Spanish' },
];
const skills = [{ skill: 'Forklift' }, { skill: 'Welding' }, { skill: 'Fork Reach Truck' }];

function mockCatalogs() {
  vi.mocked(getGenders).mockResolvedValue(genders);
  vi.mocked(getIdentificationTypes).mockResolvedValue(identificationTypes);
  vi.mocked(getAvailability).mockResolvedValue(availabilities);
  vi.mocked(getAvailabilityTimes).mockResolvedValue(availabilityTimes);
  vi.mocked(getDays).mockResolvedValue(days);
  vi.mocked(fetchLifts).mockResolvedValue(lifts);
  vi.mocked(fetchLanguages).mockResolvedValue(languages);
  vi.mocked(getSkills).mockResolvedValue(skills);
}

async function loaded() {
  mockCatalogs();
  const state = useCreateWorker();
  await state.loadCatalogs();
  return state;
}

describe('useCreateWorker', () => {
  it('starts with an empty worker', () => {
    const { worker, allDaysSelected, days: emptyDays } = useCreateWorker();

    expect(worker).toEqual({
      availabilities: [],
      availabilityTimes: [],
      availabilityDays: [],
      skills: [],
      languages: [],
      location: {},
      identificationType1File: null,
      identificationType1: null,
      identificationType2File: null,
      identificationType2: null,
      licenses: [],
      certificates: [],
      resume: null,
      otherDocuments: [],
    });
    expect(allDaysSelected.value).toBe(false);
    expect(emptyDays.value).toEqual([]);
  });

  it('loads every catalog and seeds the filtered lists', async () => {
    const state = await loaded();

    expect(state.genders.value).toEqual(genders);
    expect(state.identificationTypes.value).toEqual(identificationTypes);
    expect(state.availabilities.value).toEqual(availabilities);
    expect(state.availabilityTimes.value).toEqual(availabilityTimes);
    expect(state.days.value).toEqual(days);
    expect(state.lifts.value).toEqual(lifts);
    expect(state.languages.value).toEqual(languages);
    expect(state.skills.value).toEqual(skills);
    expect(state.filteredSkills.value).toEqual(skills);
    expect(state.filteredLanguages.value).toEqual(languages);
  });

  it('propagates a catalog failure', async () => {
    mockCatalogs();
    vi.mocked(getDays).mockRejectedValue(new Error('offline'));
    const state = useCreateWorker();

    await expect(state.loadCatalogs()).rejects.toThrow('offline');
    expect(state.lifts.value).toEqual([]);
  });

  describe('day selection', () => {
    it('selects every day when "all days" is checked', async () => {
      const state = await loaded();
      state.allDaysSelected.value = true;
      state.changeDaysSelected();

      expect(state.worker.availabilityDays).toEqual(days);
    });

    it('clears the days when "all days" is unchecked', async () => {
      const state = await loaded();
      state.worker.availabilityDays = [days[0], days[1]];
      state.allDaysSelected.value = false;
      state.changeDaysSelected();

      expect(state.worker.availabilityDays).toEqual([]);
    });

    it('checks "all days" when every day is picked individually', async () => {
      const state = await loaded();
      state.worker.availabilityDays = [...days];
      state.changeAllDays();

      expect(state.allDaysSelected.value).toBe(true);
    });

    it('unchecks "all days" when a day is missing', async () => {
      const state = await loaded();
      state.allDaysSelected.value = true;
      state.worker.availabilityDays = [days[0], days[2]];
      state.changeAllDays();

      expect(state.allDaysSelected.value).toBe(false);
    });

    it('does not duplicate days already picked when "all days" is checked', async () => {
      const state = await loaded();
      state.worker.availabilityDays = [days[0]];
      state.allDaysSelected.value = true;
      state.changeDaysSelected();

      expect(state.worker.availabilityDays).toEqual(days);
    });

    it('unchecks "all days" when every day is removed', async () => {
      const state = await loaded();
      state.allDaysSelected.value = true;
      state.worker.availabilityDays = [];
      state.changeAllDays();

      expect(state.allDaysSelected.value).toBe(false);
    });
  });

  describe('autocomplete filters', () => {
    it('filters skills case-insensitively by substring', async () => {
      const state = await loaded();
      state.getFilteredSkills('FORK');

      expect(state.filteredSkills.value).toEqual([{ skill: 'Forklift' }, { skill: 'Fork Reach Truck' }]);
    });

    it('restores all skills for an empty text', async () => {
      const state = await loaded();
      state.getFilteredSkills('weld');
      state.getFilteredSkills('');

      expect(state.filteredSkills.value).toEqual(skills);
    });

    it('filters languages case-insensitively', async () => {
      const state = await loaded();
      state.getFilteredLanguages('ench');

      expect(state.filteredLanguages.value).toEqual([{ id: 'fr', value: 'French' }]);
    });

    it('returns no language when nothing matches and restores on empty text', async () => {
      const state = await loaded();
      state.getFilteredLanguages('xyz');
      expect(state.filteredLanguages.value).toEqual([]);

      state.getFilteredLanguages('');
      expect(state.filteredLanguages.value).toEqual(languages);
    });

    it('tolerates languages without a value', async () => {
      mockCatalogs();
      vi.mocked(fetchLanguages).mockResolvedValue([{ id: 'x', value: '' }, { id: 'en', value: 'English' }]);
      const state = useCreateWorker();
      await state.loadCatalogs();
      state.getFilteredLanguages('eng');

      expect(state.filteredLanguages.value).toEqual([{ id: 'en', value: 'English' }]);
    });
  });
});
