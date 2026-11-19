import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ProfileContact from '@/modules/company/profile/components/ProfileContact.vue';
import { deleteContactPerson, getContactPeople, saveContactPerson, updateContactPerson } from '@/modules/company/profile/api';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyContactPersonModel } from '@/modules/company/profile/types';

vi.mock('@/modules/company/profile/api', () => ({
  getContactPeople: vi.fn(),
  saveContactPerson: vi.fn(),
  updateContactPerson: vi.fn(),
  deleteContactPerson: vi.fn(),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const ann: CompanyContactPersonModel = {
  id: 'contact-1', title: 'Mrs', firstName: 'Ann', middleName: 'Marie', lastName: 'Lee', position: 'Manager',
  mobileNumber: '416 555-0000', officeNumber: '416 555-0100', officeNumberExt: 22, email: 'ann@acme.com',
};
const bob: CompanyContactPersonModel = {
  id: 'contact-2', title: 'Mr', firstName: 'Bob', middleName: null, lastName: 'Ray', position: 'Supervisor',
  mobileNumber: '647 555-1111', officeNumber: '', officeNumberExt: null, email: 'bob@acme.com',
};

async function setup(contacts: CompanyContactPersonModel[] = [ann, bob]) {
  vi.mocked(getContactPeople).mockResolvedValue(contacts);
  const wrapper = mountWithPlugins(ProfileContact);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function field(wrapper: Wrapper, name: string) {
  return wrapper.find(`.modal [name="${name}"]`);
}

function value(wrapper: Wrapper, name: string) {
  return (field(wrapper, name).element as HTMLInputElement).value;
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('.modal button.is-primary').trigger('click');
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.modal .help.is-danger').map((e) => e.text());
}

async function fillNew(wrapper: Wrapper) {
  await field(wrapper, 'title').setValue('Ms');
  await field(wrapper, 'first name').setValue('Cara');
  await field(wrapper, 'last name').setValue('Diaz');
  await field(wrapper, 'email').setValue('cara@acme.com');
  await field(wrapper, 'position').setValue('Buyer');
  await field(wrapper, 'mobileNumber').setValue('9055552222');
  await field(wrapper, 'officeNumberExt').setValue('7');
}

describe('ProfileContact', () => {
  it('lists the contacts and reports how many there are', async () => {
    const wrapper = await setup();

    const cells = wrapper.findAll('tbody tr').map((row) => row.findAll('td').slice(0, 4).map((cell) => cell.text().replace(/\s+/g, ' ')));
    expect(cells[0]).toEqual(['Ann Marie Lee', 'Manager', '416 555-0000416 555-0100 Ext. 22', 'ann@acme.com']);
    expect(cells[1][0]).toBe('Bob Ray');
    expect(wrapper.emitted('loaded')).toEqual([[2]]);
  });

  it('creates a contact', async () => {
    vi.mocked(saveContactPerson).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-actions button').trigger('click');
    expect(wrapper.find('.modal h2').text()).toBe('New contact');
    await fillNew(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(getContactPeople).toHaveBeenCalledTimes(2); });
    expect(saveContactPerson).toHaveBeenCalledExactlyOnceWith(expect.objectContaining({
      title: 'Ms', firstName: 'Cara', lastName: 'Diaz', email: 'cara@acme.com', position: 'Buyer',
      mobileNumber: '905 555-2222', officeNumberExt: 7,
    }));
    expect(updateContactPerson).not.toHaveBeenCalled();
    expect(showAlertSuccess).toHaveBeenCalledWith('Profile updated');
    expect(getContactPeople).toHaveBeenCalledTimes(2);
  });

  it('validates the required fields', async () => {
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-actions button').trigger('click');
    await field(wrapper, 'email').setValue('not-an-email');
    await field(wrapper, 'officeNumberExt').setValue('abc');
    await submit(wrapper);

    await vi.waitFor(() => {
      expect(errors(wrapper)).toEqual(expect.arrayContaining([
        'Title is required', 'First name is required', 'Last name is required', 'Invalid email', 'Position is required', 'Must be 1-8 digits',
      ]));
    });
    expect(saveContactPerson).not.toHaveBeenCalled();
  });

  it('requires the mobile number', async () => {
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-actions button').trigger('click');
    await fillNew(wrapper);
    await field(wrapper, 'mobileNumber').setValue('');
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Phone is required']); });
    expect(saveContactPerson).not.toHaveBeenCalled();
  });

  it('edits a contact with its values', async () => {
    vi.mocked(updateContactPerson).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Edit contact"]').trigger('click');

    expect(wrapper.find('.modal h2').text()).toBe('Edit contact');
    expect(value(wrapper, 'first name')).toBe('Ann');
    expect(value(wrapper, 'middle name')).toBe('Marie');
    expect(value(wrapper, 'officeNumberExt')).toBe('22');
    await field(wrapper, 'position').setValue('Director');
    await submit(wrapper);

    await vi.waitFor(() => { expect(updateContactPerson).toHaveBeenCalled(); });
    expect(updateContactPerson).toHaveBeenCalledExactlyOnceWith('contact-1', expect.objectContaining({
      title: 'Mrs', firstName: 'Ann', lastName: 'Lee', position: 'Director', officeNumberExt: 22,
    }));
    expect(saveContactPerson).not.toHaveBeenCalled();
  });

  it('shows the error when saving fails', async () => {
    vi.mocked(updateContactPerson).mockRejectedValue({ data: 'save failed' });
    const wrapper = await setup();
    await wrapper.findAll('button[aria-label="Edit contact"]')[1].trigger('click');
    expect(value(wrapper, 'officeNumberExt')).toBe('');
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledExactlyOnceWith('save failed'); });
    expect(showAlertSuccess).not.toHaveBeenCalled();
  });

  it('deletes a contact once confirmed and reloads', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteContactPerson).mockResolvedValue();
    const wrapper = await setup();
    vi.mocked(getContactPeople).mockResolvedValue([ann]);
    await wrapper.findAll('button[aria-label="Delete contact"]')[1].trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this contact', 'Yes');
    expect(deleteContactPerson).toHaveBeenCalledExactlyOnceWith('contact-2');
    expect(wrapper.emitted('loaded')).toEqual([[2], [1]]);
  });

  it('keeps the contact when the deletion is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Delete contact"]').trigger('click');
    await flushPromises();

    expect(deleteContactPerson).not.toHaveBeenCalled();
    expect(getContactPeople).toHaveBeenCalledTimes(1);
  });

  it('shows the error when the deletion fails', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteContactPerson).mockRejectedValue({ data: 'delete failed' });
    const wrapper = await setup();
    await wrapper.find('button[aria-label="Delete contact"]').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith('delete failed');
    expect(getContactPeople).toHaveBeenCalledTimes(1);
  });
});
