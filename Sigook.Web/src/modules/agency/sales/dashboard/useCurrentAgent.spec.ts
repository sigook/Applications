import { describe, expect, it, vi } from 'vitest';
import { useCurrentAgent } from '@/modules/agency/sales/dashboard/useCurrentAgent';
import { getAccount } from '@/shared/api/accountApi';

vi.mock('@/shared/api/accountApi', () => ({ getAccount: vi.fn() }));

describe('useCurrentAgent', () => {
  it('starts with an empty name', () => {
    expect(useCurrentAgent().agentName.value).toBe('');
  });

  it('uses the account name when present', async () => {
    vi.mocked(getAccount).mockResolvedValue({ name: 'Jane Doe', email: 'jane@example.com' });
    const { agentName, loadAgentName } = useCurrentAgent();
    await loadAgentName();
    expect(agentName.value).toBe('Jane Doe');
  });

  it('falls back to the email when the name is empty', async () => {
    vi.mocked(getAccount).mockResolvedValue({ name: '', email: 'jane@example.com' });
    const { agentName, loadAgentName } = useCurrentAgent();
    await loadAgentName();
    expect(agentName.value).toBe('jane@example.com');
  });

  it('keeps the current name when the request fails', async () => {
    vi.mocked(getAccount)
      .mockResolvedValueOnce({ name: 'Jane Doe', email: 'jane@example.com' })
      .mockRejectedValueOnce(new Error('boom'));
    const { agentName, loadAgentName } = useCurrentAgent();
    await loadAgentName();
    await expect(loadAgentName()).resolves.toBeUndefined();
    expect(agentName.value).toBe('Jane Doe');
  });
});
