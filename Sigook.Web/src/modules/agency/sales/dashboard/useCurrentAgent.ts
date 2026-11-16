import { ref, type Ref } from 'vue';
import { getAccount } from '@/shared/api/accountApi';

export function useCurrentAgent(): { agentName: Ref<string>; loadAgentName: () => Promise<void> } {
  const agentName = ref('');

  async function loadAgentName(): Promise<void> {
    try {
      const account = await getAccount();
      agentName.value = account.name || account.email;
    } catch {
      return;
    }
  }

  return { agentName, loadAgentName };
}
