import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ProvinceSettingsModal from '@/shared/ui/ProvinceSettingsModal.vue';
import { updateProvinceSettings } from '@/shared/api/locationApi';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { ProvinceSettings } from '@/shared/types/common';

vi.mock('@/shared/api/locationApi', () => ({ updateProvinceSettings: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

function setup(currentSettings?: ProvinceSettings | null) {
  return mountWithPlugins(ProvinceSettingsModal, {
    props: { provinceId: 'prov-on', provinceName: 'Ontario', currentSettings },
  });
}

function input(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('input[name="overtimeStartsAfter"]');
}

async function save(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('button').trigger('click');
  await vi.waitFor(() => { expect(vi.mocked(updateProvinceSettings).mock.calls.length + vi.mocked(showAlertError).mock.calls.length).toBeGreaterThan(0); });
  await flushPromises();
}

describe('ProvinceSettingsModal', () => {
  it('shows the province and its current overtime threshold', async () => {
    const wrapper = setup({ overtimeStartsAfter: 44 });
    await flushPromises();

    expect(wrapper.find('h3').text()).toBe('Province Settings - Ontario');
    expect((input(wrapper).element as HTMLInputElement).value).toBe('44');
  });

  it('saves the typed threshold and emits it', async () => {
    vi.mocked(updateProvinceSettings).mockResolvedValue();
    const wrapper = setup();
    await input(wrapper).setValue('40.5');
    await save(wrapper);

    expect(updateProvinceSettings).toHaveBeenCalledWith('prov-on', { overtimeStartsAfter: 40.5 });
    expect(wrapper.emitted('saved')).toEqual([[{ overtimeStartsAfter: 40.5 }]]);
  });

  it('saves an empty threshold as null', async () => {
    vi.mocked(updateProvinceSettings).mockResolvedValue();
    const wrapper = setup({ overtimeStartsAfter: null });
    await save(wrapper);

    expect(updateProvinceSettings).toHaveBeenCalledWith('prov-on', { overtimeStartsAfter: null });
    expect(wrapper.emitted('saved')).toEqual([[{ overtimeStartsAfter: null }]]);
  });

  it('rejects a negative threshold', async () => {
    const wrapper = setup();
    await input(wrapper).setValue('-1');
    await save(wrapper);

    expect(showAlertError).toHaveBeenCalledWith('Please fill in all required fields correctly');
    expect(wrapper.find('.help.is-danger').text()).toBe('Must be >= 0');
    expect(updateProvinceSettings).not.toHaveBeenCalled();
    expect(wrapper.emitted('saved')).toBeUndefined();
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('boom');
    vi.mocked(updateProvinceSettings).mockRejectedValue(error);
    const wrapper = setup({ overtimeStartsAfter: 8 });
    await save(wrapper);

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('saved')).toBeUndefined();
  });
});
