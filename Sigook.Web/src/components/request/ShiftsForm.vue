<template>
  <div class="mt-4 w-100">
    <div class="fz-1 fw-bold mb-3">
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

    <div class="col-sm-12 col-md-12 col-lg-12 col-padding pt-1">
      <b-field label="Comments">
        <b-input type="textarea" v-model="comments" name="description" @update:modelValue="updateModel" />
      </b-field>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, watch, onMounted } from 'vue';
import dayjs from "dayjs";

const props = defineProps<{ currentShift?: any; isUpdate?: boolean }>();
const emit = defineEmits<{ (e: 'updateModel', shift: any): void }>();

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

const weekShift = reactive<Record<string, { day: string; going: any; start: Date; finish: Date }>>({
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

function mapToShift() {
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

<style scoped>
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

/* Mobile responsive */
@media (max-width: 768px) {
  .shifts-grid {
    grid-template-columns: 1fr;
    gap: 15px;
  }

  .grid-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 10px;
    background-color: transparent;
    border-bottom: 1px solid #dbdbdb;
    padding-bottom: 8px;
  }

  .grid-label {
    font-size: 1.1em;
    margin-bottom: 10px;
    border-bottom: 1px solid #dbdbdb;
    padding-bottom: 5px;
  }

  .grid-cell {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 8px;
    border: 1px solid #f0f0f0;
    border-radius: 4px;
    margin-bottom: 5px;
  }

  .apply-all-section {
    flex-direction: column;
    gap: 10px;
  }

  .apply-all-section .button {
    margin-left: 0;
    width: 100%;
  }
}
</style>
