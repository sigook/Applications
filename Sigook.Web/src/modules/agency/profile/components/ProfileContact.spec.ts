import { describe, expect, it, vi } from 'vitest';
import { computed, nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import ProfileContact from '@/modules/agency/profile/components/ProfileContact.vue';
import { updateAgencyProfile } from '@/modules/agency/profile/api';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyContactInformation, AgencyDetail } from '@/modules/agency/profile/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertSuccess: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/composables/useBreakpoint', () => ({ useBreakpoint: () => ({ isTouch: computed(() => false) }) }));
vi.mock('@/modules/agency/profile/api', () => ({ updateAgencyProfile: vi.fn() }));

const jane: AgencyContactInformation = {
  title: 'Ms', firstName: 'Jane', middleName: 'M', lastName: 'Doe', position: 'Manager',
  mobileNumber: '416 555-1234', officeNumber: '905 555-1234', officeNumberExt: 7, email: 'jane@covenant.ca',
};

const agency: AgencyDetail = { id: 'agency-1', fullName: 'Covenant', contactInformation: [jane] };

function setup(data: AgencyDetail = agency) {
  return mountWithPlugins(ProfileContact, { props: { agencyData: data } });
}

type Wrapper = ReturnType<typeof setup>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').map((r) => r.findAll('td').slice(0, 4).map((c) => c.text().replace(/\s+/g, ' ')));
}

function input(wrapper: Wrapper, name: string) {
  return wrapper.find(`.modal [name="${name}"]`);
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

async function openAdd(wrapper: Wrapper) {
  await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');
  await nextTick();
}

async function save(wrapper: Wrapper) {
  await wrapper.find('.modal button.is-primary').trigger('click');
}

async function fill(wrapper: Wrapper) {
  await input(wrapper, 'title').setValue('Mr');
  await input(wrapper, 'first name').setValue('John');
  await input(wrapper, 'last name').setValue('Smith');
  await input(wrapper, 'email').setValue('john@covenant.ca');
  await input(wrapper, 'position').setValue('Recruiter');
  await input(wrapper, 'mobileNumber').setValue('6475559876');
}

describe('ProfileContact', () => {
  it('lists the agency contacts', async () => {
    const wrapper = setup();
    await flushPromises();

    expect(rows(wrapper)).toEqual([['Jane M Doe', 'Manager', '416 555-1234905 555-1234 Ext. 7', 'jane@covenant.ca']]);
  });

  it('validates the new contact', async () => {
    const wrapper = setup();
    await openAdd(wrapper);
    await input(wrapper, 'officeNumberExt').setValue('abc');
    await save(wrapper);

    await vi.waitFor(() => {
      expect(errors(wrapper)).toEqual([
        'Title is required',
        'First name is required',
        'Last name is required',
        'Email is required',
        'Position is required',
        'Phone is required',
        'Must be 1-8 digits',
      ]);
    });
    expect(updateAgencyProfile).not.toHaveBeenCalled();
  });

  it('does not save with an invalid office number', async () => {
    const wrapper = setup();
    await openAdd(wrapper);
    await fill(wrapper);
    await input(wrapper, 'officeNumber').setValue('123');
    await save(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Invalid phone number']); });
    expect(updateAgencyProfile).not.toHaveBeenCalled();
  });

  it('adds a contact and saves the agency', async () => {
    vi.mocked(updateAgencyProfile).mockResolvedValue();
    const wrapper = setup();
    await openAdd(wrapper);
    await fill(wrapper);
    await input(wrapper, 'officeNumberExt').setValue('25');
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertSuccess).toHaveBeenCalledWith('Updated'); });
    const added = {
      title: 'Mr', firstName: 'John', middleName: '', lastName: 'Smith', email: 'john@covenant.ca', position: 'Recruiter',
      mobileNumber: '647 555-9876', officeNumber: '', officeNumberExt: 25,
    };
    const expected = { ...agency, contactInformation: [jane, added] };
    expect(updateAgencyProfile).toHaveBeenCalledExactlyOnceWith('agency-1', expected);
    expect(wrapper.emitted('update:agencyData')).toEqual([[expected]]);
    await flushPromises();
    expect(rows(wrapper)).toHaveLength(2);
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });

  it('adds the first contact of an agency without contacts', async () => {
    vi.mocked(updateAgencyProfile).mockResolvedValue();
    const wrapper = setup({ id: 'agency-1', fullName: 'Covenant' });
    await openAdd(wrapper);
    await fill(wrapper);
    await save(wrapper);

    await vi.waitFor(() => { expect(updateAgencyProfile).toHaveBeenCalledOnce(); });
    expect(vi.mocked(updateAgencyProfile).mock.calls[0][1].contactInformation).toHaveLength(1);
  });

  it('keeps the modal open when saving fails', async () => {
    vi.mocked(updateAgencyProfile).mockRejectedValue(new Error('save failed'));
    const wrapper = setup();
    await openAdd(wrapper);
    await fill(wrapper);
    await save(wrapper);

    await vi.waitFor(() => { expect(updateAgencyProfile).toHaveBeenCalledOnce(); });
    await flushPromises();
    expect(showAlertSuccess).not.toHaveBeenCalled();
    expect(showAlertError).toHaveBeenCalledWith(new Error('save failed'));
    expect(wrapper.emitted('update:agencyData')).toBeUndefined();
    expect(rows(wrapper)).toHaveLength(1);
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(true);
  });

  it('removes a contact and saves the agency', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(updateAgencyProfile).mockResolvedValue();
    const wrapper = setup();
    await flushPromises();
    await wrapper.find('tbody button.is-danger').trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this contact');
    expect(rows(wrapper)).toEqual([['No records available']]);
    expect(updateAgencyProfile).toHaveBeenCalledExactlyOnceWith('agency-1', { ...agency, contactInformation: [] });
    expect(wrapper.emitted('update:agencyData')).toEqual([[{ ...agency, contactInformation: [] }]]);
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
  });

  it('keeps the contact when the delete is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = setup();
    await flushPromises();
    await wrapper.find('tbody button.is-danger').trigger('click');
    await flushPromises();

    expect(updateAgencyProfile).not.toHaveBeenCalled();
    expect(wrapper.emitted('update:agencyData')).toBeUndefined();
    expect(rows(wrapper)).toHaveLength(1);
  });

  it('keeps the contact and shows the error when removing fails', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(updateAgencyProfile).mockRejectedValue({ data: 'In use' });
    const wrapper = setup();
    await flushPromises();
    await wrapper.find('tbody button.is-danger').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith({ data: 'In use' });
    expect(showAlertSuccess).not.toHaveBeenCalled();
    expect(wrapper.emitted('update:agencyData')).toBeUndefined();
    expect(rows(wrapper)).toHaveLength(1);
  });

  it('shows the contacts of the new agency data', async () => {
    const wrapper = setup();
    await wrapper.setProps({ agencyData: { ...agency, contactInformation: [jane, { ...jane, firstName: 'Mary', middleName: undefined }] } });
    await flushPromises();

    expect(rows(wrapper).map((r) => r[0])).toEqual(['Jane M Doe', 'Mary Doe']);
  });
});
