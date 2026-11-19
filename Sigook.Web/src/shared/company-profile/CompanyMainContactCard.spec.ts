import { describe, expect, it, vi } from 'vitest';
import CompanyMainContactCard from '@/shared/company-profile/CompanyMainContactCard.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyContactSummary } from '@/shared/company-profile/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

type FetchContacts = () => Promise<CompanyContactSummary[]>;

const main: CompanyContactSummary = {
  firstName: 'Ann',
  middleName: 'Marie',
  lastName: 'Lee',
  position: 'Plant manager',
  mobileNumber: '4165550000',
  email: 'ann@acme.com',
};

function setup(fetchContacts: FetchContacts) {
  return mountWithPlugins(CompanyMainContactCard, { props: { fetchContacts } });
}

describe('CompanyMainContactCard', () => {
  it('shows the empty state and asks to add a contact', async () => {
    const wrapper = setup(vi.fn<FetchContacts>().mockResolvedValue([]));

    await vi.waitFor(() => { expect(wrapper.emitted('loaded')).toEqual([[0]]); });
    expect(wrapper.find('.detail-empty').text()).toBe('No contacts');
    await wrapper.find('.company-contact-empty button').trigger('click');
    expect(wrapper.emitted('showAll')).toHaveLength(1);
  });

  it('shows the first contact with phone and email', async () => {
    const wrapper = setup(vi.fn<FetchContacts>().mockResolvedValue([main]));

    await vi.waitFor(() => { expect(wrapper.find('.company-contact-name').exists()).toBe(true); });
    expect(wrapper.find('.company-contact-name').text()).toBe('Ann Marie Lee');
    expect(wrapper.find('.detail-muted').text()).toBe('Plant manager');
    expect(wrapper.findAll('a').map((a) => a.attributes('href'))).toEqual(['tel:4165550000', 'mailto:ann@acme.com']);
    expect(wrapper.find('.detail-card-actions').exists()).toBe(false);
  });

  it('links to all contacts when there are several', async () => {
    const wrapper = setup(vi.fn<FetchContacts>().mockResolvedValue([
      main,
      { firstName: 'Bob', lastName: 'Stone', position: null },
    ]));

    await vi.waitFor(() => { expect(wrapper.find('.detail-card-actions').exists()).toBe(true); });
    const all = wrapper.find('.detail-card-actions button');
    expect(all.text()).toBe('All 2');
    await all.trigger('click');
    expect(wrapper.emitted('showAll')).toHaveLength(1);
    expect(wrapper.emitted('loaded')).toEqual([[2]]);
  });

  it('omits the missing position', async () => {
    const wrapper = setup(vi.fn<FetchContacts>().mockResolvedValue([{ firstName: 'Bob', lastName: 'Stone' }]));

    await vi.waitFor(() => { expect(wrapper.find('.company-contact-name').text()).toBe('Bob Stone'); });
    expect(wrapper.find('.detail-muted').exists()).toBe(false);
    expect(wrapper.find('.copy-link').exists()).toBe(false);
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('load failed');
    const wrapper = setup(vi.fn<FetchContacts>().mockRejectedValue(error));

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('loaded')).toBeUndefined();
  });
});
