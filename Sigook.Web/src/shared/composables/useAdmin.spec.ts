import { beforeEach, describe, expect, it, vi } from 'vitest';
import { reactive } from 'vue';
import { useAdmin } from '@/shared/composables/useAdmin';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));

describe('useAdmin', () => {
  beforeEach(() => {
    securityStore.userRoles = [];
  });

  it.each([
    [['superadmin'], true],
    [['admin'], true],
    [['recruiting'], false],
    [['sales'], false],
    [['company'], false],
    [['worker'], false],
    [['sales', 'admin'], true],
    [[], false],
    [['Admin'], false],
  ])('%j -> %s', (userRoles, expected) => {
    securityStore.userRoles = userRoles;
    expect(useAdmin().isAdmin.value).toBe(expected);
  });

  it('updates when the roles change', () => {
    const { isAdmin } = useAdmin();
    expect(isAdmin.value).toBe(false);

    securityStore.userRoles = ['superadmin'];
    expect(isAdmin.value).toBe(true);

    securityStore.userRoles = [];
    expect(isAdmin.value).toBe(false);
  });
});
