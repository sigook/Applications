import { beforeEach, describe, expect, it, vi } from 'vitest';
import { reactive } from 'vue';
import { useSuperAdmin } from '@/shared/composables/useSuperAdmin';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));

describe('useSuperAdmin', () => {
  beforeEach(() => {
    securityStore.userRoles = [];
  });

  it.each([
    [['superadmin'], true],
    [['admin'], false],
    [['recruiting'], false],
    [['sales'], false],
    [['worker'], false],
    [['admin', 'superadmin'], true],
    [[], false],
    [['SuperAdmin'], false],
  ])('%j -> %s', (userRoles, expected) => {
    securityStore.userRoles = userRoles;
    expect(useSuperAdmin().isSuperAdmin.value).toBe(expected);
  });

  it('updates when the roles change', () => {
    const { isSuperAdmin } = useSuperAdmin();
    expect(isSuperAdmin.value).toBe(false);

    securityStore.userRoles = ['superadmin'];
    expect(isSuperAdmin.value).toBe(true);

    securityStore.userRoles = [];
    expect(isSuperAdmin.value).toBe(false);
  });
});
