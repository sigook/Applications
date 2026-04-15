<template>
  <div class="white-container-mobile">
    <b-table :data="data.items || []" narrowed hoverable :mobile-cards="true">
      <template v-slot:empty>
        <p class="container text-center">{{ $t("NoRecordsAvailable") }}</p>
      </template>
      <template>
        <b-table-column field="day" :label="$t('Day')" v-slot="props">
          <span v-if="props.row.day">{{ date(props.row.day) }}</span>
        </b-table-column>
        <b-table-column field="clockIn" label="Clock In" v-slot="props">
          <span v-if="props.row.timeIn">{{ time(props.row.clockIn) }}</span>
        </b-table-column>
        <b-table-column field="clockOut" label="Clock Out" v-slot="props">
          <span v-if="props.row.clockOut !== null">{{ time(props.row.clockOut) }}</span>
        </b-table-column>
        <b-table-column field="totalHours" :label="$t('WorkedHours')" v-slot="props">
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
