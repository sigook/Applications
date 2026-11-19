import { afterEach, describe, expect, it, vi } from 'vitest';

type CatalogModule = typeof import('@/shared/constants/catalog');

async function loadCatalog(): Promise<CatalogModule> {
  vi.resetModules();
  return import('@/shared/constants/catalog');
}

afterEach(() => {
  vi.unstubAllEnvs();
});

describe('catalog', () => {
  it('reads the maximum hours per day from the environment', async () => {
    vi.stubEnv('VUE_APP_MAXIMUM_HOURS_DAY', '10');
    expect((await loadCatalog()).maximumHoursPerDay).toBe(10);
  });

  it('defaults the maximum hours per day to 12', async () => {
    vi.stubEnv('VUE_APP_MAXIMUM_HOURS_DAY', '');
    expect((await loadCatalog()).maximumHoursPerDay).toBe(12);
  });

  it('defaults the maximum hours per day to 12 when the value is not a number', async () => {
    vi.stubEnv('VUE_APP_MAXIMUM_HOURS_DAY', 'twelve');
    expect((await loadCatalog()).maximumHoursPerDay).toBe(12);
  });

  it('lists the residency statuses in display order', async () => {
    expect((await loadCatalog()).residencyList).toEqual(['Citizen', 'Work Permit', 'Student', 'Permanent Resident']);
  });
});
