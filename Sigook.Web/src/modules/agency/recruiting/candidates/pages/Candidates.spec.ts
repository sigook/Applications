import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Candidates from '@/modules/agency/recruiting/candidates/pages/Candidates.vue';
import {
  addCandidatePhoneNumber,
  addCandidateSkill,
  bulkAgencyCandidates,
  convertCandidateToWorker,
  deleteAgencyCandidate,
  deleteCandidatePhoneNumber,
  deleteCandidateSkill,
  getAgencyCandidates,
  updateAgencyCandidateRecruiter,
} from '@/modules/agency/recruiting/candidates/api';
import { createCandidateNote, deleteCandidateNote, getCandidateNotes } from '@/modules/agency/shared/notes/api';
import { getSources } from '@/shared/api/catalogApi';
import { useAgencyStore } from '@/modules/agency/store';
import { dateMonth, emailName } from '@/shared/format';
import { showAlertConfirm, showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyCandidateFilter, Candidate } from '@/modules/agency/recruiting/candidates/types';
import type { CatalogItem, PaginatedList } from '@/shared/types/common';
import type { NotesCreatePayload, NotesDeletePayload, NotesFetchPayload } from '@/modules/agency/shared/notes/types';

const { stub } = vi.hoisted(() => ({
  stub: (name: string, props: string[], emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/composables/useBreakpoint', () => ({ useBreakpoint: () => ({ isTouch: computed(() => false) }) }));
vi.mock('@/shared/api/catalogApi', () => ({ getSources: vi.fn() }));
vi.mock('@/modules/agency/shared/notes/api', () => ({ getCandidateNotes: vi.fn(), createCandidateNote: vi.fn(), deleteCandidateNote: vi.fn() }));
vi.mock('@/modules/agency/recruiting/candidates/api', () => ({
  getAgencyCandidates: vi.fn(),
  addCandidatePhoneNumber: vi.fn(),
  deleteCandidatePhoneNumber: vi.fn(),
  addCandidateSkill: vi.fn(),
  deleteCandidateSkill: vi.fn(),
  deleteAgencyCandidate: vi.fn(),
  updateAgencyCandidateRecruiter: vi.fn(),
  convertCandidateToWorker: vi.fn(),
  bulkAgencyCandidates: vi.fn(),
}));
vi.mock('@/modules/agency/recruiting/candidates/components/CreateCandidate.vue', () => ({ default: stub('CreateCandidate', [], ['onClose']) }));
vi.mock('@/modules/agency/recruiting/candidates/components/DetailCandidate.vue', () => ({ default: stub('DetailCandidate', ['candidateId'], ['onUpdateWorker']) }));
vi.mock('@/modules/agency/recruiting/candidates/components/ModalDocuments.vue', () => ({ default: stub('ModalDocuments', ['candidateId'], ['close']) }));
vi.mock('@/modules/agency/recruiting/candidates/components/ModalCandidateRequests.vue', () => ({ default: stub('CandidateRequest', ['candidateId'], ['onSelectRequest']) }));
vi.mock('@/modules/agency/recruiting/candidates/components/BulkData.vue', () => ({ default: stub('BulkData', ['uploadFn', 'errorFileName', 'title', 'fileLabel'], ['close']) }));
vi.mock('@/modules/agency/shared/notes/NotesPopover.vue', () => ({ default: stub('NotesPopover', ['canCreate', 'userId', 'notesCount', 'onGet', 'onCreate', 'onDelete'], ['update:count']) }));
vi.mock('@/shared/ui/FormSkillAdd.vue', () => ({ default: stub('SkillsForm', ['existingSkills'], ['onPressAdd', 'onDelete']) }));

const indeed: CatalogItem = { id: 's1', value: 'Indeed' };
const referral: CatalogItem = { id: 's2', value: 'Referral' };

const ana: Candidate = {
  id: 'c1', agencyId: 'a1', name: 'Ana Smith', email: 'ana@mail.com', address: '100 king st', postalCode: 'M5H 1A1', city: 'Toronto', province: 'ON',
  residencyStatus: 'Citizen', phoneNumbers: [{ id: 'ph1', phoneNumber: '416 555-1234' }], skills: [{ id: 'sk1', skill: 'Forklift' }],
  requests: [{ id: 'req-1', value: '1001' }], recruiter: 'john.doe@sigook.com', createdAt: '2026-11-01T10:00:00',
  hasVehicle: true, hasDocuments: true, notesCount: 3, dnu: false, source: 'Indeed', sourceId: 's1',
};
const bob: Candidate = {
  id: 'c2', agencyId: 'a1', name: 'Bob Lee', phoneNumbers: [], skills: [], createdAt: '2026-11-02T10:00:00',
  hasVehicle: false, hasDocuments: false, notesCount: 0, dnu: true,
};

let pinia: Pinia;

function page(items: Candidate[]): PaginatedList<Candidate> {
  return { items, totalItems: 42, pageIndex: 1, totalPages: 2 };
}

async function setup() {
  const wrapper = mountWithPlugins(Candidates, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function row(wrapper: Wrapper, index: number) {
  return wrapper.findAll('tbody tr')[index];
}

function cells(wrapper: Wrapper, index: number) {
  return row(wrapper, index).findAll('td').map((c) => c.text().replace(/\s+/g, ' ').trim());
}

function lastParams() {
  return vi.mocked(getAgencyCandidates).mock.calls.at(-1)?.[0] as AgencyCandidateFilter;
}

function taginput(wrapper: Wrapper, placeholder: string, index = 0) {
  return wrapper.findAllComponents({ name: 'BTaginput' }).filter((t) => t.find('input').attributes('placeholder') === placeholder)[index];
}

function searchInputs(wrapper: Wrapper) {
  return wrapper.findAll('th input[placeholder="Search..."]').filter((i) => !i.element.closest('.datepicker'));
}

async function rowAction(wrapper: Wrapper, index: number, text: string) {
  await row(wrapper, index).find('button[aria-label="Row actions"]').trigger('click');
  await flushPromises();
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).findLast((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  useAgencyStore().setAgencies([{ id: 'a1', name: 'Covenant' }]);
  vi.mocked(getAgencyCandidates).mockResolvedValue(page([structuredClone(ana), structuredClone(bob)]));
  vi.mocked(getSources).mockResolvedValue([indeed, referral]);
});

describe('Candidates', () => {
  it('loads the first page and shows the candidates', async () => {
    vi.mocked(getAgencyCandidates).mockResolvedValue({ ...page([ana]), items: [{ ...ana, notesCount: undefined as unknown as number }] });
    const wrapper = await setup();

    expect(getAgencyCandidates).toHaveBeenCalledExactlyOnceWith({ sortBy: 0, isDescending: false, pageIndex: 1, pageSize: 30 });
    expect(useAgencyStore().agencyCandidateFilter).toEqual({ sortBy: 0, isDescending: false, pageIndex: 1, pageSize: 30 });
    expect(wrapper.findComponent({ name: 'PageHeader' }).props()).toMatchObject({ title: 'Candidates', count: 42, crumbs: [{ label: 'Recruiting' }] });
    const [name, , address, , requests, source, createdAt, recruiter] = cells(wrapper, 0);
    expect(name).toContain('Ana Smith');
    expect(name).toContain('ana@mail.com');
    expect(address).toBe('100 king stToronto, ON M5H 1A1');
    expect(requests).toBe('1001');
    expect(source).toBe('Indeed');
    expect(createdAt).toBe(dateMonth('2026-11-01T10:00:00'));
    expect(recruiter).toBe(emailName('john.doe@sigook.com'));
    expect(wrapper.findComponent({ name: 'NotesPopover' }).props('notesCount')).toBe(0);
    expect(taginput(wrapper, 'Add Phone').props('modelValue')).toEqual([{ id: 'ph1', phoneNumber: '416 555-1234' }]);
    expect(wrapper.findComponent({ name: 'SkillsForm' }).props('existingSkills')).toEqual([{ id: 'sk1', skill: 'Forklift' }]);
  });

  it('shows the placeholder recruiter and status of a bare candidate', async () => {
    const wrapper = await setup();

    expect(cells(wrapper, 1)[7]).toBe('Recruiter');
    expect(row(wrapper, 0).find('.tag.is-medium').text()).toBe('Citizen');
    expect(row(wrapper, 1).find('.tag.is-medium').exists()).toBe(false);
  });

  it('restores the stored filter', async () => {
    useAgencyStore().updateAgencyCandidateFilter({
      sortBy: 3, isDescending: true, pageIndex: 2, pageSize: 60, statuses: ['Student'], sources: ['Referral'],
      createdAtFrom: '2026-11-01T04:00:00.000Z', createdAtTo: '2026-11-15T05:00:00.000Z',
    });
    const wrapper = await setup();

    expect(vi.mocked(getAgencyCandidates).mock.calls[0][0]).toMatchObject({ sortBy: 3, isDescending: true, pageIndex: 2, pageSize: 60 });
    expect(taginput(wrapper, 'Select Status').props('modelValue')).toEqual(['Student']);
    expect(taginput(wrapper, 'Select Source').props('modelValue')).toEqual([referral]);
    expect(wrapper.findComponent({ name: 'BDatepicker' }).props('modelValue')).toEqual([new Date('2026-11-01T04:00:00.000Z'), new Date('2026-11-15T05:00:00.000Z')]);
  });

  it('searches by source, status, creation date and resume', async () => {
    const wrapper = await setup();

    taginput(wrapper, 'Select Source').vm.$emit('update:modelValue', [indeed]);
    await flushPromises();
    expect(lastParams()).toMatchObject({ sources: ['Indeed'], pageIndex: 1 });

    taginput(wrapper, 'Select Status').vm.$emit('update:modelValue', ['Citizen']);
    await flushPromises();
    expect(lastParams()).toMatchObject({ statuses: ['Citizen'] });

    const from = new Date('2026-11-01T00:00:00');
    const to = new Date('2026-11-15T00:00:00');
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(lastParams()).toMatchObject({ createdAtFrom: from.toISOString(), createdAtTo: to.toISOString() });

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('icon-right-click');
    await flushPromises();
    expect(lastParams()).toMatchObject({ createdAtFrom: null, createdAtTo: null });

    const resumeOnly = wrapper.findAllComponents({ name: 'BCheckbox' }).find((c) => c.props('size') === 'is-small');
    resumeOnly?.vm.$emit('update:modelValue', true);
    await flushPromises();
    expect(lastParams()).toMatchObject({ resumeOnly: true });
    expect(getAgencyCandidates).toHaveBeenCalledTimes(6);
  });

  it('searches by the text filters on enter', async () => {
    const wrapper = await setup();
    const [name, phone, address, skills, numberId, recruiter] = searchInputs(wrapper);
    await name.setValue('ana');
    await phone.setValue('4165551234');
    await address.setValue('king');
    await skills.setValue('forklift');
    await numberId.setValue('1001');
    await recruiter.setValue('john');
    await recruiter.trigger('keyup.enter');
    await flushPromises();

    expect(lastParams()).toMatchObject({ name: 'ana', phone: '416 555-1234', address: 'king', skills: 'forklift', numberId: '1001', recruiter: 'john', pageIndex: 1 });
  });

  it('adds and removes phone numbers', async () => {
    vi.mocked(addCandidatePhoneNumber).mockResolvedValue({ id: 'ph2' });
    vi.mocked(deleteCandidatePhoneNumber).mockResolvedValue();
    const wrapper = await setup();
    const phones = taginput(wrapper, 'Add Phone');

    expect((phones.props('beforeAdding') as (value: string) => string)('4165559999')).toBe('416 555-9999');

    phones.vm.$emit('add', '416 555-9999');
    await flushPromises();
    expect(addCandidatePhoneNumber).toHaveBeenCalledExactlyOnceWith('c1', { phoneNumber: '416 555-9999' });

    phones.vm.$emit('remove', { id: 'ph1', phoneNumber: '416 555-1234' });
    await flushPromises();
    expect(deleteCandidatePhoneNumber).toHaveBeenCalledExactlyOnceWith('c1', 'ph1');
    expect(getAgencyCandidates).toHaveBeenCalledTimes(3);
  });

  it('shows the errors of the phone numbers', async () => {
    const addError = new Error('add failed');
    const deleteError = new Error('delete failed');
    vi.mocked(addCandidatePhoneNumber).mockRejectedValue(addError);
    vi.mocked(deleteCandidatePhoneNumber).mockRejectedValue(deleteError);
    const wrapper = await setup();
    taginput(wrapper, 'Add Phone').vm.$emit('add', '416 555-9999');
    taginput(wrapper, 'Add Phone').vm.$emit('remove', { phoneNumber: '416 555-1234' });
    await flushPromises();

    expect(deleteCandidatePhoneNumber).toHaveBeenCalledWith('c1', '');
    expect(showAlertError).toHaveBeenCalledWith(addError);
    expect(showAlertError).toHaveBeenCalledWith(deleteError);
    expect(getAgencyCandidates).toHaveBeenCalledTimes(1);
  });

  it('adds and removes skills', async () => {
    vi.mocked(addCandidateSkill).mockResolvedValue({ id: 'sk2' });
    vi.mocked(deleteCandidateSkill).mockResolvedValue();
    const wrapper = await setup();
    const skills = wrapper.findComponent({ name: 'SkillsForm' });
    skills.vm.$emit('onPressAdd', { skill: 'Packing' });
    skills.vm.$emit('onDelete', { id: 'sk1', skill: 'Forklift' });
    skills.vm.$emit('onDelete', { skill: 'Unsaved' });
    await flushPromises();

    expect(addCandidateSkill).toHaveBeenCalledExactlyOnceWith('c1', { skill: 'Packing' });
    expect(deleteCandidateSkill).toHaveBeenCalledExactlyOnceWith('c1', 'sk1');
    expect(getAgencyCandidates).toHaveBeenCalledTimes(3);
  });

  it('shows the errors of the skills', async () => {
    const error = new Error('skill failed');
    vi.mocked(addCandidateSkill).mockRejectedValue(error);
    vi.mocked(deleteCandidateSkill).mockRejectedValue(error);
    const wrapper = await setup();
    const skills = wrapper.findComponent({ name: 'SkillsForm' });
    skills.vm.$emit('onPressAdd', { skill: 'Packing' });
    skills.vm.$emit('onDelete', { id: 'sk1', skill: 'Forklift' });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledTimes(2);
    expect(getAgencyCandidates).toHaveBeenCalledTimes(1);
  });

  it('opens the applicants of a request and applies a candidate to another', async () => {
    const wrapper = await setup();
    const push = vi.spyOn(wrapper.vm.$router, 'push');
    await row(wrapper, 0).findAll('td')[4].find('.tag span').trigger('click');

    expect(push).toHaveBeenCalledWith({ path: '/recruiting/requests/req-1', query: { tab: 'Applicants' } });

    await row(wrapper, 1).find('button.is-small.is-rounded').trigger('click');
    await nextTick();
    const request = wrapper.findComponent({ name: 'CandidateRequest' });
    expect(request.props('candidateId')).toBe('c2');

    request.vm.$emit('onSelectRequest');
    await flushPromises();
    expect(getAgencyCandidates).toHaveBeenCalledTimes(2);
  });

  it('assigns the recruiter after confirming', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValueOnce(false).mockResolvedValueOnce(true);
    vi.mocked(updateAgencyCandidateRecruiter).mockResolvedValue();
    const wrapper = await setup();
    await row(wrapper, 1).find('.btn-icon-worker-plus').trigger('click');
    await flushPromises();

    expect(updateAgencyCandidateRecruiter).not.toHaveBeenCalled();

    await row(wrapper, 1).find('.btn-icon-worker-plus').trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Do you want to manage this candidate?', '');
    expect(updateAgencyCandidateRecruiter).toHaveBeenCalledExactlyOnceWith('c2');
    expect(getAgencyCandidates).toHaveBeenCalledTimes(2);
  });

  it('shows the error when assigning the recruiter fails', async () => {
    const error = new Error('recruiter failed');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(updateAgencyCandidateRecruiter).mockRejectedValue(error);
    const wrapper = await setup();
    await row(wrapper, 0).find('.btn-icon-worker-plus').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('edits a candidate and reloads after the update', async () => {
    const wrapper = await setup();
    (await rowAction(wrapper, 0, 'Edit')).click();
    await nextTick();

    const detail = wrapper.findComponent({ name: 'DetailCandidate' });
    expect(detail.props('candidateId')).toBe('c1');

    detail.vm.$emit('onUpdateWorker', true);
    await flushPromises();
    expect(getAgencyCandidates).toHaveBeenCalledTimes(2);
  });

  it('opens the documents from the menu and from the name of a candidate with documents', async () => {
    const wrapper = await setup();
    (await rowAction(wrapper, 1, 'Documents')).click();
    await nextTick();

    expect(wrapper.findComponent({ name: 'ModalDocuments' }).props('candidateId')).toBe('c2');

    wrapper.findComponent({ name: 'ModalDocuments' }).vm.$emit('close');
    await nextTick();
    const table = wrapper.findComponent({ name: 'BTable' });
    table.vm.$emit('cellclick', bob, { field: 'name' });
    table.vm.$emit('cellclick', ana, { field: 'address' });
    await nextTick();
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);

    table.vm.$emit('cellclick', ana, { field: 'name' });
    await nextTick();
    expect(wrapper.findComponent({ name: 'ModalDocuments' }).props('candidateId')).toBe('c1');
  });

  it('converts a candidate with email to worker', async () => {
    vi.mocked(convertCandidateToWorker).mockResolvedValue({ id: 'w1' });
    const wrapper = await setup();
    const convertBob = await rowAction(wrapper, 1, 'Convert to Worker');
    expect(convertBob.classList).toContain('is-disabled');

    (await rowAction(wrapper, 0, 'Convert to Worker')).click();
    await flushPromises();

    expect(convertCandidateToWorker).toHaveBeenCalledExactlyOnceWith('c1');
    expect(getAgencyCandidates).toHaveBeenCalledTimes(2);
  });

  it('shows the error when converting fails', async () => {
    const error = new Error('convert failed');
    vi.mocked(convertCandidateToWorker).mockRejectedValue(error);
    const wrapper = await setup();
    (await rowAction(wrapper, 0, 'Convert to Worker')).click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('deletes a candidate after confirming', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValueOnce(false).mockResolvedValueOnce(true);
    vi.mocked(deleteAgencyCandidate).mockResolvedValue();
    const wrapper = await setup();
    (await rowAction(wrapper, 0, 'Delete')).click();
    await flushPromises();

    expect(deleteAgencyCandidate).not.toHaveBeenCalled();

    (await rowAction(wrapper, 0, 'Delete')).click();
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this candidate');
    expect(deleteAgencyCandidate).toHaveBeenCalledExactlyOnceWith('c1');
    expect(getAgencyCandidates).toHaveBeenCalledTimes(2);
  });

  it('shows the error when deleting fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyCandidate).mockRejectedValue(error);
    const wrapper = await setup();
    (await rowAction(wrapper, 0, 'Delete')).click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('creates a candidate and reloads', async () => {
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Create')?.trigger('click');
    await nextTick();
    wrapper.findComponent({ name: 'CreateCandidate' }).vm.$emit('onClose', true);
    await flushPromises();

    expect(getAgencyCandidates).toHaveBeenCalledTimes(2);
  });

  it('uploads candidates in bulk', async () => {
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Actions')?.trigger('click');
    await nextTick();
    const bulk = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === 'Bulk Data');
    bulk?.click();
    await nextTick();

    expect(wrapper.findComponent({ name: 'BulkData' }).props()).toEqual({
      uploadFn: bulkAgencyCandidates,
      errorFileName: 'BulkCandidatesError',
      title: 'Bulk Candidates',
      fileLabel: 'Candidates File',
    });
  });

  it('passes the candidate notes api to the notes popover', async () => {
    vi.mocked(getCandidateNotes).mockResolvedValue({ items: [], totalItems: 0, pageIndex: 1, totalPages: 0 });
    const wrapper = await setup();
    const notes = wrapper.findComponent({ name: 'NotesPopover' });
    const pagination = { page: 1, pageSize: 10 };
    const model = { note: 'Call back' };

    await (notes.props('onGet') as (payload: NotesFetchPayload) => Promise<unknown>)({ userId: 'c1', pagination } as unknown as NotesFetchPayload);
    await (notes.props('onCreate') as (payload: NotesCreatePayload) => Promise<unknown>)({ userId: 'c1', model } as unknown as NotesCreatePayload);
    await (notes.props('onDelete') as (payload: NotesDeletePayload) => Promise<unknown>)({ userId: 'c1', id: 'n1' } as NotesDeletePayload);

    expect(getCandidateNotes).toHaveBeenCalledWith('c1', pagination);
    expect(createCandidateNote).toHaveBeenCalledWith('c1', model);
    expect(deleteCandidateNote).toHaveBeenCalledWith('c1', 'n1');

    notes.vm.$emit('update:count', 4);
    await nextTick();
    expect(wrapper.findComponent({ name: 'NotesPopover' }).props('notesCount')).toBe(4);
  });

  it('shows the error when the sources cannot be loaded', async () => {
    const error = new Error('sources failed');
    vi.mocked(getSources).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
