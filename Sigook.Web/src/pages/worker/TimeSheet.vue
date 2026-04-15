<template>
  <div class="white-container-mobile">
    <b-table :data="data.items || []" narrowed hoverable :mobile-cards="true">
      <template v-slot:empty>
        <p class="container text-center">{{ "No records available" }}</p>
      </template>
      <template>
        <b-table-column field="day" :label="'Day'" v-slot="props">
          <span v-if="props.row.day">{{ date(props.row.day) }}</span>
        </b-table-column>
        <b-table-column field="clockIn" label="Clock In" v-slot="props">
          <span v-if="props.row.timeIn">{{ time(props.row.clockIn) }}</span>
        </b-table-column>
        <b-table-column field="clockOut" label="Clock Out" v-slot="props">
          <span v-if="props.row.clockOut !== null">{{ time(props.row.clockOut) }}</span>
        </b-table-column>
        <b-table-column field="totalHours" :label="'Worked hours'" v-slot="props">
          {{ props.row.totalHours.toFixed(2) }}
        </b-table-column>
      </template>
    </b-table>
  </div>
</template>
<script lang="ts">
import { date, time } from '@/utils/filters';

export default {
  props: ['data'],
  methods: {
    date,
    time,
  },
}
</script>
