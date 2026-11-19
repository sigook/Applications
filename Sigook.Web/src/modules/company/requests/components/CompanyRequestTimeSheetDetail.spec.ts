import { describe, expect, it } from 'vitest';
import CompanyRequestTimeSheetDetail from '@/modules/company/requests/components/CompanyRequestTimeSheetDetail.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { PunchCardEditableDay } from '@/shared/punch-card/types';

function setup(editableDay: PunchCardEditableDay) {
  return mountWithPlugins(CompanyRequestTimeSheetDetail, { props: { editableDay } });
}

function report(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.container-worker-report .pl-2').map((cell) => cell.findAll('span')[1].text());
}

function values(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.columns .column').map((c) => c.find('span').text());
}

describe('CompanyRequestTimeSheetDetail', () => {
  it('shows the reported and approved hours of the day', () => {
    const wrapper = setup({
      id: 'ts-1',
      day: '2026-11-18T00:00:00',
      clockIn: '2026-11-18T08:02:00',
      clockOut: '2026-11-18T16:31:00',
      totalHours: 8.5,
      totalHoursApproved: 8,
      missingHours: '00:30:00',
      missingHoursOvertime: '00:00:00',
      comment: 'Left early',
    });

    expect(wrapper.find('h2').text()).toBe('18-Nov-2026');
    expect(report(wrapper)).toEqual(['08:02', '16:31', '8.50']);
    expect(values(wrapper)).toEqual(['8', '00:30:00', '00:00:00', 'Left early']);
  });

  it('marks what the worker did not report', () => {
    const wrapper = setup({ id: null, day: '2026-11-18T00:00:00', clockIn: null, clockOut: null, totalHoursApproved: null });

    expect(report(wrapper)).toEqual(['No reported', 'No reported', 'No reported']);
  });
});
