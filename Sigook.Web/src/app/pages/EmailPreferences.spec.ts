import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import EmailPreferences from '@/app/pages/EmailPreferences.vue';
import { unsubscribe } from '@/shared/api/sharedApi';
import { getErrorMessage } from '@/shared/utils/toast';
import { createTestRouter } from '@/testing/mountWithPlugins';

vi.mock('@/shared/api/sharedApi', () => ({ unsubscribe: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ getErrorMessage: vi.fn() }));

let location: { href: string };

async function setup(path = '/email-preferences?email=ann@acme.com&t=type-1') {
  const router = createTestRouter();
  await router.push(path);
  await router.isReady();
  return mount(EmailPreferences, { attachTo: document.body, global: { plugins: [Buefy, router] } });
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function clickButton(wrapper: Wrapper, text: string) {
  const button = wrapper.findAll('button').find((b) => b.text() === text);
  if (!button) throw new Error(`Button ${text} not found`);
  return button.trigger('click');
}

describe('EmailPreferences', () => {
  beforeEach(() => {
    window.sessionStorage.clear();
    location = { href: '/email-preferences' };
    vi.spyOn(window, 'location', 'get').mockReturnValue(location as Location);
  });

  it('goes home on cancel', async () => {
    const wrapper = await setup();
    await clickButton(wrapper, 'Cancel');

    expect(location.href).toBe('/');
    expect(unsubscribe).not.toHaveBeenCalled();
  });

  it('unsubscribes, remembers it and goes home', async () => {
    vi.mocked(unsubscribe).mockResolvedValue();
    const wrapper = await setup();
    await clickButton(wrapper, 'Yes');
    await flushPromises();

    expect(unsubscribe).toHaveBeenCalledExactlyOnceWith({ email: 'ann@acme.com', typeId: 'type-1' });
    expect(window.sessionStorage.getItem('ann@acme.comtype-1')).toBe('1');
    expect(location.href).toBe('/');
  });

  it('unsubscribes from every type without a type id', async () => {
    vi.mocked(unsubscribe).mockResolvedValue();
    const wrapper = await setup('/email-preferences?email=ann@acme.com');
    await clickButton(wrapper, 'Yes');
    await flushPromises();

    expect(unsubscribe).toHaveBeenCalledExactlyOnceWith({ email: 'ann@acme.com', typeId: undefined });
    expect(window.sessionStorage.getItem('ann@acme.com')).toBe('1');
  });

  it('goes home without an email', async () => {
    const wrapper = await setup('/email-preferences');
    await clickButton(wrapper, 'Yes');

    expect(unsubscribe).not.toHaveBeenCalled();
    expect(location.href).toBe('/');
  });

  it('does not unsubscribe twice in the same session', async () => {
    window.sessionStorage.setItem('ann@acme.comtype-1', '1');
    const wrapper = await setup();
    await clickButton(wrapper, 'Yes');

    expect(unsubscribe).not.toHaveBeenCalled();
    expect(location.href).toBe('/');
  });

  it('shows the error and stays on the page when unsubscribing fails', async () => {
    const failure = new Error('down');
    vi.mocked(unsubscribe).mockRejectedValue(failure);
    vi.mocked(getErrorMessage).mockResolvedValue('<b>Email not found</b>');
    const wrapper = await setup();
    await clickButton(wrapper, 'Yes');
    await flushPromises();

    expect(getErrorMessage).toHaveBeenCalledWith(failure);
    expect(wrapper.find('.alert-warning-red').html()).toContain('<b>Email not found</b>');
    expect(location.href).toBe('/email-preferences');
    expect(window.sessionStorage.getItem('ann@acme.comtype-1')).toBe('1');
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });
});
