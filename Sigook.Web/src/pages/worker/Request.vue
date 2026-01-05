<template>
  <div class="white-container-mobile wrapper-request">
    <b-loading v-model="isLoading"></b-loading>

    <section class="wrapper-request-top" v-if="request">
      <div>
        <img v-if="request.agencyLogo" :src="request.agencyLogo" />
        <h2 class="capitalize fz1 fw-700">{{ request.jobTitle }}</h2>
      </div>

      <div>
        <div v-if="request.status && request.status !== 'None'"
          class="option-request-top capitailized fw-700 is-inline-block" :class="request.status">
          {{ $t(request.status) }}
        </div>
        <div v-else class="option-request-top capitailized fw-700 is-inline-block" :class="request.requestStatus">
          {{ $t(request.requestStatus) }}
        </div>
        <div v-if="currentUser.approvedToWork" class="d-inline-block">
          <button v-if="canApply" class="orange-button md-btn background-btn btn-radius" @click="modalMessage = true">
            Apply
          </button>
        </div>
      </div>
    </section>

    <ul class="tabs-basic">
      <li class="active">Detail</li>
    </ul>

    <div class="container-flex">
      <section class="col-md-9 col-sm-12 section-left">
        <request-detail v-if="request" :request="request"></request-detail>
        <div v-if="currentUser.approvedToWork" class="mt-5">
          <div v-if="canApply">
            <button class="orange-button md-btn background-btn btn-radius" @click="modalMessage = true">
              Apply
            </button>
          </div>
        </div>
        <div class="alert-warning text-center" v-else>
          You are not approved to work
        </div>
      </section>
      <aside class="col-md-3 col-sm-12 section-right">
        <location :jobLocation="request.jobLocation" />
      </aside>
    </div>

    <!-- custom modal TextArea-->
    <transition name="modal">
      <div v-if="modalMessage" class="vue-modal header-fixed">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container modal-light overflow-initial border-radius">
              <button @click="modalMessage = false" type="button" class="cross-icon">
                close
              </button>
              <edit-textarea :title="'Additional Comments'" subtitle="Comments" :min-length="0" class="sm-edit-textarea"
                @updateContent="(data) => applyToRequest(data)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal TextArea-->
  </div>
</template>

<script>
import toast from "../../mixins/toastMixin";
export default {
  data() {
    return {
      isLoading: true,
      request: {},
      modalMessage: false,
    };
  },
  components: {
    RequestDetail: () => import("../../components/worker/RequestDetail"),
    Location: () => import("../../components/request/RequestLocation"),
    EditTextarea: () => import("../../components/agency_request/EditTextarea"),
  },
  mixins: [toast],
  methods: {
    getWorkerHistoryRequest() {
      this.$store
        .dispatch("worker/getWorkerRequestHistoryDetail", this.$route.params.id)
        .then((response) => {
          this.isLoading = false;
          this.request = response;
        })
        .catch(() => {
          this.isLoading = false;
        });
    },
    getWorkerRequest() {
      this.$store
        .dispatch("worker/getWorkerRequest", this.$route.params.id)
        .then((response) => {
          this.isLoading = false;
          this.request = response;
        })
        .catch(() => {
          this.isLoading = false;
        });
    },
    applyToRequest(comment) {
      this.isLoading = true;
      let model = {
        comments: comment,
      };
      this.$store
        .dispatch("worker/WorkerRequestApply", {
          requestId: this.request.id,
          model: model,
        })
        .then(() => {
          this.isLoading = false;
          this.$router.push({
            path: "/worker-request-applied/" + this.request.id,
          });
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
  },
  created() {
    if (this.$route.query.history) {
      this.getWorkerHistoryRequest();
    } else {
      this.getWorkerRequest();
    }
  },
  computed: {
    canApply() {
      let available = false;
      switch (this.request.requestStatus) {
        case this.$statusNone:
        case this.$statusRequested:
        case this.$statusInProcess:
          available = true;
          break;

        case this.$statusFinalized:
        case this.$statusCancelled:
          available = false;
          break;

        default:
          available = false;
          break;
      }
      switch (this.request.status) {
        case this.$statusReject:
        case this.$statusInQueue:
        case this.$statusDecline:
          available = false;
          break;
      }

      if (this.request.isApplicant) {
        available = false;
      }

      return available;
    },
    currentUser() {
      return this.$store.state.worker.workerBasic;
    },
  },
};
</script>