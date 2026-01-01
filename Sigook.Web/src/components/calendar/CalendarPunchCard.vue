<template>
  <div class="wrapper-calendar">
    <div class="container-flex">
      <div class="col-8">
        <h2 class="fz1">{{ selectDate | onlyMonth }} <span class="fw-100">{{ selectDate | onlyYear }}</span></h2>
      </div>
      <div class="col-4 text-right align-s-center">
        <b-button size="is-small" @click="getPreviousMonth" icon-left="chevron-left" class="btn-calendar">
        </b-button>
        <b-button size="is-small" @click="getTodayMonth" class="btn-today">Today</b-button>
        <b-button size="is-small" @click="getNextMonth" icon-right="chevron-right" class="btn-calendar">
        </b-button>
      </div>
    </div>

    <!-- Desktop: Tabla tradicional -->
    <table v-if="!isMobile && weekdays" class="w-100 isPunchCard" :class="{ 'hasEvents': hasEvents }">
      <thead>
        <tr class="no-border">
          <th>
            <div class="totalHours">
              Total
            </div>
          </th>
          <th class="pl-1 min-100" v-for="item in weekdays" :key="'weekDays' + item">{{ item }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(week, indexWeek) in calendar" :key="'calendarWeek' + indexWeek">
          <td>
            <div class="totalHours input-no-arrows">
              <b-numberinput v-model="week.totalHoursWeek" step="0.01" :controls="false"></b-numberinput>
              <b-button type="is-primary" @click="distributeWeekHours(week)">ADD</b-button>
            </div>
          </td>
          <td v-for="(item, indexDay) in week.days" :key="'calendarDay' + indexDay"
            :class="{ 'bg-gray': !isAvailableToUpdate(item.day) || !isAvailableToUpdateWorker(item.day) }">
            <div class="wrapper-day" :class="{ 'highlight-day': item.id }">
              <span :class="{ 'isToday': isToday(item.day), 'notCurrentMonth': notCurrentMonth(item.day) }">
                {{ item.day | onlyDay }}
              </span>
              <slot name="punch-input"
                v-if="item && isAvailableToUpdate(item.day) && isAvailableToUpdateWorker(item.day)" :index="indexDay"
                :item="item" />
            </div>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Mobile: Cards por semana -->
    <div v-else-if="isMobile && weekdays" class="mobile-calendar" :class="{ 'hasEvents': hasEvents }">
      <div v-for="(week, indexWeek) in calendar" :key="'mobileWeek' + indexWeek" class="week-card">
        <div class="week-header">
          <span class="week-title">Week {{ indexWeek + 1 }}</span>
          <div class="week-total">
            <b-field>
              <b-numberinput v-model="week.totalHoursWeek" step="0.01" placeholder="Total"
                :controls="false"></b-numberinput>
              <b-button type="is-primary" @click="distributeWeekHours(week)">ADD</b-button>
            </b-field>
          </div>
        </div>
        <div class="days-grid">
          <div v-for="(item, indexDay) in week.days" :key="'mobileDay' + indexDay" class="day-card" :class="{
            'bg-gray': !isAvailableToUpdate(item.day) || !isAvailableToUpdateWorker(item.day),
            'highlight-day': item.id,
            'is-today': isToday(item.day),
            'not-current-month': notCurrentMonth(item.day)
          }">
            <div class="day-header">{{ weekdays[indexDay] }}</div>
            <div class="day-number"
              :class="{ 'isToday': isToday(item.day), 'notCurrentMonth': notCurrentMonth(item.day) }">
              {{ item.day | onlyDay }}
            </div>
            <div class="day-content">
              <slot name="punch-input"
                v-if="item && isAvailableToUpdate(item.day) && isAvailableToUpdateWorker(item.day)" :index="indexDay"
                :item="item" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>

import calendarMixin from "@/mixins/calendarMixin";
import distributeHours from "@/mixins/distributeHoursMixin";
import dayjs from "dayjs";
export default {
  props: ["highlights", "workerId", "requestId", "startDate", "status", "worker"],
  mixins: [distributeHours, calendarMixin],
  data() {
    return {
      windowWidth: window.innerWidth
    }
  },
  mounted() {
    window.addEventListener('resize', this.handleResize);
  },
  beforeDestroy() {
    window.removeEventListener('resize', this.handleResize);
  },
  methods: {
    handleResize() {
      this.windowWidth = window.innerWidth;
    },
    updateParent(startDay, endDay) {
      let start = startDay.format(this.momentFormat)
      let end = endDay.format(this.momentFormat)
      this.$emit("onMonthChange", { startDate: start, endDate: end })
    },
    isAvailableToUpdateWorker(date) {
      if (this.worker && this.worker.status === this.$statusReject && this.worker.rejectedAt) {
        let start = dayjs(this.startDate).subtract(1, 'day');
        let oneMonth = dayjs(this.worker.rejectedAt).add(1, 'month');
        if (dayjs(date).toDate() > start.toDate() && dayjs(date).toDate() < oneMonth.toDate()) {
          return true;
        }
        return false;
      }
      return true;
    },
    syncHighlightsWithCalendar() {
      if (!this.highlights || !this.calendar.length) return;
      for (let iWeek = 0; iWeek < this.calendar.length; iWeek++) {
        this.calendar[iWeek].totalHoursWeek = this.calendar[iWeek].days.reduce((acc, day) => {
          if (day.totalHoursApproved) {
            return acc + day.totalHoursApproved
          }
          return acc;
        }, 0);
        for (let iDay = 0; iDay < this.calendar[iWeek].days.length; iDay++) {
          let currentDay = this.toMomentFormat(this.calendar[iWeek].days[iDay].day);
          let indexDay = this.highlights.findIndex(d => {
            return this.toMomentFormat(d.day) === currentDay
          })
          if (indexDay >= 0) {
            this.calendar[iWeek].days[iDay] = this.highlights[indexDay];
          }
        }
      }
      this.calendar.forEach(week => {
        week.totalHoursWeek = week.days.reduce((acc, day) => {
          if (day.totalHoursApproved) {
            return acc + day.totalHoursApproved
          }
          return acc;
        }, 0);
      });
    }
  },
  computed: {
    isMobile() {
      return this.windowWidth <= 768;
    },
    hasEvents() {
      return this.highlights && this.highlights.length > 0;
    }
  },
  watch: {
    highlights: {
      handler() {
        this.syncHighlightsWithCalendar();
      },
      immediate: true
    }
  }
}
</script>

<style scoped>
/* Estilos para Mobile Calendar */
.mobile-calendar {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-top: 20px;
}

.week-card {
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  background: #fff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.week-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: #f8f9fa;
  border-bottom: 1px solid #e0e0e0;
}

.week-title {
  font-weight: 600;
  color: #333;
  font-size: 16px;
}

.week-total {
  display: flex;
  align-items: center;
  gap: 8px;
}

.week-total .field {
  margin-bottom: 0;
}

.days-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 1px;
  background: #e0e0e0;
}

.day-card {
  background: white;
  padding: 12px 8px;
  text-align: center;
  min-height: 80px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  transition: background-color 0.2s;
}

.day-card.bg-gray {
  background: #f5f5f5;
  opacity: 0.6;
}

.day-card.highlight-day {
  background: #e3f2fd;
}

.day-card.is-today {
  background: #fff3e0;
  border: 2px solid #ff9800;
}

.day-header {
  font-size: 11px;
  font-weight: 600;
  color: #666;
  text-transform: uppercase;
  margin-bottom: 4px;
}

.day-number {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin-bottom: 8px;
}

.day-number.isToday {
  color: #ff9800;
}

.day-number.notCurrentMonth {
  color: #ccc;
}

.day-content {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Ajustes para componentes Buefy */
.totalHours .field {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 0;
}

.totalHours .input {
  min-width: 80px;
}

/* Responsive breakpoints */
@media (min-width: 769px) {
  .mobile-calendar {
    display: none;
  }
}

@media (max-width: 768px) {
  .wrapper-calendar table {
    display: none;
  }

  .container-flex {
    flex-wrap: wrap;
  }

  .container-flex .col-8,
  .container-flex .col-4 {
    flex: 1;
    min-width: 100%;
    text-align: center;
    margin-bottom: 10px;
  }

  .btn-calendar,
  .btn-today {
    margin: 0 4px;
  }

  /* Ajustes para pantallas muy pequeñas */
  @media (max-width: 480px) {
    .days-grid {
      grid-template-columns: repeat(7, 1fr);
      gap: 0;
    }

    .day-card {
      padding: 8px 4px;
      min-height: 70px;
      font-size: 12px;
    }

    .day-header {
      font-size: 10px;
    }

    .day-number {
      font-size: 14px;
    }

    .week-header {
      padding: 10px 12px;
    }

    .week-title {
      font-size: 14px;
    }

    .week-total .field {
      font-size: 12px;
    }
  }
}

/* Estilos adicionales para botones de navegación */
.btn-calendar.button {
  border: 1px solid #ddd;
  background: white;
}

.btn-today.button {
  border: 1px solid #ddd;
  background: white;
}

.btn-calendar:hover,
.btn-today:hover {
  background: #f5f5f5;
}
</style>