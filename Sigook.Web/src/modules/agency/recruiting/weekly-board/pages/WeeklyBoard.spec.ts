import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import WeeklyBoard from '@/modules/agency/recruiting/weekly-board/pages/WeeklyBoard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

const { securityStore } = vi.hoisted(() => ({ securityStore: { userRoles: [] as string[] } }));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/modules/agency/recruiting/weekly-board/components/AdminWeeklyBoard.vue', () => ({
  __esModule: true,
  default: { name: 'AdminWeeklyBoard', template: '<div class="admin-board" />' },
}));
vi.mock('@/modules/agency/recruiting/weekly-board/components/RecruiterWeeklyBoard.vue', () => ({
  __esModule: true,
  default: { name: 'RecruiterWeeklyBoard', template: '<div class="recruiter-board" />' },
}));

async function setup(roles: string[]) {
  securityStore.userRoles = roles;
  const wrapper = mountWithPlugins(WeeklyBoard);
  await flushPromises();
  return wrapper;
}

beforeEach(() => {
  securityStore.userRoles = [];
});

describe('WeeklyBoard', () => {
  it.each([['admin'], ['superadmin']])('shows the whole board to %s', async (role) => {
    const wrapper = await setup([role]);

    expect(wrapper.find('.admin-board').exists()).toBe(true);
    expect(wrapper.find('.recruiter-board').exists()).toBe(false);
  });

  it.each([['recruiting'], ['sales']])('shows only their own board to %s', async (role) => {
    const wrapper = await setup([role]);

    expect(wrapper.find('.recruiter-board').exists()).toBe(true);
    expect(wrapper.find('.admin-board').exists()).toBe(false);
  });
});
