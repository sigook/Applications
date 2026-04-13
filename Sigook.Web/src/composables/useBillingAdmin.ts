import { computed, ComputedRef } from 'vue';
import store from '@/store';
import roles from '@/security/roles';

export function useBillingAdmin(): { isPayrollManager: ComputedRef<boolean> } {
  const isPayrollManager = computed(() =>
    store.state.security.userRoles.some(
      (ur: string) => ur === roles.admin || ur === roles.payroll || ur === roles.agency
    )
  );

  return { isPayrollManager };
}
