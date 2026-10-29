<template>
  <div class="mt-4 w-100">
    <div class="fz-1 has-text-weight-bold mb-3">
      Shift
    </div>
    <div class="shifts-grid">

      <div class="grid-header">Day</div>
      <div v-for="(item, dayKey) in weekShift" :key="'header-' + dayKey" class="grid-header">
        <b-checkbox size="is-small" v-model="item.going" @update:modelValue="updateModel">
          {{ item.day }}
        </b-checkbox>
      </div>

      <div class="grid-label">
        <b-button @click="showStart = !showStart" type="is-ghost" size="is-small" icon-right="menu-down">
          From
        </b-button>
      </div>
      <div v-for="(item, dayKey) in weekShift" :key="'from-' + dayKey" class="grid-cell">
        <b-timepicker size="is-small" v-if="item.going" v-model="item.start" hour-format="24" @update:modelValue="updateModel">
        </b-timepicker>
      </div>

      <div v-if="showStart" class="grid-apply-all">
        <div class="apply-all-section">
          <b-timepicker size="is-small" v-model="startMain" hour-format="24" />
          <b-button type="is-primary" size="is-small" @click="changeAllFrom">Apply All</b-button>
        </div>
      </div>

      <div class="grid-label">
        <b-button @click="showFinish = !showFinish" type="is-ghost" size="is-small" icon-right="menu-down">
          To
        </b-button>
      </div>
      <div v-for="(item, dayKey) in weekShift" :key="'to-' + dayKey" class="grid-cell">
        <b-timepicker size="is-small" v-if="item.going" v-model="item.finish" hour-format="24" @update:modelValue="updateModel">
        </b-timepicker>
      </div>

      <div v-if="showFinish" class="grid-apply-all">
        <div class="apply-all-section">
          <b-timepicker size="is-small" v-model="finishMain" hour-format="24" />
          <b-button type="is-primary" size="is-small" @click="changeAllTo">Apply All</b-button>
        </div>
      </div>
    </div>

    <div class="shifts-list">
      <div class="shift-row shift-list-header">
        <span>Day</span>
        <span>From</span>
        <span>To</span>
      </div>
      <div v-for="(item, dayKey) in weekShift" :key="'row-' + dayKey" class="shift-row">
        <b-checkbox size="is-small" v-model="item.going" @update:modelValue="updateModel">
          {{ item.day }}
        </b-checkbox>
        <template v-if="item.going">
          <b-timepicker size="is-small" v-model="item.start" hour-format="24" @update:modelValue="updateModel" />
          <b-timepicker size="is-small" v-model="item.finish" hour-format="24" @update:modelValue="updateModel" />
        </template>
        <span v-else class="shift-off">Off</span>
      </div>
      <div class="shift-row shift-apply-row">
        <span class="shift-apply-label">All days</span>
        <b-timepicker size="is-small" v-model="startMain" hour-format="24" />
        <b-timepicker size="is-small" v-model="finishMain" hour-format="24" />
        <b-button class="shift-apply-button" type="is-primary" size="is-small" @click="changeAll">Apply to checked days</b-button>
      </div>
    </div>

    <div class="pt-1">
      <b-field label="Comments">
        <b-input type="textarea" v-model="comments" name="description" @update:modelValue="updateModel" />
      </b-field>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, watch, onMounted } from 'vue';
import dayjs from "dayjs";
import type { RequestShiftModel } from '@/shared/request-detail/types';

const props = defineProps<{ currentShift?: RequestShiftModel | null; isUpdate?: boolean }>();
const emit = defineEmits<{ (e: 'updateModel', shift: RequestShiftModel): void }>();

const start = new Date();
start.setMinutes(0);
start.setHours(8);
start.setSeconds(0);

const finish = new Date();
finish.setMinutes(0);
finish.setHours(17);
finish.setSeconds(0);

const zeroHour = new Date();
zeroHour.setMinutes(0);
zeroHour.setHours(0);
zeroHour.setSeconds(0);

const startMain = ref<Date>(start);
const showStart = ref(false);
const finishMain = ref<Date>(finish);
const showFinish = ref(false);
const comments = ref<string | null>(null);

const weekShift = reactive<Record<string, { day: string; going: boolean | null; start: Date; finish: Date }>>({
  sun: { day: 'Sun', going: null, start: zeroHour, finish: zeroHour },
  mon: { day: 'Mon', going: true, start, finish },
  tue: { day: 'Tue', going: true, start, finish },
  wed: { day: 'Wed', going: true, start, finish },
  thu: { day: 'Thu', going: true, start, finish },
  fri: { day: 'Fri', going: true, start, finish },
  sat: { day: 'Sat', going: null, start: zeroHour, finish: zeroHour },
});

function hoursToString(hour: Date): string {
  return dayjs(hour).format('HH:mm:ss').toString();
}

function stringToHours(time: string): Date {
  if (time) {
    const [hours, minutes, seconds] = time.split(':').map(Number);
    return dayjs().startOf('day')
      .add(hours, 'hour')
      .add(minutes, 'minute')
      .add(seconds || 0, 'second').toDate();
  }
  return zeroHour;
}

function mapToShift(): RequestShiftModel {
  return {
    sunday: weekShift.sun.going,
    monday: weekShift.mon.going,
    tuesday: weekShift.tue.going,
    wednesday: weekShift.wed.going,
    thursday: weekShift.thu.going,
    friday: weekShift.fri.going,
    saturday: weekShift.sat.going,
    sundayStart: hoursToString(weekShift.sun.start),
    sundayFinish: hoursToString(weekShift.sun.finish),
    mondayStart: hoursToString(weekShift.mon.start),
    mondayFinish: hoursToString(weekShift.mon.finish),
    tuesdayStart: hoursToString(weekShift.tue.start),
    tuesdayFinish: hoursToString(weekShift.tue.finish),
    wednesdayStart: hoursToString(weekShift.wed.start),
    wednesdayFinish: hoursToString(weekShift.wed.finish),
    thursdayStart: hoursToString(weekShift.thu.start),
    thursdayFinish: hoursToString(weekShift.thu.finish),
    fridayStart: hoursToString(weekShift.fri.start),
    fridayFinish: hoursToString(weekShift.fri.finish),
    saturdayStart: hoursToString(weekShift.sat.start),
    saturdayFinish: hoursToString(weekShift.sat.finish),
    comments: comments.value,
  };
}

function mapFromShift() {
  const cs = props.currentShift;
  weekShift.sun = { day: 'Sun', going: cs.sunday, start: stringToHours(cs.sundayStart), finish: stringToHours(cs.sundayFinish) };
  weekShift.mon = { day: 'Mon', going: cs.monday, start: stringToHours(cs.mondayStart), finish: stringToHours(cs.mondayFinish) };
  weekShift.tue = { day: 'Tue', going: cs.tuesday, start: stringToHours(cs.tuesdayStart), finish: stringToHours(cs.tuesdayFinish) };
  weekShift.wed = { day: 'Wed', going: cs.wednesday, start: stringToHours(cs.wednesdayStart), finish: stringToHours(cs.wednesdayFinish) };
  weekShift.thu = { day: 'Thu', going: cs.thursday, start: stringToHours(cs.thursdayStart), finish: stringToHours(cs.thursdayFinish) };
  weekShift.fri = { day: 'Fri', going: cs.friday, start: stringToHours(cs.fridayStart), finish: stringToHours(cs.fridayFinish) };
  weekShift.sat = { day: 'Sat', going: cs.saturday, start: stringToHours(cs.saturdayStart), finish: stringToHours(cs.saturdayFinish) };
  comments.value = cs.comments;
}

function updateModel() {
  emit('updateModel', mapToShift());
}

function changeAllFrom() {
  for (const item in weekShift) {
    if (weekShift[item].going) {
      weekShift[item].start = startMain.value;
    } else {
      weekShift[item].start = zeroHour;
    }
  }
  updateModel();
}

function changeAllTo() {
  for (const item in weekShift) {
    if (weekShift[item].going) {
      weekShift[item].finish = finishMain.value;
    } else {
      weekShift[item].finish = zeroHour;
    }
  }
  updateModel();
}

function changeAll() {
  changeAllFrom();
  changeAllTo();
}

watch(() => props.currentShift, (val) => {
  if (props.isUpdate && val !== null) {
    mapFromShift();
  }
}, { immediate: true });

onMounted(() => {
  if (props.isUpdate && props.currentShift !== null) {
    mapFromShift();
  } else {
    updateModel();
  }
});
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';
@import '@/assets/scss/breakpoints';

.shifts-grid {
  display: grid;
  grid-template-columns: 120px repeat(7, 1fr);
  gap: 8px;
  align-items: start;
}

.grid-header {
  font-weight: bold;
  text-align: center;
  padding: 8px 4px;
  background-color: #f5f5f5;
  border-radius: 4px;
}

.grid-label {
  font-weight: bold;
  padding: 8px 4px;
}

.grid-cell {
  padding: 4px;
  text-align: center;
}

.grid-apply-all {
  grid-column: 1 / -1;
  margin: 10px 0;
}

.apply-all-section {
  background-color: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 6px;
  padding: 15px;
  display: flex;
  align-items: center;
  gap: 15px;
  justify-content: center;
}

.apply-all-section .button {
  margin-left: 10px;
}

.shifts-list {
  display: none;
}

@include touch {
  .shifts-grid {
    display: none;
  }

  .shifts-list {
    display: flex;
    flex-direction: column;
    border: 1px solid $gray-border;
    border-radius: 8px;
  }
}

.shift-row {
  display: grid;
  grid-template-columns: 76px 1fr 1fr;
  gap: 8px;
  align-items: center;
  padding: 8px 10px;

  & + .shift-row {
    border-top: 1px solid $gray-border;
  }
}

.shift-list-header {
  font-size: 0.75rem;
  font-weight: 700;
  color: $grey-font;
  background: $gray-bg;
  border-radius: 8px 8px 0 0;
}

.shift-off {
  grid-column: 2 / -1;
  font-size: 0.85rem;
  color: $grey-font;
}

.shift-apply-row {
  background: $gray-bg;
  border-radius: 0 0 8px 8px;
}

.shift-apply-label {
  font-size: 0.85rem;
  font-weight: 700;
  color: $navy;
}

.shift-apply-button {
  grid-column: 1 / -1;
}
</style>
