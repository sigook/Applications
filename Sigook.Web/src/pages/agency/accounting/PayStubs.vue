<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title columns is-multiline mb-2">
      <h2 class="fz1 pt-3 column is-7-mobile is-5">
        PayStubs
      </h2>
    </div>
    <div>
      <export :url="'/api/agency/accounting/PayStubs/file'" :params="serverParams" :fileName="'PayStubs'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button tag="router-link" to="/accounting/paystubs/create" icon-left="plus">
            Create
          </b-button>
        </template>
        <template v-slot:dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="showGeneratePayStubsModal = true">
            <b-icon icon="table-plus"></b-icon>
            <span>Generate</span>
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" @click="showSkipPayrollNumberModal = true">
            <b-icon icon="step-forward"></b-icon>
            <span>Skip Payroll Number</span>
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" :disabled="checkedRows.length === 0" @click="onSendSelectedEmails">
            <b-icon icon="email-multiple"></b-icon>
            <span>Send Email</span>
          </b-dropdown-item>
        </template>
      </export>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable :default-sort="defaultSort"
        checkable checkbox-position="left" v-model:checked-rows="checkedRows"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange">
        <template v-slot:empty>
          <p class="container has-text-centered">No records available</p>
        </template>
        <template>
          <b-table-column field="payStubNumber" label="PayStub Number" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.payStubNumber" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered">
              </b-input>
            </template>
            <template v-slot="props">
              {{ props.row.payStubNumber }}
            </template>
          </b-table-column>
          <b-table-column field="createdAt" label="Created At" sortable searchable>
            <template v-slot:searchable>
              <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
                :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
                @icon-right-click="onCreatedAtCleared" range v-model="createdAtDatesSelected"
                @update:modelValue="onCreatedAtSelected" append-to-body>
              </b-datepicker>
            </template>
            <template v-slot="props">
              {{ dateMonth(props.row.createdAt) }}
            </template>
          </b-table-column>
          <b-table-column field="workerFullName" label="Worker" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.workerFullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered">
              </b-input>
            </template>
            <template v-slot="props">
              {{ props.row.workerFullName }}
            </template>
          </b-table-column>
          <b-table-column field="numberId" label="Number ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered">
              </b-input>
            </template>
            <template v-slot="props">
              {{ props.row.numberId }}
            </template>
          </b-table-column>
          <b-table-column field="totalPaid" label="Total Paid">
            <template v-slot="props">
              {{ currency(props.row.totalPaid) }}
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-field>
              <b-tooltip label="Download" type="is-dark" position="is-top" append-to-body>
                <b-button type="is-success" outlined rounded icon-right="file-multiple" class="mr-2"
                  @click="onDownloadPayStubPdf(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip :label="props.row.emailSent ? 'Email Sent' : 'Send Email'" type="is-dark" position="is-top" append-to-body>
                <b-button type="is-info" outlined rounded :icon-right="props.row.emailSent ? 'email-check' : 'email'"
                  class="mr-2" :loading="props.row.emailSending" :disabled="props.row.emailSent"
                  @click="onSendPayStubEmail(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                <b-button type="is-danger" outlined rounded icon-right="delete" @click="onDeletePayStub(props.row)">
                </b-button>
              </b-tooltip>
            </b-field>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <b-modal custom-content-class="card" v-model="showGeneratePayStubsModal" width="800px">
      <generate-pay-stubs @pay-stubs-generated="onPayStubsGenerated" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="showSkipPayrollNumberModal" width="500px">
      <skip-payroll-number></skip-payroll-number>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useAgencyStore } from '@/stores/agency';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { downloadPDF } from '@/utils/downloadFile';
import { dateMonth, currency } from '@/utils/filters';
import { getDialog } from '@/utils/buefyProgrammatic';
import {
  getAgencyPayStubs,
  downloadPayStubPdf,
  sendPayStubEmail,
  sendPayStubEmailBulk,
  deleteAgencyPayStub,
} from '@/api/agencyPayStubApi';
import { useGridSort } from '@/composables/useGridSort';
import Export from '@/components/Export.vue';
import GeneratePayStubs from '@/components/agency_accounting/GeneratePayStubs.vue';
import SkipPayrollNumber from '@/components/agency_accounting/SkipPayrollNumber.vue';

const agencyStore = useAgencyStore();

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const checkedRows = ref<any[]>([]);
const createdAtDatesSelected = ref<any[]>([]);
const serverParams = ref<any>({
  sortBy: 0,
  pageIndex: 1,
  pageSize: 30,
  isDescending: true,
});

const { defaultSort, onSortChange } = useGridSort(serverParams, {
  payStubNumber: 0,
  createdAt: 1,
  workerFullName: 2,
  numberId: 3,
  totalPaid: 4,
}, () => loadPayStubs());

const showGeneratePayStubsModal = ref(false);
const showSkipPayrollNumberModal = ref(false);

if (agencyStore.agencyPayStubFilter) {
  serverParams.value = agencyStore.agencyPayStubFilter;
  if (serverParams.value.createdAtFrom && serverParams.value.createdAtTo) {
    createdAtDatesSelected.value[0] = serverParams.value.createdAtFrom;
    createdAtDatesSelected.value[1] = serverParams.value.createdAtTo;
  }
}
loadPayStubs();

function onPageChange(params: number) {
  serverParams.value.pageIndex = params;
  loadPayStubs();
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    loadPayStubs();
  }
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0];
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1];
  loadPayStubs();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function loadPayStubs() {
  isLoading.value = true;
  agencyStore.updateAgencyPayStubFilter(serverParams.value);
  getAgencyPayStubs(serverParams.value)
    .then((response: any) => {
      rows.value = response.items.map((i: any) => ({ ...i, emailSending: false, actions: null }));
      checkedRows.value = [];
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function onDownloadPayStubPdf(payStub: any) {
  isLoading.value = true;
  downloadPayStubPdf(payStub.id)
    .then((response) => {
      isLoading.value = false;
      downloadPDF(response, `${payStub.payStubNumber} ${payStub.workerFullName}`);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function onSendPayStubEmail(payStub: any) {
  payStub.emailSending = true;
  sendPayStubEmail(payStub.id)
    .then(() => {
      payStub.emailSending = false;
      payStub.emailSent = true;
      showAlertSuccess(`Email to ${payStub.workerFullName} sent successfully`);
    })
    .catch((error) => {
      payStub.emailSending = false;
      showAlertError(error);
    });
}

function onSendSelectedEmails() {
  const count = checkedRows.value.length;
  getDialog().confirm({
    title: 'Send selected pay stubs',
    message: `You are about to email <b>${count}</b> pay stub(s). A summary will be sent to Teams when it finishes.`,
    confirmText: 'Yes, send them',
    type: 'is-info',
    hasIcon: true,
    onConfirm: () => {
      isLoading.value = true;
      const payStubIds = checkedRows.value.map((p: any) => p.id);
      sendPayStubEmailBulk(payStubIds)
        .then(() => {
          isLoading.value = false;
          checkedRows.value = [];
          showAlertSuccess(`Sending ${count} pay stub(s). A summary will arrive on Teams.`);
        })
        .catch((error) => {
          isLoading.value = false;
          showAlertError(error);
        });
    },
  });
}

function onDeletePayStub(payStub: any) {
  const message = `You are about to delete the pay stub <b>${payStub.payStubNumber}</b>
        <br>
        <br>
        If you are going to use the pay stub number <b>${payStub.payStubNumber}</b> for the same worker,
        remember that you should not generate any pay stub for any other worker before generate this pay stub again.`;
  getDialog().confirm({
    title: 'Are you sure you want to delete?',
    message: message,
    confirmText: 'Yes, I read and I want to delete',
    type: 'is-danger',
    hasIcon: true,
    onConfirm: () => {
      isLoading.value = true;
      deleteAgencyPayStub(payStub.id)
        .then(() => {
          isLoading.value = false;
          loadPayStubs();
        })
        .catch((error) => {
          isLoading.value = false;
          showAlertError(error);
        });
    },
  });
}

function onPayStubsGenerated() {
  showGeneratePayStubsModal.value = false;
  loadPayStubs();
}
</script>
