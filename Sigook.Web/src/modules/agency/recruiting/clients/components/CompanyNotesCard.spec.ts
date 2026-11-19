import { afterEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import CompanyNotesCard from '@/modules/agency/recruiting/clients/components/CompanyNotesCard.vue';
import { createAgencyCompanyNote, deleteAgencyCompanyNote, getAgencyCompanyNotes, updateAgencyCompanyNote } from '@/modules/agency/shared/notes/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { NoteItem, NotesCreatePayload, NotesDeletePayload, NotesFetchPayload, NotesUpdatePayload } from '@/modules/agency/shared/notes/types';
import type { PaginatedList } from '@/shared/types/common';

vi.mock('@/modules/agency/shared/notes/api', () => ({
  getAgencyCompanyNotes: vi.fn(),
  createAgencyCompanyNote: vi.fn(),
  updateAgencyCompanyNote: vi.fn(),
  deleteAgencyCompanyNote: vi.fn(),
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

interface ModalNotesProps {
  userId: string;
  showClose: boolean;
  onGet: (payload: NotesFetchPayload) => Promise<unknown>;
  onCreate: (payload: NotesCreatePayload) => Promise<unknown>;
  onUpdate: (payload: NotesUpdatePayload) => Promise<unknown>;
  onDelete: (payload: NotesDeletePayload) => Promise<unknown>;
}

function page(items: NoteItem[], totalItems = items.length): PaginatedList<NoteItem> {
  return { items, totalItems, pageIndex: 1, totalPages: 1 };
}

async function setup(createdAt?: string) {
  const wrapper = mountWithPlugins(CompanyNotesCard, { props: { profileId: 'company-1', createdAt } });
  await flushPromises();
  return wrapper;
}

function addButton(wrapper: Awaited<ReturnType<typeof setup>>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Add note');
  if (!button) throw new Error('Add note not found');
  return button;
}

afterEach(() => {
  vi.useRealTimers();
});

describe('CompanyNotesCard', () => {
  it('shows an empty state', async () => {
    vi.mocked(getAgencyCompanyNotes).mockResolvedValue(page([]));
    const wrapper = await setup();

    expect(getAgencyCompanyNotes).toHaveBeenCalledExactlyOnceWith('company-1', { page: 1, size: 3 });
    expect(wrapper.text()).toContain('No notes');
    expect(wrapper.text()).not.toContain('Created');
  });

  it('lists the latest notes with their author and a link to all of them', async () => {
    vi.useFakeTimers({ now: new Date('2026-06-10T12:00:00Z'), toFake: ['Date'] });
    vi.mocked(getAgencyCompanyNotes).mockResolvedValue(page([
      { id: 'n1', note: 'Call back Monday', color: '#fefefe', createdAt: '2026-06-08T12:00:00Z', createdBy: 'jane.doe@sigook.com' },
      { id: 'n2', note: 'Prefers email', color: '#ffcc00' },
    ], 7));
    const wrapper = await setup('Mar 10, 2026');

    const items = wrapper.findAll('.detail-list-item');
    expect(items.map((i) => i.find('.company-note-text').text())).toEqual(['Call back Monday', 'Prefers email']);
    expect(items[0].find('.company-note-meta').text()).toContain('2 days ago');
    expect(items[0].find('.company-note-meta').text()).toContain('·');
    expect(items[0].find('.company-note-color').classes()).toContain('has-border');
    expect(items[1].find('.company-note-color').classes()).not.toContain('has-border');
    expect(items[1].find('.company-note-meta').text()).toBe('');
    expect(wrapper.text()).toContain('See all 7 notes');
    expect(wrapper.find('.company-notes-created').text()).toBe('Created Mar 10, 2026');
  });

  it('opens the notes modal wired to the company notes api', async () => {
    vi.mocked(getAgencyCompanyNotes).mockResolvedValue(page([]));
    vi.mocked(createAgencyCompanyNote).mockResolvedValue({ id: 'n9', note: 'New' });
    vi.mocked(updateAgencyCompanyNote).mockResolvedValue();
    vi.mocked(deleteAgencyCompanyNote).mockResolvedValue();
    const wrapper = await setup();
    await addButton(wrapper).trigger('click');

    const modal = wrapper.findComponent({ name: 'ModalNotes' });
    const props = modal.props() as ModalNotesProps;
    expect(props.userId).toBe('company-1');
    expect(props.showClose).toBe(false);

    const pagination = { page: 2, size: 10 };
    await props.onGet({ userId: 'company-1', pagination });
    await props.onCreate({ userId: 'company-1', model: { note: 'New' } });
    await props.onUpdate({ userId: 'company-1', id: 'n1', model: { note: 'Edited' } });
    await props.onDelete({ userId: 'company-1', id: 'n1' });

    expect(getAgencyCompanyNotes).toHaveBeenLastCalledWith('company-1', pagination);
    expect(createAgencyCompanyNote).toHaveBeenCalledWith('company-1', { note: 'New' });
    expect(updateAgencyCompanyNote).toHaveBeenCalledWith('company-1', 'n1', { note: 'Edited' });
    expect(deleteAgencyCompanyNote).toHaveBeenCalledWith('company-1', 'n1');
  });

  it('reloads the latest notes when a note changes', async () => {
    vi.mocked(getAgencyCompanyNotes).mockResolvedValue(page([]));
    const wrapper = await setup();
    await addButton(wrapper).trigger('click');
    vi.mocked(getAgencyCompanyNotes).mockResolvedValue(page([{ id: 'n1', note: 'Fresh' }]));

    wrapper.findComponent({ name: 'ModalNotes' }).vm.$emit('onUpdateNote', { size: 1 });
    await flushPromises();

    expect(getAgencyCompanyNotes).toHaveBeenCalledTimes(2);
    expect(wrapper.find('.company-note-text').text()).toBe('Fresh');
  });

  it('shows the error when the notes cannot be loaded', async () => {
    const error = new Error('load failed');
    vi.mocked(getAgencyCompanyNotes).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
