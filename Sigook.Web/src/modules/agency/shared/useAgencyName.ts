import { computed } from 'vue';
import { useAgencyStore } from '@/modules/agency/store';

export const AGENCY_COLOR_COUNT = 6;

export function agencyCode(name: string): string {
  const words = name.trim().split(/\s+/).filter(Boolean);
  if (words.length === 0) return '';
  if (words.length === 1) return words[0].slice(0, 3).toUpperCase();
  return words.slice(0, 3).map((w) => w[0]).join('').toUpperCase();
}

export function useAgencyName() {
  const agencyStore = useAgencyStore();
  const isMultiAgency = computed(() => agencyStore.agencies.length > 1);

  function agencyName(agencyId?: string | null): string | null {
    if (!isMultiAgency.value || !agencyId) return null;
    return agencyStore.agencies.find((a) => a.id === agencyId)?.name ?? null;
  }

  function agencyColor(agencyId?: string | null): number | null {
    if (!isMultiAgency.value || !agencyId) return null;
    const index = agencyStore.agencies.findIndex((a) => a.id === agencyId);
    return index < 0 ? null : index % AGENCY_COLOR_COUNT;
  }

  return { isMultiAgency, agencyName, agencyColor };
}
