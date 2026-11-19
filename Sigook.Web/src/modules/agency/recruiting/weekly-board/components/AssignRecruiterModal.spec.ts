import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import AssignRecruiterModal from '@/modules/agency/recruiting/weekly-board/components/AssignRecruiterModal.vue';
import { getAllAgencyRequests } from '@/modules/agency/recruiting/requests/api';
import { getAgencyPersonnel } from '@/modules/agency/profile/api';
import { RequestStatus } from '@/shared/constants/enums';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';
import type { AgencyRequestListItem } from '@/modules/agency/recruiting/requests/types';
import type { AssignPreset, WeekDay } from '@/modules/agency/recruiting/weekly-board/types';

vi.mock('@/modules/agency/recruiting/requests/api', () => ({ getAllAgencyRequests: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({ getAgencyPersonnel: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const days: WeekDay[] = [
  { date: '2026-03-02', weekday: 'MON', monthShort: 'Mar', dayNum: 2 },
  { date: '2026-03-03', weekday: 'TUE', monthShort: 'Mar', dayNum: 3 },
  { date: '2026-03-04', weekday: 'WED', monthShort: 'Mar', dayNum: 4 },
];

function person(id: string, name: string, agencyIds: string[]): AgencyPersonnelListItem {
  return { id, userId: `u-${id}`, name, email: null, role: 'recruiting', agencyIds, agencies: [] };
}

const personnel = [person('r1', 'Rita Gomez', ['agency-1']), person('r2', 'Sam Lee', ['agency-2']), person('r3', 'Rob Stone', ['agency-1', 'agency-2'])];

const request = { id: 'req-1', agencyId: 'agency-2', numberId: 501, jobTitle: 'Packer', companyFullName: 'Acme Foods' } as AgencyRequestListItem;

async function setup(preset?: AssignPreset, saving = false) {
  vi.mocked(getAgencyPersonnel).mockResolvedValue(personnel);
  const wrapper = mountWithPlugins(AssignRecruiterModal, { props: { days, preset, saving } });
  await vi.mocked(getAgencyPersonnel).mock.results[0]?.value;
  await nextTick();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function taginput(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BTaginput' });
}

function recruiterOptions(wrapper: Wrapper) {
  return (taginput(wrapper).props('data') as AgencyPersonnelListItem[]).map((p) => p.id);
}

function assignButton(wrapper: Wrapper) {
  const found = wrapper.findAll('button').find((b) => b.text() === 'Assign');
  if (!found) throw new Error('Assign not found');
  return found;
}

function selectedDays(wrapper: Wrapper) {
  return wrapper.findAll('.day-cell.is-selected').map((d) => d.find('.day-num').text());
}

async function chooseRequest(wrapper: Wrapper, option: AgencyRequestListItem | null = request) {
  wrapper.findComponent({ name: 'BAutocomplete' }).vm.$emit('select', option);
  await nextTick();
}

async function chooseRecruiters(wrapper: Wrapper, ids: string[]) {
  taginput(wrapper).vm.$emit('update:modelValue', personnel.filter((p) => ids.includes(p.id)));
  await nextTick();
}

describe('AssignRecruiterModal', () => {
  it('lists the week days and every recruiter', async () => {
    const wrapper = await setup();

    expect(wrapper.findAll('.day-cell').map((d) => d.find('.day-name').text())).toEqual(['MON', 'TUE', 'WED']);
    expect(recruiterOptions(wrapper)).toEqual(['r1', 'r2', 'r3']);
    expect(selectedDays(wrapper)).toEqual([]);
    expect(assignButton(wrapper).attributes('disabled')).toBeDefined();
  });

  it('starts from the cell it was opened from', async () => {
    const wrapper = await setup({ recruiterId: 'r1', workDate: '2026-03-03' });

    expect(selectedDays(wrapper)).toEqual(['3']);
    expect(taginput(wrapper).props('modelValue')).toEqual([personnel[0]]);
  });

  it('shows the error when the recruiters fail to load', async () => {
    const error = new Error('personnel failed');
    vi.mocked(getAgencyPersonnel).mockRejectedValue(error);
    mountWithPlugins(AssignRecruiterModal, { props: { days, saving: false } });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
  });

  it('searches open and filled requests', async () => {
    vi.mocked(getAllAgencyRequests).mockResolvedValue([request]);
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BAutocomplete' }).vm.$emit('typing', '501');

    await vi.waitFor(() => { expect(getAllAgencyRequests).toHaveBeenCalledWith({ filter: '501', statuses: [RequestStatus.Open, RequestStatus.Filled] }); });
    await vi.waitFor(() => { expect(wrapper.findComponent({ name: 'BAutocomplete' }).props('data')).toEqual([request]); });
    const formatter = wrapper.findComponent({ name: 'BAutocomplete' }).props('customFormatter') as (option: AgencyRequestListItem) => string;
    expect(formatter(request)).toBe('#501 | Packer | Acme Foods');
  });

  it('limits the recruiters to the agency of the chosen request', async () => {
    const wrapper = await setup({ recruiterId: 'r1' });
    await chooseRequest(wrapper);

    expect(taginput(wrapper).props('modelValue')).toEqual([]);
    expect(recruiterOptions(wrapper)).toEqual(['r2', 'r3']);

    await chooseRequest(wrapper, null);
    expect(recruiterOptions(wrapper)).toEqual(['r1', 'r2', 'r3']);
  });

  it('filters recruiters by name and hides the selected ones', async () => {
    const wrapper = await setup();
    await chooseRecruiters(wrapper, ['r3']);
    taginput(wrapper).vm.$emit('typing', 'R');
    await nextTick();

    expect(recruiterOptions(wrapper)).toEqual(['r1']);
  });

  it('toggles the work days', async () => {
    const wrapper = await setup();
    await wrapper.findAll('.day-cell')[0].trigger('click');
    await wrapper.findAll('.day-cell')[2].trigger('click');
    expect(selectedDays(wrapper)).toEqual(['2', '4']);

    await wrapper.findAll('.day-cell')[0].trigger('click');
    expect(selectedDays(wrapper)).toEqual(['4']);
  });

  it('assigns the recruiters to the request on the chosen days', async () => {
    const wrapper = await setup({ workDate: '2026-03-02' });
    await chooseRequest(wrapper);
    await wrapper.findAll('.day-cell')[1].trigger('click');
    expect(assignButton(wrapper).attributes('disabled')).toBeDefined();

    await chooseRecruiters(wrapper, ['r2', 'r3']);
    expect(assignButton(wrapper).attributes('disabled')).toBeUndefined();
    await assignButton(wrapper).trigger('click');

    expect(wrapper.emitted('assign')).toEqual([[{ requestId: 'req-1', workDates: ['2026-03-02', '2026-03-03'], recruiterIds: ['r2', 'r3'] }]]);
  });

  it('closes on cancel', async () => {
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Cancel')?.trigger('click');

    expect(wrapper.emitted('close')).toHaveLength(1);
  });
});
