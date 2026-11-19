import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import WorkerProfile from '@/modules/worker/profile/pages/WorkerProfile.vue';
import { getMyComments, getMyProfile } from '@/modules/worker/profile/api';
import { useWorkerStore } from '@/modules/worker/store';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerCommentList } from '@/shared/worker-profile/types';

const { push, query, stub } = vi.hoisted(() => ({
  push: vi.fn(),
  query: { value: {} as Record<string, string> },
  stub: (name: string, props: string[] = [], emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}"><slot name="chips" /></div>` }),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ query: query.value }),
  useRouter: () => ({ push }),
}));
vi.mock('@/modules/worker/profile/api', () => ({ getMyComments: vi.fn(), getMyProfile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/worker-profile/cards/WorkerProfileHeader.vue', () => ({ default: stub('WorkerProfileHeader', ['worker'], ['updateProfile']) }));
vi.mock('@/shared/worker-profile/cards/ProfileIndex.vue', () => ({
  default: stub('ProfileIndex', ['sections', 'completeness', 'missingLabels', 'activeId'], ['select']),
}));
vi.mock('@/shared/worker-profile/cards/NeedsAttention.vue', () => ({ default: stub('NeedsAttention', ['items'], ['select']) }));
vi.mock('@/modules/worker/profile/components/ApprovalStatusCard.vue', () => ({ default: stub('ApprovalStatusCard', ['approved']) }));
vi.mock('@/shared/worker-profile/cards/PersonalCard.vue', () => ({ default: stub('PersonalCard', ['worker', 'showLoginEmail'], ['updateProfile']) }));
vi.mock('@/shared/worker-profile/cards/ContactCard.vue', () => ({ default: stub('ContactCard', ['worker'], ['updateProfile']) }));
vi.mock('@/shared/worker-profile/cards/DocumentsCard.vue', () => ({ default: stub('DocumentsCard', ['worker'], ['updateProfile', 'loading']) }));
vi.mock('@/shared/worker-profile/cards/PreferencesCard.vue', () => ({ default: stub('PreferencesCard', ['worker'], ['updateProfile']) }));
vi.mock('@/shared/worker-profile/cards/SkillsCard.vue', () => ({ default: stub('SkillsCard', ['worker'], ['updateProfile']) }));
vi.mock('@/shared/worker-profile/cards/ExperienceCard.vue', () => ({ default: stub('ExperienceCard', ['worker'], ['updateProfile', 'loading']) }));
vi.mock('@/shared/worker-profile/cards/CommentsCard.vue', () => ({
  default: stub('CommentsCard', ['workerProfileId', 'comments', 'pageIndex', 'pageSize', 'readonly', 'title'], ['changePage']),
}));
vi.mock('@/modules/worker/profile/components/WorkerAccountSecurity.vue', () => ({ default: stub('WorkerAccountSecurity') }));

let pinia: Pinia;

const comments: WorkerCommentList = { items: [], totalItems: 0 };

async function setup() {
  const wrapper = mountWithPlugins(WorkerProfile, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function child(wrapper: Wrapper, name: string) {
  return wrapper.findComponent({ name });
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  query.value = {};
  vi.mocked(getMyProfile).mockResolvedValue(createWorkerProfileDetail());
  vi.mocked(getMyComments).mockResolvedValue(comments);
  vi.spyOn(Element.prototype, 'scrollIntoView').mockImplementation(() => undefined);
});

describe('WorkerProfile', () => {
  it('loads the profile into the cards and the store', async () => {
    const wrapper = await setup();
    const worker = createWorkerProfileDetail();
    expect(getMyComments).toHaveBeenCalledWith({ size: 10, pageIndex: 1 });
    expect(useWorkerStore().workerProfile).toEqual(worker);
    expect(child(wrapper, 'PersonalCard').props()).toEqual({ worker, showLoginEmail: false });
    expect(child(wrapper, 'PersonalCard').attributes('id')).toBe('worker-section-personal');
    expect(child(wrapper, 'ApprovalStatusCard').props('approved')).toBe(true);
    expect(child(wrapper, 'CommentsCard').props()).toMatchObject({ workerProfileId: 'worker-profile-1', comments, pageIndex: 1, pageSize: 10, title: 'Comments from your agency' });
    expect(wrapper.find('.worker-chip').text()).toBe('Approved to work');
    expect(wrapper.find('.worker-chip-button').exists()).toBe(false);
  });

  it('points a pending worker to the items that need attention', async () => {
    vi.mocked(getMyProfile).mockResolvedValue(createWorkerProfileDetail({ approvedToWork: false, missingRequirements: ['Identification', 'ProfileImage'] }));
    const wrapper = await setup();
    expect(wrapper.find('.worker-chip').text()).toBe('Pending approval');
    const chip = wrapper.find('.worker-chip-button');
    expect(chip.text()).toBe('2 items need attention');
    await chip.trigger('click');
    expect(child(wrapper, 'ProfileIndex').props('activeId')).toBe('documents');
  });

  it('uses the singular for one item', async () => {
    vi.mocked(getMyProfile).mockResolvedValue(createWorkerProfileDetail({ missingRequirements: ['ProfileImage'] }));
    const wrapper = await setup();
    expect(wrapper.find('.worker-chip-button').text()).toBe('1 item needs attention');
  });

  it('selects the section from the query', async () => {
    query.value = { section: 'skills' };
    const wrapper = await setup();
    expect(child(wrapper, 'ProfileIndex').props('activeId')).toBe('skills');
  });

  it('reloads the profile when a card saves', async () => {
    const wrapper = await setup();
    child(wrapper, 'SkillsCard').vm.$emit('updateProfile');
    await flushPromises();
    expect(getMyProfile).toHaveBeenCalledTimes(2);
  });

  it('pages the comments', async () => {
    const wrapper = await setup();
    child(wrapper, 'CommentsCard').vm.$emit('changePage', 3);
    await flushPromises();
    expect(getMyComments).toHaveBeenLastCalledWith({ size: 10, pageIndex: 3 });
    expect(child(wrapper, 'CommentsCard').props('pageIndex')).toBe(3);
  });

  it('opens the account tab once visited', async () => {
    const wrapper = await setup();
    expect(child(wrapper, 'WorkerAccountSecurity').exists()).toBe(false);
    child(wrapper, 'BTabs').vm.$emit('update:modelValue', 'account');
    await flushPromises();
    expect(push).toHaveBeenCalledWith({ path: '/worker-profile', query: { tab: 'account' } });
    expect(child(wrapper, 'WorkerAccountSecurity').exists()).toBe(true);
  });

  it('starts on the account tab from the query', async () => {
    query.value = { tab: 'account' };
    const wrapper = await setup();
    expect(child(wrapper, 'BTabs').props('modelValue')).toBe('account');
    expect(child(wrapper, 'WorkerAccountSecurity').exists()).toBe(true);
  });

  it('reports failed loads', async () => {
    vi.mocked(getMyProfile).mockRejectedValue('profile down');
    vi.mocked(getMyComments).mockRejectedValue('comments down');
    const wrapper = await setup();
    expect(showAlertError).toHaveBeenCalledWith('profile down');
    expect(showAlertError).toHaveBeenCalledWith('comments down');
    expect(child(wrapper, 'PersonalCard').exists()).toBe(false);
  });
});
