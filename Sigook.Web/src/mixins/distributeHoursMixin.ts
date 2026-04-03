import { maximumHoursPerDay } from '@/constants/catalog';

interface WeekDay {
  totalHoursApproved?: number;
  [key: string]: any;
}

interface Week {
  totalHoursWeek: number;
  days: WeekDay[];
}

const distributeHoursMixin = {
  methods: {
    distributeWeekHours(week: Week): void {
      const hours = (this as any).distributeHours(week.days.length, week.totalHoursWeek, (this as any).maximumDailyHours);
      if (hours.length > 0) {
        for (let i = 0; i < hours.length; i++) {
          if (week.days[i].totalHoursApproved) {
            week.days[i].totalHoursApproved = hours[i] || 0;
          } else {
            (this as any).$set(week.days[i], 'totalHoursApproved', hours[i] || 0);
          }
        }
      } else {
        (this as any).showAlertError("Total hours is invalid");
      }
    },
    distributeHours(totalDays: number, totalHoursWeek: number, maximumDailyHours: number): number[] {
      const weekDays: number[] = [];
      weekDays.length = totalDays;

      const dailyHours = totalHoursWeek / totalDays;
      if (dailyHours > maximumDailyHours) return [];
      else if (totalHoursWeek < maximumDailyHours) weekDays[0] = totalHoursWeek;
      else {
        let day = 0;
        while (totalHoursWeek > 0) {
          let h: number;
          const pureHours = Math.trunc(dailyHours);
          if (pureHours === 0) h = dailyHours;
          else if (totalHoursWeek < dailyHours) h = totalHoursWeek;
          else if (pureHours == maximumDailyHours) h = pureHours;
          else if (pureHours < totalHoursWeek) h = pureHours + 1;
          else h = totalHoursWeek;
          totalHoursWeek -= h;
          weekDays[day] = h;
          day++;
        }
      }
      return weekDays;
    },
    getTotalHoursWeekItems(week: Week): number {
      let total = 0;
      for (let i = 0; i < week.days.length; i++) {
        total += parseFloat(String(week.days[i].totalHoursApproved)) || 0;
      }
      week.totalHoursWeek = total;
      return total;
    },
  },
  computed: {
    maximumDailyHours(): number {
      return maximumHoursPerDay;
    }
  }
};

export default distributeHoursMixin;
