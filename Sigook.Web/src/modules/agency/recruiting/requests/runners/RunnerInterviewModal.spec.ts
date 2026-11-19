import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import RunnerInterviewModal from '@/modules/agency/recruiting/requests/runners/RunnerInterviewModal.vue';
import { createRunnerInterview } from '@/modules/agency/recruiting/requests/runners/api';
import { InterviewType } from '@/modules/agency/recruiting/requests/runners/types';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ createRunnerInterview: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

function setup() {
  return mountWithPlugins(RunnerInterviewModal, { props: { requestId: 'req-1', runnerId: 'run-1' } });
}

type Wrapper = ReturnType<typeof setup>;

async function pickDate(wrapper: Wrapper, date: Date) {
  wrapper.findComponent({ name: 'BDatetimepicker' }).vm.$emit('update:modelValue', date);
  await nextTick();
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
}

describe('RunnerInterviewModal', () => {
  it('lists the interview types with phone selected', () => {
    const wrapper = setup();

    expect(wrapper.findAll('select option').map((o) => o.text())).toEqual(['Phone', 'Video', 'Onsite']);
    expect((wrapper.find('select').element as HTMLSelectElement).value).toBe(String(InterviewType.Phone));
  });

  it('requires the scheduled date', async () => {
    const wrapper = setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Scheduled date is required'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createRunnerInterview).not.toHaveBeenCalled();
  });

  it('creates the interview with optional fields', async () => {
    vi.mocked(createRunnerInterview).mockResolvedValue('int-1');
    const wrapper = setup();
    const date = new Date(2026, 11, 2, 10, 30);
    await pickDate(wrapper, date);
    await wrapper.find('select').setValue(String(InterviewType.Onsite));
    await wrapper.find('input[placeholder="Interviewer name"]').setValue('Maria');
    await wrapper.find('textarea').setValue('Bring ID');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('close')).toHaveLength(1); });
    expect(createRunnerInterview).toHaveBeenCalledExactlyOnceWith('req-1', 'run-1', {
      scheduledDate: date.toISOString(),
      type: InterviewType.Onsite,
      interviewer: 'Maria',
      notes: 'Bring ID',
    });
    expect(showAlertSuccess).toHaveBeenCalledWith('Interview added');
    expect(wrapper.emitted('updated')).toHaveLength(1);
  });

  it('sends empty optional fields as undefined', async () => {
    vi.mocked(createRunnerInterview).mockResolvedValue('int-1');
    const wrapper = setup();
    const date = new Date(2026, 11, 2, 10, 30);
    await pickDate(wrapper, date);
    await submit(wrapper);

    await vi.waitFor(() => { expect(createRunnerInterview).toHaveBeenCalled(); });
    expect(createRunnerInterview).toHaveBeenCalledWith('req-1', 'run-1', {
      scheduledDate: date.toISOString(),
      type: InterviewType.Phone,
      interviewer: undefined,
      notes: undefined,
    });
  });

  it('shows the error and stays open when creation fails', async () => {
    const error = new Error('boom');
    vi.mocked(createRunnerInterview).mockRejectedValue(error);
    const wrapper = setup();
    await pickDate(wrapper, new Date(2026, 11, 2, 10, 30));
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('close')).toBeUndefined();
  });

  it('closes on cancel', async () => {
    const wrapper = setup();
    await wrapper.find('footer button').trigger('click');

    expect(wrapper.emitted('close')).toHaveLength(1);
  });
});
