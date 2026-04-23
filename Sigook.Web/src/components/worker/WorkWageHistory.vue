<template>
  <div class="wage-container">
    <b-loading v-model="isLoading"></b-loading>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
      pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable
      v-model:current-page="serverParams.pageIndex" @page-change="onPageChange">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="payStubNumber" v-slot="tableProps">
          <i>{{ tableProps.row.payStubNumber }}</i>
          <p v-for="(company, idx) in tableProps.row.companies" :key="idx">
            {{ company }}
          </p>
        </b-table-column>
        <b-table-column field="weekEnding" label="Week Ending" v-slot="tableProps">
          {{ dateMonth(tableProps.row.weekEnding) }}
          <br />
          <i class="fz-1">From: {{ dateMonth(tableProps.row.start) }}</i>
          <br />
          <i class="fz-1">To: {{ dateMonth(tableProps.row.end) }}</i>
        </b-table-column>
        <b-table-column field="items" v-slot="tableProps">
          <table class="no-border-bottom">
            <thead>
              <tr>
                <th width="120px">Description</th>
                <th width="80px">Qty</th>
                <th width="80px">Total</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in tableProps.row.items" :key="idx">
                <td width="120px">{{ item.description }}</td>
                <td width="80px">{{ item.quantity }}</td>
                <td width="80px">{{ currency(item.total) }}</td>
              </tr>
              <tr>
                <td width="120px">Total:</td>
                <td width="80px">{{ getTotalQuantity(tableProps.row.items) }}</td>
                <td width="80px">{{ currency(getTotal(tableProps.row.items)) }}</td>
              </tr>
            </tbody>
          </table>
        </b-table-column>
        <b-table-column field="vacations" label="Vacations" v-slot="tableProps">
          {{ currency(tableProps.row.vacations) }}
        </b-table-column>
        <b-table-column field="totalEarnings" label="Total Earnings" v-slot="tableProps">
          {{ currency(tableProps.row.totalEarnings) }}
        </b-table-column>
        <b-table-column field="totalPaid" label="Total Paid" v-slot="tableProps">
          <p>{{ currency(tableProps.row.totalPaid) }}</p>
        </b-table-column>
        <b-table-column field="actions" v-slot="tableProps">
          <b-tooltip type="is-light" :triggers="['click']" :auto-close="['outside', 'escape']"
            @open="getAccumulated(tableProps.row)" @close="rowDetail = {}" append-to-body>
            <template v-slot:content>
              <div><strong>Qty: </strong>{{ rowDetail.quantity }}</div>
              <div><strong>Total: </strong>{{ currency(rowDetail.total) }}</div>
              <div><strong>Vacations: </strong> {{ currency(rowDetail.vacations) }}</div>
              <div><strong>Total Earnings: </strong>{{ currency(rowDetail.totalEarnings) }}</div>
              <div><strong>Total Paid: </strong>{{ currency(rowDetail.totalPaid) }}</div>
            </template>
            <b-button type="is-info" outlined rounded label="Accumulated" />
          </b-tooltip>
        </b-table-column>
      </template>
    </b-table>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from '@/utils/toast';
import { dateMonth, currency } from '@/utils/filters';
import { getWorkerProfileWageHistory as apiGetWorkerProfileWageHistory, getWorkerProfileWageHistoryAccumulated } from '@/api/workerApi';

const props = defineProps<{ workerId?: any }>();

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const serverParams = reactive({
  profileId: props.workerId,
  sortBy: 3,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
});
const rowDetail = ref<any>({});

function onPageChange(params: number) {
  serverParams.pageIndex = params;
  fetchWageHistory();
}

function fetchWageHistory() {
  isLoading.value = true;
  apiGetWorkerProfileWageHistory(serverParams)
    .then((response) => {
      isLoading.value = false;
      rows.value = response.items.map((c: any) => ({ ...c, actions: null }));
      totalItems.value = response.totalItems;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function getTotalQuantity(items: any[]) {
  const total = items.reduce((acc, item) => acc + item.quantity, 0);
  return total;
}

function getTotal(items: any[]) {
  const total = items.reduce((acc, item) => acc + item.total, 0);
  return total;
}

function getAccumulated(row: any) {
  getWorkerProfileWageHistoryAccumulated(props.workerId, row.rowNumber)
    .then((response) => (rowDetail.value = response))
    .catch((error) => showAlertError(error));
}

fetchWageHistory();
</script>
