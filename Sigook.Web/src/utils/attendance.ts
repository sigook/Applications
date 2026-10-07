import dayjs from 'dayjs';
import type { UserAttendanceHours } from '@/types/agency';

const REGULAR_HOURS_PER_DAY = 8;
export const DEFAULT_LUNCH_MINUTES = 60;

export function isWeekend(date: dayjs.ConfigType): boolean {
  const day = dayjs(date).day();
  return day === 0 || day === 6;
}

export function defaultLunchMinutes(date: dayjs.ConfigType): number {
  return isWeekend(date) ? 0 : DEFAULT_LUNCH_MINUTES;
}

export function calculateAttendanceHours(clockIn: dayjs.ConfigType, clockOut: dayjs.ConfigType, lunchMinutes = defaultLunchMinutes(clockIn)): UserAttendanceHours {
  const start = dayjs(clockIn);
  const end = dayjs(clockOut);
  if (!end.isAfter(start)) return { workedHours: 0, lunchHours: 0, regularHours: 0, overtimeHours: 0 };

  const elapsed = Math.round((end.diff(start, 'minute') / 60) * 100) / 100;
  const lunchHours = Math.min(Math.round((lunchMinutes / 60) * 100) / 100, elapsed);
  const workedHours = elapsed - lunchHours;
  const regularHours = isWeekend(start) ? 0 : Math.min(REGULAR_HOURS_PER_DAY, workedHours);
  return { workedHours, lunchHours, regularHours, overtimeHours: workedHours - regularHours };
}

export function formatHours(hours: number | null | undefined): string {
  return (hours ?? 0).toFixed(2);
}

export function formatElapsed(from: dayjs.ConfigType, to: dayjs.ConfigType = undefined): string {
  const minutes = Math.max(0, dayjs(to).diff(dayjs(from), 'minute'));
  const h = Math.floor(minutes / 60).toString().padStart(2, '0');
  const m = (minutes % 60).toString().padStart(2, '0');
  return `${h}:${m}`;
}

export function toLocalDateTime(date: dayjs.ConfigType): string {
  return dayjs(date).format('YYYY-MM-DDTHH:mm:ss');
}
