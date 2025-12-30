<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="invoiceNumberId"
        :current-page.sync="serverParams.pageIndex">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column label="Invoice Number" field="invoiceNumberId" v-slot="props">
            <span>{{ props.row.invoiceNumber }}</span>
          </b-table-column>
          <b-table-column label="Created At" field="createdAt" v-slot="props">
            <span>{{ props.row.createdAt | datetime }}</span>
          </b-table-column>
          <b-table-column label="Week Ending" field="weekEnding" v-slot="props">
            <span v-if="props.row.weekEnding">{{ props.row.weekEnding | date }}</span>
            <span v-else>N/A</span>
          </b-table-column>
          <b-table-column label="Total" field="totalNet" v-slot="props">
            <span>{{ props.row.totalNet | currency }}</span>
          </b-table-column>
        </template>
      </b-table>
    </div>
  </div>
</template>


<script>
import download from '../../mixins/downloadFileMixin';

export default {
  data() {
    return {
      isLoading: true,
      totalItems: 0,
      rows: [],
      invoiceId: null,
      invoiceNumber: null,
      showModalCompanyInvoicePay: false,
      serverParams: {
        sortBy: 0,
        isDescending: false,
        pageIndex: 1,
        pageSize: 30
      }
    }
  },
  methods: {
    getCompanyInvoice() {
      this.isLoading = true;
      this.$store.dispatch('company/getCompanyInvoice', this.serverParams)
        .then(response => {
          this.rows = response.items;
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(e => {
          this.isLoading = false;
          this.showAlertError(e);
        })
    },
    downloadInvoicePdf(item) {
      this.isLoading = true;
      this.$store.dispatch("downloadInvoicePdf", { invoiceId: item.id })
        .then((response) => {
          this.isLoading = false;
          this.downloadPDF(response, "Invoice_" + item.numberId);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    openCompanyInvoicePay(invoiceId, invoiceNumber) {
      this.invoiceId = invoiceId;
      this.showModalCompanyInvoicePay = true;
      this.invoiceNumber = invoiceNumber;
    },
    openPaymentSupportModal(invoiceId, invoiceNumber) {
      this.showModalPaymentSupport = true;
      this.invoiceId = invoiceId;
      this.invoiceNumber = invoiceNumber;
    },
    onModalCompanyInvoicePayClose(change) {
      this.showModalCompanyInvoicePay = false;
      if (change) {
        this.showAll()
      }
    },

  },
  mixins: [download],
  created() {
    this.getCompanyInvoice();
  },
}

</script>