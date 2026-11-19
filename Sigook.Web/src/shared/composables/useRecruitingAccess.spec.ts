import { beforeEach, describe, expect, it, vi } from 'vitest';
import { reactive } from 'vue';
import { useRecruitingAccess } from '@/shared/composables/useRecruitingAccess';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));

describe('useRecruitingAccess', () => {
  beforeEach(() => {
    securityStore.userRoles = [];
  });

  it.each([
    [['superadmin'], true],
    [['admin'], true],
    [['recruiting'], true],
    [['sales'], false],
    [['company'], false],
    [['company.user'], false],
    [['worker'], false],
    [['worker', 'recruiting'], true],
    [[], false],
  ])('%j -> %s', (userRoles, expected) => {
    securityStore.userRoles = userRoles;
    expect(useRecruitingAccess().hasRecruitingAccess.value).toBe(expected);
  });

  it('updates when the roles change', () => {
    const { hasRecruitingAccess } = useRecruitingAccess();
    expect(hasRecruitingAccess.value).toBe(false);

    securityStore.userRoles = ['superadmin'];
    expect(hasRecruitingAccess.value).toBe(true);

    securityStore.userRoles = [];
    expect(hasRecruitingAccess.value).toBe(false);
  });
});
