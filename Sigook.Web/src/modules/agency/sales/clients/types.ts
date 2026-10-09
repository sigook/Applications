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
  UnderReview = 4,
  Closed = 5,
  Completed = 6,
}

// GetDealsSortBy on the backend: Date=0, Company=1, Value=2, Status=3.
export enum DealSortBy {
  Date = 0,
  Value = 1,
  Status = 2,
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
  [DealStatus.UnderReview]: 'Under Review',
  [DealStatus.Closed]: 'Closed',
  [DealStatus.Completed]: 'Completed',
};

export const DEAL_STATUSES: DealStatus[] = [
  DealStatus.ToSend,
  DealStatus.Sent,
  DealStatus.UnderReview,
  DealStatus.Accepted,
  DealStatus.Rejected,
  DealStatus.Closed,
  DealStatus.Completed,
];

export const DEAL_STATUS_COLORS: Record<DealStatus, string> = {
  [DealStatus.ToSend]: '#9ad6ff',
  [DealStatus.Sent]: '#21b7ff',
  [DealStatus.Rejected]: '#ff5c5c',
  [DealStatus.Accepted]: '#3eb800',
  [DealStatus.UnderReview]: '#ffb020',
  [DealStatus.Closed]: '#8a94a6',
  [DealStatus.Completed]: '#0f9d8a',
};

// Mirrors backend DealListModel.
export interface Deal {
  id: string;
  title: string;
  companyProfileId: string;
  companyName: string;
  ownerId: string;
  owner?: string;
  date: string;
  value: number;
  type: DealType;
  status: DealStatus;
  documentId?: string | null;
  documentName?: string | null;
  documentPath?: string | null;
  createdAt: string;
  updatedAt: string;
}

// Filter for GET .../Deals. Mirrors backend GetDealsFilter.
// ownerId is honoured only for admin/superadmin; for a sales user the backend overwrites it
// with their own user id (SalesService.OwnerScope).
export interface DealFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: DealSortBy;
  ownerId?: string | null;
  type?: DealType | null;
  statuses?: DealStatus[];
  dateFrom?: string | null;
  dateTo?: string | null;
}

// Body for POST. Mirrors backend CreateDealModel.
export interface CreateDealModel {
  title: string;
  date: string;
  value: number;
  type: DealType;
  status: DealStatus;
  documentId?: string | null;
  fileName?: string | null;
}

// Body for PUT .../{id}. Mirrors backend UpdateDealModel.
export interface UpdateDealModel {
  title: string;
  date: string;
  value: number;
  type: DealType;
  status: DealStatus;
  documentId?: string | null;
  fileName?: string | null;
}

export enum InteractionType {
  Call = 0,
  Mail = 1,
  Sms = 2,
  LinkedIn = 3,
}

export enum InteractionPurpose {
  Intro = 0,
  FollowUp = 1,
  Proposal = 2,
  Negotiation = 3,
  Closing = 4,
}

export enum InteractionStatus {
  NotStarted = 0,
  InProgress = 1,
  Completed = 2,
}

// GetCompanyInteractionsSortBy on the backend: CreatedAt=0, Company=1, Status=2.
export enum CompanyInteractionSortBy {
  CreatedAt = 0,
  Status = 1,
}

export const INTERACTION_TYPE_LABELS: Record<InteractionType, string> = {
  [InteractionType.Call]: 'Call',
  [InteractionType.Mail]: 'Email',
  [InteractionType.Sms]: 'SMS',
  [InteractionType.LinkedIn]: 'LinkedIn',
};

export const INTERACTION_TYPES: InteractionType[] = [
  InteractionType.Call,
  InteractionType.Mail,
  InteractionType.Sms,
  InteractionType.LinkedIn,
];

export const INTERACTION_TYPE_ICONS: Record<InteractionType, string> = {
  [InteractionType.Call]: 'phone',
  [InteractionType.Mail]: 'email-outline',
  [InteractionType.Sms]: 'message-text-outline',
  [InteractionType.LinkedIn]: 'linkedin',
};

export const INTERACTION_TYPE_COLORS: Record<InteractionType, string> = {
  [InteractionType.Call]: '#21b7ff',
  [InteractionType.Mail]: '#3eb800',
  [InteractionType.Sms]: '#ff9932',
  [InteractionType.LinkedIn]: '#0a66c2',
};

export const INTERACTION_PURPOSE_LABELS: Record<InteractionPurpose, string> = {
  [InteractionPurpose.Intro]: 'Intro',
  [InteractionPurpose.FollowUp]: 'Follow-up',
  [InteractionPurpose.Proposal]: 'Proposal',
  [InteractionPurpose.Negotiation]: 'Negotiation',
  [InteractionPurpose.Closing]: 'Closing',
};

export const INTERACTION_PURPOSES: InteractionPurpose[] = [
  InteractionPurpose.Intro,
  InteractionPurpose.FollowUp,
  InteractionPurpose.Proposal,
  InteractionPurpose.Negotiation,
  InteractionPurpose.Closing,
];

export const INTERACTION_STATUS_LABELS: Record<InteractionStatus, string> = {
  [InteractionStatus.NotStarted]: 'Not started',
  [InteractionStatus.InProgress]: 'In progress',
  [InteractionStatus.Completed]: 'Completed',
};

export const INTERACTION_STATUSES: InteractionStatus[] = [
  InteractionStatus.NotStarted,
  InteractionStatus.InProgress,
  InteractionStatus.Completed,
];

// Mirrors backend CompanyInteractionListModel.
export interface CompanyInteraction {
  id: string;
  companyProfileId: string;
  companyName: string;
  ownerId: string;
  owner?: string;
  description: string;
  interactionPurpose: InteractionPurpose;
  interactionType: InteractionType;
  interactionStatus: InteractionStatus;
  createdAt: string;
  updatedAt: string;
}

// Filter for GET .../Interactions. Mirrors backend GetCompanyInteractionsFilter.
// ownerId is honoured only for admin/superadmin; for a sales user the backend overwrites it
// with their own user id (SalesService.OwnerScope).
export interface CompanyInteractionFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: CompanyInteractionSortBy;
  ownerId?: string | null;
  interactionPurpose?: InteractionPurpose | null;
  interactionType?: InteractionType | null;
  statuses?: InteractionStatus[];
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
}

// Body for POST. Mirrors backend CreateCompanyInteractionModel.
export interface CreateCompanyInteractionModel {
  description: string;
  interactionPurpose: InteractionPurpose;
  interactionType: InteractionType;
  interactionStatus: InteractionStatus;
}

// Body for PUT .../{id}. Mirrors backend UpdateCompanyInteractionModel.
export interface UpdateCompanyInteractionModel {
  description: string;
  interactionPurpose: InteractionPurpose;
  interactionType: InteractionType;
  interactionStatus: InteractionStatus;
}
