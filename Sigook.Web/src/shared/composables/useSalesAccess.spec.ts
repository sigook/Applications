import { beforeEach, describe, expect, it, vi } from 'vitest';
import { reactive } from 'vue';
import { useSalesAccess } from '@/shared/composables/useSalesAccess';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));

describe('useSalesAccess', () => {
  beforeEach(() => {
    securityStore.userRoles = [];
  });

  it.each([
    [['superadmin'], true],
    [['admin'], true],
    [['sales'], true],
    [['recruiting'], false],
    [['company'], false],
    [['worker'], false],
    [['recruiting', 'sales'], true],
    [[], false],
  ])('%j -> %s', (userRoles, expected) => {
    securityStore.userRoles = userRoles;
    expect(useSalesAccess().hasSalesAccess.value).toBe(expected);
  });

  it('updates when the roles change', () => {
    const { hasSalesAccess } = useSalesAccess();
    expect(hasSalesAccess.value).toBe(false);

    securityStore.userRoles = ['superadmin'];
    expect(hasSalesAccess.value).toBe(true);

    securityStore.userRoles = [];
    expect(hasSalesAccess.value).toBe(false);
  });
});
