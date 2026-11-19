import { describe, expect, it, vi } from 'vitest';
import EditVaccinationRequired from '@/modules/agency/recruiting/clients/components/EditVaccinationRequired.vue';
import { updateCompanyVaccinationRequired } from '@/modules/agency/recruiting/clients/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({ updateCompanyVaccinationRequired: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

interface Props {
  vaccinationRequired?: boolean | null;
  vaccinationComments?: string | null;
}

function setup(props: Props = {}) {
  return mountWithPlugins(EditVaccinationRequired, { props: { companyProfileId: 'company-1', ...props } });
}

function toggle(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('input[type="checkbox"]');
}

function comments(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('textarea[name="vaccinationComments"]');
}

describe('EditVaccinationRequired', () => {
  it('starts as not required with no comments', () => {
    const wrapper = setup();

    expect((toggle(wrapper).element as HTMLInputElement).checked).toBe(false);
    expect(wrapper.find('.switch').text()).toBe('No');
    expect((comments(wrapper).element as HTMLTextAreaElement).value).toBe('');
  });

  it('shows the current requirement and comments', () => {
    const wrapper = setup({ vaccinationRequired: true, vaccinationComments: 'Two doses' });

    expect((toggle(wrapper).element as HTMLInputElement).checked).toBe(true);
    expect(wrapper.find('.switch').text()).toBe('Yes');
    expect((comments(wrapper).element as HTMLTextAreaElement).value).toBe('Two doses');
  });

  it('saves the requirement with its comments and emits updated', async () => {
    vi.mocked(updateCompanyVaccinationRequired).mockResolvedValue();
    const wrapper = setup();
    await toggle(wrapper).setValue(true);
    await comments(wrapper).setValue('Booster required');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('updated')).toEqual([[{ required: true, comments: 'Booster required' }]]); });
    expect(updateCompanyVaccinationRequired).toHaveBeenCalledExactlyOnceWith('company-1', { required: true, comments: 'Booster required' });
  });

  it('sends cleared comments as an empty string', async () => {
    vi.mocked(updateCompanyVaccinationRequired).mockResolvedValue();
    const wrapper = setup({ vaccinationRequired: true, vaccinationComments: 'Old' });
    await comments(wrapper).setValue('');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('updated')).toEqual([[{ required: true, comments: '' }]]); });
  });

  it('rejects comments longer than 5000 characters', async () => {
    const wrapper = setup();
    await comments(wrapper).setValue('x'.repeat(5001));
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Max 5000 characters'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(updateCompanyVaccinationRequired).not.toHaveBeenCalled();
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(updateCompanyVaccinationRequired).mockRejectedValue(error);
    const wrapper = setup();
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updated')).toBeUndefined();
  });
});
