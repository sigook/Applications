import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import ShiftDetail from '@/shared/request-detail/ShiftDetail.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestShiftModel } from '@/shared/request-detail/types';

const shift: RequestShiftModel = {
  sunday: true,
  monday: true,
  tuesday: true,
  wednesday: true,
  thursday: true,
  friday: true,
  saturday: true,
  sundayStart: '06:00:00',
  sundayFinish: '14:00:00',
  mondayStart: '08:00:00',
  mondayFinish: '17:00:00',
  tuesdayStart: '08:00:00',
  tuesdayFinish: '17:00:00',
  wednesdayStart: '08:30:00',
  wednesdayFinish: '17:30:00',
  thursdayStart: '08:00:00',
  thursdayFinish: '17:00:00',
  fridayStart: '08:00:00',
  fridayFinish: '12:00:00',
  saturdayStart: '22:00:00',
  saturdayFinish: '06:00:00',
  comments: 'Bring boots',
};

function anchorAt(rect: Partial<DOMRect>) {
  const anchor = document.createElement('button');
  document.body.appendChild(anchor);
  vi.spyOn(anchor, 'getBoundingClientRect').mockReturnValue({ top: 0, left: 0, bottom: 0, right: 0, width: 0, height: 0, x: 0, y: 0, toJSON: () => ({}), ...rect });
  return anchor;
}

function panel() {
  const element = document.body.querySelector<HTMLElement>('.table-shift-detail');
  if (!element) throw new Error('Panel not found');
  return element;
}

describe('ShiftDetail', () => {
  it('lists every working day with its hours and the comments', () => {
    const wrapper = mountWithPlugins(ShiftDetail, { props: { shift } });

    expect(wrapper.findAll('li').map((li) => li.text())).toEqual([
      'Sun 06:00 to 14:00',
      'Mon 08:00 to 17:00',
      'Tue 08:00 to 17:00',
      'Wed 08:30 to 17:30',
      'Thu 08:00 to 17:00',
      'Fri 08:00 to 12:00',
      'Sat 22:00 to 06:00',
      '*Bring boots',
    ]);
    expect(wrapper.find('.table-shift-detail').classes()).not.toContain('is-floating');
  });

  it('skips the days off', () => {
    const wrapper = mountWithPlugins(ShiftDetail, {
      props: { shift: { monday: true, mondayStart: '09:00:00', mondayFinish: '15:00:00', friday: false } },
    });

    expect(wrapper.findAll('li').map((li) => li.text())).toEqual(['Mon 09:00 to 15:00']);
  });

  it('renders no list without a shift', () => {
    expect(mountWithPlugins(ShiftDetail, { props: { shift: null } }).find('ul').exists()).toBe(false);
  });

  it('follows the loading prop', async () => {
    const wrapper = mountWithPlugins(ShiftDetail, { props: { shift: null, isLoading: true } });
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(true);

    await wrapper.setProps({ isLoading: false });
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('floats below the anchor in the body', async () => {
    const anchor = anchorAt({ left: 120, bottom: 300 });
    mountWithPlugins(ShiftDetail, { props: { shift, anchor } });
    await nextTick();

    expect(panel().classList).toContain('is-floating');
    expect(panel().parentElement).toBe(document.body);
    expect(panel().style.top).toBe('304px');
    expect(panel().style.left).toBe('120px');
  });

  it('keeps the floating panel inside the viewport', async () => {
    const anchor = anchorAt({ left: -50, bottom: 10 });
    mountWithPlugins(ShiftDetail, { props: { shift, anchor } });
    await nextTick();
    expect(panel().style.left).toBe('8px');

    vi.mocked(anchor.getBoundingClientRect).mockReturnValue({ ...anchor.getBoundingClientRect(), left: window.innerWidth + 100, bottom: 50 });
    window.dispatchEvent(new Event('resize'));
    await nextTick();

    expect(panel().style.left).toBe(`${window.innerWidth - 8}px`);
    expect(panel().style.top).toBe('54px');
  });

  it('repositions when the page scrolls or the shift loads', async () => {
    vi.stubGlobal('requestAnimationFrame', (callback: FrameRequestCallback) => { callback(0); return 0; });
    const anchor = anchorAt({ left: 100, bottom: 100 });
    const wrapper = mountWithPlugins(ShiftDetail, { props: { shift: null, anchor } });

    vi.mocked(anchor.getBoundingClientRect).mockReturnValue({ ...anchor.getBoundingClientRect(), bottom: 200 });
    document.dispatchEvent(new Event('scroll'));
    await nextTick();
    expect(panel().style.top).toBe('204px');

    vi.mocked(anchor.getBoundingClientRect).mockReturnValue({ ...anchor.getBoundingClientRect(), bottom: 400 });
    await wrapper.setProps({ shift });
    expect(panel().style.top).toBe('404px');
  });

  it('stops listening to the window once unmounted', () => {
    const remove = vi.spyOn(window, 'removeEventListener');
    const wrapper = mountWithPlugins(ShiftDetail, { props: { shift, anchor: anchorAt({}) } });
    wrapper.unmount();

    expect(remove).toHaveBeenCalledWith('scroll', expect.any(Function), true);
    expect(remove).toHaveBeenCalledWith('resize', expect.any(Function));
  });
});
