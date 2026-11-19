import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import NoteForm from '@/modules/agency/shared/notes/NoteForm.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { NoteFormModel } from '@/modules/agency/shared/notes/types';

const draftKey = 'sigook_current_note_editing';

function setup(currentNote?: NoteFormModel) {
  return mountWithPlugins(NoteForm, { props: { currentNote } });
}

function save(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save not found');
  return button.trigger('click');
}

function textarea(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('textarea');
}

beforeEach(() => {
  localStorage.clear();
});

afterEach(() => {
  vi.useRealTimers();
});

describe('NoteForm', () => {
  it('does not save an empty note', async () => {
    const wrapper = setup();
    await save(wrapper);
    expect(wrapper.emitted('onSave')).toBeUndefined();
  });

  it('saves the note with the default color and resets the draft', async () => {
    vi.useFakeTimers();
    const wrapper = setup();
    await textarea(wrapper).setValue('Call back tomorrow');
    expect(localStorage.getItem(draftKey)).toBe('Call back tomorrow');
    await save(wrapper);

    expect(wrapper.emitted('onSave')).toEqual([[{ id: undefined, color: '#fefefe', note: 'Call back tomorrow', createdAt: undefined, createdBy: undefined }]]);
    vi.advanceTimersByTime(200);
    await wrapper.vm.$nextTick();
    expect((textarea(wrapper).element as HTMLTextAreaElement).value).toBe('');
    expect(localStorage.getItem(draftKey)).toBeNull();
  });

  it('saves on Enter', async () => {
    const wrapper = setup();
    await textarea(wrapper).setValue('Quick note');
    await wrapper.find('.form-notes').trigger('keydown', { key: 'Enter' });
    expect(wrapper.emitted('onSave')?.[0][0]).toMatchObject({ note: 'Quick note' });
  });

  it('ignores other keys', async () => {
    const wrapper = setup();
    await textarea(wrapper).setValue('Quick note');
    await wrapper.find('.form-notes').trigger('keydown', { key: 'a' });
    expect(wrapper.emitted('onSave')).toBeUndefined();
  });

  it('restores an unsaved draft', () => {
    localStorage.setItem(draftKey, 'Half written');
    expect((textarea(setup()).element as HTMLTextAreaElement).value).toBe('Half written');
  });

  it('edits an existing note keeping its id and author', async () => {
    const wrapper = setup({ id: 'n1', note: 'Old', color: '#cbf0f9', createdAt: '2026-01-01', createdBy: 'ana@x.com' });
    expect((textarea(wrapper).element as HTMLTextAreaElement).value).toBe('Old');
    await textarea(wrapper).setValue('New');
    await save(wrapper);
    expect(wrapper.emitted('onSave')).toEqual([[{ id: 'n1', note: 'New', color: '#cbf0f9', createdAt: '2026-01-01', createdBy: 'ana@x.com' }]]);
  });

  it('falls back to the default color for a note without one', async () => {
    const wrapper = setup({ id: 'n1', note: 'Old' });
    await save(wrapper);
    expect(wrapper.emitted('onSave')?.[0][0]).toMatchObject({ color: '#fefefe' });
  });

  it('takes the color from the picker', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'ColorPicker' }).vm.$emit('update:modelValue', '#aecbfa');
    await textarea(wrapper).setValue('Blue');
    await save(wrapper);
    expect(wrapper.emitted('onSave')?.[0][0]).toMatchObject({ color: '#aecbfa' });
  });
});
