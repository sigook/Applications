import { ref } from 'vue';
import type { Ref } from 'vue';
import { getSalesCompanies } from '@/api/salesApi';
import type { AgencyCompanyListItem } from '@/types/agency';
import { showAlertError } from '@/utils/toast';

const PAGE_SIZE = 50;

export interface SalesCompanySearch {
  companies: Ref<AgencyCompanyListItem[]>;
  isLoading: Ref<boolean>;
  search: (term: string) => Promise<void>;
}

export function useSalesCompanySearch(): SalesCompanySearch {
  const companies = ref<AgencyCompanyListItem[]>([]);
  const isLoading = ref(false);
  let lastTerm: string | null = null;
  let requestId = 0;

  async function search(term: string): Promise<void> {
    const normalized = term.trim();
    if (normalized === lastTerm) return;
    lastTerm = normalized;
    const current = ++requestId;
    isLoading.value = true;
    try {
      const result = await getSalesCompanies({
        pageIndex: 0,
        pageSize: PAGE_SIZE,
        businessInfo: normalized || undefined,
      });
      if (current !== requestId) return;
      companies.value = result.items;
    } catch (error) {
      if (current === requestId) {
        lastTerm = null;
        showAlertError(error);
      }
    } finally {
      if (current === requestId) isLoading.value = false;
    }
  }

  return { companies, isLoading, search };
}
