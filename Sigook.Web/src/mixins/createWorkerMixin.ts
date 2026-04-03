import {
  getGenders, getIdentificationTypes, getAvailability, getAvailabilityTimes,
  getDays, fetchLifts, fetchLanguages, getSkills
} from '@/api/catalogApi';
import type { Gender, IdentificationType, Availability, AvailabilityTime, Day, Lift, Language, Skill } from '@/types/common';

export default {
  data() {
    return {
      isLoading: false,
      skills: [] as Skill[],
      languages: [] as Language[],
      allDaysSelected: false,
      filteredSkills: [] as Skill[],
      filteredLanguages: [] as Language[],
      genders: [] as Gender[],
      identificationTypes: [] as IdentificationType[],
      availabilities: [] as Availability[],
      availabilityTimes: [] as AvailabilityTime[],
      days: [] as Day[],
      lifts: [] as Lift[],
      worker: {
        availabilities: [] as Availability[],
        availabilityTimes: [] as AvailabilityTime[],
        availabilityDays: [] as Day[],
        skills: [] as Skill[],
        languages: [] as Language[],
        location: {} as Record<string, any>,
        identificationType1File: null as any,
        identificationType1: null as any,
        identificationType2File: null as any,
        identificationType2: null as any,
        licenses: [] as any[],
        certificates: [] as any[],
        resume: null as any,
        otherDocuments: [] as any[]
      }
    };
  },
  async created() {
    const vm = this as any;
    vm.isLoading = true;
    vm.genders = await getGenders();
    vm.identificationTypes = await getIdentificationTypes();
    vm.availabilities = await getAvailability();
    vm.availabilityTimes = await getAvailabilityTimes();
    vm.days = await getDays();
    vm.lifts = await fetchLifts();
    vm.languages = await fetchLanguages();
    vm.skills = await getSkills();
    vm.filteredSkills = vm.skills;
    vm.filteredLanguages = vm.languages;
    vm.isLoading = false;
  },
  methods: {
    changeDaysSelected(): void {
      const vm = this as any;
      if (vm.allDaysSelected) {
        for (let i = 0; i < vm.days.length; i++) {
          vm.worker.availabilityDays.push(vm.days[i]);
        }
      } else {
        vm.worker.availabilityDays = [];
      }
    },
    changeAllDays(): void {
      const vm = this as any;
      for (let i = 0; i < vm.worker.availabilityDays.length; i++) {
        if (vm.worker.availabilityDays.length !== vm.days.length) {
          vm.allDaysSelected = false;
        } else {
          vm.allDaysSelected = true;
        }
      }
    },
    changeAllTimes(): void {
      const vm = this as any;
      if (vm.worker.availabilityTimes.length !== vm.availabilityTimes.length) {
        vm.allTimesSelected = false;
      } else {
        vm.allTimesSelected = true;
      }
    },
    getFilteredSkills(text: string): void {
      const vm = this as any;
      if (text) {
        vm.filteredSkills = vm.skills.filter((option: Skill) =>
          option.skill?.toLowerCase().includes(text.toLowerCase())
        );
      } else {
        vm.filteredSkills = vm.skills;
      }
    },
    getFilteredLanguages(text: string): void {
      const vm = this as any;
      if (text) {
        vm.filteredLanguages = vm.languages.filter((option: Language) =>
          option.value?.toLowerCase().includes(text.toLowerCase())
        );
      } else {
        vm.filteredLanguages = vm.languages;
      }
    }
  },
};
