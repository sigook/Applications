<template>
  <div class="wage-container">
    <b-loading v-model="isLoading"></b-loading>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
      pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable
      :current-page.sync="serverParams.pageIndex" @page-change="onPageChange">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="payStubNumber" v-slot="props">
          <i>{{ props.row.payStubNumber }}</i>
          <p v-for="company in props.row.companies">
            {{ company }}
          </p>
        </b-table-column>
        <b-table-column field="weekEnding" label="Week Ending" v-slot="props">
          {{ props.row.weekEnding | dateMonth }}
          <br />
          <i class="fz-1">From: {{ props.row.start | dateMonth }}</i>
          <br />
          <i class="fz-1">To: {{ props.row.end | dateMonth }}</i>
        </b-table-column>
        <b-table-column field="items" v-slot="props" width="350px">
          <table class="no-border-bottom">
            <thead>
              <tr>
                <th width="120px">Description</th>
                <th width="80px">Qty</th>
                <th width="80px">Total</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in props.row.items">
                <td width="120px">{{ item.description }}</td>
                <td width="80px">{{ item.quantity }}</td>
                <td width="80px">{{ item.total | currency }}</td>
              </tr>
              <tr>
                <td width="120px">Total:</td>
                <td width="80px">{{ getTotalQuantity(props.row.items) }}</td>
                <td width="80px">{{ getTotal(props.row.items) | currency }}</td>
              </tr>
            </tbody>
          </table>
        </b-table-column>
        <b-table-column field="publicHolidays" label="Public Holidays" v-slot="props">
          {{ props.row.publicHolidays | currency }}
        </b-table-column>
        <b-table-column field="vacations" label="Vacations" v-slot="props">
          {{ props.row.vacations | currency }}
        </b-table-column>
        <b-table-column field="totalEarnings" label="Total Earnings" v-slot="props">
          {{ props.row.totalEarnings | currency }}
        </b-table-column>
        <b-table-column field="totalPaid" label="Total Paid" v-slot="props">
          <p>{{ props.row.totalPaid | currency }}</p>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-tooltip type="is-light" :triggers="['click']" :auto-close="['outside', 'escape']"
            @open="getAccumulated(props.row)" @close="rowDetail = {}">
            <template v-slot:content>
              <div><strong>Qty: </strong>{{ rowDetail.quantity }}</div>
              <div><strong>Total: </strong>{{ rowDetail.total | currency }}</div>
              <div><strong>Public Holidays: </strong>{{ rowDetail.publicHolidays | currency }}</div>
              <div><strong>Vacations: </strong> {{ rowDetail.vacations | currency }}</div>
              <div><strong>Total Earnings: </strong>{{ rowDetail.totalEarnings | currency }}</div>
              <div><strong>Total Paid: </strong>{{ rowDetail.totalPaid | currency }}</div>
            </template>
            <b-button type="is-info" outlined rounded label="Accumulated" />
          </b-tooltip>
        </b-table-column>
      </template>
    </b-table>
  </div>
</template>

<script>

export default {
  props: ["workerId"],
  data() {
    return {
      isLoading: true,
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
    };
  },
  methods: {
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getWorkerProfileWageHistory();
    },
    getWorkerProfileWageHistory() {
      this.isLoading = true;
      this.$store.dispatch("worker/getWorkerProfileWageHistory", this.serverParams)
        .then((response) => {
          this.isLoading = false;
          this.rows = response.items.map(c => ({ ...c, actions: null }));
          this.totalItems = response.totalItems;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    getTotalQuantity(items) {
      const total = items.reduce((acc, item) => acc + item.quantity, 0);
      return total;
    },
    getTotal(items) {
      const total = items.reduce((acc, item) => acc + item.total, 0);
      return total;
    },
    getAccumulated(row) {
      this.$store.dispatch('worker/getWorkerProfileWageHistoryAccumulated', { profileId: this.workerId, rowNumber: row.rowNumber })
        .then((response) => this.rowDetail = response)
        .catch((error) => this.showAlertError(error));
    },
  },
  created() {
    this.getWorkerProfileWageHistory();
  },
};
</script>