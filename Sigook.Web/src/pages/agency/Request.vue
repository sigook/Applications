<template>
  <div class="white-container-mobile wrapper-request">
    <b-loading v-model="isLoading"></b-loading>
    <!-- CANCELLED -->
    <div v-if="request && !request.canEdit && request.cancellationDetail" class="alert-warning">
      <b>Cancellation detail: </b> {{ request.cancellationDetail }}
    </div>
    <section class="wrapper-request-top" v-if="request">
      <div class="asap-title-detail" :class="[isDirectHiring ? 'mb-3' : '']" v-if="request.isAsap">
        Asap
      </div>
      <div class="asap-title-detail" :class="[request.isAsap ? 'mt-6' : '']" v-if="isDirectHiring">
        DH
      </div>
      <div>
        <router-link :to="'/agency-companies/company/' + request.companyProfileId">
          <img v-if="request.companyLogo" :src="request.companyLogo" alt="logo" />
        </router-link>
        <h2 class="capitalize fz1 fw-700">
          <span class="fw-400 fz-0">{{ request.numberId }}</span>
          {{ request.jobTitle }}
          <i class="fz-2 block">{{ billingTitle }}</i>
        </h2>
      </div>
      <div>
        <div class="d-inline-block option-request-top">
          {{ request.displayRecruiters | breakWord }}
        </div>
        <div v-if="request.status && request.status !== 'None'"
          class="option-request-top capitailized fw-700 is-inline-block" :class="request.status">
          {{ $t(request.status) }}
        </div>
        <floating-menu class="is-inline-block" v-if="request.canEdit">
          <template slot="options">
            <button class="floating-menu-item"
              @click="$router.push({ path: `/agency-update-request/${request.companyProfileId}/${request.id}` })">
              <span>Edit Request</span>
            </button>
            <button class="floating-menu-item" v-on:click="showShiftModal = true">
              <span>Edit Shift</span>
            </button>
            <button v-if="canSendInvitation" class="floating-menu-item" v-on:click="sendInvitation(request.id)">
              <span>Send an email invitation</span>
            </button>
            <button disabled v-else class="floating-menu-item" :title="warningMessage">
              <span>Send an email invitation
                <span class="fz-1">
                  (Sent it {{ request.invitationSentItAt | dateFromNow }})</span></span>
            </button>
            <button class="floating-menu-item" v-if="request.canEdit" v-on:click="cancelRequestModal = true">
              <span> Cancel Order</span>
            </button>
          </template>
        </floating-menu>
        <floating-menu class="is-inline-block" v-if="!request.canEdit">
          <template slot="options">
            <button class="floating-menu-item" v-on:click="agencyRequestOpen(request.id)">
              <span>Reopen</span>
            </button>
          </template>
        </floating-menu>
      </div>
    </section>
    <b-tabs v-model="currentTab" @input="changeTab" v-if="request">
      <b-tab-item label="Detail" value="Detail">
        <detail v-if="visitedTabs.includes('Detail')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
      <b-tab-item label="Applicants" value="Applicants">
        <applicants v-if="visitedTabs.includes('Applicants')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
      <b-tab-item label="Workers" value="Workers">
        <workers v-if="visitedTabs.includes('Workers')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
      <b-tab-item label="Punch Card" value="PunchCard" v-if="!isDirectHiring">
        <punch-card v-if="visitedTabs.includes('PunchCard')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
    </b-tabs>

    <div v-if="request">

      <b-modal v-model="cancelRequestModal" width="500px">
        <cancel-list @sendReason="(reason) => cancelRequest(reason)"></cancel-list>
      </b-modal>

       <b-modal v-model="showShiftModal" width="800px">
        <shift-modal @onUpdateShift="(val) => updateShift(val)" />
      </b-modal>
    </div>
  </div>
</template>

<script>
import directHiringMixin from "../../mixins/directHiringMixin";

export default {
  data() {
    return {
      isLoading: true,
      request: null,
      cancelRequestModal: false,
      currentTab: "Detail",
      visitedTabs: ["Detail"],
      editContentModal: false,
      editContentTitle: null,
      editContentData: null,
      showShiftModal: false,
      jobTitleModal: false,
      locationModal: false,
      canSendInvitation: false,
      warningMessage: "The invitation must be sent only once every seven days.",
    };
  },
  components: {
    FloatingMenu: () => import("@/components/FloatingMenuDots"),
    Detail: () => import("@/components/agency_request/AgencyRequestDetail"),
    Workers: () => import("@/components/agency/AgencyWorkers"),
    PunchCard: () => import("@/components/agency_request/MassivePunchCard"),
    CancelList: () => import("@/components/company/CompanyCancelList"),
    EditTextarea: () => import("@/components/agency_request/EditTextarea"),
    PersonnelList: () => import("@/components/agency_request/PersonnelListModal"),
    Applicants: () => import("@/components/agency_request/Applicants"),
    ShiftModal: () => import("@/components/request/ShiftEditModal"),
  },
  mixins: [directHiringMixin],
  methods: {
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: `/agency-request/${this.$route.params.id}`,
        query: {
          tab: tab,
        },
      });
    },
    canEditRequest(request) {
      return (
        request.status === this.$statusRequested ||
        request.status === this.$statusInProcess
      );
    },
    getAgencyRequest() {
      this.$store.dispatch("agency/getAgencyRequest", this.$route.params.id)
        .then((response) => {
          this.request = response;
          this.$set(this.request, "canEdit", this.canEditRequest(response));
          this.isLoading = false;
          this.setCanSendInvitation(this.request);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    cancelRequest(reason) {
      this.cancelRequestModal = false;
      this.isLoading = true;
      this.$store.dispatch("agency/cancelRequest", {
        id: this.request.id,
        cancellationReasonId: reason.reasonId,
        otherCancellationReason: reason.otherMessage,
      })
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess(this.$t("Cancelled"));
          this.$router.push("/agency-requests");
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    editContentText(title, data) {
      this.editContentTitle = title;
      this.editContentData = data;
      this.editContentModal = true;
    },
    closeTextModal() {
      this.editContentTitle = null;
      this.editContentData = null;
      this.editContentModal = false;
    },
    updateShift(shift) {
      this.request.displayShift = shift;
      this.showShiftModal = false;
    },
    agencyRequestOpen(id) {
      this.isLoading = true;
      this.$store.dispatch("agency/agencyRequestOpen", id)
        .then(() => {
          this.isLoading = false;
          this.request.status = this.$statusInProcess;
          this.request.canEdit = true;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    setCanSendInvitation(request) {
      if (request && !request.invitationSentItAt) {
        this.canSendInvitation = true;
        return;
      }

      this.$store.dispatch("getCurrentDate").then((now) => {
        const invitationSentItAt = new Date(request.invitationSentItAt);
        invitationSentItAt.setDate(invitationSentItAt.getDate() + 7);
        if (invitationSentItAt <= now) {
          this.canSendInvitation = true;
        }
      });
    },
    sendInvitation(id) {
      this.showAlertConfirm(this.$t("AreYouSure"), this.warningMessage).then(
        (response) => {
          if (response) {
            this.isLoading = true;
            this.$store.dispatch("agency/agencyRequestSendInvitation", id)
              .then(() => {
                this.canSendInvitation = false;
                this.isLoading = false;
                this.showAlertSuccess("Sent it!");
              })
              .catch((error) => {
                this.isLoading = false;
                this.showAlertError(error);
              });
          }
        }
      );
    },
  },
  created() {
    this.getAgencyRequest();
    if (this.$route.query && this.$route.query.tab) {
      this.currentTab = this.$route.query.tab;
      if (!this.visitedTabs.includes(this.$route.query.tab)) {
        this.visitedTabs.push(this.$route.query.tab);
      }
    }
  },
  computed: {
    billingTitle() {
      if (this.request.billingTitle && this.request.jobTitle !== this.request.billingTitle) {
        return `${this.request.billingTitle}`;
      } else {
        return "";
      }
    }
  }
};
</script>
