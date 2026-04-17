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
        <div v-if="request.status"
          class="option-request-top capitailized fw-700 is-inline-block" :class="RequestStatusLabels[request.status]">
          {{ RequestStatusLabels[request.status] }}
        </div>
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body class="is-inline-block" v-if="canEdit">
          <template #trigger>
            <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
          </template>
          <b-dropdown-item aria-role="listitem" @click="alertRequestAnotherWorker">
            Request another worker
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" @click="editContentModal = true">
            Edit Requirements
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" v-if="canCancel" @click="modalValidation = true">
            Cancel Order
          </b-dropdown-item>
        </b-dropdown>
      </div>
    </section>

    <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="request">
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
            <b-field label="Requirements" :type="requirementsError ? 'is-danger' : ''"
              :message="requirementsError || ''">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="requirements" />
              </div>
            </b-field>
          </div>
          <div class="col-12 col-padding">
            <b-button type="is-primary" @click="onUpdateRequirements()">Save</b-button>
          </div>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent, computed } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { confirmationGuard } from '@/utils/confirmationGuard';
import { isDirectHiring } from '@/utils/directHiring';
import { RequestStatus, RequestStatusLabels } from "@/constants/enums";
import { getRequest, cancelRequest, editRequest, requestAnotherWorker } from "@/api/companyApi";

const requirementsSchema = yup.object({
  requirements: yup.string().test('min-text', 'Requirements must be at least 100 characters', (v) => {
    const text = (v || '').replace(/<[^>]*>/g, '').trim();
    return text.length >= 100;
  })
});

export default {
  setup() {
    const form = useStickyForm({
      schema: requirementsSchema,
      initialValues: { requirements: '' },
    });

    const requirementsError = computed(() => form.errors.value.requirements || '');

    return {
      requirements: form.fields.requirements,
      requirementsError,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateRequirements: (value: string) => form.hydrate({ requirements: value || '' }),
    };
  },
  data() {
    return {
      request: {},
      isLoading: true,
      modalValidation: false,
      modalValidationRequestAnotherWorker: false,
      currentTab: "Detail",
      visitedTabs: ["Detail"],
      editContentModal: false,
      unsavedChanges: false,
    };
  },
  components: {
    CancelList: defineAsyncComponent(() => import("../../components/company/CompanyCancelList.vue")),
    RequestAnotherWorker: defineAsyncComponent(() => import("../../components/company/DialogRequestWorker.vue")),
    Detail: defineAsyncComponent(() => import("../../components/company_request/CompanyRequestDetail.vue")),
    Workers: defineAsyncComponent(() => import("../../components/company_request/CompanyRequestWorkers.vue")),
    PunchCard: defineAsyncComponent(() => import("../../components/company_request/CompanyRequestPunchCard.vue"))
  },
  beforeRouteLeave: confirmationGuard,
  methods: {
    cancelRequest(reason) {
      this.modalValidation = false;
      this.isLoading = true;
      cancelRequest(this.request.id, reason.reasonId, reason.otherMessage)
        .then(() => {
          this.isLoading = false;
          this.unsavedChanges = false;
          showAlertSuccess("Cancelled");
          this.$router.push("/company-requests");
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    getData() {
      getRequest(this.$route.params.id)
        .then((response) => {
          this.isLoading = false;
          this.request = response;
        })
        .catch((error) => {
          showAlertError(error.data);
          this.isLoading = false;
        });
    },
    alertRequestAnotherWorker() {
      this.modalValidationRequestAnotherWorker = true;
    },
    requestAnotherWorker(comment) {
      this.modalValidationRequestAnotherWorker = false;
      this.isLoading = true;
      requestAnotherWorker(this.$route.params.id, { comments: comment })
        .then(() => {
          this.isLoading = false;
          showAlertSuccess("Requested");
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    onUpdateRequirements() {
      this.markInteracted(['requirements']);
      this.handleSubmit((values) => {
        this.isLoading = true;
        editRequest(this.$route.params.id, { requirements: values.requirements })
          .then(() => {
            this.isLoading = false;
            showAlertSuccess("Updated");
            this.request.requirements = values.requirements;
            this.editContentModal = false;
          }).catch((error) => {
            this.isLoading = false;
            showAlertError(error.data);
          });
      })();
    },
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: `/request/${this.$route.params.id}`,
        query: {
          tab: tab
        }
      });
    }
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
  watch: {
    editContentModal(val) {
      if (val) this.hydrateRequirements(this.request.requirements);
    }
  },
  computed: {
    isDirectHiring() {
      return isDirectHiring(this.request);
    },
    RequestStatus: () => RequestStatus,
    RequestStatusLabels: () => RequestStatusLabels,
    canEdit() {
      return this.request.status === RequestStatus.Open ||
             this.request.status === RequestStatus.Filled;
    },
    canCancel() {
      // Can only cancel orders in Open status without workers
      return this.request.status === RequestStatus.Open &&
             (!this.request.workersQuantityWorking || this.request.workersQuantityWorking === 0);
    }
  }
};
</script>
