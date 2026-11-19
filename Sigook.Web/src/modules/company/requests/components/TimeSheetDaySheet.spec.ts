import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import TimeSheetDaySheet from '@/modules/company/requests/components/TimeSheetDaySheet.vue';
import { maximumHoursPerDay } from '@/shared/constants/catalog';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PunchCardEditableDay } from '@/shared/punch-card/types';

vi.mock('@/shared/ui/SheetPanel.vue', () => ({
  default: {
    name: 'SheetPanel',
    props: { modelValue: Boolean, title: String },
    emits: ['update:modelValue'],
    template: '<div class="sheet"><h3 class="sheet-title">{{ title }}</h3><slot /><footer><slot name="foot" /></footer></div>',
  },
}));

const day: PunchCardEditableDay = {
  id: 'ts-1',
  day: '2026-11-18T00:00:00',
  timeIn: '2026-11-18T08:00:00',
  clockIn: '2026-11-18T08:02:00',
  clockOut: '2026-11-18T16:31:00',
  totalHours: 8.5,
  totalHoursApproved: 8,
  canUpdate: true,
  hoursApprovedToDate: new Date(2026, 10, 18, 8, 0),
  missinghoursToDate: new Date(2026, 10, 18, 0, 30),
  missingHoursOvertimeToDate: new Date(2026, 10, 18, 1, 15),
};

const newDay: PunchCardEditableDay = { id: null, day: '2026-11-19T00:00:00', totalHoursApproved: null, clockIn: null, clockOut: null };

function setup(editableDay: PunchCardEditableDay | null = day) {
  return mountWithPlugins(TimeSheetDaySheet, { props: { modelValue: true, editableDay } });
}

function report(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.container-worker-report .pl-2').map((cell) => cell.findAll('span')[1].text());
}

function button(wrapper: ReturnType<typeof setup>, text: string) {
  const found = wrapper.findAll('footer button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} button not found`);
  return found;
}

async function typeHours(wrapper: ReturnType<typeof setup>, hours: number | null) {
  wrapper.findComponent({ name: 'BNumberinput' }).vm.$emit('update:modelValue', hours);
  await nextTick();
}

describe('TimeSheetDaySheet', () => {
  it('shows the worker report of an existing day', () => {
    const wrapper = setup();

    expect(wrapper.find('.sheet-title').text()).toBe('Wednesday, November 18');
    expect(report(wrapper)).toEqual(['08:02', '16:31', '8.50']);
    expect(wrapper.findAllComponents({ name: 'BTimepicker' })).toHaveLength(3);
    expect(wrapper.find('.message').exists()).toBe(false);
  });

  it('saves the approved and missing hours', async () => {
    const wrapper = setup();
    await button(wrapper, 'Save').trigger('click');

    expect(wrapper.emitted('save')).toEqual([[{
      hours: '08:00:00',
      timeIn: '2026-11-18T00:00:00',
      missingHours: '00:30:00',
      missingHoursOvertime: '01:15:00',
    }]]);
  });

  it('deletes an updatable day', async () => {
    const wrapper = setup();
    await button(wrapper, 'Delete').trigger('click');

    expect(wrapper.emitted('delete')).toHaveLength(1);
  });

  it('hides delete when the day cannot be updated', () => {
    const wrapper = setup({ ...day, canUpdate: false, clockOut: null });

    expect(wrapper.findAll('footer button').map((b) => b.text())).toEqual(['Save']);
    expect(report(wrapper)).toEqual(['08:02', 'No reported', 'No reported']);
    expect(wrapper.find('.message.is-info').exists()).toBe(true);
  });

  it('asks only for hours on a new day and posts them', async () => {
    const wrapper = setup(newDay);

    expect(wrapper.find('.container-worker-report').exists()).toBe(false);
    expect(wrapper.findAll('footer button').map((b) => b.text())).toEqual(['Save']);
    await typeHours(wrapper, 7.25);
    await button(wrapper, 'Save').trigger('click');

    expect(wrapper.emitted('post')).toEqual([[7.25]]);
    expect(wrapper.find('.help.is-danger').exists()).toBe(false);
  });

  it.each([
    [null, `Hours must be between 0 and ${maximumHoursPerDay}`],
    [-1, `Hours must be between 0 and ${maximumHoursPerDay}`],
    [maximumHoursPerDay + 1, `Hours must be between 0 and ${maximumHoursPerDay}`],
    [1.234, 'Max 2 decimal places'],
  ])('rejects %s hours', async (hours, message) => {
    const wrapper = setup(newDay);
    await typeHours(wrapper, hours);
    await button(wrapper, 'Save').trigger('click');

    expect(wrapper.find('.help.is-danger').text()).toBe(message);
    expect(wrapper.emitted('post')).toBeUndefined();
  });

  it('clears the hours and error when another day opens', async () => {
    const wrapper = setup(newDay);
    await button(wrapper, 'Save').trigger('click');
    await wrapper.setProps({ editableDay: { ...newDay, day: '2026-11-20T00:00:00' } });

    expect(wrapper.find('.sheet-title').text()).toBe('Friday, November 20');
    expect(wrapper.find('.help.is-danger').exists()).toBe(false);
  });

  it('renders nothing without a day and relays closing', async () => {
    const wrapper = setup(null);

    expect(wrapper.find('.sheet-title').text()).toBe('');
    await button(wrapper, 'Save').trigger('click');
    expect(wrapper.emitted('save')).toBeUndefined();
    expect(wrapper.emitted('post')).toBeUndefined();

    wrapper.findComponent({ name: 'SheetPanel' }).vm.$emit('update:modelValue', false);
    expect(wrapper.emitted('update:modelValue')).toEqual([[false]]);
  });
});
