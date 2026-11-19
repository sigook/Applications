import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import RolesShiftDetail from '@/modules/agency/recruiting/clients/components/RolesShiftDetail.vue';
import { getAgencyCompanyJobPositionById } from '@/modules/agency/recruiting/clients/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({ getAgencyCompanyJobPositionById: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

function setup(displayShift?: string) {
  return mountWithPlugins(RolesShiftDetail, { props: { displayShift, roleId: 'role-1', companyProfileId: 'company-1' } });
}

function panelText() {
  return document.body.querySelector('.table-shift-detail')?.textContent.replace(/\s+/g, ' ').trim() ?? null;
}

describe('RolesShiftDetail', () => {
  it('has no toggle without a shift summary', () => {
    const wrapper = setup();

    expect(wrapper.find('button').exists()).toBe(false);
  });

  it('loads and shows the role shift when expanded, then collapses', async () => {
    vi.mocked(getAgencyCompanyJobPositionById).mockResolvedValue({
      id: 'role-1',
      jobPosition: 'Packer',
      shift: { monday: true, mondayStart: '08:00:00', mondayFinish: '16:00:00' },
    });
    const wrapper = setup('Mon 8:00-16:00');
    expect(wrapper.text()).toContain('Mon 8:00-16:00');

    await wrapper.find('button').trigger('click');
    await flushPromises();

    expect(getAgencyCompanyJobPositionById).toHaveBeenCalledExactlyOnceWith('company-1', 'role-1');
    expect(panelText()).toContain('Mon');
    expect(wrapper.find('.icon i').classes()).toContain('mdi-chevron-up');

    await wrapper.find('button').trigger('click');

    expect(panelText()).toBeNull();
    expect(getAgencyCompanyJobPositionById).toHaveBeenCalledTimes(1);
  });

  it('closes when clicking outside', async () => {
    vi.mocked(getAgencyCompanyJobPositionById).mockResolvedValue({ id: 'role-1', jobPosition: 'Packer' });
    const wrapper = setup('Mon 8:00-16:00');
    await wrapper.find('button').trigger('click');
    await flushPromises();
    expect(panelText()).not.toBeNull();

    const outside = document.createElement('div');
    document.body.appendChild(outside);
    outside.click();
    await flushPromises();

    expect(panelText()).toBeNull();
  });

  it('shows the error when the shift cannot be loaded', async () => {
    const error = new Error('load failed');
    vi.mocked(getAgencyCompanyJobPositionById).mockRejectedValue(error);
    const wrapper = setup('Mon 8:00-16:00');
    await wrapper.find('button').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
