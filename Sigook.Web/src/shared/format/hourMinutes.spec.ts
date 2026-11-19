import { describe, expect, it } from 'vitest';
import hourMinutes from '@/shared/format/hourMinutes';

describe('hourMinutes', () => {
  it('drops the seconds from a time string', () => {
    expect(hourMinutes('08:30:00')).toBe('08:30');
  });

  it('keeps an hour and minute string unchanged', () => {
    expect(hourMinutes('17:05')).toBe('17:05');
  });

  it('adds zero minutes when the value has only the hour', () => {
    expect(hourMinutes('08')).toBe('08:00');
  });

  it('returns an empty string for empty values', () => {
    expect(hourMinutes('')).toBe('');
    expect(hourMinutes(null)).toBe('');
    expect(hourMinutes(undefined)).toBe('');
  });
});
