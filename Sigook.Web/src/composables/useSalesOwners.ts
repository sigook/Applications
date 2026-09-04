import { ref, ComputedRef, Ref } from 'vue';
import { getAgencyPersonnel } from '@/api/agencyApi';
import { useAdmin } from '@/composables/useAdmin';
import { salesAccess } from '@/security/roles';
import type { AgencyPersonnelListItem } from '@/types/agency';

export function useSalesOwners(): {
  isAdmin: ComputedRef<boolean>;
  owners: Ref<AgencyPersonnelListItem[]>;
  loadOwners: () => void;
} {
  const { isAdmin } = useAdmin();
  const owners = ref<AgencyPersonnelListItem[]>([]);

  function loadOwners(): void {
    if (!isAdmin.value) return;
    getAgencyPersonnel()
      .then((items) => {
        owners.value = items.filter((p) => !p.role || salesAccess.includes(p.role));
      })
      .catch(() => {
        owners.value = [];
      });
  }

  return { isAdmin, owners, loadOwners };
}
