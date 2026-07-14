import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/stores/security';
import { accountingAccess } from '@/security/roles';

export function useAccountingAdmin(): { isAccountingManager: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const isAccountingManager = computed(() =>
    securityStore.userRoles.some((ur: string) => accountingAccess.includes(ur))
  );

  return { isAccountingManager };
}
