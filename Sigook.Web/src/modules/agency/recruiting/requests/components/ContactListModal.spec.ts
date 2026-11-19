import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import ContactListModal from '@/modules/agency/recruiting/requests/components/ContactListModal.vue';
import { getAgencyCompanyContactPerson } from '@/modules/agency/recruiting/clients/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyCompanyContactPerson } from '@/modules/agency/recruiting/clients/types';
import type { AgencyRequestPersonItem } from '@/modules/agency/recruiting/requests/types';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({ getAgencyCompanyContactPerson: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/components/ContactPersonForm.vue', () => ({
  default: {
    name: 'ContactPersonForm',
    props: { profileId: String },
    emits: ['updateContent', 'cancel'],
    template: '<div class="contact-form-stub" />',
  },
}));

const john: AgencyCompanyContactPerson = {
  id: 'c-1',
  title: 'Mr.',
  firstName: 'John',
  lastName: 'Smith',
  position: 'Manager',
  mobileNumber: '4165550000',
  officeNumber: '4165551111',
  officeNumberExt: 22,
  email: 'john@acme.com',
};
const jane: AgencyCompanyContactPerson = { id: 'c-2', firstName: 'Jane', lastName: 'Doe', position: 'Lead' };

async function setup(activeUsers: AgencyRequestPersonItem[] = []) {
  const wrapper = mountWithPlugins(ContactListModal, { props: { companyProfileId: 'cp-1', activeUsers } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('li');
}

describe('ContactListModal', () => {
  it('lists the company contacts with remove for active ones', async () => {
    vi.mocked(getAgencyCompanyContactPerson).mockResolvedValue([john, jane]);
    const wrapper = await setup([{ id: 'c-1' }]);

    expect(getAgencyCompanyContactPerson).toHaveBeenCalledExactlyOnceWith('cp-1');
    expect(rows(wrapper)).toHaveLength(2);
    expect(rows(wrapper)[0].find('b').text()).toBe('Mr. John Smith');
    expect(rows(wrapper)[0].text()).toContain('| Manager');
    expect(rows(wrapper)[0].text()).toContain('Ext. 22');
    expect(rows(wrapper)[0].find('a[href="mailto:john@acme.com"]').exists()).toBe(true);
    expect(rows(wrapper).map((r) => r.find('button.button').text())).toEqual(['Remove', 'Add']);
  });

  it('emits remove and select', async () => {
    vi.mocked(getAgencyCompanyContactPerson).mockResolvedValue([john, jane]);
    const wrapper = await setup([{ id: 'c-1' }]);
    await rows(wrapper)[0].find('button.button').trigger('click');
    await rows(wrapper)[1].find('button.button').trigger('click');

    expect(wrapper.emitted('removeContact')).toEqual([[john]]);
    expect(wrapper.emitted('selectContact')).toEqual([[jane]]);
  });

  it('creates a contact, lists it and selects it', async () => {
    vi.mocked(getAgencyCompanyContactPerson).mockResolvedValue([john]);
    const wrapper = await setup();
    await wrapper.find('.button.is-primary').trigger('click');
    const form = wrapper.findComponent({ name: 'ContactPersonForm' });
    expect(form.props('profileId')).toBe('cp-1');
    expect(wrapper.text()).toContain('New contact');
    form.vm.$emit('updateContent', jane);
    await nextTick();

    expect(wrapper.emitted('selectContact')).toEqual([[jane]]);
    expect(rows(wrapper)).toHaveLength(2);
  });

  it('goes back to the list when creation is cancelled', async () => {
    vi.mocked(getAgencyCompanyContactPerson).mockResolvedValue([john]);
    const wrapper = await setup();
    await wrapper.find('.button.is-primary').trigger('click');
    wrapper.findComponent({ name: 'ContactPersonForm' }).vm.$emit('cancel');
    await nextTick();

    expect(wrapper.findComponent({ name: 'ContactPersonForm' }).exists()).toBe(false);
    expect(rows(wrapper)).toHaveLength(1);
  });

  it('shows the error when contacts fail to load', async () => {
    const error = new Error('boom');
    vi.mocked(getAgencyCompanyContactPerson).mockRejectedValue(error);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(rows(wrapper)).toHaveLength(0);
  });
});
