import { describe, expect, it, vi } from 'vitest';
import CandidateApplyForm from '@/modules/landing/components/shared/forms/CandidateApplyForm.vue';
import { getSkills, getSources } from '@/shared/api/catalogApi';
import { getCities, getCountries, getProvinces } from '@/shared/api/locationApi';
import { submitCandidateApplication, submitRegisteredApplication } from '@/modules/landing/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { City, Country, Province, Source } from '@/shared/types/common';

vi.mock('@/shared/api/catalogApi', () => ({ getSources: vi.fn(), getSkills: vi.fn() }));
vi.mock('@/shared/api/locationApi', () => ({ getCountries: vi.fn(), getProvinces: vi.fn(), getCities: vi.fn() }));
vi.mock('@/modules/landing/api', () => ({ submitCandidateApplication: vi.fn(), submitRegisteredApplication: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const canada: Country = { id: 'country-ca', value: 'Canada', code: 'CA' };
const ontario: Province = { id: 'province-on', value: 'Ontario', code: 'ON' };
const toronto: City = { id: 'city-tor', value: 'Toronto' };
const sigookSource: Source = { id: 'source-sigook', value: 'Sigook' };
const linkedinSource: Source = { id: 'source-linkedin', value: 'Linkedin' };

async function settle() {
  for (let i = 0; i < 10; i++) await new Promise((resolve) => { setTimeout(resolve, 0); });
}

function mockCatalogs() {
  vi.mocked(getCountries).mockResolvedValue([canada]);
  vi.mocked(getSources).mockResolvedValue([sigookSource, linkedinSource]);
  vi.mocked(getSkills).mockResolvedValue([{ skill: 'Forklift' }, { skill: 'Welding' }]);
  vi.mocked(getProvinces).mockResolvedValue([ontario]);
  vi.mocked(getCities).mockResolvedValue([toronto, { id: 'city-ott', value: 'Ottawa' }]);
}

async function setup(props: { jobTitle?: string; requestId?: string } = {}) {
  mockCatalogs();
  const wrapper = mountWithPlugins(CandidateApplyForm, { props });
  await settle();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function button(wrapper: Wrapper, text: string, index = 0) {
  const matches = wrapper.findAll('button').filter((b) => b.text() === text && b.isVisible());
  const found = matches.at(index);
  if (!found) throw new Error(`Button ${text} not found`);
  return found;
}

async function selectOption(wrapper: Wrapper, name: string, label: string) {
  const option = wrapper.findAll(`select[name="${name}"] option`).find((o) => o.text() === label);
  if (!option) throw new Error(`Option ${label} not found in ${name}`);
  (option.element as HTMLOptionElement).selected = true;
  await wrapper.find(`select[name="${name}"]`).trigger('change');
  await settle();
}

async function fillPersonal(wrapper: Wrapper) {
  await wrapper.find('input[name="fullName"]').setValue('Jane Doe');
  await wrapper.find('input[name="email"]').setValue('jane@doe.com');
  await wrapper.find('input[type="tel"]').setValue('4165551234');
  await selectOption(wrapper, 'country', 'Canada');
  await selectOption(wrapper, 'province', 'Ontario');
  wrapper.findComponent({ name: 'BAutocomplete' }).vm.$emit('select', toronto);
  await wrapper.find('input[name="address"]').setValue('1 Main St');
  await settle();
}

async function goNext(wrapper: Wrapper) {
  await button(wrapper, 'Next').trigger('click');
  await settle();
}

async function reachReview(wrapper: Wrapper) {
  await fillPersonal(wrapper);
  await goNext(wrapper);
  await selectOption(wrapper, 'status', 'Work Permit');
  await goNext(wrapper);
}

function summary(wrapper: Wrapper) {
  return Object.fromEntries(wrapper.findAll('.reg-form__summary-row').map((row) => [row.find('dt').text(), row.find('dd').text()]));
}

describe('CandidateApplyForm', () => {
  it('shows a loader until the catalogs load', () => {
    mockCatalogs();
    const wrapper = mountWithPlugins(CandidateApplyForm);

    expect(wrapper.find('.reg-form__loader').text()).toBe('Loading…');
  });

  it('loads the catalogs and preselects the Sigook source', async () => {
    const wrapper = await setup();

    expect(wrapper.find('.reg-form__loader').exists()).toBe(false);
    expect(wrapper.findAll('select[name="country"] option').map((o) => o.text())).toContain('Canada');
    expect((wrapper.find('select[name="source"]').element as HTMLSelectElement).selectedOptions[0].text).toBe('Sigook');
  });

  it('still renders the form when the catalogs fail to load', async () => {
    const warn = vi.spyOn(console, 'warn').mockImplementation(() => undefined);
    vi.mocked(getCountries).mockRejectedValue(new Error('down'));
    vi.mocked(getSources).mockResolvedValue([]);
    vi.mocked(getSkills).mockResolvedValue([]);
    const wrapper = mountWithPlugins(CandidateApplyForm);
    await settle();

    expect(wrapper.find('input[name="fullName"]').exists()).toBe(true);
    expect(warn).toHaveBeenCalled();
  });

  it('shows the job context without tabs when there is no request', async () => {
    const wrapper = await setup({ jobTitle: 'Forklift Operator' });

    expect(wrapper.find('.reg-form__context-value').text()).toBe('Forklift Operator');
    expect(wrapper.find('[role="tablist"]').exists()).toBe(false);
  });

  it('blocks the first step and shows required errors', async () => {
    const wrapper = await setup();
    await goNext(wrapper);

    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    const messages = wrapper.findAll('.help.is-danger').map((h) => h.text());
    expect(messages).toEqual(expect.arrayContaining([
      'Full name is required',
      'Email is required',
      'Country is required',
      'Province/State is required',
      'City is required',
      'Address is required',
    ]));
    expect(wrapper.find('select[name="status"]').isVisible()).toBe(false);
  });

  it('rejects an invalid email', async () => {
    const wrapper = await setup();
    await wrapper.find('input[name="email"]').setValue('not-an-email');
    await goNext(wrapper);

    expect(wrapper.findAll('.help.is-danger').map((h) => h.text())).toContain('Invalid email format');
  });

  it('loads provinces and cities as the location is chosen', async () => {
    const wrapper = await setup();
    await selectOption(wrapper, 'country', 'Canada');

    expect(getProvinces).toHaveBeenCalledWith('country-ca');
    await selectOption(wrapper, 'province', 'Ontario');
    expect(getCities).toHaveBeenCalledWith('province-on');
    expect(wrapper.findComponent({ name: 'BAutocomplete' }).props('data')).toEqual([toronto, { id: 'city-ott', value: 'Ottawa' }]);
  });

  it('filters cities by the typed query', async () => {
    const wrapper = await setup();
    await selectOption(wrapper, 'country', 'Canada');
    await selectOption(wrapper, 'province', 'Ontario');
    const autocomplete = wrapper.findComponent({ name: 'BAutocomplete' });
    autocomplete.vm.$emit('update:modelValue', 'ott');
    await settle();

    expect(autocomplete.props('data')).toEqual([{ id: 'city-ott', value: 'Ottawa' }]);
  });

  it('requires the immigration status on the second step', async () => {
    const wrapper = await setup();
    await fillPersonal(wrapper);
    await goNext(wrapper);
    vi.mocked(showAlertError).mockClear();

    expect(wrapper.find('select[name="status"]').isVisible()).toBe(true);
    await goNext(wrapper);
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(wrapper.findAll('.help.is-danger').map((h) => h.text())).toContain('Immigration status is required');
  });

  it('requires a resume when the source is LinkedIn', async () => {
    const wrapper = await setup();
    await fillPersonal(wrapper);
    await goNext(wrapper);
    await selectOption(wrapper, 'status', 'Citizen');
    await selectOption(wrapper, 'source', 'Linkedin');
    await goNext(wrapper);

    expect(wrapper.find('.help.is-danger:not(.field .help)').text()).toBe('Resume is required when you apply via LinkedIn');
    expect(wrapper.find('.reg-form__summary').isVisible()).toBe(false);
  });

  it('rejects resumes above 15MB', async () => {
    const wrapper = await setup();
    const big = new File(['x'], 'big.pdf');
    Object.defineProperty(big, 'size', { value: 16 * 1024 * 1024 });
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', big);
    await settle();

    expect(showAlertError).toHaveBeenCalledWith('File exceeds 15MB limit');
    expect(wrapper.find('.document-filename').exists()).toBe(false);
  });

  it('attaches and removes a resume', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', new File(['cv'], 'cv.pdf'));
    await settle();

    expect(wrapper.find('.document-filename').text()).toBe('cv.pdf');
    await wrapper.find('.document-delete-container button').trigger('click');
    expect(wrapper.find('.document-filename').exists()).toBe(false);
  });

  it('goes back to the previous step', async () => {
    const wrapper = await setup();
    await fillPersonal(wrapper);
    await goNext(wrapper);
    await button(wrapper, 'Previous').trigger('click');
    await settle();

    expect(wrapper.find('input[name="fullName"]').isVisible()).toBe(true);
  });

  it('summarizes the application on the review step', async () => {
    const wrapper = await setup();
    await reachReview(wrapper);

    expect(summary(wrapper)).toEqual({
      Name: 'Jane Doe',
      Email: 'jane@doe.com',
      Phone: '416 555-1234',
      Country: 'Canada',
      Address: '1 Main St, Toronto, ON',
      'Immigration Status': 'Work Permit',
      'How did you hear about us': 'Sigook',
      Transportation: 'Public Transit',
    });
  });

  it('requires accepting the terms before submitting', async () => {
    const wrapper = await setup();
    await reachReview(wrapper);
    await wrapper.find('form').trigger('submit');
    await settle();

    expect(showAlertError).toHaveBeenCalledWith('You must accept the terms and conditions');
    expect(wrapper.findAll('.help.is-danger').map((h) => h.text())).toContain('You must accept the terms and conditions');
    expect(submitCandidateApplication).not.toHaveBeenCalled();
  });

  it('submits the full payload with the request id', async () => {
    vi.mocked(submitCandidateApplication).mockResolvedValue();
    const wrapper = await setup({ requestId: 'request-1' });
    await reachReview(wrapper);
    await wrapper.find('input[name="agree terms"]').setValue(true);
    await wrapper.find('form').trigger('submit');
    await settle();

    expect(submitCandidateApplication).toHaveBeenCalledExactlyOnceWith({
      fullName: 'Jane Doe',
      email: 'jane@doe.com',
      phone: '416 555-1234',
      cityId: 'city-tor',
      address: '1 Main St',
      status: 'Work Permit',
      sourceId: 'source-sigook',
      skills: [],
      hasVehicle: false,
      resume: null,
      termsAccepted: true,
    }, 'request-1');
    expect(showAlertSuccess).toHaveBeenCalledWith('Your application has been submitted');
    expect(wrapper.emitted('submitted')).toHaveLength(1);
  });

  it('sends skills, vehicle and resume', async () => {
    vi.mocked(submitCandidateApplication).mockResolvedValue();
    const wrapper = await setup();
    await fillPersonal(wrapper);
    await goNext(wrapper);
    await selectOption(wrapper, 'status', 'Citizen');
    const resume = new File(['cv'], 'cv.pdf');
    wrapper.findComponent({ name: 'BUpload' }).vm.$emit('update:modelValue', resume);
    const taginput = wrapper.findComponent({ name: 'BTaginput' });
    taginput.vm.$emit('typing', 'weld');
    await settle();
    expect(taginput.props('data')).toEqual([{ skill: 'Welding' }]);
    const createTag = taginput.props('createTag') as (raw: string) => { skill: string };
    expect(createTag('A very long custom skill name')).toEqual({ skill: 'A very long custom s' });
    taginput.vm.$emit('update:modelValue', [{ skill: 'Welding' }, { skill: 'Driving' }]);
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', true);
    await settle();
    await goNext(wrapper);

    expect(summary(wrapper)).toMatchObject({ Transportation: 'Own Vehicle', Resume: 'cv.pdf' });
    expect(wrapper.findAll('.reg-form__summary-chip').map((c) => c.text())).toEqual(['Welding', 'Driving']);
    await wrapper.find('input[name="agree terms"]').setValue(true);
    await wrapper.find('form').trigger('submit');
    await settle();

    expect(submitCandidateApplication).toHaveBeenCalledWith(
      expect.objectContaining({ skills: ['Welding', 'Driving'], hasVehicle: true, resume, status: 'Citizen' }),
      undefined,
    );
  });

  it('shows the server error when submitting fails', async () => {
    vi.mocked(submitCandidateApplication).mockRejectedValue({ data: 'Email already registered' });
    const wrapper = await setup();
    await reachReview(wrapper);
    await wrapper.find('input[name="agree terms"]').setValue(true);
    await wrapper.find('form').trigger('submit');
    await settle();

    expect(showAlertError).toHaveBeenCalledWith('Email already registered');
    expect(wrapper.emitted('submitted')).toBeUndefined();
    expect(button(wrapper, 'Submit Application').attributes('disabled')).toBeUndefined();
  });

  it('falls back to a generic error message', async () => {
    vi.mocked(submitCandidateApplication).mockRejectedValue(null);
    const wrapper = await setup();
    await reachReview(wrapper);
    await wrapper.find('input[name="agree terms"]').setValue(true);
    await wrapper.find('form').trigger('submit');
    await settle();

    expect(showAlertError).toHaveBeenCalledWith('Something went wrong');
  });

  describe('already registered tab', () => {
    async function openRegistered() {
      const wrapper = await setup({ requestId: 'request-7', jobTitle: 'Driver' });
      const tab = wrapper.findAll('[role="tab"]').find((t) => t.text() === 'Already Registered');
      await tab?.trigger('click');
      return wrapper;
    }

    it('switches between tabs', async () => {
      const wrapper = await openRegistered();

      expect(wrapper.find('input[name="registeredEmail"]').exists()).toBe(true);
      expect(wrapper.find('input[name="fullName"]').exists()).toBe(false);
      await wrapper.findAll('[role="tab"]')[0].trigger('click');
      expect(wrapper.find('input[name="fullName"]').exists()).toBe(true);
    });

    it('validates the email and clears the error while typing', async () => {
      const wrapper = await openRegistered();
      await wrapper.find('input[name="registeredEmail"]').setValue('bad');
      await wrapper.find('form').trigger('submit');
      await settle();

      expect(wrapper.find('.help.is-danger').text()).toBe('Enter a valid email address');
      expect(submitRegisteredApplication).not.toHaveBeenCalled();
      await wrapper.find('input[name="registeredEmail"]').setValue('bad2');
      expect(wrapper.find('.help.is-danger').exists()).toBe(false);
    });

    it('submits the trimmed email with the request', async () => {
      vi.mocked(submitRegisteredApplication).mockResolvedValue();
      const wrapper = await openRegistered();
      await wrapper.find('input[name="registeredEmail"]').setValue('  jane@doe.com ');
      await wrapper.find('form').trigger('submit');
      await settle();

      expect(submitRegisteredApplication).toHaveBeenCalledExactlyOnceWith('jane@doe.com', 'request-7');
      expect(showAlertSuccess).toHaveBeenCalledWith('Your application has been submitted');
      expect(wrapper.emitted('submitted')).toHaveLength(1);
    });

    it('shows the error when the registered submit fails', async () => {
      vi.mocked(submitRegisteredApplication).mockRejectedValue({ data: 'Not found' });
      const wrapper = await openRegistered();
      await wrapper.find('input[name="registeredEmail"]').setValue('jane@doe.com');
      await wrapper.find('form').trigger('submit');
      await settle();

      expect(showAlertError).toHaveBeenCalledWith('Not found');
      expect(wrapper.emitted('submitted')).toBeUndefined();
    });

    it('falls back to a generic error on registered submit', async () => {
      vi.mocked(submitRegisteredApplication).mockRejectedValue(undefined);
      const wrapper = await openRegistered();
      await wrapper.find('input[name="registeredEmail"]').setValue('jane@doe.com');
      await wrapper.find('form').trigger('submit');
      await settle();

      expect(showAlertError).toHaveBeenCalledWith('Something went wrong');
    });
  });
});
