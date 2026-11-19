import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { computed, nextTick, ref } from 'vue';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import ClientForm from '@/modules/agency/sales/dashboard/components/ClientForm.vue';
import { addIndustry, getCompanyStatus, getIndustries } from '@/shared/api/catalogApi';
import { getAgencyPersonnel } from '@/modules/agency/profile/api';
import { createAgencyCompany, updateAgencyCompanyProfileLogo } from '@/modules/agency/recruiting/clients/api';
import { useAgencyStore } from '@/modules/agency/store';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';

const admin = ref(false);

vi.mock('@/shared/composables/useAdmin', () => ({ useAdmin: () => ({ isAdmin: computed(() => admin.value) }) }));
vi.mock('@/shared/api/catalogApi', () => ({ addIndustry: vi.fn(), getCompanyStatus: vi.fn(), getIndustries: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({ getAgencyPersonnel: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({ createAgencyCompany: vi.fn(), updateAgencyCompanyProfileLogo: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => ({ getDialog: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/ui/PreviewImage.vue', () => ({
  default: { name: 'UploadImage', props: { required: Boolean }, emits: ['imageSelected'], template: '<div class="upload-image-stub" />' },
}));

const QuillStub = {
  name: 'QuillEditor',
  props: { content: String },
  emits: ['update:content'],
  template: '<div class="quill-stub" />',
};

interface PromptConfig {
  inputAttrs: { value: string };
  onConfirm: (value: string, dialog: { close: () => void }) => Promise<void>;
}

const logistics = { id: 'ind-1', value: 'Logistics' };
const rita: AgencyPersonnelListItem = { id: 'p-1', userId: 'user-1', name: 'Rita', email: 'rita@test', role: 'sales', agencyIds: [], agencies: [] };
const sam: AgencyPersonnelListItem = { id: 'p-2', userId: 'user-2', name: 'Sam', email: 'sam@test', role: 'sales', agencyIds: [], agencies: [] };

let pinia: Pinia;

async function setup() {
  const wrapper = mountWithPlugins(ClientForm, { global: { plugins: [pinia], stubs: { QuillEditor: QuillStub } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function submit(wrapper: Wrapper) {
  return (wrapper.vm as unknown as { submit: () => Promise<boolean> }).submit();
}

function autocomplete(wrapper: Wrapper, name: string) {
  const found = wrapper.findAllComponents({ name: 'BAutocomplete' }).find((a) => a.find(`input[name="${name}"]`).exists());
  if (!found) throw new Error(`${name} not found`);
  return found;
}

function statusSelect(wrapper: Wrapper) {
  const found = wrapper.findAllComponents({ name: 'BSelect' }).find((s) => s.find('select[name="state"]').exists());
  if (!found) throw new Error('status select not found');
  return found;
}

async function pick(wrapper: Wrapper, name: string, text: string, option: unknown) {
  const picker = autocomplete(wrapper, name);
  picker.vm.$emit('update:modelValue', text);
  await nextTick();
  picker.vm.$emit('select', option);
  await nextTick();
}

async function fill(wrapper: Wrapper, status = 1) {
  await wrapper.find('input[name="full name"]').setValue('Acme Logistics');
  await pick(wrapper, 'industry', 'Logistics', logistics);
  await pick(wrapper, 'salesRepresentative', 'Rita - rita@test', rita);
  statusSelect(wrapper).vm.$emit('update:modelValue', status);
  await wrapper.find('input[name="email"]').setValue('ap@acme.test');
  await nextTick();
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  admin.value = false;
  vi.mocked(getIndustries).mockResolvedValue([logistics, { id: 'ind-2', value: 'Retail' }]);
  vi.mocked(getCompanyStatus).mockResolvedValue([{ id: 1, value: 'Lead' }, { id: 5, value: 'Customer' }]);
  vi.mocked(getAgencyPersonnel).mockResolvedValue([rita, sam]);
});

describe('ClientForm', () => {
  it('loads catalogs and the sales representatives of the first agency', async () => {
    useAgencyStore().setAgencies([{ id: 'agency-1', name: 'North' }]);
    const wrapper = await setup();

    expect(getAgencyPersonnel).toHaveBeenCalledWith('agency-1');
    expect(statusSelect(wrapper).findAll('option').map((o) => o.text())).toEqual(['Select option', 'Lead', 'Customer']);
    expect(wrapper.find('select[name="agency"]').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'BCheckbox' }).exists()).toBe(false);
  });

  it('filters industries and representatives by the typed text', async () => {
    const wrapper = await setup();
    autocomplete(wrapper, 'industry').vm.$emit('update:modelValue', 'ret');
    autocomplete(wrapper, 'salesRepresentative').vm.$emit('update:modelValue', 'sam@');
    await nextTick();

    expect(autocomplete(wrapper, 'industry').props('data')).toEqual([{ id: 'ind-2', value: 'Retail' }]);
    expect(autocomplete(wrapper, 'salesRepresentative').props('data')).toEqual([sam]);
  });

  it('creates the client with its logo', async () => {
    vi.mocked(createAgencyCompany).mockResolvedValue({ id: 'company-9' });
    vi.mocked(updateAgencyCompanyProfileLogo).mockResolvedValue();
    useAgencyStore().setAgencies([{ id: 'agency-1', name: 'North' }]);
    const wrapper = await setup();
    await fill(wrapper);
    await wrapper.find('input[name="phoneExt"]').setValue('12');
    await wrapper.find('input[name="website"]').setValue('www.acme.test');
    await wrapper.find('textarea[name="about"]').setValue('Big warehouse');
    await wrapper.find('input[name="phone"]').setValue('4165550199');
    wrapper.findComponent(QuillStub).vm.$emit('update:content', '<p>VIP</p>');
    const logo = new File(['png'], 'logo.png');
    wrapper.findComponent({ name: 'UploadImage' }).vm.$emit('imageSelected', logo);
    await flushPromises();

    await expect(submit(wrapper)).resolves.toBe(true);
    expect(createAgencyCompany).toHaveBeenCalledExactlyOnceWith(expect.objectContaining({
      fullName: 'Acme Logistics',
      companyStatus: 1,
      about: 'Big warehouse',
      phone: '416 555-0199',
      phoneExt: 12,
      faxExt: null,
      website: 'www.acme.test',
      salesRepresentativeId: 'p-1',
      industry: { industry: logistics },
      email: 'ap@acme.test',
      agencyId: 'agency-1',
      internalInfo: '<p>VIP</p>',
      requiresPermissionToSeeRequests: false,
    }));
    expect(vi.mocked(createAgencyCompany).mock.calls[0][0].password).toBeUndefined();
    expect(updateAgencyCompanyProfileLogo).toHaveBeenCalledExactlyOnceWith('company-9', logo);
    expect(showAlertSuccess).toHaveBeenCalledWith('Company created');
  });

  it('asks for a password when the status is customer', async () => {
    vi.mocked(createAgencyCompany).mockResolvedValue({ id: 'company-9' });
    const wrapper = await setup();
    await fill(wrapper, 5);

    expect(wrapper.find('input[name="password"]').exists()).toBe(true);
    await expect(submit(wrapper)).resolves.toBe(false);
    await flushPromises();
    expect(wrapper.text()).toContain('Password is required');

    await wrapper.find('input[name="password"]').setValue('secret123');
    await expect(submit(wrapper)).resolves.toBe(true);
    expect(vi.mocked(createAgencyCompany).mock.calls[0][0].password).toBe('secret123');
    expect(updateAgencyCompanyProfileLogo).not.toHaveBeenCalled();
  });

  it('shows the required errors', async () => {
    const wrapper = await setup();

    await expect(submit(wrapper)).resolves.toBe(false);
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    await flushPromises();
    for (const message of ['Full name is required', 'Industry is required', 'Status is required', 'Sales representative is required', 'Email is required']) {
      expect(wrapper.text()).toContain(message);
    }
  });

  it('validates extensions and website', async () => {
    const wrapper = await setup();
    await fill(wrapper);
    await wrapper.find('input[name="faxExt"]').setValue('12a');
    await wrapper.find('input[name="website"]').setValue('not a url');

    await expect(submit(wrapper)).resolves.toBe(false);
    await flushPromises();
    expect(wrapper.text()).toContain('Must be 1-8 digits');
    expect(wrapper.text()).toContain('Invalid URL');
  });

  it('rejects typed industry and representative not picked from the list', async () => {
    const wrapper = await setup();
    await fill(wrapper);
    autocomplete(wrapper, 'industry').vm.$emit('update:modelValue', 'Mining');
    autocomplete(wrapper, 'salesRepresentative').vm.$emit('update:modelValue', 'Nobody');
    await flushPromises();

    await expect(submit(wrapper)).resolves.toBe(false);
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createAgencyCompany).not.toHaveBeenCalled();
  });

  it('shows the response data when creation fails', async () => {
    vi.mocked(createAgencyCompany).mockRejectedValue({ data: 'Email already used' });
    const wrapper = await setup();
    await fill(wrapper);

    await expect(submit(wrapper)).resolves.toBe(false);
    expect(showAlertError).toHaveBeenCalledWith('Email already used');
  });

  it('lets admins require permission to see requests', async () => {
    admin.value = true;
    vi.mocked(createAgencyCompany).mockResolvedValue({ id: 'company-9' });
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BCheckbox' }).vm.$emit('update:modelValue', true);
    await fill(wrapper);

    await expect(submit(wrapper)).resolves.toBe(true);
    expect(vi.mocked(createAgencyCompany).mock.calls[0][0].requiresPermissionToSeeRequests).toBe(true);
  });

  it('reloads representatives and clears the pick when the agency changes', async () => {
    useAgencyStore().setAgencies([{ id: 'agency-1', name: 'North' }, { id: 'agency-2', name: 'South' }]);
    const wrapper = await setup();
    await pick(wrapper, 'salesRepresentative', 'Rita - rita@test', rita);
    vi.mocked(getAgencyPersonnel).mockResolvedValue([sam]);
    const agencySelect = wrapper.findAllComponents({ name: 'BSelect' }).find((s) => s.find('select[name="agency"]').exists());
    agencySelect?.vm.$emit('update:modelValue', 'agency-2');
    await flushPromises();

    expect(getAgencyPersonnel).toHaveBeenLastCalledWith('agency-2');
    expect(autocomplete(wrapper, 'salesRepresentative').props('modelValue')).toBe('');
    expect(autocomplete(wrapper, 'salesRepresentative').props('data')).toEqual([sam]);
  });

  it('shows the error when the representatives of another agency fail to load', async () => {
    useAgencyStore().setAgencies([{ id: 'agency-1', name: 'North' }, { id: 'agency-2', name: 'South' }]);
    const wrapper = await setup();
    const error = new Error('personnel');
    vi.mocked(getAgencyPersonnel).mockRejectedValue(error);
    wrapper.findAllComponents({ name: 'BSelect' }).find((s) => s.find('select[name="agency"]').exists())?.vm.$emit('update:modelValue', 'agency-2');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('picks the first agency once the agencies arrive', async () => {
    const wrapper = await setup();
    useAgencyStore().setAgencies([{ id: 'agency-7', name: 'Late' }]);
    await flushPromises();

    expect(getAgencyPersonnel).toHaveBeenLastCalledWith('agency-7');
    expect(wrapper.find('select[name="agency"]').exists()).toBe(false);
  });

  it('adds a new industry from the footer prompt', async () => {
    const prompt = vi.fn<(config: PromptConfig) => void>();
    vi.mocked(getDialog).mockReturnValue({ prompt, confirm: vi.fn(), alert: vi.fn() } as unknown as ReturnType<typeof getDialog>);
    vi.mocked(addIndustry).mockResolvedValue({ id: 'ind-9', value: 'Mining' });
    vi.mocked(createAgencyCompany).mockResolvedValue({ id: 'company-9' });
    const wrapper = await setup();
    await fill(wrapper);
    autocomplete(wrapper, 'industry').vm.$emit('update:modelValue', 'Mining');
    await nextTick();
    autocomplete(wrapper, 'industry').vm.$emit('select-footer');

    const config = prompt.mock.calls[0][0];
    expect(config.inputAttrs.value).toBe('Mining');
    const close = vi.fn();
    await config.onConfirm('Mining', { close });
    await nextTick();

    expect(addIndustry).toHaveBeenCalledWith({ value: 'Mining' });
    expect(close).toHaveBeenCalledOnce();
    await expect(submit(wrapper)).resolves.toBe(true);
    expect(vi.mocked(createAgencyCompany).mock.calls[0][0].industry).toEqual({ industry: { id: 'ind-9', value: 'Mining' } });
  });

  it('keeps the industry prompt open and shows the error when adding fails', async () => {
    const prompt = vi.fn<(config: PromptConfig) => void>();
    vi.mocked(getDialog).mockReturnValue({ prompt, confirm: vi.fn(), alert: vi.fn() } as unknown as ReturnType<typeof getDialog>);
    const error = new Error('duplicate');
    vi.mocked(addIndustry).mockRejectedValue(error);
    const wrapper = await setup();
    autocomplete(wrapper, 'industry').vm.$emit('select-footer');

    const close = vi.fn();
    await prompt.mock.calls[0][0].onConfirm('Mining', { close });

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(close).not.toHaveBeenCalled();
  });

  it('shows the error when the catalogs fail to load', async () => {
    const error = new Error('offline');
    vi.mocked(getIndustries).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
