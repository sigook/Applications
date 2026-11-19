import { describe, expect, it, vi } from 'vitest';
import DialogWorkerComment from '@/shared/worker-profile/forms/DialogWorkerComment.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

function setup() {
  return mountWithPlugins(DialogWorkerComment);
}

function commentButton(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Comment');
  if (!button) throw new Error('Comment button not found');
  return button;
}

describe('DialogWorkerComment', () => {
  it('requires a comment', async () => {
    const wrapper = setup();
    await commentButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Comment is required'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(wrapper.emitted('createComment')).toBeUndefined();
  });

  it('rejects a comment shorter than 2 characters', async () => {
    const wrapper = setup();
    await wrapper.find('textarea').setValue('a');
    await commentButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Min 2 characters'); });
    expect(wrapper.emitted('createComment')).toBeUndefined();
  });

  it('rejects a comment longer than 500 characters', async () => {
    const wrapper = setup();
    await wrapper.find('textarea').setValue('a'.repeat(501));
    await commentButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Max 500 characters'); });
    expect(wrapper.emitted('createComment')).toBeUndefined();
  });

  it('emits the comment with the default rating of 1', async () => {
    const wrapper = setup();
    await wrapper.find('textarea').setValue('Great worker');
    await commentButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('createComment')).toEqual([[{ comment: 'Great worker', rating: 1 }]]); });
  });

  it('emits the chosen rating', async () => {
    const wrapper = setup();
    await wrapper.findAll('.rate .rate-item')[3].trigger('click');
    await wrapper.find('textarea').setValue('Very reliable');
    await commentButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('createComment')).toEqual([[{ comment: 'Very reliable', rating: 4 }]]); });
  });
});
