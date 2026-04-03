import { ref } from 'vue';
import * as catalogApi from '@/api/catalogApi';
import type {
  Gender,
  IdentificationType,
  Availability,
  AvailabilityTime,
  Day,
  Lift,
  Language,
  WsibGroup,
  Industry,
  JobPosition,
  Skill,
  CancellationReason,
  CatalogItem,
  TaxCategory,
} from '@/types/common';

// Shared cache — data is loaded once and reused across components
const genders = ref<Gender[]>([]);
const identificationTypes = ref<IdentificationType[]>([]);
const availabilities = ref<Availability[]>([]);
const availabilityTimes = ref<AvailabilityTime[]>([]);
const days = ref<Day[]>([]);
const lifts = ref<Lift[]>([]);
const languages = ref<Language[]>([]);
const wsibGroups = ref<WsibGroup[]>([]);
const jobPositions = ref<JobPosition[]>([]);
const skills = ref<Skill[]>([]);
const industries = ref<Industry[]>([]);
const cancellationReasons = ref<CancellationReason[]>([]);
const companyStatuses = ref<CatalogItem<number>[]>([]);
const taxCategories = ref<TaxCategory[]>([]);

const loading = ref(false);

const maximumHoursPerDay = Number(process.env.VUE_APP_MAXIMUM_HOURS_DAY) || 12;

const residencyList = ['Citizen', 'Work Permit', 'Student', 'Permanent Resident'];

const sourceList = [
  'Google Search', 'Online Advert', 'Friend Recommendation', 'Zip Recruiter',
  'Glassdoor', 'Indeed', 'Linkedin', 'Kijiji', 'Email', 'Other'
];

async function loadIfEmpty<T>(cache: { value: T[] }, fetcher: () => Promise<T[]>): Promise<T[]> {
  if (cache.value.length > 0) return cache.value;
  cache.value = await fetcher();
  return cache.value;
}

export function useCatalog() {
  return {
    // Reactive state
    genders,
    identificationTypes,
    availabilities,
    availabilityTimes,
    days,
    lifts,
    languages,
    wsibGroups,
    jobPositions,
    skills,
    industries,
    cancellationReasons,
    companyStatuses,
    taxCategories,
    loading,
    maximumHoursPerDay,
    residencyList,
    sourceList,

    // Loaders (fetch once, then cache)
    loadGenders: () => loadIfEmpty(genders, catalogApi.getGenders),
    loadIdentificationTypes: () => loadIfEmpty(identificationTypes, catalogApi.getIdentificationTypes),
    loadAvailabilities: () => loadIfEmpty(availabilities, catalogApi.getAvailability),
    loadAvailabilityTimes: () => loadIfEmpty(availabilityTimes, catalogApi.getAvailabilityTimes),
    loadDays: () => loadIfEmpty(days, catalogApi.getDays),
    loadLifts: () => loadIfEmpty(lifts, catalogApi.fetchLifts),
    loadLanguages: () => loadIfEmpty(languages, catalogApi.fetchLanguages),
    loadWsibGroups: () => loadIfEmpty(wsibGroups, catalogApi.getWsibGroups),
    loadJobPositions: () => loadIfEmpty(jobPositions, catalogApi.getJobPositions),
    loadSkills: () => loadIfEmpty(skills, catalogApi.getSkills),
    loadIndustries: () => loadIfEmpty(industries, catalogApi.getIndustries),
    loadCancellationReasons: () => loadIfEmpty(cancellationReasons, catalogApi.getReasonCancellationRequest),
    loadCompanyStatuses: () => loadIfEmpty(companyStatuses, catalogApi.getCompanyStatus),
    loadTaxCategories: () => loadIfEmpty(taxCategories, catalogApi.getTaxCategories),

    // Mutations
    addIndustry: async (industry: { id: string; value: string }) => {
      const created = await catalogApi.addIndustry(industry);
      industries.value = [...industries.value, created];
      return created;
    },
  };
}
