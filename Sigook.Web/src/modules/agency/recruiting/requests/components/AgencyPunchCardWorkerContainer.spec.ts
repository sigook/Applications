import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import AgencyPunchCardWorkerContainer from '@/modules/agency/recruiting/requests/components/AgencyPunchCardWorkerContainer.vue';
import {
  deleteAgencyWorkerTimeSheet,
  getAgencyTimeSheetUsages,
  getAgencyWorkerTimeSheetByDate,
  postAgencyWorkerTimeSheet,
  updateAgencyWorkerTimeSheet,
} from '@/modules/agency/recruiting/requests/timeSheetApi';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestDetail, AgencyRequestWorker } from '@/modules/agency/recruiting/requests/types';
import type { PunchCardEditableDay, TimeSheetListItem } from '@/shared/punch-card/types';

const admin = ref(true);

vi.mock('@/shared/composables/useAdmin', () => ({ useAdmin: () => ({ isAdmin: computed(() => admin.value) }) }));
vi.mock('@/modules/agency/recruiting/requests/timeSheetApi', () => ({
  getAgencyWorkerTimeSheetByDate: vi.fn(),
  postAgencyWorkerTimeSheet: vi.fn(),
  deleteAgencyWorkerTimeSheet: vi.fn(),
  getAgencyTimeSheetUsages: vi.fn(),
  updateAgencyWorkerTimeSheet: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/punch-card/CalendarPunchCard.vue', () => ({
  default: {
    name: 'CalendarPunchCard',
    props: { highlights: Array, workerProfileId: String, requestId: String, startDate: String, status: Number, worker: Object },
    emits: ['onMonthChange'],
    template: '<div class="calendar-stub"><div v-for="(h, i) in highlights" :key="i" class="day"><slot name="punch-input" :item="h" :index="i" /></div></div>',
  },
}));
vi.mock('@/modules/agency/recruiting/requests/components/AgencyRequestTimeSheetModal.vue', () => ({
  default: {
    name: 'AgencyRequestTimeSheetModal',
    props: { worker: Object, editableDay: Object },
    emits: ['updateData', 'update:editableDay'],
    template: '<div class="timesheet-modal-stub" />',
  },
}));

const request = { startAt: '2026-11-01T00:00:00', status: 1 } as Pick<AgencyRequestDetail, 'startAt' | 'status'> as AgencyRequestDetail;
const worker = { workerProfileId: 'wp-1', workerRequestStatus: 1 } as unknown as AgencyRequestWorker;
const range = { startDate: '2026-11-01', endDate: '2026-12-05' };

function sheet(overrides: Partial<TimeSheetListItem> = {}): TimeSheetListItem {
  return {
    id: 'ts-1',
    day: '2026-11-18T00:00:00',
    clockIn: '2026-11-18T08:00:00',
    clockOut: '2026-11-18T16:30:00',
    timeIn: '2026-11-18T08:00:00',
    timeOut: '2026-11-18T16:30:00',
    timeInApproved: '2026-11-18T08:00:00',
    timeOutApproved: '2026-11-18T16:30:00',
    totalHours: 8.5,
    totalHoursApproved: 0,
    canUpdate: true,
    wasApproved: false,
    missingHours: '01:30:00',
    missingHoursOvertime: '00:15:00',
    missingRateWorker: 18,
    missingRateAgency: 25,
    deductionsOthers: 0,
    bonusOrOthers: 0,
    deductionsOthersDescription: '',
    bonusOrOthersDescription: '',
    reimbursements: 0,
    reimbursementsDescription: '',
    comment: '',
    ...overrides,
  };
}

type Day = Omit<TimeSheetListItem, 'id' | 'totalHoursApproved'> & { id: string | null; totalHoursApproved: number | null };

function emptyDay(day = '2026-11-19T00:00:00'): Day {
  return { ...sheet({ day }), id: null, totalHoursApproved: null, clockIn: null, clockOut: null };
}

async function setup(days: Day[]) {
  vi.mocked(getAgencyWorkerTimeSheetByDate).mockResolvedValue(days as TimeSheetListItem[]);
  const wrapper = mountWithPlugins(AgencyPunchCardWorkerContainer, { props: { workerProfileId: 'wp-1', requestId: 'req-1', request, worker } });
  wrapper.findComponent({ name: 'CalendarPunchCard' }).vm.$emit('onMonthChange', range);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function day(wrapper: Wrapper, index = 0) {
  return wrapper.findAll('.day')[index];
}

function action(wrapper: Wrapper, icon: string, index = 0) {
  const button = day(wrapper, index).findAll('button').find((b) => b.find(`.mdi-${icon}`).exists());
  if (!button) throw new Error(`${icon} button not found`);
  return button;
}

function hasAction(wrapper: Wrapper, icon: string, index = 0) {
  return day(wrapper, index).findAll('button').some((b) => b.find(`.mdi-${icon}`).exists());
}

beforeEach(() => {
  admin.value = true;
});

afterEach(() => {
  vi.useRealTimers();
});

describe('AgencyPunchCardWorkerContainer', () => {
  it('passes the request and worker to the calendar', () => {
    const wrapper = mountWithPlugins(AgencyPunchCardWorkerContainer, { props: { workerProfileId: 'wp-1', requestId: 'req-1', request, worker } });

    expect(wrapper.findComponent({ name: 'CalendarPunchCard' }).props()).toEqual({
      highlights: [], workerProfileId: 'wp-1', requestId: 'req-1', startDate: request.startAt, status: 1, worker,
    });
  });

  it('loads the time sheets of the visible month', async () => {
    const wrapper = await setup([sheet()]);

    expect(getAgencyWorkerTimeSheetByDate).toHaveBeenCalledExactlyOnceWith('req-1', 'wp-1', range);
    const text = day(wrapper).text();
    expect(text.replace(/\s+/g, ' ')).toContain('Clock in: 08:00 to 16:30');
    expect(text).toContain('Hours: 8.50');
    expect(text).toContain('Missing Hours: 1.50');
    expect(text).not.toContain('Hours Approved');
    expect(['eye', 'pencil', 'check', 'delete'].map((i) => hasAction(wrapper, i))).toEqual([true, true, true, true]);
    expect(hasAction(wrapper, 'paperclip')).toBe(false);
  });

  it('shows approved hours and hides edit for non admins', async () => {
    admin.value = false;
    const wrapper = await setup([sheet({ totalHoursApproved: 8, clockIn: null, missingHours: '', canUpdate: false })]);

    expect(day(wrapper).text()).toContain('Hours Approved: 8');
    expect(day(wrapper).text()).not.toContain('Clock in');
    expect(['pencil', 'check', 'delete', 'paperclip'].map((i) => hasAction(wrapper, i))).toEqual([false, false, false, true]);
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('boom');
    vi.mocked(getAgencyWorkerTimeSheetByDate).mockRejectedValue(error);
    const wrapper = mountWithPlugins(AgencyPunchCardWorkerContainer, { props: { workerProfileId: 'wp-1', requestId: 'req-1', request, worker } });
    wrapper.findComponent({ name: 'CalendarPunchCard' }).vm.$emit('onMonthChange', range);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('fast approves the reported hours and reloads', async () => {
    vi.mocked(updateAgencyWorkerTimeSheet).mockResolvedValue();
    const wrapper = await setup([sheet()]);
    await action(wrapper, 'check').trigger('click');
    await flushPromises();

    expect(updateAgencyWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('req-1', 'wp-1', 'ts-1', expect.objectContaining({
      hours: '08:30:00',
      timeIn: '2026-11-18T08:00:00',
      missingHours: '01:30:00',
      missingRateWorker: 18,
    }));
    expect(getAgencyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
  });

  it('shows the error when fast approve fails', async () => {
    const error = new Error('boom');
    vi.mocked(updateAgencyWorkerTimeSheet).mockRejectedValue(error);
    const wrapper = await setup([sheet()]);
    await action(wrapper, 'check').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getAgencyWorkerTimeSheetByDate).toHaveBeenCalledTimes(1);
  });

  it('deletes an editable time sheet and reloads', async () => {
    vi.mocked(deleteAgencyWorkerTimeSheet).mockResolvedValue();
    const wrapper = await setup([sheet()]);
    await action(wrapper, 'delete').trigger('click');
    await flushPromises();

    expect(deleteAgencyWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('req-1', 'wp-1', 'ts-1');
    expect(getAgencyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
  });

  it('shows the error when deleting fails', async () => {
    const error = new Error('boom');
    vi.mocked(deleteAgencyWorkerTimeSheet).mockRejectedValue(error);
    const wrapper = await setup([sheet()]);
    await action(wrapper, 'delete').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('loads where a locked time sheet was used', async () => {
    vi.mocked(getAgencyTimeSheetUsages).mockResolvedValue({ invoiceNumber: 'INV-7', payStubNumber: 'PS-3' });
    const wrapper = await setup([sheet({ canUpdate: false, totalHoursApproved: 8 })]);
    await action(wrapper, 'paperclip').trigger('click');
    await flushPromises();

    expect(getAgencyTimeSheetUsages).toHaveBeenCalledExactlyOnceWith('req-1', 'wp-1', 'ts-1');
    expect(document.body.textContent).toContain('INV-7');
    expect(document.body.textContent).toContain('PS-3');
  });

  it('shows the error when the usages fail to load', async () => {
    const error = new Error('boom');
    vi.mocked(getAgencyTimeSheetUsages).mockRejectedValue(error);
    const wrapper = await setup([sheet({ canUpdate: false, totalHoursApproved: 8 })]);
    await action(wrapper, 'paperclip').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('opens the detail of a day', async () => {
    const wrapper = await setup([sheet({ comment: 'Late' })]);
    await action(wrapper, 'eye').trigger('click');
    await nextTick();

    const detail = wrapper.findComponent({ name: 'AgencyRequestTimeSheetDetail' });
    expect(detail.props('editableDay')).toEqual(expect.objectContaining({ id: 'ts-1', comment: 'Late' }));
  });

  it('opens the editor with the approved values as dates', async () => {
    vi.useFakeTimers({ toFake: ['Date'] });
    vi.setSystemTime(new Date(2026, 10, 19, 12));
    const wrapper = await setup([sheet({ totalHoursApproved: 7.5 })]);
    await action(wrapper, 'pencil').trigger('click');
    await nextTick();

    const modal = wrapper.findComponent({ name: 'AgencyRequestTimeSheetModal' });
    const editable = modal.props('editableDay') as PunchCardEditableDay;
    expect(modal.props('worker')).toEqual({ workerProfileId: 'wp-1' });
    expect(editable.timeInApproved).toEqual(new Date('2026-11-18T08:00:00'));
    expect(editable.timeOutApproved).toEqual(new Date('2026-11-18T16:30:00'));
    expect(editable.hoursApprovedToDate).toEqual(new Date(2026, 10, 19, 7, 30, 0));
    expect(editable.missinghoursToDate).toEqual(new Date(2026, 10, 19, 1, 30, 0));
    expect(editable.missingHoursOvertimeToDate).toEqual(new Date(2026, 10, 19, 0, 15, 0));
  });

  it('leaves missing dates empty when the day has none', async () => {
    const wrapper = await setup([sheet({ timeInApproved: null, timeOutApproved: null, missingHours: '', missingHoursOvertime: '' })]);
    await action(wrapper, 'pencil').trigger('click');
    await nextTick();

    const editable = wrapper.findComponent({ name: 'AgencyRequestTimeSheetModal' }).props('editableDay') as PunchCardEditableDay;
    expect(editable.timeInApproved).toBeNull();
    expect(editable.missinghoursToDate).toBeNull();
    expect(editable.missingHoursOvertimeToDate).toBeNull();
  });

  it('reloads and closes the editor after an update', async () => {
    const wrapper = await setup([sheet()]);
    await action(wrapper, 'pencil').trigger('click');
    await nextTick();
    wrapper.findComponent({ name: 'AgencyRequestTimeSheetModal' }).vm.$emit('updateData');
    await flushPromises();

    expect(getAgencyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
    expect(wrapper.findAllComponents({ name: 'BModal' })[0].props('modelValue')).toBe(false);
  });

  it('disables manual hours for non admins', async () => {
    admin.value = false;
    const wrapper = await setup([emptyDay()]);

    expect(day(wrapper).find('input').attributes('disabled')).toBeDefined();
  });

  it('rejects hours above the daily maximum', async () => {
    const wrapper = await setup([emptyDay()]);
    await day(wrapper).find('input').setValue('13');
    await action(wrapper, 'check').trigger('click');

    expect(day(wrapper).find('.help').text()).toBe('Hours must be between 0 and 12');
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(postAgencyWorkerTimeSheet).not.toHaveBeenCalled();
  });

  it('requires the hours', async () => {
    const wrapper = await setup([emptyDay()]);
    await action(wrapper, 'check').trigger('click');

    expect(day(wrapper).find('.help').text()).toBe('Hours must be between 0 and 12');
    expect(postAgencyWorkerTimeSheet).not.toHaveBeenCalled();
  });

  it('rejects more than two decimals', async () => {
    const wrapper = await setup([emptyDay()]);
    await day(wrapper).find('input').setValue('7.555');
    await action(wrapper, 'check').trigger('click');

    expect(day(wrapper).find('.help').text()).toBe('Max 2 decimal places');
    expect(postAgencyWorkerTimeSheet).not.toHaveBeenCalled();
  });

  it('reports manual hours for an empty day and clears the error', async () => {
    vi.mocked(postAgencyWorkerTimeSheet).mockResolvedValue({ id: 'ts-9' });
    const wrapper = await setup([emptyDay()]);
    await action(wrapper, 'check').trigger('click');
    await day(wrapper).find('input').setValue('7.5');
    await action(wrapper, 'check').trigger('click');
    await flushPromises();

    expect(postAgencyWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('req-1', 'wp-1', {
      hours: '07:30:00',
      timeIn: '2026-11-19T00:00:00',
      missingHours: '01:30:00',
      missingHoursOvertime: '00:15:00',
      missingRateWorker: 18,
      missingRateAgency: 25,
      deductionsOthers: 0,
    });
    expect(getAgencyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
    expect(day(wrapper).find('.help').text()).not.toContain('Hours must');
  });

  it('shows the error when reporting fails', async () => {
    const error = new Error('boom');
    vi.mocked(postAgencyWorkerTimeSheet).mockRejectedValue(error);
    const wrapper = await setup([emptyDay()]);
    await day(wrapper).find('input').setValue('4');
    await action(wrapper, 'check').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('exposes a refresh that reloads the month', async () => {
    const wrapper = await setup([sheet()]);
    (wrapper.vm as unknown as { updateCell: () => void }).updateCell();
    await flushPromises();

    expect(getAgencyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
  });
});
