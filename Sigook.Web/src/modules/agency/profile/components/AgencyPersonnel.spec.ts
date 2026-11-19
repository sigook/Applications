import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, reactive } from 'vue';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import AgencyPersonnel from '@/modules/agency/profile/components/AgencyPersonnel.vue';
import { deleteAgencyPersonnel, getAgencyPersonnel, getAttendancesTodayForUsers } from '@/modules/agency/profile/api';
import { useAgencyStore } from '@/modules/agency/store';
import { showAlertConfirm, showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { AttendanceStatus, type AgencyPersonnelListItem } from '@/modules/agency/profile/types';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });

const { stub } = vi.hoisted(() => ({
  stub: (name: string, props: string[], emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/composables/useBreakpoint', () => ({ useBreakpoint: () => ({ isTouch: computed(() => false) }) }));
vi.mock('@/modules/agency/profile/api', () => ({
  getAgencyPersonnel: vi.fn(),
  deleteAgencyPersonnel: vi.fn(),
  getAttendancesTodayForUsers: vi.fn(),
}));
vi.mock('@/modules/agency/profile/components/AgencyPersonnelModal.vue', () => ({
  default: stub('PersonnelForm', ['personnel'], ['updateUsers']),
}));
vi.mock('@/modules/agency/profile/components/AttendanceReportModal.vue', () => ({
  default: stub('AttendanceReportModal', ['users', 'agencyIds']),
}));

const ana: AgencyPersonnelListItem = {
  id: 'p-1', userId: 'u-1', name: 'Ana Smith', email: 'ana@sigook.com', role: 'recruiting', agencyIds: ['a1'], agencies: ['Covenant'],
};
const bob: AgencyPersonnelListItem = {
  id: 'p-2', userId: 'u-2', name: 'Bob Lee', email: 'bob@sigook.com', role: 'custom', agencyIds: ['a1', 'a2'], agencies: ['Covenant', 'Sigook'],
};
const carl: AgencyPersonnelListItem = {
  id: 'p-3', userId: 'u-3', name: 'Carl Diaz', email: null, role: 'admin', agencyIds: ['a2'], agencies: ['Sigook'],
};

let pinia: Pinia;

async function setup() {
  const wrapper = mountWithPlugins(AgencyPersonnel, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function headers(wrapper: Wrapper) {
  return wrapper.findAll('thead th').map((th) => th.text()).filter(Boolean);
}

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').map((r) => r.findAll('td').map((c) => c.text()));
}

function button(wrapper: Wrapper, text: string) {
  return wrapper.findAll('button').find((b) => b.text() === text);
}

function rowButtons(wrapper: Wrapper, index: number) {
  return wrapper.findAll('tbody tr')[index].findAll('button.is-rounded');
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  useAgencyStore().setAgencies([{ id: 'a1', name: 'Covenant' }]);
  securityStore.userRoles = ['admin'];
  vi.mocked(getAgencyPersonnel).mockResolvedValue([ana, bob, carl]);
  vi.mocked(getAttendancesTodayForUsers).mockResolvedValue([
    { userId: 'u-1', status: AttendanceStatus.ClockedIn, clockIn: '2026-11-19T08:05:00', clockOut: null, workedHours: null },
    { userId: 'u-2', status: AttendanceStatus.ClockedOut, clockIn: '2026-11-19T08:00:00', clockOut: '2026-11-19T17:00:00', workedHours: 8 },
  ]);
});

describe('AgencyPersonnel', () => {
  it('lists the users with their role and today attendance for an admin', async () => {
    const wrapper = await setup();

    expect(getAgencyPersonnel).toHaveBeenCalledWith(undefined);
    expect(getAttendancesTodayForUsers).toHaveBeenCalledWith(undefined);
    expect(wrapper.emitted('loaded')).toEqual([[3]]);
    expect(headers(wrapper)).toEqual(['Name', 'Email', 'Role', 'Today']);
    expect(rows(wrapper).map((r) => r.slice(0, 4))).toEqual([
      ['Ana Smith', 'ana@sigook.com', 'Recruiting', 'In since 08:05'],
      ['Bob Lee', 'bob@sigook.com', 'custom', 'Closed · 8.00 h'],
      ['Carl Diaz', '', 'Admin', 'Not clocked in'],
    ]);
    expect(button(wrapper, 'Add')?.exists()).toBe(true);
    expect(button(wrapper, 'Attendance report')?.exists()).toBe(true);
  });

  it('hides the admin tools for other roles', async () => {
    securityStore.userRoles = ['recruiting'];
    const wrapper = await setup();

    expect(getAttendancesTodayForUsers).not.toHaveBeenCalled();
    expect(headers(wrapper)).toEqual(['Name', 'Email', 'Role']);
    expect(button(wrapper, 'Add')).toBeUndefined();
    expect(button(wrapper, 'Attendance report')).toBeUndefined();
  });

  it('shows the agencies column when the user has several agencies', async () => {
    useAgencyStore().setAgencies([{ id: 'a1', name: 'Covenant' }, { id: 'a2', name: 'Sigook' }]);
    const wrapper = await setup();

    expect(headers(wrapper)).toContain('Agencies');
    expect(rows(wrapper)[1]).toContain('Covenant, Sigook');
  });

  it('reloads with the selected agencies without changing the total count', async () => {
    useAgencyStore().setAgencies([{ id: 'a1', name: 'Covenant' }, { id: 'a2', name: 'Sigook' }]);
    const wrapper = await setup();
    vi.mocked(getAgencyPersonnel).mockResolvedValue([carl]);
    const filter = wrapper.findComponent({ name: 'AgencyFilter' });
    filter.vm.$emit('update:modelValue', 'a2');
    filter.vm.$emit('change');
    await flushPromises();

    expect(getAgencyPersonnel).toHaveBeenLastCalledWith('a2');
    expect(getAttendancesTodayForUsers).toHaveBeenLastCalledWith('a2');
    expect(rows(wrapper)).toHaveLength(1);
    expect(wrapper.emitted('loaded')).toEqual([[3]]);
  });

  it('opens the form empty to add and with the row to edit, reloading after save', async () => {
    const wrapper = await setup();
    await button(wrapper, 'Add')?.trigger('click');
    await nextTick();

    const form = wrapper.findComponent({ name: 'PersonnelForm' });
    expect(form.props('personnel')).toBeNull();

    await rowButtons(wrapper, 0)[0].trigger('click');
    expect(wrapper.findComponent({ name: 'PersonnelForm' }).props('personnel')).toEqual(ana);

    wrapper.findComponent({ name: 'PersonnelForm' }).vm.$emit('updateUsers');
    await flushPromises();

    expect(getAgencyPersonnel).toHaveBeenCalledTimes(2);
    expect(wrapper.findComponent({ name: 'PersonnelForm' }).props('personnel')).toBeNull();
  });

  it('opens the attendance report with the listed users', async () => {
    const wrapper = await setup();
    await button(wrapper, 'Attendance report')?.trigger('click');
    await nextTick();

    expect(wrapper.findComponent({ name: 'AttendanceReportModal' }).props()).toEqual({ users: [ana, bob, carl], agencyIds: undefined });
  });

  it('deletes a user after confirming and reloads the list', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyPersonnel).mockResolvedValue();
    const wrapper = await setup();
    await rowButtons(wrapper, 1)[1].trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure?', 'You want to delete user.');
    expect(deleteAgencyPersonnel).toHaveBeenCalledExactlyOnceWith('p-2');
    expect(getAgencyPersonnel).toHaveBeenCalledTimes(2);
  });

  it('keeps the user when the delete is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await rowButtons(wrapper, 1)[1].trigger('click');
    await flushPromises();

    expect(deleteAgencyPersonnel).not.toHaveBeenCalled();
  });

  it('shows the error when deleting fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyPersonnel).mockRejectedValue(error);
    const wrapper = await setup();
    await rowButtons(wrapper, 0)[1].trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getAgencyPersonnel).toHaveBeenCalledTimes(1);
  });

  it('shows the errors when loading fails', async () => {
    const listError = new Error('list failed');
    const attendanceError = new Error('attendance failed');
    vi.mocked(getAgencyPersonnel).mockRejectedValue(listError);
    vi.mocked(getAttendancesTodayForUsers).mockRejectedValue(attendanceError);
    const wrapper = await setup();

    expect(showAlertError).toHaveBeenCalledWith(listError);
    expect(showAlertError).toHaveBeenCalledWith(attendanceError);
    expect(wrapper.emitted('loaded')).toBeUndefined();
  });
});
