import { DealStatus, DealType, InteractionPurpose, InteractionStatus, InteractionType } from '@/modules/agency/sales/clients/types';
import type { CompanyInteraction, Deal } from '@/modules/agency/sales/clients/types';
import type { SalesRecentClient } from '@/modules/agency/sales/dashboard/types';

export function createDeal(overrides: Partial<Deal> = {}): Deal {
  return {
    id: 'deal-1',
    title: 'Warehouse staffing',
    companyProfileId: 'company-1',
    companyName: 'Acme Logistics',
    ownerId: 'user-1',
    owner: 'Rita Sales',
    date: '2026-11-10T12:00:00',
    value: 25000,
    type: DealType.Temporal,
    status: DealStatus.Sent,
    documentId: null,
    documentName: null,
    documentPath: null,
    createdAt: '2026-11-01T12:00:00',
    updatedAt: '2026-11-01T12:00:00',
    ...overrides,
  };
}

export function createInteraction(overrides: Partial<CompanyInteraction> = {}): CompanyInteraction {
  return {
    id: 'int-1',
    companyProfileId: 'company-1',
    companyName: 'Acme Logistics',
    ownerId: 'user-1',
    owner: 'Rita Sales',
    description: 'Called the plant manager',
    interactionPurpose: InteractionPurpose.Intro,
    interactionType: InteractionType.Call,
    interactionStatus: InteractionStatus.Completed,
    createdAt: '2026-11-19T09:00:00',
    updatedAt: '2026-11-19T09:00:00',
    ...overrides,
  };
}

export function createRecentClient(overrides: Partial<SalesRecentClient> = {}): SalesRecentClient {
  return {
    id: 'company-1',
    fullName: 'Acme Logistics',
    email: 'ap@acme.test',
    lastInteractionAt: '2026-11-19T07:00:00',
    ...overrides,
  };
}
