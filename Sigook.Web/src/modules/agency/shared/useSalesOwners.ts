import { ref, ComputedRef, Ref } from 'vue';
import { getAgencyPersonnel } from '@/modules/agency/profile/api';
import { useAdmin } from '@/shared/composables/useAdmin';
import { salesAccess } from '@/app/security/roles';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';

export function useSalesOwners(): {
  isAdmin: ComputedRef<boolean>;
  owners: Ref<AgencyPersonnelListItem[]>;
  loadOwners: () => Promise<void>;
} {
  const { isAdmin } = useAdmin();
  const owners = ref<AgencyPersonnelListItem[]>([]);

  async function loadOwners(): Promise<void> {
    if (!isAdmin.value) return;
    try {
      const items = await getAgencyPersonnel();
      owners.value = items.filter((p) => !p.role || salesAccess.includes(p.role));
    } catch {
      owners.value = [];
    }
  }

  return { isAdmin, owners, loadOwners };
}
