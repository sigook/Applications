<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <p class="modal-card-title">{{ isEdit ? 'Edit Payment Date' : 'Mark as Paid' }}</p>
    </header>
    <form @submit.prevent="submit">
      <section class="modal-card-body" style="min-width: 420px; position: relative">
        <b-loading v-model="isLoading" :is-full-page="false" />
        <p class="mb-4">Invoice <b>{{ invoice.invoiceNumber }}</b> · {{ invoice.companyFullName }}</p>
        <b-field label="Payment date" required :type="errors.paidAt ? 'is-danger' : ''" :message="errors.paidAt">
          <b-datepicker v-model="paidAt" :mobile-native="false" :max-date="today" placeholder="Select a date"
            append-to-body />
        </b-field>
      </section>
      <footer class="modal-card-foot">
        <b-button @click="emit('close')">Cancel</b-button>
        <b-button type="is-primary" native-type="submit">{{ isEdit ? 'Save' : 'Mark as Paid' }}</b-button>
      </footer>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { changeInvoiceStatus, updateInvoicePaidAt } from '@/modules/agency/accounting/invoices/api';
import { InvoiceStatus } from '@/modules/agency/accounting/invoices/types';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import type { AgencyInvoiceListItem } from '@/modules/agency/accounting/invoices/types';

const props = defineProps<{ invoice: AgencyInvoiceListItem }>();
const emit = defineEmits<{ (e: 'updated'): void; (e: 'close'): void }>();

const today = new Date();
const isLoading = ref(false);
const isEdit = props.invoice.status === InvoiceStatus.Paid;

const schema = yup.object({
  paidAt: yup.date().required('Payment date is required'),
});

const form = useStickyForm<{ paidAt: Date | null }>({
  schema,
  initialValues: { paidAt: isEdit ? (props.invoice.paidAt ? new Date(props.invoice.paidAt) : null) : today },
});
const { paidAt } = form.fields;
const errors = form.errors;

function save(paidAtIso: string): Promise<void> {
  return isEdit
    ? updateInvoicePaidAt(props.invoice.id, { paidAt: paidAtIso })
    : changeInvoiceStatus(props.invoice.id, { status: InvoiceStatus.Paid, paidAt: paidAtIso });
}

function submit() {
  form.markInteracted();
  form.handleSubmit(
    values => {
      if (!values.paidAt) return;
      isLoading.value = true;
      save(values.paidAt.toISOString())
        .then(() => {
          showAlertSuccess(isEdit ? 'Payment date updated' : 'Invoice marked as Paid');
          emit('updated');
          emit('close');
        })
        .catch(err => {
          isLoading.value = false;
          showAlertError(err);
        });
    },
    () => showAlertError('Please select the payment date'),
  )();
}
</script>
