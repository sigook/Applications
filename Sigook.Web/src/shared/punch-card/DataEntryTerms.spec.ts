import { beforeEach, describe, expect, it, vi } from 'vitest';
import { reactive } from 'vue';
import DataEntryTerms from '@/shared/punch-card/DataEntryTerms.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));

function headings() {
  return mountWithPlugins(DataEntryTerms).findAll('h1').map((h) => h.text());
}

beforeEach(() => {
  securityStore.userRoles = [];
});

describe('DataEntryTerms', () => {
  it('starts collapsed behind its trigger', () => {
    const wrapper = mountWithPlugins(DataEntryTerms);

    expect(wrapper.find('u').text()).toBe('Data Entry Terms');
    expect(wrapper.find('.collapse-content').isVisible()).toBe(false);
  });

  it.each([
    [['company'], ['Business Owner Data Entry', 'Data Entry Terms']],
    [['company.user'], ['Business Owner Data Entry', 'Data Entry Terms']],
    [['worker'], ['Worker Data Entry', 'Data Entry Terms']],
    [['admin'], ['Data Entry Terms']],
    [['company', 'worker'], ['Business Owner Data Entry', 'Data Entry Terms']],
  ])('shows the terms for %j', (roles, expected) => {
    securityStore.userRoles = roles;
    expect(headings()).toEqual(expected);
  });
});
