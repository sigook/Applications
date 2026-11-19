import { describe, expect, it, vi } from 'vitest';
import CompanyCancelList from '@/shared/request-detail/CompanyCancelList.vue';
import { getReasonCancellationRequest } from '@/shared/api/catalogApi';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/api/catalogApi', () => ({ getReasonCancellationRequest: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const reasons = [
  { id: 'r1', value: 'No longer needed' },
  { id: 'r2', value: 'Found other staff' },
];

async function setup() {
  vi.mocked(getReasonCancellationRequest).mockResolvedValue(reasons);
  const wrapper = mountWithPlugins(CompanyCancelList);
  await vi.waitFor(() => { expect(wrapper.findAll('.cancellation-list .item')).toHaveLength(2); });
  return wrapper;
}

async function send(wrapper: Awaited<ReturnType<typeof setup>>) {
  await wrapper.find('button.is-primary').trigger('click');
}

describe('CompanyCancelList', () => {
  it('shows the error and stops loading when the reasons fail to load', async () => {
    const error = new Error('catalog down');
    vi.mocked(getReasonCancellationRequest).mockRejectedValue(error);
    const wrapper = mountWithPlugins(CompanyCancelList);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    await vi.waitFor(() => { expect(wrapper.find('.loading-overlay').exists()).toBe(false); });
    expect(wrapper.findAll('.item')).toHaveLength(0);
  });

  it('lists the cancellation reasons', async () => {
    const wrapper = await setup();

    expect(wrapper.findAll('.item').map((i) => i.text())).toEqual(['No longer needed', 'Found other staff']);
    expect(wrapper.find('.message.is-warning').text()).toContain('all workers in the order will be rejected');
    expect(wrapper.find('textarea').exists()).toBe(false);
  });

  it('asks for a reason when none is selected', async () => {
    const wrapper = await setup();
    await send(wrapper);

    expect(showAlertError).toHaveBeenCalledWith('Please select a cancellation reason');
    expect(wrapper.emitted('sendReason')).toBeUndefined();
  });

  it('marks the selected reason and requires an explanation', async () => {
    const wrapper = await setup();
    await wrapper.findAll('.item')[1].trigger('click');

    expect(wrapper.findAll('.item')[1].classes()).toContain('selected');
    await send(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Reason is required'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(wrapper.emitted('sendReason')).toBeUndefined();
  });

  it('sends the selected reason with the explanation', async () => {
    const wrapper = await setup();
    await wrapper.findAll('.item')[0].trigger('click');
    await wrapper.find('textarea').setValue('Project cancelled');
    await send(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('sendReason')).toEqual([[{ reasonId: 'r1', otherMessage: 'Project cancelled' }]]); });
  });
});
