import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import CreateCandidate from '@/modules/agency/recruiting/candidates/components/CreateCandidate.vue';
import { createAgencyCandidate } from '@/modules/agency/recruiting/candidates/api';
import { getGenders, getSources } from '@/shared/api/catalogApi';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CatalogItem, City, LocationModel } from '@/shared/types/common';

const { validateAddress } = vi.hoisted(() => ({ validateAddress: vi.fn<() => Promise<boolean>>() }));

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({ getGenders: vi.fn(), getSources: vi.fn() }));
vi.mock('@/modules/agency/recruiting/candidates/api', () => ({ createAgencyCandidate: vi.fn() }));
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
const indeed: CatalogItem = { id: 's1', value: 'Indeed' };
const toronto: City = { id: 'to', value: 'Toronto', province: { id: 'on', value: 'Ontario' } };
const resume = new File(['%PDF'], 'cv.pdf', { type: 'application/pdf' });

async function setup() {
  const wrapper = mountWithPlugins(CreateCandidate);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function step(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BSteps' }).props('modelValue') as number;
}

function selects(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'BSelect' });
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

async function fillBasic(wrapper: Wrapper, location: LocationModel = { address: '100 King St', postalCode: 'M5H 1A1', city: toronto }) {
  await wrapper.find('input[type="text"]').setValue('Ana Smith');
  await wrapper.find('input[type="email"]').setValue('ana@mail.com');
  await wrapper.find('input[name="phone"]').setValue('4165551234');
  wrapper.findComponent({ name: 'LocationAddress' }).vm.$emit('update:model', location);
  selects(wrapper)[0].vm.$emit('update:modelValue', 's1');
  await nextTick();
}

async function next(wrapper: Wrapper) {
  vi.mocked(showAlertError).mockClear();
  await button(wrapper, 'Next').trigger('click');
  await vi.waitFor(() => {
    if (step(wrapper) === 0 && vi.mocked(showAlertError).mock.calls.length === 0) throw new Error('step pending');
  });
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
}

beforeEach(() => {
  validateAddress.mockResolvedValue(true);
  vi.mocked(getGenders).mockResolvedValue([male]);
  vi.mocked(getSources).mockResolvedValue([indeed]);
});

describe('CreateCandidate', () => {
  it('starts on the basic step with the catalogs loaded', async () => {
    const wrapper = await setup();

    expect(step(wrapper)).toBe(0);
    expect(selects(wrapper)[0].findAll('option').map((o) => o.text())).toContain('Indeed');
    expect(selects(wrapper)[1].findAll('option').map((o) => o.text())).toEqual(expect.arrayContaining(['Citizen', 'Work Permit', 'Student', 'Permanent Resident']));
    expect(selects(wrapper)[2].findAll('option').map((o) => o.text())).toContain('Male');
    expect(wrapper.findComponent({ name: 'LocationAddress' }).props('requirePostalCode')).toBe(false);
  });

  it('validates the basic step before moving on', async () => {
    validateAddress.mockResolvedValue(false);
    const wrapper = await setup();
    await next(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Full name is required', 'Email is required', 'Phone is required', 'Source is required']); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(step(wrapper)).toBe(0);
  });

  it('clears the source error once a source is chosen', async () => {
    const wrapper = await setup();
    await next(wrapper);
    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Source is required'); });

    selects(wrapper)[0].vm.$emit('update:modelValue', 's1');
    await nextTick();

    expect(errors(wrapper)).not.toContain('Source is required');
  });

  it('does not move on with an invalid address', async () => {
    validateAddress.mockResolvedValue(false);
    const wrapper = await setup();
    await fillBasic(wrapper);
    await next(wrapper);

    expect(step(wrapper)).toBe(0);
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
  });

  it('moves between the steps', async () => {
    const wrapper = await setup();
    await fillBasic(wrapper);
    await next(wrapper);

    expect(step(wrapper)).toBe(1);

    await button(wrapper, 'Previous').trigger('click');
    expect(step(wrapper)).toBe(0);
  });

  it('creates the candidate with the resume and the additional data', async () => {
    vi.mocked(createAgencyCandidate).mockResolvedValue({ id: 'cand-1' });
    const wrapper = await setup();
    await fillBasic(wrapper);
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', resume);
    await nextTick();
    expect(wrapper.text()).toContain('cv.pdf');
    await next(wrapper);

    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', ['Forklift', 'Packing']);
    selects(wrapper)[1].vm.$emit('update:modelValue', 'Student');
    selects(wrapper)[2].vm.$emit('update:modelValue', male);
    await wrapper.find('.switch input').setValue(true);
    expect(wrapper.find('.switch').text()).toBe('Yes');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('onClose')).toEqual([[true]]); });
    expect(createAgencyCandidate).toHaveBeenCalledExactlyOnceWith({
      name: 'Ana Smith',
      email: 'ana@mail.com',
      address: '100 King St',
      postalCode: 'M5H 1A1',
      city: { id: 'to', value: 'Toronto' },
      skills: [{ skill: 'Forklift' }, { skill: 'Packing' }],
      phoneNumbers: [{ phoneNumber: '416 555-1234' }],
      sourceId: 's1',
      gender: male,
      hasVehicle: true,
      residencyStatus: 'Student',
      fileName: expect.stringMatching(/^Resume_[0-9a-f]{32}\.pdf$/) as string,
    }, resume);
    expect(showAlertSuccess).toHaveBeenCalledWith('Created');
  });

  it('creates a candidate without resume nor city', async () => {
    vi.mocked(createAgencyCandidate).mockResolvedValue({ id: 'cand-1' });
    const wrapper = await setup();
    await fillBasic(wrapper, { address: '', postalCode: '', city: null });
    await next(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(createAgencyCandidate).toHaveBeenCalledOnce(); });
    expect(vi.mocked(createAgencyCandidate).mock.calls[0]).toEqual([
      expect.objectContaining({ postalCode: undefined, city: null, skills: [], gender: null, hasVehicle: false, residencyStatus: null, fileName: null }),
      null,
    ]);
  });

  it('goes back to the basic step when the phone is no longer valid', async () => {
    const wrapper = await setup();
    await fillBasic(wrapper);
    await next(wrapper);
    expect(step(wrapper)).toBe(1);
    await wrapper.find('input[name="phone"]').setValue('123');
    await submit(wrapper);

    await vi.waitFor(() => { expect(step(wrapper)).toBe(0); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createAgencyCandidate).not.toHaveBeenCalled();
  });

  it('shows the error when creating fails', async () => {
    const error = new Error('create failed');
    vi.mocked(createAgencyCandidate).mockRejectedValue(error);
    const wrapper = await setup();
    await fillBasic(wrapper);
    await next(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('onClose')).toBeUndefined();
  });

  it('shows the error when the catalogs fail to load', async () => {
    const error = new Error('catalog failed');
    vi.mocked(getSources).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
