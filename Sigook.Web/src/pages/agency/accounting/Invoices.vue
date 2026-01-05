<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-2">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Invoices
        <span class="fw-100 fz-1">
          ({{ totalItems }})
        </span>
        <b-tag size="is-medium"><b>{{ total | currency }}</b></b-tag>
      </h2>
    </div>
    <div>
      <export :url="'/api/agency/accounting/Invoices/file'" :params="serverParams" :fileName="'Invoices'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button tag="router-link" to="/accounting/create-invoice" icon-left="plus">
            Create
          </b-button>
        </template>
      </export>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="invoiceNumber"
        :current-page.sync="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="invoiceNumber" label="Invoice Number" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.invoiceNumber" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered">
              </b-input>
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
                @input="onCreatedAtSelected">
              </b-datepicker>
            </template>
            <template v-slot="props">
              {{ props.row.createdAt | dateMonth }}
            </template>
          </b-table-column>
          <b-table-column field="companyFullName" label="Company" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.companyFullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered">
              </b-input>
            </template>
            <template v-slot="props">
              {{ props.row.companyFullName }}
            </template>
          </b-table-column>
          <b-table-column field="salesRepresentative" label="Sales Rep" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.salesRepresentative" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered">
              </b-input>
            </template>
            <template v-slot="props">
              {{ props.row.salesRepresentative }}
            </template>
          </b-table-column>
          <b-table-column field="totalNet" label="Total">
            <template v-slot="props">
              {{ props.row.totalNet | currency }}
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-field>
              <b-tooltip label="Download" type="is-dark" position="is-top">
                <b-button type="is-success" outlined rounded icon-right="file-pdf" class="mr-2"
                  @click="downloadInvoicePdf(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip label="Send Email" type="is-dark" position="is-top">
                <b-button type="is-info" outlined rounded icon-right="email" class="mr-2"
                  @click="sendInvoiceEmail(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip label="Delete" type="is-dark" position="is-top">
                <b-button type="is-danger" outlined rounded icon-right="delete" @click="deleteInvoice(props.row)">
                </b-button>
              </b-tooltip>
            </b-field>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <b-modal v-model="showDeleteModal" width="800px">
      <delete-invoice v-if="currentInvoice" :invoice="currentInvoice" @deleted="onDeleteInvoice" />
    </b-modal>

    <b-modal v-model="showSendEmailModal" width="500px">
      <send-invoice-email v-if="currentInvoice" :invoice="currentInvoice" @sent="onSendInvoiceEmail" />
    </b-modal>
  </div>
</template>

<script>
import download from "@/mixins/downloadFileMixin";

export default {
  components: {
    Export: () => import("@/components/Export"),
    DeleteInvoice: () => import("@/components/agency_accounting/DeleteInvoice"),
    SendInvoiceEmail: () => import("@/components/agency_accounting/SendInvoiceEmail"),
  },
  mixins: [download],
  data() {
    return {
      isLoading: true,
      totalItems: 0,
      total: 0,
      rows: [],
      createdAtDatesSelected: [],
      serverParams: {
        sortBy: 0,
        pageIndex: 1,
        pageSize: 30,
        isDescending: true
      },
      showDeleteModal: false,
      currentInvoice: null,
      showSendEmailModal: false,
    };
  },
  created() {
    if (this.$store.state.agency.agencyInvoiceFilter) {
      this.serverParams = this.$store.state.agency.agencyInvoiceFilter;
      if (this.serverParams.createdAtFrom && this.serverParams.createdAtTo) {
        this.createdAtDatesSelected[0] = this.serverParams.createdAtFrom;
        this.createdAtDatesSelected[1] = this.serverParams.createdAtTo;
      }
    }
    this.getInvoices();
  },
  methods: {
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getInvoices();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'invoiceNumber':
          this.serverParams.sortBy = 0;
          break;
        case 'createdAt':
          this.serverParams.sortBy = 1;
          break;
        case 'companyFullName':
          this.serverParams.sortBy = 2;
          break;
        case 'salesRepresentative':
          this.serverParams.sortBy = 3;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getInvoices();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getInvoices();
      }
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.getInvoices();
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    getInvoices() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyInvoiceFilter", this.serverParams);
      this.$store.dispatch("agency/getInvoices", this.serverParams)
        .then((response) => {
          this.rows = response.detail.items.map(i => ({ ...i, actions: null }));
          this.totalItems = response.detail.totalItems;
          this.total = response.total;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    downloadInvoicePdf(invoice) {
      this.isLoading = true;
      this.$store.dispatch("agency/downloadInvoicePdf", invoice.id)
        .then(response => {
          this.isLoading = false;
          this.downloadPDF(response, `${invoice.invoiceNumber} ${invoice.companyFullName}`);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    sendInvoiceEmail(invoice) {
      this.currentInvoice = invoice;
      this.showSendEmailModal = true;
    },
    onSendInvoiceEmail() {
      this.showSendEmailModal = false;
      this.getInvoices();
    },
    deleteInvoice(invoice) {
      this.currentInvoice = invoice;
      this.showDeleteModal = true;
    },
    onDeleteInvoice() {
      this.showDeleteModal = false;
      this.getInvoices();
    }
  }
};
</script>