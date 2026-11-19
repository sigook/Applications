import { describe, expect, it } from 'vitest';
import { buildMultipartFormData } from '@/shared/utils/multipart';

describe('buildMultipartFormData', () => {
  it('serializes the data as JSON under the data key', () => {
    const formData = buildMultipartFormData({ name: 'Acme', active: true });
    expect(formData.get('data')).toBe('{"name":"Acme","active":true}');
    expect([...formData.keys()]).toEqual(['data']);
  });

  it('appends every provided file under its key', () => {
    const logo = new File(['logo'], 'logo.png');
    const contract = new File(['contract'], 'contract.pdf');
    const formData = buildMultipartFormData({ id: 1 }, { logo, contract });
    expect((formData.get('logo') as File).name).toBe('logo.png');
    expect((formData.get('contract') as File).name).toBe('contract.pdf');
  });

  it('skips null and undefined files', () => {
    const formData = buildMultipartFormData(null, { logo: null, contract: undefined });
    expect(formData.get('data')).toBe('null');
    expect(formData.has('logo')).toBe(false);
    expect(formData.has('contract')).toBe(false);
  });
});
