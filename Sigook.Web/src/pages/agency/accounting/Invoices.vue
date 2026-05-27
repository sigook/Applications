<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-2">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Invoices
        <span class="fw-100 fz-1">
          ({{ totalItems }})
        </span>
        <b-tag size="is-medium"><b>{{ currency(total) }}</b></b-tag>
      </h2>
    </div>
    <div>
      <export :url="'/api/agency/accounting/Invoices/file'" :params="serverParams" :fileName="'Invoices'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button tag="router-link" to="/accounting/create-invoice" icon-left="plus">
            Create
          </b-button>
        </template>
      </export>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="invoiceNumber"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="invoiceNumber" label="Invoice Number" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.invoiceNumber" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered">
              </b-input>
            </template>
            <template v-slot="props">
              {{ props.row.invoiceNumber }}
            </template>
          </b-table-column>
          <b-table-column field="createdAt" label="Created At (From - To)" sortable searchable>
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
          <b-table-column field="companyFullName" label="Company" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.companyFullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered">
              </b-input>
            </template>
            <template v-slot="props">
              {{ props.row.companyFullName }}
            </template>
          </b-table-column>
          <b-table-column field="salesRepresentative" label="Sales Rep" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.salesRepresentative" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered">
              </b-input>
            </template>
            <template v-slot="props">
              {{ props.row.salesRepresentative }}
            </template>
          </b-table-column>
          <b-table-column field="totalNet" label="Total">
            <template v-slot="props">
              {{ currency(props.row.totalNet) }}
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-field>
              <b-tooltip label="Download" type="is-dark" position="is-top" append-to-body>
                <b-button type="is-success" outlined rounded icon-right="file-multiple" class="mr-2"
                  @click="onDownloadInvoicePdf(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip label="Send Email" type="is-dark" position="is-top" append-to-body>
                <b-button type="is-info" outlined rounded icon-right="email" class="mr-2"
                  @click="openSendEmailModal(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                <b-button type="is-danger" outlined rounded icon-right="delete" @click="openDeleteModal(props.row)">
                </b-button>
              </b-tooltip>
            </b-field>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <b-modal v-model="showDeleteModal" width="800px">
      <delete-invoice v-if="currentInvoice" :invoice="currentInvoice" @deleted="onDeleteInvoice" />
    </b-modal>

    <b-modal v-model="showSendEmailModal" width="500px">
      <send-invoice-email v-if="currentInvoice" :invoice="currentInvoice" @sent="onSendInvoiceEmail" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useAgencyStore } from '@/stores/agency';
import { showAlertError } from '@/utils/toast';
import { downloadPDF } from '@/utils/downloadFile';
import { getAgencyInvoices, downloadInvoicePdf } from '@/api/agencyInvoiceApi';
import { currency, dateMonth } from '@/utils/filters';
import Export from '@/components/Export.vue';
import DeleteInvoice from '@/components/agency_accounting/DeleteInvoice.vue';
import SendInvoiceEmail from '@/components/agency_accounting/SendInvoiceEmail.vue';

const agencyStore = useAgencyStore();

const isLoading = ref(true);
const totalItems = ref(0);
const total = ref(0);
const rows = ref<any[]>([]);
const createdAtDatesSelected = ref<any[]>([]);
const serverParams = ref<any>({
  sortBy: 0,
  pageIndex: 1,
  pageSize: 30,
  isDescending: true,
});
const showDeleteModal = ref(false);
const currentInvoice = ref<any>(null);
const showSendEmailModal = ref(false);

if (agencyStore.agencyInvoiceFilter) {
  serverParams.value = agencyStore.agencyInvoiceFilter;
  if (serverParams.value.createdAtFrom && serverParams.value.createdAtTo) {
    createdAtDatesSelected.value[0] = serverParams.value.createdAtFrom;
    createdAtDatesSelected.value[1] = serverParams.value.createdAtTo;
  }
}
loadInvoices();

function onPageChange(params: number) {
  serverParams.value.pageIndex = params;
  loadInvoices();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'invoiceNumber':
      serverParams.value.sortBy = 0;
      break;
    case 'createdAt':
      serverParams.value.sortBy = 1;
      break;
    case 'companyFullName':
      serverParams.value.sortBy = 2;
      break;
    case 'salesRepresentative':
      serverParams.value.sortBy = 3;
      break;
  }
  serverParams.value.isDescending = order !== 'asc';
  loadInvoices();
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    loadInvoices();
  }
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0];
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1];
  loadInvoices();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function loadInvoices() {
  isLoading.value = true;
  agencyStore.updateAgencyInvoiceFilter(serverParams.value);
  getAgencyInvoices(serverParams.value)
    .then((response: any) => {
      rows.value = response.detail.items.map((i: any) => ({ ...i, actions: null }));
      totalItems.value = response.detail.totalItems;
      total.value = response.total;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function onDownloadInvoicePdf(invoice: any) {
  isLoading.value = true;
  downloadInvoicePdf(invoice.id)
    .then((response) => {
      isLoading.value = false;
      downloadPDF(response, `${invoice.invoiceNumber} ${invoice.companyFullName}`);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function openSendEmailModal(invoice: any) {
  currentInvoice.value = invoice;
  showSendEmailModal.value = true;
}

function onSendInvoiceEmail() {
  showSendEmailModal.value = false;
  loadInvoices();
}

function openDeleteModal(invoice: any) {
  currentInvoice.value = invoice;
  showDeleteModal.value = true;
}

function onDeleteInvoice() {
  showDeleteModal.value = false;
  loadInvoices();
}
</script>
