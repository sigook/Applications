// Deal types. Mirror backend Covenant.Common.Models.Company.
// The API (System.Text.Json, no JsonStringEnumConverter) serializes enums as
// their NUMERIC value, so these must match the backend int values exactly.

export enum DealType {
  Temporal = 0,
  Permanent = 1,
  TempToPerm = 2,
}

export enum DealStatus {
  ToSend = 0,
  Sent = 1,
  Rejected = 2,
  Accepted = 3,
}

// GetDealsSortBy on the backend: Date=0, Company=1, Value=2, Status=3.
export enum DealSortBy {
  Date = 0,
  Company = 1,
  Value = 2,
  Status = 3,
}

export const DEAL_TYPE_LABELS: Record<DealType, string> = {
  [DealType.Temporal]: 'Temporal',
  [DealType.Permanent]: 'Permanent',
  [DealType.TempToPerm]: 'Temp to Perm',
};

export const DEAL_TYPES: DealType[] = [DealType.Temporal, DealType.Permanent, DealType.TempToPerm];

export const DEAL_STATUS_LABELS: Record<DealStatus, string> = {
  [DealStatus.ToSend]: 'To Send',
  [DealStatus.Sent]: 'Sent',
  [DealStatus.Rejected]: 'Rejected',
  [DealStatus.Accepted]: 'Accepted',
};

export const DEAL_STATUSES: DealStatus[] = [
  DealStatus.ToSend,
  DealStatus.Sent,
  DealStatus.Rejected,
  DealStatus.Accepted,
];

export const DEAL_STATUS_COLORS: Record<DealStatus, string> = {
  [DealStatus.ToSend]: '#9ad6ff',
  [DealStatus.Sent]: '#21b7ff',
  [DealStatus.Rejected]: '#ff5c5c',
  [DealStatus.Accepted]: '#3eb800',
};

// Mirrors backend DealListModel.
export interface Deal {
  id: string;
  title: string;
  companyProfileId: string;
  companyName: string;
  ownerId: string;
  ownerName?: string;
  date: string;
  value: number;
  type: DealType;
  status: DealStatus;
  documentId?: string | null;
  documentName?: string | null;
}

// Filter for GET .../deals. Mirrors backend GetDealsFilter.
// OwnerId is forced server-side to the current sales user, so it is not exposed here.
export interface DealFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: DealSortBy;
  companyProfileId?: string | null;
  type?: DealType | null;
  statuses?: DealStatus[];
  dateFrom?: string | null;
  dateTo?: string | null;
}

// Body for POST. Mirrors backend CreateDealModel.
export interface CreateDealModel {
  title: string;
  companyProfileId: string;
  date: string;
  value: number;
  type: DealType;
  status: DealStatus;
  documentId?: string | null;
}

// Body for PUT .../{id}. Mirrors backend UpdateDealModel.
export interface UpdateDealModel {
  title: string;
  date: string;
  value: number;
  type: DealType;
  status: DealStatus;
  documentId?: string | null;
}
