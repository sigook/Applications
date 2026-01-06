import dayjs from "dayjs";

const calendarMixin = {
  data() {
    return {
      calendar: [],
      selectDate: null,
      today: null,
      weekdays: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
      momentFormat: 'YYYY-MM-DD'
    }
  },
  methods: {
    getCurrentMonth() {
      this.calendar = [];
      const startDay = dayjs(this.selectDate).startOf('month').startOf('week');
      const endDay = dayjs(this.selectDate).endOf('month').endOf('week');
      let date = startDay;
      while (date.isBefore(endDay, 'day') || date.isSame(endDay, 'day')) {
        const week = {
          totalHoursWeek: 0,
          days: []
        };
        for (let i = 0; i < 7; i++) {
          week.days.push({
            id: null,
            day: date.toDate()
          });
          date = date.add(1, 'day');
        }
        this.calendar.push(week);
      }
      this.updateParent(startDay, endDay);
    },
    getTodayMonth() {
      this.selectDate = dayjs(this.today).startOf('month').format(this.momentFormat);
      this.getCurrentMonth();
    },
    getNextMonth() {
      this.selectDate = dayjs(this.selectDate).add(1, 'month');
      this.getCurrentMonth();
    },
    getPreviousMonth() {
      this.selectDate = dayjs(this.selectDate).subtract(1, 'month');
      this.getCurrentMonth();
    },
    isToday(date) {
      return dayjs(date).format(this.momentFormat) === dayjs(this.today).format(this.momentFormat)
    },
    notCurrentMonth(date) {
      return dayjs(date).format('MMMM') !== dayjs(this.selectDate).format('MMMM')
    },
    toMomentFormat(date) {
      return dayjs(date).format(this.momentFormat).toString()
    },
    isAvailableToUpdate(date) {
      /*
      * Works for add the class to show disable days before the request start
      * */
      let start = dayjs(this.startDate).subtract(1, 'day');
      let oneMonth = dayjs().add(1, 'month');
      if (dayjs(date).toDate() > start.toDate() && dayjs(date).toDate() < oneMonth.toDate()) {
        return true;
      }
      return false;
    }
  },
  created() {
    this.today = dayjs().toDate();
    if (this.status) {
      switch (this.status) {
        case this.$statusFinalized:
        case this.$statusCancelled:
          this.selectDate = dayjs(this.startDate).toDate();
          this.getCurrentMonth();
          return;
        default:
          this.selectDate = this.today;
          this.getTodayMonth();
          return;
      }
    } else {
      this.getTodayMonth();
    }
  },
  filters: {
    onlyMonth(value) {
      return value ? dayjs(value).format('MMMM').toString() : value;
    },
    onlyYear(value) {
      return value ? dayjs(value).format('YYYY').toString() : value;
    },
    onlyDay(value) {
      return value ? dayjs(value).format('DD').toString() : value;
    }
  }
};

export default calendarMixin;