import { computed, ComputedRef } from 'vue';
import { useRoute } from 'vue-router';
import type { PageBreadcrumb } from '@/types/common';

export function useModuleBase(): {
  isSalesView: ComputedRef<boolean>;
  requestBase: ComputedRef<string>;
  companyBase: ComputedRef<string>;
  moduleCrumbs: ComputedRef<PageBreadcrumb[]>;
} {
  const route = useRoute();
  const isSalesView = computed(() => route.path.startsWith('/sales'));
  const requestBase = computed(() => (isSalesView.value ? '/sales/requests' : '/recruiting/requests'));
  const companyBase = computed(() => (isSalesView.value ? '/sales/companies' : '/recruiting/companies'));
  const moduleCrumbs = computed<PageBreadcrumb[]>(() => [{ label: isSalesView.value ? 'Sales' : 'Recruiting' }]);

  return { isSalesView, requestBase, companyBase, moduleCrumbs };
}
