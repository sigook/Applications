import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/stores/security';
import roles from '@/security/roles';

export function useBillingAdmin(): { isPayrollManager: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const isPayrollManager = computed(() =>
    securityStore.userRoles.some(
      (ur: string) => ur === roles.admin || ur === roles.payroll || ur === roles.agency
    )
  );

  return { isPayrollManager };
}
