import { describe, expect, it, vi } from 'vitest';
import CompanyInfoCard from '@/modules/agency/recruiting/clients/components/CompanyInfoCard.vue';
import { updateAgencyCompanyEmail } from '@/modules/agency/recruiting/clients/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany } from '@/testing/agencyCompanyFixtures';
import type { AgencyCompanyProfileDetail } from '@/modules/agency/recruiting/clients/types';

const { stub } = vi.hoisted(() => ({
  stub: (name: string, props: string[], emits: string[]) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

vi.mock('@/modules/agency/recruiting/clients/api', () => ({ updateAgencyCompanyEmail: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/components/ContactInformationForm.vue', () => ({
  default: stub('ContactInformationForm', ['model'], ['update:model', 'save']),
}));
vi.mock('@/modules/agency/recruiting/clients/components/DialogCompanyUpdateEmail.vue', () => ({
  default: stub('DialogCompanyUpdateEmail', ['save'], ['closeModal']),
}));
vi.mock('@/modules/agency/recruiting/clients/components/EditVaccinationRequired.vue', () => ({
  default: stub('EditVaccinationRequired', ['companyProfileId', 'vaccinationRequired', 'vaccinationComments'], ['updated']),
}));

function setup(overrides: Partial<AgencyCompanyProfileDetail> = {}) {
  return mountWithPlugins(CompanyInfoCard, { props: { company: createAgencyCompany(overrides) } });
}

function fields(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.detail-field').map((f) => [f.find('dt').text(), f.find('dd').text()]);
}

function fieldButton(wrapper: ReturnType<typeof setup>, label: string) {
  const field = wrapper.findAll('.detail-field').find((f) => f.find('dt').text() === label);
  const button = field?.find('.detail-field-link');
  if (!button?.exists()) throw new Error(`${label} action not found`);
  return button;
}

function updates(wrapper: ReturnType<typeof setup>) {
  return (wrapper.emitted('update:company') ?? []).map(([value]) => value as AgencyCompanyProfileDetail);
}

describe('CompanyInfoCard', () => {
  it('lists the company facts', () => {
    const wrapper = setup({ phoneExt: 12, fax: '416 555-0200', faxExt: 3, vaccinationRequired: true });

    expect(fields(wrapper)).toEqual([
      ['Phone', expect.stringContaining('416 555-0100 Ext. 12') as string],
      ['Fax', '416 555-0200 ext 3'],
      ['Website', 'www.acme.com'],
      ['Email', expect.stringContaining('ops@acme.com') as string],
      ['Industry', 'Logistics'],
      ['Vaccination', expect.stringContaining('Required') as string],
    ]);
    expect(wrapper.find('a[href="https://www.acme.com"]').exists()).toBe(true);
  });

  it('shows dashes for missing facts', () => {
    const wrapper = setup({ phone: null, fax: null, website: '', email: '', industry: null, vaccinationRequired: false });

    expect(fields(wrapper).map(([, value]) => value)).toEqual(['—', '—', '—', '—Change', '—', 'Not requiredEdit']);
  });

  it('shows the vaccination comments only when vaccination is required', () => {
    expect(setup({ vaccinationRequired: true, vaccinationRequiredComments: 'Two doses' }).find('.company-info-note').text()).toBe('Two doses');
    expect(setup({ vaccinationRequired: false, vaccinationRequiredComments: 'Two doses' }).find('.company-info-note').exists()).toBe(false);
  });

  it('edits the contact information and forwards the change', async () => {
    const wrapper = setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Edit')?.trigger('click');
    const form = wrapper.findComponent({ name: 'ContactInformationForm' });
    expect(form.props('model')).toMatchObject({ id: 'company-profile-1' });

    const changed = createAgencyCompany({ website: 'acme.ca' });
    form.vm.$emit('update:model', changed);
    form.vm.$emit('save');
    await wrapper.vm.$nextTick();

    expect(updates(wrapper)).toEqual([changed]);
    expect(modalOpen(wrapper)).toEqual([false, false, false]);
  });

  it('changes the email through the agency endpoint', async () => {
    vi.mocked(updateAgencyCompanyEmail).mockResolvedValue();
    const wrapper = setup();
    await fieldButton(wrapper, 'Email').trigger('click');
    const dialog = wrapper.findComponent({ name: 'DialogCompanyUpdateEmail' });

    await (dialog.props('save') as (email: string) => Promise<void>)('new@acme.com');
    expect(updateAgencyCompanyEmail).toHaveBeenCalledExactlyOnceWith('company-profile-1', { newEmail: 'new@acme.com' });

    dialog.vm.$emit('closeModal', true, 'new@acme.com');
    await wrapper.vm.$nextTick();

    expect(updates(wrapper)).toEqual([createAgencyCompany({ email: 'new@acme.com' })]);
    expect(modalOpen(wrapper)).toEqual([false, false, false]);
  });

  it('keeps the company when the email dialog closes without a new email', async () => {
    const wrapper = setup();
    await fieldButton(wrapper, 'Email').trigger('click');
    wrapper.findComponent({ name: 'DialogCompanyUpdateEmail' }).vm.$emit('closeModal', false, '');
    await wrapper.vm.$nextTick();

    expect(updates(wrapper)).toEqual([]);
  });

  it('edits the vaccination requirement', async () => {
    const wrapper = setup({ vaccinationRequired: true, vaccinationRequiredComments: 'Old' });
    await fieldButton(wrapper, 'Vaccination').trigger('click');
    const editor = wrapper.findComponent({ name: 'EditVaccinationRequired' });
    expect(editor.props()).toEqual({ companyProfileId: 'company-profile-1', vaccinationRequired: true, vaccinationComments: 'Old' });

    editor.vm.$emit('updated', { required: false, comments: null });
    await wrapper.vm.$nextTick();

    expect(updates(wrapper)).toEqual([createAgencyCompany({ vaccinationRequired: false, vaccinationRequiredComments: '' })]);
  });
});

function modalOpen(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAllComponents({ name: 'BModal' }).map((m) => m.props('modelValue') as boolean);
}
