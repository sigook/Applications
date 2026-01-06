<template>
  <div class="white-container-mobile has-menu-bottom">
    <b-loading v-model="isLoading"></b-loading>

    <section class="wrapper-request-top" v-if="request">
      <div>
        <router-link :to="'/agency-companies/company/' + request.companyProfileId">
          <img v-if="request.companyLogo" :src="request.companyLogo" />
        </router-link>
        <h2 class="capitalize fz1 fw-700">
          <span class="fw-400 fz-0">{{ request.numberId }}</span>
          {{ request.jobTitle }}
        </h2>
      </div>
      <div>
        <div v-if="request.status && request.status !== 'None'"
          class="option-request-top capitailized fw-700 is-inline-block" :class="request.status">
          {{ $t(request.status) }}
        </div>
        <floating-menu class="is-inline-block" v-if="canEdit">
          <template slot="options">
            <button class="floating-menu-item" v-on:click="alertRequestAnotherWorker">
              <span>{{ $t("RequestAnotherWorker") }}</span>
            </button>
            <button class="floating-menu-item" v-on:click="editContentModal = true">
              <span>Edit Requirements</span>
            </button>
            <button class="floating-menu-item" v-on:click="modalValidation = true">
              <span>{{ $t("CancelRequest") }}</span>
            </button>
          </template>
        </floating-menu>
      </div>
    </section>

    <b-tabs v-model="currentTab" @input="changeTab" v-if="request">
      <b-tab-item label="Detail" value="Detail">
        <detail v-if="visitedTabs.includes('Detail')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
      <b-tab-item label="Workers" value="Workers">
        <workers v-if="visitedTabs.includes('Workers')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
      <b-tab-item label="Punch Card" value="PunchCard" v-if="!isDirectHiring">
        <punch-card v-if="visitedTabs.includes('PunchCard')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
    </b-tabs>

    <b-modal v-model="modalValidation" width="500px">
      <cancel-list @sendReason="(reason) => cancelRequest(reason)"></cancel-list>
    </b-modal>

    <b-modal v-model="modalValidationRequestAnotherWorker" width="500px">
      <request-another-worker
      @sendAnotherWorker="(comment) => requestAnotherWorker(comment)"></request-another-worker>
    </b-modal>

    <b-modal v-model="editContentModal" width="800px">
      <div class="p-3">
        <div class="container-flex">
          <div class="col-12 col-padding">
            <b-field label="Requirements" :type="errors.has('requirements') ? 'is-danger' : ''"
              :message="errors.has('requirements') ? errors.first('requirements') : ''">
              <vue-editor id="requirements-input" v-model="request.requirements" :name="'requirements'"
                v-validate="'required|min:100'" />
            </b-field>
          </div>
          <div class="col-12 col-padding">
            <b-button type="is-primary" @click="onUpdateRequirements(request.requirements)">Save</b-button>
          </div>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script>
import confirmationAlert from "../../mixins/confirmationAlert";
import directHiringMixin from "../../mixins/directHiringMixin";

export default {
  data() {
    return {
      request: {},
      isLoading: true,
      modalValidation: false,
      modalValidationRequestAnotherWorker: false,
      currentTab: "Detail",
      visitedTabs: ["Detail"],
      editContentModal: false,
    };
  },
  components: {
    CancelList: () => import("../../components/company/CompanyCancelList"),
    RequestAnotherWorker: () => import("../../components/company/DialogRequestWorker"),
    FloatingMenu: () => import("../../components/FloatingMenuDots"),
    Detail: () => import("../../components/company_request/CompanyRequestDetail"),
    Workers: () => import("../../components/company_request/CompanyRequestWorkers"),
    PunchCard: () => import("../../components/company_request/CompanyRequestPunchCard"),
  },
  mixins: [confirmationAlert, directHiringMixin],
  methods: {
    cancelRequest(reason) {
      this.modalValidation = false;
      this.isLoading = true;
      this.$store
        .dispatch("company/cancelRequest", {
          id: this.request.id,
          cancellationReasonId: reason.reasonId,
          otherCancellationReason: reason.otherMessage,
        })
        .then(() => {
          this.isLoading = false;
          this.unsavedChanges = false;
          this.showAlertSuccess(this.$t("Cancelled"));
          this.$router.push("/company-requests");
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    getData() {
      this.$store.dispatch("company/getRequest", this.$route.params.id)
        .then((response) => {
          this.isLoading = false;
          this.request = response;
        })
        .catch((error) => {
          this.showAlertError(error.data);
          this.isLoading = false;
        });
    },
    alertRequestAnotherWorker() {
      this.modalValidationRequestAnotherWorker = true;
    },
    requestAnotherWorker(comment) {
      this.modalValidationRequestAnotherWorker = false;
      this.isLoading = true;
      this.$store.dispatch("company/RequestAnotherWorker", { requestId: this.$route.params.id, comment: { comments: comment } })
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess(this.$t("Requested"));
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    onUpdateRequirements(data) {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.isLoading = true;
          this.$store.dispatch("company/editRequest", { id: this.$route.params.id, model: { requirements: data } })
            .then(() => {
              this.isLoading = false;
              this.showAlertSuccess(this.$t("Updated"));
              this.request.requirements = data;
              this.editContentModal = false;
            }).catch((error) => {
              this.isLoading = false;
              this.showAlertError(error.data);
            });
        }
      });
    },
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: `/request/${this.$route.params.id}`,
        query: {
          tab: tab,
        },
      });
    },
  },
  created() {
    this.getData();
    if (this.$route.query && this.$route.query.tab) {
      this.currentTab = this.$route.query.tab;
      if (!this.visitedTabs.includes(this.$route.query.tab)) {
        this.visitedTabs.push(this.$route.query.tab);
      }
    }
  },
  computed: {
    canEdit() {
      if (this.request.status === this.$statusFinalized) {
        return false;
      }
      return this.request.status !== this.$statusCancelled;
    },
  },
};
</script>
