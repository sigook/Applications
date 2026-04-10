<template>
  <div class="white-container-mobile">
    <b-table :data="data.items || []" narrowed hoverable :mobile-cards="true">
      <template v-slot:empty>
        <p class="container text-center">{{ $t("NoRecordsAvailable") }}</p>
      </template>
      <template>
        <b-table-column field="day" :label="$t('Day')" v-slot="props">
          <span v-if="props.row.day">{{ props.row.day | date }}</span>
        </b-table-column>
        <b-table-column field="clockIn" label="Clock In" v-slot="props">
          <span v-if="props.row.timeIn">{{ props.row.clockIn | time }}</span>
        </b-table-column>
        <b-table-column field="clockOut" label="Clock Out" v-slot="props">
          <span v-if="props.row.clockOut !== null">{{ props.row.clockOut | time }}</span>
        </b-table-column>
        <b-table-column field="totalHours" :label="$t('WorkedHours')" v-slot="props">
          {{ props.row.totalHours.toFixed(2) }}
        </b-table-column>
      </template>
    </b-table>
  </div>
</template>
<script lang="ts">
export default {
  props: ['data']
}
</script>
