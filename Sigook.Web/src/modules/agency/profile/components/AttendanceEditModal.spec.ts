import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import AttendanceEditModal from '@/modules/agency/profile/components/AttendanceEditModal.vue';
import { updateAttendance } from '@/modules/agency/profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { UserAttendanceListItem } from '@/modules/agency/profile/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({ updateAttendance: vi.fn() }));

const punch: UserAttendanceListItem = {
  id: 'att-1',
  userId: 'u-1',
  name: 'Ana Smith',
  date: '2026-11-18T00:00:00',
  clockIn: '2026-11-18T08:00:00',
  clockOut: '2026-11-18T18:00:00',
  lunchMinutes: 60,
  workedHours: 9,
  lunchHours: 1,
  regularHours: 8,
  overtimeHours: 1,
  isWeekend: false,
  isMissingClockOut: false,
  isEdited: true,
  editedBy: 'admin@sigook.com',
  editedAt: '2026-11-18T19:00:00',
  editReason: 'Forgot to clock out',
};

function setup(overrides: Partial<UserAttendanceListItem> = {}) {
  return mountWithPlugins(AttendanceEditModal, { props: { attendance: { ...punch, ...overrides } } });
}

type Wrapper = ReturnType<typeof setup>;

function preview(wrapper: Wrapper) {
  return wrapper.findAll('.level-item .title').map((t) => t.text());
}

function timepickers(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'BTimepicker' });
}

async function setTime(wrapper: Wrapper, index: number, time: string) {
  timepickers(wrapper)[index].vm.$emit('update:modelValue', new Date(`2026-01-01T${time}`));
  await nextTick();
}

async function save(wrapper: Wrapper) {
  await wrapper.find('button.is-primary').trigger('click');
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

describe('AttendanceEditModal', () => {
  it('shows the punch and its hours', () => {
    const wrapper = setup();

    expect(wrapper.text()).toContain('Ana Smith · Wed, Nov 18, 2026');
    expect(wrapper.find('.message').exists()).toBe(false);
    expect((wrapper.find('textarea').element as HTMLTextAreaElement).value).toBe('Forgot to clock out');
    expect(preview(wrapper)).toEqual(['9.00', '1.00', '8.00', '1.00']);
  });

  it('warns about a missing clock out', () => {
    const wrapper = setup({ clockOut: null, isMissingClockOut: true });

    expect(wrapper.find('.message.is-warning').text()).toContain('Missing clock-out');
    expect(preview(wrapper)).toEqual(['0.00', '0.00', '0.00', '0.00']);
  });

  it('explains that clearing the clock out reopens the punch', async () => {
    const wrapper = setup();
    await wrapper.find('button[aria-label="Clear clock out"]').trigger('click');

    expect(wrapper.find('.message.is-info').text()).toContain('reopens the punch');
    expect(wrapper.find('button[aria-label="Clear clock out"]').attributes('disabled')).toBeDefined();
  });

  it('recalculates the hours while editing', async () => {
    const wrapper = setup();
    await setTime(wrapper, 1, '20:30:00');
    wrapper.findComponent({ name: 'BNumberinput' }).vm.$emit('update:modelValue', 30);
    await nextTick();

    expect(preview(wrapper)).toEqual(['12.00', '0.50', '8.00', '4.00']);
  });

  it('rejects a clock out before the clock in', async () => {
    const wrapper = setup();
    await setTime(wrapper, 1, '07:00:00');
    await save(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Clock out must be after clock in'); });
    expect(updateAttendance).not.toHaveBeenCalled();
  });

  it('requires a reason', async () => {
    const wrapper = setup({ editReason: null });
    await save(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Reason is required'); });
    expect(updateAttendance).not.toHaveBeenCalled();
  });

  it('saves the times on the punch day and notifies the report', async () => {
    vi.mocked(updateAttendance).mockResolvedValue();
    const wrapper = setup();
    await setTime(wrapper, 0, '07:45:00');
    await setTime(wrapper, 1, '16:15:00');
    await wrapper.find('textarea').setValue('Wrong punch');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('saved')).toHaveLength(1); });
    expect(updateAttendance).toHaveBeenCalledExactlyOnceWith('att-1', {
      clockIn: '2026-11-18T07:45:00',
      clockOut: '2026-11-18T16:15:00',
      lunchMinutes: 60,
      reason: 'Wrong punch',
    });
  });

  it('saves without a clock out to reopen the punch', async () => {
    vi.mocked(updateAttendance).mockResolvedValue();
    const wrapper = setup();
    await wrapper.find('button[aria-label="Clear clock out"]').trigger('click');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('saved')).toHaveLength(1); });
    expect(updateAttendance).toHaveBeenCalledWith('att-1', expect.objectContaining({ clockIn: '2026-11-18T08:00:00', clockOut: null }));
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(updateAttendance).mockRejectedValue(error);
    const wrapper = setup();
    await save(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('saved')).toBeUndefined();
  });
});
