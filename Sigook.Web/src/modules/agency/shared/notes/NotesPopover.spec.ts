import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import NotesPopover from '@/modules/agency/shared/notes/NotesPopover.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PaginatedList } from '@/shared/types/common';
import type { NoteItem, RequestNotesFetchPayload } from '@/modules/agency/shared/notes/types';

const { modalNotesStub } = vi.hoisted(() => ({
  modalNotesStub: {
    name: 'ModalNotes',
    props: { canCreate: Boolean, userId: String, requestId: String, onGet: Function, onCreate: Function, onUpdate: Function, onDelete: Function },
    emits: ['onUpdateNote', 'close'],
    template: '<div class="modal-notes"><button class="inside">inside</button></div>',
  },
}));

vi.mock('@/modules/agency/shared/notes/ModalNotes.vue', () => ({ default: modalNotesStub }));

const onGet = vi.fn<(payload: RequestNotesFetchPayload) => Promise<PaginatedList<NoteItem>>>();

function setup(notesCount?: number) {
  return mountWithPlugins(NotesPopover, { props: { userId: 'u1', requestId: 'r1', notesCount, onGet } });
}

function popover() {
  return document.body.querySelector<HTMLElement>('.notes-tooltip');
}

async function toggle(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('.notes-popover-trigger').trigger('click');
  await nextTick();
  await nextTick();
}

describe('NotesPopover', () => {
  it('shows the notes count in the badge', () => {
    expect(setup(4).find('.notes-popover-trigger').text()).toContain('4');
    expect(setup(0).find('.notes-popover-trigger').text()).not.toContain('0');
  });

  it('opens the notes for the user and closes on a second click', async () => {
    const wrapper = setup(1);
    await toggle(wrapper);
    expect(popover()).not.toBeNull();
    expect(popover()?.style.position).toBe('fixed');
    expect(wrapper.findComponent(modalNotesStub).props()).toMatchObject({ userId: 'u1', requestId: 'r1', onGet });

    await toggle(wrapper);
    expect(popover()).toBeNull();
  });

  it('forwards the new count', async () => {
    const wrapper = setup(1);
    await toggle(wrapper);
    wrapper.findComponent(modalNotesStub).vm.$emit('onUpdateNote', { size: 2 });
    expect(wrapper.emitted('update:count')).toEqual([[2]]);
  });

  it('closes from the notes panel', async () => {
    const wrapper = setup();
    await toggle(wrapper);
    wrapper.findComponent(modalNotesStub).vm.$emit('close');
    await nextTick();
    expect(popover()).toBeNull();
  });

  it('closes on an outside click but not on an inside one', async () => {
    const wrapper = setup();
    await toggle(wrapper);
    document.body.querySelector('.inside')?.dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));
    await nextTick();
    expect(popover()).not.toBeNull();

    document.body.dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));
    await nextTick();
    expect(popover()).toBeNull();
  });

  it('repositions on scroll and hides when the trigger leaves its scroll container', async () => {
    const container = document.createElement('div');
    container.style.overflowY = 'auto';
    document.body.appendChild(container);
    const wrapper = mountWithPlugins(NotesPopover, { props: { userId: 'u1', onGet }, attachTo: container });
    const trigger = wrapper.find('.notes-popover-trigger').element;
    vi.spyOn(container, 'getBoundingClientRect').mockReturnValue(new DOMRect(0, 100, 500, 300));
    const rect = vi.spyOn(trigger, 'getBoundingClientRect').mockReturnValue(new DOMRect(400, 150, 20, 20));

    await toggle(wrapper);
    expect(popover()?.style.left).toBe('120px');
    expect(popover()?.style.top).toBe('175px');

    rect.mockReturnValue(new DOMRect(400, 10, 20, 20));
    window.dispatchEvent(new Event('scroll'));
    await nextTick();
    expect(popover()?.style.display).toBe('none');
  });

  it('opens above the trigger near the bottom of the window', async () => {
    const wrapper = setup();
    vi.spyOn(wrapper.find('.notes-popover-trigger').element, 'getBoundingClientRect')
      .mockReturnValue(new DOMRect(100, window.innerHeight - 30, 20, 20));
    await toggle(wrapper);
    expect(popover()?.style.left).toBe('10px');
    expect(popover()?.style.top).toBe(`${Math.max(10, window.innerHeight - 30 - window.innerHeight * 0.5)}px`);
  });
});
