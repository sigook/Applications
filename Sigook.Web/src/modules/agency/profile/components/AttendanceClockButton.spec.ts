import { afterEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import AttendanceClockButton from '@/modules/agency/profile/components/AttendanceClockButton.vue';
import { getAttendanceToday, toggleAttendance } from '@/modules/agency/profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { AttendanceStatus, type UserAttendanceToday } from '@/modules/agency/profile/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({ getAttendanceToday: vi.fn(), toggleAttendance: vi.fn() }));

const notStarted: UserAttendanceToday = { userId: 'u-1', status: AttendanceStatus.NotStarted, clockIn: null, clockOut: null, workedHours: null };
const clockedIn: UserAttendanceToday = { ...notStarted, status: AttendanceStatus.ClockedIn, clockIn: '2026-11-19T08:00:00' };
const clockedOut: UserAttendanceToday = { ...clockedIn, status: AttendanceStatus.ClockedOut, clockOut: '2026-11-19T17:00:00', workedHours: 8 };

async function setup(today: UserAttendanceToday) {
  vi.mocked(getAttendanceToday).mockResolvedValue(today);
  const wrapper = mountWithPlugins(AttendanceClockButton);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function clock(wrapper: Wrapper) {
  return wrapper.find('.attendance-clock-button');
}

function modalButton(text: string) {
  const found = Array.from(document.body.querySelectorAll<HTMLButtonElement>('.modal button')).find((b) => b.textContent.trim() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

afterEach(() => {
  vi.useRealTimers();
});

describe('AttendanceClockButton', () => {
  it('renders nothing until the attendance loads', () => {
    vi.mocked(getAttendanceToday).mockReturnValue(new Promise(() => undefined));
    const wrapper = mountWithPlugins(AttendanceClockButton);

    expect(wrapper.find('.attendance-clock').exists()).toBe(false);
  });

  it('clocks in directly when the day has not started', async () => {
    vi.mocked(toggleAttendance).mockResolvedValue(clockedIn);
    const wrapper = await setup(notStarted);

    expect(clock(wrapper).classes()).toContain('is-idle');
    expect(clock(wrapper).attributes('aria-label')).toBe('Clock in');

    await clock(wrapper).trigger('click');
    await flushPromises();

    expect(toggleAttendance).toHaveBeenCalledOnce();
    expect(clock(wrapper).classes()).toContain('is-running');
  });

  it('shows the running time and asks before clocking out', async () => {
    vi.useFakeTimers({ now: new Date('2026-11-19T18:30:00'), toFake: ['Date'] });
    vi.mocked(toggleAttendance).mockResolvedValue(clockedOut);
    const wrapper = await setup(clockedIn);

    expect(clock(wrapper).attributes('aria-label')).toBe('Clock out · in since 08:00 (10:30)');

    await clock(wrapper).trigger('click');
    await nextTick();

    expect(toggleAttendance).not.toHaveBeenCalled();
    const modal = document.body.querySelector('.modal');
    expect(modal?.textContent).toContain('Clock out for today?');
    expect(Array.from(document.body.querySelectorAll('.modal .title.is-4')).map((t) => t.textContent.trim())).toEqual(['08:00', '18:30']);
    expect(Array.from(document.body.querySelectorAll('.modal tbody tr')).map((r) => Array.from(r.children).map((c) => c.textContent.trim()).join(' '))).toEqual([
      'Worked 9.50 h',
      'Regular 8.00 h',
      'Overtime 1.50 h',
    ]);

    modalButton('Clock out').click();
    await flushPromises();

    expect(toggleAttendance).toHaveBeenCalledOnce();
    expect(clock(wrapper).classes()).toContain('is-closed');
    expect(clock(wrapper).attributes('aria-label')).toBe('Day closed · 8.00 h worked');
  });

  it('cancels the clock out', async () => {
    const wrapper = await setup(clockedIn);
    await clock(wrapper).trigger('click');
    await nextTick();

    modalButton('Cancel').click();
    await flushPromises();

    expect(toggleAttendance).not.toHaveBeenCalled();
    expect(clock(wrapper).classes()).toContain('is-running');
  });

  it('does nothing once the day is closed', async () => {
    const wrapper = await setup(clockedOut);

    expect(clock(wrapper).attributes('aria-disabled')).toBe('true');
    await clock(wrapper).trigger('click');
    await flushPromises();

    expect(toggleAttendance).not.toHaveBeenCalled();
    expect(document.body.querySelector('.modal')?.textContent ?? '').not.toContain('Clock out for today?');
  });

  it('refreshes the elapsed time every 30 seconds', async () => {
    vi.useFakeTimers({ now: new Date('2026-11-19T09:00:00'), toFake: ['Date', 'setInterval', 'clearInterval'] });
    const wrapper = await setup(clockedIn);

    expect(clock(wrapper).attributes('aria-label')).toContain('(01:00)');
    vi.advanceTimersByTime(60000);
    await nextTick();

    expect(clock(wrapper).attributes('aria-label')).toContain('(01:01)');
  });

  it('shows the error when loading fails', async () => {
    const error = new Error('load failed');
    vi.mocked(getAttendanceToday).mockRejectedValue(error);
    const wrapper = mountWithPlugins(AttendanceClockButton);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.find('.attendance-clock').exists()).toBe(false);
  });

  it('shows the error when clocking fails', async () => {
    const error = new Error('toggle failed');
    vi.mocked(toggleAttendance).mockRejectedValue(error);
    const wrapper = await setup(notStarted);
    await clock(wrapper).trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(clock(wrapper).classes()).toContain('is-idle');
    expect((clock(wrapper).element as HTMLButtonElement).disabled).toBe(false);
  });
});
