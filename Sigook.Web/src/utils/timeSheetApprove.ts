import dayjs from 'dayjs';

export interface TimeSheetFastApproveItem {
  id: string;
  totalHours: number;
  timeIn: string;
  missingHours: number;
  missingHoursOvertime: number;
  missingRateWorker: number;
  missingRateAgency: number;
  deductionsOthers: number;
  bonusOrOthers: number;
  deductionsOthersDescription: string;
  bonusOrOthersDescription: string;
}

export interface TimeSheetFastApproveModel {
  hours: string;
  timeIn: string;
  missingHours: number;
  missingHoursOvertime: number;
  missingRateWorker: number;
  missingRateAgency: number;
  deductionsOthers: number;
  bonusOrOthers: number;
  deductionsOthersDescription: string;
  bonusOrOthersDescription: string;
}

export function buildTimeSheetApproveModel(item: TimeSheetFastApproveItem): TimeSheetFastApproveModel {
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
