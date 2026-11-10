import { describe, expect, it } from 'vitest';
import { validateUploadFile } from '@/shared/utils/fileValidation';

function fileOf(name: string, sizeKb: number): File {
  const file = new File([''], name);
  Object.defineProperty(file, 'size', { value: sizeKb * 1024 });
  return file;
}

describe('validateUploadFile', () => {
  it('requires a file', () => {
    expect(validateUploadFile(null)).toBe('File is required');
  });

  it.each(['cv.pdf', 'photo.JPG', 'scan.png', 'sheet.xlsx', 'letter.docx'])('accepts %s', (name) => {
    expect(validateUploadFile(fileOf(name, 10))).toBe('');
  });

  it.each(['script.exe', 'archive.zip', 'noextension'])('rejects extension of %s', (name) => {
    expect(validateUploadFile(fileOf(name, 10))).toMatch(/^File extension not allowed/);
  });

  it('accepts files at the size limit', () => {
    expect(validateUploadFile(fileOf('cv.pdf', 15500))).toBe('');
  });

  it('rejects files over the size limit', () => {
    expect(validateUploadFile(fileOf('cv.pdf', 15501))).toBe('File size must be less than 15500 KB');
  });
});
