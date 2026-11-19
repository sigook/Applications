import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import AgencyShiftDetail from '@/shared/request-detail/AgencyShiftDetail.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestShiftModel } from '@/shared/request-detail/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

type FetchShift = (requestId: string) => Promise<RequestShiftModel>;

const shift: RequestShiftModel = { monday: true, mondayStart: '08:00:00', mondayFinish: '17:00:00' };

function setup(fetchShift: FetchShift, displayShift: string | undefined = 'Mon-Fri 08:00-17:00') {
  return mountWithPlugins(AgencyShiftDetail, { props: { requestId: 'request-1', displayShift, fetchShift } });
}

function toggle(wrapper: ReturnType<typeof setup>) {
  return wrapper.find('button.shift-toggle');
}

describe('AgencyShiftDetail', () => {
  it('shows only the summary without a toggle when there is no shift', () => {
    const wrapper = setup(vi.fn<FetchShift>(), '');

    expect(toggle(wrapper).exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'ShiftDetail' }).exists()).toBe(false);
  });

  it('loads and shows the shift detail on click', async () => {
    let resolve: (value: RequestShiftModel) => void = () => undefined;
    const fetchShift = vi.fn<FetchShift>().mockReturnValue(new Promise((r) => { resolve = r; }));
    const wrapper = setup(fetchShift);

    expect(wrapper.text()).toBe('Mon-Fri 08:00-17:00');
    expect(toggle(wrapper).attributes('aria-label')).toBe('Show shift detail');
    await toggle(wrapper).trigger('click');

    expect(fetchShift).toHaveBeenCalledExactlyOnceWith('request-1');
    const detail = wrapper.findComponent({ name: 'ShiftDetail' });
    expect(detail.props('isLoading')).toBe(true);
    expect(detail.props('anchor')).toBe(wrapper.element);
    resolve(shift);
    await vi.waitFor(() => { expect(detail.props('shift')).toEqual(shift); });
    expect(detail.props('isLoading')).toBe(false);
    expect(toggle(wrapper).attributes('aria-label')).toBe('Hide shift detail');
    expect(toggle(wrapper).attributes('aria-expanded')).toBe('true');
  });

  it('hides the detail on a second click', async () => {
    const fetchShift = vi.fn<FetchShift>().mockResolvedValue(shift);
    const wrapper = setup(fetchShift);
    await toggle(wrapper).trigger('click');
    await toggle(wrapper).trigger('click');

    expect(wrapper.findComponent({ name: 'ShiftDetail' }).exists()).toBe(false);
    expect(fetchShift).toHaveBeenCalledTimes(1);
  });

  it('closes when clicking outside', async () => {
    const wrapper = setup(vi.fn<FetchShift>().mockResolvedValue(shift));
    await toggle(wrapper).trigger('click');
    await nextTick();

    document.body.click();
    await nextTick();

    expect(wrapper.findComponent({ name: 'ShiftDetail' }).exists()).toBe(false);
  });

  it('stops loading and shows the error when the shift fails to load', async () => {
    const error = new Error('load failed');
    const wrapper = setup(vi.fn<FetchShift>().mockRejectedValue(error));
    await toggle(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.findComponent({ name: 'ShiftDetail' }).props('isLoading')).toBe(false);
  });
});
