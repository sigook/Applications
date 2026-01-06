import dayjs from "dayjs";

export default {
  methods: {
    timeSheetFastApprove(item, requestId, workerId) {
      this.isLoading = true;
      let model = {
        hours: this.changeDecimalToHour(item.totalHours),
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
      this.$store.dispatch('agency/updateWorkerTimeSheet', { requestId: requestId, workerId: workerId, id: item.id, model: model })
        .then(() => {
          this.updateCell();
          this.isLoading = false;
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    changeDecimalToHour(time) {
      return dayjs().startOf('day').add(time, 'hours').format('HH:mm:ss');
    }
  }
}; 