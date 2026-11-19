import { describe, expect, it } from 'vitest';
import splitCapital from '@/shared/format/splitCapital';

describe('splitCapital', () => {
  it('inserts a space before every capital letter', () => {
    expect(splitCapital('LongTerm')).toBe('Long Term');
    expect(splitCapital('inQueueStatus')).toBe('in Queue Status');
  });

  it('keeps a single word unchanged', () => {
    expect(splitCapital('Open')).toBe('Open');
  });

  it('returns an empty string for empty values', () => {
    expect(splitCapital('')).toBe('');
    expect(splitCapital(null)).toBe('');
    expect(splitCapital(undefined)).toBe('');
  });
});
