import { describe, expect, it, vi } from 'vitest';
import EditTextarea from '@/shared/ui/EditTextarea.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

function setup(data?: string) {
  return mountWithPlugins(EditTextarea, { props: { title: 'Description', minLength: 5, data } });
}

describe('EditTextarea', () => {
  it('starts with the given content', () => {
    expect((setup('Hello world').find('textarea').element as HTMLTextAreaElement).value).toBe('Hello world');
  });

  it('emits the edited content on save', async () => {
    const wrapper = setup('Hello');
    await wrapper.find('textarea').setValue('Updated description');
    await wrapper.find('button').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('updateContent')).toEqual([['Updated description']]); });
  });

  it('blocks saving content shorter than the minimum', async () => {
    const wrapper = setup();
    await wrapper.find('textarea').setValue('abc');
    await wrapper.find('button').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalled(); });
    expect(wrapper.emitted('updateContent')).toBeUndefined();
    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Min 5 characters'); });
  });

  it('requires content', async () => {
    const wrapper = setup();
    await wrapper.find('button').trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Description is required'); });
    expect(wrapper.emitted('updateContent')).toBeUndefined();
  });
});
