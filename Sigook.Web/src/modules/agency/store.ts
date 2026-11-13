import { defineStore } from 'pinia';
import { appGlobals } from '@/app/globals';
import type { AgencyDetail, AgencyOption } from '@/modules/agency/profile/types';
import type { AgencyListFilter } from '@/modules/agency/sales/agencies/types';
import type { AgencyRequestFilter } from '@/modules/agency/recruiting/requests/types';
import type { AgencyApplicantsFilter } from '@/modules/agency/recruiting/applicants/types';
import type { AgencyRunnersFilter } from '@/modules/agency/recruiting/runners/types';
import type { AgencyWorkerFilter } from '@/modules/agency/recruiting/workers/types';
import type { AgencyCompanyFilter } from '@/modules/agency/recruiting/clients/types';
import type { AgencyCandidateFilter } from '@/modules/agency/recruiting/candidates/types';
import type { AgencyInvoiceFilter } from '@/modules/agency/accounting/invoices/types';
import type { AgencyPayStubFilter } from '@/modules/agency/accounting/paystubs/types';

interface AgencyStoreState {
  agency: AgencyDetail;
  agencies: AgencyOption[];
  agencyRequestFilter: AgencyRequestFilter | null;
  agencyApplicantsFilter: AgencyApplicantsFilter | null;
  agencyRunnersFilter: AgencyRunnersFilter | null;
  agencyCandidateFilter: AgencyCandidateFilter | null;
  agencyWorkerProfileFilter: AgencyWorkerFilter | null;
  agencyCompanyProfileFilter: AgencyCompanyFilter | null;
  agencyInvoiceFilter: AgencyInvoiceFilter | null;
  agencyPayStubFilter: AgencyPayStubFilter | null;
  agencyListFilter: AgencyListFilter | null;
}

export const useAgencyStore = defineStore('agency', {
  state: (): AgencyStoreState => ({
    // Starts as an empty shell (not null) because multiple callers access
    // `agencyStore.agency.*` directly without null checks. Populated by setAgency
    // once the user's profile loads.
    agency: {} as AgencyDetail,
    agencies: [],
    agencyRequestFilter: null,
    agencyApplicantsFilter: null,
    agencyRunnersFilter: null,
    agencyCandidateFilter: null,
    agencyWorkerProfileFilter: null,
    agencyCompanyProfileFilter: null,
    agencyInvoiceFilter: null,
    agencyPayStubFilter: null,
    agencyListFilter: null,
  }),
  actions: {
    setAgency(data: AgencyDetail) {
      this.agency = {
        ...data,
        masterAgency: data.agencyType === appGlobals.$agencyTypeMaster,
      };
    },
    setAgencies(data: AgencyOption[]) {
      this.agencies = data;
    },
    updateAgencyRequestFilter(data: AgencyRequestFilter | null) {
      this.agencyRequestFilter = data;
    },
    updateAgencyApplicantsFilter(data: AgencyApplicantsFilter | null) {
      this.agencyApplicantsFilter = data;
    },
    updateAgencyRunnersFilter(data: AgencyRunnersFilter | null) {
      this.agencyRunnersFilter = data;
    },
    updateAgencyCandidateFilter(data: AgencyCandidateFilter | null) {
      this.agencyCandidateFilter = data;
    },
    updateAgencyWorkerProfileFilter(data: AgencyWorkerFilter | null) {
      this.agencyWorkerProfileFilter = data;
    },
    updateAgencyCompanyProfileFilter(data: AgencyCompanyFilter | null) {
      this.agencyCompanyProfileFilter = data;
    },
    updateAgencyInvoiceFilter(data: AgencyInvoiceFilter | null) {
      this.agencyInvoiceFilter = data;
    },
    updateAgencyPayStubFilter(data: AgencyPayStubFilter | null) {
      this.agencyPayStubFilter = data;
    },
    updateAgencyListFilter(data: AgencyListFilter | null) {
      this.agencyListFilter = data;
    },
  },
});
