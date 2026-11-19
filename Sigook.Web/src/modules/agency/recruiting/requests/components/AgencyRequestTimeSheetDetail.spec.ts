import { describe, expect, it } from 'vitest';
import AgencyRequestTimeSheetDetail from '@/modules/agency/recruiting/requests/components/AgencyRequestTimeSheetDetail.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PunchCardEditableDay } from '@/shared/punch-card/types';

const reported: PunchCardEditableDay = {
  id: 'ts-1',
  day: '2026-11-18T00:00:00',
  clockIn: '2026-11-18T08:05:00',
  clockOut: '2026-11-18T16:30:00',
  totalHours: 8.25,
  totalHoursApproved: 8,
  missingHours: '01:00:00',
  missingHoursOvertime: '00:30:00',
  missingRateWorker: 18,
  missingRateAgency: 25,
  deductionsOthers: 10,
  deductionsOthersDescription: 'Boots',
  bonusOrOthers: 20,
  bonusOrOthersDescription: 'Referral',
  reimbursements: 5,
  reimbursementsDescription: 'Parking',
  comment: 'Late start',
};

function setup(editableDay: PunchCardEditableDay) {
  return mountWithPlugins(AgencyRequestTimeSheetDetail, { props: { editableDay } });
}

function worker(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.container-worker-report .pl-2').map((c) => c.findAll('span')[1].text());
}

function fields(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.columns .column').map((c) => [c.find('label').text(), c.find('span').text()]);
}

describe('AgencyRequestTimeSheetDetail', () => {
  it('shows the worker report and approved values', () => {
    const wrapper = setup(reported);

    expect(wrapper.find('h2').text()).toBe('18-Nov-2026');
    expect(worker(wrapper)).toEqual(['08:05', '16:30', '8.25']);
    expect(fields(wrapper)).toEqual([
      ['Hours Approved', '8'],
      ['Missing Hours', '01:00:00'],
      ['Missing Hours Over time', '00:30:00'],
      ['Missing Worker Rate', '18'],
      ['Missing Agency Rate', '25'],
      ['Deductions', '10'],
      ['Description', 'Boots'],
      ['Bonus or others', '20'],
      ['Description', 'Referral'],
      ['Reimbursements', '5'],
      ['Description', 'Parking'],
      ['Comments', 'Late start'],
    ]);
  });

  it('marks missing clock data as not reported', () => {
    const wrapper = setup({ id: null, day: new Date(2026, 10, 18), totalHoursApproved: null, clockIn: null, clockOut: null, totalHours: 0 });

    expect(worker(wrapper)).toEqual(['No reported', 'No reported', 'No reported']);
  });
});
