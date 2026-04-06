interface CompanyState {
  companyRequestFilter: Record<string, unknown> | null;
}

export default {
  namespaced: true,
  state: {
    companyRequestFilter: null
  } as CompanyState,
  mutations: {
    setCompanyRequestFilter(state: CompanyState, data: Record<string, unknown> | null) {
      state.companyRequestFilter = data;
    },
  },
};
