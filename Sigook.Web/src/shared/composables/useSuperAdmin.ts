import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/app/stores/security';
import roles from '@/app/security/roles';

export function useSuperAdmin(): { isSuperAdmin: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const isSuperAdmin = computed(() =>
    securityStore.userRoles.some((ur: string) => ur === roles.superAdmin)
  );

  return { isSuperAdmin };
}
