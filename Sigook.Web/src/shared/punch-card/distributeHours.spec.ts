import { describe, expect, it } from 'vitest';
import { distributeHours } from '@/shared/punch-card/distributeHours';

describe('distributeHours', () => {
  it('returns nothing when the daily average exceeds the maximum', () => {
    expect(distributeHours(5, 61, 12)).toEqual([]);
  });

  it('puts the whole week on the first day when it fits in one day', () => {
    const hours = distributeHours(5, 6, 12);
    expect(hours).toHaveLength(5);
    expect(hours[0]).toBe(6);
    expect(hours.slice(1)).toEqual([undefined, undefined, undefined, undefined]);
  });

  it('fills every day with the maximum when the average equals it', () => {
    expect(distributeHours(5, 60, 12)).toEqual([12, 12, 12, 12, 12]);
  });

  it.each([
    [5, 40, [8, 8, 8, 8, 8]],
    [5, 30, [6, 6, 6, 6, 6]],
    [7, 14, [2, 2, 2, 2, 2, 2, 2]],
  ])('splits %d days / %d hours evenly when the average is a whole number', (days, total, expected) => {
    expect(distributeHours(days, total, 12)).toEqual(expected);
  });

  it('rounds a fractional average up and leaves the remainder on the last day', () => {
    expect(distributeHours(5, 37, 8)).toEqual([8, 8, 8, 8, 5]);
  });

  it('splits evenly when the average is below one hour', () => {
    expect(distributeHours(5, 3, 2)).toEqual([0.6, 0.6, 0.6, 0.6, 0.6]);
  });

  it('never assigns more hours than the weekly total', () => {
    const hours = distributeHours(5, 37, 8);
    expect(hours.reduce((sum, h) => sum + h, 0)).toBe(37);
  });
});
