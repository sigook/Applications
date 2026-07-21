import { ref, type Ref } from 'vue';
import { getAgencyPersonnel } from '@/api/agencyApi';
import { useSecurityStore } from '@/stores/security';

export function useCurrentAgent(): { agentName: Ref<string>; loadAgentName: () => Promise<void> } {
  const securityStore = useSecurityStore();
  const agentName = ref('');

  async function loadAgentName(): Promise<void> {
    const user = await securityStore.getUser();
    const claims = user?.profile;
    if (!claims) return;

    const identifier = (claims.email || claims.name || '').toLowerCase();
    agentName.value = claims.name && !claims.name.includes('@') ? claims.name : '';
    if (!identifier) return;

    try {
      const personnel = await getAgencyPersonnel();
      const match = personnel.find((item) => item.email?.toLowerCase() === identifier);
      if (match?.name) agentName.value = match.name;
    } catch {
      return;
    }
  }

  return { agentName, loadAgentName };
}
