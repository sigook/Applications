import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, ref } from 'vue';
import { useSalesOwners } from '@/modules/agency/shared/useSalesOwners';
import { getAgencyPersonnel } from '@/modules/agency/profile/api';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';

const adminFlag = ref(true);

vi.mock('@/shared/composables/useAdmin', () => ({
  useAdmin: () => ({ isAdmin: computed(() => adminFlag.value) }),
}));

vi.mock('@/modules/agency/profile/api', () => ({ getAgencyPersonnel: vi.fn() }));

function person(id: string, role: string): AgencyPersonnelListItem {
  return { id, userId: `user-${id}`, name: `Person ${id}`, email: null, role, agencyIds: [], agencies: [] };
}

describe('useSalesOwners', () => {
  beforeEach(() => {
    adminFlag.value = true;
    vi.mocked(getAgencyPersonnel).mockReset();
  });

  it('exposes the admin flag', () => {
    adminFlag.value = false;
    expect(useSalesOwners().isAdmin.value).toBe(false);
  });

  it('keeps only personnel with sales access or without a role', async () => {
    vi.mocked(getAgencyPersonnel).mockResolvedValue([
      person('1', 'superadmin'),
      person('2', 'admin'),
      person('3', 'sales'),
      person('4', 'recruiting'),
      person('5', ''),
      person('6', 'worker'),
    ]);
    const { owners, loadOwners } = useSalesOwners();
    await loadOwners();
    expect(getAgencyPersonnel).toHaveBeenCalledWith();
    expect(owners.value.map((p) => p.id)).toEqual(['1', '2', '3', '5']);
  });

  it('skips loading for non admins', async () => {
    adminFlag.value = false;
    const { owners, loadOwners } = useSalesOwners();
    await loadOwners();
    expect(getAgencyPersonnel).not.toHaveBeenCalled();
    expect(owners.value).toEqual([]);
  });

  it('resets owners when the request fails', async () => {
    vi.mocked(getAgencyPersonnel)
      .mockResolvedValueOnce([person('1', 'sales')])
      .mockRejectedValueOnce(new Error('boom'));
    const { owners, loadOwners } = useSalesOwners();
    await loadOwners();
    expect(owners.value).toHaveLength(1);
    await loadOwners();
    expect(owners.value).toEqual([]);
  });
});
