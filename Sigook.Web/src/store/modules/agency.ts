import Vue from "vue";

export interface AgencyState {
  agency: any;
  personnelAgencies: any[];
  agencyRequestFilter: any;
  agencyCandidateFilter: any;
  agencyWorkerProfileFilter: any;
  agencyCompanyProfileFilter: any;
  agencyInvoiceFilter: any;
  agencyPayStubFilter: any;
  agencyListFilter: any;
}

export default {
  namespaced: true,
  state: {
    agency: {},
    personnelAgencies: [],
    agencyRequestFilter: null,
    agencyCandidateFilter: null,
    agencyWorkerProfileFilter: null,
    agencyCompanyProfileFilter: null,
    agencyInvoiceFilter: null,
    agencyPayStubFilter: null,
    agencyListFilter: null,
  } as AgencyState,
  mutations: {
    setAgency(state: AgencyState, data: any) {
      state.agency = {
        ...data,
        agencies: data.agencies || (state.agency && state.agency.agencies) || [],
        usaAgency: data.locations.some((l: any) => l.isUSA),
        masterAgency: data.agencyType === (Vue.prototype as any).$agencyTypeMaster
      }
    },
    setPersonnelAgencies(state: AgencyState, data: any) {
      state.agency.agencies = data;
    },
    setAgencyRequestFilter(state: AgencyState, data: any) {
      state.agencyRequestFilter = data;
    },
    setAgencyCandidateFilter(state: AgencyState, data: any) {
      state.agencyCandidateFilter = data;
    },
    setAgencyWorkerProfileFilter(state: AgencyState, data: any) {
      state.agencyWorkerProfileFilter = data;
    },
    setAgencyCompanyProfileFilter(state: AgencyState, data: any) {
      state.agencyCompanyProfileFilter = data;
    },
    setAgencyInvoiceFilter(state: AgencyState, data: any) {
      state.agencyInvoiceFilter = data;
    },
    setAgencyPayStubFilter(state: AgencyState, data: any) {
      state.agencyPayStubFilter = data;
    },
    setAgencyListFilter(state: AgencyState, data: any) {
      state.agencyListFilter = data;
    },
  },
  actions: {
    updateAgencyRequestFilter(context: any, data: any) {
      context.commit("setAgencyRequestFilter", data);
    },
    updateAgencyCandidateFilter(context: any, data: any) {
      context.commit("setAgencyCandidateFilter", data);
    },
    updateAgencyWorkerProfileFilter(context: any, data: any) {
      context.commit("setAgencyWorkerProfileFilter", data);
    },
    updateAgencyCompanyProfileFilter(context: any, data: any) {
      context.commit("setAgencyCompanyProfileFilter", data);
    },
    updateAgencyInvoiceFilter(context: any, data: any) {
      context.commit("setAgencyInvoiceFilter", data);
    },
    updateAgencyPayStubFilter(context: any, data: any) {
      context.commit("setAgencyPayStubFilter", data);
    },
    updateAgencyListFilter(context: any, data: any) {
      context.commit("setAgencyListFilter", data);
    },
  },
};
