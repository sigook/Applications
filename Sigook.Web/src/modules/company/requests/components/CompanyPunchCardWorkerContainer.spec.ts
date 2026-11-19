import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import CompanyPunchCardWorkerContainer from '@/modules/company/requests/components/CompanyPunchCardWorkerContainer.vue';
import {
  companyTimeSheetClockIn,
  deleteCompanyWorkerTimeSheet,
  getCompanyWorkerTimeSheetByDate,
  postCompanyWorkerTimeSheet,
  updateCompanyRequestWorkerTimeSheet,
} from '@/modules/company/requests/api';
import { DurationTerm, RequestStatus, WorkerRequestStatus } from '@/shared/constants/enums';
import { maximumHoursPerDay } from '@/shared/constants/catalog';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyRequestDetail, CompanyRequestWorker } from '@/modules/company/requests/types';
import type { CompanyTimeSheetListItem, PunchCardDay, PunchCardEditableDay } from '@/shared/punch-card/types';

const touch = ref(false);

vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: computed(() => touch.value) }),
}));
vi.mock('@/modules/company/requests/api', () => ({
  getCompanyWorkerTimeSheetByDate: vi.fn(),
  postCompanyWorkerTimeSheet: vi.fn(),
  deleteCompanyWorkerTimeSheet: vi.fn(),
  companyTimeSheetClockIn: vi.fn(),
  updateCompanyRequestWorkerTimeSheet: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/punch-card/CalendarPunchCard.vue', () => ({
  default: {
    name: 'CalendarPunchCard',
    props: { highlights: Array, workerProfileId: String, requestId: String, startDate: String, status: Number, worker: Object },
    emits: ['onMonthChange'],
    template: '<div class="calendar"><div v-for="(item, index) in highlights" :key="index" class="cal-day"><slot name="punch-input" :item="item" :index="index" /></div></div>',
  },
}));
vi.mock('@/modules/company/requests/components/PunchCardAgenda.vue', () => ({
  default: {
    name: 'PunchCardAgenda',
    props: { highlights: Array, startDate: String, worker: Object, errors: Object },
    emits: ['monthChange', 'view', 'edit', 'approve', 'postHours', 'clockIn'],
    template: '<div class="agenda" />',
  },
}));
vi.mock('@/modules/company/requests/components/TimeSheetDaySheet.vue', () => ({
  default: {
    name: 'TimeSheetDaySheet',
    props: { modelValue: Boolean, editableDay: Object },
    emits: ['update:modelValue', 'save', 'post', 'delete'],
    template: '<div class="day-sheet" />',
  },
}));
vi.mock('@/shared/ui/SheetPanel.vue', () => ({
  default: { name: 'SheetPanel', props: { modelValue: Boolean, title: String }, emits: ['update:modelValue'], template: '<div class="sheet-panel"><slot /></div>' },
}));
vi.mock('@/modules/company/requests/components/CompanyRequestTimeSheetModal.vue', () => ({
  default: { name: 'TimeSheetModal', props: { editableDay: Object, worker: Object }, emits: ['updateData', 'update:editableDay'], template: '<div class="ts-modal" />' },
}));
vi.mock('@/modules/company/requests/components/CompanyRequestTimeSheetDetail.vue', () => ({
  default: { name: 'TimeSheetDetail', props: { editableDay: Object }, template: '<div class="ts-detail" />' },
}));

const request: CompanyRequestDetail = {
  id: 'request-1', numberId: 10, jobTitle: 'Forklift operator', workersQuantity: 2, workersQuantityWorking: 1,
  durationBreak: '00:30:00', breakIsPaid: false, holidayIsPaid: false, isAsap: false, jobIsOnBranchOffice: false,
  status: RequestStatus.Open, durationTerm: DurationTerm.LongTerm, createdAt: '2026-11-01T00:00:00', startAt: '2026-11-02T00:00:00',
};

const worker: CompanyRequestWorker = {
  numberId: 101, requestId: 'request-1', id: 'wr-1', workerId: 'worker-1', workerProfileId: 'wp-1', name: 'Ann Lee',
  workerRequestStatus: WorkerRequestStatus.Booked, profileImage: '', isSubcontractor: false,
  totalHoursApproved: 32, totalHoursWorker: 40, startWorking: '2026-11-02T00:00:00', rejectedAt: null,
};

function timeSheet(overrides: Partial<CompanyTimeSheetListItem>): CompanyTimeSheetListItem {
  return {
    id: 'ts-1', day: '2026-11-16T00:00:00', clockIn: '2026-11-16T08:00:00', clockOut: '2026-11-16T16:30:00',
    timeIn: '2026-11-16T08:00:00', timeOut: '2026-11-16T16:30:00', totalHours: 8.5, totalHoursApproved: 0,
    canUpdate: true, wasApproved: false, missingHours: '00:30:00', missingHoursOvertime: '00:00:00',
    deductionsOthers: 0, bonusOrOthers: 0, deductionsOthersDescription: '', bonusOrOthersDescription: '', comment: '',
    ...overrides,
  };
}

const pending = timeSheet({});
const approvedLocked = timeSheet({ id: 'ts-2', day: '2026-11-17T00:00:00', totalHoursApproved: 8, canUpdate: false });
const range = { startDate: '2026-11-01', endDate: '2026-12-05' };

async function setup(sheets: PunchCardDay[] = [pending, approvedLocked]) {
  vi.mocked(getCompanyWorkerTimeSheetByDate).mockResolvedValue(structuredClone(sheets) as CompanyTimeSheetListItem[]);
  const wrapper = mountWithPlugins(CompanyPunchCardWorkerContainer, {
    props: { workerProfileId: 'wp-1', requestId: 'request-1', request, worker },
  });
  if (touch.value) wrapper.findComponent({ name: 'PunchCardAgenda' }).vm.$emit('monthChange', range);
  else wrapper.findComponent({ name: 'CalendarPunchCard' }).vm.$emit('onMonthChange', range);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function day(wrapper: Wrapper, index: number) {
  return wrapper.findAll('.cal-day')[index];
}

function button(scope: ReturnType<Wrapper['find']>, icon: string) {
  const found = scope.findAll('button').find((b) => b.find(`.mdi-${icon}`).exists());
  if (!found) throw new Error(`${icon} button not found`);
  return found;
}

function emptyDay(dayValue: string): PunchCardDay {
  return { id: null, day: dayValue, totalHoursApproved: null };
}

beforeEach(() => {
  touch.value = false;
  vi.useFakeTimers({ toFake: ['Date'] });
  vi.setSystemTime(new Date(2026, 10, 19, 12));
});

afterEach(() => {
  vi.useRealTimers();
});

describe('CompanyPunchCardWorkerContainer', () => {
  it('loads the timesheets of the visible range into the calendar', async () => {
    const wrapper = await setup();

    expect(getCompanyWorkerTimeSheetByDate).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', range);
    expect(wrapper.findComponent({ name: 'CalendarPunchCard' }).props()).toMatchObject({
      highlights: [pending, approvedLocked], startDate: '2026-11-02T00:00:00', status: RequestStatus.Open,
    });
    expect(day(wrapper, 0).text().replace(/\s+/g, ' ')).toContain('Clock in: 08:00 to 16:30');
    expect(day(wrapper, 0).text()).toContain('Hours: 8.50');
    expect(day(wrapper, 1).text()).toContain('Hours Approved: 8');
  });

  it('shows the error when the timesheets cannot load', async () => {
    const error = new Error('load failed');
    vi.mocked(getCompanyWorkerTimeSheetByDate).mockRejectedValue(error);
    const wrapper = mountWithPlugins(CompanyPunchCardWorkerContainer, { props: { workerProfileId: 'wp-1', requestId: 'request-1', request, worker } });
    wrapper.findComponent({ name: 'CalendarPunchCard' }).vm.$emit('onMonthChange', range);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
  });

  it('approves a pending day with the worker hours', async () => {
    vi.mocked(updateCompanyRequestWorkerTimeSheet).mockResolvedValue();
    const wrapper = await setup();
    await button(day(wrapper, 0), 'check').trigger('click');
    await flushPromises();

    expect(updateCompanyRequestWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', 'ts-1', expect.objectContaining({
      hours: '08:30:00', timeIn: '2026-11-16T08:00:00', missingHours: '00:30:00',
    }));
    expect(getCompanyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
  });

  it('shows the error when approving fails', async () => {
    const error = new Error('approve failed');
    vi.mocked(updateCompanyRequestWorkerTimeSheet).mockRejectedValue(error);
    const wrapper = await setup();
    await button(day(wrapper, 0), 'check').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
  });

  it('opens the detail of a locked day', async () => {
    const wrapper = await setup();
    await button(day(wrapper, 1), 'eye').trigger('click');

    expect(wrapper.findComponent({ name: 'TimeSheetDetail' }).props('editableDay')).toEqual(approvedLocked);
  });

  it('edits a day in the timesheet modal and reloads once saved', async () => {
    const wrapper = await setup();
    await button(day(wrapper, 1), 'pencil').trigger('click');

    const modal = wrapper.findComponent({ name: 'TimeSheetModal' });
    const editable = modal.props('editableDay') as PunchCardEditableDay;
    expect(modal.props('worker')).toEqual({ workerProfileId: 'wp-1' });
    expect(editable.hoursApprovedToDate?.getHours()).toBe(8);
    expect(editable.missinghoursToDate?.getMinutes()).toBe(30);
    expect(editable.missingHoursOvertimeToDate?.getHours()).toBe(0);
    expect(editable.timeInApproved).toBeNull();

    modal.vm.$emit('updateData');
    await flushPromises();
    expect(getCompanyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
  });

  it('deletes an updatable day once confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyWorkerTimeSheet).mockResolvedValue();
    const wrapper = await setup();
    await button(day(wrapper, 0), 'delete').trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this item?');
    expect(deleteCompanyWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', 'ts-1');
    expect(getCompanyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
  });

  it('keeps the day when the deletion is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await button(day(wrapper, 0), 'delete').trigger('click');
    await flushPromises();

    expect(deleteCompanyWorkerTimeSheet).not.toHaveBeenCalled();
  });

  it('shows the error when the deletion fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyWorkerTimeSheet).mockRejectedValue(error);
    const wrapper = await setup();
    await button(day(wrapper, 0), 'delete').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
  });

  it('posts the typed hours of an empty day', async () => {
    vi.mocked(postCompanyWorkerTimeSheet).mockResolvedValue({ id: 'ts-9' });
    const wrapper = await setup([emptyDay('2026-11-18T00:00:00')]);
    day(wrapper, 0).findComponent({ name: 'BNumberinput' }).vm.$emit('update:modelValue', 7.5);
    await nextTick();
    await button(day(wrapper, 0), 'check').trigger('click');
    await flushPromises();

    expect(postCompanyWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', expect.objectContaining({
      hours: '07:30:00', timeIn: '2026-11-18T00:00:00',
    }));
    expect(getCompanyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
  });

  it.each([
    [null, `Hours must be between 0 and ${maximumHoursPerDay}`],
    [maximumHoursPerDay + 1, `Hours must be between 0 and ${maximumHoursPerDay}`],
    [1.234, 'Max 2 decimal places'],
  ])('rejects %s typed hours', async (hours, message) => {
    const wrapper = await setup([emptyDay('2026-11-18T00:00:00')]);
    day(wrapper, 0).findComponent({ name: 'BNumberinput' }).vm.$emit('update:modelValue', hours);
    await nextTick();
    await button(day(wrapper, 0), 'check').trigger('click');

    expect(day(wrapper, 0).find('.help.is-danger').text()).toBe(message);
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(postCompanyWorkerTimeSheet).not.toHaveBeenCalled();
  });

  it('shows the error when posting fails', async () => {
    const error = new Error('post failed');
    vi.mocked(postCompanyWorkerTimeSheet).mockRejectedValue(error);
    const wrapper = await setup([emptyDay('2026-11-18T00:00:00')]);
    day(wrapper, 0).findComponent({ name: 'BNumberinput' }).vm.$emit('update:modelValue', 4);
    await nextTick();
    await button(day(wrapper, 0), 'check').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
  });

  it('clocks a worker in today', async () => {
    vi.mocked(companyTimeSheetClockIn).mockResolvedValue({ timeSheetId: 'ts-9', workerFullName: 'Ann Lee', finish: false });
    const wrapper = await setup([emptyDay('2026-11-19T00:00:00'), emptyDay('2026-11-18T00:00:00')]);
    expect(day(wrapper, 1).text()).not.toContain('Clock in');
    const clockIn = day(wrapper, 0).findAll('button').find((b) => b.text() === 'Clock in');
    await clockIn?.trigger('click');

    wrapper.findComponent({ name: 'BTimepicker' }).vm.$emit('update:modelValue', new Date(2026, 10, 19, 7, 15));
    await nextTick();
    await wrapper.find('.modal.is-active button.is-primary').trigger('click');
    await flushPromises();

    expect(companyTimeSheetClockIn).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', { clockIn: '07:15:00' });
    expect(getCompanyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
  });

  it('shows the error when the clock in fails', async () => {
    const error = new Error('clock in failed');
    vi.mocked(companyTimeSheetClockIn).mockRejectedValue(error);
    const wrapper = await setup([emptyDay('2026-11-19T00:00:00')]);
    await day(wrapper, 0).findAll('button').find((b) => b.text() === 'Clock in')?.trigger('click');
    await wrapper.find('.modal.is-active button.is-primary').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
  });

  describe('on touch screens', () => {
    beforeEach(() => {
      touch.value = true;
    });

    it('uses the agenda and relays its actions', async () => {
      vi.mocked(updateCompanyRequestWorkerTimeSheet).mockResolvedValue();
      const wrapper = await setup();
      const agenda = wrapper.findComponent({ name: 'PunchCardAgenda' });

      expect(wrapper.findComponent({ name: 'CalendarPunchCard' }).exists()).toBe(false);
      expect(agenda.props()).toMatchObject({ highlights: [pending, approvedLocked], startDate: '2026-11-02T00:00:00', errors: {} });

      agenda.vm.$emit('approve', pending);
      await flushPromises();
      expect(updateCompanyRequestWorkerTimeSheet).toHaveBeenCalledWith('request-1', 'wp-1', 'ts-1', expect.objectContaining({ hours: '08:30:00' }));

      agenda.vm.$emit('view', approvedLocked);
      await nextTick();
      expect(wrapper.findComponent({ name: 'SheetPanel' }).props()).toEqual({ modelValue: true, title: 'Tuesday, November 17' });
    });

    it('edits a day in the day sheet and saves it', async () => {
      vi.mocked(updateCompanyRequestWorkerTimeSheet).mockResolvedValue();
      const wrapper = await setup();
      wrapper.findComponent({ name: 'PunchCardAgenda' }).vm.$emit('edit', pending);
      await nextTick();
      const sheet = wrapper.findComponent({ name: 'TimeSheetDaySheet' });
      expect(sheet.props('modelValue')).toBe(true);
      expect(sheet.props('editableDay')).toMatchObject({ id: 'ts-1' });

      const model = { hours: '08:00:00', timeIn: '2026-11-16T00:00:00', missingHours: '00:00:00', missingHoursOvertime: '00:00:00' };
      sheet.vm.$emit('save', model);
      await flushPromises();

      expect(updateCompanyRequestWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', 'ts-1', model);
      expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
      expect(sheet.props('modelValue')).toBe(false);
      expect(getCompanyWorkerTimeSheetByDate).toHaveBeenCalledTimes(2);
    });

    it('shows the error when the day sheet save fails', async () => {
      const error = new Error('save failed');
      vi.mocked(updateCompanyRequestWorkerTimeSheet).mockRejectedValue(error);
      const wrapper = await setup();
      wrapper.findComponent({ name: 'PunchCardAgenda' }).vm.$emit('edit', pending);
      await nextTick();
      wrapper.findComponent({ name: 'TimeSheetDaySheet' }).vm.$emit('save', { hours: '08:00:00', timeIn: '2026-11-16T00:00:00' });
      await flushPromises();

      expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
    });

    it('posts hours typed in the day sheet', async () => {
      vi.mocked(postCompanyWorkerTimeSheet).mockResolvedValue({ id: 'ts-9' });
      const wrapper = await setup([emptyDay('2026-11-18T00:00:00')]);
      wrapper.findComponent({ name: 'PunchCardAgenda' }).vm.$emit('edit', emptyDay('2026-11-18T00:00:00'));
      await nextTick();
      wrapper.findComponent({ name: 'TimeSheetDaySheet' }).vm.$emit('post', 6);
      await flushPromises();

      expect(postCompanyWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', expect.objectContaining({ hours: '06:00:00', timeIn: '2026-11-18T00:00:00' }));
    });

    it('deletes from the day sheet once confirmed', async () => {
      vi.mocked(showAlertConfirm).mockResolvedValue(true);
      vi.mocked(deleteCompanyWorkerTimeSheet).mockResolvedValue();
      const wrapper = await setup();
      wrapper.findComponent({ name: 'PunchCardAgenda' }).vm.$emit('edit', pending);
      await nextTick();
      wrapper.findComponent({ name: 'TimeSheetDaySheet' }).vm.$emit('delete');
      await flushPromises();

      expect(deleteCompanyWorkerTimeSheet).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', 'ts-1');
    });

    it('keeps the day when the day sheet deletion is not confirmed', async () => {
      vi.mocked(showAlertConfirm).mockResolvedValue(false);
      const wrapper = await setup();
      wrapper.findComponent({ name: 'PunchCardAgenda' }).vm.$emit('edit', pending);
      await nextTick();
      wrapper.findComponent({ name: 'TimeSheetDaySheet' }).vm.$emit('delete');
      await flushPromises();

      expect(deleteCompanyWorkerTimeSheet).not.toHaveBeenCalled();
    });

    it('validates hours posted from the agenda by day', async () => {
      const wrapper = await setup();
      const agenda = wrapper.findComponent({ name: 'PunchCardAgenda' });
      agenda.vm.$emit('postHours', { ...emptyDay('2026-11-18T00:00:00'), totalHoursApproved: -1 });
      await nextTick();
      expect(agenda.props('errors')).toEqual({ '2026-11-18': `Hours must be between 0 and ${maximumHoursPerDay}` });

      vi.mocked(postCompanyWorkerTimeSheet).mockResolvedValue({ id: 'ts-9' });
      agenda.vm.$emit('postHours', { ...emptyDay('2026-11-18T00:00:00'), totalHoursApproved: 5 });
      await flushPromises();
      expect(agenda.props('errors')).toEqual({});
      expect(postCompanyWorkerTimeSheet).toHaveBeenCalledOnce();
    });

    it('opens the clock in modal from the agenda', async () => {
      const wrapper = await setup();
      wrapper.findComponent({ name: 'PunchCardAgenda' }).vm.$emit('clockIn');
      await nextTick();

      expect(wrapper.find('.modal.is-active').text()).toContain('At what time did the worker clock in?');
    });
  });
});
