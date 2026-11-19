import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import Register from '@/modules/worker/register/pages/Register.vue';
import { registerWorker } from '@/modules/worker/register/api';
import {
  fetchLanguages,
  fetchLifts,
  getAvailability,
  getAvailabilityTimes,
  getDays,
  getGenders,
  getIdentificationTypes,
  getSkills,
} from '@/shared/api/catalogApi';
import { compressFile } from '@/shared/utils/compressFile';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { formDataPayload, oversizedFile } from '@/testing/workerProfileFixtures';
import { IdentificationTypeCode, type IdentificationType } from '@/shared/types/common';

const { push, route, security, validateAddress, validatePhone } = vi.hoisted(() => ({
  push: vi.fn(),
  route: { params: {} as Record<string, string> },
  security: { user: null as { id: string } | null },
  validateAddress: vi.fn<() => Promise<boolean>>(),
  validatePhone: vi.fn<() => Promise<boolean>>(),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
  useRouter: () => ({ push }),
}));
vi.mock('@/app/stores/app', () => ({ useAppStore: () => ({ getCurrentDate: () => Promise.resolve(new Date('2026-06-03T12:00:00')) }) }));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => security }));
vi.mock('@/modules/worker/register/api', () => ({ registerWorker: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({
  getGenders: vi.fn(),
  getIdentificationTypes: vi.fn(),
  getAvailability: vi.fn(),
  getAvailabilityTimes: vi.fn(),
  getDays: vi.fn(),
  fetchLifts: vi.fn(),
  fetchLanguages: vi.fn(),
  getSkills: vi.fn(),
}));
vi.mock('@/shared/utils/compressFile', () => ({ compressFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/ui/Address.vue', () => ({
  default: {
    name: 'CvnAddress',
    props: { model: Object },
    emits: ['update:model', 'isCanada', 'isLoading'],
    methods: { validateAddress: () => validateAddress() },
    template: '<div class="address-stub" />',
  },
}));
vi.mock('@/shared/ui/PhoneInput.vue', () => ({
  default: {
    name: 'PhoneInput',
    props: { required: Boolean, label: String, defaultValue: String },
    emits: ['formattedPhone'],
    methods: { validatePhone: () => validatePhone() },
    template: '<div class="phone-stub" />',
  },
}));
vi.mock('@/shared/ui/PreviewImage.vue', () => ({
  default: { name: 'UploadImage', props: { editedImage: Object }, emits: ['imageSelected'], template: '<div class="image-stub" />' },
}));

const sin: IdentificationType = { id: 'type-sin', value: 'SIN / SSN', code: IdentificationTypeCode.SinSsn };
const passport: IdentificationType = { id: 'type-passport', value: 'Passport', code: 1 };
const days = [{ id: 'd1', value: 'Monday' }, { id: 'd2', value: 'Tuesday' }];
const requiredMessage = 'Please make sure all required fields are filled out correctly';
const location = { address: '1 King St', cityId: 'c1' };

async function settle() {
  await flushPromises();
  await new Promise((resolve) => { setTimeout(resolve, 10); });
  await flushPromises();
}

function pdf(name: string) {
  return new File(['x'], name, { type: 'application/pdf' });
}

async function setup() {
  const wrapper = mountWithPlugins(Register);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function step(wrapper: Wrapper, title: string) {
  const found = wrapper.findAll('.step-item').find((s) => s.find('h1').exists() && s.find('h1').text() === title);
  if (!found) throw new Error(`${title} step not found`);
  return found;
}

async function click(wrapper: Wrapper, title: string, label: string) {
  const button = step(wrapper, title).findAll('button').find((b) => b.text() === label);
  if (!button) throw new Error(`${label} not found in ${title}`);
  await button.trigger('click');
  await settle();
}

function activeStep(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BSteps' }).props('modelValue');
}

function messages(wrapper: Wrapper, title: string) {
  return step(wrapper, title).findAll('.help.is-danger').filter((e) => e.isVisible() && e.text()).map((e) => e.text());
}

function select(wrapper: Wrapper, name: string) {
  const found = wrapper.findAllComponents({ name: 'BSelect' }).find((s) => s.find(`select[name="${name}"]`).exists());
  if (!found) throw new Error(`${name} select not found`);
  return found;
}

function upload(wrapper: Wrapper, index: number, file: File) {
  wrapper.findAllComponents({ name: 'BUpload' })[index].vm.$emit('update:modelValue', file);
  return settle();
}

async function fillBasic(wrapper: Wrapper) {
  await wrapper.find('input[name="name"]').setValue('Jane');
  await wrapper.find('input[name="lastname"]').setValue('Doe');
  wrapper.findAllComponents({ name: 'BDatepicker' })[0].vm.$emit('update:modelValue', new Date('1995-03-10T12:00:00'));
  await wrapper.find('select[name="gender"]').setValue('g1');
  wrapper.findComponent({ name: 'CvnAddress' }).vm.$emit('update:model', location);
  wrapper.findComponent({ name: 'PhoneInput' }).vm.$emit('formattedPhone', '+14165550100');
  await flushPromises();
}

async function goToDocuments(wrapper: Wrapper) {
  await fillBasic(wrapper);
  await click(wrapper, 'Basic Information', 'Next');
  if (!security.user) await click(wrapper, 'Preferences', 'Next');
}

async function chooseId(wrapper: Wrapper, index: 1 | 2, type: IdentificationType, number: string) {
  const native = wrapper.find(`select[name="identificationType${index}"]`);
  const element = native.element as HTMLSelectElement;
  element.selectedIndex = Array.from(element.options).findIndex((o) => o.text.trim() === type.value);
  await native.trigger('change');
  await wrapper.find(`input[name="identificationNumber${index}"]`).setValue(number);
}

async function goToAccount(wrapper: Wrapper) {
  await goToDocuments(wrapper);
  await upload(wrapper, 0, pdf('passport.pdf'));
  await chooseId(wrapper, 1, passport, 'P1234');
  await click(wrapper, 'Documents', 'Next');
}

async function fillAccount(wrapper: Wrapper, password = 'Secret123') {
  await wrapper.find('input[name="email"]').setValue('jane@x.com');
  await wrapper.find('input[name="password"]').setValue(password);
  await wrapper.find('input[name="confirmPassword"]').setValue(password);
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
  await settle();
}

function sentPayload() {
  const formData = vi.mocked(registerWorker).mock.lastCall?.[0];
  if (!formData) throw new Error('registerWorker was not called');
  return { formData, data: formDataPayload(formData) as Record<string, unknown> };
}

beforeEach(() => {
  route.params = {};
  security.user = null;
  validateAddress.mockResolvedValue(true);
  validatePhone.mockResolvedValue(true);
  vi.mocked(compressFile).mockImplementation((file) => Promise.resolve(file));
  vi.mocked(getGenders).mockResolvedValue([{ id: 'g1', value: 'Female' }]);
  vi.mocked(getIdentificationTypes).mockResolvedValue([sin, passport]);
  vi.mocked(getAvailability).mockResolvedValue([{ id: 'a1', value: 'Full time' }]);
  vi.mocked(getAvailabilityTimes).mockResolvedValue([{ id: 't1', value: 'Morning' }]);
  vi.mocked(getDays).mockResolvedValue(days);
  vi.mocked(fetchLifts).mockResolvedValue([{ id: 'l1', value: '50 lbs' }]);
  vi.mocked(fetchLanguages).mockResolvedValue([{ id: 'en', value: 'English' }]);
  vi.mocked(getSkills).mockResolvedValue([{ skill: 'Forklift' }]);
  vi.mocked(registerWorker).mockResolvedValue('new-worker');
});

afterEach(() => {
  vi.useRealTimers();
});

describe('Register basic step', () => {
  it('loads the catalogs and limits the birth date to adults', async () => {
    const wrapper = await setup();
    expect(wrapper.findAll('select[name="gender"] option').map((o) => o.text())).toEqual(['Female']);
    expect(wrapper.findAllComponents({ name: 'BDatepicker' })[0].props('maxDate')).toEqual(new Date('2008-06-03T12:00:00'));
    expect(activeStep(wrapper)).toBe(0);
  });

  it('reports a failed catalog load and hides the overlay', async () => {
    vi.mocked(getGenders).mockRejectedValue({ data: 'Catalogs unavailable' });
    const wrapper = await setup();
    await settle();
    expect(showAlertError).toHaveBeenCalledWith('Catalogs unavailable');
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('requires the basic information', async () => {
    const wrapper = await setup();
    await click(wrapper, 'Basic Information', 'Next');
    expect(messages(wrapper, 'Basic Information')).toEqual(['Name is required', 'Last name is required', 'Date of birth is required', 'Gender is required']);
    expect(showAlertError).toHaveBeenCalledWith(requiredMessage);
    expect(activeStep(wrapper)).toBe(0);
  });

  it('limits the name length', async () => {
    const wrapper = await setup();
    await fillBasic(wrapper);
    await wrapper.find('input[name="name"]').setValue('J');
    await wrapper.find('input[name="lastname"]').setValue('D'.repeat(21));
    await click(wrapper, 'Basic Information', 'Next');
    expect(messages(wrapper, 'Basic Information')).toEqual(['Min 2 characters', 'Max 20 characters']);
    expect(activeStep(wrapper)).toBe(0);
  });

  it('stays when the address or phone is invalid', async () => {
    const wrapper = await setup();
    await fillBasic(wrapper);
    validateAddress.mockResolvedValue(false);
    await click(wrapper, 'Basic Information', 'Next');
    expect(activeStep(wrapper)).toBe(0);

    validateAddress.mockResolvedValue(true);
    validatePhone.mockResolvedValue(false);
    await click(wrapper, 'Basic Information', 'Next');
    expect(activeStep(wrapper)).toBe(0);
    expect(showAlertError).toHaveBeenCalledTimes(2);
  });

  it('moves through preferences and back', async () => {
    const wrapper = await setup();
    await fillBasic(wrapper);
    await click(wrapper, 'Basic Information', 'Next');
    expect(activeStep(wrapper)).toBe(1);
    await click(wrapper, 'Preferences', 'Next');
    expect(activeStep(wrapper)).toBe(2);
    await click(wrapper, 'Documents', 'Previous');
    expect(activeStep(wrapper)).toBe(1);
    await click(wrapper, 'Preferences', 'Previous');
    expect(activeStep(wrapper)).toBe(0);
  });
});

describe('Register documents step', () => {
  it('requires one identification document', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await click(wrapper, 'Documents', 'Next');
    expect(messages(wrapper, 'Documents')).toEqual(['At least one identification document is required']);
    expect(activeStep(wrapper)).toBe(2);
  });

  it('requires the identification type and number', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, pdf('passport.pdf'));
    expect(wrapper.find('.document-filename').text()).toBe('PASSPORT.PDF');
    await click(wrapper, 'Documents', 'Next');
    expect(messages(wrapper, 'Documents')).toEqual(['Identification type is required']);

    select(wrapper, 'identificationType1').vm.$emit('update:modelValue', passport);
    await click(wrapper, 'Documents', 'Next');
    expect(messages(wrapper, 'Documents')).toEqual(['Identification number is required']);
    expect(activeStep(wrapper)).toBe(2);
  });

  it('checks the SIN length', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, pdf('sin.pdf'));
    await chooseId(wrapper, 1, sin, '1234');
    await click(wrapper, 'Documents', 'Next');
    expect(messages(wrapper, 'Documents')).toEqual(['SIN / SSN must be between 9 and 15 characters']);
  });

  it('validates the second identification and blocks a third upload', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, pdf('passport.pdf'));
    await upload(wrapper, 0, pdf('licence.pdf'));
    expect(wrapper.findAllComponents({ name: 'BUpload' })[0].props('disabled')).toBeTruthy();
    await chooseId(wrapper, 1, passport, 'P1234');
    await click(wrapper, 'Documents', 'Next');
    expect(messages(wrapper, 'Documents')).toEqual(['Identification type is required']);
  });

  it('enables the upload as a boolean until both identifications are uploaded', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, pdf('passport.pdf'));
    expect(wrapper.findAllComponents({ name: 'BUpload' })[0].props('disabled')).toBe(false);
    await upload(wrapper, 0, pdf('licence.pdf'));
    expect(wrapper.findAllComponents({ name: 'BUpload' })[0].props('disabled')).toBe(true);
  });

  it('disables the identification type already chosen in the other document', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, pdf('passport.pdf'));
    await upload(wrapper, 0, pdf('licence.pdf'));
    await chooseId(wrapper, 1, passport, 'P1234');
    const options = wrapper.findAll('select[name="identificationType2"] option')
      .map((o) => ({ text: o.text(), disabled: (o.element as HTMLOptionElement).disabled }));
    expect(options).toEqual(expect.arrayContaining([
      { text: 'Passport', disabled: true },
      { text: 'SIN / SSN', disabled: false },
    ]));
  });

  it('moves the second identification up when the first is deleted', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, pdf('passport.pdf'));
    await upload(wrapper, 0, pdf('licence.pdf'));
    await chooseId(wrapper, 2, sin, '123456789');
    await wrapper.findAll('.document-card')[0].find('button').trigger('click');
    await flushPromises();

    expect(wrapper.findAll('.document-filename').map((f) => f.text())).toEqual(['LICENCE.PDF']);
    expect(select(wrapper, 'identificationType1').props('modelValue')).toEqual(sin);
    expect((wrapper.find('input[name="identificationNumber1"]').element as HTMLInputElement).value).toBe('123456789');
  });

  it('deletes the second or the only identification', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, pdf('passport.pdf'));
    await upload(wrapper, 0, pdf('licence.pdf'));
    await wrapper.findAll('.document-card')[1].find('button').trigger('click');
    await flushPromises();
    expect(wrapper.findAll('.document-filename').map((f) => f.text())).toEqual(['PASSPORT.PDF']);
    await wrapper.findAll('.document-card')[0].find('button').trigger('click');
    await flushPromises();
    expect(wrapper.findAll('.document-filename')).toHaveLength(0);
  });

  it('rejects files over 15MB', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, oversizedFile('big.pdf'));
    expect(showAlertError).toHaveBeenCalledWith('File exceeds 15MB limit');
    expect(wrapper.findAll('.document-card')).toHaveLength(0);
  });

  it('validates licenses and certificates', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, pdf('passport.pdf'));
    await chooseId(wrapper, 1, passport, 'P1234');
    await upload(wrapper, 1, pdf('forklift.pdf'));
    await upload(wrapper, 1, pdf('wheel.pdf'));
    await upload(wrapper, 2, pdf('whmis.pdf'));
    await upload(wrapper, 2, pdf('first-aid.pdf'));
    await upload(wrapper, 2, pdf('cpr.pdf'));
    await wrapper.find('input[name="description1"]').setValue('Bad!');
    await wrapper.find('input[name="descriptioncer1"]').setValue('x'.repeat(101));
    await wrapper.find('input[name="descriptioncer2"]').setValue('No$');
    await click(wrapper, 'Documents', 'Next');

    expect(messages(wrapper, 'Documents')).toEqual([
      'Description is required',
      'Expiration date is required',
      'Only letters, numbers, spaces and -_',
      'Expiration date is required',
      'Description is required',
      'Max 100 characters',
      'Only letters, numbers, spaces and -_',
    ]);
    expect(activeStep(wrapper)).toBe(2);
  });

  it('accepts a license that does not expire and removes uploaded items', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    await upload(wrapper, 0, pdf('passport.pdf'));
    await chooseId(wrapper, 1, passport, 'P1234');
    await upload(wrapper, 1, pdf('forklift.pdf'));
    await wrapper.find('input[name="description0"]').setValue('Forklift');
    await wrapper.findAll('.document-card')[1].find('input[type="checkbox"]').setValue(false);
    await upload(wrapper, 2, pdf('whmis.pdf'));
    await upload(wrapper, 3, pdf('resume.pdf'));
    expect(wrapper.findAllComponents({ name: 'BUpload' })[3].props('disabled')).toBe(true);

    const cards = () => wrapper.findAll('.document-card');
    await cards()[2].find('button').trigger('click');
    await cards()[2].find('button').trigger('click');
    expect(wrapper.findAll('.document-filename').map((f) => f.text())).toEqual(['PASSPORT.PDF', 'FORKLIFT.PDF']);

    await click(wrapper, 'Documents', 'Next');
    expect(activeStep(wrapper)).toBe(3);
  });

  it('asks Canadian workers for WHMIS and drops it outside Canada', async () => {
    const wrapper = await setup();
    await goToDocuments(wrapper);
    expect(wrapper.text()).not.toContain('WHMIS and Health and Safety Training');
    wrapper.findComponent({ name: 'CvnAddress' }).vm.$emit('isCanada', true);
    await nextTick();
    expect(wrapper.text()).toContain('WHMIS and Health and Safety Training');

    await upload(wrapper, 4, pdf('whmis.pdf'));
    await upload(wrapper, 4, pdf('booklet.pdf'));
    await wrapper.find('input[name="descriptionOther1"]').setValue('y'.repeat(101));
    await wrapper.find('input[name="descriptionOther0"]').setValue('No!');
    await upload(wrapper, 0, pdf('passport.pdf'));
    await chooseId(wrapper, 1, passport, 'P1234');
    await click(wrapper, 'Documents', 'Next');
    expect(messages(wrapper, 'Documents')).toEqual(['Only letters, numbers, spaces and -_', 'Max 100 characters']);

    await wrapper.findAll('.document-card')[1].find('button').trigger('click');
    expect(wrapper.findAll('.document-card')).toHaveLength(2);
    wrapper.findComponent({ name: 'CvnAddress' }).vm.$emit('isCanada', false);
    await nextTick();
    expect(wrapper.findAll('.document-card')).toHaveLength(1);
  });
});

describe('Register account step', () => {
  it('validates the account fields and terms', async () => {
    const wrapper = await setup();
    await goToAccount(wrapper);
    expect(activeStep(wrapper)).toBe(3);
    await submit(wrapper);
    expect(messages(wrapper, 'Account')).toEqual([
      'Email is required',
      'Password is required',
      'Confirm password is required',
      'You must accept the Terms & Conditions to continue',
    ]);
    expect(registerWorker).not.toHaveBeenCalled();
  });

  it('requires a strong matching password', async () => {
    const wrapper = await setup();
    await goToAccount(wrapper);
    await wrapper.find('input[name="email"]').setValue('jane@x.com');
    await wrapper.find('input[name="password"]').setValue('secret12');
    await wrapper.find('input[name="confirmPassword"]').setValue('other');
    await submit(wrapper);
    expect(messages(wrapper, 'Account')).toEqual([
      'Must include an uppercase letter, a lowercase letter and a number, with no spaces',
      'Passwords must match',
      'You must accept the Terms & Conditions to continue',
    ]);
    expect(showAlertError).toHaveBeenLastCalledWith(requiredMessage);
  });

  it('registers the worker with every step in the payload', async () => {
    route.params = { requestId: '42' };
    const wrapper = await setup();
    wrapper.findComponent({ name: 'UploadImage' }).vm.$emit('imageSelected', new File(['img'], 'me.png', { type: 'image/png' }));
    await fillBasic(wrapper);
    await click(wrapper, 'Basic Information', 'Next');

    const preferences = step(wrapper, 'Preferences');
    const checkboxes = preferences.findAll('input[type="checkbox"]');
    await checkboxes[0].setValue(true);
    await checkboxes[1].setValue(true);
    await checkboxes[2].setValue(true);
    await preferences.find('select').setValue((preferences.findAll('select option')[1].element as HTMLOptionElement).value);
    await preferences.find('.switch input').setValue(true);
    await click(wrapper, 'Preferences', 'Next');

    await upload(wrapper, 0, pdf('passport.pdf'));
    await chooseId(wrapper, 1, passport, 'P1234');
    await upload(wrapper, 1, pdf('forklift.pdf'));
    await wrapper.find('input[name="description0"]').setValue('Forklift');
    wrapper.findAllComponents({ name: 'BDatepicker' })[1].vm.$emit('update:modelValue', new Date('2027-01-01T12:00:00'));
    await upload(wrapper, 3, pdf('cv.pdf'));
    await click(wrapper, 'Documents', 'Next');

    await fillAccount(wrapper);
    await step(wrapper, 'Account').find('input[type="checkbox"]').setValue(true);
    await submit(wrapper);

    expect(registerWorker).toHaveBeenCalledWith(expect.any(FormData), 42);
    const { formData, data } = sentPayload();
    expect(data).toMatchObject({
      firstName: 'Jane',
      lastName: 'Doe',
      gender: { id: 'g1' },
      email: 'jane@x.com',
      password: 'Secret123',
      confirmPassword: 'Secret123',
      location,
      mobileNumber: '+14165550100',
      availabilities: [{ id: 'a1', value: 'Full time' }],
      availabilityTimes: [{ id: 't1', value: 'Morning' }],
      availabilityDays: days,
      lift: { id: 'l1', value: '50 lbs' },
      hasVehicle: true,
      agreeTermsAndConditions: true,
      identificationType1: passport,
      identificationNumber1: 'P1234',
      identificationType2: null,
      identificationNumber2: '',
      licenses: [{ license: { description: 'Forklift' }, expires: new Date('2027-01-01T12:00:00').toISOString() }],
    });
    expect(data.birthDay).toBe(new Date('1995-03-10T12:00:00').toISOString());
    expect(data.profileImage).toEqual({ fileName: expect.stringMatching(/^ProfileImage_\w+\.png$/) });
    expect(data.identificationType1File).toEqual({ fileName: expect.stringMatching(/^Document_\w+\.pdf$/), description: '' });
    expect(data.resume).toEqual({ fileName: expect.stringMatching(/^Resume_\w+\.pdf$/) });
    expect(Array.from(formData.keys())).toHaveLength(5);
    expect(showAlertSuccess).toHaveBeenCalledWith('Your account has been created');
    expect(push).toHaveBeenCalledWith('/home');
  });

  it('selects all days at once', async () => {
    const wrapper = await setup();
    await fillBasic(wrapper);
    await click(wrapper, 'Basic Information', 'Next');
    const preferences = step(wrapper, 'Preferences');
    const allDays = preferences.findAll('input[type="checkbox"]')[2];
    await allDays.setValue(true);
    const dayBoxes = preferences.findAll('input[type="checkbox"]').slice(3, 5);
    expect(dayBoxes.map((b) => (b.element as HTMLInputElement).checked)).toEqual([true, true]);
    await dayBoxes[0].setValue(false);
    expect((allDays.element as HTMLInputElement).checked).toBe(false);
  });

  it('ignores a bad request id', async () => {
    route.params = { requestId: 'abc' };
    const wrapper = await setup();
    await goToAccount(wrapper);
    await fillAccount(wrapper);
    await step(wrapper, 'Account').find('input[type="checkbox"]').setValue(true);
    await submit(wrapper);
    expect(registerWorker).toHaveBeenCalledWith(expect.any(FormData), undefined);
  });

  it('reports a failed registration', async () => {
    vi.mocked(registerWorker).mockRejectedValue({ data: 'Email already registered' });
    const wrapper = await setup();
    await goToAccount(wrapper);
    await fillAccount(wrapper);
    await step(wrapper, 'Account').find('input[type="checkbox"]').setValue(true);
    await submit(wrapper);
    expect(showAlertError).toHaveBeenCalledWith('Email already registered');
    expect(push).not.toHaveBeenCalled();
  });
});

describe('Register by an agency user', () => {
  beforeEach(() => {
    security.user = { id: 'recruiter' };
  });

  it('skips preferences and terms and opens the new worker', async () => {
    const wrapper = await setup();
    expect(wrapper.find('.register-page__hero').exists()).toBe(false);
    await fillBasic(wrapper);
    await click(wrapper, 'Basic Information', 'Next');
    expect(activeStep(wrapper)).toBe(2);
    await click(wrapper, 'Documents', 'Previous');
    expect(activeStep(wrapper)).toBe(0);

    await click(wrapper, 'Basic Information', 'Next');
    await upload(wrapper, 0, pdf('passport.pdf'));
    await chooseId(wrapper, 1, passport, 'P1234');
    await click(wrapper, 'Documents', 'Next');
    await fillAccount(wrapper);
    await submit(wrapper);

    expect(sentPayload().data).toMatchObject({ agreeTermsAndConditions: false });
    expect(push).toHaveBeenCalledWith('/recruiting/workers/new-worker');
  });
});
