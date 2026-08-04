import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/stores/security';
import { adminAccess } from '@/security/roles';

export function useAdmin(): { isAdmin: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const isAdmin = computed(() =>
    securityStore.userRoles.some((ur: string) => adminAccess.includes(ur))
  );

  return { isAdmin };
}
