<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="PayStubs" :crumbs="accountingCrumbs" />
    <div>
      <SigookGrid ref="grid" :fetch="loadPayStubs" v-model:params="serverParams" :sort-map="sortMap"
        :export="{ url: '/api/agency/accounting/paystubs/file', fileName: 'PayStubs' }" focusable
        checkable v-model:checked-rows="checkedRows" @update:loading="(value) => isLoading = value">
        <template #actions>
          <b-button tag="router-link" to="/accounting/paystubs/create" icon-left="plus">
            Create
          </b-button>
        </template>
        <template #dropdown-actions>
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
        <b-table-column field="payStubNumber" label="PayStub Number" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.payStubNumber" placeholder="Search..." icon="magnify" size="is-small"></b-input>
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
            <b-input v-model="serverParams.workerFullName" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.workerFullName }}
          </template>
        </b-table-column>
        <b-table-column field="numberId" label="Number ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
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
          <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
            <template #trigger>
              <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
            </template>
            <b-dropdown-item aria-role="listitem" @click="onDownloadPayStubPdf(props.row)">Download</b-dropdown-item>
            <b-dropdown-item v-if="props.row.pdfUrl || props.row.pdfGenerated" aria-role="listitem"
              @click="onRegeneratePayStubPdf(props.row)">
              Regenerate PDF
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" :disabled="props.row.emailSent || props.row.emailSending"
              @click="onSendPayStubEmail(props.row)">
              {{ props.row.emailSent ? 'Email Sent' : props.row.emailSending ? 'Sending...' : 'Send Email' }}
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" class="has-text-danger" @click="onDeletePayStub(props.row)">
              Delete
            </b-dropdown-item>
          </b-dropdown>
        </b-table-column>
      </SigookGrid>
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
import { ref, useTemplateRef } from 'vue';
import { useAgencyStore } from '@/modules/agency/store';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { downloadPDF, openFileUrl } from '@/shared/utils/downloadFile';
import { dateMonth, currency } from '@/shared/format';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import {
  getAgencyPayStubs,
  downloadPayStubPdf,
  regeneratePayStubPdf,
  sendPayStubEmail,
  sendPayStubEmailBulk,
  deleteAgencyPayStub,
} from '@/modules/agency/accounting/paystubs/api';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import GeneratePayStubs from '@/modules/agency/accounting/paystubs/components/GeneratePayStubs.vue';
import SkipPayrollNumber from '@/modules/agency/accounting/paystubs/components/SkipPayrollNumber.vue';
import PageHeader from '@/shared/ui/PageHeader.vue';
import { accountingCrumbs } from '@/shared/ui/breadcrumbs';
import type { AgencyPayStubFilter, AgencyPayStubRow } from '@/modules/agency/accounting/paystubs/types';
import type { GridHandle, PaginatedList } from '@/shared/types/common';

const agencyStore = useAgencyStore();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  payStubNumber: 0,
  createdAt: 1,
  workerFullName: 2,
  numberId: 3,
  totalPaid: 4,
};

const isLoading = ref(true);
const checkedRows = ref<AgencyPayStubRow[]>([]);
const createdAtDatesSelected = ref<Date[]>([]);
const serverParams = ref<AgencyPayStubFilter>(agencyStore.agencyPayStubFilter ?? {
  sortBy: 0,
  isDescending: true,
});

const showGeneratePayStubsModal = ref(false);
const showSkipPayrollNumberModal = ref(false);

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

function loadPayStubs(params: AgencyPayStubFilter): Promise<PaginatedList<AgencyPayStubRow>> {
  agencyStore.updateAgencyPayStubFilter(params);
  return getAgencyPayStubs(params)
    .then((response) => ({ ...response, items: response.items.map((i) => ({ ...i, emailSending: false })) }));
}

function onDownloadPayStubPdf(payStub: AgencyPayStubRow) {
  if (payStub.pdfUrl) {
    openFileUrl(payStub.pdfUrl);
    return;
  }
  renderPayStubPdf(payStub, downloadPayStubPdf);
}

function onRegeneratePayStubPdf(payStub: AgencyPayStubRow) {
  renderPayStubPdf(payStub, regeneratePayStubPdf);
}

function renderPayStubPdf(payStub: AgencyPayStubRow, render: (payStubId: string) => Promise<Blob>) {
  isLoading.value = true;
  render(payStub.id)
    .then((response) => {
      isLoading.value = false;
      payStub.pdfGenerated = true;
      downloadPDF(response, `${payStub.payStubNumber} ${payStub.workerFullName}`);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function onSendPayStubEmail(payStub: AgencyPayStubRow) {
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
      const payStubIds = checkedRows.value.map((p) => p.id);
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

function onDeletePayStub(payStub: AgencyPayStubRow) {
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
          grid.value?.reload();
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
  grid.value?.reload();
}
</script>
