<template>
  <div class="wrapper-calendar">
    <div class="columns is-multiline">
      <div class="column is-8">
        <h2 class="fz1">{{ onlyMonth(selectDate) }} <span class="has-text-weight-light">{{ onlyYear(selectDate) }}</span></h2>
      </div>
      <div class="column is-4 has-text-right is-align-self-center">
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
        <tr class="border-0">
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
                {{ onlyDay(item.day) }}
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
              {{ onlyDay(item.day) }}
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
<script setup lang="ts">
import { ref, computed, watch, onMounted, onBeforeUnmount } from 'vue';
import { showAlertError } from "@/utils/toast";
import { distributeHours } from "@/utils/distributeHours";
import { maximumHoursPerDay } from "@/constants/catalog";
import dayjs from "dayjs";
import { WorkerRequestStatus } from "@/constants/enums";

const props = defineProps<{
  highlights?: any[];
  workerProfileId?: any;
  requestId?: any;
  startDate?: any;
  status?: any;
  worker?: any;
}>();

const emit = defineEmits<{ (e: 'onMonthChange', v: { startDate: string; endDate: string }): void }>();

const windowWidth = ref(window.innerWidth);
const calendar = ref<any[]>([]);
const selectDate = ref<any>(null);
const today = ref<any>(null);
const weekdays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const momentFormat = 'YYYY-MM-DD';

function onlyMonth(value: any) {
  return value ? dayjs(value).format('MMMM').toString() : String(value);
}
function onlyYear(value: any) {
  return value ? dayjs(value).format('YYYY').toString() : String(value);
}
function onlyDay(value: any) {
  return value ? dayjs(value).format('DD').toString() : String(value);
}

function updateParent(startDay: any, endDay: any) {
  const start = startDay.format(momentFormat);
  const end = endDay.format(momentFormat);
  emit('onMonthChange', { startDate: start, endDate: end });
}

function getCurrentMonth() {
  calendar.value = [];
  const startDay = dayjs(selectDate.value).startOf('month').startOf('week');
  const endDay = dayjs(selectDate.value).endOf('month').endOf('week');
  let date = startDay;
  while (date.isBefore(endDay, 'day') || date.isSame(endDay, 'day')) {
    const week: any = { totalHoursWeek: 0, days: [] };
    for (let i = 0; i < 7; i++) {
      week.days.push({ id: null, day: date.toDate(), totalHoursApproved: 0 });
      date = date.add(1, 'day');
    }
    calendar.value.push(week);
  }
  updateParent(startDay, endDay);
}

function getTodayMonth() {
  selectDate.value = dayjs(today.value).startOf('month').format(momentFormat);
  getCurrentMonth();
}

function getNextMonth() {
  selectDate.value = dayjs(selectDate.value).add(1, 'month');
  getCurrentMonth();
}

function getPreviousMonth() {
  selectDate.value = dayjs(selectDate.value).subtract(1, 'month');
  getCurrentMonth();
}

function isToday(date: any) {
  return dayjs(date).format(momentFormat) === dayjs(today.value).format(momentFormat);
}

function notCurrentMonth(date: any) {
  return dayjs(date).format('MMMM') !== dayjs(selectDate.value).format('MMMM');
}

function toMomentFormat(date: any) {
  return dayjs(date).format(momentFormat).toString();
}

function isAvailableToUpdate(date: any) {
  const start = dayjs(props.startDate).subtract(1, 'day');
  const oneMonth = dayjs().add(1, 'month');
  if (dayjs(date).toDate() > start.toDate() && dayjs(date).toDate() < oneMonth.toDate()) {
    return true;
  }
  return false;
}

function handleResize() {
  windowWidth.value = window.innerWidth;
}

function distributeWeekHours(week: any) {
  const hours = distributeHours(week.days.length, week.totalHoursWeek, maximumHoursPerDay);
  if (hours.length > 0) {
    for (let i = 0; i < hours.length; i++) {
      week.days[i].totalHoursApproved = hours[i] || 0;
    }
  } else {
    showAlertError("Total hours is invalid");
  }
}

function isAvailableToUpdateWorker(date: any) {
  if (props.worker && props.worker.workerRequestStatus === WorkerRequestStatus.Rejected && props.worker.rejectedAt) {
    const start = dayjs(props.startDate).subtract(1, 'day');
    const oneMonth = dayjs(props.worker.rejectedAt).add(1, 'month');
    if (dayjs(date).toDate() > start.toDate() && dayjs(date).toDate() < oneMonth.toDate()) {
      return true;
    }
    return false;
  }
  return true;
}

function syncHighlightsWithCalendar() {
  if (!props.highlights || !calendar.value.length) return;
  for (let iWeek = 0; iWeek < calendar.value.length; iWeek++) {
    calendar.value[iWeek].totalHoursWeek = calendar.value[iWeek].days.reduce((acc: number, day: any) => {
      if (day.totalHoursApproved) return acc + day.totalHoursApproved;
      return acc;
    }, 0);
    for (let iDay = 0; iDay < calendar.value[iWeek].days.length; iDay++) {
      const currentDay = toMomentFormat(calendar.value[iWeek].days[iDay].day);
      const indexDay = props.highlights.findIndex((d: any) => toMomentFormat(d.day) === currentDay);
      if (indexDay >= 0) {
        calendar.value[iWeek].days[iDay] = props.highlights[indexDay];
      }
    }
  }
  calendar.value.forEach((week: any) => {
    week.totalHoursWeek = week.days.reduce((acc: number, day: any) => {
      if (day.totalHoursApproved) return acc + day.totalHoursApproved;
      return acc;
    }, 0);
  });
}

const isMobile = computed(() => windowWidth.value <= 768);
const hasEvents = computed(() => props.highlights && props.highlights.length > 0);

watch(() => props.highlights, () => {
  syncHighlightsWithCalendar();
}, { immediate: true });

// created()
today.value = dayjs().toDate();
selectDate.value = today.value;
getTodayMonth();

onMounted(() => {
  window.addEventListener('resize', handleResize);
});

onBeforeUnmount(() => {
  window.removeEventListener('resize', handleResize);
});

defineExpose({ WorkerRequestStatus });
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
  color: #b35c00;
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

  .columns .column {
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

<style scoped lang="scss">
.wrapper-calendar {
  table {
    border-collapse: collapse;

    td:nth-child(1),
    td:nth-last-child(1) {
      background: #f7f7f7;
    }

    &.isPunchCard {
      table-layout: fixed;
      width: 100%;

      th:nth-child(1),
      td:nth-child(1) {
        width: 60px;
        background: #e4e4e4;
        border-left: 1px solid white;
      }
      td:nth-child(2),
      td:nth-last-child(1) {
        background: #f7f7f7;
      }
    }

    td {
      padding: 0 !important;
    }
    tbody td {
      border: 1px solid #d2d2d2;
    }
    .wrapper-day {
      min-height: 60px;
      padding: 5px;
      font-size: 14px;
    }

    .isToday {
      color: #1575bb;
      font-weight: bold;
    }

    .notCurrentMonth {
      color: #767676;
    }
    tr:hover {
      background: white !important;
    }
  }
  .btn-calendar {
    width: 14px;
    border: 0;
    display: inline-block;
    vertical-align: middle;
    margin: 0 5px;
  }
  .btn-prev img {
    transform: rotate(180deg);
  }
  .btn-today {
    border: 1px solid #a7a7a7;
    border-radius: 5px;
    margin: 0 5px;
    font-weight: bold;
    color: #6b6b6b;
    font-size: 14px;
    padding: 3px 8px;
  }
  .no-border {
    border: 0;
  }
  .highlight-day {
    & > span {
      line-height: 1;
      width: 22px;
      height: 22px;
      background: #ff9c28;
      display: inline-block;
      text-align: center;
      border-radius: 50%;
      padding: 5px 0;
      font-size: 13px;
      color: white;
    }
  }

  .bg-gray {
    background-color: #dadada !important;
    opacity: .5;
  }
}
</style>
