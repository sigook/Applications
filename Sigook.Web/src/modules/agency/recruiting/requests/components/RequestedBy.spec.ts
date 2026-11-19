import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import RequestedBy from '@/modules/agency/recruiting/requests/components/RequestedBy.vue';
import { deleteAgencyRequestRequestedBy, postAgencyRequestRequestedBy } from '@/modules/agency/recruiting/requests/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyCompanyContactPerson } from '@/modules/agency/recruiting/clients/types';
import type { AgencyRequestPersonItem } from '@/modules/agency/recruiting/requests/types';

vi.mock('@/modules/agency/recruiting/requests/api', () => ({ postAgencyRequestRequestedBy: vi.fn(), deleteAgencyRequestRequestedBy: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/recruiting/requests/components/ContactListModal.vue', () => ({
  default: {
    name: 'ContactListModal',
    props: { companyProfileId: String, activeUsers: Array },
    emits: ['selectContact', 'removeContact'],
    template: '<div class="contact-list-stub" />',
  },
}));

const john: AgencyRequestPersonItem = { id: 'c-1', firstName: 'John', lastName: 'Smith' };
const jane: AgencyCompanyContactPerson = { id: 'c-2', firstName: 'Jane', lastName: 'Doe' };

function setup(modelValue: AgencyRequestPersonItem[] = [john], canEdit = true) {
  return mountWithPlugins(RequestedBy, { props: { requestId: 'req-1', companyProfileId: 'cp-1', canEdit, modelValue } });
}

type Wrapper = ReturnType<typeof setup>;

function lastModel(wrapper: Wrapper) {
  return wrapper.emitted<[AgencyRequestPersonItem[]]>('update:modelValue')?.at(-1)?.[0];
}

async function openList(wrapper: Wrapper) {
  await wrapper.find('button.is-primary').trigger('click');
  await nextTick();
  return wrapper.findComponent({ name: 'ContactListModal' });
}

function modalOpen(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BModal' }).props('modelValue');
}

describe('RequestedBy', () => {
  it('lists the contacts with remove buttons when editable', () => {
    const wrapper = setup();

    expect(wrapper.find('h3').text()).toBe('Requested by');
    expect(wrapper.find('li').text()).toBe('John Smith');
    expect(wrapper.find('li button').attributes('aria-label')).toBe('Remove John Smith');
  });

  it('is read-only without edit permission', () => {
    const wrapper = setup([john], false);

    expect(wrapper.find('button').exists()).toBe(false);
  });

  it('passes the company and active contacts to the list', async () => {
    const wrapper = setup();
    const list = await openList(wrapper);

    expect(list.props()).toEqual({ companyProfileId: 'cp-1', activeUsers: [john] });
  });

  it('adds a contact and closes the list', async () => {
    vi.mocked(postAgencyRequestRequestedBy).mockResolvedValue();
    const wrapper = setup();
    const list = await openList(wrapper);
    list.vm.$emit('selectContact', jane);
    await flushPromises();

    expect(postAgencyRequestRequestedBy).toHaveBeenCalledExactlyOnceWith('req-1', 'c-2');
    expect(lastModel(wrapper)).toEqual([john, jane]);
    expect(modalOpen(wrapper)).toBe(false);
  });

  it('ignores a contact without id', async () => {
    const wrapper = setup();
    const list = await openList(wrapper);
    list.vm.$emit('selectContact', { firstName: 'X', lastName: 'Y' });
    list.vm.$emit('removeContact', { firstName: 'X', lastName: 'Y' });
    await flushPromises();

    expect(postAgencyRequestRequestedBy).not.toHaveBeenCalled();
    expect(deleteAgencyRequestRequestedBy).not.toHaveBeenCalled();
  });

  it('keeps the list open when adding fails', async () => {
    const error = new Error('boom');
    vi.mocked(postAgencyRequestRequestedBy).mockRejectedValue(error);
    const wrapper = setup();
    const list = await openList(wrapper);
    list.vm.$emit('selectContact', jane);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(lastModel(wrapper)).toBeUndefined();
    expect(modalOpen(wrapper)).toBe(true);
  });

  it('removes a contact from the row', async () => {
    vi.mocked(deleteAgencyRequestRequestedBy).mockResolvedValue();
    const wrapper = setup();
    await wrapper.find('li button').trigger('click');
    await flushPromises();

    expect(deleteAgencyRequestRequestedBy).toHaveBeenCalledExactlyOnceWith('req-1', 'c-1');
    expect(lastModel(wrapper)).toEqual([]);
  });

  it('shows the error when removing fails', async () => {
    const error = new Error('boom');
    vi.mocked(deleteAgencyRequestRequestedBy).mockRejectedValue(error);
    const wrapper = setup();
    await wrapper.find('li button').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(lastModel(wrapper)).toBeUndefined();
  });
});
