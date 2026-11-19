import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import CompanyRequestTimeSheetModal from '@/modules/company/requests/components/CompanyRequestTimeSheetModal.vue';
import { validateHoursTimeSheet } from '@/modules/company/requests/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PunchCardEditableDay } from '@/shared/punch-card/types';

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'request-1' } }),
}));
vi.mock('@/modules/company/requests/api', () => ({ validateHoursTimeSheet: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const day: PunchCardEditableDay = {
  id: 'ts-1',
  day: '2026-11-18T00:00:00',
  timeIn: '2026-11-18T08:00:00',
  clockIn: '2026-11-18T08:02:00',
  clockOut: '2026-11-18T16:31:00',
  totalHours: 8.5,
  totalHoursApproved: 8,
  hoursApprovedToDate: new Date(2026, 10, 18, 8, 0),
  missinghoursToDate: new Date(2026, 10, 18, 0, 30),
  missingHoursOvertimeToDate: new Date(2026, 10, 18, 0, 0),
};

function setup(editableDay: PunchCardEditableDay = day) {
  return mountWithPlugins(CompanyRequestTimeSheetModal, { props: { editableDay, worker: { workerProfileId: 'worker-profile-1' } } });
}

function report(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.container-worker-report .pl-2').map((cell) => cell.findAll('span')[1].text());
}

describe('CompanyRequestTimeSheetModal', () => {
  it('shows the worker report for the day', () => {
    const wrapper = setup();

    expect(wrapper.find('h2').text()).toBe('18-Nov-2026');
    expect(report(wrapper)).toEqual(['08:02', '16:31', '8.50']);
    expect(wrapper.find('.message').exists()).toBe(false);
  });

  it('asks for the hours when the worker did not clock out', () => {
    const wrapper = setup({ ...day, clockOut: null });

    expect(report(wrapper)).toEqual(['08:02', 'No reported', 'No reported']);
    expect(wrapper.find('.message.is-info').text()).toContain("The worker didn't clock out");
  });

  it('saves the approved and missing hours', async () => {
    vi.mocked(validateHoursTimeSheet).mockResolvedValue();
    const wrapper = setup();
    const [approved] = wrapper.findAllComponents({ name: 'BTimepicker' });
    approved.vm.$emit('update:modelValue', new Date(2026, 10, 18, 7, 45));
    await wrapper.find('button.is-primary').trigger('click');
    await flushPromises();

    expect(validateHoursTimeSheet).toHaveBeenCalledExactlyOnceWith('request-1', 'worker-profile-1', 'ts-1', {
      hours: '07:45:00',
      timeIn: '2026-11-18T00:00:00',
      missingHours: '00:30:00',
      missingHoursOvertime: '00:00:00',
    });
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
    expect(wrapper.emitted('updateData')).toHaveLength(1);
  });

  it('does nothing for a day without timesheet', async () => {
    const wrapper = setup({ ...day, id: null });
    await wrapper.find('button.is-primary').trigger('click');
    await flushPromises();

    expect(validateHoursTimeSheet).not.toHaveBeenCalled();
  });

  it('reloads the values when the day changes', async () => {
    const wrapper = setup();
    await wrapper.setProps({ editableDay: { ...day, day: '2026-11-19T00:00:00', clockIn: null } });

    expect(wrapper.find('h2').text()).toBe('19-Nov-2026');
    expect(report(wrapper)[0]).toBe('No reported');
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(validateHoursTimeSheet).mockRejectedValue(error);
    const wrapper = setup();
    await wrapper.find('button.is-primary').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
    expect(wrapper.emitted('updateData')).toBeUndefined();
  });
});
