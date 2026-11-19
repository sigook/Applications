import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ModalNotes from '@/modules/agency/shared/notes/ModalNotes.vue';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PaginatedList } from '@/shared/types/common';
import type {
  CreateNoteResponse,
  NoteItem,
  RequestNotesCreatePayload,
  RequestNotesDeletePayload,
  RequestNotesFetchPayload,
  RequestNotesUpdatePayload,
} from '@/modules/agency/shared/notes/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const { noteFormStub } = vi.hoisted(() => ({
  noteFormStub: { name: 'NoteForm', props: { currentNote: Object }, emits: ['onSave'], template: '<div class="note-form" />' },
}));

vi.mock('@/modules/agency/shared/notes/NoteForm.vue', () => ({ default: noteFormStub }));

function page(items: NoteItem[], totalPages = 1): PaginatedList<NoteItem> {
  return { items, totalItems: items.length, pageIndex: 1, totalPages };
}

const firstNote: NoteItem = { id: 'n1', note: 'Called worker', color: '#fefefe', createdAt: '2026-05-30T12:00:00', createdBy: 'ana.lopez@sigook.com' };
const secondNote: NoteItem = { id: 'n2', note: 'No answer', color: '#f28c82' };

interface Handlers {
  onGet?: (payload: RequestNotesFetchPayload) => Promise<PaginatedList<NoteItem>>;
  onCreate?: (payload: RequestNotesCreatePayload) => Promise<CreateNoteResponse>;
  onUpdate?: (payload: RequestNotesUpdatePayload) => Promise<void>;
  onDelete?: (payload: RequestNotesDeletePayload) => Promise<void>;
  showClose?: boolean;
}

function handlers() {
  return {
    onGet: vi.fn<(payload: RequestNotesFetchPayload) => Promise<PaginatedList<NoteItem>>>().mockResolvedValue(page([firstNote, secondNote])),
    onCreate: vi.fn<(payload: RequestNotesCreatePayload) => Promise<CreateNoteResponse>>(),
    onUpdate: vi.fn<(payload: RequestNotesUpdatePayload) => Promise<void>>(),
    onDelete: vi.fn<(payload: RequestNotesDeletePayload) => Promise<void>>(),
  };
}

async function setup(props: Handlers) {
  const wrapper = mountWithPlugins(ModalNotes, { props: { userId: 'u1', requestId: 'r1', ...props } });
  await flushPromises();
  return wrapper;
}

function noteTexts(wrapper: Awaited<ReturnType<typeof setup>>) {
  return wrapper.findAll('.note-list li').map((li) => li.find('div').text());
}

beforeEach(() => {
  vi.useFakeTimers({ now: new Date('2026-06-01T12:00:00'), toFake: ['Date'] });
});

afterEach(() => {
  vi.useRealTimers();
});

describe('ModalNotes', () => {
  it('loads the first page of notes', async () => {
    const h = handlers();
    const wrapper = await setup(h);
    expect(h.onGet).toHaveBeenCalledWith({ userId: 'u1', requestId: 'r1', pagination: { page: 1, size: 20 } });
    expect(noteTexts(wrapper)[0]).toContain('Called worker');
    expect(noteTexts(wrapper)[0]).toContain('By: ana.lopez');
    expect(noteTexts(wrapper)[1]).toBe('No answer');
  });

  it('shows the empty state', async () => {
    const h = handlers();
    h.onGet.mockResolvedValue(page([]));
    const wrapper = await setup(h);
    expect(wrapper.text()).toContain('No notes yet');
  });

  it('shows nothing without a loader', async () => {
    const wrapper = await setup({});
    expect(wrapper.find('.note-list').exists()).toBe(false);
    expect(wrapper.text()).not.toContain('No notes yet');
  });

  it('reports a failed load', async () => {
    const h = handlers();
    h.onGet.mockRejectedValue('boom');
    await setup(h);
    expect(showAlertError).toHaveBeenCalledWith('boom');
  });

  it('paginates when there are several pages', async () => {
    const h = handlers();
    h.onGet.mockResolvedValue(page([firstNote], 3));
    const wrapper = await setup(h);
    const pagination = wrapper.findComponent({ name: 'BPagination' });
    expect(pagination.exists()).toBe(true);
    pagination.vm.$emit('change', 2);
    await flushPromises();
    expect(h.onGet).toHaveBeenLastCalledWith({ userId: 'u1', requestId: 'r1', pagination: { page: 2, size: 20 } });
  });

  it('adds a created note on top', async () => {
    const h = handlers();
    h.onCreate.mockResolvedValue({ id: 'n3', note: 'Hired', createdAt: '2026-06-01T11:00:00', createdBy: 'bob@sigook.com' });
    const wrapper = await setup(h);
    wrapper.findComponent(noteFormStub).vm.$emit('onSave', { note: 'Hired', color: '#ccff91' });
    await flushPromises();

    expect(h.onCreate).toHaveBeenCalledWith({ userId: 'u1', requestId: 'r1', model: { note: 'Hired', color: '#ccff91' } });
    expect(noteTexts(wrapper)).toHaveLength(3);
    expect(noteTexts(wrapper)[0]).toContain('Hired');
    expect(wrapper.emitted('onUpdateNote')).toEqual([[{ size: 3 }]]);
  });

  it('reports a failed creation', async () => {
    const h = handlers();
    h.onCreate.mockRejectedValue('nope');
    const wrapper = await setup(h);
    wrapper.findComponent(noteFormStub).vm.$emit('onSave', { note: 'Hired' });
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('nope');
    expect(wrapper.emitted('onUpdateNote')).toBeUndefined();
  });

  it('ignores saves without a creator', async () => {
    const h = handlers();
    const wrapper = await setup({ onGet: h.onGet });
    wrapper.findComponent(noteFormStub).vm.$emit('onSave', { note: 'Hired' });
    await flushPromises();
    expect(noteTexts(wrapper)).toHaveLength(2);
    expect(wrapper.find('button .mdi-pencil').exists()).toBe(false);
    expect(wrapper.find('.has-text-danger').exists()).toBe(false);
  });

  it('deletes a note', async () => {
    const h = handlers();
    h.onDelete.mockResolvedValue();
    const wrapper = await setup(h);
    await wrapper.findAll('.note-list li')[0].find('.has-text-danger').trigger('click');
    await flushPromises();

    expect(h.onDelete).toHaveBeenCalledWith({ userId: 'u1', requestId: 'r1', id: 'n1' });
    expect(noteTexts(wrapper)).toEqual(['No answer']);
    expect(showAlertSuccess).toHaveBeenCalledWith('Deleted');
    expect(wrapper.emitted('onUpdateNote')).toEqual([[{ size: 1 }]]);
  });

  it('reports a failed delete', async () => {
    const h = handlers();
    h.onDelete.mockRejectedValue('locked');
    const wrapper = await setup(h);
    await wrapper.findAll('.note-list li')[0].find('.has-text-danger').trigger('click');
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('locked');
    expect(noteTexts(wrapper)).toHaveLength(2);
  });

  it('edits a note in a modal', async () => {
    const h = handlers();
    h.onUpdate.mockResolvedValue();
    const wrapper = await setup(h);
    await wrapper.findAll('.note-list li')[1].findAll('.note-actions button')[0].trigger('click');
    await flushPromises();

    const editForm = wrapper.findAllComponents(noteFormStub).find((f) => f.props('currentNote'));
    if (!editForm) throw new Error('edit form not found');
    expect(editForm.props('currentNote')).toEqual(secondNote);
    editForm.vm.$emit('onSave', { id: 'n2', note: 'Answered', color: '#a7ffeb' });
    await flushPromises();

    expect(h.onUpdate).toHaveBeenCalledWith({ userId: 'u1', requestId: 'r1', id: 'n2', model: { id: 'n2', note: 'Answered', color: '#a7ffeb' } });
    expect(noteTexts(wrapper)[1]).toBe('Answered');
    expect(wrapper.emitted('onUpdateNote')).toEqual([[{ size: 2 }]]);
    expect(wrapper.findAllComponents(noteFormStub).filter((f) => f.props('currentNote'))).toHaveLength(0);
  });

  it('reports a failed edit', async () => {
    const h = handlers();
    h.onUpdate.mockRejectedValue('bad');
    const wrapper = await setup(h);
    await wrapper.findAll('.note-list li')[0].findAll('.note-actions button')[0].trigger('click');
    await flushPromises();
    const editForm = wrapper.findAllComponents(noteFormStub).find((f) => f.props('currentNote'));
    editForm?.vm.$emit('onSave', { note: 'x' });
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('bad');
    expect(noteTexts(wrapper)[0]).toContain('Called worker');
  });

  it('emits close and can hide the close button', async () => {
    const wrapper = await setup(handlers());
    await wrapper.find('h3 + button').trigger('click');
    expect(wrapper.emitted('close')).toHaveLength(1);

    const hidden = await setup({ ...handlers(), showClose: false });
    expect(hidden.find('h3 + button').exists()).toBe(false);
  });
});
