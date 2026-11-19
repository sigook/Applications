import { describe, expect, it } from 'vitest';
import { effectScope, ref } from 'vue';
import { companyIndustryLabel, companyWebsiteUrl, useCompanyDetail } from '@/shared/company-profile/useCompanyDetail';
import { CompanyStatus } from '@/shared/constants/enums';
import type { CompanyProfileDetail } from '@/shared/company-profile/types';

function company(overrides: Partial<CompanyProfileDetail> = {}): CompanyProfileDetail {
  return {
    id: 'c1',
    numberId: 1,
    companyId: 'u1',
    fullName: 'Acme',
    phone: null,
    phoneExt: null,
    fax: null,
    faxExt: null,
    email: 'acme@example.com',
    website: 'acme.com',
    about: '',
    internalInfo: '',
    companyStatus: CompanyStatus.Client,
    active: true,
    paidHolidays: false,
    requiredPaymentMethod: false,
    createdAt: '2026-03-15T12:00:00',
    vaccinationRequired: null,
    vaccinationRequiredComments: '',
    requiresPermissionToSeeRequests: false,
    logo: null,
    industry: null,
    salesRepresentativeId: null,
    agencyId: null,
    overtimeStartsAfter: 44,
    paymentTermDays: 30,
    ...overrides,
  };
}

function setup(initial: CompanyProfileDetail | null) {
  const source = ref<CompanyProfileDetail | null>(initial);
  const scope = effectScope();
  const detail = scope.run(() => useCompanyDetail(source));
  if (!detail) throw new Error('scope did not run');
  return { source, detail };
}

describe('companyIndustryLabel', () => {
  it('prefers the catalog industry', () => {
    expect(
      companyIndustryLabel(company({ industry: { industry: { id: 'i1', value: 'Logistics' }, otherIndustry: 'Other' } })),
    ).toBe('Logistics');
  });

  it('falls back to the free-text industry', () => {
    expect(companyIndustryLabel(company({ industry: { industry: null, otherIndustry: 'Bakery' } }))).toBe('Bakery');
  });

  it.each([[undefined], [null], [company()], [company({ industry: { industry: null } })]])(
    'returns empty for case %#',
    (value) => {
      expect(companyIndustryLabel(value)).toBe('');
    },
  );
});

describe('companyWebsiteUrl', () => {
  it.each([
    [undefined, ''],
    [null, ''],
    ['', ''],
    ['acme.com', 'https://acme.com'],
    ['http://acme.com', 'http://acme.com'],
    ['https://acme.com', 'https://acme.com'],
  ])('%j -> %j', (input, expected) => {
    expect(companyWebsiteUrl(input)).toBe(expected);
  });
});

describe('useCompanyDetail', () => {
  it('is empty without a company', () => {
    const { detail } = setup(null);

    expect(detail.statusLabel.value).toBe('');
    expect(detail.statusVariant.value).toBe('');
    expect(detail.industry.value).toBe('');
    expect(detail.vaccinationChip.value).toEqual([]);
    expect(detail.createdAt.value).toBe('');
  });

  it.each([
    [CompanyStatus.Client, 'Client', 'is-success'],
    [CompanyStatus.Blocked, 'Blocked', 'is-danger'],
    [CompanyStatus.Lead, 'Lead', 'is-info'],
    [CompanyStatus.Potential, 'Potential', 'is-info'],
    [CompanyStatus.Prospect, 'Prospect', 'is-info'],
    [CompanyStatus.Quoted, 'Quoted', 'is-info'],
    [CompanyStatus.Inactive, 'Inactive', ''],
  ])('status %s -> %s / %j', (status, label, variant) => {
    const { detail } = setup(company({ companyStatus: status }));

    expect(detail.statusLabel.value).toBe(label);
    expect(detail.statusVariant.value).toBe(variant);
  });

  it('returns an empty label for an unknown status', () => {
    const { detail } = setup(company({ companyStatus: 99 as CompanyStatus }));

    expect(detail.statusLabel.value).toBe('');
    expect(detail.statusVariant.value).toBe('');
  });

  it('shows the vaccination chip only when required', () => {
    const { source, detail } = setup(company({ vaccinationRequired: true }));
    expect(detail.vaccinationChip.value).toEqual([{ label: 'Vaccination required', variant: 'is-warning' }]);

    source.value = company({ vaccinationRequired: false });
    expect(detail.vaccinationChip.value).toEqual([]);
  });

  it('formats the creation date and reacts to changes', () => {
    const { source, detail } = setup(company({ industry: { industry: { id: 'i', value: 'Retail' } } }));

    expect(detail.createdAt.value).toBe('Sun 15 March, 2026');
    expect(detail.industry.value).toBe('Retail');

    source.value = company({ createdAt: '', companyStatus: CompanyStatus.Blocked });
    expect(detail.createdAt.value).toBe('');
    expect(detail.statusLabel.value).toBe('Blocked');
  });
});
