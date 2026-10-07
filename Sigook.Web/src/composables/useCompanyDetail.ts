import { computed, type Ref } from 'vue';
import { CompanyStatus } from '@/constants/enums';
import { date } from '@/utils/filters';
import type { CompanyProfileDetail } from '@/types/company';
import type { DetailChip, DetailChipVariant } from '@/types/detailPage';

const statusVariants: Partial<Record<CompanyStatus, DetailChipVariant>> = {
  [CompanyStatus.Client]: 'is-success',
  [CompanyStatus.Blocked]: 'is-danger',
  [CompanyStatus.Lead]: 'is-info',
  [CompanyStatus.Potential]: 'is-info',
  [CompanyStatus.Prospect]: 'is-info',
  [CompanyStatus.Quoted]: 'is-info',
};

export function companyIndustryLabel(company?: CompanyProfileDetail | null): string {
  return company?.industry?.industry?.value || company?.industry?.otherIndustry || '';
}

export function companyWebsiteUrl(website?: string | null): string {
  if (!website) return '';
  return website.startsWith('http') ? website : `https://${website}`;
}

export function useCompanyDetail(company: Ref<CompanyProfileDetail | null>) {
  const statusLabel = computed(() => {
    const status = company.value?.companyStatus as unknown as CompanyStatus | undefined;
    return status ? CompanyStatus[status] ?? '' : '';
  });

  const statusVariant = computed<DetailChipVariant>(() =>
    statusVariants[company.value?.companyStatus as unknown as CompanyStatus] ?? '');

  const industry = computed(() => companyIndustryLabel(company.value));

  const vaccinationChip = computed<DetailChip[]>(() =>
    company.value?.vaccinationRequired ? [{ label: 'Vaccination required', variant: 'is-warning' }] : []);

  const createdAt = computed(() => (company.value?.createdAt ? date(company.value.createdAt) : ''));

  return { statusLabel, statusVariant, industry, vaccinationChip, createdAt };
}
