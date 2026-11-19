import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { defineComponent } from 'vue';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import CreateClient from '@/modules/agency/recruiting/clients/pages/CreateClient.vue';
import { createAgencyCompany as createCompanyApi, getAgencyCompany, updateAgencyCompany, updateAgencyCompanyProfileLogo } from '@/modules/agency/recruiting/clients/api';
import { addIndustry, getCompanyStatus, getIndustries } from '@/shared/api/catalogApi';
import { getAgencyPersonnel } from '@/modules/agency/profile/api';
import { useAgencyStore } from '@/modules/agency/store';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { CompanyStatus } from '@/shared/constants/enums';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany } from '@/testing/agencyCompanyFixtures';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';
import type { CompanyProfileFormState } from '@/modules/agency/recruiting/clients/types';
import type { Industry } from '@/shared/types/common';

const { route, push, securityStore } = vi.hoisted(() => ({
  route: { path: '/recruiting/companies/create', params: {} as Record<string, string>, query: {} },
  push: vi.fn(),
  securityStore: { userRoles: [] as string[] },
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
  useRouter: () => ({ push }),
}));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  createAgencyCompany: vi.fn(),
  getAgencyCompany: vi.fn(),
  updateAgencyCompany: vi.fn(),
  updateAgencyCompanyProfileLogo: vi.fn(),
}));
vi.mock('@/shared/api/catalogApi', () => ({ getIndustries: vi.fn(), getCompanyStatus: vi.fn(), addIndustry: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({ getAgencyPersonnel: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => ({ getDialog: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/ui/PreviewImage.vue', () => ({
  default: { name: 'PreviewImage', props: { required: Boolean }, emits: ['imageSelected'], template: '<div class="preview-image-stub" />' },
}));

const QuillEditorStub = defineComponent({
  name: 'QuillEditor',
  props: { content: { type: String, default: '' } },
  emits: ['update:content'],
  template: '<div class="quill-stub" />',
});

interface PromptConfig {
  inputAttrs?: Record<string, unknown>;
  onConfirm?: (value: string, dialog: { close: () => void }) => void | Promise<void>;
}

const logistics: Industry = { id: 'i1', value: 'Logistics' };
const retail: Industry = { id: 'i2', value: 'Retail' };
const rita: AgencyPersonnelListItem = { id: 'p1', userId: 'u1', name: 'Rita Sales', email: 'rita@sigook.com', role: 'sales', agencyIds: ['agency-1'], agencies: ['Covenant'] };
const sam: AgencyPersonnelListItem = { id: 'p2', userId: 'u2', name: 'Sam Seller', email: 'sam@sigook.com', role: 'sales', agencyIds: ['agency-2'], agencies: ['Sigook'] };
const statuses = [
  { id: CompanyStatus.Lead, value: 'Lead' },
  { id: CompanyStatus.Client, value: 'Client' },
];
const logo = new File(['png'], 'logo.png', { type: 'image/png' });
const prompt = vi.fn<(config: PromptConfig) => void>();

let pinia: Pinia;

async function setup() {
  const wrapper = mountWithPlugins(CreateClient, { global: { plugins: [pinia], components: { QuillEditor: QuillEditorStub } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function input(wrapper: Wrapper, name: string) {
  return wrapper.find(`[name="${name}"]`);
}

function value(wrapper: Wrapper, name: string) {
  return (input(wrapper, name).element as HTMLInputElement).value;
}

function autocomplete(wrapper: Wrapper, index: number) {
  return wrapper.findAllComponents({ name: 'BAutocomplete' })[index];
}

async function chooseIndustry(wrapper: Wrapper, option: Industry, text = option.value) {
  autocomplete(wrapper, 0).vm.$emit('update:modelValue', text);
  autocomplete(wrapper, 0).vm.$emit('select', option);
  await flushPromises();
}

async function chooseSalesRep(wrapper: Wrapper, option: AgencyPersonnelListItem) {
  autocomplete(wrapper, 1).vm.$emit('update:modelValue', `${option.name} - ${option.email}`);
  autocomplete(wrapper, 1).vm.$emit('select', option);
  await flushPromises();
}

async function fillRequired(wrapper: Wrapper, status = CompanyStatus.Lead) {
  await input(wrapper, 'full name').setValue('Globex');
  await chooseIndustry(wrapper, logistics);
  await input(wrapper, 'state').setValue(String(status));
  await chooseSalesRep(wrapper, rita);
  await input(wrapper, 'email').setValue('ops@globex.com');
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
  await vi.waitFor(() => {
    expect([showAlertError, showAlertSuccess].some((fn) => vi.mocked(fn).mock.calls.length > 0)).toBe(true);
  });
  await flushPromises();
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

function payload(mock: typeof createCompanyApi | typeof updateAgencyCompany): CompanyProfileFormState {
  const call = vi.mocked(mock).mock.lastCall;
  const last = call?.at(-1);
  if (!last || typeof last === 'string') throw new Error('not called');
  return last;
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  route.path = '/recruiting/companies/create';
  route.params = {};
  securityStore.userRoles = ['recruiting'];
  useAgencyStore().setAgencies([{ id: 'agency-1', name: 'Covenant' }]);
  vi.mocked(getCompanyStatus).mockResolvedValue(statuses);
  vi.mocked(getIndustries).mockResolvedValue([logistics, retail]);
  vi.mocked(getAgencyPersonnel).mockResolvedValue([rita]);
  vi.mocked(getDialog).mockReturnValue({ alert: vi.fn(), confirm: vi.fn(), prompt });
});

describe('CreateClient', () => {
  it('loads the catalogs and the sales reps of the first agency', async () => {
    const wrapper = await setup();

    expect(getAgencyCompany).not.toHaveBeenCalled();
    expect(getAgencyPersonnel).toHaveBeenCalledExactlyOnceWith('agency-1');
    expect(wrapper.find('.page-header-title').text()).toBe('Create Client');
    expect(wrapper.findAll('select[name="state"] option').map((o) => o.text())).toEqual(['Select option', 'Lead', 'Client']);
    expect(autocomplete(wrapper, 0).props('data')).toEqual([logistics, retail]);
    expect(autocomplete(wrapper, 1).props('data')).toEqual([rita]);
    expect(input(wrapper, 'agency').exists()).toBe(false);
    expect(input(wrapper, 'email').exists()).toBe(true);
    expect(input(wrapper, 'password').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'BSwitch' }).exists()).toBe(false);
  });

  it('filters the industries and sales reps by what is typed', async () => {
    vi.mocked(getAgencyPersonnel).mockResolvedValue([rita, sam]);
    const wrapper = await setup();
    autocomplete(wrapper, 0).vm.$emit('update:modelValue', 'ret');
    autocomplete(wrapper, 1).vm.$emit('update:modelValue', 'sam@');
    await flushPromises();

    expect(autocomplete(wrapper, 0).props('data')).toEqual([retail]);
    expect(autocomplete(wrapper, 1).props('data')).toEqual([sam]);
  });

  it('requires the general fields and the email', async () => {
    const wrapper = await setup();
    await submit(wrapper);

    await vi.waitFor(() => {
      expect(errors(wrapper)).toEqual([
        'Full name is required',
        'Industry is required',
        'Status is required',
        'Sales representative is required',
        'Email is required',
      ]);
    });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createCompanyApi).not.toHaveBeenCalled();
  });

  it('asks for a password when creating a client', async () => {
    const wrapper = await setup();
    await input(wrapper, 'state').setValue(String(CompanyStatus.Client));
    await flushPromises();

    expect(input(wrapper, 'password').exists()).toBe(true);
    await submit(wrapper);
    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Password is required'); });
  });

  it.each([
    ['industry', 0, 'Mining'],
    ['sales representative', 1, 'Nobody'],
  ])('rejects a typed %s that was not picked from the list', async (_, index, text) => {
    const wrapper = await setup();
    await fillRequired(wrapper);
    autocomplete(wrapper, index).vm.$emit('update:modelValue', text);
    await flushPromises();
    await submit(wrapper);

    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createCompanyApi).not.toHaveBeenCalled();
  });

  it('validates the optional fields', async () => {
    const wrapper = await setup();
    await fillRequired(wrapper);
    await input(wrapper, 'about').setValue('x');
    await input(wrapper, 'phoneExt').setValue('ab');
    await input(wrapper, 'website').setValue('not a url');
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Min 2 characters', 'Must be 1-8 digits', 'Invalid URL']); });
    expect(createCompanyApi).not.toHaveBeenCalled();
  });

  it('creates a lead, uploads its logo and opens it', async () => {
    vi.mocked(createCompanyApi).mockResolvedValue({ id: 'new-1' });
    vi.mocked(updateAgencyCompanyProfileLogo).mockResolvedValue();
    const wrapper = await setup();
    await fillRequired(wrapper);
    await input(wrapper, 'phoneExt').setValue('12');
    await input(wrapper, 'website').setValue('globex.com');
    await input(wrapper, 'phone').setValue('4165550100');
    wrapper.findComponent(QuillEditorStub).vm.$emit('update:content', '<p>VIP</p>');
    wrapper.findComponent({ name: 'PreviewImage' }).vm.$emit('imageSelected', logo);
    await submit(wrapper);

    expect(payload(createCompanyApi)).toEqual({
      companyProfileId: null,
      logo: {},
      requiresPermissionToSeeRequests: false,
      internalInfo: '<p>VIP</p>',
      phone: '416 555-0100',
      fullName: 'Globex',
      companyStatus: CompanyStatus.Lead,
      about: '',
      phoneExt: 12,
      faxExt: null,
      website: 'globex.com',
      salesRepresentativeId: 'p1',
      industry: { industry: logistics },
      email: 'ops@globex.com',
      agencyId: 'agency-1',
    });
    expect(updateAgencyCompanyProfileLogo).toHaveBeenCalledExactlyOnceWith('new-1', logo);
    expect(showAlertSuccess).toHaveBeenCalledWith('Company created');
    expect(push).toHaveBeenCalledWith('/recruiting/companies/new-1');
  });

  it('creates a client with its password and no logo', async () => {
    vi.mocked(createCompanyApi).mockResolvedValue({ id: 'new-2' });
    const wrapper = await setup();
    await fillRequired(wrapper, CompanyStatus.Client);
    await input(wrapper, 'password').setValue('secret123');
    await submit(wrapper);

    expect(payload(createCompanyApi)).toMatchObject({ companyStatus: CompanyStatus.Client, password: 'secret123' });
    expect(updateAgencyCompanyProfileLogo).not.toHaveBeenCalled();
  });

  it('shows the server message when creating fails', async () => {
    vi.mocked(createCompanyApi).mockRejectedValue({ data: 'Email already in use' });
    const wrapper = await setup();
    await fillRequired(wrapper);
    await submit(wrapper);

    expect(showAlertError).toHaveBeenCalledWith('Email already in use');
    expect(push).not.toHaveBeenCalled();
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('lets admins require permission to see requests', async () => {
    securityStore.userRoles = ['admin'];
    vi.mocked(createCompanyApi).mockResolvedValue({ id: 'new-3' });
    const wrapper = await setup();
    await fillRequired(wrapper);
    await wrapper.findComponent({ name: 'BSwitch' }).find('input').setValue(true);
    await submit(wrapper);

    expect(payload(createCompanyApi).requiresPermissionToSeeRequests).toBe(true);
  });

  it('picks the agency when the user has several and reloads its sales reps', async () => {
    useAgencyStore().setAgencies([{ id: 'agency-1', name: 'Covenant' }, { id: 'agency-2', name: 'Sigook' }]);
    vi.mocked(createCompanyApi).mockResolvedValue({ id: 'new-4' });
    const wrapper = await setup();
    await chooseSalesRep(wrapper, rita);

    vi.mocked(getAgencyPersonnel).mockResolvedValue([sam]);
    await input(wrapper, 'agency').setValue('agency-2');
    await flushPromises();

    expect(getAgencyPersonnel).toHaveBeenLastCalledWith('agency-2');
    expect(autocomplete(wrapper, 1).props('modelValue')).toBe('');
    expect(autocomplete(wrapper, 1).props('data')).toEqual([sam]);

    await fillRequired(wrapper);
    await chooseSalesRep(wrapper, sam);
    await submit(wrapper);
    expect(payload(createCompanyApi)).toMatchObject({ agencyId: 'agency-2', salesRepresentativeId: 'p2' });
  });

  it('shows the error when the sales reps of an agency cannot be loaded', async () => {
    useAgencyStore().setAgencies([{ id: 'agency-1', name: 'Covenant' }, { id: 'agency-2', name: 'Sigook' }]);
    const wrapper = await setup();
    const error = new Error('personnel down');
    vi.mocked(getAgencyPersonnel).mockRejectedValue(error);
    await input(wrapper, 'agency').setValue('agency-2');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('selects the first agency once the agencies arrive', async () => {
    useAgencyStore().setAgencies([]);
    const wrapper = await setup();
    expect(getAgencyPersonnel).toHaveBeenCalledWith(undefined);

    useAgencyStore().setAgencies([{ id: 'agency-9', name: 'Late' }]);
    await flushPromises();

    expect(getAgencyPersonnel).toHaveBeenLastCalledWith('agency-9');
    expect(wrapper.exists()).toBe(true);
  });

  it('adds a new industry from the autocomplete footer', async () => {
    const mining: Industry = { id: 'i3', value: 'Mining' };
    vi.mocked(addIndustry).mockResolvedValue(mining);
    vi.mocked(createCompanyApi).mockResolvedValue({ id: 'new-5' });
    const wrapper = await setup();
    autocomplete(wrapper, 0).vm.$emit('update:modelValue', 'Min');
    autocomplete(wrapper, 0).vm.$emit('select-footer');
    await flushPromises();

    const config = prompt.mock.lastCall?.[0];
    expect(config?.inputAttrs).toMatchObject({ value: 'Min', maxlength: 100 });
    const dialog = { close: vi.fn() };
    await config?.onConfirm?.('Mining', dialog);
    await flushPromises();

    expect(addIndustry).toHaveBeenCalledExactlyOnceWith({ value: 'Mining' });
    expect(dialog.close).toHaveBeenCalledTimes(1);
    expect(autocomplete(wrapper, 0).props('modelValue')).toBe('Mining');

    await input(wrapper, 'full name').setValue('Globex');
    await input(wrapper, 'state').setValue(String(CompanyStatus.Lead));
    await chooseSalesRep(wrapper, rita);
    await input(wrapper, 'email').setValue('ops@globex.com');
    await submit(wrapper);
    expect(payload(createCompanyApi).industry).toEqual({ industry: mining });
  });

  it('shows the error when the catalogs cannot be loaded', async () => {
    const error = new Error('catalog down');
    vi.mocked(getIndustries).mockRejectedValue(error);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getAgencyPersonnel).not.toHaveBeenCalled();
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  describe('when updating', () => {
    const existing = createAgencyCompany({
      companyStatus: CompanyStatus.Client,
      salesRepresentativeId: 'p1',
      phoneExt: 7,
      faxExt: null,
      agencyId: 'agency-2',
      requiresPermissionToSeeRequests: true,
    });

    beforeEach(() => {
      route.path = '/recruiting/companies/update/company-profile-1';
      route.params = { companyProfileId: 'company-profile-1' };
      useAgencyStore().setAgencies([{ id: 'agency-1', name: 'Covenant' }, { id: 'agency-2', name: 'Sigook' }]);
      vi.mocked(getAgencyCompany).mockResolvedValue(existing);
    });

    it('fills the form from the company', async () => {
      const wrapper = await setup();

      expect(getAgencyCompany).toHaveBeenCalledWith('company-profile-1');
      expect(getAgencyPersonnel).toHaveBeenCalledWith('agency-2');
      expect(wrapper.find('.page-header-title').text()).toBe('Update Client');
      expect(value(wrapper, 'full name')).toBe('ACME LOGISTICS');
      expect(autocomplete(wrapper, 0).props('modelValue')).toBe('Logistics');
      expect(autocomplete(wrapper, 1).props('modelValue')).toBe('Rita Sales - rita@sigook.com');
      expect(value(wrapper, 'phoneExt')).toBe('7');
      expect(value(wrapper, 'faxExt')).toBe('');
      expect(value(wrapper, 'website')).toBe('www.acme.com');
      expect(input(wrapper, 'email').exists()).toBe(false);
      expect(input(wrapper, 'password').exists()).toBe(false);
      expect(input(wrapper, 'agency').exists()).toBe(false);
    });

    it('saves the changes without email or agency and opens the company', async () => {
      vi.mocked(updateAgencyCompany).mockResolvedValue();
      const wrapper = await setup();
      await input(wrapper, 'full name').setValue('ACME RENAMED');
      await submit(wrapper);

      const sent = payload(updateAgencyCompany);
      expect(updateAgencyCompany).toHaveBeenCalledWith('company-profile-1', sent);
      expect(sent).toMatchObject({
        companyProfileId: 'company-profile-1',
        fullName: 'ACME RENAMED',
        companyStatus: CompanyStatus.Client,
        phoneExt: 7,
        salesRepresentativeId: 'p1',
        industry: { id: 'ind-1', industry: logistics },
        logo: {},
      });
      expect(sent.email).toBe('ops@acme.com');
      expect(sent.password).toBeUndefined();
      expect(showAlertSuccess).toHaveBeenCalledWith('Company updated');
      expect(push).toHaveBeenCalledWith('/recruiting/companies/company-profile-1');
    });

    it('uploads a new logo after updating', async () => {
      vi.mocked(updateAgencyCompany).mockResolvedValue();
      vi.mocked(updateAgencyCompanyProfileLogo).mockResolvedValue();
      const wrapper = await setup();
      wrapper.findComponent({ name: 'PreviewImage' }).vm.$emit('imageSelected', logo);
      await submit(wrapper);

      expect(updateAgencyCompanyProfileLogo).toHaveBeenCalledExactlyOnceWith('company-profile-1', logo);
    });

    it('shows the server message when updating fails', async () => {
      vi.mocked(updateAgencyCompany).mockRejectedValue({ data: 'Conflict' });
      const wrapper = await setup();
      await submit(wrapper);

      expect(showAlertError).toHaveBeenCalledWith('Conflict');
      expect(push).not.toHaveBeenCalled();
    });

    it('leaves the sales rep empty when it is no longer in the agency', async () => {
      vi.mocked(getAgencyPersonnel).mockResolvedValue([sam]);
      const wrapper = await setup();

      expect(autocomplete(wrapper, 1).props('modelValue')).toBe('');
    });
  });
});
