import { beforeEach, describe, expect, it, vi } from 'vitest';
import { defineComponent, reactive } from 'vue';
import { flushPromises } from '@vue/test-utils';
import Address from '@/shared/ui/Address.vue';
import { createCity, getCities, getCountries, getProvinces } from '@/shared/api/locationApi';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { City, Country, LocationDetailModel, Province } from '@/shared/types/common';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/shared/api/locationApi', () => ({
  getCountries: vi.fn(),
  getProvinces: vi.fn(),
  getCities: vi.fn(),
  createCity: vi.fn(),
}));
vi.mock('@/shared/utils/buefyProgrammatic', () => ({ getDialog: vi.fn() }));

const ProvinceSettingsStub = defineComponent({
  name: 'ProvinceSettingsModal',
  props: { provinceId: { type: String, default: '' }, provinceName: { type: String, default: '' }, currentSettings: { type: Object, default: null } },
  emits: ['saved'],
  template: '<div class="province-settings-stub" />',
});

const canada: Country = { id: 'ca', value: 'Canada', code: 'CA' };
const usa: Country = { id: 'us', value: 'United States', code: 'US' };
const ontario: Province = { id: 'on', value: 'Ontario', country: canada, settings: null };
const quebec: Province = { id: 'qc', value: 'Quebec', country: canada };
const toronto: City = { id: 'to', value: 'Toronto', province: ontario };
const ottawa: City = { id: 'ot', value: 'Ottawa', province: ontario };

function savedModel(): LocationDetailModel {
  return { id: 'loc-1', address: '100 King St', postalCode: 'M5H 1A1', city: { ...toronto, province: { ...ontario, country: canada } } };
}

interface Props {
  model?: LocationDetailModel | null;
  enableProvinceSettings?: boolean;
  requirePostalCode?: boolean;
}

async function setup(props: Props = {}) {
  const wrapper = mountWithPlugins(Address, { props: { model: null, ...props }, global: { stubs: { ProvinceSettingsModal: ProvinceSettingsStub } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function select(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BSelect' });
}

function provinceInput(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'BAutocomplete' })[0];
}

function cityInput(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'BAutocomplete' })[1];
}

function inputValue(wrapper: Wrapper, name: string) {
  return (wrapper.find(`input[name="${name}"]`).element as HTMLInputElement).value;
}

function isDisabled(wrapper: Wrapper, name: string) {
  return (wrapper.find(`input[name="${name}"]`).element as HTMLInputElement).disabled;
}

function messages(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((m) => m.text());
}

function lastModel(wrapper: Wrapper) {
  return wrapper.emitted('update:model')?.at(-1)?.[0] as LocationDetailModel;
}

async function validate(wrapper: Wrapper) {
  const valid = await (wrapper.vm as unknown as { validateAddress: () => Promise<boolean> }).validateAddress();
  await flushPromises();
  return valid;
}

async function pickCountry(wrapper: Wrapper, country: Country) {
  select(wrapper).vm.$emit('update:modelValue', country);
  await flushPromises();
}

async function pickProvince(wrapper: Wrapper, province: Province) {
  provinceInput(wrapper).vm.$emit('update:modelValue', province.value);
  provinceInput(wrapper).vm.$emit('select', province);
  await flushPromises();
}

async function pickCity(wrapper: Wrapper, city: City) {
  cityInput(wrapper).vm.$emit('update:modelValue', city.value);
  cityInput(wrapper).vm.$emit('select', city);
  await flushPromises();
}

beforeEach(() => {
  securityStore.userRoles = [];
  vi.mocked(getCountries).mockResolvedValue([canada, usa]);
  vi.mocked(getProvinces).mockResolvedValue([ontario, quebec]);
  vi.mocked(getCities).mockResolvedValue([toronto, ottawa]);
});

describe('Address', () => {
  it('mounts without a model', async () => {
    const wrapper = mountWithPlugins(Address, { global: { stubs: { ProvinceSettingsModal: ProvinceSettingsStub } } });
    await flushPromises();

    expect(select(wrapper).exists()).toBe(true);
  });

  it('loads the countries and keeps province and city locked until chosen', async () => {
    const wrapper = await setup();

    expect(wrapper.findAll('select option').map((o) => o.text())).toEqual(['Select', 'Canada', 'United States']);
    expect(wrapper.emitted('isLoading')).toEqual([[true], [false]]);
    expect(isDisabled(wrapper, 'province')).toBe(true);
    expect(isDisabled(wrapper, 'city')).toBe(true);
    expect(getProvinces).not.toHaveBeenCalled();
  });

  it('prefills a saved location', async () => {
    const wrapper = await setup({ model: savedModel() });

    expect(getProvinces).toHaveBeenCalledWith('ca');
    expect(getCities).toHaveBeenCalledWith('on');
    expect(inputValue(wrapper, 'province')).toBe('Ontario');
    expect(inputValue(wrapper, 'city')).toBe('Toronto');
    expect(inputValue(wrapper, 'address')).toBe('100 King St');
    expect(inputValue(wrapper, 'postalCode')).toBe('M5H 1A1');
    expect(await validate(wrapper)).toBe(true);
    expect(messages(wrapper)).toEqual([]);
  });

  it('loads provinces for the chosen country and reports whether it is Canada', async () => {
    const wrapper = await setup();
    await pickCountry(wrapper, canada);

    expect(getProvinces).toHaveBeenCalledWith('ca');
    expect(isDisabled(wrapper, 'province')).toBe(false);
    expect(provinceInput(wrapper).props('data')).toEqual([ontario, quebec]);

    await pickCountry(wrapper, usa);
    expect(wrapper.emitted('isCanada')).toEqual([[true], [false]]);
  });

  it('filters provinces and cities by the typed text', async () => {
    const wrapper = await setup({ model: savedModel() });

    provinceInput(wrapper).vm.$emit('update:modelValue', 'que');
    await flushPromises();
    expect(provinceInput(wrapper).props('data')).toEqual([quebec]);

    await pickProvince(wrapper, ontario);
    cityInput(wrapper).vm.$emit('update:modelValue', 'TT');
    await flushPromises();
    expect(cityInput(wrapper).props('data')).toEqual([ottawa]);
  });

  it('clears the city when the province text no longer matches', async () => {
    const wrapper = await setup({ model: savedModel() });

    provinceInput(wrapper).vm.$emit('update:modelValue', 'Ont');
    await flushPromises();

    expect(inputValue(wrapper, 'city')).toBe('');
    expect(isDisabled(wrapper, 'city')).toBe(true);
  });

  it('emits the location with the chosen city, province and country', async () => {
    const wrapper = await setup({ model: { address: '', postalCode: '' } });
    await pickCountry(wrapper, canada);
    await pickProvince(wrapper, quebec);

    expect(getCities).toHaveBeenCalledWith('qc');
    await pickCity(wrapper, ottawa);

    expect(lastModel(wrapper).city).toEqual({ ...ottawa, province: { ...quebec, country: canada } });
  });

  it('emits typed address and postal code', async () => {
    const wrapper = await setup({ model: savedModel() });
    await wrapper.find('input[name="address"]').setValue('200 Bay St');
    await flushPromises();
    expect(lastModel(wrapper).address).toBe('200 Bay St');

    await wrapper.find('input[name="postalCode"]').setValue('M5J 2J1');
    await flushPromises();
    expect(lastModel(wrapper).postalCode).toBe('M5J 2J1');
  });

  it('does not emit edits without a model', async () => {
    const wrapper = await setup();
    await wrapper.find('input[name="address"]').setValue('200 Bay St');
    await flushPromises();

    expect(wrapper.emitted('update:model')).toBeUndefined();
  });

  it('reports every required field', async () => {
    const wrapper = await setup();

    expect(await validate(wrapper)).toBe(false);
    expect(messages(wrapper)).toEqual([
      'Country is required',
      'Province is required',
      'City is required',
      'Address is required',
      'Postal code is required',
    ]);
  });

  it('does not require the postal code when told so', async () => {
    const wrapper = await setup({ requirePostalCode: false });
    await validate(wrapper);

    expect(messages(wrapper)).not.toContain('Postal code is required');
    expect(wrapper.findAll('label .has-text-danger')).toHaveLength(4);
  });

  it.each([
    ['12345', 'Address must be at least 6 characters'],
    ['100 King St!', 'Invalid address format'],
  ])('rejects the address %s', async (address, message) => {
    const wrapper = await setup({ model: savedModel() });
    await wrapper.find('input[name="address"]').setValue(address);

    expect(await validate(wrapper)).toBe(false);
    expect(messages(wrapper)).toEqual([message]);
  });

  it('rejects an invalid postal code', async () => {
    const wrapper = await setup({ model: savedModel() });
    await wrapper.find('input[name="postalCode"]').setValue('ABC');

    expect(await validate(wrapper)).toBe(false);
    expect(messages(wrapper)).toEqual(['Invalid postal code']);
  });

  it('asks to pick province and city from the list', async () => {
    const wrapper = await setup({ model: savedModel() });
    provinceInput(wrapper).vm.$emit('update:modelValue', 'Atlantis');
    await flushPromises();
    cityInput(wrapper).vm.$emit('update:modelValue', 'Gotham');
    await flushPromises();

    expect(await validate(wrapper)).toBe(false);
    expect(messages(wrapper)).toEqual(['Please select a province from the list', 'Please select a city from the list']);
  });

  it('asks to pick a city from the list when the typed city differs', async () => {
    const wrapper = await setup({ model: savedModel() });
    cityInput(wrapper).vm.$emit('update:modelValue', 'Toronto East');
    await flushPromises();

    expect(await validate(wrapper)).toBe(false);
    expect(messages(wrapper)).toEqual(['Please select a city from the list']);
  });

  it('follows a new model from the parent', async () => {
    const wrapper = await setup({ model: savedModel() });
    await wrapper.setProps({ model: { ...savedModel(), id: 'loc-2' } });
    await wrapper.find('input[name="address"]').setValue('300 Front St');
    await flushPromises();

    expect(lastModel(wrapper).id).toBe('loc-2');
  });

  describe('province settings', () => {
    it('is hidden for non admins or when disabled', async () => {
      securityStore.userRoles = ['recruiting'];
      expect((await setup({ model: savedModel(), enableProvinceSettings: true })).find('.province-configure-link').exists()).toBe(false);

      securityStore.userRoles = ['admin'];
      expect((await setup({ model: savedModel() })).find('.province-configure-link').exists()).toBe(false);
    });

    it('lets an admin configure the province and emits the saved settings', async () => {
      securityStore.userRoles = ['admin'];
      const wrapper = await setup({ model: savedModel(), enableProvinceSettings: true });
      const link = wrapper.find('.province-configure-link');
      expect(link.text()).toBe('Configure');

      await link.trigger('click');
      const modal = wrapper.findComponent(ProvinceSettingsStub);
      expect(modal.props()).toEqual({ provinceId: 'on', provinceName: 'Ontario', currentSettings: null });

      modal.vm.$emit('saved', { overtimeStartsAfter: 44 });
      await flushPromises();

      expect(lastModel(wrapper).city?.province?.settings).toEqual({ overtimeStartsAfter: 44 });
      expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
      expect(wrapper.find('.province-configure-link').text()).toBe('See Settings');
    });
  });

  describe('adding a city', () => {
    it('is offered only to agency staff', async () => {
      expect((await setup({ model: savedModel() })).html()).not.toContain('Add new...');

      securityStore.userRoles = ['recruiting'];
      const wrapper = await setup({ model: savedModel() });
      await wrapper.find('input[name="city"]').trigger('focus');
      await flushPromises();
      expect(wrapper.html()).toContain('Add new...');
    });

    it('creates the city in the selected province and closes the prompt', async () => {
      securityStore.userRoles = ['recruiting'];
      const prompt = vi.fn();
      vi.mocked(getDialog).mockReturnValue({ prompt } as unknown as ReturnType<typeof getDialog>);
      const created: City = { id: 'mk', value: 'Markham' };
      vi.mocked(createCity).mockResolvedValue(created);
      const wrapper = await setup({ model: savedModel() });
      cityInput(wrapper).vm.$emit('update:modelValue', 'Mark');
      cityInput(wrapper).vm.$emit('select-footer');
      await flushPromises();

      const options = prompt.mock.calls[0][0] as { inputAttrs: { value: string }; onConfirm: (value: string, dialog: { close: () => void }) => Promise<void> };
      expect(options.inputAttrs.value).toBe('Mark');

      const close = vi.fn();
      await options.onConfirm('Markham', { close });
      await flushPromises();

      expect(createCity).toHaveBeenCalledWith({ value: 'Markham', province: { id: 'on' } });
      expect(close).toHaveBeenCalledOnce();
      expect(inputValue(wrapper, 'city')).toBe('Markham');
    });
  });
});
