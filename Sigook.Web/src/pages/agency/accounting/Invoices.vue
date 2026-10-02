<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Invoices" :count="totalItems" :crumbs="accountingCrumbs">
      <b-tag size="is-medium"><b>{{ currency(total) }}</b></b-tag>
    </PageHeader>
    <div>
      <SigookGrid ref="grid" :fetch="loadInvoices" v-model:params="serverParams" :sort-map="sortMap"
        :export="{ url: '/api/agency/accounting/Invoices/file', fileName: 'Invoices' }" focusable
        @update:loading="(value) => isLoading = value" @loaded="(count) => totalItems = count">
        <template #actions>
          <b-button tag="router-link" to="/accounting/invoices/create" icon-left="plus">
            Create
          </b-button>
        </template>
        <b-table-column field="invoiceNumber" label="Invoice Number" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.invoiceNumber" placeholder="Search..." icon="magnify" size="is-small"></b-input>
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
            <b-input v-model="serverParams.companyFullName" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.companyFullName }}
          </template>
        </b-table-column>
        <b-table-column field="salesRepresentative" label="Sales Rep" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.salesRepresentative" placeholder="Search..." icon="magnify" size="is-small"></b-input>
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
      </SigookGrid>
    </div>

    <b-modal custom-content-class="card" v-model="showDeleteModal" width="800px">
      <delete-invoice v-if="currentInvoice" :invoice="currentInvoice" @deleted="onDeleteInvoice" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="showSendEmailModal" width="500px">
      <send-invoice-email v-if="currentInvoice" :invoice="currentInvoice" @sent="onSendInvoiceEmail" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { useAgencyStore } from '@/stores/agency';
import { showAlertError } from '@/utils/toast';
import { downloadPDF } from '@/utils/downloadFile';
import { getAgencyInvoices, downloadInvoicePdf } from '@/api/agencyInvoiceApi';
import { currency, dateMonth } from '@/utils/filters';
import SigookGrid from '@/components/SigookGrid.vue';
import DeleteInvoice from '@/components/agency_accounting/DeleteInvoice.vue';
import SendInvoiceEmail from '@/components/agency_accounting/SendInvoiceEmail.vue';
import PageHeader from '@/components/PageHeader.vue';
import { accountingCrumbs } from '@/constants/breadcrumbs';
import type { AgencyInvoiceFilter, AgencyInvoiceListItem } from '@/types/accounting';
import type { GridHandle, PaginatedList } from '@/types/common';

const agencyStore = useAgencyStore();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  invoiceNumber: 0,
  createdAt: 1,
  companyFullName: 2,
  salesRepresentative: 3,
};

const isLoading = ref(true);
const totalItems = ref(0);
const total = ref(0);
const createdAtDatesSelected = ref<Date[]>([]);
const serverParams = ref<AgencyInvoiceFilter>(agencyStore.agencyInvoiceFilter ?? {
  sortBy: 0,
  isDescending: true,
});

const showDeleteModal = ref(false);
const currentInvoice = ref<AgencyInvoiceListItem | null>(null);
const showSendEmailModal = ref(false);

if (serverParams.value.createdAtFrom && serverParams.value.createdAtTo) {
  createdAtDatesSelected.value[0] = new Date(serverParams.value.createdAtFrom);
  createdAtDatesSelected.value[1] = new Date(serverParams.value.createdAtTo);
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function loadInvoices(params: AgencyInvoiceFilter): Promise<PaginatedList<AgencyInvoiceListItem>> {
  agencyStore.updateAgencyInvoiceFilter(params);
  return getAgencyInvoices(params)
    .then((response) => {
      total.value = response.total;
      return response.detail;
    });
}

function onDownloadInvoicePdf(invoice: AgencyInvoiceListItem) {
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

function openSendEmailModal(invoice: AgencyInvoiceListItem) {
  currentInvoice.value = invoice;
  showSendEmailModal.value = true;
}

function onSendInvoiceEmail() {
  showSendEmailModal.value = false;
  grid.value?.reload();
}

function openDeleteModal(invoice: AgencyInvoiceListItem) {
  currentInvoice.value = invoice;
  showDeleteModal.value = true;
}

function onDeleteInvoice() {
  showDeleteModal.value = false;
  grid.value?.reload();
}
</script>
