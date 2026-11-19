import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, reactive, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import CreateRequest from '@/modules/agency/recruiting/requests/pages/CreateRequest.vue';
import {
  approveAgencyRequest,
  duplicateAgencyRequest,
  getAgencyRequestLookup,
  postAgencyRequest,
  updateAgencyRequest,
} from '@/modules/agency/recruiting/requests/api';
import {
  createAgencyCompanyJobPosition,
  createAgencyCompanyJobPositionWithoutRates,
  createAgencyCompanyLocation,
  getAgencyCompanyJobPositionById,
  getAgencyCompanyJobPositionsDropdown,
  getAgencyCompanyLocation,
  updateAgencyCompanyJobPosition,
} from '@/modules/agency/recruiting/clients/api';
import { DurationTerm, EmploymentType, RequestStatus } from '@/shared/constants/enums';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestDetail, AgencyRequestLookup } from '@/modules/agency/recruiting/requests/types';
import type { AgencyCompanyJobPosition, AgencyCompanyLocationModel } from '@/modules/agency/recruiting/clients/types';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';
import type { CompanyUserModel } from '@/shared/company-profile/types';
import type { CreateAgencyRequestModel, RequestComplianceItem } from '@/shared/request-detail/types';

const { childStub } = vi.hoisted(() => ({
  childStub: (name: string, props: string[], emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

const admin = ref(true);
const recruiting = ref(true);
const route = reactive<{ path: string; query: Record<string, string>; params: Record<string, string>; meta: { isDuplicate?: boolean } }>({
  path: '/recruiting/requests/create/cp-1',
  query: {},
  params: { companyProfileId: 'cp-1' },
  meta: {},
});

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
}));
vi.mock('@/shared/composables/useAdmin', () => ({ useAdmin: () => ({ isAdmin: computed(() => admin.value) }) }));
vi.mock('@/shared/composables/useRecruitingAccess', () => ({ useRecruitingAccess: () => ({ hasRecruitingAccess: computed(() => recruiting.value) }) }));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({
  postAgencyRequest: vi.fn(),
  updateAgencyRequest: vi.fn(),
  duplicateAgencyRequest: vi.fn(),
  getAgencyRequestLookup: vi.fn(),
  approveAgencyRequest: vi.fn(),
}));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  createAgencyCompanyLocation: vi.fn(),
  getAgencyCompanyJobPositionsDropdown: vi.fn(),
  getAgencyCompanyLocation: vi.fn(),
  createAgencyCompanyJobPosition: vi.fn(),
  createAgencyCompanyJobPositionWithoutRates: vi.fn(),
  updateAgencyCompanyJobPosition: vi.fn(),
  getAgencyCompanyJobPositionById: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/company-profile/JobPositionForm.vue', () => ({
  default: childStub('JobPositionForm', ['kind', 'create', 'update', 'load', 'currentPosition', 'isDuplicate'], ['updateContent']),
}));
vi.mock('@/shared/company-profile/LocationForm.vue', () => ({ default: childStub('LocationForm', ['save'], ['updateContent']) }));
vi.mock('@/modules/agency/recruiting/applicants/components/RequestComplianceModal.vue', () => ({ default: childStub('RequestComplianceModal', ['items'], ['save']) }));

const QuillStub = {
  name: 'QuillEditor',
  props: ['content', 'theme', 'contentType'],
  emits: ['update:content'],
  template: '<textarea class="quill" :value="content" @input="$emit(\'update:content\', $event.target.value)" />',
};

const forklift: AgencyCompanyJobPosition = { id: 'jp-1', jobPosition: 'Forklift', rate: 30, workerRate: 20, shift: { monday: true } };
const packer: AgencyCompanyJobPosition = { id: 'jp-2', jobPosition: 'Packer', rate: 0, workerRate: 0 };
const king: AgencyCompanyLocationModel = { id: 'loc-1', address: '100 King St', formattedAddress: '100 King St, Toronto' };
const queen: AgencyCompanyLocationModel = { id: 'loc-2', address: '5 Queen St', formattedAddress: '5 Queen St, Toronto' };
const rep: AgencyPersonnelListItem = { id: 'p-1', userId: 'u-1', name: 'Sam', email: 'sam@agency.com', role: 'Sales', agencyIds: [], agencies: [] };
const companyUser: CompanyUserModel = { id: 'cu-1', companyId: 'c-1', createdAt: '2026-01-01', email: 'boss@acme.com', name: 'Bob', lastname: 'Boss', position: 'Manager', mobileNumber: '' };

function source(overrides: Partial<AgencyRequestDetail> = {}): AgencyRequestDetail {
  return {
    id: 'req-1',
    agencyId: 'ag-1',
    numberId: 1001,
    jobTitle: 'Forklift operator',
    billingTitle: 'Forklift operator',
    jobCosting: 'JC-1',
    companyProfileId: 'cp-1',
    description: 'Move pallets around',
    requirements: 'Valid forklift license',
    jobLocation: { address: '100 King St' },
    workersQuantity: 3,
    workersQuantityWorking: 1,
    jobPositionId: 'jp-1',
    holidayIsPaid: true,
    breakIsPaid: true,
    status: RequestStatus.Open,
    createdAt: '2026-11-10T12:00:00',
    startAt: '2026-11-25T05:00:00.000Z',
    finishAt: '2026-12-25T05:00:00.000Z',
    durationBreak: '00:30:00',
    incentive: null,
    incentiveDescription: '',
    agencyRate: 30,
    workerRate: 20,
    workerSalary: null,
    durationTerm: DurationTerm.ShortTerm,
    employmentType: EmploymentType.PartTime,
    isAsap: false,
    usesRunners: true,
    punchCardOptionEnabled: true,
    salesRepresentativeId: 'p-1',
    companyUserIds: ['cu-1'],
    complianceItems: [{ id: 'ci-1', name: 'WHMIS', isMandatory: true, documentTarget: 0 }],
    skills: [],
    requestedBy: [],
    reportTo: [],
    applicantsCount: 0,
    runnersCount: 0,
    workersCount: 0,
    ...overrides,
  };
}

function lookup(overrides: Partial<AgencyRequestLookup> = {}): AgencyRequestLookup {
  return {
    request: null,
    jobPositions: [forklift, packer],
    locations: [king, queen],
    personnel: [rep],
    companyUsers: [companyUser],
    companyPaidHolidays: true,
    ...overrides,
  };
}

async function setup(data: AgencyRequestLookup = lookup()) {
  vi.mocked(getAgencyRequestLookup).mockResolvedValue(data);
  const wrapper = mountWithPlugins(CreateRequest, { global: { stubs: { QuillEditor: QuillStub } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function field(wrapper: Wrapper, label: string) {
  const found = wrapper.findAllComponents({ name: 'BField' }).find((f) => f.props('label') === label);
  if (!found) throw new Error(`${label} field not found`);
  return found;
}

function hasField(wrapper: Wrapper, label: string) {
  return wrapper.findAllComponents({ name: 'BField' }).some((f) => f.props('label') === label);
}

function autocomplete(wrapper: Wrapper, label: string) {
  return field(wrapper, label).findComponent({ name: 'BAutocomplete' });
}

function switchInput(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('.settings-list .switch').find((s) => s.text().startsWith(text));
  if (!found) throw new Error(`${text} switch not found`);
  return found.find('input');
}

async function pick<T>(wrapper: Wrapper, label: string, text: string, option: T) {
  const input = autocomplete(wrapper, label);
  input.vm.$emit('update:modelValue', text);
  input.vm.$emit('select', option);
  await nextTick();
}

async function fill(wrapper: Wrapper) {
  await field(wrapper, 'Job title *').find('input').setValue('Forklift operator');
  await field(wrapper, 'Workers Quantity *').find('input').setValue('2');
  await pick(wrapper, 'Rol *', 'Forklift', forklift);
  await pick(wrapper, 'Branch office *', '100 King St, Toronto', king);
  const editors = wrapper.findAll('textarea.quill');
  await editors[0].setValue('Move pallets around');
  await editors[1].setValue('Help the team');
  await editors[2].setValue('Valid forklift license');
  field(wrapper, 'Start *').findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date(2026, 10, 25));
  await nextTick();
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
  await flushPromises();
}

function message(wrapper: Wrapper, label: string) {
  return field(wrapper, label).props('message') as string;
}

function lastPayload(mock: typeof postAgencyRequest | typeof updateAgencyRequest | typeof approveAgencyRequest) {
  const call = vi.mocked(mock).mock.lastCall;
  if (!call) throw new Error('not called');
  return call[call.length - 1] as CreateAgencyRequestModel;
}

beforeEach(() => {
  admin.value = true;
  recruiting.value = true;
  route.params = { companyProfileId: 'cp-1' };
  route.meta = {};
  vi.useFakeTimers({ toFake: ['Date'] });
  vi.setSystemTime(new Date(2026, 10, 19, 12));
});

afterEach(() => {
  vi.useRealTimers();
});

describe('CreateRequest', () => {
  it('loads the company lookup for a new request', async () => {
    const wrapper = await setup();

    expect(getAgencyRequestLookup).toHaveBeenCalledExactlyOnceWith('cp-1', undefined);
    expect(wrapper.find('.page-header-title').text()).toBe('Create Request');
    expect(wrapper.find('.settings-actions button').text()).toBe('Create');
    expect(hasField(wrapper, 'Job Costing')).toBe(true);
    expect(hasField(wrapper, 'Sales Representative')).toBe(true);
    expect(hasField(wrapper, 'Company User')).toBe(true);
    expect(hasField(wrapper, 'Finish')).toBe(false);
  });

  it('hides admin fields for non admins', async () => {
    admin.value = false;
    const wrapper = await setup(lookup({ companyUsers: [] }));

    expect(hasField(wrapper, 'Job Costing')).toBe(false);
    expect(hasField(wrapper, 'Sales Representative')).toBe(false);
    expect(hasField(wrapper, 'Company User')).toBe(false);
  });

  it('shows the error when the lookup fails', async () => {
    const error = new Error('boom');
    vi.mocked(getAgencyRequestLookup).mockRejectedValue(error);
    mountWithPlugins(CreateRequest, { global: { stubs: { QuillEditor: QuillStub } } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('validates the required fields', async () => {
    const wrapper = await setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(message(wrapper, 'Job title *')).toBe('Job title is required'); });
    expect(message(wrapper, 'Rol *')).toBe('Rol is required');
    expect(message(wrapper, 'Branch office *')).toBe('Branch office is required');
    expect(message(wrapper, 'Description *')).toBe('Description is required');
    expect(message(wrapper, 'Requirements *')).toBe('Requirements are required');
    expect(message(wrapper, 'Start *')).toBe('Start date is required');
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(postAgencyRequest).not.toHaveBeenCalled();
  });

  it('copies the job title into the billing title until unlinked', async () => {
    const wrapper = await setup();
    await field(wrapper, 'Job title *').find('input').setValue('Packer');
    await nextTick();
    const billing = field(wrapper, 'Billing Title *').find('input');
    expect((billing.element as HTMLInputElement).value).toBe('Packer');
    expect(billing.attributes('disabled')).toBeDefined();

    await switchInput(wrapper, 'Job title same for billing title').setValue(false);
    await nextTick();
    expect((billing.element as HTMLInputElement).value).toBe('');
    expect(billing.attributes('disabled')).toBeUndefined();

    await switchInput(wrapper, 'Job title same for billing title').setValue(true);
    await nextTick();
    expect((billing.element as HTMLInputElement).value).toBe('Packer');
  });

  it('filters the roles, locations and sales reps as the user types', async () => {
    const wrapper = await setup();
    autocomplete(wrapper, 'Rol *').vm.$emit('update:modelValue', 'pack');
    autocomplete(wrapper, 'Branch office *').vm.$emit('update:modelValue', 'queen');
    autocomplete(wrapper, 'Sales Representative').vm.$emit('update:modelValue', 'nobody');
    await nextTick();

    expect(autocomplete(wrapper, 'Rol *').props('data')).toEqual([packer]);
    expect(autocomplete(wrapper, 'Branch office *').props('data')).toEqual([queen]);
    expect(autocomplete(wrapper, 'Sales Representative').props('data')).toEqual([]);
    const formatter = autocomplete(wrapper, 'Sales Representative').props('customFormatter') as (o: AgencyPersonnelListItem) => string;
    expect(formatter(rep)).toBe('Sam - sam@agency.com');
  });

  it('shows the rate of the selected role', async () => {
    const wrapper = await setup();
    await pick(wrapper, 'Rol *', 'Forklift', forklift);
    expect(wrapper.find('.role-rate').text()).toContain('Rate for this position: 20');

    await pick(wrapper, 'Rol *', 'Packer', packer);
    expect(wrapper.find('.role-rate').text()).toContain('Rates pending');

    autocomplete(wrapper, 'Rol *').vm.$emit('select', null);
    await nextTick();
    expect(wrapper.find('.role-rate').exists()).toBe(false);
  });

  it('creates the request with the form values', async () => {
    vi.mocked(postAgencyRequest).mockResolvedValue(source({ id: 'req-9' }));
    const wrapper = await setup();
    await fill(wrapper);
    await field(wrapper, 'Job Costing').find('input').setValue('JC-7');
    await pick(wrapper, 'Sales Representative', 'Sam - sam@agency.com', rep);
    field(wrapper, 'Company User').findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [companyUser]);
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(postAgencyRequest).toHaveBeenCalledOnce(); });
    expect(lastPayload(postAgencyRequest)).toEqual(expect.objectContaining({
      companyProfileId: 'cp-1',
      jobTitle: 'Forklift operator',
      billingTitle: 'Forklift operator',
      jobCosting: 'JC-7',
      workersQuantity: 2,
      workerSalary: null,
      description: 'Move pallets around',
      requirements: 'Valid forklift license',
      responsibilities: 'Help the team',
      incentive: null,
      startAt: new Date(2026, 10, 25).toISOString(),
      finishAt: null,
      durationBreak: '00:00',
      jobPositionRateId: 'jp-1',
      rate: 20,
      shift: { monday: true },
      locationId: 'loc-1',
      salesRepresentativeId: 'p-1',
      companyUserIds: ['cu-1'],
      usesRunners: true,
      holidayIsPaid: true,
    }));
    expect(showAlertSuccess).toHaveBeenCalledWith('Request created');
    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/recruiting/requests/req-9');
  });

  it('rejects a typed role or location that was not selected', async () => {
    const wrapper = await setup();
    await fill(wrapper);
    autocomplete(wrapper, 'Rol *').vm.$emit('update:modelValue', 'Forklift X');
    autocomplete(wrapper, 'Branch office *').vm.$emit('update:modelValue', 'Somewhere');
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(postAgencyRequest).not.toHaveBeenCalled();
  });

  it('clears the sales rep when its text changes', async () => {
    vi.mocked(postAgencyRequest).mockResolvedValue(source({ id: 'req-9' }));
    const wrapper = await setup();
    await fill(wrapper);
    await pick(wrapper, 'Sales Representative', 'Sam - sam@agency.com', rep);
    autocomplete(wrapper, 'Sales Representative').vm.$emit('update:modelValue', 'Sam');
    await nextTick();
    autocomplete(wrapper, 'Sales Representative').vm.$emit('select', null);
    await submit(wrapper);

    await vi.waitFor(() => { expect(postAgencyRequest).toHaveBeenCalledOnce(); });
    expect(lastPayload(postAgencyRequest).salesRepresentativeId).toBeNull();
  });

  it('validates the incentive decimals', async () => {
    const wrapper = await setup();
    await fill(wrapper);
    await field(wrapper, 'Incentive').find('input').setValue('1.234');
    await submit(wrapper);

    await vi.waitFor(() => { expect(message(wrapper, 'Incentive')).toBe('Max 2 decimals'); });
    expect(postAgencyRequest).not.toHaveBeenCalled();
  });

  it('requires the salary for direct hiring and sends it', async () => {
    vi.mocked(postAgencyRequest).mockResolvedValue(source({ id: 'req-9' }));
    const wrapper = await setup();
    await switchInput(wrapper, 'Direct Hiring').setValue(true);
    await nextTick();
    expect(hasField(wrapper, 'Rol *')).toBe(false);
    expect(wrapper.text()).not.toContain('Punch card is visible in app');
    await submit(wrapper);
    await vi.waitFor(() => { expect(message(wrapper, 'Worker Salary *')).toBe('Worker salary is required'); });

    await fill(wrapper).catch(() => undefined);
    await field(wrapper, 'Job title *').find('input').setValue('Manager');
    await field(wrapper, 'Worker Salary *').find('input').setValue('55000');
    await pick(wrapper, 'Branch office *', '100 King St, Toronto', king);
    const editors = wrapper.findAll('textarea.quill');
    await editors[0].setValue('Lead the warehouse team');
    await editors[2].setValue('Five years experience');
    field(wrapper, 'Start *').findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date(2026, 10, 25));
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(postAgencyRequest).toHaveBeenCalledOnce(); });
    expect(lastPayload(postAgencyRequest).workerSalary).toBe(55000);
  });

  it('resets the salary when direct hiring is turned off', async () => {
    const wrapper = await setup();
    await switchInput(wrapper, 'Direct Hiring').setValue(true);
    await nextTick();
    await field(wrapper, 'Worker Salary *').find('input').setValue('55000');
    await switchInput(wrapper, 'Direct Hiring').setValue(false);
    await nextTick();
    await switchInput(wrapper, 'Direct Hiring').setValue(true);
    await nextTick();

    expect((field(wrapper, 'Worker Salary *').find('input').element as HTMLInputElement).value).toBe('');
  });

  it('adds the default compliance items when runners are not used', async () => {
    const wrapper = await setup();
    expect(wrapper.find('.compliance-configure-link').exists()).toBe(false);
    await switchInput(wrapper, 'Uses Runners').setValue(false);
    await nextTick();

    expect(wrapper.find('.compliance-configure-link').text()).toBe('Configure (6)');
    await wrapper.find('.compliance-configure-link').trigger('click');
    await nextTick();
    const modal = wrapper.findComponent({ name: 'RequestComplianceModal' });
    expect((modal.props('items') as RequestComplianceItem[]).map((i) => i.name)).toEqual(['ID', 'SIN/SSN', 'WP', 'W4', 'I9', 'Banking']);
    modal.vm.$emit('save', [{ name: 'ID', isMandatory: true, documentTarget: 1 }]);
    await nextTick();
    expect(wrapper.find('.compliance-configure-link').text()).toBe('Configure (1)');
  });

  it('cannot pay holidays when the company does not', async () => {
    vi.mocked(postAgencyRequest).mockResolvedValue(source({ id: 'req-9' }));
    const wrapper = await setup(lookup({ companyPaidHolidays: false }));

    expect(wrapper.text()).toContain('Holiday is not paid (company does not pay holidays)');
    expect(switchInput(wrapper, 'Holiday is not paid').attributes('disabled')).toBeDefined();
    await fill(wrapper);
    await submit(wrapper);
    await vi.waitFor(() => { expect(postAgencyRequest).toHaveBeenCalledOnce(); });
    expect(lastPayload(postAgencyRequest).holidayIsPaid).toBe(false);
  });

  it('shows a finish date for short term requests', async () => {
    const wrapper = await setup();
    await field(wrapper, 'Duration Term').find('select').setValue(String(DurationTerm.ShortTerm));
    await nextTick();

    expect(hasField(wrapper, 'Finish')).toBe(true);
  });

  it('shows the error when creating fails', async () => {
    const error = new Error('boom');
    vi.mocked(postAgencyRequest).mockRejectedValue(error);
    const wrapper = await setup();
    await fill(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/');
  });

  it('loads an existing request for update', async () => {
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    const wrapper = await setup(lookup({ request: source() }));

    expect(getAgencyRequestLookup).toHaveBeenCalledWith('cp-1', 'req-1');
    expect(wrapper.find('.page-header-title').text()).toBe('Update Request (1001)');
    expect(wrapper.find('.settings-actions button').text()).toBe('Update');
    expect((field(wrapper, 'Job title *').find('input').element as HTMLInputElement).value).toBe('Forklift operator');
    expect(autocomplete(wrapper, 'Rol *').props('modelValue')).toBe('Forklift');
    expect(autocomplete(wrapper, 'Branch office *').props('modelValue')).toBe('100 King St, Toronto');
    expect(autocomplete(wrapper, 'Sales Representative').props('modelValue')).toBe('Sam - sam@agency.com');
    expect(field(wrapper, 'Company User').findComponent({ name: 'BTaginput' }).props('modelValue')).toEqual([companyUser]);
    expect(hasField(wrapper, 'Finish')).toBe(true);
  });

  it('updates the request', async () => {
    vi.mocked(updateAgencyRequest).mockResolvedValue(source());
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    const wrapper = await setup(lookup({ request: source() }));
    await submit(wrapper);

    await vi.waitFor(() => { expect(updateAgencyRequest).toHaveBeenCalledOnce(); });
    expect(vi.mocked(updateAgencyRequest).mock.lastCall?.[0]).toBe('req-1');
    expect(lastPayload(updateAgencyRequest)).toEqual(expect.objectContaining({
      id: 'req-1',
      jobTitle: 'Forklift operator',
      jobCosting: 'JC-1',
      workersQuantity: 3,
      startAt: '2026-11-25T05:00:00.000Z',
      finishAt: '2026-12-25T05:00:00.000Z',
      durationBreak: '00:30',
      locationId: 'loc-1',
      jobPositionRateId: 'jp-1',
    }));
    expect(showAlertSuccess).toHaveBeenCalledWith('Request updated');
    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/recruiting/requests/req-1');
  });

  it('keeps the stored job costing for non admins', async () => {
    admin.value = false;
    vi.mocked(updateAgencyRequest).mockResolvedValue(source());
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    const wrapper = await setup(lookup({ request: source({ breakIsPaid: false }) }));
    await submit(wrapper);

    await vi.waitFor(() => { expect(updateAgencyRequest).toHaveBeenCalledOnce(); });
    expect(lastPayload(updateAgencyRequest)).toEqual(expect.objectContaining({ jobCosting: 'JC-1', durationBreak: '00:00' }));
  });

  it('shows the error when updating fails', async () => {
    const error = new Error('boom');
    vi.mocked(updateAgencyRequest).mockRejectedValue(error);
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    const wrapper = await setup(lookup({ request: source() }));
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
  });

  it('loads a direct hiring request with its own billing title', async () => {
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    const wrapper = await setup(lookup({ request: source({ workerSalary: 60000, billingTitle: 'Supervisor', jobPositionId: null, jobLocation: null, salesRepresentativeId: null }) }));

    expect((field(wrapper, 'Worker Salary *').find('input').element as HTMLInputElement).value).toBe('60000');
    expect(field(wrapper, 'Billing Title *').find('input').attributes('disabled')).toBeUndefined();
    expect((field(wrapper, 'Billing Title *').find('input').element as HTMLInputElement).value).toBe('Supervisor');
  });

  it('approves a pending review request', async () => {
    vi.mocked(approveAgencyRequest).mockResolvedValue();
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    const wrapper = await setup(lookup({ request: source({ status: RequestStatus.PendingReview }) }));

    expect(wrapper.find('.page-header-title').text()).toBe('Review Request (1001)');
    expect(wrapper.find('.settings-actions button').text()).toBe('Save & Approve');
    await submit(wrapper);

    await vi.waitFor(() => { expect(approveAgencyRequest).toHaveBeenCalledOnce(); });
    expect(vi.mocked(approveAgencyRequest).mock.lastCall?.[0]).toBe('req-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Request approved');
    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/recruiting/requests/req-1');
  });

  it('blocks approving a role without rates', async () => {
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    const wrapper = await setup(lookup({ request: source({ status: RequestStatus.PendingReview, jobPositionId: 'jp-2' }) }));
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('The role has no rates yet. An agency admin must set them before approving.'); });
    expect(approveAgencyRequest).not.toHaveBeenCalled();
  });

  it('shows the error when approving fails', async () => {
    const error = new Error('boom');
    vi.mocked(approveAgencyRequest).mockRejectedValue(error);
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    const wrapper = await setup(lookup({ request: source({ status: RequestStatus.PendingReview }) }));
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
  });

  it('only updates a pending request without recruiting access', async () => {
    recruiting.value = false;
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    const wrapper = await setup(lookup({ request: source({ status: RequestStatus.PendingReview }) }));

    expect(wrapper.find('.settings-actions button').text()).toBe('Update');
  });

  it('duplicates a request without its identity or dates', async () => {
    vi.mocked(duplicateAgencyRequest).mockResolvedValue(source({ id: 'req-2' }));
    route.params = { companyProfileId: 'cp-1', requestId: 'req-1' };
    route.meta = { isDuplicate: true };
    const wrapper = await setup(lookup({ request: source() }));

    expect(wrapper.find('.page-header-title').text()).toBe('Duplicate Request (from 1001)');
    expect(wrapper.find('.settings-actions button').text()).toBe('Create');
    await submit(wrapper);
    await vi.waitFor(() => { expect(message(wrapper, 'Start *')).toBe('Start date is required'); });

    field(wrapper, 'Start *').findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date(2026, 11, 1));
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(duplicateAgencyRequest).toHaveBeenCalledOnce(); });
    expect(vi.mocked(duplicateAgencyRequest).mock.lastCall?.[0]).toBe('req-1');
    const payload = vi.mocked(duplicateAgencyRequest).mock.lastCall?.[1];
    expect(payload).toEqual(expect.objectContaining({
      id: undefined,
      numberId: undefined,
      status: undefined,
      finishAt: null,
      startAt: new Date(2026, 11, 1).toISOString(),
      complianceItems: [{ name: 'WHMIS', isMandatory: true, documentTarget: 0 }],
    }));
    expect(showAlertSuccess).toHaveBeenCalledWith('Request duplicated');
    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/recruiting/requests/req-2');
  });

  it('adds a new role from the role picker and selects it', async () => {
    vi.mocked(getAgencyCompanyJobPositionsDropdown).mockResolvedValue([forklift, packer, { id: 'jp-3', jobPosition: 'Welder', rate: 40, workerRate: 28 }]);
    const wrapper = await setup();
    autocomplete(wrapper, 'Rol *').vm.$emit('select-footer');
    await nextTick();
    const form = wrapper.findComponent({ name: 'JobPositionForm' });
    expect(form.props()).toEqual(expect.objectContaining({ kind: 'full', currentPosition: null, isDuplicate: false }));
    form.vm.$emit('updateContent', 'jp-3');
    await flushPromises();

    expect(getAgencyCompanyJobPositionsDropdown).toHaveBeenCalledWith('cp-1');
    expect(autocomplete(wrapper, 'Rol *').props('modelValue')).toBe('Welder');
    expect(wrapper.find('.role-rate').text()).toContain('Rate for this position: 28');
  });

  it('ignores an unknown role after saving', async () => {
    vi.mocked(getAgencyCompanyJobPositionsDropdown).mockResolvedValue([forklift]);
    const wrapper = await setup();
    autocomplete(wrapper, 'Rol *').vm.$emit('select-footer');
    await nextTick();
    wrapper.findComponent({ name: 'JobPositionForm' }).vm.$emit('updateContent', undefined);
    await flushPromises();

    expect(autocomplete(wrapper, 'Rol *').props('modelValue')).toBe('');
  });

  it('edits and duplicates the selected role', async () => {
    vi.mocked(updateAgencyCompanyJobPosition).mockResolvedValue();
    vi.mocked(getAgencyCompanyJobPositionById).mockResolvedValue(forklift);
    vi.mocked(createAgencyCompanyJobPosition).mockResolvedValue({ id: 'jp-9' });
    const wrapper = await setup();
    await pick(wrapper, 'Rol *', 'Forklift', forklift);
    await wrapper.find('button[aria-label="Edit role rates"]').trigger('click');
    await nextTick();
    let form = wrapper.findComponent({ name: 'JobPositionForm' });
    expect(form.props('currentPosition')).toEqual(forklift);
    await (form.props('update') as (id: string, m: AgencyCompanyJobPosition) => Promise<void>)('jp-1', forklift);
    await (form.props('load') as (id: string) => Promise<AgencyCompanyJobPosition>)('jp-1');
    expect(updateAgencyCompanyJobPosition).toHaveBeenCalledWith('cp-1', 'jp-1', forklift);
    expect(getAgencyCompanyJobPositionById).toHaveBeenCalledWith('cp-1', 'jp-1');

    wrapper.findAllComponents({ name: 'BModal' })[0].vm.$emit('close');
    await nextTick();
    await wrapper.find('button[aria-label="Duplicate role"]').trigger('click');
    await nextTick();
    form = wrapper.findComponent({ name: 'JobPositionForm' });
    expect(form.props('isDuplicate')).toBe(true);
    await (form.props('create') as (m: AgencyCompanyJobPosition) => Promise<{ id: string }>)(forklift);
    expect(createAgencyCompanyJobPosition).toHaveBeenCalledWith('cp-1', forklift);
  });

  it('creates roles without rates for non admins', async () => {
    admin.value = false;
    vi.mocked(createAgencyCompanyJobPositionWithoutRates).mockResolvedValue({ id: 'jp-9' });
    const wrapper = await setup();
    await pick(wrapper, 'Rol *', 'Forklift', forklift);
    expect(wrapper.find('button[aria-label="Edit role rates"]').exists()).toBe(false);
    autocomplete(wrapper, 'Rol *').vm.$emit('select-footer');
    await nextTick();
    const form = wrapper.findComponent({ name: 'JobPositionForm' });
    expect(form.props('kind')).toBe('basic');
    await (form.props('create') as (m: AgencyCompanyJobPosition) => Promise<{ id: string }>)(packer);

    expect(createAgencyCompanyJobPositionWithoutRates).toHaveBeenCalledWith('cp-1', packer);
  });

  it('adds a new location and reloads the list', async () => {
    vi.mocked(createAgencyCompanyLocation).mockResolvedValue({ id: 'loc-3' });
    vi.mocked(getAgencyCompanyLocation).mockResolvedValue([king]);
    const wrapper = await setup();
    autocomplete(wrapper, 'Branch office *').vm.$emit('select-footer');
    await nextTick();
    const form = wrapper.findComponent({ name: 'LocationForm' });
    await (form.props('save') as (l: AgencyCompanyLocationModel) => Promise<unknown>)(queen);
    expect(createAgencyCompanyLocation).toHaveBeenCalledWith('cp-1', queen);

    form.vm.$emit('updateContent');
    await flushPromises();
    expect(getAgencyCompanyLocation).toHaveBeenCalledWith('cp-1');
    expect(autocomplete(wrapper, 'Branch office *').props('data')).toEqual([king]);
  });
});
