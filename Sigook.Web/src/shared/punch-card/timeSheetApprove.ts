import dayjs from 'dayjs';
import type { PunchCardDay, TimeSheetModel } from '@/shared/punch-card/types';

function pad(value: number): string {
  return String(value).padStart(2, '0');
}

export function formatTimeSpan(hours: number): string {
  const totalSeconds = Math.round(hours * 3600);
  const days = Math.floor(totalSeconds / 86400);
  const time = `${pad(Math.floor((totalSeconds % 86400) / 3600))}:${pad(Math.floor((totalSeconds % 3600) / 60))}:${pad(totalSeconds % 60)}`;
  return days > 0 ? `${days}.${time}` : time;
}

export function buildTimeSheetApproveModel(item: PunchCardDay): TimeSheetModel {
  return {
    hours: formatTimeSpan(item.totalHours ?? 0),
    timeIn: dayjs(item.timeIn).format('YYYY-MM-DDTHH:mm:ss'),
    missingHours: item.missingHours,
    missingHoursOvertime: item.missingHoursOvertime,
    missingRateWorker: item.missingRateWorker,
    missingRateAgency: item.missingRateAgency,
    deductionsOthers: item.deductionsOthers,
    bonusOrOthers: item.bonusOrOthers,
    deductionsOthersDescription: item.deductionsOthersDescription,
    bonusOrOthersDescription: item.bonusOrOthersDescription,
  };
}
