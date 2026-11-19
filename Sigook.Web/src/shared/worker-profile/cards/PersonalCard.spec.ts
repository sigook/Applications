import { afterEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import PersonalCard from '@/shared/worker-profile/cards/PersonalCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

const { formStub } = vi.hoisted(() => ({
  formStub: (name: string) => ({ name, props: { data: Object, save: Function }, emits: ['closeModal'], template: `<div class="${name}" />` }),
}));

vi.mock('@/shared/worker-profile/forms/WorkBasicInformationForm.vue', () => ({ default: formStub('BasicInformationForm') }));
vi.mock('@/shared/worker-profile/forms/WorkEmailForm.vue', () => ({ default: formStub('EmailForm') }));
vi.mock('@/shared/worker-profile/forms/WorkSinForm.vue', () => ({ default: formStub('SinForm') }));

interface SetupOptions {
  worker?: Partial<WorkerProfileDetail>;
  showLoginEmail?: boolean;
  updateEmail?: (workerId: string, model: { newEmail: string }) => Promise<unknown>;
}

function setup({ worker = {}, showLoginEmail, updateEmail }: SetupOptions = {}) {
  return mountWithPlugins(PersonalCard, {
    props: { worker: createWorkerProfileDetail(worker), showLoginEmail, updateEmail },
  });
}

function field(wrapper: ReturnType<typeof setup>, label: string) {
  const row = wrapper.findAll('.profile-field').find((f) => f.find('dt').text() === label);
  return row?.find('dd');
}

function menuItems() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).map((i) => i.textContent.trim());
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

afterEach(() => {
  vi.useRealTimers();
});

describe('PersonalCard', () => {
  it('shows the basic personal data', () => {
    const wrapper = setup({
      worker: { firstName: 'John', middleName: 'Paul', lastName: 'Smith', secondLastName: 'Lee', gender: { id: 'g1', value: 'Male' }, hasVehicle: true },
    });

    expect(field(wrapper, 'Full name')?.text()).toBe('John Paul Smith Lee');
    expect(field(wrapper, 'Date of birth')?.text()).toBe('15-May-1990');
    expect(field(wrapper, 'Gender')?.text()).toBe('Male');
    expect(field(wrapper, 'Own vehicle')?.text()).toBe('Yes');
    expect(field(wrapper, 'Login email')?.find('a').attributes('href')).toBe('mailto:john.smith@example.com');
  });

  it('shows placeholders for missing data', () => {
    const wrapper = setup();

    expect(field(wrapper, 'Full name')?.text()).toBe('John Smith');
    expect(field(wrapper, 'Gender')?.text()).toBe('—');
    expect(field(wrapper, 'Own vehicle')?.text()).toBe('No');
    expect(field(wrapper, 'SIN/SSN')?.text()).toBe('—');
    expect(field(wrapper, 'SIN/SSN expiry')?.text()).toBe('Does not expire');
    expect(field(wrapper, 'SIN/SSN file')?.text()).toBe('—');
  });

  it('hides the login email when disabled', () => {
    const wrapper = setup({ showLoginEmail: false });

    expect(field(wrapper, 'Login email')).toBeUndefined();
    expect(menuItems()).toEqual(['Basic information', 'SIN/SSN']);
  });

  it('masks the SIN until revealed', async () => {
    const wrapper = setup({ worker: { socialInsurance: '123456789' } });
    expect(field(wrapper, 'SIN/SSN')?.text()).toBe('******6789');

    await wrapper.find('button[aria-label="Show SIN/SSN"]').trigger('click');
    expect(field(wrapper, 'SIN/SSN')?.text()).toBe('123456789');

    await wrapper.find('button[aria-label="Hide SIN/SSN"]').trigger('click');
    expect(field(wrapper, 'SIN/SSN')?.text()).toBe('******6789');
  });

  it('shows the SIN expiry status', () => {
    vi.useFakeTimers({ now: new Date('2026-06-01T12:00:00'), toFake: ['Date'] });
    const wrapper = setup({ worker: { socialInsuranceExpire: true, dueDate: '2026-05-01T12:00:00' } });

    expect(field(wrapper, 'SIN/SSN expiry')?.text()).toBe('01-May-2026 Expired');
    expect(field(wrapper, 'SIN/SSN expiry')?.find('.profile-status').classes()).toContain('is-danger');
  });

  it('links the SIN file', () => {
    const wrapper = setup({ worker: { socialInsuranceFile: { fileName: 'sin card.pdf', pathFile: 'https://files/sin.pdf' } } });
    const link = field(wrapper, 'SIN/SSN file')?.find('a');

    expect(link?.attributes('href')).toBe('https://files/sin.pdf');
    expect(link?.text()).toBe('SIN.PDF');
  });

  it('opens the login email form with the given save function', async () => {
    const updateEmail = vi.fn<(workerId: string, model: { newEmail: string }) => Promise<unknown>>();
    const wrapper = setup({ updateEmail });
    menuItem('Login email').click();
    await nextTick();

    const form = wrapper.findComponent({ name: 'EmailForm' });
    expect(form.props('save')).toBe(updateEmail);
    form.vm.$emit('closeModal');
    expect(wrapper.emitted('updateProfile')).toHaveLength(1);
  });

  it('does not open the login email form without a save function', async () => {
    const wrapper = setup();
    menuItem('Login email').click();
    await nextTick();

    expect(wrapper.findComponent({ name: 'EmailForm' }).exists()).toBe(false);
  });

  it.each([
    ['Basic information', 'BasicInformationForm'],
    ['SIN/SSN', 'SinForm'],
  ])('opens the %s form and emits updateProfile once it closes', async (item, name) => {
    const wrapper = setup();
    menuItem(item).click();
    await nextTick();

    const form = wrapper.findComponent({ name });
    expect(form.exists()).toBe(true);
    form.vm.$emit('closeModal');
    expect(wrapper.emitted('updateProfile')).toHaveLength(1);
  });
});
