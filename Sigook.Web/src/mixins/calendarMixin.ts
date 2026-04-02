import dayjs from "dayjs";

interface CalendarDay {
  id: string | null;
  day: Date;
  totalHoursApproved?: number;
  [key: string]: any;
}

interface CalendarWeek {
  totalHoursWeek: number;
  days: CalendarDay[];
}

const calendarMixin = {
  data() {
    return {
      calendar: [] as CalendarWeek[],
      selectDate: null as Date | string | null,
      today: null as Date | null,
      weekdays: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
      momentFormat: 'YYYY-MM-DD'
    };
  },
  methods: {
    getCurrentMonth(): void {
      const vm = this as any;
      vm.calendar = [];
      const startDay = dayjs(vm.selectDate).startOf('month').startOf('week');
      const endDay = dayjs(vm.selectDate).endOf('month').endOf('week');
      let date = startDay;
      while (date.isBefore(endDay, 'day') || date.isSame(endDay, 'day')) {
        const week: CalendarWeek = {
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
        vm.calendar.push(week);
      }
      vm.updateParent(startDay, endDay);
    },
    getTodayMonth(): void {
      const vm = this as any;
      vm.selectDate = dayjs(vm.today).startOf('month').format(vm.momentFormat);
      vm.getCurrentMonth();
    },
    getNextMonth(): void {
      const vm = this as any;
      vm.selectDate = dayjs(vm.selectDate).add(1, 'month');
      vm.getCurrentMonth();
    },
    getPreviousMonth(): void {
      const vm = this as any;
      vm.selectDate = dayjs(vm.selectDate).subtract(1, 'month');
      vm.getCurrentMonth();
    },
    isToday(date: Date | string): boolean {
      const vm = this as any;
      return dayjs(date).format(vm.momentFormat) === dayjs(vm.today).format(vm.momentFormat);
    },
    notCurrentMonth(date: Date | string): boolean {
      const vm = this as any;
      return dayjs(date).format('MMMM') !== dayjs(vm.selectDate).format('MMMM');
    },
    toMomentFormat(date: Date | string): string {
      const vm = this as any;
      return dayjs(date).format(vm.momentFormat).toString();
    },
    isAvailableToUpdate(date: Date | string): boolean {
      const vm = this as any;
      const start = dayjs(vm.startDate).subtract(1, 'day');
      const oneMonth = dayjs().add(1, 'month');
      if (dayjs(date).toDate() > start.toDate() && dayjs(date).toDate() < oneMonth.toDate()) {
        return true;
      }
      return false;
    }
  },
  created() {
    const vm = this as any;
    vm.today = dayjs().toDate();
    vm.selectDate = vm.today;
    vm.getTodayMonth();
  },
  filters: {
    onlyMonth(value: string | Date): string {
      return value ? dayjs(value).format('MMMM').toString() : String(value);
    },
    onlyYear(value: string | Date): string {
      return value ? dayjs(value).format('YYYY').toString() : String(value);
    },
    onlyDay(value: string | Date): string {
      return value ? dayjs(value).format('DD').toString() : String(value);
    }
  }
};

export default calendarMixin;
