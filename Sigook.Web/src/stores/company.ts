import { defineStore } from 'pinia';
import type { CompanyRequestFilter } from '@/types/company';

interface CompanyStoreState {
  companyRequestFilter: CompanyRequestFilter | null;
}

export const useCompanyStore = defineStore('company', {
  state: (): CompanyStoreState => ({
    companyRequestFilter: null,
  }),
  actions: {
    setCompanyRequestFilter(data: CompanyRequestFilter | null) {
      this.companyRequestFilter = data;
    },
  },
});
