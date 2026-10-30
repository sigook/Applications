import dayjs from 'dayjs';
import { dateHHmm } from '@/shared/format';
import { WorkerTimeSheetStatus } from '@/modules/worker/activity/types';
import type { WorkerWeekTimeSheet } from '@/modules/worker/activity/types';

export function hoursLabel(hours: number | null | undefined): string {
  return (hours ?? 0).toFixed(1);
}

export function weekStartOf(day: dayjs.ConfigType = undefined): string {
  return dayjs(day).startOf('week').format('YYYY-MM-DD');
}

const statusTags: Record<string, string> = {
  Active: 'is-info is-light',
  Completed: 'is-success is-light',
  Ended: '',
  [WorkerTimeSheetStatus[WorkerTimeSheetStatus.Open]]: 'is-info is-light',
  [WorkerTimeSheetStatus[WorkerTimeSheetStatus.Pending]]: 'is-warning is-light',
  [WorkerTimeSheetStatus[WorkerTimeSheetStatus.Approved]]: 'is-success is-light',
  [WorkerTimeSheetStatus[WorkerTimeSheetStatus.Paid]]: 'is-success',
  'No clock-out': 'is-danger is-light',
};

export function statusTagType(status: string | undefined): string {
  return statusTags[status ?? ''] ?? '';
}

export function timeSheetStatusLabel(row: WorkerWeekTimeSheet): string {
  if (row.status === WorkerTimeSheetStatus.Open && dayjs(row.date).isBefore(dayjs(), 'day')) return 'No clock-out';
  return WorkerTimeSheetStatus[row.status];
}

export function isManualEntry(row: WorkerWeekTimeSheet): boolean {
  return !row.clockIn && !row.clockOut;
}

export function timeSheetRangeLabel(row: WorkerWeekTimeSheet): string {
  if (isManualEntry(row)) return 'Manual entry';
  return `${dateHHmm(row.clockIn ?? null) ?? '—'} – ${dateHHmm(row.clockOut ?? null) ?? '—'}`;
}

export function spanMinutes(value: string | null | undefined): number {
  const [hours = 0, minutes = 0] = (value ?? '').split(':').map((part) => parseInt(part, 10) || 0);
  return hours * 60 + minutes;
}

export function breakLabel(row: WorkerWeekTimeSheet): string {
  const total = spanMinutes(row.durationBreak);
  if (!total) return '—';
  return row.breakIsPaid ? `${total} min (paid)` : `${total} min`;
}
