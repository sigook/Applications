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
    <b-table v-if="rows.length > 0" :data="rows" :checked-rows.sync="selectedPayStubs" checkable>
      <template>
        <b-table-column field="payStubNumber" label="Pay Stub Number" v-slot="props">
          {{ props.row.payStubNumber }}
        </b-table-column>
      </template>
    </b-table>
    <b-field label="Verification Code" :type="{ 'is-danger': errors.has('verificationCode') }"
      :message="errors.first('verificationCode')">
      <b-input v-model="verificationCode" v-validate="'required'" name="verificationCode" />
    </b-field>
    <b-button @click="deleteInvoice" type="is-danger">Delete</b-button>
  </div>
</template>
<script>
export default {
  props: ["invoice"],
  data() {
    return {
      isLoading: true,
      rows: [],
      verificationCode: null,
      selectedPayStubs: [],
    }
  },
  methods: {
    async getPayStubs() {
      this.rows = await this.$store.dispatch("agency/getPayStubsByInvoice", this.invoice.id);
    },
    async requestVerificationCode() {
      await this.$store.dispatch("agency/getVerificationCode", this.invoice.id);
    },
    async deleteInvoice() {
      this.isLoading = true;
      await this.$store.dispatch("agency/deleteInvoice", {
        invoiceId: this.invoice.id,
        verificationCode: this.verificationCode,
        payStubs: this.selectedPayStubs.map(payStub => payStub.payStubId)
      }).catch(error => {
        this.isLoading = false;
        this.showAlertError(error);
      });
      this.isLoading = false;
      this.$emit("deleted");
    }
  },
  async created() {
    await this.getPayStubs();
    await this.requestVerificationCode();
    this.isLoading = false;
  }
}
</script>