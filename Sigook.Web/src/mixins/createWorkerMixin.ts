interface CatalogItem {
  id?: number;
  value?: string;
  skill?: string;
}

export default {
  data() {
    return {
      isLoading: false,
      skills: [] as CatalogItem[],
      languages: [] as CatalogItem[],
      allDaysSelected: false,
      filteredSkills: [] as CatalogItem[],
      filteredLanguages: [] as CatalogItem[],
      genders: [] as CatalogItem[],
      identificationTypes: [] as CatalogItem[],
      availabilities: [] as CatalogItem[],
      availabilityTimes: [] as CatalogItem[],
      days: [] as CatalogItem[],
      lifts: [] as CatalogItem[],
      worker: {
        availabilities: [] as CatalogItem[],
        availabilityTimes: [] as CatalogItem[],
        availabilityDays: [] as CatalogItem[],
        skills: [] as CatalogItem[],
        languages: [] as CatalogItem[],
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
    vm.genders = await vm.$store.dispatch('getGenders');
    vm.identificationTypes = await vm.$store.dispatch('getIdentificationTypes');
    vm.availabilities = await vm.$store.dispatch('getAvailability');
    vm.availabilityTimes = await vm.$store.dispatch('getAvailabilityTimes');
    vm.days = await vm.$store.dispatch('getDays');
    vm.lifts = await vm.$store.dispatch('getLifts');
    vm.languages = await vm.$store.dispatch('getLanguages');
    vm.skills = await vm.$store.dispatch('getSkills');
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
        vm.filteredSkills = vm.skills.filter((option: CatalogItem) =>
          option.skill?.toLowerCase().includes(text.toLowerCase())
        );
      } else {
        vm.filteredSkills = vm.skills;
      }
    },
    getFilteredLanguages(text: string): void {
      const vm = this as any;
      if (text) {
        vm.filteredLanguages = vm.languages.filter((option: CatalogItem) =>
          option.value?.toLowerCase().includes(text.toLowerCase())
        );
      } else {
        vm.filteredLanguages = vm.languages;
      }
    }
  },
};
