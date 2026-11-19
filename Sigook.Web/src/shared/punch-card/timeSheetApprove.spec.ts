import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { buildTimeSheetApproveModel, formatTimeSpan } from '@/shared/punch-card/timeSheetApprove';
import type { PunchCardDay } from '@/shared/punch-card/types';

function day(overrides: Partial<PunchCardDay> = {}): PunchCardDay {
  return { id: 'ts-1', day: '2026-11-18', totalHoursApproved: null, ...overrides };
}

beforeEach(() => {
  vi.useFakeTimers();
  vi.setSystemTime(new Date(2026, 10, 19, 15, 0, 0));
});

afterEach(() => {
  vi.useRealTimers();
});

describe('buildTimeSheetApproveModel', () => {
  it('converts decimal total hours to an HH:mm:ss duration', () => {
    expect(buildTimeSheetApproveModel(day({ totalHours: 7.5 })).hours).toBe('07:30:00');
    expect(buildTimeSheetApproveModel(day({ totalHours: 8.25 })).hours).toBe('08:15:00');
  });

  it.each([
    [new Date(2026, 2, 8, 15, 0, 0)],
    [new Date(2026, 10, 1, 15, 0, 0)],
  ])('is not affected by a daylight saving change on %s', (now) => {
    vi.setSystemTime(now);
    expect(buildTimeSheetApproveModel(day({ totalHours: 8 })).hours).toBe('08:00:00');
  });

  it('uses zero hours when total hours is missing', () => {
    expect(buildTimeSheetApproveModel(day()).hours).toBe('00:00:00');
  });

  it('formats time in as a local datetime without offset', () => {
    expect(buildTimeSheetApproveModel(day({ timeIn: '2026-11-18T12:30:00Z' })).timeIn).toBe('2026-11-18T07:30:00');
  });

  it('copies the adjustment fields', () => {
    const model = buildTimeSheetApproveModel(day({
      totalHours: 8,
      timeIn: '2026-11-18T08:00:00',
      missingHours: '01:00:00',
      missingHoursOvertime: '00:30:00',
      missingRateWorker: 18.5,
      missingRateAgency: 24,
      deductionsOthers: 10,
      bonusOrOthers: 25,
      deductionsOthersDescription: 'Boots',
      bonusOrOthersDescription: 'Referral',
    }));
    expect(model).toEqual({
      hours: '08:00:00',
      timeIn: '2026-11-18T08:00:00',
      missingHours: '01:00:00',
      missingHoursOvertime: '00:30:00',
      missingRateWorker: 18.5,
      missingRateAgency: 24,
      deductionsOthers: 10,
      bonusOrOthers: 25,
      deductionsOthersDescription: 'Boots',
      bonusOrOthersDescription: 'Referral',
    });
  });

  it('leaves missing adjustment fields undefined', () => {
    const model = buildTimeSheetApproveModel(day({ totalHours: 4, timeIn: '2026-11-18T08:00:00' }));
    expect(model.missingHours).toBeUndefined();
    expect(model.bonusOrOthers).toBeUndefined();
    expect(model.deductionsOthersDescription).toBeUndefined();
  });
});

describe('formatTimeSpan', () => {
  it.each([
    [0, '00:00:00'],
    [0.5, '00:30:00'],
    [7.75, '07:45:00'],
    [8 + 1 / 60, '08:01:00'],
    [23.99, '23:59:24'],
    [24, '1.00:00:00'],
    [26.5, '1.02:30:00'],
  ])('formats %s hours as %s', (hours, expected) => {
    expect(formatTimeSpan(hours)).toBe(expected);
  });
});
