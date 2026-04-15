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
    <b-table v-if="rows.length > 0" :data="rows" v-model:checked-rows="selectedPayStubs" checkable>
      <template>
        <b-table-column field="payStubNumber" label="Pay Stub Number" v-slot="props">
          {{ props.row.payStubNumber }}
        </b-table-column>
      </template>
    </b-table>
    <b-field label="Verification Code" :type="{ 'is-danger': errors.verificationCode }"
      :message="errors.verificationCode">
      <b-input v-model="verificationCode" name="verificationCode" />
    </b-field>
    <b-button @click="submitDeleteInvoice" type="is-danger">Delete</b-button>
  </div>
</template>
<script lang="ts">
import { useForm } from 'vee-validate';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import {
  getPayStubsByInvoice,
  sendInvoiceVerificationCode,
  deleteAgencyInvoice
} from "@/api/agencyInvoiceApi";

export default {
  props: ["invoice"],
  setup() {
    const schema = yup.object({
      verificationCode: yup.string().required('Verification code is required'),
    });
    const { handleSubmit, errors, defineField } = useForm({
      validationSchema: schema,
    });
    const [verificationCode] = defineField('verificationCode');
    return { verificationCode, errors, handleSubmit };
  },
  data() {
    return {
      isLoading: true,
      rows: [] as any[],
      selectedPayStubs: [] as any[],
    };
  },
  methods: {
    async loadPayStubs() {
      this.rows = await getPayStubsByInvoice(this.invoice.id);
    },
    async requestVerificationCode() {
      await sendInvoiceVerificationCode(this.invoice.id);
    },
    submitDeleteInvoice() {
      (this as any).handleSubmit(async (values: any) => {
        this.isLoading = true;
        await deleteAgencyInvoice({
          invoiceId: this.invoice.id,
          verificationCode: values.verificationCode,
          payStubs: this.selectedPayStubs.map((payStub: any) => payStub.payStubId),
        }).catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
        this.isLoading = false;
        this.$emit("deleted");
      })();
    },
  },
  async created() {
    await this.loadPayStubs();
    await this.requestVerificationCode();
    this.isLoading = false;
  },
};
</script>
