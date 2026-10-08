import type { City, LocationDetailModel, Province } from '@/shared/types/common';

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
  location?: LocationDetailModel;
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

// Filter used by Agencies list page
export interface AgencyTypeOption {
  value: number;
  label: string;
}

// Detail returned by GET /api/agency/sales/agencies/{id} and used by edit pages.
// Modeled to be permissive: many fields come from the API and are
// only used by specific tabs/components.
export interface AgencyDetail {
  id: string;
  fullName: string;
  hstNumber?: string;
  businessNumber?: string;
  phonePrincipal?: string;
  phonePrincipalExt?: number | null;
  webPage?: string;
  wsibGroup?: { value: string }[];
  contactInformation?: AgencyContactInformation[];
  locations?: AgencyLocationDetail[];
  agencies?: PersonnelAgencyItem[];
  agencyType?: AgencyType;
  logo?: { fileName?: string; pathFile?: string } | null;
  usaAgency?: boolean;
  masterAgency?: boolean;
  profileImage?: string | null;
}

// Mirrors backend AgencyContactInformationModel (Covenant.Common.Models.Agency).
export interface AgencyContactInformation {
  id?: string;
  title?: string;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  position?: string;
  mobileNumber?: string;
  officeNumber?: string;
  officeNumberExt?: number | null;
  email?: string;
}

// Used by ProfileBilling.vue and the /api/agency/profile/locations endpoint.
// Fields are flat (not wrapped in a Location object).
export interface AgencyLocationDetail {
  id?: string;
  address: string;
  city: City;
  province?: Province;
  postalCode: string;
  formattedAddress?: string;
  isBilling?: boolean;
}

// Body used by AgencyPersonnelModal.vue (POST /api/agency/profile/personnel, PUT /api/agency/profile/personnel/{id})
export interface AgencyPersonnelCreateModel {
  name: string | null;
  email: string | null;
  role: string | null;
}

// Item returned by GET /api/agency/profile/personnel
export interface AgencyPersonnelListItem {
  id: string;
  userId: string;
  name: string;
  email: string;
  role: string;
}

// Mirrors backend AttendanceStatus
export enum AttendanceStatus {
  NotStarted = 0,
  ClockedIn = 1,
  ClockedOut = 2,
}

// GET /api/agency/profile/attendance/today, POST /api/agency/profile/attendance, GET /api/agency/profile/attendance/today/users
export interface UserAttendanceToday {
  userId: string;
  status: AttendanceStatus;
  clockIn: string | null;
  clockOut: string | null;
  workedHours: number | null;
}

export interface UserAttendanceHours {
  workedHours: number;
  lunchHours: number;
  regularHours: number;
  overtimeHours: number;
}

export interface UserAttendanceListItem extends UserAttendanceHours {
  id: string;
  userId: string;
  name: string;
  date: string;
  clockIn: string;
  clockOut: string | null;
  lunchMinutes: number;
  isWeekend: boolean;
  isMissingClockOut: boolean;
  isEdited: boolean;
  editedBy: string | null;
  editedAt: string | null;
  editReason: string | null;
}

// GET /api/agency/profile/attendance/report
export interface UserAttendanceReport {
  items: UserAttendanceListItem[];
  totals: UserAttendanceHours;
}

export interface UserAttendanceReportFilter {
  userId?: string;
  from: string;
  to: string;
}

// PUT /api/agency/profile/attendance/{id}
export interface UpdateUserAttendanceModel {
  clockIn: string;
  clockOut: string | null;
  lunchMinutes: number;
  reason: string;
}

// Item returned by GET /api/agency/profile/personnel/agencies. Mirrors backend PersonnelAgencyModel.
export interface PersonnelAgencyItem {
  id: string;
  name: string;
  email: string;
  agencyId: string;
  isPrimary: boolean;
  logo?: string | null;
}
