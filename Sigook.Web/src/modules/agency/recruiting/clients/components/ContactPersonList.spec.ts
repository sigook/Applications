import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ContactPersonList from '@/modules/agency/recruiting/clients/components/ContactPersonList.vue';
import { deleteAgencyCompanyContactPerson, getAgencyCompanyContactPerson } from '@/modules/agency/recruiting/clients/api';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyCompanyContactPerson } from '@/modules/agency/recruiting/clients/types';

const { route } = vi.hoisted(() => ({ route: { path: '/recruiting/companies/company-1', params: { id: 'company-1' }, query: {} } }));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
}));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => ({ userRoles: [] }) }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  getAgencyCompanyContactPerson: vi.fn(),
  deleteAgencyCompanyContactPerson: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/components/ContactPersonForm.vue', () => ({
  default: {
    name: 'ContactPersonForm',
    props: { currentContact: Object, profileId: String },
    emits: ['updateContent', 'cancel'],
    template: '<div class="contact-form-stub" />',
  },
}));

const jane: AgencyCompanyContactPerson = {
  id: 'c1',
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
const bob: AgencyCompanyContactPerson = { id: 'c2', title: 'Mr', firstName: 'Bob', lastName: 'Ray', position: 'Lead', mobileNumber: null, officeNumber: null, email: null };

async function setup(contacts: AgencyCompanyContactPerson[] = [jane, bob]) {
  vi.mocked(getAgencyCompanyContactPerson).mockResolvedValue(contacts);
  const wrapper = mountWithPlugins(ContactPersonList);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').map((row) => row.findAll('td').slice(0, 5).map((cell) => cell.text().replace(/\s+/g, ' ')));
}

function modal(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BModal' });
}

describe('ContactPersonList', () => {
  it('loads the contacts of the routed company and reports the count', async () => {
    const wrapper = await setup();

    expect(getAgencyCompanyContactPerson).toHaveBeenCalledExactlyOnceWith('company-1');
    expect(wrapper.emitted('changed')).toEqual([[2]]);
    expect(rows(wrapper)).toEqual([
      ['Ms Jane Ann Doe', 'HR Manager', '416 555-0101', '416 555-0102 Ext. 12', 'jane@acme.com'],
      ['Mr Bob Ray', 'Lead', 'None', 'None', ''],
    ]);
  });

  it('opens an empty form to create a contact', async () => {
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Create')?.trigger('click');

    expect(wrapper.find('h2').text()).toBe('New contact');
    expect(wrapper.findComponent({ name: 'ContactPersonForm' }).props()).toEqual({ currentContact: null, profileId: 'company-1' });
  });

  it('edits a contact and reloads after saving', async () => {
    const wrapper = await setup();
    await wrapper.find('tbody tr button.is-info').trigger('click');

    expect(wrapper.find('h2').text()).toBe('Edit contact');
    const form = wrapper.findComponent({ name: 'ContactPersonForm' });
    expect(form.props('currentContact')).toEqual(jane);

    form.vm.$emit('updateContent', jane);
    await flushPromises();

    expect(getAgencyCompanyContactPerson).toHaveBeenCalledTimes(2);
    expect(modal(wrapper).props('modelValue')).toBe(false);
  });

  it('closes the form on cancel', async () => {
    const wrapper = await setup();
    await wrapper.find('tbody tr button.is-info').trigger('click');
    wrapper.findComponent({ name: 'ContactPersonForm' }).vm.$emit('cancel');
    await wrapper.vm.$nextTick();

    expect(modal(wrapper).props('modelValue')).toBe(false);
  });

  it('deletes a contact after confirming', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyCompanyContactPerson).mockResolvedValue();
    const wrapper = await setup();
    vi.mocked(getAgencyCompanyContactPerson).mockResolvedValue([bob]);
    await wrapper.findAll('tbody tr')[0].find('button.is-danger').trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this contact');
    expect(deleteAgencyCompanyContactPerson).toHaveBeenCalledExactlyOnceWith('company-1', 'c1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Deleted');
    expect(wrapper.emitted('changed')).toEqual([[2], [1]]);
    expect(rows(wrapper)).toHaveLength(1);
  });

  it('keeps the contact when the deletion is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await wrapper.findAll('tbody tr')[0].find('button.is-danger').trigger('click');
    await flushPromises();

    expect(deleteAgencyCompanyContactPerson).not.toHaveBeenCalled();
  });

  it('shows the error when deleting fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyCompanyContactPerson).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.findAll('tbody tr')[0].find('button.is-danger').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('load failed');
    vi.mocked(getAgencyCompanyContactPerson).mockRejectedValue(error);
    const wrapper = mountWithPlugins(ContactPersonList);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('changed')).toBeUndefined();
  });
});
