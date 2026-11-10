import { describe, expect, it, vi } from 'vitest';
import { generateFileName } from '@/shared/utils/fileNaming';

describe('generateFileName', () => {
  it('builds prefix, dashless guid and original extension', () => {
    expect(generateFileName('resume', 'My CV.final.pdf')).toMatch(/^resume_[0-9a-f]{32}\.pdf$/);
  });

  it('omits the extension when the original has none', () => {
    expect(generateFileName('doc', 'README')).toMatch(/^doc_[0-9a-f]{32}$/);
  });

  it('generates unique names', () => {
    expect(generateFileName('a', 'x.png')).not.toBe(generateFileName('a', 'x.png'));
  });

  it('falls back when crypto.randomUUID is unavailable', () => {
    vi.stubGlobal('crypto', undefined);
    expect(generateFileName('img', 'photo.jpg')).toMatch(/^img_[0-9a-f]{32}\.jpg$/);
  });
});
