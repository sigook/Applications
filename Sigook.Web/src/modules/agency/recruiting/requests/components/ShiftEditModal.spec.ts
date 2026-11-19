import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ShiftEditModal from '@/modules/agency/recruiting/requests/components/ShiftEditModal.vue';
import { getAgencyRequestShift, updateAgencyRequestShift } from '@/modules/agency/recruiting/requests/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestShiftModel } from '@/shared/request-detail/types';

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'req-1' } }),
}));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({ getAgencyRequestShift: vi.fn(), updateAgencyRequestShift: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/request-detail/ShiftsForm.vue', () => ({
  default: {
    name: 'ShiftsForm',
    props: { currentShift: Object, isUpdate: Boolean },
    emits: ['updateModel'],
    template: '<div class="shift-stub" />',
  },
}));

const shift: RequestShiftModel = { monday: true, mondayStart: '08:00:00', mondayFinish: '16:00:00' };

async function setup() {
  const wrapper = mountWithPlugins(ShiftEditModal);
  await flushPromises();
  return wrapper;
}

describe('ShiftEditModal', () => {
  it('loads the request shift into the form', async () => {
    vi.mocked(getAgencyRequestShift).mockResolvedValue(shift);
    const wrapper = await setup();

    expect(getAgencyRequestShift).toHaveBeenCalledExactlyOnceWith('req-1');
    expect(wrapper.findComponent({ name: 'ShiftsForm' }).props()).toEqual({ currentShift: shift, isUpdate: true });
  });

  it('saves the edited shift and emits the display text', async () => {
    vi.mocked(getAgencyRequestShift).mockResolvedValue(shift);
    vi.mocked(updateAgencyRequestShift).mockResolvedValue({ id: 's-1', displayShift: 'Mon 8-4' });
    const wrapper = await setup();
    const edited: RequestShiftModel = { ...shift, tuesday: true };
    wrapper.findComponent({ name: 'ShiftsForm' }).vm.$emit('updateModel', edited);
    await wrapper.find('button.is-primary').trigger('click');
    await flushPromises();

    expect(updateAgencyRequestShift).toHaveBeenCalledExactlyOnceWith('req-1', edited);
    expect(wrapper.emitted('onUpdateShift')).toEqual([['Mon 8-4']]);
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('boom');
    vi.mocked(getAgencyRequestShift).mockResolvedValue(shift);
    vi.mocked(updateAgencyRequestShift).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.find('button.is-primary').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('onUpdateShift')).toBeUndefined();
  });

  it('shows the error and does nothing on save when loading fails', async () => {
    const error = new Error('boom');
    vi.mocked(getAgencyRequestShift).mockRejectedValue(error);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.findComponent({ name: 'ShiftsForm' }).exists()).toBe(false);
    await wrapper.find('button.is-primary').trigger('click');
    expect(updateAgencyRequestShift).not.toHaveBeenCalled();
  });
});
