import { afterEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import Export from '@/shared/ui/Export.vue';
import { downloadAgencyReport } from '@/shared/api/reportApi';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

function setup(slots: Record<string, string> = {}) {
  return mountWithPlugins(Export, {
    props: { url: 'api/agency/report', params: { status: 1 }, fileName: 'Workers' },
    slots,
  });
}

function exportItem(): HTMLElement {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.includes('Export'));
  if (!item) throw new Error('Export item not found');
  return item;
}

afterEach(() => {
  vi.useRealTimers();
});

describe('Export', () => {
  it('renders the extra actions next to the dropdown', () => {
    const wrapper = setup({
      actions: '<button class="extra-action">New</button>',
      'dropdown-actions': '<span class="extra-item">Print</span>',
    });

    expect(wrapper.find('.extra-action').exists()).toBe(true);
    expect(wrapper.find('button').text()).toContain('New');
    expect(document.body.querySelector('.extra-item')).not.toBeNull();
  });

  it('downloads the report named after the file and today', async () => {
    vi.useFakeTimers({ toFake: ['Date'] });
    vi.setSystemTime(new Date(2026, 10, 19, 10));
    const blob = new Blob(['x']);
    vi.mocked(downloadAgencyReport).mockResolvedValue(blob);
    const wrapper = setup();

    exportItem().click();
    await flushPromises();

    expect(downloadAgencyReport).toHaveBeenCalledWith('api/agency/report', { status: 1 });
    expect(downloadFile).toHaveBeenCalledWith(blob, `Workers_${new Date(2026, 10, 19).toLocaleDateString()}`);
    expect(wrapper.emitted('onDataLoading')).toEqual([[true], [false]]);
  });

  it('stops loading and shows the error when the download fails', async () => {
    const error = new Error('nope');
    vi.mocked(downloadAgencyReport).mockRejectedValue(error);
    const wrapper = setup();

    exportItem().click();
    await flushPromises();

    expect(downloadFile).not.toHaveBeenCalled();
    expect(wrapper.emitted('onDataLoading')).toEqual([[true], [false]]);
    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
