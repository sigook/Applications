<template>
  <div class="mt-4 w-100">
    <div class="fz-1 fw-700 mb-3">
      Shift
    </div>
    <div class="shifts-grid">

      <div class="grid-header">Day</div>
      <div v-for="(item, dayKey) in weekShift" :key="'header-' + dayKey" class="grid-header">
        <b-checkbox size="is-small" v-model="item.going" @input="updateModel">
          {{ item.day }}
        </b-checkbox>
      </div>

      <div class="grid-label">
        <b-button @click="showStart = !showStart" type="is-ghost" size="is-small" icon-right="menu-down">
          From
        </b-button>
      </div>
      <div v-for="(item, dayKey) in weekShift" :key="'from-' + dayKey" class="grid-cell">
        <b-timepicker size="is-small" v-if="item.going" v-model="item.start" hour-format="24" @input="updateModel">
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
        <b-timepicker size="is-small" v-if="item.going" v-model="item.finish" hour-format="24" @input="updateModel">
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
      <b-field :type="errors.has('description') ? 'is-danger' : ''" label="Comments"
        :message="errors.has('description') ? errors.first('description') : ''">
        <b-input type="textarea" v-model="comments" name="description" @input="updateModel" />
      </b-field>
    </div>
  </div>
</template>

<script>
import dayjs from "dayjs";

export default {
  props: ['currentShift', 'isUpdate'],
  data() {
    let start = new Date()

    start.setMinutes(0);
    start.setHours(8);
    start.setSeconds(0);

    let finish = new Date()
    finish.setMinutes(0);
    finish.setHours(17);
    finish.setSeconds(0);

    let zeroHour = new Date();
    zeroHour.setMinutes(0);
    zeroHour.setHours(0);
    zeroHour.setSeconds(0);

    return {
      zeroHour: zeroHour,
      startMain: start,
      showStart: false,
      finishMain: finish,
      showFinish: false,
      weekShift: {
        sun: {
          day: 'Sun',
          going: null,
          start: zeroHour,
          finish: zeroHour
        },
        mon: {
          day: 'Mon',
          going: true,
          start: start,
          finish: finish
        },
        tue: {
          day: 'Tue',
          going: true,
          start: start,
          finish: finish
        },
        wed: {
          day: 'Wed',
          going: true,
          start: start,
          finish: finish
        },
        thu: {
          day: 'Thu',
          going: true,
          start: start,
          finish: finish
        },
        fri: {
          day: 'Fri',
          going: true,
          start: start,
          finish: finish
        },
        sat: {
          day: 'Sat',
          going: null,
          start: zeroHour,
          finish: zeroHour
        }
      },
      comments: null
    }
  },
  watch: {
    currentShift: {
      handler(val) {
        if (this.isUpdate && val !== null) {
          this.mapFromShift();
        }
      },
      immediate: true,
      deep: false
    }
  },
  methods: {
    changeAllFrom() {
      for (const item in this.weekShift) {
        if (this.weekShift[item].going) {
          this.weekShift[item].start = this.startMain
        } else {
          this.weekShift[item].start = this.zeroHour
        }
      }
      this.updateModel()
    },
    changeAllTo() {
      for (const item in this.weekShift) {
        if (this.weekShift[item].going) {
          this.weekShift[item].finish = this.finishMain
        } else {
          this.weekShift[item].finish = this.zeroHour
        }
      }
      this.updateModel()
    },
    updateModel() {
      this.$emit("updateModel", this.mapToShift());
    },
    mapToShift() {
      return {
        sunday: this.weekShift.sun.going,
        monday: this.weekShift.mon.going,
        tuesday: this.weekShift.tue.going,
        wednesday: this.weekShift.wed.going,
        thursday: this.weekShift.thu.going,
        friday: this.weekShift.fri.going,
        saturday: this.weekShift.sat.going,
        sundayStart: this.hoursToString(this.weekShift.sun.start),
        sundayFinish: this.hoursToString(this.weekShift.sun.finish),
        mondayStart: this.hoursToString(this.weekShift.mon.start),
        mondayFinish: this.hoursToString(this.weekShift.mon.finish),
        tuesdayStart: this.hoursToString(this.weekShift.tue.start),
        tuesdayFinish: this.hoursToString(this.weekShift.tue.finish),
        wednesdayStart: this.hoursToString(this.weekShift.wed.start),
        wednesdayFinish: this.hoursToString(this.weekShift.wed.finish),
        thursdayStart: this.hoursToString(this.weekShift.thu.start),
        thursdayFinish: this.hoursToString(this.weekShift.thu.finish),
        fridayStart: this.hoursToString(this.weekShift.fri.start),
        fridayFinish: this.hoursToString(this.weekShift.fri.finish),
        saturdayStart: this.hoursToString(this.weekShift.sat.start),
        saturdayFinish: this.hoursToString(this.weekShift.sat.finish),
        comments: this.comments
      }
    },
    mapFromShift() {
      this.weekShift = {
        sun: {
          day: 'Sun',
          going: this.currentShift.sunday,
          start: this.stringToHours(this.currentShift.sundayStart),
          finish: this.stringToHours(this.currentShift.sundayFinish)
        },
        mon: {
          day: 'Mon',
          going: this.currentShift.monday,
          start: this.stringToHours(this.currentShift.mondayStart),
          finish: this.stringToHours(this.currentShift.mondayFinish)
        },
        tue: {
          day: 'Tue',
          going: this.currentShift.tuesday,
          start: this.stringToHours(this.currentShift.tuesdayStart),
          finish: this.stringToHours(this.currentShift.tuesdayFinish)
        },
        wed: {
          day: 'Wed',
          going: this.currentShift.wednesday,
          start: this.stringToHours(this.currentShift.wednesdayStart),
          finish: this.stringToHours(this.currentShift.wednesdayFinish)
        },
        thu: {
          day: 'Thu',
          going: this.currentShift.thursday,
          start: this.stringToHours(this.currentShift.thursdayStart),
          finish: this.stringToHours(this.currentShift.thursdayFinish)
        },
        fri: {
          day: 'Fri',
          going: this.currentShift.friday,
          start: this.stringToHours(this.currentShift.fridayStart),
          finish: this.stringToHours(this.currentShift.fridayFinish)
        },
        sat: {
          day: 'Sat',
          going: this.currentShift.saturday,
          start: this.stringToHours(this.currentShift.saturdayStart),
          finish: this.stringToHours(this.currentShift.saturdayFinish)
        },
      }
      this.comments = this.currentShift.comments
    },
    hoursToString(hour) {
      return dayjs(hour).format('HH:mm:ss').toString();
    },
    stringToHours(time) {
      if (time) {
        const [hours, minutes, seconds] = time.split(':').map(Number);
        return dayjs().startOf('day')
          .add(hours, 'hour')
          .add(minutes, 'minute')
          .add(seconds || 0, 'second').toDate();
      }
      return this.zeroHour;
    }
  },
  mounted() {
    if (this.isUpdate && this.currentShift !== null) {
      this.mapFromShift();
    } else {
      this.updateModel();
    }
  }
}
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