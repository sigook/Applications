import { afterEach, describe, expect, it, vi } from 'vitest';
import GetInTouchForm from '@/modules/landing/components/shared/forms/GetInTouchForm.vue';
import { submitContactForm } from '@/modules/landing/api';
import { getCountries, getProvinces } from '@/shared/api/locationApi';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/api', () => ({ submitContactForm: vi.fn() }));
vi.mock('@/shared/api/locationApi', () => ({ getCountries: vi.fn(), getProvinces: vi.fn() }));
vi.mock('vue3-recaptcha2', async () => {
  const { defineComponent } = await import('vue');
  return {
    default: defineComponent({
      name: 'Vue3Recaptcha2',
      props: { sitekey: { type: String, default: '' } },
      emits: ['verify', 'expire', 'fail'],
      template: '<div class="recaptcha-stub" />',
    }),
  };
});

async function settle() {
  for (let i = 0; i < 10; i++) await new Promise((resolve) => { setTimeout(resolve, 0); });
}

async function setup(size?: 'default' | 'compact') {
  vi.mocked(getCountries).mockResolvedValue([
    { id: 'country-ca', value: 'Canada', code: 'CA' },
    { id: 'country-us', value: 'United States', code: 'USA' },
  ]);
  vi.mocked(getProvinces).mockResolvedValue([{ id: 'state-ny', value: 'New York' }, { id: 'state-tx', value: 'Texas' }]);
  const wrapper = mountWithPlugins(GetInTouchForm, { props: size ? { size } : {} });
  await settle();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

async function fill(wrapper: Wrapper) {
  await wrapper.find('input[placeholder="First name"]').setValue('Ada');
  await wrapper.find('input[placeholder="Last name"]').setValue('Lovelace');
  await wrapper.find('input[type="email"]').setValue('ada@acme.com');
  await wrapper.find('input[placeholder="Your company"]').setValue('Acme');
  await wrapper.find('select').setValue('Texas');
  await wrapper.find('input[placeholder="Select one"]').setValue('Logistics');
  await wrapper.find('textarea').setValue('Need 10 pickers');
}

function verifyCaptcha(wrapper: Wrapper, token = 'captcha-token') {
  wrapper.findComponent({ name: 'Vue3Recaptcha2' }).vm.$emit('verify', token);
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
  await settle();
}

function fieldErrors(wrapper: Wrapper) {
  return wrapper.findAll('.git-card__field-error').map((e) => e.text());
}

function firstName(wrapper: Wrapper) {
  return (wrapper.find('input[placeholder="First name"]').element as HTMLInputElement).value;
}

describe('GetInTouchForm', () => {
  afterEach(() => {
    vi.useRealTimers();
  });

  it('applies the size modifier', async () => {
    expect((await setup()).classes()).toContain('git-card--default');
    expect((await setup('compact')).classes()).toContain('git-card--compact');
  });

  it('loads the US states', async () => {
    const wrapper = await setup();

    expect(getProvinces).toHaveBeenCalledWith('country-us');
    expect(wrapper.findAll('option').map((o) => o.text())).toEqual(['Select a state', 'New York', 'Texas']);
  });

  it('leaves the states empty when there is no USA country', async () => {
    vi.mocked(getCountries).mockResolvedValue([]);
    const wrapper = mountWithPlugins(GetInTouchForm);
    await settle();

    expect(getProvinces).not.toHaveBeenCalled();
    expect(wrapper.findAll('option')).toHaveLength(1);
  });

  it('leaves the states empty when loading fails', async () => {
    vi.mocked(getCountries).mockRejectedValue(new Error('down'));
    const wrapper = mountWithPlugins(GetInTouchForm);
    await settle();

    expect(wrapper.findAll('option')).toHaveLength(1);
  });

  it('shows the required errors and does not submit', async () => {
    const wrapper = await setup();
    await submit(wrapper);

    expect(fieldErrors(wrapper)).toEqual([
      'First name is required',
      'Last name is required',
      'Email is required',
      'Company is required',
      'State is required',
      'Message is required',
    ]);
    expect(submitContactForm).not.toHaveBeenCalled();
  });

  it('rejects an invalid email', async () => {
    const wrapper = await setup();
    await fill(wrapper);
    await wrapper.find('input[type="email"]').setValue('nope');
    await submit(wrapper);

    expect(fieldErrors(wrapper)).toEqual(['Invalid email address']);
  });

  it('asks to verify the captcha', async () => {
    const wrapper = await setup();
    await fill(wrapper);
    await submit(wrapper);

    expect(fieldErrors(wrapper)).toEqual(['Please verify you are human.']);
    expect(submitContactForm).not.toHaveBeenCalled();
  });

  it('flags a failed captcha and drops an expired token', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'Vue3Recaptcha2' }).vm.$emit('fail');
    await settle();
    expect(fieldErrors(wrapper)).toEqual(['Please verify you are human.']);

    verifyCaptcha(wrapper);
    await settle();
    expect(fieldErrors(wrapper)).toEqual([]);

    wrapper.findComponent({ name: 'Vue3Recaptcha2' }).vm.$emit('expire');
    await fill(wrapper);
    await submit(wrapper);
    expect(fieldErrors(wrapper)).toEqual(['Please verify you are human.']);
  });

  it('sends the contact request and resets the form', async () => {
    vi.mocked(submitContactForm).mockResolvedValue();
    const wrapper = await setup();
    await fill(wrapper);
    verifyCaptcha(wrapper);
    await submit(wrapper);

    expect(submitContactForm).toHaveBeenCalledExactlyOnceWith({
      title: 'Get in Touch',
      name: 'Ada Lovelace',
      email: 'ada@acme.com',
      phone: '',
      company: 'Acme',
      location: 'Texas',
      message: 'Need 10 pickers',
      subject: 'Logistics',
      captchaResponse: 'captcha-token',
    });
    expect(wrapper.find('.git-card__feedback--success').text()).toBe('Thank you! We\'ll reach out to you shortly.');
    expect(firstName(wrapper)).toBe('');
    expect(fieldErrors(wrapper)).toEqual([]);
  });

  it('sends an empty subject without an industry', async () => {
    vi.mocked(submitContactForm).mockResolvedValue();
    const wrapper = await setup();
    await fill(wrapper);
    await wrapper.find('input[placeholder="Select one"]').setValue('');
    verifyCaptcha(wrapper);
    await submit(wrapper);

    expect(submitContactForm).toHaveBeenCalledWith(expect.objectContaining({ subject: '' }));
  });

  it('shows an error when sending fails and re-renders the captcha', async () => {
    vi.mocked(submitContactForm).mockRejectedValue(new Error('boom'));
    const wrapper = await setup();
    await fill(wrapper);
    verifyCaptcha(wrapper);
    await submit(wrapper);

    expect(wrapper.find('.git-card__feedback--error').text()).toBe('Something went wrong. Please try again later.');
    expect(wrapper.find('.git-card__feedback--success').exists()).toBe(false);
    expect(firstName(wrapper)).toBe('Ada');
    await vi.waitFor(() => { expect(wrapper.find('.recaptcha-stub').exists()).toBe(true); });
  });

  it('resets the information and the captcha', async () => {
    vi.useFakeTimers();
    vi.mocked(getCountries).mockResolvedValue([{ id: 'country-us', value: 'United States', code: 'USA' }]);
    vi.mocked(getProvinces).mockResolvedValue([{ id: 'state-tx', value: 'Texas' }]);
    const wrapper = mountWithPlugins(GetInTouchForm);
    await vi.advanceTimersByTimeAsync(10);
    await fill(wrapper);
    verifyCaptcha(wrapper);
    const reset = wrapper.findAll('button').find((b) => b.text() === 'Reset information');
    await reset?.trigger('click');

    expect(firstName(wrapper)).toBe('');
    expect(wrapper.find('.recaptcha-stub').exists()).toBe(false);
    await vi.advanceTimersByTimeAsync(100);
    expect(wrapper.find('.recaptcha-stub').exists()).toBe(true);
    await fill(wrapper);
    await wrapper.find('form').trigger('submit');
    await vi.advanceTimersByTimeAsync(10);
    expect(fieldErrors(wrapper)).toEqual(['Please verify you are human.']);
    expect(submitContactForm).not.toHaveBeenCalled();
  });
});
