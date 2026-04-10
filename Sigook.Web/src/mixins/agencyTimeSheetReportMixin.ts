import dayjs from "dayjs";
import { updateAgencyWorkerTimeSheet } from "@/api/agencyTimeSheetApi";

interface TimeSheetItem {
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

export default {
  methods: {
    timeSheetFastApprove(item: TimeSheetItem, requestId: string, workerId: string): void {
      const vm = this as any;
      vm.isLoading = true;
      const model = {
        hours: vm.changeDecimalToHour(item.totalHours),
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
      updateAgencyWorkerTimeSheet(requestId, workerId, item.id, model)
        .then(() => {
          vm.updateCell();
          vm.isLoading = false;
        }).catch((error: any) => {
          vm.isLoading = false;
          vm.showAlertError(error);
        });
    },
    changeDecimalToHour(time: number): string {
      return dayjs().startOf('day').add(time, 'hours').format('HH:mm:ss');
    }
  }
};
