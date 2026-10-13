<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <p class="modal-card-title">Mark as Paid</p>
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
        <b-button type="is-primary" native-type="submit">Mark as Paid</b-button>
      </footer>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { changeInvoiceStatus } from '@/modules/agency/accounting/invoices/api';
import { InvoiceStatus } from '@/modules/agency/accounting/invoices/types';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import type { AgencyInvoiceListItem } from '@/modules/agency/accounting/invoices/types';

const props = defineProps<{ invoice: AgencyInvoiceListItem }>();
const emit = defineEmits<{ (e: 'updated'): void; (e: 'close'): void }>();

const today = new Date();
const isLoading = ref(false);

const schema = yup.object({
  paidAt: yup.date().required('Payment date is required'),
});

const form = useStickyForm<{ paidAt: Date | null }>({
  schema,
  initialValues: { paidAt: today },
});
const { paidAt } = form.fields;
const errors = form.errors;

function submit() {
  form.markInteracted();
  form.handleSubmit(
    values => {
      isLoading.value = true;
      changeInvoiceStatus(props.invoice.id, {
        status: InvoiceStatus.Paid,
        paidAt: values.paidAt ? values.paidAt.toISOString() : undefined,
      })
        .then(() => {
          showAlertSuccess('Invoice marked as Paid');
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
