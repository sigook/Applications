import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import CompanySettingsCard from '@/modules/agency/recruiting/clients/components/CompanySettingsCard.vue';
import { updateOvertime, updatePaidHolidays, updatePaymentTerms, updatePermissionToSeeRequests } from '@/modules/agency/recruiting/clients/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany } from '@/testing/agencyCompanyFixtures';
import type { AgencyCompanyProfileDetail } from '@/modules/agency/recruiting/clients/types';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  updateOvertime: vi.fn(),
  updatePaidHolidays: vi.fn(),
  updatePaymentTerms: vi.fn(),
  updatePermissionToSeeRequests: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const company = createAgencyCompany({ requiresPermissionToSeeRequests: true, paidHolidays: false, overtimeStartsAfter: 44, paymentTermDays: 30 });

function setup(current: AgencyCompanyProfileDetail = company) {
  return mountWithPlugins(CompanySettingsCard, { props: { company: current } });
}

function checkboxes(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll<HTMLInputElement>('input[type="checkbox"]');
}

function numberInputs(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll<HTMLInputElement>('input[type="number"]');
}

function saveButtons(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('button').filter((b) => b.text() === 'Save');
}

function emitted(wrapper: ReturnType<typeof setup>) {
  return (wrapper.emitted('update:company') ?? []).map(([value]) => value as AgencyCompanyProfileDetail);
}

describe('CompanySettingsCard', () => {
  it('shows the current settings', () => {
    const wrapper = setup();

    expect(checkboxes(wrapper).map((c) => c.element.checked)).toEqual([true, false]);
    expect(numberInputs(wrapper).map((i) => i.element.value)).toEqual(['44', '30']);
  });

  it('follows the company when it changes', async () => {
    const wrapper = setup();
    await wrapper.setProps({ company: { ...company, overtimeStartsAfter: 40, paymentTermDays: 15 } });

    expect(numberInputs(wrapper).map((i) => i.element.value)).toEqual(['40', '15']);
  });

  it.each([
    [0, 'requiresPermissionToSeeRequests', false, updatePermissionToSeeRequests],
    [1, 'paidHolidays', true, updatePaidHolidays],
  ] as const)('saves checkbox %i as %s', async (index, key, value, action) => {
    vi.mocked(action).mockResolvedValue();
    const wrapper = setup();
    await checkboxes(wrapper)[index].setValue(value);
    await flushPromises();

    const updated = { ...company, [key]: value };
    expect(emitted(wrapper)).toEqual([updated]);
    expect(action).toHaveBeenCalledExactlyOnceWith('company-profile-1', updated);
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
  });

  it('saves the overtime threshold', async () => {
    vi.mocked(updateOvertime).mockResolvedValue();
    const wrapper = setup();
    await numberInputs(wrapper)[0].setValue('40.5');
    await saveButtons(wrapper)[0].trigger('click');
    await flushPromises();

    expect(updateOvertime).toHaveBeenCalledExactlyOnceWith('company-profile-1', { ...company, overtimeStartsAfter: 40.5 });
    expect(emitted(wrapper)).toEqual([{ ...company, overtimeStartsAfter: 40.5 }]);
  });

  it('restores the previous company when a setting fails', async () => {
    const error = new Error('denied');
    vi.mocked(updatePaidHolidays).mockRejectedValue(error);
    const wrapper = setup();
    await checkboxes(wrapper)[1].setValue(true);
    await flushPromises();

    expect(emitted(wrapper)).toEqual([{ ...company, paidHolidays: true }, company]);
    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(showAlertSuccess).not.toHaveBeenCalled();
  });

  it('saves the payment terms', async () => {
    vi.mocked(updatePaymentTerms).mockResolvedValue();
    const wrapper = setup();
    await numberInputs(wrapper)[1].setValue('45');
    await saveButtons(wrapper)[1].trigger('click');
    await flushPromises();

    expect(updatePaymentTerms).toHaveBeenCalledExactlyOnceWith('company-profile-1', { paymentTermDays: 45 });
    expect(emitted(wrapper)).toEqual([{ ...company, paymentTermDays: 45 }]);
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
  });

  it('restores the payment terms when saving fails', async () => {
    const error = new Error('out of range');
    vi.mocked(updatePaymentTerms).mockRejectedValue(error);
    const wrapper = setup();
    await numberInputs(wrapper)[1].setValue('400');
    await saveButtons(wrapper)[1].trigger('click');
    await flushPromises();

    expect(emitted(wrapper)).toEqual([{ ...company, paymentTermDays: 400 }, company]);
    expect(numberInputs(wrapper)[1].element.value).toBe('30');
    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
