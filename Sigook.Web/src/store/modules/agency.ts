import Vue from "vue";
import type { ActionContext } from "vuex";
import type { RootState } from "@/store";
import type {
  AgencyProfile,
  AgencyListFilter,
  AgencyRequestFilter,
  AgencyWorkerFilter,
  AgencyCompanyFilter,
} from "@/types/agency";
import type { AgencyCandidateFilter } from "@/types/candidate";
import type { AgencyInvoiceFilter, AgencyPayStubFilter } from "@/types/accounting";

export interface AgencyState {
  agency: AgencyProfile | null;
  personnelAgencies: AgencyProfile[];
  agencyRequestFilter: AgencyRequestFilter | null;
  agencyCandidateFilter: AgencyCandidateFilter | null;
  agencyWorkerProfileFilter: AgencyWorkerFilter | null;
  agencyCompanyProfileFilter: AgencyCompanyFilter | null;
  agencyInvoiceFilter: AgencyInvoiceFilter | null;
  agencyPayStubFilter: AgencyPayStubFilter | null;
  agencyListFilter: AgencyListFilter | null;
}

type AgencyActionContext = ActionContext<AgencyState, RootState>;

export default {
  namespaced: true,
  state: {
    agency: null,
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
    setAgency(state: AgencyState, data: AgencyProfile) {
      state.agency = {
        ...data,
        agencies: data.agencies || (state.agency && state.agency.agencies) || [],
        // The backend flattens an `isUSA` flag onto each location in this specific
        // endpoint's response — not part of the AgencyLocation shape used elsewhere.
        usaAgency: data.locations.some(
          (l) => (l as unknown as { isUSA?: boolean }).isUSA === true
        ),
        masterAgency: data.agencyType === (Vue.prototype as { $agencyTypeMaster?: number }).$agencyTypeMaster,
      };
    },
    setPersonnelAgencies(state: AgencyState, data: AgencyProfile[]) {
      if (state.agency) {
        state.agency.agencies = data;
      }
    },
    setAgencyRequestFilter(state: AgencyState, data: AgencyRequestFilter | null) {
      state.agencyRequestFilter = data;
    },
    setAgencyCandidateFilter(state: AgencyState, data: AgencyCandidateFilter | null) {
      state.agencyCandidateFilter = data;
    },
    setAgencyWorkerProfileFilter(state: AgencyState, data: AgencyWorkerFilter | null) {
      state.agencyWorkerProfileFilter = data;
    },
    setAgencyCompanyProfileFilter(state: AgencyState, data: AgencyCompanyFilter | null) {
      state.agencyCompanyProfileFilter = data;
    },
    setAgencyInvoiceFilter(state: AgencyState, data: AgencyInvoiceFilter | null) {
      state.agencyInvoiceFilter = data;
    },
    setAgencyPayStubFilter(state: AgencyState, data: AgencyPayStubFilter | null) {
      state.agencyPayStubFilter = data;
    },
    setAgencyListFilter(state: AgencyState, data: AgencyListFilter | null) {
      state.agencyListFilter = data;
    },
  },
  actions: {
    updateAgencyRequestFilter(context: AgencyActionContext, data: AgencyRequestFilter | null) {
      context.commit("setAgencyRequestFilter", data);
    },
    updateAgencyCandidateFilter(context: AgencyActionContext, data: AgencyCandidateFilter | null) {
      context.commit("setAgencyCandidateFilter", data);
    },
    updateAgencyWorkerProfileFilter(context: AgencyActionContext, data: AgencyWorkerFilter | null) {
      context.commit("setAgencyWorkerProfileFilter", data);
    },
    updateAgencyCompanyProfileFilter(context: AgencyActionContext, data: AgencyCompanyFilter | null) {
      context.commit("setAgencyCompanyProfileFilter", data);
    },
    updateAgencyInvoiceFilter(context: AgencyActionContext, data: AgencyInvoiceFilter | null) {
      context.commit("setAgencyInvoiceFilter", data);
    },
    updateAgencyPayStubFilter(context: AgencyActionContext, data: AgencyPayStubFilter | null) {
      context.commit("setAgencyPayStubFilter", data);
    },
    updateAgencyListFilter(context: AgencyActionContext, data: AgencyListFilter | null) {
      context.commit("setAgencyListFilter", data);
    },
  },
};
