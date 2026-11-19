import { describe, expect, it, vi } from 'vitest';
import DialogRequestWorker from '@/modules/company/requests/components/DialogRequestWorker.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

describe('DialogRequestWorker', () => {
  it('sends the comment', async () => {
    const wrapper = mountWithPlugins(DialogRequestWorker);
    await wrapper.find('textarea').setValue('Need one more forklift operator');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('sendAnotherWorker')).toEqual([['Need one more forklift operator']]); });
    expect(showAlertError).not.toHaveBeenCalled();
  });

  it('requires a comment', async () => {
    const wrapper = mountWithPlugins(DialogRequestWorker);
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Comment is required'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(wrapper.emitted('sendAnotherWorker')).toBeUndefined();
  });
});
