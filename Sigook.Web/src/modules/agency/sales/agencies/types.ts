import type { CovenantFileModel, LocationDetailModel, WsibGroup } from '@/shared/types/common';
import { AgencyType, AgencyContactInformation } from '@/modules/agency/profile/types';

export interface AgencyListFilter {
  pageIndex?: number;
  pageSize?: number;
  sortBy?: number;
  isDescending?: boolean;
  fullName?: string;
  email?: string;
  agencyTypes?: number[];
}

// Item returned by GET /api/agency/sales/agencies list
export interface AgencyListItem {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  isActive: boolean;
  agencyType: AgencyType;
  createdAt: string;
}

// Body for POST /api/agency/sales/agencies. Mirrors backend AgencyModel
// (Covenant.Common.Models.Agency.AgencyModel).
export interface CreateAgencyModel {
  id?: string;
  fullName: string;
  hstNumber?: string;
  businessNumber?: string;
  webPage?: string;
  phonePrincipal?: string;
  phonePrincipalExt?: number | null;
  logo?: CovenantFileModel | null;
  email: string;
  agencyType?: AgencyType;
  wsibGroup?: WsibGroup[];
  locations?: LocationDetailModel[];
  contactInformation?: AgencyContactInformation[];
}
