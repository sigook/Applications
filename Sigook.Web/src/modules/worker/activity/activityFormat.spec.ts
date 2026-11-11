import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import {
  breakLabel,
  hoursLabel,
  isManualEntry,
  spanMinutes,
  statusTagType,
  timeSheetRangeLabel,
  timeSheetStatusLabel,
  weekStartOf,
} from '@/modules/worker/activity/activityFormat';
import { WorkerTimeSheetStatus, type WorkerWeekTimeSheet } from '@/modules/worker/activity/types';

function timeSheet(overrides: Partial<WorkerWeekTimeSheet> = {}): WorkerWeekTimeSheet {
  return {
    id: '1',
    requestId: '1',
    date: '2026-01-15T00:00:00',
    clientName: 'Acme',
    jobTitle: 'Packer',
    clockIn: '2026-01-15T08:00:00',
    clockOut: '2026-01-15T16:30:00',
    durationBreak: '00:30:00',
    breakIsPaid: false,
    regularHours: 8,
    overtimeHours: 0,
    holidayHours: 0,
    totalHours: 8,
    status: WorkerTimeSheetStatus.Approved,
    ...overrides,
  } as WorkerWeekTimeSheet;
}

describe('hoursLabel', () => {
  it('formats with one decimal', () => {
    expect(hoursLabel(7.26)).toBe('7.3');
    expect(hoursLabel(null)).toBe('0.0');
  });
});

describe('weekStartOf', () => {
  it('returns the Sunday that starts the week', () => {
    expect(weekStartOf('2026-01-15')).toBe('2026-01-11');
    expect(weekStartOf('2026-01-11')).toBe('2026-01-11');
  });
});

describe('statusTagType', () => {
  it.each([
    ['Open', 'is-info is-light'],
    ['Pending', 'is-warning is-light'],
    ['Paid', 'is-success'],
    ['No clock-out', 'is-danger is-light'],
    ['Ended', ''],
    ['Unknown', ''],
    [undefined, ''],
  ])('%s -> "%s"', (status, expected) => {
    expect(statusTagType(status)).toBe(expected);
  });
});

describe('timeSheetStatusLabel', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date('2026-01-15T12:00:00'));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('flags an open time sheet from a past day as missing clock-out', () => {
    expect(timeSheetStatusLabel(timeSheet({ status: WorkerTimeSheetStatus.Open, date: '2026-01-14T00:00:00' }))).toBe('No clock-out');
  });

  it('keeps today open time sheets as open', () => {
    expect(timeSheetStatusLabel(timeSheet({ status: WorkerTimeSheetStatus.Open }))).toBe('Open');
  });

  it('uses the status name otherwise', () => {
    expect(timeSheetStatusLabel(timeSheet({ status: WorkerTimeSheetStatus.Approved, date: '2026-01-01T00:00:00' }))).toBe('Approved');
  });
});

describe('time sheet range', () => {
  it('shows clock-in and clock-out times', () => {
    expect(timeSheetRangeLabel(timeSheet())).toBe('08:00 – 16:30');
  });

  it('shows a dash for a missing clock-out', () => {
    expect(timeSheetRangeLabel(timeSheet({ clockOut: null }))).toBe('08:00 – —');
  });

  it('labels entries without clock times as manual', () => {
    const manual = timeSheet({ clockIn: null, clockOut: null });
    expect(isManualEntry(manual)).toBe(true);
    expect(timeSheetRangeLabel(manual)).toBe('Manual entry');
  });
});

describe('breaks', () => {
  it.each([
    ['01:15:00', 75],
    ['00:30', 30],
    ['', 0],
    [null, 0],
  ])('spanMinutes(%s) -> %d', (value, expected) => {
    expect(spanMinutes(value)).toBe(expected);
  });

  it('labels paid and unpaid breaks', () => {
    expect(breakLabel(timeSheet())).toBe('30 min');
    expect(breakLabel(timeSheet({ breakIsPaid: true }))).toBe('30 min (paid)');
    expect(breakLabel(timeSheet({ durationBreak: '00:00:00' }))).toBe('—');
  });
});
