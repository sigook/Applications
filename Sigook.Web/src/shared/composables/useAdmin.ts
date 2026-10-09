import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/app/stores/security';
import { adminAccess } from '@/app/security/roles';

export function useAdmin(): { isAdmin: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const isAdmin = computed(() =>
    securityStore.userRoles.some((ur: string) => adminAccess.includes(ur))
  );

  return { isAdmin };
}
