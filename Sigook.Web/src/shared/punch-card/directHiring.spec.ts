import { describe, expect, it } from 'vitest';
import { isDirectHiring } from '@/shared/punch-card/directHiring';

describe('isDirectHiring', () => {
  it('is true when the request has a worker salary', () => {
    expect(isDirectHiring({ workerSalary: 55000 })).toBe(true);
  });

  it('is false when the salary is missing or zero', () => {
    expect(isDirectHiring({ workerSalary: 0 })).toBe(false);
    expect(isDirectHiring({ workerSalary: null })).toBe(false);
    expect(isDirectHiring({})).toBe(false);
  });

  it('is false without a request', () => {
    expect(isDirectHiring(null)).toBe(false);
    expect(isDirectHiring(undefined)).toBe(false);
  });
});
