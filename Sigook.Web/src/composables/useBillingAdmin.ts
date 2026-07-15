import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/stores/security';
import { hasAnyRole, roleGroups } from '@/security/roles';

export function useBillingAdmin(): { isPayrollManager: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const isPayrollManager = computed(() =>
    hasAnyRole(securityStore.userRoles, roleGroups.accounting)
  );

  return { isPayrollManager };
}
