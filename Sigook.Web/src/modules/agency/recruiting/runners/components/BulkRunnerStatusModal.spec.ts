import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import BulkRunnerStatusModal from '@/modules/agency/recruiting/runners/components/BulkRunnerStatusModal.vue';
import { RUNNER_STATUS_LABELS, RunnerStatus } from '@/modules/agency/recruiting/requests/runners/types';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

function setup(isSaving = false) {
  return mountWithPlugins(BulkRunnerStatusModal, { props: { count: 3, isSaving } });
}

type Wrapper = ReturnType<typeof setup>;

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
  await flushPromises();
}

describe('BulkRunnerStatusModal', () => {
  it('shows the number of runners and every status but the automatic one', () => {
    const wrapper = setup();

    expect(wrapper.find('.modal-card-title').text()).toBe('Change status · 3 runners');
    expect(wrapper.find('button[type="submit"]').text()).toBe('Update 3 runners');
    const options = wrapper.findAll('select option').map((o) => o.text());
    expect(options).toContain(RUNNER_STATUS_LABELS[RunnerStatus.Hired]);
    expect(options).not.toContain(RUNNER_STATUS_LABELS[RunnerStatus.InterviewRescheduled]);
  });

  it('requires a status', async () => {
    const wrapper = setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(wrapper.find('.help.is-danger').text()).toBe('Select a status');
    expect(wrapper.emitted('submit')).toBeUndefined();
  });

  it('emits the status with its comments', async () => {
    const wrapper = setup();
    await wrapper.find('select').setValue(String(RunnerStatus.Rejected));
    await wrapper.find('textarea').setValue('Not a fit');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('submit')).toEqual([[{ status: RunnerStatus.Rejected, comments: 'Not a fit', startDate: undefined }]]); });
    expect(wrapper.findComponent({ name: 'BDatepicker' }).exists()).toBe(false);
  });

  it('leaves out empty comments', async () => {
    const wrapper = setup();
    await wrapper.find('select').setValue(String(RunnerStatus.NoShow));
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('submit')).toEqual([[{ status: RunnerStatus.NoShow, comments: undefined, startDate: undefined }]]); });
  });

  it('requires the start date to hire', async () => {
    const wrapper = setup();
    await wrapper.find('select').setValue(String(RunnerStatus.Hired));
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Start date is required to hire a runner'); });
    expect(wrapper.emitted('submit')).toBeUndefined();
  });

  it('emits the start date when hiring', async () => {
    const wrapper = setup();
    await wrapper.find('select').setValue(String(RunnerStatus.Hired));
    const start = new Date('2026-04-06T00:00:00');
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', start);
    await flushPromises();
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('submit')).toEqual([[{ status: RunnerStatus.Hired, comments: undefined, startDate: start.toISOString() }]]); });
  });

  it('closes on cancel', async () => {
    const wrapper = setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Cancel')?.trigger('click');

    expect(wrapper.emitted('close')).toHaveLength(1);
  });
});
