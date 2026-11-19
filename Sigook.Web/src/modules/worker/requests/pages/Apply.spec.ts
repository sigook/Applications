import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import Apply from '@/modules/worker/requests/pages/Apply.vue';
import { requestApplyByEmail } from '@/modules/worker/requests/api';
import { getErrorMessage } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

const { query } = vi.hoisted(() => ({ query: { value: {} as Record<string, string | string[]> } }));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ query: query.value }),
}));
vi.mock('@/modules/worker/requests/api', () => ({ requestApplyByEmail: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ getErrorMessage: vi.fn() }));

const location = { href: '' };

async function setup(params: Record<string, string | string[]>) {
  query.value = params;
  const wrapper = mountWithPlugins(Apply);
  await flushPromises();
  return wrapper;
}

function title(wrapper: Awaited<ReturnType<typeof setup>>) {
  return wrapper.find('.worker-apply-page__status-title').text();
}

beforeEach(() => {
  location.href = '';
  vi.stubGlobal('location', location);
  sessionStorage.clear();
});

describe('Apply', () => {
  it('applies with the emailed invitation', async () => {
    vi.mocked(requestApplyByEmail).mockResolvedValue();
    const wrapper = await setup({ n: '120', e: 'jane@x.com' });
    expect(requestApplyByEmail).toHaveBeenCalledWith(120, 'jane@x.com');
    expect(title(wrapper)).toBe('Application sent');
    expect(wrapper.text()).toContain('one of our recruiters will contact you soon');
    expect(sessionStorage.getItem('120|jane@x.com')).toBe('1');
  });

  it('uses the first value of repeated params', async () => {
    vi.mocked(requestApplyByEmail).mockResolvedValue();
    await setup({ n: ['7', '8'], e: ['a@x.com', 'b@x.com'] });
    expect(requestApplyByEmail).toHaveBeenCalledWith(7, 'a@x.com');
  });

  it('does not apply twice in the same session', async () => {
    sessionStorage.setItem('120|jane@x.com', '1');
    const wrapper = await setup({ n: '120', e: 'jane@x.com' });
    expect(requestApplyByEmail).not.toHaveBeenCalled();
    expect(title(wrapper)).toBe('Application sent');
  });

  it('shows the server error', async () => {
    vi.mocked(requestApplyByEmail).mockRejectedValue('closed');
    vi.mocked(getErrorMessage).mockResolvedValue('The request is closed');
    const wrapper = await setup({ n: '120', e: 'jane@x.com' });
    expect(title(wrapper)).toBe('We could not send your application');
    expect(wrapper.find('.worker-apply-page__status-text').text()).toBe('The request is closed');
    expect(sessionStorage.getItem('120|jane@x.com')).toBeNull();
  });

  it('falls back to a default error', async () => {
    vi.mocked(requestApplyByEmail).mockRejectedValue('x');
    vi.mocked(getErrorMessage).mockResolvedValue('');
    const wrapper = await setup({ n: '120', e: 'jane@x.com' });
    expect(wrapper.find('.worker-apply-page__status-text').text()).toBe('We could not process your application, please contact the agency.');
  });

  it.each([
    [{ e: 'jane@x.com' }],
    [{ n: 'abc', e: 'jane@x.com' }],
    [{ n: '-3', e: 'jane@x.com' }],
    [{ n: '1.5', e: 'jane@x.com' }],
    [{ n: '120' }],
  ])('goes home with an invalid invitation %o', async (params) => {
    await setup(params);
    expect(requestApplyByEmail).not.toHaveBeenCalled();
    expect(location.href).toBe('/');
  });

  it('shows a progress state and returns home from the result', async () => {
    let resolve: () => void = () => undefined;
    vi.mocked(requestApplyByEmail).mockReturnValue(new Promise<void>((r) => { resolve = r; }));
    const wrapper = await setup({ n: '120', e: 'jane@x.com' });
    expect(title(wrapper)).toBe('Sending your application');
    expect(wrapper.find('button').exists()).toBe(false);

    resolve();
    await flushPromises();
    await wrapper.find('button').trigger('click');
    expect(location.href).toBe('/');
  });
});
