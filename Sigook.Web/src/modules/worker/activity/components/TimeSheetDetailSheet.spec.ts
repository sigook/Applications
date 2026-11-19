import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import TimeSheetDetailSheet from '@/modules/worker/activity/components/TimeSheetDetailSheet.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { WorkerTimeSheetStatus, type WorkerWeekTimeSheet } from '@/modules/worker/activity/types';

const entry: WorkerWeekTimeSheet = {
  id: 't1',
  requestId: 'r1',
  date: '2026-06-01T00:00:00',
  clientName: 'Acme',
  jobTitle: 'Picker',
  clockIn: '2026-06-01T07:00:00',
  clockOut: '2026-06-01T15:30:00',
  durationBreak: '00:30',
  breakIsPaid: true,
  regularHours: 8,
  overtimeHours: 0.5,
  holidayHours: 0,
  totalHours: 8.5,
  status: WorkerTimeSheetStatus.Pending,
  missingHours: '00:00',
  missingHoursOvertime: '00:00',
  bonusOrOthers: 0,
  deductionsOthers: 0,
  reimbursements: 0,
};

function setup(value: WorkerWeekTimeSheet | null, modelValue = true) {
  return mountWithPlugins(TimeSheetDetailSheet, { props: { modelValue, entry: value } });
}

function sections() {
  return Array.from(document.body.querySelectorAll('.detail-section')).map((section) => ({
    title: section.querySelector('.detail-section__title')?.textContent,
    rows: Array.from(section.querySelectorAll('.detail-row')).map((row) => [
      row.querySelector('.detail-row__label')?.textContent,
      row.querySelector('.detail-row__value')?.textContent,
      row.querySelector('.detail-row__hint')?.textContent ?? null,
    ]),
  }));
}

beforeEach(() => {
  vi.useFakeTimers({ now: new Date('2026-06-01T18:00:00'), toFake: ['Date'] });
});

afterEach(() => {
  vi.useRealTimers();
});

describe('TimeSheetDetailSheet', () => {
  it('shows the day hours without adjustments', () => {
    setup(entry);
    expect(document.body.querySelector('.sheet-panel__title')?.textContent.trim()).toBe('Mon, Jun 1, 2026');
    expect(document.body.querySelector('.detail-head')?.textContent).toContain('Acme · Picker');
    expect(document.body.querySelector('.detail-head .tag')?.textContent).toBe('Pending');
    expect(sections()).toEqual([{
      title: 'Hours',
      rows: [
        ['In – Out', '07:00 – 15:30', null],
        ['Break', '30 min (paid)', null],
        ['Regular', '8.0', null],
        ['Overtime', '0.5', null],
        ['Total', '8.5', null],
      ],
    }]);
  });

  it('lists every adjustment with its description', () => {
    setup({
      ...entry,
      missingHours: '01:30',
      missingHoursOvertime: '00:45',
      bonusOrOthers: 50,
      bonusOrOthersDescription: 'Safety bonus',
      deductionsOthers: 20,
      deductionsOthersDescription: 'Boots',
      reimbursements: 15,
      reimbursementsDescription: 'Parking',
    });
    expect(sections()[1]).toEqual({
      title: 'Adjustments',
      rows: [
        ['Missing hours', '1.5 h', null],
        ['Missing overtime', '0.8 h', null],
        ['Bonus / others', '$50.00', 'Safety bonus'],
        ['Deductions', '−$20.00', 'Boots'],
        ['Reimbursements', '$15.00', 'Parking'],
      ],
    });
  });

  it('renders nothing for a missing entry', () => {
    setup(null);
    expect(document.body.querySelector('.sheet-panel__title')?.textContent.trim()).toBe('');
    expect(sections()).toEqual([]);
  });

  it('closes from the footer', async () => {
    const wrapper = setup(entry);
    const close = Array.from(document.body.querySelectorAll<HTMLButtonElement>('.sheet-panel__foot button')).find((b) => b.textContent.trim() === 'Close');
    close?.click();
    await wrapper.vm.$nextTick();
    expect(wrapper.emitted('update:modelValue')).toEqual([[false]]);
  });

  it('forwards the panel close', async () => {
    const wrapper = setup(entry);
    document.body.querySelector<HTMLElement>('.sheet-panel__overlay')?.click();
    await wrapper.vm.$nextTick();
    expect(wrapper.emitted('update:modelValue')).toEqual([[false]]);
  });
});
