import { describe, expect, it } from 'vitest';
import { ref } from 'vue';
import { currency } from '@/shared/format';
import { useWorkerRequestSummary } from '@/modules/worker/requests/useWorkerRequestSummary';
import type { WorkerRequestDetail } from '@/modules/worker/requests/types';

function detail(overrides: Partial<WorkerRequestDetail> = {}): WorkerRequestDetail {
  return {
    id: 'r1',
    jobPosition: 'Forklift Operator',
    jobTitle: 'Warehouse',
    workerRate: 20,
    workerSalary: null,
    incentive: null,
    startAt: '2026-03-01T08:00:00',
    finishAt: '2026-03-15T08:00:00',
    displayShift: '08:00 - 16:00',
    durationBreak: '00:30:00',
    breakIsPaid: false,
    holidayIsPaid: true,
    durationTerm: 'ShortTerm',
    requestStatus: 'Open',
    ...overrides,
  };
}

function valueOf(request: WorkerRequestDetail, label: string): string | undefined {
  const { fields } = useWorkerRequestSummary(ref(request));
  return fields.value.find((field) => field.label === label)?.value;
}

describe('useWorkerRequestSummary', () => {
  it('is empty without a request', () => {
    expect(useWorkerRequestSummary(ref(null)).fields.value).toEqual([]);
  });

  it('summarizes a short term request', () => {
    const { fields } = useWorkerRequestSummary(ref(detail()));

    expect(fields.value).toEqual([
      { label: 'Role (position)', value: 'Forklift Operator' },
      { label: 'Rate', value: `${currency(20)} / hr` },
      { label: 'Start / Finish', value: '01-Mar-2026 → 15-Mar-2026' },
      { label: 'Shift', value: '08:00 - 16:00 · 30 min break · unpaid' },
      { label: 'Term', value: 'Short Term' },
      { label: 'Break', value: '30 min break · unpaid' },
      { label: 'Holiday', value: 'Paid' },
    ]);
  });

  it('falls back from position to title to a dash', () => {
    expect(valueOf(detail({ jobPosition: '' }), 'Role (position)')).toBe('Warehouse');
    expect(valueOf(detail({ jobPosition: undefined, jobTitle: undefined }), 'Role (position)')).toBe('—');
  });

  it('shows the salary and incentive', () => {
    const { fields } = useWorkerRequestSummary(ref(detail({ workerSalary: 52000, incentive: 2 })));

    expect(fields.value[1]).toEqual({ label: 'Salary', value: `${currency(52000)} / year + ${currency(2)} incentive` });
  });

  it('hides the finish date of an open long term request', () => {
    const request = detail({ durationTerm: 'LongTerm' });

    expect(valueOf(request, 'Start')).toBe('01-Mar-2026');
    expect(valueOf(request, 'Term')).toBe('Long Term');
  });

  it.each(['Filled', 'Cancelled'])('shows the finish date of a %s long term request', (requestStatus) => {
    expect(valueOf(detail({ durationTerm: 'LongTerm', requestStatus }), 'Start / Finish')).toBe('01-Mar-2026 → 15-Mar-2026');
  });

  it('shows the shift alone without a break and marks holidays as not paid', () => {
    const request = detail({ durationBreak: undefined, displayShift: undefined, holidayIsPaid: false });

    expect(valueOf(request, 'Shift')).toBe('—');
    expect(valueOf(request, 'Break')).toBe('—');
    expect(valueOf(request, 'Holiday')).toBe('Not paid');
  });

  it('reacts to request changes', () => {
    const request = ref<WorkerRequestDetail | null>(null);
    const { fields } = useWorkerRequestSummary(request);
    expect(fields.value).toHaveLength(0);

    request.value = detail({ durationBreak: '01:00:00', breakIsPaid: true });
    expect(fields.value.find((field) => field.label === 'Break')?.value).toBe('1 h break · paid');
  });
});
