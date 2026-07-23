import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/stores/security';
import { administrationAccess } from '@/security/roles';

export function useAdministration(): { isAdministration: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const isAdministration = computed(() =>
    securityStore.userRoles.some((ur: string) => administrationAccess.includes(ur))
  );

  return { isAdministration };
}
