import { Location } from './common';

export enum CompanyStatus {
  Lead = 'Lead',
  Potential = 'Potential',
  Prospect = 'Prospect',
  Quoted = 'Quoted',
  Client = 'Client',
  Blocked = 'Blocked',
  Inactive = 'Inactive',
}

export interface CompanyProfile {
  id: string;
  agencyId: string;
  businessName: string;
  dbaName: string;
  businessNumber: string;
  hstNumber: string;
  companyStatus: CompanyStatus;
  email: string;
  phoneNumber: string;
  website: string;
  requiresPermissionToSeeOrders: boolean;
  createdAt: string;
  fullName: string;
  logo: string | null;
  locations: CompanyProfileLocation[];
  jobPositionRates: CompanyProfileJobPositionRate[];
  contactPersons: CompanyProfileContactPerson[];
}

export interface CompanyProfileLocation {
  id: string;
  companyProfileId: string;
  name: string;
  locationId: string;
  location?: Location;
  isActive: boolean;
}

export interface CompanyProfileJobPositionRate {
  id: string;
  companyProfileId: string;
  jobTitle: string;
  workerRate: number;
  agencyRate: number;
  nightShiftRate: number | null;
  holidayRate: number | null;
  overtimeRate: number | null;
  currency: string;
  isActive: boolean;
}

export interface CompanyProfileContactPerson {
  id: string;
  companyProfileId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  position: string;
  isPrimaryContact: boolean;
}

export interface CompanyUser {
  id: string;
  companyProfileId: string;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  canSeeOrders: boolean;
  isActive: boolean;
}

export interface CompanyFilter {
  page: number;
  pageSize: number;
  searchTerm?: string;
  companyStatus?: CompanyStatus;
  sortBy?: string;
  sortDesc?: boolean;
}
