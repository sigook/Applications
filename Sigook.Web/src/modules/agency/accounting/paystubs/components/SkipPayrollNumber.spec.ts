import { afterEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import SkipPayrollNumber from '@/modules/agency/accounting/paystubs/components/SkipPayrollNumber.vue';
import { addSkipPayrollNumber, getSkipPayrollNumbers } from '@/modules/agency/accounting/paystubs/api';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/accounting/paystubs/api', () => ({ addSkipPayrollNumber: vi.fn(), getSkipPayrollNumbers: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => ({ getDialog: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

interface PromptConfig {
  inputAttrs: { value: string | null };
  onConfirm: (value: string, dialog: { close: () => void }) => Promise<void>;
}

function setupDialog() {
  const prompt = vi.fn<(config: PromptConfig) => void>();
  vi.mocked(getDialog).mockReturnValue({ prompt, confirm: vi.fn(), alert: vi.fn() } as unknown as ReturnType<typeof getDialog>);
  return prompt;
}

function autocomplete(wrapper: ReturnType<typeof mountWithPlugins>) {
  return wrapper.findComponent({ name: 'BAutocomplete' });
}

afterEach(() => {
  vi.useRealTimers();
});

describe('SkipPayrollNumber', () => {
  it('searches skip numbers after the debounce', async () => {
    vi.useFakeTimers({ toFake: ['setTimeout', 'clearTimeout'] });
    vi.mocked(getSkipPayrollNumbers).mockResolvedValue([{ id: 's-1', value: '1001' }]);
    const wrapper = mountWithPlugins(SkipPayrollNumber);
    autocomplete(wrapper).vm.$emit('typing', '10');
    vi.advanceTimersByTime(300);
    await flushPromises();

    expect(getSkipPayrollNumbers).toHaveBeenCalledExactlyOnceWith({ searchTerm: '10' });
    expect(autocomplete(wrapper).props('data')).toEqual([{ id: 's-1', value: '1001' }]);
  });

  it('ignores empty searches', async () => {
    vi.useFakeTimers({ toFake: ['setTimeout', 'clearTimeout'] });
    const wrapper = mountWithPlugins(SkipPayrollNumber);
    autocomplete(wrapper).vm.$emit('typing', null);
    vi.advanceTimersByTime(300);
    await flushPromises();

    expect(getSkipPayrollNumbers).not.toHaveBeenCalled();
  });

  it('adds a new number from the footer prompt and closes it', async () => {
    const prompt = setupDialog();
    vi.mocked(addSkipPayrollNumber).mockResolvedValue();
    const wrapper = mountWithPlugins(SkipPayrollNumber);
    autocomplete(wrapper).vm.$emit('update:modelValue', '2002');
    await nextTick();
    autocomplete(wrapper).vm.$emit('select-footer');

    const config = prompt.mock.calls[0][0];
    expect(config.inputAttrs.value).toBe('2002');
    const close = vi.fn();
    await config.onConfirm('2002', { close });

    expect(addSkipPayrollNumber).toHaveBeenCalledExactlyOnceWith({ value: '2002' });
    expect(close).toHaveBeenCalledOnce();
  });

  it('keeps the prompt open and shows the error when adding fails', async () => {
    const prompt = setupDialog();
    const error = new Error('duplicate');
    vi.mocked(addSkipPayrollNumber).mockRejectedValue(error);
    const wrapper = mountWithPlugins(SkipPayrollNumber);
    autocomplete(wrapper).vm.$emit('select-footer');

    const close = vi.fn();
    await prompt.mock.calls[0][0].onConfirm('2002', { close });

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(close).not.toHaveBeenCalled();
  });
});
