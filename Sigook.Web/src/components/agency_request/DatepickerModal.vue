<template>
  <div class="p-3">
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field label="Start Working">
          <b-datepicker v-model="localStartWorking" name="start" inline :focused-date="today" :min-date="minDate">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="bookWorker">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import dayjs from "dayjs";

export default {
  props: ['startWorking'],
  data() {
    return {
      today: new Date(),
      localStartWorking: this.startWorking ? new Date(this.startWorking) : dayjs().toDate(),
    }
  },
  watch: {
    startWorking(newVal) {
      this.localStartWorking = newVal ? new Date(newVal) : dayjs().toDate();
    }
  },
  methods: {
    bookWorker() {
      this.$emit('update:startWorking', this.localStartWorking);
      this.$emit('onSelectCalendar', this.localStartWorking)
    }
  },
  computed: {
    minDate() {
      return dayjs().subtract(1, 'month').toDate();
    }
  },
  created() {
    if (!this.startWorking) {
      this.localStartWorking = dayjs().toDate();
    }
  }
}
</script>