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
        <template #dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="onDownloadPaymentsReport">
            <b-icon icon="file-excel"></b-icon>
            <span>Payments report</span>
          </b-dropdown-item>
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
              :icon-right="dateRanges.createdAt.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onDateRangeCleared('createdAt')" range v-model="dateRanges.createdAt"
              @update:modelValue="onDateRangeSelected('createdAt')" append-to-body>
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
            <b-tooltip :label="statusTooltip(props.row)" :active="!!props.row.updatedByName" type="is-dark" position="is-top"
              multilined append-to-body>
              <b-tag :type="invoiceStatusTagType(props.row.status)">{{ INVOICE_STATUS_LABELS[props.row.status] }}</b-tag>
            </b-tooltip>
          </template>
        </b-table-column>
        <b-table-column field="dueDate" label="Due Date (From - To)" searchable>
          <template v-slot:searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
              :icon-right="dateRanges.dueDate.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onDateRangeCleared('dueDate')" range v-model="dateRanges.dueDate"
              @update:modelValue="onDateRangeSelected('dueDate')" append-to-body>
            </b-datepicker>
          </template>
          <template v-slot="props">
            <template v-if="props.row.status !== InvoiceStatus.Paid">
              {{ dateMonth(props.row.dueDate) }}
              <p class="is-size-7">
                <span class="has-text-grey">Net {{ props.row.paymentTermDays }}</span>
                <span v-if="props.row.dueDays > 0" class="has-text-danger has-text-weight-semibold">
                  · {{ props.row.dueDays }} {{ props.row.dueDays === 1 ? 'day' : 'days' }} due
                </span>
              </p>
            </template>
          </template>
        </b-table-column>
        <b-table-column field="paidAt" label="Payment Date (From - To)" searchable>
          <template v-slot:searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
              :icon-right="dateRanges.paidAt.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onDateRangeCleared('paidAt')" range v-model="dateRanges.paidAt"
              @update:modelValue="onDateRangeSelected('paidAt')" append-to-body>
            </b-datepicker>
          </template>
          <template v-slot="props">
            {{ dateMonth(props.row.paidAt ?? null) }}
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
            <b-dropdown-item v-if="props.row.pdfUrl" aria-role="listitem" @click="onRegenerateInvoicePdf(props.row)">
              Regenerate PDF
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" @click="openSendEmailModal(props.row)">Send Email</b-dropdown-item>
            <template v-if="props.row.status === InvoiceStatus.Paid">
              <b-dropdown-item aria-role="listitem" @click="openMarkPaidModal(props.row)">
                Edit Payment Date
              </b-dropdown-item>
              <b-dropdown-item aria-role="listitem" @click="onMarkAsPending(props.row)">
                Mark as Pending
              </b-dropdown-item>
            </template>
            <b-dropdown-item v-else aria-role="listitem" @click="openMarkPaidModal(props.row)">
              Mark as Paid
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

    <b-modal has-modal-card v-model="showMarkPaidModal" width="460px" :destroy-on-hide="true">
      <mark-invoice-paid-modal v-if="currentInvoice" :invoice="currentInvoice" @updated="onStatusUpdated"
        @close="showMarkPaidModal = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { useAgencyStore } from '@/modules/agency/store';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { downloadFile, downloadPDF, openFileUrl } from '@/shared/utils/downloadFile';
import { getAgencyInvoices, downloadInvoicePdf, regenerateInvoicePdf, downloadInvoicesPaymentsReport, changeInvoiceStatus } from '@/modules/agency/accounting/invoices/api';
import { currency, dateMonth } from '@/shared/format';
import { InvoiceStatus, INVOICE_STATUSES, INVOICE_STATUS_LABELS, invoiceStatusTagType } from '@/modules/agency/accounting/invoices/types';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import DeleteInvoice from '@/modules/agency/accounting/invoices/components/DeleteInvoice.vue';
import SendInvoiceEmail from '@/modules/agency/accounting/invoices/components/SendInvoiceEmail.vue';
import MarkInvoicePaidModal from '@/modules/agency/accounting/invoices/components/MarkInvoicePaidModal.vue';
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
const serverParams = ref<AgencyInvoiceFilter>(agencyStore.agencyInvoiceFilter ?? {
  sortBy: 0,
  isDescending: true,
});

const showDeleteModal = ref(false);
const currentInvoice = ref<AgencyInvoiceListItem | null>(null);
const showSendEmailModal = ref(false);
const showMarkPaidModal = ref(false);

const dateRangeKeys = {
  createdAt: ['createdAtFrom', 'createdAtTo'],
  dueDate: ['dueDateFrom', 'dueDateTo'],
  paidAt: ['paidAtFrom', 'paidAtTo'],
} as const satisfies Record<string, readonly [keyof AgencyInvoiceFilter, keyof AgencyInvoiceFilter]>;
type DateRangeKey = keyof typeof dateRangeKeys;

function initialRange(key: DateRangeKey): Date[] {
  const [fromKey, toKey] = dateRangeKeys[key];
  const from = serverParams.value[fromKey];
  const to = serverParams.value[toKey];
  return from && to ? [new Date(from), new Date(to)] : [];
}

const dateRanges = ref<Record<DateRangeKey, Date[]>>({
  createdAt: initialRange('createdAt'),
  dueDate: initialRange('dueDate'),
  paidAt: initialRange('paidAt'),
});

function onDateRangeSelected(key: DateRangeKey) {
  const [fromKey, toKey] = dateRangeKeys[key];
  const [from, to] = dateRanges.value[key];
  serverParams.value[fromKey] = from?.toISOString() ?? null;
  serverParams.value[toKey] = to?.toISOString() ?? null;
  grid.value?.search();
}

function onDateRangeCleared(key: DateRangeKey) {
  dateRanges.value[key] = [];
  onDateRangeSelected(key);
}

function onStatusChange() {
  grid.value?.search();
}

function statusTooltip(invoice: AgencyInvoiceListItem): string {
  return `Updated: ${invoice.updatedByName} · ${dateMonth(invoice.updatedAt ?? null)}`;
}

async function onMarkAsPending(invoice: AgencyInvoiceListItem) {
  const confirmed = await showAlertConfirm('Mark as Pending', `Invoice <b>${invoice.invoiceNumber}</b> will be marked as Pending and its payment date cleared.`, 'Confirm');
  if (!confirmed) return;
  isLoading.value = true;
  try {
    await changeInvoiceStatus(invoice.id, { status: InvoiceStatus.Pending });
    showAlertSuccess('Invoice marked as Pending');
    grid.value?.reload();
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

function openMarkPaidModal(invoice: AgencyInvoiceListItem) {
  currentInvoice.value = invoice;
  showMarkPaidModal.value = true;
}

function onStatusUpdated() {
  grid.value?.reload();
}

function onDownloadPaymentsReport() {
  isLoading.value = true;
  downloadInvoicesPaymentsReport(serverParams.value)
    .then((file) => downloadFile(file, `Payments_${new Date().toLocaleDateString()}`))
    .catch((error) => showAlertError(error))
    .finally(() => { isLoading.value = false; });
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
  if (invoice.pdfUrl) {
    openFileUrl(invoice.pdfUrl);
    return;
  }
  renderInvoicePdf(invoice, downloadInvoicePdf);
}

function onRegenerateInvoicePdf(invoice: AgencyInvoiceListItem) {
  renderInvoicePdf(invoice, regenerateInvoicePdf);
}

function renderInvoicePdf(invoice: AgencyInvoiceListItem, render: (invoiceId: string) => Promise<Blob>) {
  isLoading.value = true;
  render(invoice.id)
    .then((response) => {
      downloadPDF(response, `${invoice.invoiceNumber} ${invoice.companyFullName}`);
      grid.value?.reload();
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
