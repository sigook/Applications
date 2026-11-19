import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import PunchCard from '@/modules/worker/requests/components/PunchCard.vue';
import { getPunchStatus, workerRegisterTime } from '@/modules/worker/requests/api';
import { useWorkerStore } from '@/modules/worker/store';
import { getErrorText, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { ClockType } from '@/shared/constants/enums';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PunchStatus, WorkerRequestDetail } from '@/modules/worker/requests/types';

type Success = (position: GeolocationPosition) => void;
type Failure = () => void;

const { watchPosition } = vi.hoisted(() => ({ watchPosition: vi.fn<(success: Success, failure: Failure) => number>() }));

vi.mock('@/app/stores/app', () => ({ useAppStore: () => ({ getCurrentDate: () => Promise.resolve(new Date()) }) }));
vi.mock('@/modules/worker/requests/api', () => ({ getPunchStatus: vi.fn(), workerRegisterTime: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ getErrorText: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/punch-card/DataEntryTerms.vue', () => ({ default: { name: 'DataEntryTerms', template: '<div />' } }));

let pinia: Pinia;

const site = { latitude: 43.65, longitude: -79.38 };
const metersPerDegree = 6376500 * Math.PI / 180;

const request: WorkerRequestDetail = {
  id: 'r1',
  numberId: 300,
  location: '1 King St',
  startAt: '2026-06-01T12:00:00',
  finishAt: '2026-06-30T12:00:00',
  punchCheckpoint: { ...site, radiusMeters: 100 },
};

function at(metersNorth: number, accuracy = 10): GeolocationPosition {
  return {
    coords: { latitude: site.latitude + metersNorth / metersPerDegree, longitude: site.longitude, accuracy },
  } as GeolocationPosition;
}

function punchStatus(overrides: Partial<PunchStatus> = {}): PunchStatus {
  return { clockType: ClockType.ClockIn, dayHours: 0, weekHours: 12.5, ...overrides };
}

async function setup({ position = at(0) as GeolocationPosition | null, value = request as WorkerRequestDetail | undefined } = {}) {
  watchPosition.mockImplementation((success, failure) => {
    if (position) success(position);
    else failure();
    return 1;
  });
  const wrapper = mountWithPlugins(PunchCard, { props: { requestId: 'r1', request: value }, global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function text(wrapper: Wrapper, selector: string) {
  return wrapper.find(selector).text();
}

function cta(wrapper: Wrapper) {
  return wrapper.find('.punch-cta');
}

function button(wrapper: Wrapper, label: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === label);
  if (!found) throw new Error(`${label} not found`);
  return found;
}

async function punch(wrapper: Wrapper) {
  await cta(wrapper).trigger('click');
  await button(wrapper, 'Confirm').trigger('click');
  await flushPromises();
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  vi.useFakeTimers({ now: new Date('2026-06-03T10:00:00'), toFake: ['Date', 'setInterval', 'clearInterval'] });
  vi.stubGlobal('navigator', { userAgent: 'test', geolocation: { watchPosition } });
  vi.mocked(getPunchStatus).mockResolvedValue(punchStatus());
});

afterEach(() => {
  vi.useRealTimers();
});

describe('PunchCard', () => {
  it('is ready to clock in at the check point', async () => {
    const wrapper = await setup();
    expect(getPunchStatus).toHaveBeenCalledWith('r1', site.latitude, site.longitude);
    expect(text(wrapper, '.punch-card__head-date')).toBe('Wed 03 June, 2026');
    expect(text(wrapper, '.punch-pill')).toBe('Not started');
    expect(text(wrapper, '.punch-clock__value')).toBe('10:00:00');
    expect(text(wrapper, '.punch-clock__label')).toBe('Request runs Mon 01 June, 2026 - Tue 30 June, 2026');
    expect(wrapper.find('.punch-geo').classes()).toContain('is-ok');
    expect(text(wrapper, '.punch-geo__title')).toBe('At the check point');
    expect(text(wrapper, '.punch-geo__coords')).toBe('43.650000, -79.380000 (±10 m)');
    expect(cta(wrapper).text()).toBe('Clock in');
    expect(cta(wrapper).attributes('disabled')).toBeUndefined();
    expect(wrapper.find('.punch-hint').exists()).toBe(false);
    expect(text(wrapper, '.punch-foot')).toBe('This week: 12.50 h');
    expect(wrapper.findAll('.punch-stamps__value').map((v) => v.text())).toEqual(['--:--', '--:--']);
  });

  it('ticks the clock every second', async () => {
    const wrapper = await setup();
    await vi.advanceTimersByTimeAsync(2000);
    expect(text(wrapper, '.punch-clock__value')).toBe('10:00:02');
  });

  it('clocks in after confirming', async () => {
    vi.mocked(workerRegisterTime).mockResolvedValue();
    const wrapper = await setup();
    await cta(wrapper).trigger('click');
    expect(text(wrapper, '.punch-confirm__question')).toBe('Are you sure you want to clock in?');
    vi.mocked(getPunchStatus).mockResolvedValue(punchStatus({ clockType: ClockType.ClockOut, clockIn: '2026-06-03T09:00:00' }));
    await button(wrapper, 'Confirm').trigger('click');
    await flushPromises();

    expect(workerRegisterTime).toHaveBeenCalledWith('r1', site.latitude, site.longitude);
    expect(showAlertSuccess).toHaveBeenCalledWith('Clocked in successfully!');
    expect(wrapper.emitted('refreshRequest')).toHaveLength(1);
    expect(text(wrapper, '.punch-pill')).toBe('Working');
    expect(text(wrapper, '.punch-clock__value')).toBe('01:00:00');
    expect(text(wrapper, '.punch-clock__label')).toBe('Since 09:00');
    expect(cta(wrapper).text()).toBe('Clock out');
  });

  it('cancels the confirmation', async () => {
    const wrapper = await setup();
    await cta(wrapper).trigger('click');
    await button(wrapper, 'Cancel').trigger('click');
    expect(wrapper.find('.punch-confirm').exists()).toBe(false);
    expect(workerRegisterTime).not.toHaveBeenCalled();
  });

  it('clocks out while working', async () => {
    vi.mocked(getPunchStatus).mockResolvedValue(punchStatus({ clockType: ClockType.ClockOut, clockIn: '2026-06-03T09:00:00' }));
    vi.mocked(workerRegisterTime).mockResolvedValue();
    const wrapper = await setup();
    await cta(wrapper).trigger('click');
    expect(text(wrapper, '.punch-confirm__question')).toBe('Are you sure you want to clock out?');
    vi.mocked(getPunchStatus).mockResolvedValue(punchStatus({ clockType: ClockType.None, clockIn: '2026-06-03T09:00:00', clockOut: '2026-06-03T17:30:00', dayHours: 8.5 }));
    await button(wrapper, 'Confirm').trigger('click');
    await flushPromises();

    expect(showAlertSuccess).toHaveBeenCalledWith('Clocked out successfully!');
    expect(text(wrapper, '.punch-pill')).toBe('Shift completed');
    expect(text(wrapper, '.punch-clock__value')).toBe('8.50 h');
    expect(text(wrapper, '.punch-clock__label')).toBe('Sent to your agency for approval');
    expect(wrapper.findAll('.punch-stamps__value').map((v) => v.text())).toEqual(['09:00', '17:30']);
    expect(cta(wrapper).attributes('disabled')).toBeDefined();
  });

  it('shows the rejection when the server says the worker is too far', async () => {
    useWorkerStore().setWorkerProfile({ numberId: 1001, firstName: 'John', lastName: 'Smith' });
    vi.mocked(workerRegisterTime).mockRejectedValue({ data: 'too far' });
    vi.mocked(getErrorText).mockResolvedValue('You are too far from the workplace');
    const wrapper = await setup();
    await punch(wrapper);

    expect(getErrorText).toHaveBeenCalledWith('too far');
    expect(text(wrapper, '.punch-rejected__message')).toBe('You are too far from the workplace');
    expect(text(wrapper, '.punch-rejected__ref')).toBe('Worker #1001 · John Smith · Request #300 · Jun 03, 2026 10:00 AM');
    expect(showAlertError).not.toHaveBeenCalled();

    await wrapper.find('.punch-rejected__close').trigger('click');
    expect(wrapper.find('.punch-rejected').exists()).toBe(false);
  });

  it('reports other punch errors', async () => {
    vi.mocked(workerRegisterTime).mockRejectedValue({ data: 'Shift closed' });
    vi.mocked(getErrorText).mockResolvedValue('Shift closed');
    const wrapper = await setup();
    await punch(wrapper);
    expect(showAlertError).toHaveBeenCalledWith('Shift closed');
    expect(wrapper.find('.punch-rejected').exists()).toBe(false);
  });

  it('warns near the edge of the check point', async () => {
    const wrapper = await setup({ position: at(50, 60) });
    expect(wrapper.find('.punch-geo').classes()).toContain('is-edge');
    expect(text(wrapper, '.punch-geo__title')).toBe('About 50 m from the check point (GPS ±60 m)');
    expect(cta(wrapper).attributes('disabled')).toBeUndefined();
  });

  it('lets a worker with poor GPS try from a bit too far', async () => {
    const wrapper = await setup({ position: at(150, 80) });
    expect(wrapper.find('.punch-geo').classes()).toContain('is-far');
    expect(text(wrapper, '.punch-geo__title')).toBe('About 150 m north of the check point');
    expect(text(wrapper, '.punch-geo__sub')).toBe('Get closer before you clock in.');
    expect(wrapper.find('.punch-geo__ref').exists()).toBe(false);
    expect(cta(wrapper).attributes('disabled')).toBeUndefined();
  });

  it('blocks punching when clearly out of range', async () => {
    vi.mocked(getPunchStatus).mockResolvedValue(punchStatus({ clockType: ClockType.ClockOut, clockIn: '2026-06-03T09:00:00' }));
    const wrapper = await setup({ position: at(2500) });
    expect(text(wrapper, '.punch-geo__title')).toBe('About 2.5 km north of the check point');
    expect(text(wrapper, '.punch-geo__sub')).toBe('Get closer to clock out. Already on site? Contact your agency.');
    expect(text(wrapper, '.punch-geo__ref')).toBe('Worker: unknown · Request #300');
    expect(cta(wrapper).attributes('disabled')).toBeDefined();
  });

  it('shares the location when the request has no check point', async () => {
    const wrapper = await setup({ value: { ...request, punchCheckpoint: null } });
    expect(wrapper.find('.punch-geo').classes()).toContain('is-neutral');
    expect(text(wrapper, '.punch-geo__sub')).toBe('1 King St · your location is checked when you punch.');
  });

  it('asks for the location when it is turned off', async () => {
    const wrapper = await setup({ position: null, value: { id: 'r1' } });
    expect(getPunchStatus).not.toHaveBeenCalled();
    expect(wrapper.find('.punch-geo').classes()).toContain('is-denied');
    expect(text(wrapper, '.punch-geo__title')).toBe('Location is turned off');
    expect(text(wrapper, '.punch-pill')).toBe('Not started');
    expect(text(wrapper, '.punch-clock__label')).toBe('Current time');
    expect(text(wrapper, '.punch-hint')).toBe('Turn on location to clock in or out.');
    expect(cta(wrapper).attributes('disabled')).toBeDefined();
  });

  it('says when there is nothing to punch', async () => {
    vi.mocked(getPunchStatus).mockResolvedValue(punchStatus({ clockType: ClockType.None }));
    const wrapper = await setup();
    expect(text(wrapper, '.punch-pill')).toBe('Nothing to punch');
    expect(text(wrapper, '.punch-clock__value')).toBe('--:--');
    expect(text(wrapper, '.punch-clock__label')).toBe('No shift to punch on this day');
    expect(cta(wrapper).text()).toBe('Unavailable');
    expect(text(wrapper, '.punch-hint')).toBe('There is nothing to punch for this day.');
  });

  it('retries after the status could not be loaded', async () => {
    vi.mocked(getPunchStatus).mockRejectedValue('offline');
    const wrapper = await setup();
    expect(text(wrapper, '.punch-pill')).toBe('Status unavailable');
    expect(text(wrapper, '.punch-clock__label')).toBe('Could not check your shift status');
    expect(text(wrapper, '.punch-hint')).toBe('We could not reach the server. Check your connection and retry.');
    expect(cta(wrapper).text()).toBe('Retry');

    vi.mocked(getPunchStatus).mockResolvedValue(punchStatus());
    await cta(wrapper).trigger('click');
    await flushPromises();
    expect(wrapper.emitted('refreshRequest')).toHaveLength(1);
    expect(text(wrapper, '.punch-pill')).toBe('Not started');
  });

  it('refreshes the request when the page becomes visible', async () => {
    const wrapper = await setup();
    Object.defineProperty(document, 'visibilityState', { value: 'hidden', configurable: true });
    document.dispatchEvent(new Event('visibilitychange'));
    expect(wrapper.emitted('refreshRequest')).toBeUndefined();
    Object.defineProperty(document, 'visibilityState', { value: 'visible', configurable: true });
    document.dispatchEvent(new Event('visibilitychange'));
    expect(wrapper.emitted('refreshRequest')).toHaveLength(1);
    Reflect.deleteProperty(document, 'visibilityState');
  });
});
