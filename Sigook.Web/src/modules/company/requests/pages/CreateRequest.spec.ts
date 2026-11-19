import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import CreateRequest from '@/modules/company/requests/pages/CreateRequest.vue';
import { createCompanyJobPosition, createProfileLocation, getCompanyJobPositions, getCompanyUser, getProfileLocations } from '@/modules/company/profile/api';
import { createRequest } from '@/modules/company/requests/api';
import { DurationTerm, EmploymentType } from '@/shared/constants/enums';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyJobPosition, CompanyProfileLocationDetail, CompanyUserModel } from '@/shared/company-profile/types';
import type { RequestShiftModel } from '@/shared/request-detail/types';

const { push, securityStore } = vi.hoisted(() => ({
  push: vi.fn(),
  securityStore: { userRoles: ['company'] as string[] },
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRouter: () => ({ push }),
}));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/modules/company/profile/api', () => ({
  createProfileLocation: vi.fn(),
  getProfileLocations: vi.fn(),
  getCompanyJobPositions: vi.fn(),
  getCompanyUser: vi.fn(),
  createCompanyJobPosition: vi.fn(),
}));
vi.mock('@/modules/company/requests/api', () => ({ createRequest: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/ui/PageHeader.vue', () => ({
  default: { name: 'PageHeader', props: { title: String, crumbs: Array, backTo: String }, template: '<h2>{{ title }}</h2>' },
}));
vi.mock('@/shared/company-profile/JobPositionForm.vue', () => ({
  default: { name: 'JobPositionForm', props: { kind: String, create: Function }, emits: ['updateContent'], template: '<div class="job-position-form" />' },
}));
vi.mock('@/shared/company-profile/LocationForm.vue', () => ({
  default: { name: 'LocationForm', props: { save: Function }, emits: ['updateContent'], template: '<div class="location-form" />' },
}));

const QuillEditor = {
  name: 'QuillEditor',
  props: { content: String },
  emits: ['update:content'],
  template: '<textarea class="quill" :value="content" @input="$emit(\'update:content\', $event.target.value)" />',
};

const shift: RequestShiftModel = { monday: true, mondayStart: '08:00:00', mondayFinish: '16:00:00' };
const forklift: CompanyJobPosition = { id: 'jp-1', jobPosition: 'Forklift', shift };
const packer: CompanyJobPosition = { id: 'jp-2', jobPosition: 'Packer', shift: null };
const king: CompanyProfileLocationDetail = { id: 'loc-1', address: '1 King St', postalCode: 'M5H 1A1', isBilling: true, latitude: null, longitude: null, entrance: '', mainIntersection: '', formattedAddress: '1 King St, Toronto' };
const queen: CompanyProfileLocationDetail = { id: 'loc-2', address: '9 Queen St', postalCode: 'L5B 2C3', isBilling: false, latitude: null, longitude: null, entrance: '', mainIntersection: '', formattedAddress: '9 Queen St, Mississauga' };
const ann: CompanyUserModel = {
  id: 'user-1', companyId: 'company-1', createdAt: '2026-01-01T00:00:00', email: 'ann@acme.com',
  name: 'Ann', lastname: 'Lee', position: 'Manager', mobileNumber: '',
};

async function setup() {
  vi.mocked(getProfileLocations).mockResolvedValue([king, queen]);
  vi.mocked(getCompanyJobPositions).mockResolvedValue([forklift, packer]);
  vi.mocked(getCompanyUser).mockResolvedValue([ann]);
  const wrapper = mountWithPlugins(CreateRequest, { global: { components: { QuillEditor } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function autocomplete(wrapper: Wrapper, field: string) {
  const found = wrapper.findAllComponents({ name: 'BAutocomplete' }).find((a) => a.props('field') === field);
  if (!found) throw new Error(`${field} autocomplete not found`);
  return found;
}

async function chooseRole(wrapper: Wrapper, role: CompanyJobPosition) {
  await wrapper.find('input[name="rol"]').setValue(role.jobPosition);
  autocomplete(wrapper, 'jobPosition').vm.$emit('select', role);
}

async function chooseLocation(wrapper: Wrapper, location: CompanyProfileLocationDetail) {
  await wrapper.find('input[name="branchOffice"]').setValue(location.formattedAddress);
  autocomplete(wrapper, 'formattedAddress').vm.$emit('select', location);
}

async function fillRequired(wrapper: Wrapper) {
  await wrapper.find('input[name="job title"]').setValue('Forklift operator');
  const quills = wrapper.findAll('textarea.quill');
  await quills[0].setValue('<p>Warehouse work</p>');
  await quills[2].setValue('<p>Steel-toe boots</p>');
  wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date(2026, 10, 23));
  await nextTick();
}

async function submit(wrapper: Wrapper) {
  await new Promise((resolve) => { setTimeout(resolve, 50); });
  await wrapper.find('form').trigger('submit');
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

beforeEach(() => {
  securityStore.userRoles = ['company'];
  push.mockResolvedValue(undefined);
  vi.useFakeTimers({ toFake: ['Date'] });
  vi.setSystemTime(new Date(2026, 10, 19, 12));
});

afterEach(() => {
  vi.useRealTimers();
});

describe('CreateRequest', () => {
  it('loads roles, locations and users for a company admin', async () => {
    const wrapper = await setup();

    expect(autocomplete(wrapper, 'jobPosition').props('data')).toEqual([forklift, packer]);
    expect(autocomplete(wrapper, 'formattedAddress').props('data')).toEqual([king, queen]);
    expect(wrapper.findComponent({ name: 'BTaginput' }).props('data')).toEqual([ann]);
  });

  it('does not load users for a company user', async () => {
    securityStore.userRoles = ['company.user'];
    const wrapper = await setup();

    expect(getCompanyUser).not.toHaveBeenCalled();
    expect(wrapper.findComponent({ name: 'BTaginput' }).exists()).toBe(false);
  });

  it('filters roles and locations by the typed text', async () => {
    const wrapper = await setup();
    await wrapper.find('input[name="rol"]').setValue('PACK');
    await wrapper.find('input[name="branchOffice"]').setValue('queen');

    expect(autocomplete(wrapper, 'jobPosition').props('data')).toEqual([packer]);
    expect(autocomplete(wrapper, 'formattedAddress').props('data')).toEqual([queen]);
  });

  it('requires the mandatory fields', async () => {
    const wrapper = await setup();
    await submit(wrapper);

    await vi.waitFor(() => {
      expect(errors(wrapper)).toEqual(expect.arrayContaining([
        'Job title is required', 'Rol is required', 'Branch office is required', 'Description is required',
        'Requirements are required', 'Start date is required',
      ]));
    });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createRequest).not.toHaveBeenCalled();
  });

  it('creates the request and opens it', async () => {
    vi.mocked(createRequest).mockResolvedValue({ id: 'request-9' });
    const wrapper = await setup();
    await fillRequired(wrapper);
    await chooseRole(wrapper, forklift);
    await chooseLocation(wrapper, king);
    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [ann]);
    await wrapper.findAll('textarea.quill')[1].setValue('<p>Drive</p>');
    await wrapper.find('input[name="incentive"]').setValue('2.5');
    await wrapper.find('input[name="incentiveDes"]').setValue('Night bonus');
    await wrapper.findAll('.settings-list input[type="checkbox"]')[1].setValue(true);
    await submit(wrapper);

    await vi.waitFor(() => { expect(push).toHaveBeenCalledWith('/company-requests/request-9'); });
    expect(createRequest).toHaveBeenCalledExactlyOnceWith({
      jobTitle: 'Forklift operator',
      workersQuantity: 1,
      workerSalary: null,
      description: '<p>Warehouse work</p>',
      requirements: '<p>Steel-toe boots</p>',
      responsibilities: '<p>Drive</p>',
      incentive: 2.5,
      incentiveDescription: 'Night bonus',
      startAt: new Date(2026, 10, 23).toISOString(),
      finishAt: undefined,
      durationBreak: '00:00',
      breakIsPaid: false,
      isAsap: true,
      durationTerm: DurationTerm.LongTerm,
      employmentType: EmploymentType.FullTime,
      punchCardOptionEnabled: true,
      shift,
      jobPositionRateId: 'jp-1',
      locationId: 'loc-1',
      companyUserIds: ['user-1'],
    });
    expect(showAlertSuccess).toHaveBeenCalledWith('Request created');
  });

  it('rejects a role or location typed but not picked', async () => {
    const wrapper = await setup();
    await fillRequired(wrapper);
    await chooseRole(wrapper, forklift);
    await wrapper.find('input[name="rol"]').setValue('Forklift senior');
    await wrapper.find('input[name="branchOffice"]').setValue('Somewhere');
    await submit(wrapper);

    await vi.waitFor(() => {
      expect(errors(wrapper)).toEqual(expect.arrayContaining(['Please select a role from the list', 'Please select a location from the list']));
    });
    expect(createRequest).not.toHaveBeenCalled();
  });

  it('asks for a salary instead of a role for direct hiring', async () => {
    vi.mocked(createRequest).mockResolvedValue({ id: 'request-9' });
    const wrapper = await setup();
    await wrapper.findAll('.settings-list input[type="checkbox"]')[0].setValue(true);

    expect(wrapper.find('input[name="rol"]').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'BTimepicker' }).exists()).toBe(false);
    await fillRequired(wrapper);
    await chooseLocation(wrapper, queen);
    await submit(wrapper);
    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Worker salary is required'); });

    await wrapper.find('input[name="workerSalary"]').setValue('65000');
    await submit(wrapper);

    await vi.waitFor(() => { expect(createRequest).toHaveBeenCalledOnce(); });
    expect(vi.mocked(createRequest).mock.calls[0][0]).toMatchObject({ workerSalary: 65000, locationId: 'loc-2', jobPositionRateId: undefined });
  });

  it('sends the paid break and the finish date of a short term', async () => {
    vi.mocked(createRequest).mockResolvedValue({ id: 'request-9' });
    const wrapper = await setup();
    await fillRequired(wrapper);
    await chooseRole(wrapper, packer);
    await chooseLocation(wrapper, king);
    await wrapper.findAll('.settings-list input[type="checkbox"]')[2].setValue(true);
    wrapper.findComponent({ name: 'BTimepicker' }).vm.$emit('update:modelValue', new Date(2026, 10, 19, 0, 30));
    await wrapper.findAll('select')[0].setValue(String(DurationTerm.ShortTerm));
    const pickers = wrapper.findAllComponents({ name: 'BDatepicker' });
    expect(pickers).toHaveLength(2);
    pickers[1].vm.$emit('update:modelValue', new Date(2026, 11, 20));
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(createRequest).toHaveBeenCalledOnce(); });
    expect(vi.mocked(createRequest).mock.calls[0][0]).toMatchObject({
      breakIsPaid: true, durationBreak: '00:30', durationTerm: DurationTerm.ShortTerm,
      finishAt: new Date(2026, 11, 20).toISOString(), shift: null, jobPositionRateId: 'jp-2',
    });
  });

  it('adds a role from the autocomplete and selects it', async () => {
    const wrapper = await setup();
    autocomplete(wrapper, 'jobPosition').vm.$emit('select-footer');
    await nextTick();
    const form = wrapper.findComponent({ name: 'JobPositionForm' });
    expect(form.props()).toEqual({ kind: 'basic', create: createCompanyJobPosition });

    const welder: CompanyJobPosition = { id: 'jp-3', jobPosition: 'Welder', shift: null };
    vi.mocked(getCompanyJobPositions).mockResolvedValue([forklift, packer, welder]);
    form.vm.$emit('updateContent', 'jp-3');
    await flushPromises();

    expect((wrapper.find('input[name="rol"]').element as HTMLInputElement).value).toBe('Welder');
  });

  it('ignores an added role that is not found', async () => {
    const wrapper = await setup();
    autocomplete(wrapper, 'jobPosition').vm.$emit('select-footer');
    await nextTick();
    wrapper.findComponent({ name: 'JobPositionForm' }).vm.$emit('updateContent', 'jp-404');
    await flushPromises();

    expect((wrapper.find('input[name="rol"]').element as HTMLInputElement).value).toBe('');
  });

  it('adds a location from the autocomplete and reloads the list', async () => {
    const wrapper = await setup();
    autocomplete(wrapper, 'formattedAddress').vm.$emit('select-footer');
    await nextTick();
    const form = wrapper.findComponent({ name: 'LocationForm' });
    expect(form.props('save')).toBe(createProfileLocation);

    vi.mocked(getProfileLocations).mockResolvedValue([king]);
    form.vm.$emit('updateContent');
    await flushPromises();

    expect(autocomplete(wrapper, 'formattedAddress').props('data')).toEqual([king]);
  });

  it('clears the selection when the picked role or location is cleared', async () => {
    const wrapper = await setup();
    await chooseRole(wrapper, forklift);
    await chooseLocation(wrapper, king);
    autocomplete(wrapper, 'jobPosition').vm.$emit('select', null);
    autocomplete(wrapper, 'formattedAddress').vm.$emit('select', null);
    await fillRequired(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => {
      expect(errors(wrapper)).toEqual(expect.arrayContaining(['Please select a role from the list', 'Please select a location from the list']));
    });
  });

  it('shows the error and hides the loader when the initial data cannot load', async () => {
    const error = new Error('load failed');
    vi.mocked(getProfileLocations).mockRejectedValue(error);
    vi.mocked(getCompanyJobPositions).mockResolvedValue([forklift, packer]);
    vi.mocked(getCompanyUser).mockResolvedValue([ann]);
    const wrapper = mountWithPlugins(CreateRequest, { global: { components: { QuillEditor } } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('shows the error when creating fails', async () => {
    const error = new Error('create failed');
    vi.mocked(createRequest).mockRejectedValue(error);
    const wrapper = await setup();
    await fillRequired(wrapper);
    await chooseRole(wrapper, forklift);
    await chooseLocation(wrapper, king);
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error); });
    expect(push).not.toHaveBeenCalled();
  });
});
