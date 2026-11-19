import { describe, expect, it, vi } from 'vitest';
import ContactInformationForm from '@/modules/agency/recruiting/clients/components/ContactInformationForm.vue';
import { updateAgencyCompanyContactInformation } from '@/modules/agency/recruiting/clients/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany } from '@/testing/agencyCompanyFixtures';
import type { CompanyProfileDetail } from '@/shared/company-profile/types';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({ updateAgencyCompanyContactInformation: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

function setup(model: CompanyProfileDetail = createAgencyCompany({ phoneExt: 5, faxExt: null })) {
  return mountWithPlugins(ContactInformationForm, { props: { model } });
}

function input(wrapper: ReturnType<typeof setup>, name: string) {
  return wrapper.find(`input[name="${name}"]`);
}

function value(wrapper: ReturnType<typeof setup>, name: string) {
  return (input(wrapper, name).element as HTMLInputElement).value;
}

function errors(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

describe('ContactInformationForm', () => {
  it('fills the form from the company', () => {
    const wrapper = setup();

    expect(value(wrapper, 'phone')).toBe('416 555-0100');
    expect(value(wrapper, 'phoneExt')).toBe('5');
    expect(value(wrapper, 'fax')).toBe('');
    expect(value(wrapper, 'faxExt')).toBe('');
    expect(value(wrapper, 'website')).toBe('www.acme.com');
  });

  it('refreshes the form when the company changes', async () => {
    const wrapper = setup();
    await wrapper.setProps({ model: createAgencyCompany({ phoneExt: null, faxExt: 7, website: 'acme.ca' }) });

    expect(value(wrapper, 'phoneExt')).toBe('');
    expect(value(wrapper, 'faxExt')).toBe('7');
    expect(value(wrapper, 'website')).toBe('acme.ca');
  });

  it('requires the phone and a valid website', async () => {
    const wrapper = setup(createAgencyCompany({ phone: null, website: '' }));
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(errors(wrapper)).toHaveLength(2); });
    expect(errors(wrapper)).toContain('Website is required');
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(updateAgencyCompanyContactInformation).not.toHaveBeenCalled();
  });

  it('rejects non numeric extensions and an invalid URL', async () => {
    const wrapper = setup();
    await input(wrapper, 'phoneExt').setValue('ab');
    await input(wrapper, 'faxExt').setValue('123456789');
    await input(wrapper, 'website').setValue('not a url');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Must be 1-8 digits', 'Must be 1-8 digits', 'Invalid URL']); });
    expect(updateAgencyCompanyContactInformation).not.toHaveBeenCalled();
  });

  it('blocks saving when the fax is invalid', async () => {
    const wrapper = setup();
    await input(wrapper, 'fax').setValue('123');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(updateAgencyCompanyContactInformation).not.toHaveBeenCalled();
  });

  it('saves the contact information and emits the updated model', async () => {
    vi.mocked(updateAgencyCompanyContactInformation).mockResolvedValue();
    const wrapper = setup();
    await input(wrapper, 'phone').setValue('6475550123');
    await input(wrapper, 'faxExt').setValue('99');
    await input(wrapper, 'website').setValue('https://acme.com/jobs');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('save')).toHaveLength(1); });
    const expected = { phone: '647 555-0123', phoneExt: 5, faxExt: 99, website: 'https://acme.com/jobs' };
    expect(wrapper.emitted('update:model')?.[0][0]).toMatchObject(expected);
    expect(updateAgencyCompanyContactInformation).toHaveBeenCalledExactlyOnceWith('company-profile-1', expect.objectContaining(expected));
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
  });

  it('shows the error and stays open when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(updateAgencyCompanyContactInformation).mockRejectedValue(error);
    const wrapper = setup();
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('save')).toBeUndefined();
  });
});
