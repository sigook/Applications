<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message title="Are you sure you want to delete this invoice?" type="is-warning" has-icon :closable="false">
      You are about to delete the invoice
      <b> {{ invoice.invoiceNumber }}</b>
      <br>
      If you are going to use the invoice number
      <b> {{ invoice.invoiceNumber }}</b>
      for the same company,
      remember that you should not generate any invoice for any other company before generate this invoice again.
      <p v-if="rows.length > 0" class="mt-3">
        <b>If you also want to delete the paystubs please check the paystubs you want to delete, the paystubs number may
          change.</b>
      </p>
    </b-message>
    <div class="paystubs-table" v-if="rows.length > 0">
      <b-table :data="rows" v-model:checked-rows="selectedPayStubs" checkable>
        <template>
          <b-table-column field="payStubNumber" label="Pay Stub Number" v-slot="props">
            {{ props.row.payStubNumber }}
          </b-table-column>
        </template>
      </b-table>
    </div>
    <b-button @click="submitDeleteInvoice" type="is-danger">Delete</b-button>
  </div>
</template>
<style scoped>
.paystubs-table {
  max-height: var(--grid-height);
  overflow-y: auto;
  margin-bottom: 0.75rem;
}
</style>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import {
  getPayStubsByInvoice,
  deleteAgencyInvoice
} from "@/api/agencyInvoiceApi";
import type { AgencyInvoiceListItem, PayStubDeleteWarningItem } from '@/types/accounting';

const props = defineProps<{ invoice: AgencyInvoiceListItem }>();
const emit = defineEmits<{(e: 'deleted'): void}>();

const isLoading = ref(true);
const rows = ref<PayStubDeleteWarningItem[]>([]);
const selectedPayStubs = ref<PayStubDeleteWarningItem[]>([]);

async function loadPayStubs() {
  rows.value = await getPayStubsByInvoice(props.invoice.id);
}

async function submitDeleteInvoice() {
  isLoading.value = true;
  await deleteAgencyInvoice({
    invoiceId: props.invoice.id,
    payStubs: selectedPayStubs.value.map((payStub) => payStub.payStubId),
  }).catch((error: unknown) => {
    isLoading.value = false;
    showAlertError(error);
  });
  isLoading.value = false;
  emit("deleted");
}

(async () => {
  await loadPayStubs();
  isLoading.value = false;
})();
</script>
