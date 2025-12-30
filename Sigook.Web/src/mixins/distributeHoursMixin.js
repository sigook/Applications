const distributeHoursMixin = {
  methods: {
    distributeWeekHours(week) {
      let hours = this.distributeHours(week.days.length, week.totalHoursWeek, this.maximumDailyHours);
      if (hours.length > 0) {
        for (let i = 0; i < hours.length; i++) {
          if (week.days[i].totalHoursApproved) {
            week.days[i].totalHoursApproved = hours[i] || 0;
          } else {
            this.$set(week.days[i], 'totalHoursApproved', hours[i] || 0);
          }
        }
      } else {
        this.showAlertError("Total hours is invalid");
      }
    },
    distributeHours(totalDays, totalHoursWeek, maximumDailyHours) {
      let weekDays = [];
      weekDays.length = totalDays;

      let dailyHours = totalHoursWeek / totalDays;
      if (dailyHours > maximumDailyHours) return [];
      else if (totalHoursWeek < maximumDailyHours) weekDays[0] = totalHoursWeek;
      else {
        let day = 0;
        while (totalHoursWeek > 0) {
          let h;
          let pureHours = Math.trunc(dailyHours);
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
    getTotalHoursWeekItems(week) {
      let total = 0;
      for (let i = 0; i < week.days.length; i++) {
        total += parseFloat(week.days[i].totalHoursApproved) || 0;
      }
      week.totalHoursWeek = total;
      return total;
    },
  },
  computed: {
    maximumDailyHours() {
      return this.$store.state.catalog.maximumHoursPerDay ? this.$store.state.catalog.maximumHoursPerDay : 12;
    }
  }
};

export default distributeHoursMixin;