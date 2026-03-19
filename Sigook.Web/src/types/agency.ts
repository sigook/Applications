import { Location } from './common';

export enum AgencyType {
  Master = 1,
  Regular = 2,
  BusinessPartner = 3,
}

export interface AgencyProfile {
  id: string;
  fullName: string;
  businessNumber: string;
  hstNumber: string;
  agencyType: AgencyType;
  email: string;
  phoneNumber: string;
  website: string;
  isActive: boolean;
  createdAt: string;
  locations: AgencyLocation[];
  agencies: AgencyProfile[];
  usaAgency: boolean;
  masterAgency: boolean;
}

export interface AgencyLocation {
  id: string;
  agencyId: string;
  name: string;
  locationId: string;
  location?: Location;
  isBillingAddress: boolean;
  isActive: boolean;
}

export enum PersonnelType {
  Recruiter = 'Recruiter',
  SalesRepresentative = 'SalesRepresentative',
  AccountManager = 'AccountManager',
  Administrator = 'Administrator',
}

export interface AgencyPersonnel {
  id: string;
  agencyId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  type: PersonnelType;
  isActive: boolean;
}
