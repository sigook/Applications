import { CompanyStatus } from '@/shared/constants/enums';
import type { AgencyCompanyProfileDetail } from '@/modules/agency/recruiting/clients/types';
import type { CompanyProfileSummary } from '@/shared/company-profile/types';

export function createCompanySummary(overrides: Partial<CompanyProfileSummary> = {}): CompanyProfileSummary {
  return {
    openRequestsCount: 4,
    asapRequestsCount: 0,
    workersWorkingCount: 12,
    rolesCount: 3,
    contactsCount: 2,
    usersCount: 1,
    locationsCount: 2,
    documentsCount: 5,
    salesRepresentativeName: null,
    ...overrides,
  };
}

export function createAgencyCompany(overrides: Partial<AgencyCompanyProfileDetail> = {}): AgencyCompanyProfileDetail {
  return {
    id: 'company-profile-1',
    numberId: 501,
    companyId: 'company-user-1',
    fullName: 'ACME LOGISTICS',
    phone: '416 555-0100',
    phoneExt: null,
    fax: null,
    faxExt: null,
    email: 'ops@acme.com',
    website: 'www.acme.com',
    about: 'Warehouse operator',
    internalInfo: '',
    companyStatus: CompanyStatus.Client,
    active: true,
    paidHolidays: false,
    requiredPaymentMethod: false,
    createdAt: '2026-03-10T15:00:00Z',
    vaccinationRequired: false,
    vaccinationRequiredComments: '',
    requiresPermissionToSeeRequests: false,
    logo: { id: 'logo-1', pathFile: 'https://files/logo.png' },
    industry: { id: 'ind-1', industry: { id: 'i1', value: 'Logistics' } },
    salesRepresentativeId: null,
    agencyId: 'agency-1',
    overtimeStartsAfter: 44,
    paymentTermDays: 30,
    summary: createCompanySummary(),
    ...overrides,
  };
}
