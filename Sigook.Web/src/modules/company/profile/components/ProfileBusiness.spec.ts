import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ProfileBusiness from '@/modules/company/profile/components/ProfileBusiness.vue';
import { updateProfile } from '@/modules/company/profile/api';
import { getIndustries } from '@/shared/api/catalogApi';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany } from '@/testing/agencyCompanyFixtures';
import type { CompanyProfileDetail } from '@/shared/company-profile/types';
import type { Industry } from '@/shared/types/common';

vi.mock('@/modules/company/profile/api', () => ({ updateProfile: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({ getIndustries: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const logistics: Industry = { id: 'i1', value: 'Logistics' };
const food: Industry = { id: 'i2', value: 'Food processing' };

async function setup(overrides: Partial<CompanyProfileDetail> = {}) {
  vi.mocked(getIndustries).mockResolvedValue([logistics, food]);
  const companyData: CompanyProfileDetail = createAgencyCompany({ phoneExt: 12, ...overrides });
  const wrapper = mountWithPlugins(ProfileBusiness, { props: { companyData } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function input(wrapper: Wrapper, name: string) {
  return wrapper.find(`input[name="${name}"]`);
}

function value(wrapper: Wrapper, name: string) {
  return (input(wrapper, name).element as HTMLInputElement).value;
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('button.is-primary').trigger('click');
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

function updated(wrapper: Wrapper): CompanyProfileDetail {
  const payload = wrapper.emitted<[CompanyProfileDetail]>('update:companyData')?.[0]?.[0];
  if (!payload) throw new Error('companyData not emitted');
  return payload;
}

describe('ProfileBusiness', () => {
  it('fills the form with the company', async () => {
    const wrapper = await setup();

    expect(value(wrapper, 'full name')).toBe('ACME LOGISTICS');
    expect(value(wrapper, 'phoneExt')).toBe('12');
    expect(value(wrapper, 'website')).toBe('www.acme.com');
    expect(value(wrapper, 'faxExt')).toBe('');
    expect(value(wrapper, 'phone')).toBe('416 555-0100');
    expect((wrapper.find('.autocomplete input').element as HTMLInputElement).value).toBe('Logistics');
  });

  it('saves the edited business keeping the current industry', async () => {
    vi.mocked(updateProfile).mockResolvedValue();
    const wrapper = await setup();
    await input(wrapper, 'full name').setValue('Acme Foods');
    await input(wrapper, 'phoneExt').setValue('');
    await input(wrapper, 'faxExt').setValue('45');
    await input(wrapper, 'website').setValue('https://acme.ca/jobs');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('saved')).toHaveLength(1); });
    const payload = updated(wrapper);
    expect(payload).toMatchObject({
      id: 'company-profile-1',
      fullName: 'Acme Foods',
      phoneExt: null,
      faxExt: 45,
      website: 'https://acme.ca/jobs',
      industry: { id: 'ind-1', industry: logistics },
    });
    expect(updateProfile).toHaveBeenCalledExactlyOnceWith('company-profile-1', payload);
    expect(showAlertSuccess).toHaveBeenCalledWith('Profile updated');
  });

  it('saves the selected industry', async () => {
    vi.mocked(updateProfile).mockResolvedValue();
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BAutocomplete' }).vm.$emit('select', food);
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('saved')).toHaveLength(1); });
    expect(updated(wrapper).industry).toEqual({ id: 'ind-1', industry: food });
  });

  it('saves without industry when none is selected', async () => {
    vi.mocked(updateProfile).mockResolvedValue();
    const wrapper = await setup({ industry: null });
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('saved')).toHaveLength(1); });
    expect(updated(wrapper).industry).toBeNull();
  });

  it('saves without industry when the industry text is cleared', async () => {
    vi.mocked(updateProfile).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('.autocomplete input').setValue('');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('saved')).toHaveLength(1); });
    expect(updated(wrapper).industry).toBeNull();
  });

  it('filters the industries by the typed text', async () => {
    const wrapper = await setup();
    await wrapper.find('.autocomplete input').setValue('FOOD');

    expect(wrapper.findComponent({ name: 'BAutocomplete' }).props('data')).toEqual([food]);
  });

  it('validates name, extensions and website', async () => {
    const wrapper = await setup();
    await input(wrapper, 'full name').setValue('');
    await input(wrapper, 'phoneExt').setValue('12a');
    await input(wrapper, 'website').setValue('not a url');
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(expect.arrayContaining(['Full name is required', 'Must be 1-8 digits', 'Invalid URL'])); });
    expect(updateProfile).not.toHaveBeenCalled();
    expect(wrapper.emitted('update:companyData')).toBeUndefined();
  });

  it('requires the phone', async () => {
    const wrapper = await setup({ phone: null });
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Phone is required'); });
    expect(updateProfile).not.toHaveBeenCalled();
  });

  it('refills the form when the company changes', async () => {
    const wrapper = await setup();
    await wrapper.setProps({ companyData: createAgencyCompany({ fullName: 'Other Co', faxExt: 9, website: '' }) });

    expect(value(wrapper, 'full name')).toBe('Other Co');
    expect(value(wrapper, 'faxExt')).toBe('9');
    expect(value(wrapper, 'website')).toBe('');
  });

  it('shows the error when saving fails', async () => {
    vi.mocked(updateProfile).mockRejectedValue({ data: 'save failed' });
    const wrapper = await setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledExactlyOnceWith('save failed'); });
    expect(wrapper.emitted('saved')).toBeUndefined();
  });
});
