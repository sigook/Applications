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
          class="option-request-top uppercase fw-700 is-inline-block" :class="getStatusColorClass(request)">
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
            <template v-if="request.status === $statusOpen">
              <button v-if="canSendInvitation" class="floating-menu-item" v-on:click="sendInvitation(request.id)">
                <span>Send an email invitation</span>
              </button>
              <button disabled v-else class="floating-menu-item" :title="warningMessage">
                <span>Send an email invitation
                  <span class="fz-1">
                    (Sent it {{ request.invitationSentItAt | dateFromNow }})</span></span>
              </button>
            </template>
            <button class="floating-menu-item" v-if="request.canCancel" v-on:click="cancelRequestModal = true">
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
        <detail v-if="visitedTabs.includes('Detail')" :key="request.id + '-' + request.workersQuantity + '-' + request.status" :request="request" class="p-2 p-sm-0" @refreshRequest="onRefreshRequest" />
      </b-tab-item>
      <b-tab-item label="Applicants" value="Applicants">
        <applicants v-if="visitedTabs.includes('Applicants')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
      <b-tab-item label="Workers" value="Workers">
        <workers v-if="visitedTabs.includes('Workers')" :request="request" class="p-2 p-sm-0" @refreshRequest="onRefreshRequest" />
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

<script lang="ts">
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
    FloatingMenu: () => import("@/components/FloatingMenuDots.vue"),
    Detail: () => import("@/components/agency_request/AgencyRequestDetail.vue"),
    Workers: () => import("@/components/agency/AgencyWorkers.vue"),
    PunchCard: () => import("@/components/agency_request/MassivePunchCard.vue"),
    CancelList: () => import("@/components/company/CompanyCancelList.vue"),
    Applicants: () => import("@/components/agency_request/Applicants.vue"),
    ShiftModal: () => import("@/components/request/ShiftEditModal.vue"),
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
      return request.status === this.$statusOpen ||
             request.status === this.$statusFilled;
    },
    canCancelRequest(request) {
      // Can only cancel orders in Open status without workers
      return request.status === this.$statusOpen &&
             (!request.workersQuantityWorking || request.workersQuantityWorking === 0);
    },
    getAgencyRequest() {
      console.log('📡 Loading request data from API...');
      this.isLoading = true;
      this.$store.dispatch("agency/getAgencyRequest", this.$route.params.id)
        .then((response) => {
          console.log('📥 API response received:', {
            id: response.id,
            workersQuantity: response.workersQuantity,
            workersQuantityWorking: response.workersQuantityWorking,
            status: response.status
          });
          // Use $set to ensure Vue detects the change
          const updatedRequest = Object.assign({}, response, {
            canEdit: this.canEditRequest(response),
            canCancel: this.canCancelRequest(response)
          });
          this.$set(this, 'request', updatedRequest);
          console.log('✅ Request data updated successfully');
          this.setCanSendInvitation(this.request);
          this.isLoading = false;
        })
        .catch((error) => {
          console.error('❌ Error loading request:', error);
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
          this.request.status = this.$statusOpen;
          this.request.canEdit = this.canEditRequest(this.request);
          this.request.canCancel = this.canCancelRequest(this.request);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    setCanSendInvitation(request) {
      // Cannot send invitations to Filled orders
      if (request.status === this.$statusFilled) {
        this.canSendInvitation = false;
        return;
      }

      if (request && !request.invitationSentItAt) {
        this.canSendInvitation = true;
        return;
      }

      this.$store.dispatch("getCurrentDate").then((now) => {
        const invitationSentItAt = new Date(request.invitationSentItAt);
        invitationSentItAt.setDate(invitationSentItAt.getDate() + 7);
        if (invitationSentItAt <= now) {
          this.canSendInvitation = true;
        } else {
          this.canSendInvitation = false;
        }
      });
    },
    onRefreshRequest() {
      console.log('🔄 Refresh request event received, calling getAgencyRequest()');
      // Refresh the entire request from the API to get updated status
      this.getAgencyRequest();
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
    getStatusColorClass(request) {
      // Return text color class matching TableRequests visual style
      // Show blue color (like InProgress) for Open orders with workers but not full
      if (request.status === this.$statusOpen &&
          request.workersQuantityWorking > 0 &&
          request.workersQuantityWorking < request.workersQuantity) {
        return 'Book'; // Blue color (similar to InProgress)
      }
      // Return standard status color classes
      return request.status; // Open (orange), Filled (green), Cancelled (red)
    }
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
  },
  watch: {
    request: {
      handler(newVal, oldVal) {
        if (newVal && oldVal) {
          console.log('👁️ Request data changed:', {
            old: {
              workersQuantity: oldVal.workersQuantity,
              workersQuantityWorking: oldVal.workersQuantityWorking,
              status: oldVal.status
            },
            new: {
              workersQuantity: newVal.workersQuantity,
              workersQuantityWorking: newVal.workersQuantityWorking,
              status: newVal.status
            },
            componentKey: newVal.id + '-' + newVal.workersQuantity + '-' + newVal.status
          });
        }
      },
      deep: true
    }
  }
};
</script>
