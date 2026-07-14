import { ref, reactive, Ref } from 'vue';
import {
  getGenders,
  getIdentificationTypes,
  getAvailability,
  getAvailabilityTimes,
  getDays,
  fetchLifts,
  fetchLanguages,
  getSkills,
} from '@/api/catalogApi';
import type {
  Gender,
  IdentificationType,
  Availability,
  AvailabilityTime,
  Day,
  Lift,
  Language,
  Skill,
} from '@/types/common';

export interface WorkerDocumentForm {
  fileName: string;
  description: string;
}

export interface WorkerLicenseForm {
  license: WorkerDocumentForm;
  expires?: Date | null;
  number?: string;
  issued?: Date | null;
}

export interface WorkerResumeForm {
  fileName: string;
}

export interface CreateWorkerState {
  availabilities: Availability[];
  availabilityTimes: AvailabilityTime[];
  availabilityDays: Day[];
  skills: Skill[];
  languages: Language[];
  location: Record<string, unknown>;
  identificationType1File: WorkerDocumentForm | null;
  identificationType1: IdentificationType | null;
  identificationType2File: WorkerDocumentForm | null;
  identificationType2: IdentificationType | null;
  licenses: WorkerLicenseForm[];
  certificates: WorkerDocumentForm[];
  resume: WorkerResumeForm | null;
  otherDocuments: WorkerDocumentForm[];
  [key: string]: unknown;
}

export function useCreateWorker() {
  const skills: Ref<Skill[]> = ref([]);
  const languages: Ref<Language[]> = ref([]);
  const allDaysSelected = ref(false);
  const filteredSkills: Ref<Skill[]> = ref([]);
  const filteredLanguages: Ref<Language[]> = ref([]);
  const genders: Ref<Gender[]> = ref([]);
  const identificationTypes: Ref<IdentificationType[]> = ref([]);
  const availabilities: Ref<Availability[]> = ref([]);
  const availabilityTimes: Ref<AvailabilityTime[]> = ref([]);
  const days: Ref<Day[]> = ref([]);
  const lifts: Ref<Lift[]> = ref([]);

  const worker = reactive<CreateWorkerState>({
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

  async function loadCatalogs(): Promise<void> {
    genders.value = await getGenders();
    identificationTypes.value = await getIdentificationTypes();
    availabilities.value = await getAvailability();
    availabilityTimes.value = await getAvailabilityTimes();
    days.value = await getDays();
    lifts.value = await fetchLifts();
    languages.value = await fetchLanguages();
    skills.value = await getSkills();
    filteredSkills.value = skills.value;
    filteredLanguages.value = languages.value;
  }

  function changeDaysSelected(): void {
    if (allDaysSelected.value) {
      for (let i = 0; i < days.value.length; i++) {
        worker.availabilityDays.push(days.value[i]);
      }
    } else {
      worker.availabilityDays = [];
    }
  }

  function changeAllDays(): void {
    for (let i = 0; i < worker.availabilityDays.length; i++) {
      if (worker.availabilityDays.length !== days.value.length) {
        allDaysSelected.value = false;
      } else {
        allDaysSelected.value = true;
      }
    }
  }

  function getFilteredSkills(text: string): void {
    if (text) {
      filteredSkills.value = skills.value.filter((option) =>
        option.skill?.toLowerCase().includes(text.toLowerCase())
      );
    } else {
      filteredSkills.value = skills.value;
    }
  }

  function getFilteredLanguages(text: string): void {
    if (text) {
      filteredLanguages.value = languages.value.filter((option) =>
        option.value?.toLowerCase().includes(text.toLowerCase())
      );
    } else {
      filteredLanguages.value = languages.value;
    }
  }

  return {
    skills,
    languages,
    allDaysSelected,
    filteredSkills,
    filteredLanguages,
    genders,
    identificationTypes,
    availabilities,
    availabilityTimes,
    days,
    lifts,
    worker,
    loadCatalogs,
    changeDaysSelected,
    changeAllDays,
    getFilteredSkills,
    getFilteredLanguages,
  };
}
