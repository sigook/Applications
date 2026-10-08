import dayjs from 'dayjs';
import type { PunchCardDay, TimeSheetModel } from '@/shared/punch-card/types';

export function buildTimeSheetApproveModel(item: PunchCardDay): TimeSheetModel {
  return {
    hours: dayjs().startOf('day').add(item.totalHours, 'hours').format('HH:mm:ss'),
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
