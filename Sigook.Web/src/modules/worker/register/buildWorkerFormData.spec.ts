import { beforeEach, describe, expect, it, vi } from 'vitest';
import { createMultipartFormData } from '@/modules/worker/register/buildWorkerFormData';
import { compressFile } from '@/shared/utils/compressFile';
import type { CreateWorkerState } from '@/modules/worker/register/useCreateWorker';
import type { WorkerFileObjects } from '@/modules/worker/register/types';

vi.mock('@/shared/utils/compressFile', () => ({ compressFile: vi.fn() }));

let sequence = 0;

vi.mock('@/shared/utils/fileNaming', () => ({
  generateFileName: (prefix: string, originalFileName: string) => {
    sequence++;
    const dot = originalFileName.lastIndexOf('.');
    return `${prefix}_${sequence}${dot === -1 ? '' : originalFileName.substring(dot)}`;
  },
}));

function file(name: string, content = 'data'): File {
  return new File([content], name, { type: 'application/octet-stream' });
}

function worker(overrides: Partial<CreateWorkerState> = {}): CreateWorkerState {
  return {
    firstName: 'Ana',
    lastName: 'Lopez',
    birthDay: '1990-05-01',
    gender: { id: 'g1', value: 'Female' },
    email: 'ana@example.com',
    password: 'Secret123!',
    confirmPassword: 'Secret123!',
    mobileNumber: '4165550199',
    lift: { id: 'l1', value: '50 lbs' },
    hasVehicle: true,
    agreeTermsAndConditions: true,
    identificationNumber1: 'A123',
    identificationNumber2: null,
    availabilities: [{ id: 'a1', value: 'Full time' }],
    availabilityTimes: [{ id: 't1', value: 'Morning' }],
    availabilityDays: [{ id: 'd1', value: 'Monday' }],
    skills: [{ skill: 'Forklift' }],
    languages: [{ id: 'en', value: 'English' }],
    location: { cityId: 'c1' },
    identificationType1File: null,
    identificationType1: { id: 'it1', value: 'Passport' },
    identificationType2File: null,
    identificationType2: null,
    licenses: [],
    certificates: [],
    resume: null,
    otherDocuments: [],
    ...overrides,
  };
}

function noFiles(overrides: Partial<WorkerFileObjects> = {}): WorkerFileObjects {
  return {
    profileImage: null,
    identificationType1: null,
    identificationType2: null,
    licenses: [],
    certificates: [],
    resume: null,
    otherDocuments: [],
    ...overrides,
  };
}

function parseData(formData: FormData): Record<string, unknown> {
  return JSON.parse(String(formData.get('data'))) as Record<string, unknown>;
}

function fileKeys(formData: FormData): string[] {
  return [...formData.keys()].filter((key) => key !== 'data');
}

describe('createMultipartFormData', () => {
  beforeEach(() => {
    sequence = 0;
  });

  it('serializes only the worker fields when there are no files', async () => {
    const formData = await createMultipartFormData(worker(), noFiles());
    const data = parseData(formData);

    expect(fileKeys(formData)).toEqual([]);
    expect(data).toMatchObject({
      firstName: 'Ana',
      lastName: 'Lopez',
      email: 'ana@example.com',
      hasVehicle: true,
      skills: [{ skill: 'Forklift' }],
      identificationType1: { id: 'it1', value: 'Passport' },
      identificationType2: null,
    });
    expect(data).not.toHaveProperty('profileImage');
    expect(data).not.toHaveProperty('identificationType1File');
    expect(data).not.toHaveProperty('licenses');
    expect(data).not.toHaveProperty('certificates');
    expect(data).not.toHaveProperty('resume');
    expect(data).not.toHaveProperty('otherDocuments');
  });

  it('does not send unknown worker keys', async () => {
    const data = parseData(await createMultipartFormData(worker({ extra: 'x' }), noFiles()));
    expect(data).not.toHaveProperty('extra');
  });

  it('appends the compressed profile image under its generated name', async () => {
    const compressed = new Blob(['small'], { type: 'image/jpeg' });
    vi.mocked(compressFile).mockResolvedValue(compressed);

    const formData = await createMultipartFormData(worker(), noFiles({ profileImage: file('me.jpg', 'large-image') }));

    expect(parseData(formData).profileImage).toEqual({ fileName: 'ProfileImage_1.jpg' });
    const appended = formData.get('ProfileImage_1.jpg') as File;
    expect(await appended.text()).toBe('small');
  });

  it('falls back to the original profile image when compression fails', async () => {
    vi.mocked(compressFile).mockRejectedValue('boom');

    const formData = await createMultipartFormData(worker(), noFiles({ profileImage: file('me.png', 'original') }));

    const appended = formData.get('ProfileImage_1.png') as File;
    expect(await appended.text()).toBe('original');
  });

  it('links identification files with their descriptions', async () => {
    const formData = await createMultipartFormData(
      worker({
        identificationType1File: { fileName: 'id1.pdf', description: 'Front' },
        identificationType2File: null,
      }),
      noFiles({ identificationType1: file('id1.pdf'), identificationType2: file('id2.pdf') }),
    );
    const data = parseData(formData);

    expect(data.identificationType1File).toEqual({ fileName: 'Document_1.pdf', description: 'Front' });
    expect(data.identificationType2File).toEqual({ fileName: 'Document_2.pdf', description: '' });
    expect(fileKeys(formData)).toEqual(['Document_1.pdf', 'Document_2.pdf']);
  });

  it('pairs licenses, certificates and other documents with their files by index', async () => {
    const issued = new Date('2025-01-10T00:00:00');
    const formData = await createMultipartFormData(
      worker({
        licenses: [
          { license: { fileName: 'a.pdf', description: 'Forklift' }, doesExpire: false, number: 'L1', issued, expires: null },
          { license: { fileName: 'b.pdf', description: 'G2' }, doesExpire: true, number: 'L2' },
        ],
        certificates: [{ fileName: 'c.pdf', description: 'WHMIS' }],
        otherDocuments: [{ fileName: 'o.docx', description: 'Reference' }],
      }),
      noFiles({
        licenses: [file('a.pdf'), file('b.pdf')],
        certificates: [file('c.pdf')],
        otherDocuments: [file('o.docx')],
      }),
    );
    const data = parseData(formData);

    expect(data.licenses).toEqual([
      { license: { fileName: 'License_1.pdf', description: 'Forklift' }, number: 'L1', issued: issued.toISOString(), expires: null },
      { license: { fileName: 'License_2.pdf', description: 'G2' }, number: 'L2' },
    ]);
    expect(data.certificates).toEqual([{ fileName: 'Certificate_3.pdf', description: 'WHMIS' }]);
    expect(data.otherDocuments).toEqual([{ fileName: 'OtherDocument_4.docx', description: 'Reference' }]);
    expect(fileKeys(formData)).toEqual(['License_1.pdf', 'License_2.pdf', 'Certificate_3.pdf', 'OtherDocument_4.docx']);
  });

  it('attaches the resume', async () => {
    const formData = await createMultipartFormData(
      worker({ resume: { fileName: 'cv.pdf' } }),
      noFiles({ resume: file('cv.pdf', 'resume') }),
    );

    expect(parseData(formData).resume).toEqual({ fileName: 'Resume_1.pdf' });
    expect(await (formData.get('Resume_1.pdf') as File).text()).toBe('resume');
  });

  it('keeps the file name without extension when the original has none', async () => {
    const formData = await createMultipartFormData(
      worker({ resume: { fileName: 'cv' } }),
      noFiles({ resume: file('cv') }),
    );

    expect(parseData(formData).resume).toEqual({ fileName: 'Resume_1' });
  });
});
