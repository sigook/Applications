import { computed, ComputedRef } from 'vue';
import { useRoute } from 'vue-router';

export function useModuleBase(): {
  isSalesView: ComputedRef<boolean>;
  requestBase: ComputedRef<string>;
  companyBase: ComputedRef<string>;
} {
  const route = useRoute();
  const isSalesView = computed(() => route.path.startsWith('/sales'));
  const requestBase = computed(() => (isSalesView.value ? '/sales/requests' : '/recruiting/requests'));
  const companyBase = computed(() => (isSalesView.value ? '/sales/companies' : '/recruiting/companies'));

  return { isSalesView, requestBase, companyBase };
}
