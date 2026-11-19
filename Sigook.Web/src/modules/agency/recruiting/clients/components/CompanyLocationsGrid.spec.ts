import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { reactive } from 'vue';
import CompanyLocationsGrid from '@/modules/agency/recruiting/clients/components/CompanyLocationsGrid.vue';
import { createAgencyCompanyLocation, deleteAgencyCompanyLocation, getAgencyCompanyLocation, updateAgencyCompanyLocation } from '@/modules/agency/recruiting/clients/api';
import { getLocationTax, upsertLocationTax } from '@/shared/api/locationApi';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyCompanyLocationModel } from '@/modules/agency/recruiting/clients/types';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  getAgencyCompanyLocation: vi.fn(),
  createAgencyCompanyLocation: vi.fn(),
  updateAgencyCompanyLocation: vi.fn(),
  deleteAgencyCompanyLocation: vi.fn(),
}));
vi.mock('@/shared/api/locationApi', () => ({ getLocationTax: vi.fn(), upsertLocationTax: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => ({ getDialog: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/company-profile/LocationForm.vue', () => ({
  default: {
    name: 'LocationForm',
    props: { currentLocation: Object, save: Function, enableProvinceSettings: Boolean },
    emits: ['updateContent'],
    template: '<div class="location-form-stub" />',
  },
}));

interface PromptConfig {
  message?: string;
  inputAttrs?: Record<string, unknown>;
  onConfirm?: (value: string, dialog: { close: () => void }) => void | Promise<void>;
}

type Save = (location: AgencyCompanyLocationModel, id?: string) => Promise<unknown>;

const ontario = { id: 'on', value: 'Ontario', code: 'ON' };
const warehouse: AgencyCompanyLocationModel = {
  id: 'l1',
  address: '100 King St',
  city: { id: 'to', value: 'Toronto', province: ontario },
  postalCode: 'M5H 1A1',
  isBilling: true,
  latitude: 43.6,
  longitude: -79.3,
};
const office: AgencyCompanyLocationModel = { id: 'l2', address: '5 Bay St', city: { id: 'ot', value: 'Ottawa', province: ontario }, postalCode: 'K1A 0B1' };

const prompt = vi.fn<(config: PromptConfig) => void>();

async function setup(locations: AgencyCompanyLocationModel[] = [warehouse, office]) {
  vi.mocked(getAgencyCompanyLocation).mockResolvedValue(locations);
  const wrapper = mountWithPlugins(CompanyLocationsGrid, { props: { profileId: 'company-1' } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').map((row) => row.findAll('td').slice(0, 4).map((cell) => cell.text().replace(/\s+/g, ' ')));
}

async function openActions(wrapper: Wrapper, index: number) {
  await wrapper.findAll('tbody tr')[index].find('td:last-child .dropdown').trigger('click');
  await flushPromises();
}

function menuItems() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).map((i) => i.textContent.trim());
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

function lastPrompt() {
  const config = prompt.mock.calls.at(-1)?.[0];
  if (!config?.onConfirm) throw new Error('prompt not opened');
  return { ...config, onConfirm: config.onConfirm };
}

beforeEach(() => {
  securityStore.userRoles = ['recruiting'];
  vi.mocked(getDialog).mockReturnValue({ alert: vi.fn(), confirm: vi.fn(), prompt });
});

describe('CompanyLocationsGrid', () => {
  it('lists the locations and reports the count', async () => {
    const wrapper = await setup();

    expect(getAgencyCompanyLocation).toHaveBeenCalledExactlyOnceWith('company-1');
    expect(rows(wrapper)).toEqual([
      ['100 King St Billing', 'Toronto', 'ON', 'M5H 1A1'],
      ['5 Bay St', 'Ottawa', 'ON', 'K1A 0B1'],
    ]);
    expect(wrapper.emitted('changed')).toEqual([[2]]);
  });

  it('filters the locations by address, city or postal code', async () => {
    const wrapper = await setup();
    await wrapper.find('thead input').setValue('ottawa');
    await flushPromises();

    expect(rows(wrapper).map((r) => r[0])).toEqual(['5 Bay St']);
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('load failed');
    vi.mocked(getAgencyCompanyLocation).mockRejectedValue(error);
    mountWithPlugins(CompanyLocationsGrid, { props: { profileId: 'company-1' } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('offers tax configuration only to admins', async () => {
    const wrapper = await setup();
    await openActions(wrapper, 0);
    expect(menuItems()).toEqual(['Edit', 'Open in maps', 'Delete']);

    securityStore.userRoles = ['admin'];
    const adminWrapper = await setup();
    await openActions(adminWrapper, 0);
    expect(menuItems()).toContain('Configure tax');
  });

  it('creates and updates locations through the company endpoints', async () => {
    vi.mocked(createAgencyCompanyLocation).mockResolvedValue({ id: 'l3' });
    vi.mocked(updateAgencyCompanyLocation).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add location')?.trigger('click');
    const form = wrapper.findComponent({ name: 'LocationForm' });
    expect(form.props('currentLocation')).toBeNull();
    expect(form.props('enableProvinceSettings')).toBe(true);

    const save = form.props('save') as Save;
    await save(office);
    await save(warehouse, 'l1');

    expect(createAgencyCompanyLocation).toHaveBeenCalledExactlyOnceWith('company-1', office);
    expect(updateAgencyCompanyLocation).toHaveBeenCalledExactlyOnceWith('company-1', 'l1', warehouse);
  });

  it('edits a location and reloads once saved', async () => {
    const wrapper = await setup();
    await openActions(wrapper, 1);
    menuItem('Edit').click();
    await flushPromises();

    const form = wrapper.findComponent({ name: 'LocationForm' });
    expect(form.props('currentLocation')).toEqual(office);

    form.vm.$emit('updateContent');
    await flushPromises();

    expect(getAgencyCompanyLocation).toHaveBeenCalledTimes(2);
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });

  it('opens the location in maps when it has coordinates', async () => {
    const open = vi.fn();
    vi.stubGlobal('open', open);
    const wrapper = await setup();
    await openActions(wrapper, 0);
    menuItem('Open in maps').click();

    expect(open).toHaveBeenCalledWith('https://www.google.com/maps/search/?api=1&query=43.6,-79.3', '_blank', 'noopener');

    await openActions(wrapper, 1);
    const items = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).filter((i) => i.textContent.trim() === 'Open in maps');
    expect(items.map((i) => i.classList.contains('is-disabled'))).toEqual([false, true]);
  });

  it('deletes a location after confirming', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyCompanyLocation).mockResolvedValue();
    const wrapper = await setup();
    vi.mocked(getAgencyCompanyLocation).mockResolvedValue([office]);
    await openActions(wrapper, 0);
    menuItem('Delete').click();
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this location');
    expect(deleteAgencyCompanyLocation).toHaveBeenCalledExactlyOnceWith('company-1', 'l1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Deleted');
    expect(wrapper.emitted('changed')).toEqual([[2], [1]]);
  });

  it('keeps the location when deletion is cancelled and shows delete errors', async () => {
    const error = new Error('in use');
    vi.mocked(showAlertConfirm).mockResolvedValueOnce(false).mockResolvedValueOnce(true);
    vi.mocked(deleteAgencyCompanyLocation).mockRejectedValue(error);
    const wrapper = await setup();

    await openActions(wrapper, 0);
    menuItem('Delete').click();
    await flushPromises();
    expect(deleteAgencyCompanyLocation).not.toHaveBeenCalled();

    await openActions(wrapper, 0);
    menuItem('Delete').click();
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('configures the location tax', async () => {
    securityStore.userRoles = ['admin'];
    vi.mocked(getLocationTax).mockResolvedValue({ tax1: 13 });
    vi.mocked(upsertLocationTax).mockResolvedValue();
    const wrapper = await setup();
    await openActions(wrapper, 0);
    menuItem('Configure tax').click();
    await flushPromises();

    expect(getLocationTax).toHaveBeenCalledWith('l1');
    const config = lastPrompt();
    expect(config.message).toBe('Tax (%) for 100 King St, Toronto, ON, M5H 1A1');
    expect(config.inputAttrs).toMatchObject({ value: '13', min: '0', max: '100' });

    const dialog = { close: vi.fn() };
    await config.onConfirm('150', dialog);
    expect(showAlertError).toHaveBeenCalledWith('Tax must be a number between 0 and 100');
    expect(upsertLocationTax).not.toHaveBeenCalled();

    await config.onConfirm('13.5', dialog);
    expect(upsertLocationTax).toHaveBeenCalledExactlyOnceWith('l1', { tax1: 13.5 });
    expect(showAlertSuccess).toHaveBeenCalledWith('Saved');
    expect(dialog.close).toHaveBeenCalledTimes(1);
  });

  it('defaults a missing tax to zero and keeps the prompt open when saving fails', async () => {
    securityStore.userRoles = ['admin'];
    const error = new Error('save failed');
    vi.mocked(getLocationTax).mockResolvedValue(null);
    vi.mocked(upsertLocationTax).mockRejectedValue(error);
    const wrapper = await setup();
    await openActions(wrapper, 1);
    menuItem('Configure tax').click();
    await flushPromises();

    const config = lastPrompt();
    expect(config.inputAttrs).toMatchObject({ value: '0' });
    const dialog = { close: vi.fn() };
    await config.onConfirm('5', dialog);

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(dialog.close).not.toHaveBeenCalled();
  });

  it('does not open the prompt when the tax cannot be loaded', async () => {
    securityStore.userRoles = ['admin'];
    const error = new Error('load failed');
    vi.mocked(getLocationTax).mockRejectedValue(error);
    const wrapper = await setup();
    await openActions(wrapper, 0);
    menuItem('Configure tax').click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(prompt).not.toHaveBeenCalled();
  });
});
