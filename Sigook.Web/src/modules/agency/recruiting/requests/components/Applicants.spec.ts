import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import Applicants from '@/modules/agency/recruiting/requests/components/Applicants.vue';
import {
  deleteAgencyRequestApplicant,
  getAgencyRequestApplicant,
  postAgencyRequestApplicant,
  updateAgencyRequestApplicant,
} from '@/modules/agency/recruiting/requests/api';
import { convertCandidateToWorker } from '@/modules/agency/recruiting/candidates/api';
import { createAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import { RequestApplicantStatus } from '@/shared/request-detail/requestApplicant';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestApplicant } from '@/modules/agency/recruiting/requests/types';
import type { PaginatedList } from '@/shared/types/common';

const { childStub } = vi.hoisted(() => ({
  childStub: (name: string, props: string[], emits: string[]) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'req-1' } }),
}));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({
  getAgencyRequestApplicant: vi.fn(),
  postAgencyRequestApplicant: vi.fn(),
  deleteAgencyRequestApplicant: vi.fn(),
  updateAgencyRequestApplicant: vi.fn(),
}));
vi.mock('@/modules/agency/recruiting/candidates/api', () => ({ convertCandidateToWorker: vi.fn() }));
vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ createAgencyRunner: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/recruiting/applicants/components/ApplicantComplianceDetail.vue', () => ({
  default: childStub('ApplicantComplianceDetail', ['requestId', 'applicantId', 'name', 'status', 'workerProfileId'], ['loaded', 'statusChanged']),
}));
vi.mock('@/modules/agency/recruiting/requests/components/ManageApplicantsModal.vue', () => ({ default: childStub('ManageApplicantsModal', [], ['updateApplicants']) }));
vi.mock('@/modules/agency/recruiting/requests/runners/SelectRunnerTypeModal.vue', () => ({ default: childStub('SelectRunnerTypeModal', ['name'], ['select', 'close']) }));
vi.mock('@/shared/ui/EditTextarea.vue', () => ({ default: childStub('EditTextarea', ['data', 'title', 'minLength'], ['updateContent']) }));
vi.mock('@/modules/agency/recruiting/candidates/components/DetailCandidate.vue', () => ({ default: childStub('DetailCandidate', ['candidateId'], ['onUpdateWorker']) }));

const worker: AgencyRequestApplicant = {
  id: 'ap-1',
  workerProfileId: 'wp-1',
  name: 'Ana Lopez',
  email: 'ana@mail.com',
  phoneNumber: '4165550000',
  comments: 'Good fit',
  createdAt: '2026-11-10T12:00:00',
  createdBy: 'maria@agency.com',
  status: RequestApplicantStatus.Confirmed,
  complianceTotal: 4,
  complianceCompleted: 3,
  mandatoryPending: 1,
};
const candidate: AgencyRequestApplicant = {
  id: 'ap-2',
  candidateId: 'cand-1',
  name: 'Leo Park',
  createdAt: '2026-11-11T12:00:00',
  status: RequestApplicantStatus.Pending,
  complianceTotal: 0,
  complianceCompleted: 0,
  mandatoryPending: 0,
};

function page(items: AgencyRequestApplicant[]): PaginatedList<AgencyRequestApplicant> {
  return { items, pageIndex: 1, totalPages: 1, totalItems: items.length };
}

async function setup(items: AgencyRequestApplicant[] = [worker, candidate]) {
  vi.mocked(getAgencyRequestApplicant).mockResolvedValue(page(items.map((i) => ({ ...i }))));
  const wrapper = mountWithPlugins(Applicants);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').filter((r) => !r.classes('detail'));
}

function cell(wrapper: Wrapper, row: number, column: number) {
  return rows(wrapper)[row].findAll('td').filter((c) => !c.classes('chevron-cell'))[column];
}

async function openActions(wrapper: Wrapper, row: number) {
  await rows(wrapper)[row].find('.dropdown').trigger('click');
  await nextTick();
  await nextTick();
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item[role="listitem"]')).map((i) => i.textContent.trim());
}

async function chooseAction(wrapper: Wrapper, row: number, text: string) {
  await openActions(wrapper, row);
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item[role="listitem"]')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  item.click();
  await flushPromises();
}

function modal(wrapper: Wrapper, name: string) {
  return wrapper.findComponent({ name });
}

describe('Applicants', () => {
  it('loads the applicants of the request newest first', async () => {
    await setup();

    expect(getAgencyRequestApplicant).toHaveBeenCalledWith(expect.objectContaining({ requestId: 'req-1', sortBy: 1, isDescending: true }));
  });

  it('renders each applicant with status, compliance and contact', async () => {
    const wrapper = await setup();

    expect(cell(wrapper, 0, 1).text()).toContain('Ana Lopez');
    expect(cell(wrapper, 0, 1).find('a[href="mailto:ana@mail.com"]').exists()).toBe(true);
    expect(cell(wrapper, 0, 2).find('.tag').text()).toBe('Confirmed');
    expect(cell(wrapper, 0, 2).find('.tag').classes()).toContain('is-success');
    expect(cell(wrapper, 0, 3).text()).toBe('3/4');
    expect(cell(wrapper, 0, 3).find('.applicants-compliance-bar span').attributes('style')).toContain('width: 75%');
    expect(cell(wrapper, 0, 4).find('a').attributes('href')).toBe('tel:4165550000');
    expect(cell(wrapper, 0, 5).text()).toContain('maria');
    expect(cell(wrapper, 0, 5).text()).toContain('10-Nov-2026');
    expect(cell(wrapper, 0, 6).text()).toBe('Good fit');
    expect(cell(wrapper, 1, 3).text()).toBe('0/0');
    expect(cell(wrapper, 1, 3).find('.applicants-compliance-bar span').classes()).toContain('is-complete');
    expect(cell(wrapper, 1, 4).text()).toBe('Phone');
    expect(cell(wrapper, 1, 5).text()).toContain('Added by');
  });

  it('offers actions by applicant kind', async () => {
    const wrapper = await setup();

    expect(await openActions(wrapper, 0)).toEqual(['Add Runner', 'Delete']);
  });

  it('offers candidate actions', async () => {
    const wrapper = await setup();

    expect(await openActions(wrapper, 1)).toEqual(['Edit Candidate', 'Convert to Worker', 'Delete']);
  });

  it('opens the worker profile from a cell', async () => {
    const wrapper = await setup();
    await cell(wrapper, 0, 1).trigger('click');
    await flushPromises();

    expect(wrapper.vm.$router.currentRoute.value.path).toBe('/recruiting/workers/wp-1');
  });

  it('does not navigate for a candidate or comments', async () => {
    const wrapper = await setup();
    await cell(wrapper, 1, 1).trigger('click');
    await cell(wrapper, 0, 6).trigger('click');
    await flushPromises();

    expect(wrapper.vm.$router.currentRoute.value.path).toBe('/');
  });

  it('toggles the compliance detail from the status cell', async () => {
    const wrapper = await setup();
    await cell(wrapper, 0, 2).trigger('click');
    await nextTick();

    const detail = modal(wrapper, 'ApplicantComplianceDetail');
    expect(detail.props()).toEqual({ requestId: 'req-1', applicantId: 'ap-1', name: 'Ana Lopez', status: RequestApplicantStatus.Confirmed, workerProfileId: 'wp-1' });

    detail.vm.$emit('loaded', { mandatoryCompleted: true, itemsCount: 5, completedCount: 5 });
    await nextTick();
    expect(cell(wrapper, 0, 3).text()).toBe('5/5');
    expect(cell(wrapper, 0, 3).find('.applicants-compliance-bar span').classes()).toContain('is-complete');

    await cell(wrapper, 0, 3).trigger('click');
    await nextTick();
    expect(modal(wrapper, 'ApplicantComplianceDetail').exists()).toBe(false);
  });

  it('marks pending mandatory items reported by the checklist', async () => {
    const wrapper = await setup([candidate]);
    await cell(wrapper, 0, 3).trigger('click');
    await nextTick();
    modal(wrapper, 'ApplicantComplianceDetail').vm.$emit('loaded', { mandatoryCompleted: false, itemsCount: 2, completedCount: 1 });
    await nextTick();

    expect(cell(wrapper, 0, 3).text()).toBe('1/2');
    expect(cell(wrapper, 0, 3).find('.applicants-compliance-bar span').classes()).not.toContain('is-complete');
  });

  it('reloads after a compliance status change', async () => {
    const wrapper = await setup();
    await cell(wrapper, 0, 2).trigger('click');
    await nextTick();
    modal(wrapper, 'ApplicantComplianceDetail').vm.$emit('statusChanged');
    await flushPromises();

    expect(getAgencyRequestApplicant).toHaveBeenCalledTimes(2);
    expect(wrapper.emitted('changed')).toHaveLength(1);
  });

  it('filters by status', async () => {
    const wrapper = await setup();
    const taginput = wrapper.findComponent({ name: 'BTaginput' });
    expect((taginput.props('data') as { value: string }[]).map((s) => s.value)).toEqual(['Pending', 'In progress', 'Confirmed', 'Cancelled']);
    taginput.vm.$emit('update:modelValue', [{ id: RequestApplicantStatus.Pending, value: 'Pending' }]);
    await flushPromises();
    expect(getAgencyRequestApplicant).toHaveBeenLastCalledWith(expect.objectContaining({ statuses: [RequestApplicantStatus.Pending], pageIndex: 1 }));

    taginput.vm.$emit('update:modelValue', []);
    await flushPromises();
    expect(vi.mocked(getAgencyRequestApplicant).mock.lastCall?.[0].statuses).toBeUndefined();
  });

  it('filters by creation dates and clears them', async () => {
    const wrapper = await setup();
    const picker = wrapper.findComponent({ name: 'BDatepicker' });
    const from = new Date(2026, 10, 1);
    const to = new Date(2026, 10, 15);
    picker.vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(getAgencyRequestApplicant).toHaveBeenLastCalledWith(expect.objectContaining({ createdAtFrom: from.toISOString(), createdAtTo: to.toISOString() }));

    picker.vm.$emit('icon-right-click');
    await flushPromises();
    expect(getAgencyRequestApplicant).toHaveBeenLastCalledWith(expect.objectContaining({ createdAtFrom: null, createdAtTo: null }));
  });

  it('formats the phone filter and searches on enter', async () => {
    const wrapper = await setup();
    const phone = wrapper.findAll('thead input[placeholder="Search..."]')[1];
    await phone.setValue('(416) 555-0000');
    await phone.trigger('keyup', { key: 'Enter' });
    await flushPromises();

    expect(getAgencyRequestApplicant).toHaveBeenLastCalledWith(expect.objectContaining({ phone: '416 555-0000', pageIndex: 1 }));
  });

  it('adds an applicant from the manage modal', async () => {
    vi.mocked(postAgencyRequestApplicant).mockResolvedValue(worker);
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-actions button').trigger('click');
    await nextTick();
    modal(wrapper, 'ManageApplicantsModal').vm.$emit('updateApplicants', { model: { workerProfileId: 'wp-9', candidateId: null, comments: null } });
    await flushPromises();

    expect(postAgencyRequestApplicant).toHaveBeenCalledExactlyOnceWith('req-1', { workerProfileId: 'wp-9', candidateId: null, comments: null });
    expect(getAgencyRequestApplicant).toHaveBeenCalledTimes(2);
    expect(wrapper.emitted('changed')).toHaveLength(1);
  });

  it('shows the error when adding fails', async () => {
    const error = new Error('boom');
    vi.mocked(postAgencyRequestApplicant).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-actions button').trigger('click');
    await nextTick();
    modal(wrapper, 'ManageApplicantsModal').vm.$emit('updateApplicants', { model: { candidateId: 'c-1' } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('changed')).toBeUndefined();
  });

  it('edits the comments of an applicant', async () => {
    vi.mocked(updateAgencyRequestApplicant).mockResolvedValue();
    const wrapper = await setup();
    await cell(wrapper, 0, 6).find('button').trigger('click');
    await nextTick();
    const editor = modal(wrapper, 'EditTextarea');
    expect(editor.props('data')).toBe('Good fit');
    editor.vm.$emit('updateContent', 'Great fit');
    await flushPromises();

    expect(updateAgencyRequestApplicant).toHaveBeenCalledExactlyOnceWith('req-1', 'ap-1', { comments: 'Great fit' });
    expect(wrapper.emitted('changed')).toHaveLength(1);
  });

  it('shows the error when the comment fails to save', async () => {
    const error = new Error('boom');
    vi.mocked(updateAgencyRequestApplicant).mockRejectedValue(error);
    const wrapper = await setup();
    await cell(wrapper, 0, 6).find('button').trigger('click');
    await nextTick();
    modal(wrapper, 'EditTextarea').vm.$emit('updateContent', 'x');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('deletes an applicant', async () => {
    vi.mocked(deleteAgencyRequestApplicant).mockResolvedValue();
    const wrapper = await setup();
    await chooseAction(wrapper, 1, 'Delete');

    expect(deleteAgencyRequestApplicant).toHaveBeenCalledExactlyOnceWith('req-1', 'ap-2');
    expect(wrapper.emitted('changed')).toHaveLength(1);
  });

  it('shows the error when deleting fails', async () => {
    const error = new Error('boom');
    vi.mocked(deleteAgencyRequestApplicant).mockRejectedValue(error);
    const wrapper = await setup();
    await chooseAction(wrapper, 1, 'Delete');

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('converts a candidate to worker', async () => {
    vi.mocked(convertCandidateToWorker).mockResolvedValue({ id: 'wp-5' });
    const wrapper = await setup();
    await chooseAction(wrapper, 1, 'Convert to Worker');

    expect(convertCandidateToWorker).toHaveBeenCalledExactlyOnceWith('cand-1');
    expect(wrapper.emitted('changed')).toHaveLength(1);
  });

  it('shows the error when converting fails', async () => {
    const error = new Error('boom');
    vi.mocked(convertCandidateToWorker).mockRejectedValue(error);
    const wrapper = await setup();
    await chooseAction(wrapper, 1, 'Convert to Worker');

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('edits a candidate and reloads when it is updated', async () => {
    const wrapper = await setup();
    await chooseAction(wrapper, 1, 'Edit Candidate');
    const detail = modal(wrapper, 'DetailCandidate');
    expect(detail.props('candidateId')).toBe('cand-1');
    detail.vm.$emit('onUpdateWorker', true);
    await flushPromises();

    expect(getAgencyRequestApplicant).toHaveBeenCalledTimes(2);
    expect(wrapper.emitted('changed')).toHaveLength(1);
  });

  it('adds a worker applicant as runner', async () => {
    vi.mocked(createAgencyRunner).mockResolvedValue('run-1');
    const wrapper = await setup();
    await chooseAction(wrapper, 0, 'Add Runner');
    const picker = modal(wrapper, 'SelectRunnerTypeModal');
    expect(picker.props('name')).toBe('Ana Lopez');
    picker.vm.$emit('select', RunnerType.Passive);
    await flushPromises();

    expect(createAgencyRunner).toHaveBeenCalledExactlyOnceWith('req-1', { workerProfileId: 'wp-1', type: RunnerType.Passive });
    expect(showAlertSuccess).toHaveBeenCalledWith('Runner added');
    expect(wrapper.emitted('changed')).toHaveLength(1);
  });

  it('shows the error when adding the runner fails', async () => {
    const error = new Error('boom');
    vi.mocked(createAgencyRunner).mockRejectedValue(error);
    const wrapper = await setup();
    await chooseAction(wrapper, 0, 'Add Runner');
    modal(wrapper, 'SelectRunnerTypeModal').vm.$emit('select', RunnerType.Active);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('changed')).toBeUndefined();
  });

  it('closes the runner type picker on cancel', async () => {
    const wrapper = await setup();
    await chooseAction(wrapper, 0, 'Add Runner');
    modal(wrapper, 'SelectRunnerTypeModal').vm.$emit('close');
    await nextTick();

    expect(createAgencyRunner).not.toHaveBeenCalled();
    expect(wrapper.findAllComponents({ name: 'BModal' })[2].props('modelValue')).toBe(false);
  });
});
