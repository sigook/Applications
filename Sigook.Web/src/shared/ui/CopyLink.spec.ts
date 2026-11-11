import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import CopyLink from '@/shared/ui/CopyLink.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

describe('CopyLink', () => {
  const writeText = vi.fn();

  beforeEach(() => {
    vi.useFakeTimers();
    writeText.mockReset().mockResolvedValue(undefined);
    vi.stubGlobal('navigator', { ...navigator, clipboard: { writeText } });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('renders nothing without a value', () => {
    const wrapper = mountWithPlugins(CopyLink, { props: { kind: 'email', value: null } });
    expect(wrapper.find('.copy-link').exists()).toBe(false);
  });

  it('links emails with mailto', () => {
    const wrapper = mountWithPlugins(CopyLink, { props: { kind: 'email', value: 'ana@acme.com' } });
    const link = wrapper.find('a');
    expect(link.attributes('href')).toBe('mailto:ana@acme.com');
    expect(link.text()).toBe('ana@acme.com');
  });

  it('links phones with tel and shows the extension', () => {
    const wrapper = mountWithPlugins(CopyLink, { props: { kind: 'phone', value: '416 555-0199', ext: 12 } });
    const link = wrapper.find('a');
    expect(link.attributes('href')).toBe('tel:416 555-0199');
    expect(link.text()).toBe('416 555-0199 Ext. 12');
  });

  it('copies the value and shows the check icon for a moment', async () => {
    const wrapper = mountWithPlugins(CopyLink, { props: { kind: 'email', value: 'ana@acme.com' } });
    const button = wrapper.find('button.copy-link-copy');

    await button.trigger('click');
    await flushPromises();
    expect(writeText).toHaveBeenCalledWith('ana@acme.com');
    expect(button.find('.mdi-check').exists()).toBe(true);

    await vi.advanceTimersByTimeAsync(1500);
    expect(button.find('.mdi-content-copy').exists()).toBe(true);
  });

  it('shows an error when the clipboard rejects', async () => {
    writeText.mockRejectedValue(new Error('denied'));
    const wrapper = mountWithPlugins(CopyLink, { props: { kind: 'phone', value: '4165550199' } });

    await wrapper.find('button.copy-link-copy').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith('Could not copy to clipboard');
  });
});
