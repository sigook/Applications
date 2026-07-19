// Company interaction types. Mirror backend Covenant.Common.Models.Company.
// The API (System.Text.Json, no JsonStringEnumConverter) serializes enums as
// their NUMERIC value, so these must match the backend int values exactly.

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
  Company = 1,
  Status = 2,
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
  ownerName?: string;
  description: string;
  interactionPurpose: InteractionPurpose;
  interactionType: InteractionType;
  interactionStatus: InteractionStatus;
  createdAt: string;
  updatedAt: string;
}

// Filter for GET .../companyinteractions. Mirrors backend GetCompanyInteractionsFilter.
// OwnerId is forced server-side to the current sales user, so it is not exposed here.
export interface CompanyInteractionFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: CompanyInteractionSortBy;
  companyProfileId?: string | null;
  interactionPurpose?: InteractionPurpose | null;
  interactionType?: InteractionType | null;
  statuses?: InteractionStatus[];
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
}

// Body for POST. Mirrors backend CreateCompanyInteractionModel.
export interface CreateCompanyInteractionModel {
  companyProfileId: string;
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
