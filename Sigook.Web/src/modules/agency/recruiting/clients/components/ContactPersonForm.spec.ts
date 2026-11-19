import { describe, expect, it, vi } from 'vitest';
import ContactPersonForm from '@/modules/agency/recruiting/clients/components/ContactPersonForm.vue';
import { createAgencyCompanyContactPerson, updateAgencyCompanyContactPerson } from '@/modules/agency/recruiting/clients/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyCompanyContactPerson } from '@/modules/agency/recruiting/clients/types';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  createAgencyCompanyContactPerson: vi.fn(),
  updateAgencyCompanyContactPerson: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const existing: AgencyCompanyContactPerson = {
  id: 'contact-1',
  companyProfileId: 'company-1',
  title: 'Ms',
  firstName: 'Jane',
  middleName: 'Ann',
  lastName: 'Doe',
  position: 'HR Manager',
  mobileNumber: '416 555-0101',
  officeNumber: '416 555-0102',
  officeNumberExt: 12,
  email: 'jane@acme.com',
};

function setup(currentContact: AgencyCompanyContactPerson | null = null) {
  return mountWithPlugins(ContactPersonForm, { props: { profileId: 'company-1', currentContact } });
}

function value(wrapper: ReturnType<typeof setup>, selector: string) {
  return (wrapper.find(selector).element as HTMLInputElement).value;
}

function errors(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

async function fillRequired(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('select[name="title"]').setValue('Mr');
  await wrapper.find('input[name="name"]').setValue('John');
  await wrapper.find('input[name="lastname"]').setValue('Smith');
  await wrapper.find('input[name="position"]').setValue('Supervisor');
}

describe('ContactPersonForm', () => {
  it('offers the title options and a Create button for a new contact', () => {
    const wrapper = setup();

    expect(wrapper.findAll('select[name="title"] option').map((o) => o.text())).toEqual(['Mr', 'Mrs', 'Ms', 'Miss', 'Mx', 'Master', 'Madam']);
    expect(wrapper.find('button.is-primary').text()).toBe('Create');
  });

  it('requires title, name, last name and position', async () => {
    const wrapper = setup();
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => {
      expect(errors(wrapper)).toEqual(['Title is required', 'Name is required', 'Last name is required', 'Position is required']);
    });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createAgencyCompanyContactPerson).not.toHaveBeenCalled();
  });

  it('validates the extension and email format', async () => {
    const wrapper = setup();
    await fillRequired(wrapper);
    await wrapper.find('input[name="officeNumberExt"]').setValue('12a');
    await wrapper.find('input[name="email"]').setValue('bad');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Must be 1-8 digits', 'Invalid email']); });
    expect(createAgencyCompanyContactPerson).not.toHaveBeenCalled();
  });

  it('creates the contact and emits it with the new id', async () => {
    vi.mocked(createAgencyCompanyContactPerson).mockResolvedValue({ id: 'contact-9' });
    const wrapper = setup();
    await fillRequired(wrapper);
    await wrapper.find('input[name="officeNumberExt"]').setValue('45');
    await wrapper.find('input[name="officeNumber"]').setValue('4165550199');
    await wrapper.find('button.is-primary').trigger('click');

    const payload = {
      mobileNumber: null,
      officeNumber: '416 555-0199',
      title: 'Mr',
      firstName: 'John',
      middleName: '',
      lastName: 'Smith',
      position: 'Supervisor',
      officeNumberExt: 45,
      email: null,
    };
    await vi.waitFor(() => { expect(wrapper.emitted('updateContent')).toEqual([[{ ...payload, id: 'contact-9' }]]); });
    expect(createAgencyCompanyContactPerson).toHaveBeenCalledExactlyOnceWith('company-1', payload);
    expect(showAlertSuccess).toHaveBeenCalledWith('Created');
  });

  it('loads the contact to edit and updates it', async () => {
    vi.mocked(updateAgencyCompanyContactPerson).mockResolvedValue();
    const wrapper = setup(existing);

    expect(value(wrapper, 'select[name="title"]')).toBe('Ms');
    expect(value(wrapper, 'input[name="name"]')).toBe('Jane');
    expect(value(wrapper, 'input[name="middlename"]')).toBe('Ann');
    expect(value(wrapper, 'input[name="officeNumberExt"]')).toBe('12');
    expect(value(wrapper, 'input[name="email"]')).toBe('jane@acme.com');
    expect(wrapper.find('button.is-primary').text()).toBe('Save');

    await wrapper.find('input[name="position"]').setValue('Director');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('updateContent')).toEqual([[{ ...existing, position: 'Director' }]]); });
    expect(updateAgencyCompanyContactPerson).toHaveBeenCalledExactlyOnceWith('company-1', 'contact-1', { ...existing, position: 'Director' });
    expect(createAgencyCompanyContactPerson).not.toHaveBeenCalled();
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
  });

  it('shows the error when creating fails', async () => {
    const error = new Error('create failed');
    vi.mocked(createAgencyCompanyContactPerson).mockRejectedValue(error);
    const wrapper = setup();
    await fillRequired(wrapper);
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updateContent')).toBeUndefined();
  });

  it('shows the error when updating fails', async () => {
    const error = new Error('update failed');
    vi.mocked(updateAgencyCompanyContactPerson).mockRejectedValue(error);
    const wrapper = setup(existing);
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updateContent')).toBeUndefined();
  });

  it('emits cancel', async () => {
    const wrapper = setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Cancel')?.trigger('click');

    expect(wrapper.emitted('cancel')).toHaveLength(1);
  });
});
