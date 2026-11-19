import { describe, expect, it } from 'vitest';
import ShiftsForm from '@/shared/request-detail/ShiftsForm.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestShiftModel } from '@/shared/request-detail/types';

const defaultShift: RequestShiftModel = {
  sunday: null,
  monday: true,
  tuesday: true,
  wednesday: true,
  thursday: true,
  friday: true,
  saturday: null,
  sundayStart: '00:00:00',
  sundayFinish: '00:00:00',
  mondayStart: '08:00:00',
  mondayFinish: '17:00:00',
  tuesdayStart: '08:00:00',
  tuesdayFinish: '17:00:00',
  wednesdayStart: '08:00:00',
  wednesdayFinish: '17:00:00',
  thursdayStart: '08:00:00',
  thursdayFinish: '17:00:00',
  fridayStart: '08:00:00',
  fridayFinish: '17:00:00',
  saturdayStart: '00:00:00',
  saturdayFinish: '00:00:00',
  comments: undefined,
};

const nightShift: RequestShiftModel = {
  ...defaultShift,
  monday: true,
  tuesday: false,
  wednesday: false,
  thursday: false,
  friday: false,
  saturday: true,
  mondayStart: '22:00:00',
  mondayFinish: '06:00:00',
  saturdayStart: '20:00:00',
  saturdayFinish: '04:30:00',
  comments: 'Night crew',
};

function setup(currentShift?: RequestShiftModel | null, isUpdate?: boolean) {
  return mountWithPlugins(ShiftsForm, { props: { currentShift, isUpdate } });
}

function listRows(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.shifts-list .shift-row').filter((r) => r.find('input[type="checkbox"]').exists());
}

function lastShift(wrapper: ReturnType<typeof setup>): RequestShiftModel {
  const events = wrapper.emitted<[RequestShiftModel]>('updateModel');
  const last = events?.at(-1);
  if (!last) throw new Error('No updateModel emitted');
  return last[0];
}

describe('ShiftsForm', () => {
  it('emits the default Monday to Friday 8 to 17 shift for a new record', () => {
    const wrapper = setup();

    expect(wrapper.emitted('updateModel')).toHaveLength(1);
    expect(lastShift(wrapper)).toEqual(defaultShift);
    expect(listRows(wrapper).map((r) => r.find('.shift-off').exists())).toEqual([true, false, false, false, false, false, true]);
    expect(listRows(wrapper)[1].findAllComponents({ name: 'BTimepicker' })).toHaveLength(2);
  });

  it.each([undefined, null])('emits the default shift when updating without a current shift (%s)', (currentShift) => {
    const wrapper = setup(currentShift, true);

    expect(wrapper.emitted('updateModel')).toHaveLength(1);
    expect(lastShift(wrapper)).toEqual(defaultShift);
  });

  it('loads the current shift without emitting when updating', () => {
    const wrapper = setup(nightShift, true);
    const rows = listRows(wrapper);

    expect(wrapper.emitted('updateModel')).toBeUndefined();
    expect(rows.map((r) => (r.find('input[type="checkbox"]').element as HTMLInputElement).checked)).toEqual([
      false, true, false, false, false, false, true,
    ]);
    expect((wrapper.find('textarea').element as HTMLTextAreaElement).value).toBe('Night crew');
  });

  it('keeps the loaded times when the user edits', async () => {
    const wrapper = setup(nightShift, true);
    await wrapper.find('textarea').setValue('Night crew, bring boots');

    expect(lastShift(wrapper)).toEqual({ ...nightShift, comments: 'Night crew, bring boots' });
  });

  it('emits the new days when one is toggled', async () => {
    const wrapper = setup();
    await listRows(wrapper)[6].find('input[type="checkbox"]').setValue(true);

    expect(lastShift(wrapper)).toMatchObject({ saturday: true, saturdayStart: '00:00:00' });
    expect(listRows(wrapper)[6].find('.shift-off').exists()).toBe(false);
  });

  it('applies the default times to the checked days and clears the rest', async () => {
    const wrapper = setup(nightShift, true);
    await wrapper.find('.shift-apply-button').trigger('click');

    expect(lastShift(wrapper)).toMatchObject({
      mondayStart: '08:00:00',
      mondayFinish: '17:00:00',
      saturdayStart: '08:00:00',
      saturdayFinish: '17:00:00',
      tuesdayStart: '00:00:00',
      tuesdayFinish: '00:00:00',
    });
  });

  it('applies only the start time from the From menu', async () => {
    const wrapper = setup(nightShift, true);
    const from = wrapper.findAll('.grid-label button')[0];
    await from.trigger('click');
    await wrapper.find('.grid-apply-all button.is-primary').trigger('click');

    expect(lastShift(wrapper)).toMatchObject({ mondayStart: '08:00:00', mondayFinish: '06:00:00' });
  });

  it('applies only the finish time from the To menu', async () => {
    const wrapper = setup(nightShift, true);
    const to = wrapper.findAll('.grid-label button')[1];
    await to.trigger('click');
    await wrapper.find('.grid-apply-all button.is-primary').trigger('click');

    expect(lastShift(wrapper)).toMatchObject({ mondayStart: '22:00:00', mondayFinish: '17:00:00' });
  });

  it('reloads when the current shift changes', async () => {
    const wrapper = setup(defaultShift, true);
    await wrapper.setProps({ currentShift: nightShift });

    expect((wrapper.find('textarea').element as HTMLTextAreaElement).value).toBe('Night crew');
  });
});
