import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ProfileBusiness from '@/modules/agency/profile/components/ProfileBusiness.vue';
import { updateAgencyProfile } from '@/modules/agency/profile/api';
import { getWsibGroups } from '@/shared/api/catalogApi';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyDetail } from '@/modules/agency/profile/types';
import type { WsibGroup } from '@/shared/types/common';

vi.mock('@/shared/utils/toast', () => ({ showAlertSuccess: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({ getWsibGroups: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({ updateAgencyProfile: vi.fn() }));

const groups: WsibGroup[] = [{ id: 'g1', value: 'G1' }, { id: 'g2', value: 'G2' }];

const agency: AgencyDetail = {
  id: 'agency-1',
  fullName: 'Covenant Staffing',
  hstNumber: '123456789RT0001',
  businessNumber: '123456789',
  phonePrincipal: '416 555-1234',
  phonePrincipalExt: 12,
  webPage: 'covenant.ca',
  wsibGroup: [{ value: 'G1' }],
};

async function setup(data: AgencyDetail = agency) {
  const wrapper = mountWithPlugins(ProfileBusiness, { props: { agencyData: data } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function input(wrapper: Wrapper, name: string) {
  return wrapper.find(`input[name="${name}"]`);
}

function value(wrapper: Wrapper, name: string) {
  return (input(wrapper, name).element as HTMLInputElement).value;
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

async function save(wrapper: Wrapper) {
  await wrapper.find('button.is-primary').trigger('click');
  await flushPromises();
}

beforeEach(() => {
  vi.mocked(getWsibGroups).mockResolvedValue(groups);
});

describe('ProfileBusiness', () => {
  it('fills the form with the agency data and the WSIB catalog', async () => {
    const wrapper = await setup();

    expect(value(wrapper, 'agency name')).toBe('Covenant Staffing');
    expect(value(wrapper, 'hts #')).toBe('123456789RT0001');
    expect(value(wrapper, 'business #')).toBe('123456789');
    expect(value(wrapper, 'phonePrincipal')).toBe('416 555-1234');
    expect(value(wrapper, 'phonePrincipalExt')).toBe('12');
    expect(value(wrapper, 'web page')).toBe('covenant.ca');
    const taginput = wrapper.findComponent({ name: 'BTaginput' });
    expect(taginput.props('data')).toEqual(groups);
    expect(taginput.props('modelValue')).toEqual([{ value: 'G1' }]);
  });

  it('does not add a WSIB group twice', async () => {
    const wrapper = await setup();
    const beforeAdding = wrapper.findComponent({ name: 'BTaginput' }).props('beforeAdding') as (tag: WsibGroup) => boolean;

    expect(beforeAdding(groups[0])).toBe(false);
    expect(beforeAdding(groups[1])).toBe(true);
  });

  it('validates the business fields', async () => {
    const wrapper = await setup({ id: 'agency-1', fullName: '', phonePrincipal: '416 555-1234' });
    await input(wrapper, 'hts #').setValue('123');
    await input(wrapper, 'phonePrincipalExt').setValue('ext');
    await input(wrapper, 'web page').setValue('not a url');
    await save(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Agency name is required', 'Min 15 characters', 'Business number is required', 'Must be 1-8 digits', 'Invalid URL']); });
    expect(updateAgencyProfile).not.toHaveBeenCalled();
  });

  it('does not save without a valid phone', async () => {
    const wrapper = await setup();
    await input(wrapper, 'phonePrincipal').setValue('');
    await save(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Phone is required']); });
    expect(updateAgencyProfile).not.toHaveBeenCalled();
    expect(wrapper.emitted('update:agencyData')).toBeUndefined();
  });

  it('saves the edited business data', async () => {
    vi.mocked(updateAgencyProfile).mockResolvedValue();
    const wrapper = await setup();
    await input(wrapper, 'agency name').setValue('Covenant Group');
    await input(wrapper, 'phonePrincipal').setValue('6475559876');
    await input(wrapper, 'phonePrincipalExt').setValue('');
    await save(wrapper);

    const expected: AgencyDetail = {
      ...agency,
      fullName: 'Covenant Group',
      phonePrincipal: '647 555-9876',
      phonePrincipalExt: null,
    };
    await vi.waitFor(() => { expect(wrapper.emitted('saved')).toHaveLength(1); });
    expect(wrapper.emitted('update:agencyData')).toEqual([[expected]]);
    expect(updateAgencyProfile).toHaveBeenCalledExactlyOnceWith('agency-1', expected);
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
    expect(wrapper.emitted('saved')).toHaveLength(1);
  });

  it('keeps the modal open when saving fails', async () => {
    vi.mocked(updateAgencyProfile).mockRejectedValue(new Error('save failed'));
    const wrapper = await setup();
    await save(wrapper);

    await vi.waitFor(() => { expect(updateAgencyProfile).toHaveBeenCalledWith('agency-1', expect.objectContaining({ phonePrincipalExt: 12 })); });
    await flushPromises();
    expect(showAlertSuccess).not.toHaveBeenCalled();
    expect(showAlertError).toHaveBeenCalledWith(new Error('save failed'));
    expect(wrapper.emitted('saved')).toBeUndefined();
    expect(wrapper.emitted('update:agencyData')).toBeUndefined();
  });

  it('refills the form when the agency changes', async () => {
    const wrapper = await setup();
    await wrapper.setProps({ agencyData: { ...agency, fullName: 'Sigook', phonePrincipalExt: null, webPage: undefined } });
    await flushPromises();

    expect(value(wrapper, 'agency name')).toBe('Sigook');
    expect(value(wrapper, 'phonePrincipalExt')).toBe('');
    expect(value(wrapper, 'web page')).toBe('');
  });

  it('still opens when the WSIB catalog fails', async () => {
    vi.mocked(getWsibGroups).mockRejectedValue(new Error('catalog failed'));
    const wrapper = await setup();

    expect(wrapper.findComponent({ name: 'BTaginput' }).props('data')).toEqual([]);
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });
});
