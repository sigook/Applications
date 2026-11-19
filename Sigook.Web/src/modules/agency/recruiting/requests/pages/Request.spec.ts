import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, reactive, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import Request from '@/modules/agency/recruiting/requests/pages/Request.vue';
import {
  agencyRequestOpen,
  agencyRequestSendInvitation,
  cancelAgencyRequest,
  getAgencyRequest,
  increaseWorkersQuantityByOne,
  reduceWorkersQuantityByOne,
  updateAgencyRequestIsAsap,
} from '@/modules/agency/recruiting/requests/api';
import {
  createAgencyCompanyJobPosition,
  createAgencyCompanyJobPositionWithoutRates,
  getAgencyCompanyJobPositionById,
  updateAgencyCompanyJobPosition,
} from '@/modules/agency/recruiting/clients/api';
import { DurationTerm, EmploymentType, RequestStatus } from '@/shared/constants/enums';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestDetail } from '@/modules/agency/recruiting/requests/types';
import type { AgencyCompanyJobPosition } from '@/modules/agency/recruiting/clients/types';
import type { RequestFact, RequestKpi } from '@/shared/request-detail/types';

const { childStub } = vi.hoisted(() => ({
  childStub: (name: string, props: string[], emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

const admin = ref(true);
const recruiting = ref(true);
const route = reactive<{ path: string; query: Record<string, string>; params: Record<string, string> }>({ path: '/recruiting/requests/req-1', query: {}, params: { id: 'req-1' } });

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
}));
vi.mock('@/app/stores/app', () => ({ useAppStore: () => ({ getCurrentDate: () => Promise.resolve(new Date()) }) }));
vi.mock('@/shared/composables/useAdmin', () => ({ useAdmin: () => ({ isAdmin: computed(() => admin.value) }) }));
vi.mock('@/shared/composables/useRecruitingAccess', () => ({ useRecruitingAccess: () => ({ hasRecruitingAccess: computed(() => recruiting.value) }) }));
vi.mock('@/modules/agency/shared/useAgencyName', () => ({ useAgencyName: () => ({ agencyName: (id?: string | null) => (id === 'ag-1' ? 'Covenant' : null) }) }));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({
  getAgencyRequest: vi.fn(),
  cancelAgencyRequest: vi.fn(),
  agencyRequestOpen: vi.fn(),
  agencyRequestSendInvitation: vi.fn(),
  getAgencyRequestShift: vi.fn(),
  increaseWorkersQuantityByOne: vi.fn(),
  reduceWorkersQuantityByOne: vi.fn(),
  updateAgencyRequestIsAsap: vi.fn(),
}));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  createAgencyCompanyJobPosition: vi.fn(),
  createAgencyCompanyJobPositionWithoutRates: vi.fn(),
  updateAgencyCompanyJobPosition: vi.fn(),
  getAgencyCompanyJobPositionById: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/request-detail/RequestHeader.vue', () => ({
  default: {
    name: 'RequestHeader',
    props: ['numberId', 'title', 'subtitle', 'orgName', 'orgLink', 'logo', 'statusLabel', 'statusVariant', 'isAsap', 'isDirectHiring', 'chips', 'workersWorking', 'workersTotal', 'kpis'],
    template: `<div class="header-stub">
      <div class="slot-alert"><slot name="alert" /></div>
      <div class="slot-actions"><slot name="actions" /></div>
      <div class="slot-quantity"><slot name="quantity-actions" /></div>
      <div class="slot-rate"><slot name="kpi-rate-extra" /></div>
      <div class="slot-shift"><slot name="kpi-shift" /></div>
      <div class="slot-shift-extra"><slot name="kpi-shift-extra" /></div>
      <slot />
    </div>`,
  },
}));
vi.mock('@/shared/request-detail/RequestDetailTab.vue', () => ({
  default: {
    name: 'RequestDetailTab',
    props: ['fields', 'showSkills', 'description', 'responsibilities', 'requirements', 'internalRequirements', 'incentive', 'incentiveDescription', 'complianceItems'],
    template: '<div class="detail-tab-stub"><slot name="skills" /><slot name="rail" /></div>',
  },
}));
vi.mock('@/shared/request-detail/RequestStaffingCard.vue', () => ({ default: childStub('RequestStaffingCard', ['items'], ['action']) }));
vi.mock('@/shared/request-detail/RequestLocationCard.vue', () => ({ default: childStub('RequestLocationCard', ['location']) }));
vi.mock('@/shared/request-detail/AgencyShiftDetail.vue', () => ({ default: childStub('AgencyShiftDetail', ['requestId', 'displayShift', 'fetchShift']) }));
vi.mock('@/modules/agency/recruiting/requests/components/AgencyRequestSkills.vue', () => ({ default: childStub('AgencyRequestSkills', ['modelValue', 'requestId', 'canEdit']) }));
vi.mock('@/modules/agency/recruiting/requests/components/RequestedBy.vue', () => ({ default: childStub('RequestedBy', ['modelValue', 'canEdit', 'requestId', 'companyProfileId']) }));
vi.mock('@/modules/agency/recruiting/requests/components/ReportTo.vue', () => ({ default: childStub('ReportTo', ['modelValue', 'canEdit', 'requestId', 'companyProfileId']) }));
vi.mock('@/modules/agency/recruiting/requests/components/RequestNotes.vue', () => ({ default: childStub('RequestNotes', ['canEdit']) }));
vi.mock('@/modules/agency/shared/AgencyWorkers.vue', () => ({ default: childStub('AgencyWorkers', ['request'], ['refreshRequest']) }));
vi.mock('@/shared/request-detail/CompanyCancelList.vue', () => ({ default: childStub('CompanyCancelList', [], ['sendReason']) }));
vi.mock('@/modules/agency/recruiting/requests/components/Applicants.vue', () => ({ default: childStub('Applicants', ['request'], ['changed']) }));
vi.mock('@/modules/agency/recruiting/requests/components/Runners.vue', () => ({ default: childStub('Runners', ['request'], ['changed']) }));
vi.mock('@/modules/agency/recruiting/requests/components/ShiftEditModal.vue', () => ({ default: childStub('ShiftEditModal', [], ['onUpdateShift']) }));
vi.mock('@/shared/company-profile/JobPositionForm.vue', () => ({
  default: childStub('JobPositionForm', ['kind', 'create', 'update', 'load', 'currentPosition', 'isDuplicate'], ['updateContent']),
}));

function detail(overrides: Partial<AgencyRequestDetail> = {}): AgencyRequestDetail {
  return {
    id: 'req-1',
    agencyId: 'ag-1',
    numberId: 1001,
    jobTitle: 'Forklift operator',
    billingTitle: 'Operator II',
    fullName: 'Acme Foods',
    companyLogo: 'https://logo.png',
    companyProfileId: 'cp-1',
    description: 'Move pallets',
    workersQuantity: 5,
    workersQuantityWorking: 2,
    jobPositionId: 'jp-1',
    jobPosition: 'Forklift',
    holidayIsPaid: true,
    breakIsPaid: false,
    status: RequestStatus.Open,
    createdAt: '2026-11-10T12:00:00',
    createdBy: 'maria@agency.com',
    startAt: '2026-11-20T00:00:00',
    finishAt: '2026-12-20T00:00:00',
    invitationSentItAt: null,
    durationBreak: '00:30:00',
    incentive: null,
    agencyRate: 30,
    workerRate: 20,
    workerSalary: null,
    durationTerm: DurationTerm.ShortTerm,
    employmentType: EmploymentType.FullTime,
    displayRecruiters: 'maria|john',
    displayShift: 'Mon-Fri 8-4',
    isAsap: false,
    usesRunners: true,
    vaccinationRequired: false,
    punchCardOptionEnabled: false,
    complianceItems: [],
    skills: [],
    requestedBy: [],
    reportTo: [],
    applicantsCount: 4,
    runnersCount: 1,
    workersCount: 2,
    ...overrides,
  };
}

async function setup(overrides: Partial<AgencyRequestDetail> = {}) {
  vi.mocked(getAgencyRequest).mockResolvedValue(detail(overrides));
  const wrapper = mountWithPlugins(Request);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function header(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'RequestHeader' });
}

function kpis(wrapper: Wrapper) {
  return header(wrapper).props('kpis') as RequestKpi[];
}

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text || b.attributes('aria-label') === text);
  if (!found) throw new Error(`${text} button not found`);
  return found;
}

async function openMenu(wrapper: Wrapper) {
  await button(wrapper, 'More actions').trigger('click');
  await nextTick();
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item'));
}

async function menuItem(wrapper: Wrapper, text: string) {
  const found = (await openMenu(wrapper)).find((i) => i.textContent.trim().startsWith(text));
  if (!found) throw new Error(`${text} not found`);
  return found;
}

function path(wrapper: Wrapper) {
  return wrapper.vm.$router.currentRoute.value.fullPath;
}

beforeEach(() => {
  admin.value = true;
  recruiting.value = true;
  route.path = '/recruiting/requests/req-1';
  route.query = {};
  vi.useFakeTimers({ toFake: ['Date'] });
  vi.setSystemTime(new Date(2026, 10, 19, 12));
});

afterEach(() => {
  vi.useRealTimers();
});

describe('Request', () => {
  it('loads the request into the header', async () => {
    const wrapper = await setup();

    expect(getAgencyRequest).toHaveBeenCalledExactlyOnceWith('req-1');
    expect(header(wrapper).props()).toEqual(expect.objectContaining({
      numberId: 1001,
      title: 'Forklift operator',
      subtitle: 'Billed as Operator II',
      orgName: 'Acme Foods',
      orgLink: '/recruiting/companies/cp-1',
      logo: 'https://logo.png',
      statusLabel: 'Open · Booking',
      statusVariant: 'is-warning',
      isAsap: false,
      isDirectHiring: false,
      chips: ['Covenant', 'Full Time · Short Term'],
      workersWorking: 2,
      workersTotal: 5,
    }));
    expect(kpis(wrapper)).toEqual([
      { key: 'rate', label: 'Rate', value: '$20.00 / hr', hint: undefined },
      { key: 'dates', label: 'Start / Finish', value: '20-Nov-2026', hint: '→ 20-Dec-2026' },
      { key: 'shift', label: 'Shift', value: 'Mon-Fri 8-4', hint: '30 min break · unpaid' },
      { key: 'compliance', label: 'Compliance', value: 'None', hint: 'No vaccination' },
      { key: 'recruiters', label: 'Recruiters', value: 'maria, john' },
    ]);
    expect(wrapper.text()).toContain('Created by maria');
    expect(wrapper.find('.slot-alert').text()).toBe('');
  });

  it('shows the request facts in the detail tab', async () => {
    const wrapper = await setup();

    expect(wrapper.findComponent({ name: 'RequestDetailTab' }).props('fields') as RequestFact[]).toEqual([
      { label: 'Role (position)', value: 'Forklift' },
      { label: 'Billing title', value: 'Operator II' },
      { label: 'Employment', value: 'Full Time · Short Term' },
      { label: 'Break', value: '30 min break · unpaid' },
      { label: 'Holiday', value: 'Paid' },
      { label: 'Bill rate', value: '$30.00 / hr' },
      { label: 'Vaccination', value: 'Not required', to: '/recruiting/companies/cp-1', linkLabel: 'Change in company' },
    ]);
    expect(wrapper.findComponent({ name: 'RequestedBy' }).props()).toEqual({ modelValue: [], canEdit: true, requestId: 'req-1', companyProfileId: 'cp-1' });
    expect(wrapper.findComponent({ name: 'AgencyRequestSkills' }).props('canEdit')).toBe(true);
  });

  it('describes a direct hiring long term request', async () => {
    const wrapper = await setup({
      workerSalary: 55000, workerRate: null, agencyRate: null, billingTitle: 'Forklift operator', durationTerm: DurationTerm.LongTerm,
      displayRecruiters: undefined, jobPosition: undefined, vaccinationRequired: true, holidayIsPaid: false,
      complianceItems: [{ id: 'c-1', name: 'WHMIS', isMandatory: true, documentTarget: 0 }], incentive: 2,
    });

    expect(header(wrapper).props('subtitle')).toBe('');
    expect(header(wrapper).props('isDirectHiring')).toBe(true);
    expect(kpis(wrapper)).toEqual([
      { key: 'rate', label: 'Salary', value: '$55,000.00 / year', hint: '+ $2.00 incentive' },
      { key: 'dates', label: 'Start', value: '20-Nov-2026', hint: undefined },
      { key: 'shift', label: 'Shift', value: 'Mon-Fri 8-4', hint: '30 min break · unpaid' },
      { key: 'compliance', label: 'Compliance', value: '1 item', hint: 'Vaccination required' },
    ]);
    const fields = wrapper.findComponent({ name: 'RequestDetailTab' }).props('fields') as RequestFact[];
    expect(fields.map((f) => f.value)).toEqual(['—', 'Forklift operator', 'Full Time · Long Term', '30 min break · unpaid', 'Not paid', '—', 'Required']);
    expect(wrapper.find('.slot-rate').text()).toBe('');
  });

  it('flags missing rates', async () => {
    const wrapper = await setup({ agencyRate: 0, workerRate: 0 });

    expect(kpis(wrapper)[0]).toEqual({ key: 'rate', label: 'Rate', value: 'Rates pending', hint: 'Set by an agency admin' });
  });

  it('shows the error when the request fails to load', async () => {
    const error = new Error('boom');
    vi.mocked(getAgencyRequest).mockRejectedValue(error);
    const wrapper = mountWithPlugins(Request);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.findComponent({ name: 'RequestHeader' }).exists()).toBe(false);
  });

  it('edits the request', async () => {
    const wrapper = await setup();
    await button(wrapper, 'Edit request').trigger('click');
    await flushPromises();

    expect(path(wrapper)).toBe('/recruiting/requests/update/cp-1/req-1');
  });

  it('asks to review a pending request', async () => {
    const wrapper = await setup({ status: RequestStatus.PendingReview });

    expect(wrapper.find('.slot-alert').text()).toContain('Pending review:');
    await button(wrapper, 'Review & approve').trigger('click');
    await flushPromises();
    expect(path(wrapper)).toBe('/recruiting/requests/update/cp-1/req-1');
  });

  it('only offers edit on a pending request without recruiting access', async () => {
    recruiting.value = false;
    const wrapper = await setup({ status: RequestStatus.PendingReview });

    expect(button(wrapper, 'Edit request').exists()).toBe(true);
    expect(wrapper.text()).not.toContain('Review & approve');
    expect(wrapper.text()).not.toContain('Runners');
  });

  it('reopens a cancelled request', async () => {
    vi.mocked(agencyRequestOpen).mockResolvedValue();
    const wrapper = await setup({ status: RequestStatus.Cancelled, cancellationDetail: 'Client closed' });

    expect(wrapper.find('.slot-alert').text()).toContain('Cancellation detail: Client closed');
    expect(wrapper.find('.slot-quantity').text()).toBe('');
    await button(wrapper, 'Reopen').trigger('click');
    await flushPromises();

    expect(agencyRequestOpen).toHaveBeenCalledExactlyOnceWith('req-1');
    expect(header(wrapper).props('statusLabel')).toBe('Open · Booking');
  });

  it('shows the error when reopening fails', async () => {
    const error = new Error('boom');
    vi.mocked(agencyRequestOpen).mockRejectedValue(error);
    const wrapper = await setup({ status: RequestStatus.Cancelled });
    await button(wrapper, 'Reopen').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('lists the menu actions of an open request', async () => {
    const wrapper = await setup({ workersQuantityWorking: 0 });

    expect((await openMenu(wrapper)).map((i) => i.textContent.trim())).toEqual(['Duplicate request', 'Mark as ASAP', 'Send an email invitation', 'Cancel request']);
  });

  it('only allows duplicating a cancelled request', async () => {
    const wrapper = await setup({ status: RequestStatus.Cancelled });
    const items = await openMenu(wrapper);
    expect(items.map((i) => i.textContent.trim())).toEqual(['Duplicate request']);

    items[0].click();
    await flushPromises();
    expect(path(wrapper)).toBe('/recruiting/requests/duplicate/cp-1/req-1');
  });

  it('toggles asap', async () => {
    vi.mocked(updateAgencyRequestIsAsap).mockResolvedValue();
    const wrapper = await setup({ isAsap: true });
    (await menuItem(wrapper, 'Remove ASAP')).click();
    await flushPromises();

    expect(updateAgencyRequestIsAsap).toHaveBeenCalledExactlyOnceWith('req-1');
    expect(header(wrapper).props('isAsap')).toBe(false);
  });

  it('shows the error when toggling asap fails', async () => {
    const error = new Error('boom');
    vi.mocked(updateAgencyRequestIsAsap).mockRejectedValue(error);
    const wrapper = await setup();
    (await menuItem(wrapper, 'Mark as ASAP')).click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(header(wrapper).props('isAsap')).toBe(false);
  });

  it('sends the invitation after confirming', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(agencyRequestSendInvitation).mockResolvedValue();
    const wrapper = await setup();
    (await menuItem(wrapper, 'Send an email invitation')).click();
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure?', 'The invitation must be sent only once every seven days.');
    expect(agencyRequestSendInvitation).toHaveBeenCalledExactlyOnceWith('req-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Sent it!');
    expect((await menuItem(wrapper, 'Send an email invitation')).classList).toContain('is-disabled');
  });

  it('does not send the invitation when not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    (await menuItem(wrapper, 'Send an email invitation')).click();
    await flushPromises();

    expect(agencyRequestSendInvitation).not.toHaveBeenCalled();
  });

  it('shows the error when the invitation fails', async () => {
    const error = new Error('boom');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(agencyRequestSendInvitation).mockRejectedValue(error);
    const wrapper = await setup();
    (await menuItem(wrapper, 'Send an email invitation')).click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('blocks a new invitation within seven days', async () => {
    const wrapper = await setup({ invitationSentItAt: '2026-11-15T12:00:00' });
    const item = await menuItem(wrapper, 'Send an email invitation');

    expect(item.classList).toContain('is-disabled');
    expect(item.textContent).toContain('(Sent 4 days ago)');
  });

  it('allows a new invitation after seven days', async () => {
    const wrapper = await setup({ invitationSentItAt: '2026-11-01T12:00:00' });

    expect((await menuItem(wrapper, 'Send an email invitation')).classList).not.toContain('is-disabled');
  });

  it('cancels the request and goes back to the list', async () => {
    vi.mocked(cancelAgencyRequest).mockResolvedValue();
    const wrapper = await setup({ workersQuantityWorking: 0 });
    (await menuItem(wrapper, 'Cancel request')).click();
    await nextTick();
    wrapper.findComponent({ name: 'CompanyCancelList' }).vm.$emit('sendReason', { reasonId: 'r-1', otherMessage: 'Closed' });
    await flushPromises();

    expect(cancelAgencyRequest).toHaveBeenCalledExactlyOnceWith('req-1', { cancellationReasonId: 'r-1', otherCancellationReason: 'Closed' });
    expect(showAlertSuccess).toHaveBeenCalledWith('Cancelled');
    expect(path(wrapper)).toBe('/recruiting/requests');
  });

  it('shows the error when cancelling fails', async () => {
    const error = new Error('boom');
    vi.mocked(cancelAgencyRequest).mockRejectedValue(error);
    const wrapper = await setup({ workersQuantityWorking: 0 });
    (await menuItem(wrapper, 'Cancel request')).click();
    await nextTick();
    wrapper.findComponent({ name: 'CompanyCancelList' }).vm.$emit('sendReason', { reasonId: 'r-1', otherMessage: '' });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(path(wrapper)).toBe('/');
  });

  it('adds and removes a worker spot', async () => {
    vi.mocked(increaseWorkersQuantityByOne).mockResolvedValue();
    vi.mocked(reduceWorkersQuantityByOne).mockResolvedValue();
    const wrapper = await setup();
    await button(wrapper, 'Add one worker').trigger('click');
    await flushPromises();
    await button(wrapper, 'Remove one worker').trigger('click');
    await flushPromises();

    expect(increaseWorkersQuantityByOne).toHaveBeenCalledExactlyOnceWith('req-1');
    expect(reduceWorkersQuantityByOne).toHaveBeenCalledExactlyOnceWith('req-1');
    expect(getAgencyRequest).toHaveBeenCalledTimes(3);
  });

  it('cannot remove spots already filled or the last one', async () => {
    const wrapper = await setup({ workersQuantity: 2, workersQuantityWorking: 2 });

    expect(button(wrapper, 'Remove one worker').attributes('disabled')).toBeDefined();
  });

  it('shows the error when changing the quantity fails', async () => {
    const error = new Error('boom');
    vi.mocked(increaseWorkersQuantityByOne).mockRejectedValue(error);
    vi.mocked(reduceWorkersQuantityByOne).mockRejectedValue(error);
    const wrapper = await setup();
    await button(wrapper, 'Add one worker').trigger('click');
    await flushPromises();
    await button(wrapper, 'Remove one worker').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledTimes(2);
    expect(getAgencyRequest).toHaveBeenCalledTimes(1);
  });

  it('edits the role rates as admin', async () => {
    vi.mocked(updateAgencyCompanyJobPosition).mockResolvedValue();
    vi.mocked(getAgencyCompanyJobPositionById).mockResolvedValue({ id: 'jp-1' } as AgencyCompanyJobPosition);
    const wrapper = await setup();
    await button(wrapper, 'Edit role rates').trigger('click');
    await nextTick();
    const form = wrapper.findComponent({ name: 'JobPositionForm' });
    expect(form.props()).toEqual(expect.objectContaining({ kind: 'full', currentPosition: { id: 'jp-1' }, isDuplicate: false }));

    const model = { jobPosition: 'Forklift' } as AgencyCompanyJobPosition;
    await (form.props('update') as (id: string, m: AgencyCompanyJobPosition) => Promise<void>)('jp-1', model);
    await (form.props('load') as (id: string) => Promise<AgencyCompanyJobPosition>)('jp-1');
    expect(updateAgencyCompanyJobPosition).toHaveBeenCalledWith('cp-1', 'jp-1', model);
    expect(getAgencyCompanyJobPositionById).toHaveBeenCalledWith('cp-1', 'jp-1');

    form.vm.$emit('updateContent');
    await flushPromises();
    expect(showAlertSuccess).toHaveBeenCalledWith('Role updated');
    expect(getAgencyRequest).toHaveBeenCalledTimes(2);
  });

  it('duplicates the role with rates as admin', async () => {
    vi.mocked(createAgencyCompanyJobPosition).mockResolvedValue({ id: 'jp-2' });
    const wrapper = await setup();
    await button(wrapper, 'Duplicate role').trigger('click');
    await nextTick();
    const form = wrapper.findComponent({ name: 'JobPositionForm' });
    expect(form.props('isDuplicate')).toBe(true);
    const model = { jobPosition: 'Forklift' } as AgencyCompanyJobPosition;
    await (form.props('create') as (m: AgencyCompanyJobPosition) => Promise<{ id: string }>)(model);
    expect(createAgencyCompanyJobPosition).toHaveBeenCalledWith('cp-1', model);

    form.vm.$emit('updateContent');
    await flushPromises();
    expect(showAlertSuccess).toHaveBeenCalledWith('Role duplicated');
    expect(getAgencyRequest).toHaveBeenCalledTimes(1);
  });

  it('duplicates the role without rates for non admins', async () => {
    admin.value = false;
    vi.mocked(createAgencyCompanyJobPositionWithoutRates).mockResolvedValue({ id: 'jp-2' });
    const wrapper = await setup();

    expect(wrapper.find('button[aria-label="Edit role rates"]').exists()).toBe(false);
    await button(wrapper, 'Duplicate role').trigger('click');
    await nextTick();
    const form = wrapper.findComponent({ name: 'JobPositionForm' });
    expect(form.props('kind')).toBe('basic');
    const model = { jobPosition: 'Forklift' } as AgencyCompanyJobPosition;
    await (form.props('create') as (m: AgencyCompanyJobPosition) => Promise<{ id: string }>)(model);
    expect(createAgencyCompanyJobPositionWithoutRates).toHaveBeenCalledWith('cp-1', model);
  });

  it('updates the shift from the shift editor', async () => {
    const wrapper = await setup();
    expect(wrapper.findComponent({ name: 'AgencyShiftDetail' }).props('displayShift')).toBe('Mon-Fri 8-4');
    await button(wrapper, 'Edit shift').trigger('click');
    await nextTick();
    wrapper.findComponent({ name: 'ShiftEditModal' }).vm.$emit('onUpdateShift', 'Sat 9-5');
    await nextTick();

    expect(wrapper.findComponent({ name: 'AgencyShiftDetail' }).props('displayShift')).toBe('Sat 9-5');
    wrapper.findComponent({ name: 'ShiftEditModal' }).vm.$emit('onUpdateShift', undefined);
    await nextTick();
    expect(wrapper.findComponent({ name: 'AgencyShiftDetail' }).props('displayShift')).toBe('Sat 9-5');
  });

  it('shows the tab counts and opens a tab from the query', async () => {
    route.query = { tab: 'Workers' };
    const wrapper = await setup();

    expect(wrapper.find('.tabs').text().replace(/\s+/g, ' ')).toContain('Applicants4');
    expect(wrapper.findComponent({ name: 'AgencyWorkers' }).exists()).toBe(true);
    expect(wrapper.findComponent({ name: 'Applicants' }).exists()).toBe(false);
  });

  it('falls back to the detail tab when runners are not allowed', async () => {
    recruiting.value = false;
    route.query = { tab: 'Runners' };
    const wrapper = await setup();

    expect(wrapper.findComponent({ name: 'Runners' }).exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'RequestDetailTab' }).exists()).toBe(true);
  });

  it('opens the applicants tab from the staffing card and refreshes the counts', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'RequestStaffingCard' }).vm.$emit('action', 'applicants');
    await flushPromises();
    expect(path(wrapper)).toBe('/recruiting/requests/req-1?tab=Applicants');

    vi.mocked(getAgencyRequest).mockResolvedValue(detail({ applicantsCount: 9, runnersCount: 3, workersCount: 4, workersQuantityWorking: 5, status: RequestStatus.Filled }));
    wrapper.findComponent({ name: 'Applicants' }).vm.$emit('changed');
    await flushPromises();
    expect(wrapper.find('.tabs').text()).toContain('9');
    expect(header(wrapper).props('statusLabel')).toBe('Filled');
    expect(header(wrapper).props('workersWorking')).toBe(5);
  });

  it('refreshes the counts from the runners and workers tabs', async () => {
    route.query = { tab: 'Runners' };
    const wrapper = await setup();
    wrapper.findComponent({ name: 'Runners' }).vm.$emit('changed');
    await flushPromises();
    expect(getAgencyRequest).toHaveBeenCalledTimes(2);

    const error = new Error('boom');
    vi.mocked(getAgencyRequest).mockRejectedValue(error);
    wrapper.findComponent({ name: 'Runners' }).vm.$emit('changed');
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
