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
        </template>
        <template v-slot:dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="showGeneratePayStubsModal = true">
            <b-icon icon="table-plus"></b-icon>
            <span>Generate</span>
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" @click="showSkipPayrollNumberModal = true">
            <b-icon icon="step-forward"></b-icon>
            <span>Skip Payroll Number</span>
          </b-dropdown-item>
        </template>
      </export>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="payStubNumber"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange">
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
                @input="onCreatedAtSelected" append-to-body>
              </b-datepicker>
            </template>
            <template v-slot="props">
              {{ dateMonth(props.row.createdAt) }}
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
          <b-table-column field="numberId" label="Number ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered">
              </b-input>
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
            <b-field>
              <b-tooltip label="Download" type="is-dark" position="is-top">
                <b-button type="is-success" outlined rounded icon-right="file-pdf" class="mr-2"
                  @click="onDownloadPayStubPdf(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip :label="props.row.emailSent ? 'Email Sent' : 'Send Email'" type="is-dark" position="is-top">
                <b-button type="is-info" outlined rounded :icon-right="props.row.emailSent ? 'email-check' : 'email'"
                  class="mr-2" :loading="props.row.emailSending" :disabled="props.row.emailSent"
                  @click="onSendPayStubEmail(props.row)">
                </b-button>
              </b-tooltip>
              <b-tooltip label="Delete" type="is-dark" position="is-top">
                <b-button type="is-danger" outlined rounded icon-right="delete" @click="onDeletePayStub(props.row)">
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

<script lang="ts">
import { downloadPDF } from "@/utils/downloadFile";
import { dateMonth, currency } from '@/utils/filters';
import {
  getAgencyPayStubs,
  downloadPayStubPdf,
  sendPayStubEmail,
  deleteAgencyPayStub,
} from "@/api/agencyPayStubApi";

export default {
  components: {
    Export: () => import("@/components/Export.vue"),
    GeneratePayStubs: () => import("@/components/agency_accounting/GeneratePayStubs.vue"),
    SkipPayrollNumber: () => import("@/components/agency_accounting/SkipPayrollNumber.vue")
  },
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
    this.loadPayStubs();
  },
  methods: {
    downloadPDF,
    dateMonth,
    currency,
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.loadPayStubs();
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
        case 'numberId':
          this.serverParams.sortBy = 3;
          break;
        case 'totalPaid':
          this.serverParams.sortBy = 4;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.loadPayStubs();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.loadPayStubs();
      }
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.loadPayStubs();
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    loadPayStubs() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyPayStubFilter", this.serverParams);
      getAgencyPayStubs(this.serverParams)
        .then((response) => {
          this.rows = response.items.map((i) => ({ ...i, emailSending: false, actions: null }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    onDownloadPayStubPdf(payStub) {
      this.isLoading = true;
      downloadPayStubPdf(payStub.id)
        .then(response => {
          this.isLoading = false;
          this.downloadPDF(response, `${payStub.payStubNumber} ${payStub.workerFullName}`);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    onSendPayStubEmail(payStub) {
      payStub.emailSending = true;
      sendPayStubEmail(payStub.id)
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
    async onDeletePayStub(payStub) {
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
          deleteAgencyPayStub(payStub.id)
            .then(() => {
              this.isLoading = false;
              this.loadPayStubs();
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
      this.loadPayStubs();
    }
  }
};
</script>