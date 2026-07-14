import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/stores/security';
import { recruitingAccess } from '@/security/roles';

export function useRecruitingAccess(): { hasRecruitingAccess: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const hasRecruitingAccess = computed(() =>
    securityStore.userRoles.some((ur: string) => recruitingAccess.includes(ur))
  );

  return { hasRecruitingAccess };
}
