import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import Notes from '@/modules/agency/recruiting/workers/components/Notes.vue';
import { createWorkerProfileNote, getWorkerProfileNotes } from '@/modules/agency/shared/notes/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { NoteItem, NotesCreatePayload, NotesFetchPayload } from '@/modules/agency/shared/notes/types';
import type { PaginatedList } from '@/shared/types/common';

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'wp-1' } }),
}));
vi.mock('@/modules/agency/shared/notes/api', () => ({ getWorkerProfileNotes: vi.fn(), createWorkerProfileNote: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/shared/notes/ModalNotes.vue', () => ({
  default: {
    name: 'ModalNotes',
    props: { userId: String, onGet: Function, onCreate: Function, showClose: Boolean },
    emits: ['onUpdateNote'],
    template: '<div class="modal-notes-stub" />',
  },
}));

const notes: NoteItem[] = [
  { id: 'n1', note: 'Prefers night shifts', color: '#ff0000', createdBy: 'jane.doe@sigook.com', createdAt: '2026-01-01T12:00:00' },
  { id: 'n2', note: 'Has safety boots', color: '#fefefe' },
];

function page(items: NoteItem[]): PaginatedList<NoteItem> {
  return { items, totalItems: items.length, pageIndex: 1, totalPages: 1 };
}

async function setup(items: NoteItem[] = notes) {
  vi.mocked(getWorkerProfileNotes).mockResolvedValue(page(items));
  const wrapper = mountWithPlugins(Notes);
  await flushPromises();
  return wrapper;
}

async function openModal(wrapper: Awaited<ReturnType<typeof setup>>) {
  await wrapper.find('button').trigger('click');
  await flushPromises();
  return wrapper.findComponent({ name: 'ModalNotes' });
}

describe('Notes', () => {
  it('loads the first page of the worker notes', async () => {
    const wrapper = await setup();

    expect(getWorkerProfileNotes).toHaveBeenCalledExactlyOnceWith('wp-1', { page: 1, size: 8 });
    const items = wrapper.findAll('li');
    expect(items).toHaveLength(2);
    expect(items[0].text()).toContain('Prefers night shifts');
    expect(items[0].text()).toContain('By:');
    expect(items[1].text()).not.toContain('By:');
    expect(items[1].find('.note-color-icon').classes()).toContain('has-border');
    expect(items[0].find('.note-color-icon').classes()).not.toContain('has-border');
  });

  it('shows no list when the worker has no notes', async () => {
    const wrapper = await setup([]);

    expect(wrapper.find('ul').exists()).toBe(false);
  });

  it('shows the error when the notes fail to load', async () => {
    const error = new Error('load failed');
    vi.mocked(getWorkerProfileNotes).mockRejectedValue(error);
    mountWithPlugins(Notes);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('opens the notes modal wired to the worker notes endpoints', async () => {
    const wrapper = await setup();
    const modal = await openModal(wrapper);

    expect(modal.props('userId')).toBe('wp-1');
    const onGet = modal.props('onGet') as (payload: NotesFetchPayload) => Promise<unknown>;
    const onCreate = modal.props('onCreate') as (payload: NotesCreatePayload) => Promise<unknown>;
    await onGet({ userId: 'wp-1', pagination: { page: 2, size: 20 } });
    expect(getWorkerProfileNotes).toHaveBeenLastCalledWith('wp-1', { page: 2, size: 20 });
    vi.mocked(createWorkerProfileNote).mockResolvedValue({ id: 'n3', note: 'New' });
    await onCreate({ userId: 'wp-1', model: { note: 'New' } });
    expect(createWorkerProfileNote).toHaveBeenCalledWith('wp-1', { note: 'New' });
  });

  it('reloads the notes when one changes and when the modal closes', async () => {
    const wrapper = await setup();
    const modal = await openModal(wrapper);

    modal.vm.$emit('onUpdateNote', { size: 20 });
    await flushPromises();
    expect(getWorkerProfileNotes).toHaveBeenCalledTimes(2);

    wrapper.findComponent({ name: 'BModal' }).vm.$emit('close');
    await flushPromises();
    expect(getWorkerProfileNotes).toHaveBeenCalledTimes(3);
  });
});
