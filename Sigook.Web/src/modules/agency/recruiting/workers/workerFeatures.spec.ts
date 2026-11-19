import { describe, expect, it } from 'vitest';
import { workerFeatures } from '@/modules/agency/recruiting/workers/workerFeatures';

describe('workerFeatures', () => {
  it('lists the worker filters with sequential ids', () => {
    expect(workerFeatures.map((feature) => feature.id)).toEqual([0, 1, 2, 3, 4]);
    expect(workerFeatures.map((feature) => feature.value)).toEqual([
      'Working',
      'Not Working',
      'DNU',
      'Approved To Work',
      'Subcontractor',
    ]);
  });

  it('has unique labels', () => {
    expect(new Set(workerFeatures.map((feature) => feature.value)).size).toBe(workerFeatures.length);
  });
});
