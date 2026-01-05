<template>
  <div class="p-3">
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field label="Start Working">
          <b-datepicker v-model="startWorking" name="start" inline :focused-date="today" :min-date="minDate">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="bookWorker">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script>
import dayjs from "dayjs";

export default {
  props: ['startWorking'],
  data() {
    return {
      today: new Date(),
    }
  },
  methods: {
    bookWorker() {
      this.$emit('onSelectCalendar', this.startWorking)
    }
  },
  computed: {
    minDate() {
      return dayjs().subtract(1, 'month').toDate();
    }
  },
  created() {
    if (this.startWorking) {
      this.startWorking = dayjs().toDate();
    }
  }
}
</script>