import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick, reactive } from 'vue';
import { flushPromises } from '@vue/test-utils';
import DetailCandidate from '@/modules/agency/recruiting/candidates/components/DetailCandidate.vue';
import { getAgencyCandidate, updateAgencyCandidate } from '@/modules/agency/recruiting/candidates/api';
import { getGenders, getSources } from '@/shared/api/catalogApi';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CandidateDetail } from '@/modules/agency/recruiting/candidates/types';
import type { CatalogItem, City, LocationModel } from '@/shared/types/common';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });
const { validateAddress } = vi.hoisted(() => ({ validateAddress: vi.fn<() => Promise<boolean>>() }));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({ getGenders: vi.fn(), getSources: vi.fn() }));
vi.mock('@/modules/agency/recruiting/candidates/api', () => ({ getAgencyCandidate: vi.fn(), updateAgencyCandidate: vi.fn() }));
vi.mock('@/shared/ui/Address.vue', async () => {
  const { defineComponent } = await import('vue');
  return {
    default: defineComponent({
      name: 'LocationAddress',
      props: { model: { type: Object, default: null }, requirePostalCode: { type: Boolean, default: true } },
      emits: ['update:model', 'isLoading'],
      setup(_, { expose }) {
        expose({ validateAddress });
      },
      template: '<div class="address-stub" />',
    }),
  };
});

const male: CatalogItem = { id: 'g1', value: 'Male' };
const female: CatalogItem = { id: 'g2', value: 'Female' };
const indeed: CatalogItem = { id: 's1', value: 'Indeed' };
const referral: CatalogItem = { id: 's2', value: 'Referral' };
const toronto: City = { id: 'to', value: 'Toronto', province: { id: 'on', value: 'Ontario' } };

const candidate: CandidateDetail = {
  id: 'cand-1',
  numberId: 77,
  name: 'Ana Smith',
  email: 'ana@mail.com',
  address: '100 King St',
  postalCode: 'M5H 1A1',
  city: toronto,
  gender: female,
  hasVehicle: true,
  residencyStatus: 'Citizen',
  sourceId: 's1',
  dnu: false,
  phoneNumbers: [{ id: 'ph1', phoneNumber: '416 555-1234' }],
};

async function setup(candidateId: number | string = 'cand-1') {
  const wrapper = mountWithPlugins(DetailCandidate, { props: { candidateId } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function value(wrapper: Wrapper, name: string) {
  return (wrapper.find(`input[name="${name}"]`).element as HTMLInputElement).value;
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

function selects(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'BSelect' });
}

function dnu(wrapper: Wrapper) {
  return wrapper.find('input[type="checkbox"]').element as HTMLInputElement;
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
}

beforeEach(() => {
  securityStore.userRoles = ['recruiting'];
  validateAddress.mockResolvedValue(true);
  vi.mocked(getGenders).mockResolvedValue([male, female]);
  vi.mocked(getSources).mockResolvedValue([indeed, referral]);
  vi.mocked(getAgencyCandidate).mockResolvedValue(structuredClone(candidate));
});

describe('DetailCandidate', () => {
  it('loads the candidate into the form', async () => {
    const wrapper = await setup(77);

    expect(getAgencyCandidate).toHaveBeenCalledWith('77');
    expect(wrapper.find('h2').text()).toBe('Ana Smith');
    expect(value(wrapper, 'name')).toBe('Ana Smith');
    expect(value(wrapper, 'email')).toBe('ana@mail.com');
    expect(value(wrapper, 'phone')).toBe('416 555-1234');
    expect(wrapper.findComponent({ name: 'LocationAddress' }).props()).toEqual({
      model: { address: '100 King St', postalCode: 'M5H 1A1', city: toronto },
      requirePostalCode: false,
    });
    expect(selects(wrapper).map((s) => s.props('modelValue'))).toEqual(['s1', 'Citizen', female]);
    expect(selects(wrapper)[0].findAll('option').map((o) => o.text())).toContain('Referral');
    expect(wrapper.find('.switch').text()).toBe('Yes');
    expect(dnu(wrapper).disabled).toBe(false);
  });

  it('only lets an admin clear the DNU flag', async () => {
    vi.mocked(getAgencyCandidate).mockResolvedValue({ ...candidate, dnu: true });
    const wrapper = await setup();

    expect(dnu(wrapper).disabled).toBe(true);

    securityStore.userRoles = ['admin'];
    await nextTick();

    expect(dnu(wrapper).disabled).toBe(false);
  });

  it('validates the candidate before updating', async () => {
    vi.mocked(getAgencyCandidate).mockResolvedValue({ ...candidate, sourceId: null, email: undefined, phoneNumbers: [] });
    validateAddress.mockResolvedValue(false);
    const wrapper = await setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Email is required', 'Phone is required', 'Source is required']); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(updateAgencyCandidate).not.toHaveBeenCalled();
  });

  it('clears the source error once a source is chosen', async () => {
    vi.mocked(getAgencyCandidate).mockResolvedValue({ ...candidate, sourceId: null });
    const wrapper = await setup();
    await submit(wrapper);
    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Source is required']); });

    selects(wrapper)[0].vm.$emit('update:modelValue', 's2');
    await nextTick();

    expect(errors(wrapper)).toEqual([]);
  });

  it('updates the edited candidate', async () => {
    vi.mocked(updateAgencyCandidate).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('input[name="name"]').setValue('Ana Brown');
    await wrapper.find('input[name="phone"]').setValue('6475559876');
    const location: LocationModel = { address: '9 Queen St', postalCode: '', city: toronto };
    wrapper.findComponent({ name: 'LocationAddress' }).vm.$emit('update:model', location);
    selects(wrapper)[2].vm.$emit('update:modelValue', male);
    await dnu(wrapper).click();
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('onUpdateWorker')).toEqual([[true]]); });
    expect(updateAgencyCandidate).toHaveBeenCalledExactlyOnceWith('cand-1', {
      ...candidate,
      name: 'Ana Brown',
      address: '9 Queen St',
      postalCode: undefined,
      city: { id: 'to', value: 'Toronto' },
      gender: male,
      dnu: true,
      phoneNumbers: [{ phoneNumber: '647 555-9876' }],
    });
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
  });

  it('sends no city when the location has none', async () => {
    vi.mocked(updateAgencyCandidate).mockResolvedValue();
    vi.mocked(getAgencyCandidate).mockResolvedValue({ ...candidate, address: undefined, postalCode: undefined, city: null });
    const wrapper = await setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(updateAgencyCandidate).toHaveBeenCalledOnce(); });
    expect(vi.mocked(updateAgencyCandidate).mock.calls[0][1]).toMatchObject({ address: '', postalCode: undefined, city: null });
  });

  it('shows the error when updating fails', async () => {
    const error = new Error('update failed');
    vi.mocked(updateAgencyCandidate).mockRejectedValue(error);
    const wrapper = await setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('onUpdateWorker')).toBeUndefined();
  });

  it('shows the error when the candidate cannot be loaded', async () => {
    const error = new Error('load failed');
    vi.mocked(getAgencyCandidate).mockRejectedValue(error);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.find('form').exists()).toBe(false);
  });

  it('does not load the candidate when the catalogs fail', async () => {
    const error = new Error('catalog failed');
    vi.mocked(getSources).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getAgencyCandidate).not.toHaveBeenCalled();
  });
});
