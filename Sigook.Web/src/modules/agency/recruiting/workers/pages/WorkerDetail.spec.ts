import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import WorkerDetail from '@/modules/agency/recruiting/workers/pages/WorkerDetail.vue';
import {
  agencyCommentWorker,
  getAgencyWorker,
  getAgencyWorkerComments,
  updateAgencyWorkerEmail,
  updateAgencyWorkerProfileDNU,
  updateApprovedToWork,
} from '@/modules/agency/recruiting/workers/api';
import { useAgencyStore } from '@/modules/agency/store';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerCommentList, WorkerProfileDetail } from '@/shared/worker-profile/types';

const { route, push, securityStore, stub, commentsOpen } = vi.hoisted(() => {
  const commentsOpen = vi.fn();
  return {
    route: { path: '/recruiting/workers/wp-1', params: { id: 'wp-1' } as Record<string, string>, query: {} as Record<string, string> },
    push: vi.fn(),
    securityStore: { userRoles: [] as string[] },
    commentsOpen,
    stub: (name: string, props: string[] = [], template = `<div class="${name}" />`, expose = false) => ({
      name,
      props,
      emits: ['updateProfile', 'loading', 'select', 'commentCreated', 'changePage'],
      template,
      ...(expose ? { methods: { open: commentsOpen } } : {}),
    }),
  };
});

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
  useRouter: () => ({ push }),
}));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/modules/agency/recruiting/workers/api', () => ({
  agencyCommentWorker: vi.fn(),
  getAgencyWorker: vi.fn(),
  getAgencyWorkerComments: vi.fn(),
  updateAgencyWorkerEmail: vi.fn(),
  updateAgencyWorkerProfileDNU: vi.fn(),
  updateApprovedToWork: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/worker-profile/cards/WorkerProfileHeader.vue', () => ({
  default: stub('WorkerProfileHeader', ['worker', 'numberClass'], '<div class="WorkerProfileHeader"><slot name="chips" /><slot name="actions" /></div>'),
}));
vi.mock('@/shared/worker-profile/cards/ProfileIndex.vue', () => ({ default: stub('ProfileIndex', ['sections', 'completeness', 'missingLabels', 'activeId']) }));
vi.mock('@/shared/worker-profile/cards/NeedsAttention.vue', () => ({ default: stub('NeedsAttention', ['items']) }));
vi.mock('@/shared/worker-profile/cards/PersonalCard.vue', () => ({ default: stub('PersonalCard', ['worker', 'updateEmail']) }));
vi.mock('@/shared/worker-profile/cards/ContactCard.vue', () => ({ default: stub('ContactCard', ['worker']) }));
vi.mock('@/shared/worker-profile/cards/DocumentsCard.vue', () => ({ default: stub('DocumentsCard', ['worker']) }));
vi.mock('@/shared/worker-profile/cards/PreferencesCard.vue', () => ({ default: stub('PreferencesCard', ['worker']) }));
vi.mock('@/shared/worker-profile/cards/SkillsCard.vue', () => ({ default: stub('SkillsCard', ['worker']) }));
vi.mock('@/shared/worker-profile/cards/ExperienceCard.vue', () => ({ default: stub('ExperienceCard', ['worker']) }));
vi.mock('@/shared/worker-profile/cards/CommentsCard.vue', () => ({
  default: stub('CommentsCard', ['workerProfileId', 'comments', 'pageIndex', 'pageSize', 'createComment'], '<div class="CommentsCard" />', true),
}));
vi.mock('@/modules/agency/recruiting/workers/components/Notes.vue', () => ({ default: stub('Notes') }));
vi.mock('@/modules/agency/recruiting/workers/components/WorkerSettings.vue', () => ({ default: stub('WorkerSettings', ['worker']) }));
vi.mock('@/modules/agency/recruiting/workers/components/WorkWageHistory.vue', () => ({ default: stub('WorkWageHistory', ['workerId']) }));
vi.mock('@/modules/agency/recruiting/workers/components/TimeSheetHistory.vue', () => ({ default: stub('TimeSheetHistory', ['workerId']) }));
vi.mock('@/modules/agency/recruiting/workers/components/AgencyWorkerRequestHistory.vue', () => ({ default: stub('AgencyWorkerRequestHistory', ['workerId']) }));

const comments: WorkerCommentList = { items: [], totalItems: 0 };

let pinia: Pinia;

async function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  vi.mocked(getAgencyWorker).mockResolvedValue(createWorkerProfileDetail({ id: 'wp-1', ...overrides }));
  vi.mocked(getAgencyWorkerComments).mockResolvedValue(comments);
  const wrapper = mountWithPlugins(WorkerDetail, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function chips(wrapper: Wrapper) {
  return wrapper.findAll('.worker-chip').map((c) => c.text());
}

function tabLabels(wrapper: Wrapper) {
  return wrapper.findAll('.tabs li').map((li) => li.text());
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

function hasMenuItem(text: string) {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).some((i) => i.textContent.trim() === text);
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  securityStore.userRoles = [];
  route.query = {};
});

describe('WorkerDetail', () => {
  it('loads the worker and its comments', async () => {
    const wrapper = await setup();

    expect(getAgencyWorker).toHaveBeenCalledWith('wp-1');
    expect(getAgencyWorkerComments).toHaveBeenCalledWith('wp-1', { size: 10, pageIndex: 1 });
    expect(wrapper.findComponent({ name: 'CommentsCard' }).props()).toMatchObject({
      workerProfileId: 'wp-1',
      comments,
      pageIndex: 1,
      pageSize: 10,
      createComment: agencyCommentWorker,
    });
    expect(wrapper.findComponent({ name: 'PersonalCard' }).props('updateEmail')).toBe(updateAgencyWorkerEmail);
    expect(wrapper.findComponent({ name: 'WorkerProfileHeader' }).props('worker')).toMatchObject({ id: 'wp-1' });
  });

  it('renders nothing and shows the error when the worker fails to load', async () => {
    const error = new Error('not found');
    vi.mocked(getAgencyWorker).mockRejectedValue(error);
    const wrapper = mountWithPlugins(WorkerDetail, { global: { plugins: [pinia] } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.find('.contain-worker').exists()).toBe(false);
  });

  it('shows the error when the comments fail to load', async () => {
    const error = new Error('comments failed');
    vi.mocked(getAgencyWorker).mockResolvedValue(createWorkerProfileDetail({ id: 'wp-1' }));
    vi.mocked(getAgencyWorkerComments).mockRejectedValue(error);
    const wrapper = mountWithPlugins(WorkerDetail, { global: { plugins: [pinia] } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.findComponent({ name: 'CommentsCard' }).exists()).toBe(false);
  });

  it('shows the status chips of an approved worker', async () => {
    useAgencyStore().setAgencies([{ id: 'agency-1', name: 'Covenant' }, { id: 'agency-2', name: 'Sigook' }]);
    const wrapper = await setup({ isSubcontractor: true, isContractor: true, dnu: true });

    expect(chips(wrapper)).toEqual(['Approved to work', 'Covenant', 'Subcontractor', 'Contractor', 'DNU']);
  });

  it('shows not approved without agency chip for a single agency user', async () => {
    const wrapper = await setup({ approvedToWork: false });

    expect(chips(wrapper)).toEqual(['Not approved']);
  });

  it('counts the items that need attention and scrolls to the first', async () => {
    const wrapper = await setup({ missingRequirements: ['ProfileImage', 'Identification'] });
    const button = wrapper.find('.worker-chip-button');

    expect(button.text()).toBe('2 items need attention');
    await button.trigger('click');
  });

  it('shows only the profile, timesheet and requests tabs to recruiters', async () => {
    securityStore.userRoles = ['recruiting'];
    const wrapper = await setup();

    expect(tabLabels(wrapper)).toEqual(['Profile', 'Timesheet', 'Requests']);
  });

  it('shows the settings and pay stubs tabs to admins', async () => {
    securityStore.userRoles = ['admin'];
    const wrapper = await setup();

    expect(tabLabels(wrapper)).toEqual(['Profile', 'Settings', 'PayStubs', 'Timesheet', 'Requests']);
  });

  it('opens the tab from the query and renders it', async () => {
    route.query = { tab: 'timeSheetHistory' };
    const wrapper = await setup();

    expect(wrapper.findComponent({ name: 'TimeSheetHistory' }).props('workerId')).toBe('wp-1');
    expect(wrapper.findComponent({ name: 'AgencyWorkerRequestHistory' }).exists()).toBe(false);
  });

  it('renders a tab once visited and keeps it in the url', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTabs' }).vm.$emit('update:modelValue', 'requestHistory');
    await flushPromises();

    expect(wrapper.findComponent({ name: 'AgencyWorkerRequestHistory' }).exists()).toBe(true);
    expect(push).toHaveBeenCalledWith({ path: '/recruiting/workers/wp-1', query: { tab: 'requestHistory' } });
  });

  it('opens the comment form from the header', async () => {
    const wrapper = await setup();
    const button = wrapper.findAll('button').find((b) => b.text() === 'Add comment');
    await button?.trigger('click');

    expect(commentsOpen).toHaveBeenCalledOnce();
  });

  it('reloads the comments when the page changes', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'CommentsCard' }).vm.$emit('changePage', 3);
    await flushPromises();

    expect(getAgencyWorkerComments).toHaveBeenLastCalledWith('wp-1', { size: 10, pageIndex: 3 });
  });

  it('reloads the worker when a card updates the profile', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'ContactCard' }).vm.$emit('updateProfile');
    await flushPromises();

    expect(getAgencyWorker).toHaveBeenCalledTimes(2);
  });

  it('approves a worker that is not approved', async () => {
    vi.mocked(updateApprovedToWork).mockResolvedValue();
    await setup({ approvedToWork: false });

    expect(hasMenuItem('Reject to work')).toBe(false);
    menuItem('Approve to work').click();
    await flushPromises();

    expect(updateApprovedToWork).toHaveBeenCalledWith('wp-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
    expect(getAgencyWorker).toHaveBeenCalledTimes(2);
  });

  it('rejects an approved worker once confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(updateApprovedToWork).mockResolvedValue();
    await setup();

    expect(hasMenuItem('Approve to work')).toBe(false);
    menuItem('Reject to work').click();
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure?', 'You want to disable the worker. This worker will not be able to apply to new requests');
    expect(updateApprovedToWork).toHaveBeenCalledWith('wp-1');
  });

  it('keeps the worker approved when the rejection is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    await setup();
    menuItem('Reject to work').click();
    await flushPromises();

    expect(updateApprovedToWork).not.toHaveBeenCalled();
  });

  it('shows the error when the approval fails and reloads the worker', async () => {
    const error = new Error('approve failed');
    vi.mocked(updateApprovedToWork).mockRejectedValue(error);
    await setup({ approvedToWork: false });
    menuItem('Approve to work').click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getAgencyWorker).toHaveBeenCalledTimes(2);
  });

  it('toggles DNU and reloads the worker', async () => {
    vi.mocked(updateAgencyWorkerProfileDNU).mockResolvedValue();
    const wrapper = await setup();
    const dnu = wrapper.find('.worker-profile-layout-rail input[type="checkbox"]');
    expect(dnu.attributes('disabled')).toBeUndefined();

    await dnu.setValue(true);
    await flushPromises();

    expect(updateAgencyWorkerProfileDNU).toHaveBeenCalledWith('wp-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
    expect(getAgencyWorker).toHaveBeenCalledTimes(2);
  });

  it('shows the error when the DNU toggle fails', async () => {
    const error = new Error('dnu failed');
    vi.mocked(updateAgencyWorkerProfileDNU).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.find('.worker-profile-layout-rail input[type="checkbox"]').setValue(true);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it.each([
    [['recruiting'], 'disabled'],
    [['admin'], undefined],
  ])('lets %j lift a DNU flag: %s', async (roles, disabled) => {
    securityStore.userRoles = roles;
    const wrapper = await setup({ dnu: true });

    expect(wrapper.find('.worker-profile-layout-rail input[type="checkbox"]').attributes('disabled')).toBe(disabled === 'disabled' ? '' : undefined);
  });
});
