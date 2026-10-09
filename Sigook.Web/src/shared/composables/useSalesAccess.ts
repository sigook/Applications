import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/app/stores/security';
import { salesAccess } from '@/app/security/roles';

export function useSalesAccess(): { hasSalesAccess: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const hasSalesAccess = computed(() =>
    securityStore.userRoles.some((ur: string) => salesAccess.includes(ur))
  );

  return { hasSalesAccess };
}
