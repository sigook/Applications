<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Invoices" :count="totalItems" :crumbs="accountingCrumbs">
      <b-tag size="is-medium"><b>{{ currency(total) }}</b></b-tag>
    </PageHeader>
    <div>
      <SigookGrid ref="grid" :fetch="loadInvoices" v-model:params="serverParams" :sort-map="sortMap"
        :export="{ url: '/api/agency/accounting/invoices/file', fileName: 'Invoices' }" focusable
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
        <b-table-column field="status" label="Status" sortable searchable>
          <template v-slot:searchable>
            <b-select v-model="serverParams.status" size="is-small" expanded @update:modelValue="onStatusChange">
              <option :value="null">All</option>
              <option v-for="s in INVOICE_STATUSES" :key="s" :value="s">{{ INVOICE_STATUS_LABELS[s] }}</option>
            </b-select>
          </template>
          <template v-slot="props">
            <b-tooltip v-if="props.row.updatedAt" :label="statusTooltip(props.row)" type="is-dark" position="is-top"
              append-to-body>
              <b-tag :type="invoiceStatusTagType(props.row.status)">{{ INVOICE_STATUS_LABELS[props.row.status] }}</b-tag>
            </b-tooltip>
            <b-tag v-else :type="invoiceStatusTagType(props.row.status)">{{ INVOICE_STATUS_LABELS[props.row.status] }}</b-tag>
          </template>
        </b-table-column>
        <b-table-column field="totalNet" label="Total">
          <template v-slot="props">
            {{ currency(props.row.totalNet) }}
          </template>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
            <template #trigger>
              <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
            </template>
            <b-dropdown-item aria-role="listitem" @click="onDownloadInvoicePdf(props.row)">Download</b-dropdown-item>
            <b-dropdown-item aria-role="listitem" @click="openSendEmailModal(props.row)">Send Email</b-dropdown-item>
            <b-dropdown-item aria-role="listitem" @click="onToggleStatus(props.row)">
              {{ props.row.status === InvoiceStatus.Paid ? 'Mark as Pending' : 'Mark as Paid' }}
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" class="has-text-danger" @click="openDeleteModal(props.row)">
              Delete
            </b-dropdown-item>
          </b-dropdown>
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
import { useAgencyStore } from '@/modules/agency/store';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { downloadPDF } from '@/shared/utils/downloadFile';
import { getAgencyInvoices, downloadInvoicePdf, changeInvoiceStatus } from '@/modules/agency/accounting/invoices/api';
import { currency, dateMonth } from '@/shared/format';
import { InvoiceStatus, INVOICE_STATUSES, INVOICE_STATUS_LABELS, invoiceStatusTagType } from '@/modules/agency/accounting/invoices/types';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import DeleteInvoice from '@/modules/agency/accounting/invoices/components/DeleteInvoice.vue';
import SendInvoiceEmail from '@/modules/agency/accounting/invoices/components/SendInvoiceEmail.vue';
import PageHeader from '@/shared/ui/PageHeader.vue';
import { accountingCrumbs } from '@/shared/ui/breadcrumbs';
import type { AgencyInvoiceFilter, AgencyInvoiceListItem } from '@/modules/agency/accounting/invoices/types';
import type { GridHandle, PaginatedList } from '@/shared/types/common';

const agencyStore = useAgencyStore();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  invoiceNumber: 0,
  createdAt: 1,
  companyFullName: 2,
  salesRepresentative: 3,
  status: 4,
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

function onStatusChange() {
  grid.value?.search();
}

function statusTooltip(invoice: AgencyInvoiceListItem): string {
  const who = invoice.updatedByName ? `${invoice.updatedByName} · ` : '';
  return `${who}${dateMonth(invoice.updatedAt ?? null)}`;
}

async function onToggleStatus(invoice: AgencyInvoiceListItem) {
  const next = invoice.status === InvoiceStatus.Paid ? InvoiceStatus.Pending : InvoiceStatus.Paid;
  const label = INVOICE_STATUS_LABELS[next];
  const confirmed = await showAlertConfirm(`Mark as ${label}`, `Invoice <b>${invoice.invoiceNumber}</b> will be marked as ${label}.`, 'Confirm');
  if (!confirmed) return;
  isLoading.value = true;
  try {
    await changeInvoiceStatus(invoice.id, { status: next });
    showAlertSuccess(`Invoice marked as ${label}`);
    grid.value?.reload();
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
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
