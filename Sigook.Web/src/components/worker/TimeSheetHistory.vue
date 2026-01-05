<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
      pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable
      :current-page.sync="serverParams.pageIndex" @page-change="onPageChange">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <b-table-column field="businessName" label="Company" v-slot="props">
        {{ props.row.businessName }}
      </b-table-column>
      <b-table-column field="numberId" label="Request" v-slot="props">
        {{ props.row.numberId }}
        <i class="fz-2 block"> {{ props.row.jobTitle }}</i>
      </b-table-column>
      <b-table-column field="date" label="Date" v-slot="props">
        {{ props.row.date | dateMonth }} <i v-if="props.row.isHoliday" class="holiday-text">Holiday</i>
      </b-table-column>
      <b-table-column field="regularHours" label="Regular" v-slot="props">
        {{ props.row.regularHours | hour }}
      </b-table-column>
      <b-table-column field="holidayHours" label="Holiday" v-slot="props">
        {{ props.row.holidayHours }}
      </b-table-column>
      <b-table-column field="overtimeHours" label="Overtime" v-slot="props">
        {{ props.row.overtimeHours }}
      </b-table-column>
      <b-table-column field="missingHours" label="Missing" v-slot="props">
        {{ props.row.missingHours }}
      </b-table-column>
      <b-table-column field="missingHoursOvertime" label="Missing Overtime" v-slot="props">
        {{ props.row.missingHoursOvertime }}
      </b-table-column>
      <b-table-column field="totalHours" label="Total" v-slot="props">
        {{ props.row.totalHours | hour }}
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <b-tooltip type="is-light" :triggers="['click']" :auto-close="['outside', 'escape']"
          @open="getAccumulated(props.row)" @close="rowDetail = {}">
          <template v-slot:content>
            <div><strong>Regular: </strong>{{ rowDetail.regularHours }}</div>
            <div><strong>Holiday: </strong>{{ rowDetail.holidayHours }}</div>
            <div><strong>Overtime: </strong>{{ rowDetail.overtimeHours }}</div>
            <div><strong>Missing: </strong>{{ rowDetail.missingHours }}</div>
            <div><strong>Missing Overtime: </strong>{{ rowDetail.missingHoursOvertime }}</div>
            <div><strong>Total: </strong>{{ rowDetail.totalHours }}</div>
          </template>
          <b-button type="is-info" outlined rounded label="Accumulated" />
        </b-tooltip>
      </b-table-column>
    </b-table>
  </div>
</template>
<script>
export default {
  props: ['workerId'],
  data() {
    return {
      isLoading: false,
      totalItems: 0,
      rows: [],
      serverParams: {
        profileId: this.workerId,
        sortBy: 3,
        isDescending: true,
        pageIndex: 1,
        pageSize: 30
      },
      rowDetail: {}
    }
  },
  methods: {
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getWorkerProfileTimeSheetHistory();
    },
    getWorkerProfileTimeSheetHistory() {
      this.isLoading = true;
      this.$store.dispatch('worker/getWorkerProfileTimeSheetHistory', this.serverParams)
        .then(response => {
          this.isLoading = false;
          this.rows = response.items.map(c => ({ ...c, actions: null }));
          this.totalItems = response.totalItems;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    getAccumulated(row) {
      this.$store.dispatch('worker/getWorkerProfileTimeSheetHistoryAccumulated', { profileId: this.workerId, rowNumber: row.rowNumber })
        .then((response) => this.rowDetail = response)
        .catch((error) => this.showAlertError(error));
    },
  },
  created() {
    this.getWorkerProfileTimeSheetHistory();
  }
}
</script>