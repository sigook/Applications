import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { effectScope, nextTick, ref, type EffectScope } from 'vue';
import dayjs from 'dayjs';
import { usePunchCardCalendar, type PunchCardCalendar } from '@/modules/company/requests/usePunchCardCalendar';
import { showAlertError } from '@/shared/utils/toast';
import { WorkerRequestStatus } from '@/shared/constants/enums';
import type { PunchCardDay, PunchCardWeek, PunchCardWorker } from '@/shared/punch-card/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

interface Setup {
  calendar: PunchCardCalendar;
  highlights: ReturnType<typeof ref<PunchCardDay[] | undefined>>;
  worker: ReturnType<typeof ref<PunchCardWorker | undefined>>;
  onMonthChange: ReturnType<typeof vi.fn<(range: { startDate: string; endDate: string }) => void>>;
}

let scope: EffectScope;

function setup(
  startDate: string | undefined = '2026-11-10',
  initialWorker?: PunchCardWorker,
  initialHighlights?: PunchCardDay[],
): Setup {
  const highlights = ref<PunchCardDay[] | undefined>(initialHighlights);
  const worker = ref<PunchCardWorker | undefined>(initialWorker);
  const onMonthChange = vi.fn<(range: { startDate: string; endDate: string }) => void>();
  scope = effectScope();
  const calendar = scope.run(() =>
    usePunchCardCalendar({
      highlights: () => highlights.value,
      startDate: () => startDate,
      worker: () => worker.value,
      onMonthChange,
    }),
  );
  if (!calendar) throw new Error('calendar not created');
  return { calendar, highlights, worker, onMonthChange };
}

function keys(week: PunchCardWeek): string[] {
  return week.days.map((d) => dayjs(d.day).format('YYYY-MM-DD'));
}

function week(totalHoursWeek: number | null): PunchCardWeek {
  return {
    totalHoursWeek,
    days: Array.from({ length: 7 }, (_, i) => ({ id: null, day: new Date(2026, 10, 1 + i), totalHoursApproved: 99 })),
  };
}

describe('usePunchCardCalendar', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date('2026-11-19T12:00:00'));
  });

  afterEach(() => {
    scope.stop();
    vi.useRealTimers();
  });

  it('builds the current month in full Sunday-to-Saturday weeks', () => {
    const { calendar, onMonthChange } = setup();

    expect(calendar.monthLabel.value).toBe('November');
    expect(calendar.yearLabel.value).toBe('2026');
    expect(calendar.weekdays).toEqual(['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']);
    expect(calendar.calendar.value).toHaveLength(5);
    expect(keys(calendar.calendar.value[0])[0]).toBe('2026-11-01');
    expect(keys(calendar.calendar.value[4]).at(-1)).toBe('2026-12-05');
    expect(calendar.calendar.value[2].days[0]).toEqual({ id: null, day: new Date(2026, 10, 15), totalHoursApproved: 0 });
    expect(onMonthChange).toHaveBeenCalledWith({ startDate: '2026-11-01', endDate: '2026-12-05' });
  });

  it('navigates to the next and previous months, crossing the year', () => {
    const { calendar, onMonthChange } = setup();

    calendar.getNextMonth();
    expect(calendar.monthLabel.value).toBe('December');
    expect(onMonthChange).toHaveBeenLastCalledWith({ startDate: '2026-11-29', endDate: '2027-01-02' });

    calendar.getNextMonth();
    expect(calendar.monthLabel.value).toBe('January');
    expect(calendar.yearLabel.value).toBe('2027');

    calendar.getPreviousMonth();
    calendar.getPreviousMonth();
    calendar.getPreviousMonth();
    expect(calendar.monthLabel.value).toBe('October');
    expect(calendar.yearLabel.value).toBe('2026');
    expect(onMonthChange).toHaveBeenLastCalledWith({ startDate: '2026-09-27', endDate: '2026-10-31' });
  });

  it('returns to the current month', () => {
    const { calendar, onMonthChange } = setup();
    calendar.getNextMonth();
    calendar.getNextMonth();
    calendar.getTodayMonth();

    expect(calendar.monthLabel.value).toBe('November');
    expect(onMonthChange).toHaveBeenLastCalledWith({ startDate: '2026-11-01', endDate: '2026-12-05' });
  });

  it('recognises today and days outside the displayed month', () => {
    const { calendar } = setup();

    expect(calendar.isToday('2026-11-19')).toBe(true);
    expect(calendar.isToday(new Date(2026, 10, 18, 23, 59))).toBe(false);
    expect(calendar.notCurrentMonth('2026-11-30')).toBe(false);
    expect(calendar.notCurrentMonth('2026-12-01')).toBe(true);
    expect(calendar.notCurrentMonth('2026-10-31')).toBe(true);
  });

  it('formats keys and week ranges', () => {
    const { calendar } = setup();

    expect(calendar.dayKey(new Date(2026, 0, 5, 22, 30))).toBe('2026-01-05');
    expect(calendar.weekRange(calendar.calendar.value[4])).toBe('Nov 29 – Dec 5');
    expect(calendar.weekRange({ totalHoursWeek: 0, days: [] })).toBe('');
  });

  describe('isDayEditable', () => {
    it('allows days from the request start until one month from now', () => {
      const { calendar } = setup('2026-11-10');

      expect(calendar.isDayEditable('2026-11-09')).toBe(false);
      expect(calendar.isDayEditable('2026-11-10')).toBe(true);
      expect(calendar.isDayEditable('2026-12-19')).toBe(true);
      expect(calendar.isDayEditable('2026-12-20')).toBe(false);
    });

    it('ignores the end date of a booked worker', () => {
      const { calendar } = setup('2026-11-10', {
        workerRequestStatus: WorkerRequestStatus.Booked,
        finishedAt: '2026-11-12',
      });

      expect(calendar.isDayEditable('2026-12-15')).toBe(true);
    });

    it('limits a finished worker to one month after finishing', () => {
      const { calendar } = setup('2026-10-01', {
        workerRequestStatus: WorkerRequestStatus.Finished,
        finishedAt: '2026-11-10',
      });

      expect(calendar.isDayEditable('2026-12-09')).toBe(true);
      expect(calendar.isDayEditable('2026-12-10')).toBe(false);
      expect(calendar.isDayEditable('2026-09-30')).toBe(false);
    });

    it('uses the rejection date when the worker was rejected', () => {
      const { calendar } = setup('2026-10-01', {
        workerRequestStatus: WorkerRequestStatus.Rejected,
        rejectedAt: '2026-10-20',
      });

      expect(calendar.isDayEditable('2026-11-19')).toBe(true);
      expect(calendar.isDayEditable('2026-11-20')).toBe(false);
    });

    it('treats a non-booked worker without end date as unrestricted', () => {
      const { calendar } = setup('2026-10-01', { workerRequestStatus: WorkerRequestStatus.Finished });

      expect(calendar.isDayEditable('2026-12-19')).toBe(true);
    });
  });

  describe('distributeWeekHours', () => {
    it('spreads the weekly total across the days', () => {
      const { calendar } = setup();
      const target = week(40);
      calendar.distributeWeekHours(target);

      expect(target.days.map((d) => d.totalHoursApproved)).toEqual([6, 6, 6, 6, 6, 6, 4]);
      expect(showAlertError).not.toHaveBeenCalled();
    });

    it('puts a total under the daily maximum on the first day and zeroes the rest', () => {
      const { calendar } = setup();
      const target = week(7.5);
      calendar.distributeWeekHours(target);

      expect(target.days.map((d) => d.totalHoursApproved)).toEqual([7.5, 0, 0, 0, 0, 0, 0]);
    });

    it('treats a missing total as zero hours', () => {
      const { calendar } = setup();
      const target = week(null);
      calendar.distributeWeekHours(target);

      expect(target.days.every((d) => d.totalHoursApproved === 0)).toBe(true);
    });

    it('rejects a total above the daily maximum', () => {
      const { calendar } = setup();
      const target = week(100);
      calendar.distributeWeekHours(target);

      expect(showAlertError).toHaveBeenCalledWith('Total hours is invalid');
      expect(target.days[0].totalHoursApproved).toBe(99);
    });
  });

  describe('highlights', () => {
    it('places highlights passed at creation', () => {
      const { calendar } = setup('2026-11-10', undefined, [{ id: 't1', day: '2026-11-16T00:00:00', totalHoursApproved: 8 }]);

      expect(calendar.calendar.value[2].days[1].id).toBe('t1');
      expect(calendar.calendar.value[2].totalHoursWeek).toBe(8);
    });

    it('places highlighted days in the calendar and sums the week', async () => {
      const { calendar, highlights } = setup();
      highlights.value = [
        { id: 't1', day: '2026-11-16T00:00:00', totalHoursApproved: 8 },
        { id: 't2', day: '2026-11-17T00:00:00', totalHoursApproved: 7.5 },
        { id: 't3', day: '2026-11-24T00:00:00', totalHoursApproved: null },
      ];
      await nextTick();

      const third = calendar.calendar.value[2];
      expect(third.days[1].id).toBe('t1');
      expect(third.days[2].id).toBe('t2');
      expect(third.totalHoursWeek).toBe(15.5);
      expect(calendar.calendar.value[3].days[2].id).toBe('t3');
      expect(calendar.calendar.value[3].totalHoursWeek).toBe(0);
      expect(calendar.calendar.value[0].totalHoursWeek).toBe(0);
    });

    it('clears days that are no longer highlighted', async () => {
      const { calendar, highlights } = setup();
      highlights.value = [{ id: 't1', day: '2026-11-16', totalHoursApproved: 8 }];
      await nextTick();
      highlights.value = [];
      await nextTick();

      const day = calendar.calendar.value[2].days[1];
      expect(day.id).toBeNull();
      expect(day.totalHoursApproved).toBe(0);
      expect(dayjs(day.day).format('YYYY-MM-DD')).toBe('2026-11-16');
      expect(calendar.calendar.value[2].totalHoursWeek).toBe(0);
    });

    it('leaves the calendar untouched when highlights become undefined', async () => {
      const { calendar, highlights } = setup();
      highlights.value = [{ id: 't1', day: '2026-11-16', totalHoursApproved: 8 }];
      await nextTick();
      highlights.value = undefined;
      await nextTick();

      expect(calendar.calendar.value[2].days[1].id).toBe('t1');
    });
  });
});
