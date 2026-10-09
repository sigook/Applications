import type { DetailChipVariant, DetailFact, DetailKpi } from '@/shared/detail-page/types';
import type { ComplianceDocumentTarget } from '@/shared/request-detail/requestApplicant';
import type { LocationDetailModel } from '@/shared/types/common';

export type RequestChipVariant = DetailChipVariant;

export type RequestFact = DetailFact;

export type RequestKpi = DetailKpi;

export interface RequestStatusSource {
  status: number;
  workersQuantity: number;
  workersQuantityWorking: number;
}

export interface RequestStaffingItem {
  key: string;
  title: string;
  detail?: string;
  variant: 'is-warning' | 'is-neutral';
  actionLabel?: string;
}

// Compliance requirement configured on a request. Mirrors backend RequestComplianceItemModel.
export interface RequestComplianceItem {
  id?: string | null;
  name: string;
  isMandatory: boolean;
  documentTarget: ComplianceDocumentTarget;
}

// Payload for POST/PUT /api/AgencyRequest.
// Mirrors backend RequestCreateModel.
export interface CreateAgencyRequestModel {
  jobTitle: string;
  billingTitle?: string;
  jobCosting?: string;
  workersQuantity: number;
  description?: string;
  durationBreak: string;
  breakIsPaid: boolean;
  incentive?: number | null;
  incentiveDescription?: string;
  requirements?: string;
  internalRequirements?: string;
  responsibilities?: string;
  isAsap: boolean;
  usesRunners: boolean;
  jobIsOnBranchOffice?: boolean;
  anotherLocation?: LocationDetailModel | null;
  locationId?: string | null;
  jobPositionRateId?: string | null;
  durationTerm: number;
  employmentType: number;
  companyProfileId: string;
  agencyId?: string;
  startAt: string;
  finishAt?: string | null;
  shift?: RequestShiftModel | null;
  punchCardOptionEnabled: boolean;
  workerSalary?: number | null;
  salesRepresentativeId?: string | null;
  companyUserIds?: string[];
  complianceItems?: RequestComplianceItem[];
}

// Payload for PUT /api/AgencyRequest/{id}/Shift. Mirrors backend ShiftModel.
export interface RequestShiftModel {
  sunday?: boolean | null;
  monday?: boolean | null;
  tuesday?: boolean | null;
  wednesday?: boolean | null;
  thursday?: boolean | null;
  friday?: boolean | null;
  saturday?: boolean | null;
  sundayStart?: string | null;
  sundayFinish?: string | null;
  mondayStart?: string | null;
  mondayFinish?: string | null;
  tuesdayStart?: string | null;
  tuesdayFinish?: string | null;
  wednesdayStart?: string | null;
  wednesdayFinish?: string | null;
  thursdayStart?: string | null;
  thursdayFinish?: string | null;
  fridayStart?: string | null;
  fridayFinish?: string | null;
  saturdayStart?: string | null;
  saturdayFinish?: string | null;
  comments?: string;
}
