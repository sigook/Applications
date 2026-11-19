import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import RunnerStatusModal from '@/modules/agency/recruiting/requests/runners/RunnerStatusModal.vue';
import { changeRunnerStatus } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerStatus } from '@/modules/agency/recruiting/requests/runners/types';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ changeRunnerStatus: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

function setup(currentStatus = RunnerStatus.SentToClient) {
  return mountWithPlugins(RunnerStatusModal, { props: { requestId: 'req-1', runnerId: 'run-1', currentStatus } });
}

type Wrapper = ReturnType<typeof setup>;

function options(wrapper: Wrapper) {
  return wrapper.findAll('select option').map((o) => o.text());
}

function submitButton(wrapper: Wrapper) {
  return wrapper.find('footer button.is-primary');
}

async function chooseStatus(wrapper: Wrapper, status: RunnerStatus) {
  await wrapper.find('select').setValue(String(status));
  await nextTick();
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
}

describe('RunnerStatusModal', () => {
  it('lists the selectable statuses without the automatic rescheduled one', () => {
    const wrapper = setup();

    expect(options(wrapper)).not.toContain('Interview rescheduled');
    expect(options(wrapper)).toHaveLength(9);
    expect(submitButton(wrapper).attributes('disabled')).toBeDefined();
    expect(wrapper.text()).not.toContain('Start date');
  });

  it('keeps the rescheduled status when it is the current one', () => {
    const wrapper = setup(RunnerStatus.InterviewRescheduled);

    expect(options(wrapper)).toContain('Interview rescheduled');
  });

  it('changes the status with comments', async () => {
    vi.mocked(changeRunnerStatus).mockResolvedValue();
    const wrapper = setup();
    await chooseStatus(wrapper, RunnerStatus.NoShow);
    await wrapper.find('textarea').setValue('Did not come');
    expect(submitButton(wrapper).attributes('disabled')).toBeUndefined();
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('close')).toHaveLength(1); });
    expect(changeRunnerStatus).toHaveBeenCalledExactlyOnceWith('req-1', 'run-1', { status: RunnerStatus.NoShow, comments: 'Did not come', startDate: undefined });
    expect(showAlertSuccess).toHaveBeenCalledWith('Status updated');
    expect(wrapper.emitted('updated')).toHaveLength(1);
  });

  it('requires a start date to hire', async () => {
    const wrapper = setup();
    await chooseStatus(wrapper, RunnerStatus.Hired);
    expect(wrapper.text()).toContain('Start date');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Start date is required to hire a runner'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(changeRunnerStatus).not.toHaveBeenCalled();
  });

  it('hires with the chosen start date', async () => {
    vi.mocked(changeRunnerStatus).mockResolvedValue();
    const wrapper = setup();
    await chooseStatus(wrapper, RunnerStatus.Hired);
    const date = new Date(2026, 11, 1);
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', date);
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(changeRunnerStatus).toHaveBeenCalled(); });
    expect(changeRunnerStatus).toHaveBeenCalledWith('req-1', 'run-1', { status: RunnerStatus.Hired, comments: undefined, startDate: date.toISOString() });
  });

  it('shows the error and stays open when the update fails', async () => {
    const error = new Error('boom');
    vi.mocked(changeRunnerStatus).mockRejectedValue(error);
    const wrapper = setup();
    await chooseStatus(wrapper, RunnerStatus.Rejected);
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('close')).toBeUndefined();
    expect(wrapper.emitted('updated')).toBeUndefined();
  });

  it('closes on cancel', async () => {
    const wrapper = setup();
    await wrapper.find('footer button').trigger('click');

    expect(wrapper.emitted('close')).toHaveLength(1);
  });
});
