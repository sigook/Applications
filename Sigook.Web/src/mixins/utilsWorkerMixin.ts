import { getSkills, fetchLanguages } from '@/api/catalogApi';
import type { Skill, Language } from '@/types/common';
import type { WorkerExperienceForm, WorkerDocumentFile } from '@/types/worker';

export default {
  data() {
    return {
      experiences: [] as WorkerExperienceForm[],
      licenses: [] as WorkerDocumentFile[],
      certificates: [] as WorkerDocumentFile[],
      resume: null as WorkerDocumentFile | null,
      filteredSkills: [] as Skill[],
      filteredLanguages: [] as Language[]
    };
  },
  methods: {
    addTag(newTag: string): void {
      (this as any).filteredSkills.push({ skill: newTag });
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
      (this as any).experiences.forEach((item: WorkerExperienceForm, indexItem: number) => {
        if (index !== indexItem) {
          item.currentJob = false;
        }
      });
    },
    disableEndDate(index: number): void {
      (this as any).experiences.forEach((item: WorkerExperienceForm, indexItem: number) => {
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
    vm.filteredSkills = await getSkills();
    vm.filteredLanguages = await fetchLanguages();
  }
};
