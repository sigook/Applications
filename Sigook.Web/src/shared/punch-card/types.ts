// TimeSheet models
export interface TimeSheetListItem {
  id: string;
  day: string;
  clockIn: string | null;
  clockOut: string | null;
  clockInRounded?: string | null;
  clockOutRounded?: string | null;
  timeIn: string;
  timeOut: string | null;
  timeInApproved?: string | null;
  timeOutApproved?: string | null;
  totalHours: number;
  totalHoursApproved: number;
  canUpdate: boolean;
  wasApproved: boolean;
  missingHours: string;
  missingHoursOvertime: string;
  missingRateWorker: number;
  missingRateAgency: number;
  deductionsOthers: number;
  bonusOrOthers: number;
  deductionsOthersDescription: string;
  bonusOrOthersDescription: string;
  reimbursements: number;
  reimbursementsDescription: string;
  comment: string;
}

export interface TimeSheetModel {
  hours: string;
  timeIn: string;
  missingHours: string | number;
  missingHoursOvertime: string | number;
  deductionsOthers?: number;
  bonusOrOthers?: number;
  deductionsOthersDescription?: string;
  bonusOrOthersDescription?: string;
  comments?: string;
  missingRateWorker?: number;
  missingRateAgency?: number;
  reimbursements?: number;
  reimbursementsDescription?: string;
}

export interface PunchCardDay extends Partial<Omit<TimeSheetListItem, 'id' | 'day' | 'totalHoursApproved'>> {
  id: string | null;
  day: string | Date;
  totalHoursApproved: number | null;
}

export interface PunchCardWeek {
  totalHoursWeek: number | null;
  days: PunchCardDay[];
}

export interface PunchCardEditableDay extends Omit<PunchCardDay, 'timeInApproved' | 'timeOutApproved'> {
  hoursApprovedToDate?: Date | null;
  missinghoursToDate?: Date | null;
  missingHoursOvertimeToDate?: Date | null;
  timeInApproved?: Date | string | null;
  timeOutApproved?: Date | string | null;
}

export interface PunchCardWorker {
  workerRequestStatus: number;
  rejectedAt?: string | null;
}

// Response shape of GET /api/agency/recruiting/requests/{}/Workers/{}/TimeSheets/{}/Usages.
// Mirrors backend TimeSheetUsagesModel — single object, not an array.
export interface TimeSheetUsagesModel {
  invoiceNumber?: number | null;
  payStubNumber?: string | null;
}
