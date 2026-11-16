import { afterEach, describe, expect, it, vi } from 'vitest';
import {
  calculateAttendanceHours,
  defaultLunchMinutes,
  formatElapsed,
  formatHours,
  isWeekend,
  toLocalDateTime,
} from '@/modules/agency/profile/attendance';

const WEDNESDAY = '2026-01-14';
const SATURDAY = '2026-01-17';
const SUNDAY = '2026-01-18';

describe('isWeekend / defaultLunchMinutes', () => {
  it.each([
    [WEDNESDAY, false, 60],
    [SATURDAY, true, 0],
    [SUNDAY, true, 0],
  ])('%s weekend=%s lunch=%d', (date, weekend, lunch) => {
    expect(isWeekend(date)).toBe(weekend);
    expect(defaultLunchMinutes(date)).toBe(lunch);
  });
});

describe('calculateAttendanceHours', () => {
  it('deducts the default lunch and splits overtime above 8 hours on weekdays', () => {
    expect(calculateAttendanceHours(`${WEDNESDAY}T08:00:00`, `${WEDNESDAY}T17:30:00`)).toEqual({
      workedHours: 8.5,
      lunchHours: 1,
      regularHours: 8,
      overtimeHours: 0.5,
    });
  });

  it('keeps everything as regular time up to 8 hours', () => {
    expect(calculateAttendanceHours(`${WEDNESDAY}T09:00:00`, `${WEDNESDAY}T15:00:00`)).toEqual({
      workedHours: 5,
      lunchHours: 1,
      regularHours: 5,
      overtimeHours: 0,
    });
  });

  it('counts all weekend hours as overtime with no default lunch', () => {
    expect(calculateAttendanceHours(`${SATURDAY}T08:00:00`, `${SATURDAY}T14:00:00`)).toEqual({
      workedHours: 6,
      lunchHours: 0,
      regularHours: 0,
      overtimeHours: 6,
    });
  });

  it('uses an explicit lunch length', () => {
    expect(calculateAttendanceHours(`${WEDNESDAY}T08:00:00`, `${WEDNESDAY}T16:30:00`, 30).workedHours).toBe(8);
  });

  it('never deducts more lunch than the elapsed time', () => {
    expect(calculateAttendanceHours(`${WEDNESDAY}T08:00:00`, `${WEDNESDAY}T08:30:00`)).toEqual({
      workedHours: 0,
      lunchHours: 0.5,
      regularHours: 0,
      overtimeHours: 0,
    });
  });

  it('returns zeros when clock-out is not after clock-in', () => {
    const zero = { workedHours: 0, lunchHours: 0, regularHours: 0, overtimeHours: 0 };
    expect(calculateAttendanceHours(`${WEDNESDAY}T08:00:00`, `${WEDNESDAY}T08:00:00`)).toEqual(zero);
    expect(calculateAttendanceHours(`${WEDNESDAY}T17:00:00`, `${WEDNESDAY}T08:00:00`)).toEqual(zero);
  });
});

describe('formatHours', () => {
  it('formats with two decimals', () => {
    expect(formatHours(8.5)).toBe('8.50');
    expect(formatHours(null)).toBe('0.00');
  });
});

describe('formatElapsed', () => {
  afterEach(() => {
    vi.useRealTimers();
  });

  it('formats elapsed time as HH:mm', () => {
    expect(formatElapsed(`${WEDNESDAY}T08:00:00`, `${WEDNESDAY}T17:05:00`)).toBe('09:05');
  });

  it('never returns negative time', () => {
    expect(formatElapsed(`${WEDNESDAY}T09:00:00`, `${WEDNESDAY}T08:00:00`)).toBe('00:00');
  });

  it('measures until now by default', () => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date(`${WEDNESDAY}T10:30:00`));
    expect(formatElapsed(`${WEDNESDAY}T08:00:00`)).toBe('02:30');
  });
});

describe('toLocalDateTime', () => {
  it('formats without timezone', () => {
    expect(toLocalDateTime(new Date(2026, 0, 14, 8, 5, 9))).toBe('2026-01-14T08:05:09');
  });
});
