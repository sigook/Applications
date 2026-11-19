import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import AgencyRequestTimeSheetModal from '@/modules/agency/recruiting/requests/components/AgencyRequestTimeSheetModal.vue';
import { updateAgencyWorkerTimeSheet } from '@/modules/agency/recruiting/requests/timeSheetApi';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PunchCardEditableDay } from '@/shared/punch-card/types';

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'req-1' } }),
}));
vi.mock('@/modules/agency/recruiting/requests/timeSheetApi', () => ({ updateAgencyWorkerTimeSheet: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

function editable(overrides: Partial<PunchCardEditableDay> = {}): PunchCardEditableDay {
  return {
    id: 'ts-1',
    day: '2026-11-18T00:00:00',
    clockIn: '2026-11-18T08:03:00',
    clockOut: '2026-11-18T16:31:00',
    clockInRounded: '2026-11-18T08:00:00',
    clockOutRounded: '2026-11-18T16:30:00',
    timeIn: '2026-11-18T08:00:00',
    totalHours: 8.5,
    totalHoursApproved: 8,
    hoursApprovedToDate: new Date(2026, 10, 18, 8, 0, 0),
    missinghoursToDate: new Date(2026, 10, 18, 1, 30, 0),
    missingHoursOvertimeToDate: new Date(2026, 10, 18, 0, 15, 0),
    missingRateWorker: 18,
    missingRateAgency: 25,
    deductionsOthers: 10,
    deductionsOthersDescription: 'Boots',
    bonusOrOthers: 20,
    bonusOrOthersDescription: 'Referral',
    reimbursements: 5,
    reimbursementsDescription: 'Parking',
    comment: 'Late start',
    ...overrides,
  };
}

function setup(day: PunchCardEditableDay = editable()) {
  return mountWithPlugins(AgencyRequestTimeSheetModal, { props: { editableDay: day, worker: { workerProfileId: 'wp-1' } } });
}

type Wrapper = ReturnType<typeof setup>;

function report(wrapper: Wrapper) {
  return wrapper.find('.container-worker-report').findAll('h3, span').map((s) => s.text()).join(' ');
}

function field(wrapper: Wrapper, label: string) {
  const found = wrapper.findAllComponents({ name: 'BField' }).find((f) => f.props('label') === label);
  if (!found) throw new Error(`${label} not found`);
  return found;
}

async function save(wrapper: Wrapper) {
  await wrapper.find('button.is-primary').trigger('click');
  await flushPromises();
}

describe('AgencyRequestTimeSheetModal', () => {
  it('shows the worker report with rounded times', () => {
    const wrapper = setup();

    expect(wrapper.find('h2').text()).toBe('18-Nov-2026');
    expect(report(wrapper)).toBe('Worker Report Clock In 08:03 Rounded 08:00 Clock Out 16:31 Rounded 16:30 Hours 8.50');
    expect(wrapper.find('.message').exists()).toBe(false);
  });

  it('warns when the worker did not clock out', () => {
    const wrapper = setup(editable({ clockOut: null, clockInRounded: null, clockOutRounded: null }));

    expect(report(wrapper)).toBe('Worker Report Clock In 08:03 Clock Out No reported Hours No reported');
    expect(wrapper.find('.message').text()).toContain("The worker didn't clock out");
  });

  it('marks a day without punches as not reported', () => {
    const wrapper = setup(editable({ clockIn: null, clockOut: null, clockInRounded: null, clockOutRounded: null }));

    expect(report(wrapper)).toBe('Worker Report Clock In No reported Clock Out No reported Hours No reported');
  });

  it('prefills the form from the day', () => {
    const wrapper = setup();
    const inputs = wrapper.findAll('input:not([readonly])').map((i) => (i.element as HTMLInputElement).value);

    expect(inputs).toEqual(expect.arrayContaining(['18', '25', '10', 'Boots', '20', 'Referral', '5', 'Parking']));
    expect((wrapper.find('textarea').element as HTMLTextAreaElement).value).toBe('Late start');
  });

  it('saves the approved values and emits the updated day', async () => {
    vi.mocked(updateAgencyWorkerTimeSheet).mockResolvedValue();
    const wrapper = setup();
    wrapper.findAllComponents({ name: 'BTimepicker' })[0].vm.$emit('update:modelValue', new Date(2026, 10, 18, 7, 45, 0));
    await nextTick();
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateData')).toHaveLength(1); });
    expect(updateAgencyWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('req-1', 'wp-1', 'ts-1', {
      hours: '07:45:00',
      timeIn: '2026-11-18T00:00:00',
      missingHours: '01:30:00',
      missingHoursOvertime: '00:15:00',
      deductionsOthers: 10,
      deductionsOthersDescription: 'Boots',
      bonusOrOthers: 20,
      bonusOrOthersDescription: 'Referral',
      comments: 'Late start',
      missingRateAgency: 25,
      missingRateWorker: 18,
      reimbursements: 5,
      reimbursementsDescription: 'Parking',
    });
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
    expect(wrapper.emitted('updateData')).toHaveLength(1);
    expect(wrapper.emitted<[PunchCardEditableDay]>('update:editableDay')?.[0][0]).toEqual(expect.objectContaining({
      id: 'ts-1',
      hoursApprovedToDate: new Date(2026, 10, 18, 7, 45, 0),
      comment: 'Late start',
    }));
  });

  it('sends zero for empty amounts', async () => {
    vi.mocked(updateAgencyWorkerTimeSheet).mockResolvedValue();
    const wrapper = setup(editable({ missingRateWorker: undefined, missingRateAgency: undefined, deductionsOthers: undefined, bonusOrOthers: undefined, reimbursements: undefined, bonusOrOthersDescription: undefined, reimbursementsDescription: undefined }));
    await save(wrapper);

    await vi.waitFor(() => { expect(updateAgencyWorkerTimeSheet).toHaveBeenCalled(); });
    expect(updateAgencyWorkerTimeSheet).toHaveBeenCalledWith('req-1', 'wp-1', 'ts-1', expect.objectContaining({
      deductionsOthers: 0,
      bonusOrOthers: 0,
      bonusOrOthersDescription: '',
      missingRateAgency: 0,
      missingRateWorker: 0,
      reimbursements: 0,
      reimbursementsDescription: '',
    }));
  });

  it('sends zero missing hours when they are empty', async () => {
    vi.mocked(updateAgencyWorkerTimeSheet).mockResolvedValue();
    const wrapper = setup(editable({ missinghoursToDate: null, missingHoursOvertimeToDate: null }));
    await save(wrapper);

    await vi.waitFor(() => { expect(updateAgencyWorkerTimeSheet).toHaveBeenCalled(); });
    expect(updateAgencyWorkerTimeSheet).toHaveBeenCalledWith('req-1', 'wp-1', 'ts-1', expect.objectContaining({
      missingHours: '00:00:00',
      missingHoursOvertime: '00:00:00',
    }));
  });

  it('shows each rounded time only when it exists', () => {
    const wrapper = setup(editable({ clockInRounded: null }));

    expect(report(wrapper)).toBe('Worker Report Clock In 08:03 Clock Out 16:31 Rounded 16:30 Hours 8.50');
  });

  it('hides the clock out rounded time when it is missing', () => {
    const wrapper = setup(editable({ clockOutRounded: null }));

    expect(report(wrapper)).toBe('Worker Report Clock In 08:03 Rounded 08:00 Clock Out 16:31 Hours 8.50');
  });

  it('requires the approved hours', async () => {
    const wrapper = setup(editable({ hoursApprovedToDate: null }));
    await save(wrapper);

    await vi.waitFor(() => { expect(field(wrapper, 'Hours Approved').props('message')).toBe('Hours Approved is required'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(updateAgencyWorkerTimeSheet).not.toHaveBeenCalled();
  });

  it('rejects out of range amounts', async () => {
    const wrapper = setup(editable({ missingRateWorker: 101, deductionsOthers: -1 }));
    await save(wrapper);

    await vi.waitFor(() => { expect(field(wrapper, 'Missing Worker Rate').props('message')).toBe('Maximum is 100'); });
    expect(field(wrapper, 'Deductions Others').props('message')).toBe('Minimum is 0');
    expect(updateAgencyWorkerTimeSheet).not.toHaveBeenCalled();
  });

  it('does not save a day without a time sheet', async () => {
    const wrapper = setup(editable({ id: null }));
    await save(wrapper);
    await new Promise((resolve) => { setTimeout(resolve, 50); });

    expect(updateAgencyWorkerTimeSheet).not.toHaveBeenCalled();
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('boom');
    vi.mocked(updateAgencyWorkerTimeSheet).mockRejectedValue(error);
    const wrapper = setup();
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updateData')).toBeUndefined();
  });

  it('reloads the form when the day changes', async () => {
    const wrapper = setup();
    await wrapper.setProps({ editableDay: editable({ day: '2026-11-20T00:00:00', comment: 'Changed' }) });
    await nextTick();

    expect(wrapper.find('h2').text()).toBe('20-Nov-2026');
    expect((wrapper.find('textarea').element as HTMLTextAreaElement).value).toBe('Changed');
  });
});
