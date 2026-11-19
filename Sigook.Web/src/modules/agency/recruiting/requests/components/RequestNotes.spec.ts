import { afterEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import RequestNotes from '@/modules/agency/recruiting/requests/components/RequestNotes.vue';
import { createAgencyRequestNote, deleteAgencyRequestNote, getAgencyRequestNotes, updateAgencyRequestNote } from '@/modules/agency/shared/notes/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { NoteItem } from '@/modules/agency/shared/notes/types';
import type { PaginatedList } from '@/shared/types/common';

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'req-1' } }),
}));
vi.mock('@/modules/agency/shared/notes/api', () => ({
  getAgencyRequestNotes: vi.fn(),
  createAgencyRequestNote: vi.fn(),
  updateAgencyRequestNote: vi.fn(),
  deleteAgencyRequestNote: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/shared/notes/ModalNotes.vue', () => ({
  default: {
    name: 'ModalNotes',
    props: { userId: String, showClose: Boolean, onGet: Function, onCreate: Function, onUpdate: Function, onDelete: Function },
    emits: ['onUpdateNote'],
    template: '<div class="modal-notes-stub" />',
  },
}));

type Callback = (payload: Record<string, unknown>) => Promise<unknown>;

function page(items: NoteItem[]): PaginatedList<NoteItem> {
  return { items, pageIndex: 1, totalPages: 1, totalItems: items.length };
}

async function setup() {
  const wrapper = mountWithPlugins(RequestNotes);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

async function openModal(wrapper: Wrapper) {
  await wrapper.find('button').trigger('click');
  await nextTick();
  return wrapper.findComponent({ name: 'ModalNotes' });
}

afterEach(() => {
  vi.useRealTimers();
});

describe('RequestNotes', () => {
  it('shows the latest three notes', async () => {
    vi.useFakeTimers({ toFake: ['Date'] });
    vi.setSystemTime(new Date(2026, 10, 19, 12));
    vi.mocked(getAgencyRequestNotes).mockResolvedValue(page([
      { id: 'n-1', note: 'Call client', color: '#fefefe', createdBy: 'maria@agency.com', createdAt: '2026-11-17T12:00:00' },
      { id: 'n-2', note: 'No author' },
    ]));
    const wrapper = await setup();

    expect(getAgencyRequestNotes).toHaveBeenCalledExactlyOnceWith('req-1', { page: 1, size: 3 });
    const items = wrapper.findAll('li');
    expect(items).toHaveLength(2);
    expect(items[0].text()).toContain('Call client');
    expect(items[0].text()).toContain('By: maria |');
    expect(items[0].text()).toContain('2 days ago');
    expect(items[0].find('.note-color-icon').classes()).toContain('has-border');
    expect(items[1].find('i').exists()).toBe(false);
  });

  it('hides the list when there are no notes', async () => {
    vi.mocked(getAgencyRequestNotes).mockResolvedValue(page([]));
    const wrapper = await setup();

    expect(wrapper.find('ul').exists()).toBe(false);
  });

  it('shows the error when notes fail to load', async () => {
    const error = new Error('boom');
    vi.mocked(getAgencyRequestNotes).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('wires the notes modal to the request notes api', async () => {
    vi.mocked(getAgencyRequestNotes).mockResolvedValue(page([]));
    vi.mocked(createAgencyRequestNote).mockResolvedValue({ id: 'n-9', note: 'New note' });
    vi.mocked(updateAgencyRequestNote).mockResolvedValue();
    vi.mocked(deleteAgencyRequestNote).mockResolvedValue();
    const wrapper = await setup();
    const modal = await openModal(wrapper);
    expect(modal.props('userId')).toBe('req-1');
    expect(modal.props('showClose')).toBe(false);

    const pagination = { page: 2, size: 20 };
    const model = { note: 'x', color: '#fff' };
    await (modal.props('onGet') as Callback)({ userId: 'req-1', pagination });
    await (modal.props('onCreate') as Callback)({ userId: 'req-1', model });
    await (modal.props('onUpdate') as Callback)({ userId: 'req-1', id: 'n-1', model });
    await (modal.props('onDelete') as Callback)({ userId: 'req-1', id: 'n-1' });

    expect(getAgencyRequestNotes).toHaveBeenLastCalledWith('req-1', pagination);
    expect(createAgencyRequestNote).toHaveBeenCalledWith('req-1', model);
    expect(updateAgencyRequestNote).toHaveBeenCalledWith('req-1', 'n-1', model);
    expect(deleteAgencyRequestNote).toHaveBeenCalledWith('req-1', 'n-1');
  });

  it('reloads the notes after an update and when the modal closes', async () => {
    vi.mocked(getAgencyRequestNotes).mockResolvedValue(page([]));
    const wrapper = await setup();
    const modal = await openModal(wrapper);
    modal.vm.$emit('onUpdateNote', { size: 20 });
    await flushPromises();
    expect(getAgencyRequestNotes).toHaveBeenCalledTimes(2);

    wrapper.findComponent({ name: 'BModal' }).vm.$emit('close');
    await flushPromises();

    expect(getAgencyRequestNotes).toHaveBeenCalledTimes(3);
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });
});
