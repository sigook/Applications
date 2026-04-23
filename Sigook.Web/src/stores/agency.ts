import { defineStore } from 'pinia';
import { appGlobals } from '@/varaibles';
import type {
  AgencyProfile,
  AgencyListFilter,
  AgencyRequestFilter,
  AgencyWorkerFilter,
  AgencyCompanyFilter,
} from '@/types/agency';
import type { AgencyCandidateFilter } from '@/types/candidate';
import type { AgencyInvoiceFilter, AgencyPayStubFilter } from '@/types/accounting';

interface AgencyStoreState {
  agency: AgencyProfile;
  personnelAgencies: AgencyProfile[];
  agencyRequestFilter: AgencyRequestFilter | null;
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
    agency: {} as AgencyProfile,
    personnelAgencies: [],
    agencyRequestFilter: null,
    agencyCandidateFilter: null,
    agencyWorkerProfileFilter: null,
    agencyCompanyProfileFilter: null,
    agencyInvoiceFilter: null,
    agencyPayStubFilter: null,
    agencyListFilter: null,
  }),
  actions: {
    setAgency(data: AgencyProfile) {
      this.agency = {
        ...data,
        agencies: data.agencies || (this.agency && this.agency.agencies) || [],
        // The backend flattens an `isUSA` flag onto each location in this specific
        // endpoint's response — not part of the AgencyLocation shape used elsewhere.
        usaAgency: data.locations.some(
          (l) => (l as unknown as { isUSA?: boolean }).isUSA === true
        ),
        masterAgency: data.agencyType === appGlobals.$agencyTypeMaster,
      };
    },
    setPersonnelAgencies(data: AgencyProfile[]) {
      if (this.agency) {
        this.agency.agencies = data;
      }
    },
    updateAgencyRequestFilter(data: AgencyRequestFilter | null) {
      this.agencyRequestFilter = data;
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
