import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import CompanyUserProfile from '@/modules/company/profile/pages/CompanyUserProfile.vue';
import { getCompanyUserDetail } from '@/modules/company/profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyUserModel } from '@/shared/company-profile/types';

vi.mock('@/modules/company/profile/api', () => ({ getCompanyUserDetail: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/company/profile/components/CompanyUserUpdate.vue', () => ({
  default: { name: 'CompanyUserUpdate', props: { user: Object }, emits: ['update:user', 'saved'], template: '<div class="user-update" />' },
}));

const user: CompanyUserModel = {
  id: 'user-1', companyId: 'company-1', createdAt: '2026-01-01T00:00:00', email: 'ann@acme.com',
  name: 'Ann', lastname: 'Lee', position: 'Manager', mobileNumber: '416 555-0000',
};

async function setup(current: CompanyUserModel = user) {
  vi.mocked(getCompanyUserDetail).mockResolvedValue(current);
  const wrapper = mountWithPlugins(CompanyUserProfile);
  await flushPromises();
  return wrapper;
}

function fields(wrapper: Awaited<ReturnType<typeof setup>>) {
  return wrapper.findAll('.detail-field').map((f) => [f.find('dt').text(), f.find('dd').text()]);
}

describe('CompanyUserProfile', () => {
  it('shows the signed-in user', async () => {
    const wrapper = await setup();

    expect(wrapper.find('.detail-header-title').text()).toBe('Ann Lee');
    expect(wrapper.find('.detail-header-meta').text()).toBe('Manager');
    expect(fields(wrapper)).toEqual([
      ['Name', 'Ann'],
      ['Last name', 'Lee'],
      ['Position', 'Manager'],
      ['Mobile number', '416 555-0000'],
      ['Email', 'ann@acme.com'],
    ]);
  });

  it('uses dashes for missing data', async () => {
    const wrapper = await setup({ ...user, lastname: '', position: '', mobileNumber: '' });

    expect(wrapper.find('.detail-header-title').text()).toBe('Ann');
    expect(wrapper.find('.detail-header-meta').exists()).toBe(false);
    expect(fields(wrapper).map(([, value]) => value)).toEqual(['Ann', '—', '—', '—', 'ann@acme.com']);
  });

  it('edits the user and shows the saved values', async () => {
    const wrapper = await setup();
    await wrapper.find('.detail-link').trigger('click');
    const form = wrapper.findComponent({ name: 'CompanyUserUpdate' });
    expect(form.props('user')).toEqual(user);

    form.vm.$emit('update:user', { ...user, name: 'Anna' });
    form.vm.$emit('saved');
    await flushPromises();

    expect(wrapper.find('.detail-header-title').text()).toBe('Anna Lee');
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });

  it('shows the error when the user cannot load', async () => {
    const error = new Error('load failed');
    vi.mocked(getCompanyUserDetail).mockRejectedValue(error);
    const wrapper = mountWithPlugins(CompanyUserProfile);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
    expect(wrapper.find('.detail-page').exists()).toBe(false);
  });
});
