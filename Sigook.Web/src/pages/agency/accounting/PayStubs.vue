<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-2">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        PayStubs
      </h2>
    </div>
    <div>
      <export :url="'/api/agency/accounting/PayStubs/file'" :params="serverParams" :fileName="'PayStubs'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button tag="router-link" to="/accounting/create-paystub" icon-left="plus">
            Create
          </b-button>
          <b-button icon-left="table-plus" @click="showGeneratePayStubsModal = true">Generate</b-button>
          <b-button icon-left="step-forward" @click="showSkipPayrollNumberModal = true">Skip Payroll Number</b-button>
        </template>
      </export>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="payStubNumber"
        :current-page.sync="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="payStubNumber" label="PayStub Number" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.payStubNumber" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered">
              </b-input>
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
                @input="onCreatedAtSelected">
              </b-datepicker>
            </template>
            <template v-slot="props">
              {{ props.row.createdAt | dateMonth }}
            </template>
          </b-table-column>
          <b-table-column field="workerFullName" label="Worker" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.workerFullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered">
              </b-input>
            </template>
            <template v-slot="props">
              {{ props.row.workerFullName }}
            </template>
          </b-table-column>
          <b-table-column field="totalPaid" label="Total Paid">
            <template v-slot="props">
              {{ props.row.totalPaid | currency }}
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-field>
              <b-tooltip label="Download" type="is-dark" position="is-top">
                <b-button type="is-success" outlined rounded icon-right="file-pdf" class="mr-2"
                  @click="downloadPayStubPdf(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip :label="props.row.emailSent ? 'Email Sent' : 'Send Email'" type="is-dark" position="is-top">
                <b-button type="is-info" outlined rounded :icon-right="props.row.emailSent ? 'email-check' : 'email'"
                  class="mr-2" :loading="props.row.emailSending" :disabled="props.row.emailSent"
                  @click="sendPayStubEmail(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip label="Delete" type="is-dark" position="is-top">
                <b-button type="is-danger" outlined rounded icon-right="delete" @click="deletePayStub(props.row)">
                </b-button>
              </b-tooltip>
            </b-field>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <b-modal v-model="showGeneratePayStubsModal" width="800px">
      <generate-pay-stubs @pay-stubs-generated="onPayStubsGenerated" />
    </b-modal>

    <b-modal v-model="showSkipPayrollNumberModal" width="500px">
      <skip-payroll-number></skip-payroll-number>
    </b-modal>
  </div>
</template>

<script>
import download from "@/mixins/downloadFileMixin";

export default {
  components: {
    Export: () => import("@/components/Export"),
    GeneratePayStubs: () => import("@/components/agency_accounting/GeneratePayStubs"),
    SkipPayrollNumber: () => import("@/components/agency_accounting/SkipPayrollNumber")
  },
  mixins: [download],
  data() {
    return {
      isLoading: true,
      totalItems: 0,
      rows: [],
      createdAtDatesSelected: [],
      serverParams: {
        sortBy: 0,
        pageIndex: 1,
        pageSize: 30,
        isDescending: true
      },
      showGeneratePayStubsModal: false,
      showSkipPayrollNumberModal: false
    };
  },
  created() {
    if (this.$store.state.agency.agencyPayStubFilter) {
      this.serverParams = this.$store.state.agency.agencyPayStubFilter;
      if (this.serverParams.createdAtFrom && this.serverParams.createdAtTo) {
        this.createdAtDatesSelected[0] = this.serverParams.createdAtFrom;
        this.createdAtDatesSelected[1] = this.serverParams.createdAtTo;
      }
    }
    this.getPayStubs();
  },
  methods: {
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getPayStubs();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'payStubNumber':
          this.serverParams.sortBy = 0;
          break;
        case 'createdAt':
          this.serverParams.sortBy = 1;
          break;
        case 'workerFullName':
          this.serverParams.sortBy = 2;
          break;
        case 'totalPaid':
          this.serverParams.sortBy = 3;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getPayStubs();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getPayStubs();
      }
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.getPayStubs();
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    getPayStubs() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyPayStubFilter", this.serverParams);
      this.$store.dispatch("agency/getPayStubs", this.serverParams)
        .then((response) => {
          this.rows = response.items.map(i => ({ ...i, emailSending: false, actions: null }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    downloadPayStubPdf(payStub) {
      this.isLoading = true;
      this.$store.dispatch("agency/downloadPayStubPdf", payStub.id)
        .then(response => {
          this.isLoading = false;
          this.downloadPDF(response, `${payStub.payStubNumber} ${payStub.workerFullName}`);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    sendPayStubEmail(payStub) {
      payStub.emailSending = true;
      this.$store.dispatch("agency/sendPayStubEmail", payStub.id)
        .then(() => {
          payStub.emailSending = false;
          payStub.emailSent = true;
          console.log(payStub);
          this.showAlertSuccess(`Email to ${payStub.workerFullName} sent successfully`);
        })
        .catch(error => {
          payStub.emailSending = false;
          this.showAlertError(error);
        });
    },
    async deletePayStub(payStub) {
      const message = `You are about to delete the pay stub <b>${payStub.payStubNumber}</b>
        <br>
        <br>
        If you are going to use the pay stub number <b>${payStub.payStubNumber}</b> for the same worker,
        remember that you should not generate any pay stub for any other worker before generate this pay stub again.`;
      this.$buefy.dialog.confirm({
        title: "Are you sure you want to delete?",
        message: message,
        confirmText: "Yes, I read and I want to delete",
        type: "is-danger",
        hasIcon: true,
        onConfirm: () => {
          this.isLoading = true;
          this.$store.dispatch("agency/deletePayStub", payStub.id)
            .then(() => {
              this.isLoading = false;
              this.getPayStubs();
            })
            .catch(error => {
              this.isLoading = false;
              this.showAlertError(error);
            });
        }
      })
    },
    onPayStubsGenerated() {
      this.showGeneratePayStubsModal = false;
      this.getPayStubs();
    }
  }
};
</script>