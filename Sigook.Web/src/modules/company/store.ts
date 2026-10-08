import { defineStore } from 'pinia';
import type { CompanyRequestFilter } from '@/modules/company/requests/types';

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
