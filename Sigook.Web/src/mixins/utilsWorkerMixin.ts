interface Experience {
  id: string | null;
  companyName: string;
  title: string;
  startDate: Date | null;
  endDate: Date | null;
  currentJob: boolean;
  description: string;
}

interface FileReference {
  id: string | null;
  fileName: string;
}

interface Tag {
  id: string | null;
  value: string;
}

export default {
  data() {
    return {
      experiences: [] as Experience[],
      licenses: [] as FileReference[],
      certificates: [] as FileReference[],
      resume: null as FileReference | null,
      filteredSkills: [] as Tag[],
      filteredLanguages: [] as Tag[]
    };
  },
  methods: {
    addTag(newTag: string): void {
      const tag: Tag = {
        id: null,
        value: newTag
      };
      (this as any).filteredSkills.push(tag);
    },
    validateExperience(index: number): Promise<boolean> {
      const vm = this as any;
      vm.$validator.validateAll()
        .then((results: boolean) => {
          if (!results) {
            vm.errors.items.forEach((item: any) => {
              const iName = item.field.includes(index);
              if (iName) {
                return false;
              }
            });
          } else {
            return true;
          }
        });
      const current = vm.$validator.fields.filter((x: any) => x.name.includes(index));
      return new Promise((resolve) => {
        Promise.all(current.map((field: any) => vm.$validator.validate(field)))
          .then((validatedFields: any[]) => {
            let isValidForm = true;
            validatedFields.forEach((item: any) => {
              if (item.valid === false) {
                isValidForm = false;
              }
            });
            resolve(isValidForm);
          });
      });
    },
    addExperience(): void {
      (this as any).experiences.push({
        id: null,
        companyName: '',
        title: '',
        startDate: null,
        endDate: null,
        currentJob: false,
        description: ''
      });
    },
    addLicense(file: { fileName: string }): void {
      (this as any).licenses.push({
        id: null,
        fileName: file.fileName
      });
    },
    addCertificate(file: { fileName: string }): void {
      (this as any).certificates.push({
        id: null,
        fileName: file.fileName
      });
    },
    deleteLicense(index: number): void {
      (this as any).licenses.splice(index, 1);
    },
    deleteCertificate(index: number): void {
      (this as any).certificates.splice(index, 1);
    },
    verifyAllCurrentJob(index: number): void {
      (this as any).experiences.forEach((item: Experience, indexItem: number) => {
        if (index !== indexItem) {
          item.currentJob = false;
        }
      });
    },
    disableEndDate(index: number): void {
      (this as any).experiences.forEach((item: Experience, indexItem: number) => {
        if (index === indexItem) {
          if (item.currentJob) {
            item.endDate = null;
          }
        }
      });
    },
    removeExperience(index: number): void {
      (this as any).experiences.splice(index, 1);
    },
    addResume(file: { fileName: string }): void {
      (this as any).resume = {
        id: null,
        fileName: file.fileName
      };
    },
    deleteResume(): void {
      (this as any).resume = null;
    }
  },
  async created() {
    const vm = this as any;
    vm.filteredSkills = await vm.$store.dispatch('getSkills');
    vm.filteredLanguages = await vm.$store.dispatch('getLanguages');
  }
};
