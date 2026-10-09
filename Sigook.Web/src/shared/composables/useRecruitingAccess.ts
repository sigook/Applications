import { computed, ComputedRef } from 'vue';
import { useSecurityStore } from '@/app/stores/security';
import { recruitingAccess } from '@/app/security/roles';

export function useRecruitingAccess(): { hasRecruitingAccess: ComputedRef<boolean> } {
  const securityStore = useSecurityStore();
  const hasRecruitingAccess = computed(() =>
    securityStore.userRoles.some((ur: string) => recruitingAccess.includes(ur))
  );

  return { hasRecruitingAccess };
}
