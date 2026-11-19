import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import T4 from '@/modules/agency/accounting/reports/components/T4.vue';
import { getT4Report } from '@/modules/agency/accounting/reports/api';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/accounting/reports/api', () => ({ getT4Report: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

async function pickDates(wrapper: ReturnType<typeof mountWithPlugins>) {
  wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [new Date(2026, 0, 1), new Date(2026, 11, 31)]);
  await nextTick();
}

async function generate(wrapper: ReturnType<typeof mountWithPlugins>) {
  await wrapper.find('button').trigger('click');
}

describe('T4', () => {
  it('requires the date range', async () => {
    const wrapper = mountWithPlugins(T4);
    await generate(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Dates are required'); });
    expect(getT4Report).not.toHaveBeenCalled();
  });

  it('downloads the report for the picked dates', async () => {
    const blob = new Blob(['x']);
    vi.mocked(getT4Report).mockResolvedValue(blob);
    const wrapper = mountWithPlugins(T4);
    await pickDates(wrapper);
    await generate(wrapper);

    await vi.waitFor(() => { expect(downloadFile).toHaveBeenCalledWith(blob, 'T4_2026-01-01_2026-12-31'); });
    expect(getT4Report).toHaveBeenCalledExactlyOnceWith({ startDate: '2026-01-01', endDate: '2026-12-31' });
  });

  it('shows the error when the report fails', async () => {
    const error = new Error('report');
    vi.mocked(getT4Report).mockRejectedValue(error);
    const wrapper = mountWithPlugins(T4);
    await pickDates(wrapper);
    await generate(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    await flushPromises();
    expect(wrapper.findComponent({ name: 'BButton' }).props('loading')).toBe(false);
  });
});
