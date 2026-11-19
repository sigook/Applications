import { describe, expect, it } from 'vitest';
import fileName from '@/shared/format/fileNameFilter';

describe('fileNameFilter', () => {
  it('keeps the first word and the extension in upper case', () => {
    expect(fileName('resume.pdf')).toBe('RESUME.PDF');
    expect(fileName('my resume_final.v2.docx')).toBe('MY.DOCX');
  });

  it('treats repeated separators as one', () => {
    expect(fileName('pay__stub  2026..pdf')).toBe('PAY.PDF');
  });

  it('keeps a name without separators once', () => {
    expect(fileName('README')).toBe('README');
  });

  it('returns null for empty values', () => {
    expect(fileName('')).toBeNull();
    expect(fileName(null)).toBeNull();
    expect(fileName(undefined)).toBeNull();
  });
});
