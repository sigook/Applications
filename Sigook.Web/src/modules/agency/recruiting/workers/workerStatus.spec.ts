import { describe, expect, it } from 'vitest';
import { workerColor } from '@/modules/agency/recruiting/workers/workerStatus';

describe('workerColor', () => {
  it.each([
    [false, false, 'Rejected'],
    [false, true, 'Rejected'],
    [true, true, 'Blue'],
    [true, false, undefined],
  ])('approved=%s subcontractor=%s -> %s', (approved, subcontractor, expected) => {
    expect(workerColor(approved, subcontractor)).toBe(expected);
  });
});
