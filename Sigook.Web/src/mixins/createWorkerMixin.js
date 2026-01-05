export default {
  data() {
    return {
      isLoading: false,
      skills: [],
      languages: [],
      allDaysSelected: false,
      filteredSkills: [],
      filteredLanguages: [],
      genders: [],
      identificationTypes: [],
      availabilities: [],
      availabilityTimes: [],
      days: [],
      lifts: [],
      worker: {
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
        otherDocuments: []
      }
    }
  },
  async created() {
    this.isLoading = true;
    this.genders = await this.$store.dispatch('getGenders');
    this.identificationTypes = await this.$store.dispatch('getIdentificationTypes');
    this.availabilities = await this.$store.dispatch('getAvailability');
    this.availabilityTimes = await this.$store.dispatch('getAvailabilityTimes');
    this.days = await this.$store.dispatch('getDays');
    this.lifts = await this.$store.dispatch('getLifts');
    this.languages = await this.$store.dispatch('getLanguages');
    this.skills = await this.$store.dispatch('getSkills');
    this.filteredSkills = this.skills;
    this.filteredLanguages = this.languages;
    this.isLoading = false;
  },
  methods: {
    changeDaysSelected() {
      if (this.allDaysSelected) {
        for (let i = 0; i < this.days.length; i++) {
          this.worker.availabilityDays.push(this.days[i]);
        }
      } else {
        this.worker.availabilityDays = []
      }
    },
    changeAllDays() {
      for (let i = 0; i < this.worker.availabilityDays.length; i++) {
        if (this.worker.availabilityDays.length !== this.days.length) {
          this.allDaysSelected = false;
        } else {
          this.allDaysSelected = true;
        }
      }
    },
    changeAllTimes() {
      if (this.worker.availabilityTimes.length !== this.availabilityTimes.length) {
        this.allTimesSelected = false;
      } else {
        this.allTimesSelected = true;
      }
    },
    getFilteredSkills(text) {
      if (text) {
        this.filteredSkills = this.skills.filter((option) => option.skill.toLowerCase().includes(text.toLowerCase()))
      } else {
        this.filteredSkills = this.skills;
      }
    },
    getFilteredLanguages(text) {
      if (text) {
        this.filteredLanguages = this.languages.filter((option) =>
          option.value.toLowerCase().includes(text.toLowerCase())
        );
      } else {
        this.filteredLanguages = this.languages;
      }
    }
  },
} 