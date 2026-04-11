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
            {{ worker.firstName | lowercase }}
            {{ worker.middleName | lowercase }}
            {{ worker.lastName | lowercase }}
            {{ worker.secondLastName | lowercase }}
            <b-tooltip v-if="worker.dnu" label="DNU" type="is-dark">
              <b-icon icon="alert" size="is-small" type="is-danger"></b-icon>
            </b-tooltip>
          </h2>
        </div>
      </div>
      <div>
        <floating-menu class="is-inline-block">
          <template slot="options">
            <button class="floating-menu-item" v-if="!worker.approvedToWork" @click="onUpdateApprovedToWork(worker)">
              <span>Approve to work</span>
            </button>
            <button class="floating-menu-item" v-if="worker.approvedToWork" @click="confirmDelete(worker)">
              <span>Reject to work</span>
            </button>
          </template>
        </floating-menu>
      </div>
    </section>
    <b-tabs v-model="currentTab" @input="changeTab">
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
              <h3>{{ $t("WorkerWorkInformation") }}</h3>
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
            <b-checkbox v-model="worker.dnu" @input="toggleWorkerProfileDNU"
              :disabled="hasDnuPermission">
              {{ $t("DNU") }}
            </b-checkbox>

            <span class="line-gray" />
            <licenses :worker.sync="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <certificates :worker.sync="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray"></span>
            <other-documents :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <section class="worker-experience" id="experience">
              <div class="button-right">
                <h3>{{ $t("WorkerWorkExperience") }}</h3>
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
                          {{ $t("Close") }}
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
        <worker-settings v-if="visitedTabs.includes('workerSettings')" :worker.sync="worker" />
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
import statusWorkerMixin from "@/mixins/statusWorkerMixin";
import billingAdminMixin from "@/mixins/billingAdminMixin";
import { getCommentsWorker } from '@/api/workerApi';
import { getAgencyWorker, updateAgencyWorkerProfileDNU, updateApprovedToWork } from '@/api/agencyWorkerApi';

export default {
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
  mixins: [statusWorkerMixin, billingAdminMixin],
  components: {
    imageDetail: () => import("../../components/worker/WorkImageDetail.vue"),
    Comments: () => import("../../components/Comments.vue"),
    workExperienceForm: () => import("../../components/worker/WorkExperienceForm.vue"),
    workExperienceDetail: () => import("../../components/worker/WorkExperienceDetail.vue"),
    socialInsurance: () => import("../../components/worker/WorkSinDetail.vue"),
    basicInformation: () => import("../../components/worker/WorkBasicInformationDetail.vue"),
    emergencyInformation: () => import("../../components/worker/WorkEmergencyInformationDetail.vue"),
    documents: () => import("../../components/worker/WorkDocumentsDetail.vue"),
    resume: () => import("../../components/worker/WorkResumeDetail.vue"),
    contactInformation: () => import("../../components/worker/WorkContactInformationDetail.vue"),
    emailDetail: () => import("../../components/worker/WorkEmailDetail.vue"),
    availability: () => import("../../components/worker/WorkAvailabilitiesDetail.vue"),
    availabilityTimes: () => import("../../components/worker/WorkAvailabilityTimesDetail.vue"),
    availabilityDays: () => import("../../components/worker/WorkAvailabilityDaysDetail.vue"),
    locationPreferences: () => import("../../components/worker/WorkLocationPreferencesDetail.vue"),
    lift: () => import("../../components/worker/WorkLiftDetail.vue"),
    languages: () => import("../../components/worker/WorkLanguagesDetail.vue"),
    skills: () => import("../../components/worker/WorkSkillsDetail.vue"),
    licenses: () => import("../../components/worker/WorkLicenseDetail.vue"),
    certificates: () => import("../../components/worker/WorkCertificatesDetail.vue"),
    workerSettings: () => import('@/components/worker/WorkerSettings.vue'),
    wageHistory: () => import("../../components/worker/WorkWageHistory.vue"),
    requestHistory: () => import("../../components/agency/AgencyWorkerRequestHistory.vue"),
    timeSheetHistory: () => import("../../components/worker/TimeSheetHistory.vue"),
    notes: () => import("../../components/worker/Notes.vue"),
    FloatingMenu: () => import("../../components/FloatingMenuDots.vue"),
    otherDocuments: () => import("../../components/worker/WorkerOtherDocumentsDetail.vue"),
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
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: `/agency-workers/worker/${this.$route.params.id}`,
        query: {
          tab: tab,
        },
      });
    },
    updateComments() {
      this.isLoading = true;
      getCommentsWorker({
        workerId: this.worker.workerId,
        size: this.commentSize,
        pageIndex: this.commentPageIndex,
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
          this.showAlertError(error);
          this.isLoading = false;
        });
    },
    toggleWorkerProfileDNU() {
      this.isLoading = true;
      updateAgencyWorkerProfileDNU(this.worker.id)
        .then(() => {
          this.showAlertSuccess(this.$t("Updated"));
          this.isLoading = false;
          this.loadWorker();
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
          this.loadWorker();
        });
    },
    confirmDelete(worker) {
      this.showAlertConfirm(
        this.$t("AreYouSure"),
        this.$t("YouWantToDisableTheWorker") +
        ". " +
        this.$t("ThisWorkerWillNotBeAbleToApplyToNewRequests")
      )
        .then((response) => {
          if (response) {
            this.onUpdateApprovedToWork(worker);
          }
        })
        .catch((error) => {
          this.showAlertError(error);
        });
    },
    onUpdateApprovedToWork(worker) {
      this.isLoading = true;
      updateApprovedToWork(worker.id)
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess(this.$t("Updated"));
          this.loadWorker();
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
          this.loadWorker();
        });
    },
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
    },
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
