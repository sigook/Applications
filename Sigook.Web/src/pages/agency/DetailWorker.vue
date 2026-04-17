<template>
  <div class="contain-worker white-container-mobile has-menu-bottom" v-if="worker">
    <b-loading v-model="isLoading"></b-loading>

    <section class="wrapper-worker-top mb-0">
      <div>
        <image-detail class="d-inline-block v-top" :data="worker" @updateProfile="() => loadWorker()" />
        <div class="d-inline-block pl-4 v-top">
          <h2 class="fz1 fw-700">
            <span class="fw-400" :class="workerColor(worker.approvedToWork, worker.isSubcontractor)">
              {{ worker.numberId }}
            </span>
            {{ lowercase(worker.firstName) }}
            {{ lowercase(worker.middleName) }}
            {{ lowercase(worker.lastName) }}
            {{ lowercase(worker.secondLastName) }}
            <b-tooltip v-if="worker.dnu" label="DNU" type="is-dark" append-to-body>
              <b-icon icon="alert" size="is-small" type="is-danger"></b-icon>
            </b-tooltip>
          </h2>
        </div>
      </div>
      <div>
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body class="is-inline-block">
          <template #trigger>
            <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
          </template>
          <b-dropdown-item aria-role="listitem" v-if="!worker.approvedToWork" @click="onUpdateApprovedToWork(worker)">
            Approve to work
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" v-if="worker.approvedToWork" @click="confirmDelete(worker)">
            Reject to work
          </b-dropdown-item>
        </b-dropdown>
      </div>
    </section>
    <b-tabs v-model="currentTab" @update:modelValue="changeTab">
      <b-tab-item label="Profile" value="profile">
        <div v-if="visitedTabs.includes('profile')" class="wrapper-request">
        <div class="container-flex">
          <section class="col-md-9 col-sm-12">
            <basic-information :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <social-insurance :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <documents :worker="worker" @updateProfile="() => loadWorker()" />
            <resume :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <contact-information :worker="worker" @updateProfile="() => loadWorker()" />
            <email-detail class="mb-5" :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <emergency-information :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <section class="worker-information">
              <h3>{{ "Work information" }}</h3>
              <availability :worker="worker" @updateProfile="() => loadWorker()" />
              <availability-times :worker="worker" @updateProfile="() => loadWorker()" />
              <availability-days :worker="worker" @updateProfile="() => loadWorker()" />
              <location-preferences :worker="worker" @updateProfile="() => loadWorker()" />
              <lift :worker="worker" @updateProfile="() => loadWorker()" />
              <languages :worker="worker" @updateProfile="() => loadWorker()" />
            </section>

            <span class="padding-top" id="skills" />
            <skills :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <b-checkbox v-model="worker.dnu" @update:modelValue="toggleWorkerProfileDNU"
              :disabled="hasDnuPermission">
              {{ "DNU" }}
            </b-checkbox>

            <span class="line-gray" />
            <licenses v-model:worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <certificates v-model:worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray"></span>
            <other-documents :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <section class="worker-experience" id="experience">
              <div class="button-right">
                <h3>{{ "Work Experience" }}</h3>
                <button class="outline-btn md-btn orange-button btn-radius" @click="modalWorkExperience = true">
                  Add experience +
                </button>
              </div>
              <ul>
                <li v-for="(item, index) in worker.jobExperiences" v-bind:class="{ active: currentJobEx === index }"
                  v-on:click="currentJobEx = index" v-bind:key="'jobExperiences' + index">
                  <work-experience-detail :item="item" :workerId="worker.id" @getWorker="() => loadWorker()" />
                </li>
              </ul>

              <!-- custom modal -->
              <transition name="modal">
                <div v-if="modalWorkExperience" class="vue-modal">
                  <div class="modal-mask">
                    <div class="modal-wrapper">
                      <div class="modal-container modal-light overflow-initial">
                        <span class="fz1 fw-700">Work Experience</span>
                        <button @click="modalWorkExperience = false" class="cross-icon">
                          {{ "Close" }}
                        </button>
                        <work-experience-form :workerId="worker.id" @updateExperience="() => updateExperience()" />
                      </div>
                    </div>
                  </div>
                </div>
              </transition>
            </section>

            <span class="line-gray" />

            <span class="line-gray" id="comments" />
            <comments v-if="comments" :user-id="this.worker.workerId" :data="comments" :size-comments="this.commentSize"
              @newComment="() => updateComments()" @changePage="(page) => changePageComments(page)" />
          </section>
          <aside class="col-md-3 col-sm-12 section-right">
            <notes />
          </aside>
        </div>
        </div>
      </b-tab-item>
      <b-tab-item label="Settings" value="workerSettings" v-if="isPayrollManager">
        <worker-settings v-if="visitedTabs.includes('workerSettings')" v-model:worker="worker" />
      </b-tab-item>
      <b-tab-item label="PayStubs" value="wageHistory" v-if="isPayrollManager">
        <wage-history v-if="visitedTabs.includes('wageHistory')" :workerId="worker.id" />
      </b-tab-item>
      <b-tab-item label="Timesheet" value="timeSheetHistory">
        <time-sheet-history v-if="visitedTabs.includes('timeSheetHistory')" :workerId="worker.id" />
      </b-tab-item>
      <b-tab-item label="Orders" value="requestHistory">
        <request-history v-if="visitedTabs.includes('requestHistory')" :workerId="worker.id" />
      </b-tab-item>
    </b-tabs>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { showAlertConfirm, showAlertError, showAlertSuccess } from "@/utils/toast";
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { workerColor } from '@/utils/workerStatus';
import { getCommentsWorker } from '@/api/workerApi';
import { getAgencyWorker, updateAgencyWorkerProfileDNU, updateApprovedToWork } from '@/api/agencyWorkerApi';
import { lowercase } from '@/utils/filters';

export default {
  setup() {
    return { ...useBillingAdmin() };
  },
  data() {
    return {
      currentJobEx: 0,
      isLoading: true,
      commentSize: 10,
      commentPageIndex: 1,
      modalWorkExperience: false,
      currentTab: "profile",
      visitedTabs: ["profile"],
      worker: null,
      comments: {}
    };
  },
  components: {
    imageDetail: defineAsyncComponent(() => import("../../components/worker/WorkImageDetail.vue")),
    Comments: defineAsyncComponent(() => import("../../components/Comments.vue")),
    workExperienceForm: defineAsyncComponent(() => import("../../components/worker/WorkExperienceForm.vue")),
    workExperienceDetail: defineAsyncComponent(() => import("../../components/worker/WorkExperienceDetail.vue")),
    socialInsurance: defineAsyncComponent(() => import("../../components/worker/WorkSinDetail.vue")),
    basicInformation: defineAsyncComponent(() => import("../../components/worker/WorkBasicInformationDetail.vue")),
    emergencyInformation: defineAsyncComponent(() => import("../../components/worker/WorkEmergencyInformationDetail.vue")),
    documents: defineAsyncComponent(() => import("../../components/worker/WorkDocumentsDetail.vue")),
    resume: defineAsyncComponent(() => import("../../components/worker/WorkResumeDetail.vue")),
    contactInformation: defineAsyncComponent(() => import("../../components/worker/WorkContactInformationDetail.vue")),
    emailDetail: defineAsyncComponent(() => import("../../components/worker/WorkEmailDetail.vue")),
    availability: defineAsyncComponent(() => import("../../components/worker/WorkAvailabilitiesDetail.vue")),
    availabilityTimes: defineAsyncComponent(() => import("../../components/worker/WorkAvailabilityTimesDetail.vue")),
    availabilityDays: defineAsyncComponent(() => import("../../components/worker/WorkAvailabilityDaysDetail.vue")),
    locationPreferences: defineAsyncComponent(() => import("../../components/worker/WorkLocationPreferencesDetail.vue")),
    lift: defineAsyncComponent(() => import("../../components/worker/WorkLiftDetail.vue")),
    languages: defineAsyncComponent(() => import("../../components/worker/WorkLanguagesDetail.vue")),
    skills: defineAsyncComponent(() => import("../../components/worker/WorkSkillsDetail.vue")),
    licenses: defineAsyncComponent(() => import("../../components/worker/WorkLicenseDetail.vue")),
    certificates: defineAsyncComponent(() => import("../../components/worker/WorkCertificatesDetail.vue")),
    workerSettings: defineAsyncComponent(() => import('@/components/worker/WorkerSettings.vue')),
    wageHistory: defineAsyncComponent(() => import("../../components/worker/WorkWageHistory.vue")),
    requestHistory: defineAsyncComponent(() => import("../../components/agency/AgencyWorkerRequestHistory.vue")),
    timeSheetHistory: defineAsyncComponent(() => import("../../components/worker/TimeSheetHistory.vue")),
    notes: defineAsyncComponent(() => import("../../components/worker/Notes.vue")),
    otherDocuments: defineAsyncComponent(() => import("../../components/worker/WorkerOtherDocumentsDetail.vue"))
  },
  async created() {
    this.loadWorker();
    if (this.$route.query && this.$route.query.tab) {
      this.currentTab = this.$route.query.tab;
      if (!this.visitedTabs.includes(this.$route.query.tab)) {
        this.visitedTabs.push(this.$route.query.tab);
      }
    }
  },
  methods: {
    lowercase,
    workerColor,
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: `/agency-workers/worker/${this.$route.params.id}`,
        query: {
          tab: tab
        }
      });
    },
    updateComments() {
      this.isLoading = true;
      getCommentsWorker({
        workerId: this.worker.workerId,
        size: this.commentSize,
        pageIndex: this.commentPageIndex
      })
        .then((data) => {
          this.comments = data;
          this.isLoading = false;
        });
    },
    updateExperience() {
      this.modalWorkExperience = false;
      this.loadWorker();
    },
    changePageComments(page) {
      this.commentPageIndex = page;
      this.updateComments();
    },
    loadWorker() {
      this.isLoading = true;
      getAgencyWorker(this.$route.params.id)
        .then((worker) => {
          this.isLoading = false;
          this.worker = worker;
          this.updateComments();
        })
        .catch((error) => {
          showAlertError(error);
          this.isLoading = false;
        });
    },
    toggleWorkerProfileDNU() {
      this.isLoading = true;
      updateAgencyWorkerProfileDNU(this.worker.id)
        .then(() => {
          showAlertSuccess("Updated");
          this.isLoading = false;
          this.loadWorker();
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
          this.loadWorker();
        });
    },
    confirmDelete(worker) {
      showAlertConfirm(
        "Are you sure?",
        "You want to disable the worker" +
        ". " +
        "This worker will not be able to apply to new requests"
      )
        .then((response) => {
          if (response) {
            this.onUpdateApprovedToWork(worker);
          }
        })
        .catch((error) => {
          showAlertError(error);
        });
    },
    onUpdateApprovedToWork(worker) {
      this.isLoading = true;
      updateApprovedToWork(worker.id)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess("Updated");
          this.loadWorker();
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
          this.loadWorker();
        });
    }
  },
  computed: {
    hasDnuPermission() {
      if (!this.worker.dnu) {
        return false;
      } else if (
        this.worker.dnu && this.isPayrollManager
      ) {
        return false;
      } else {
        return true;
      }
    }
  }
};
</script>

<style lang="scss" scoped>
@import "../../assets/scss/detail-worker";

.icon-hash {
  font-weight: 200;
  margin: 0 0 5px;

  &:before {
    content: "#";
    font-size: 16px;
    padding: 0 15px 0 8px;
    font-weight: 400;
  }
}
</style>
