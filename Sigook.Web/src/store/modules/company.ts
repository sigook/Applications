import type { CompanyRequestFilter } from '@/types/company';

interface CompanyState {
  companyRequestFilter: CompanyRequestFilter | null;
}

export default {
  namespaced: true,
  state: {
    companyRequestFilter: null
  } as CompanyState,
  mutations: {
    setCompanyRequestFilter(state: CompanyState, data: CompanyRequestFilter | null) {
      state.companyRequestFilter = data;
    },
  },
};
