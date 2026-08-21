<template>
  <div class="contain-worker has-menu-bottom" v-if="worker">
    <b-loading v-model="isLoading"></b-loading>

    <Breadcrumbs :crumbs="crumbs" back-to="/recruiting/workers" />
    <section class="wrapper-worker-top mb-0">
      <div>
        <image-detail class="is-inline-block align-text-top" :data="worker" @updateProfile="() => loadWorker()" />
        <div class="is-inline-block pl-4 align-text-top">
          <h2 class="fz1 has-text-weight-bold">
            <span class="has-text-weight-normal" :class="workerColor(worker.approvedToWork, worker.isSubcontractor)">
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
        <div class="columns is-multiline">
          <section class="column is-9">
            <basic-information :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <email-detail :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <social-insurance :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <documents :worker="worker" @updateProfile="() => loadWorker()" />
            <resume :worker="worker" @updateProfile="() => loadWorker()" />

            <span class="line-gray" />
            <contact-information :worker="worker" @updateProfile="() => loadWorker()" />

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
              <div class="is-flex is-align-items-center is-justify-content-space-between">
                <h3 class="has-text-weight-bold fz-0">{{ "Work Experience" }}</h3>
                <b-button type="is-primary" icon-right="plus" @click="modalWorkExperience = true">
                  Add experience
                </b-button>
              </div>
              <ul>
                <li v-for="(item, index) in worker.jobExperiences" v-bind:class="{ active: currentJobEx === index }"
                  v-on:click="currentJobEx = Number(index)" v-bind:key="'jobExperiences' + index">
                  <work-experience-detail :item="item" :workerId="worker.id" @getWorker="() => loadWorker()" />
                </li>
              </ul>

              <b-modal custom-content-class="card" v-model="modalWorkExperience" width="500px">
                <work-experience-form :workerId="worker.id" @updateExperience="() => updateExperience()" />
              </b-modal>
            </section>

            <span class="line-gray" id="comments" />
            <comments v-if="commentsData" :worker-profile-id="worker.id" :data="commentsData" :size-comments="commentSize"
              @newComment="() => updateComments()" @changePage="(page) => changePageComments(page)" />
          </section>
          <aside class="column is-3 section-right">
            <notes />
          </aside>
        </div>
        </div>
      </b-tab-item>
      <b-tab-item label="Settings" value="workerSettings" v-if="isAdmin">
        <worker-settings v-if="visitedTabs.includes('workerSettings')" v-model:worker="worker" />
      </b-tab-item>
      <b-tab-item label="PayStubs" value="wageHistory" v-if="isAdmin">
        <wage-history v-if="visitedTabs.includes('wageHistory')" :workerId="worker.id" />
      </b-tab-item>
      <b-tab-item label="Timesheet" value="timeSheetHistory">
        <time-sheet-history v-if="visitedTabs.includes('timeSheetHistory')" :workerId="worker.id" />
      </b-tab-item>
      <b-tab-item label="Requests" value="requestHistory">
        <request-history v-if="visitedTabs.includes('requestHistory')" :workerId="worker.id" />
      </b-tab-item>
    </b-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';

import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
import { useAdmin } from '@/composables/useAdmin';
import { useModuleBase } from '@/composables/useModuleBase';
import Breadcrumbs from '@/components/Breadcrumbs.vue';
import type { PageBreadcrumb } from '@/types/common';
import { workerColor } from '@/utils/workerStatus';
import { getCommentsWorker } from '@/api/workerApi';
import { getAgencyWorker, updateAgencyWorkerProfileDNU, updateApprovedToWork } from '@/api/agencyWorkerApi';
import { lowercase } from '@/utils/filters';
import imageDetail from '@/components/worker/WorkImageDetail.vue';
import Comments from '@/components/Comments.vue';
import workExperienceForm from '@/components/worker/WorkExperienceForm.vue';
import workExperienceDetail from '@/components/worker/WorkExperienceDetail.vue';
import socialInsurance from '@/components/worker/WorkSinDetail.vue';
import basicInformation from '@/components/worker/WorkBasicInformationDetail.vue';
import emergencyInformation from '@/components/worker/WorkEmergencyInformationDetail.vue';
import documents from '@/components/worker/WorkDocumentsDetail.vue';
import resume from '@/components/worker/WorkResumeDetail.vue';
import contactInformation from '@/components/worker/WorkContactInformationDetail.vue';
import emailDetail from '@/components/worker/WorkEmailDetail.vue';
import availability from '@/components/worker/WorkAvailabilitiesDetail.vue';
import availabilityTimes from '@/components/worker/WorkAvailabilityTimesDetail.vue';
import availabilityDays from '@/components/worker/WorkAvailabilityDaysDetail.vue';
import locationPreferences from '@/components/worker/WorkLocationPreferencesDetail.vue';
import lift from '@/components/worker/WorkLiftDetail.vue';
import languages from '@/components/worker/WorkLanguagesDetail.vue';
import skills from '@/components/worker/WorkSkillsDetail.vue';
import licenses from '@/components/worker/WorkLicenseDetail.vue';
import certificates from '@/components/worker/WorkCertificatesDetail.vue';
import workerSettings from '@/components/worker/WorkerSettings.vue';
import wageHistory from '@/components/worker/WorkWageHistory.vue';
import requestHistory from '@/components/agency/AgencyWorkerRequestHistory.vue';
import timeSheetHistory from '@/components/worker/TimeSheetHistory.vue';
import notes from '@/components/worker/Notes.vue';
import otherDocuments from '@/components/worker/WorkerOtherDocumentsDetail.vue';

const route = useRoute();
const router = useRouter();
const { moduleCrumbs } = useModuleBase();
const crumbs = computed<PageBreadcrumb[]>(() => [...moduleCrumbs.value, { label: 'Workers', to: '/recruiting/workers' }]);
const { isAdmin } = useAdmin();

const currentJobEx = ref(0);
const isLoading = ref(true);
const commentSize = ref(10);
const commentPageIndex = ref(1);
const modalWorkExperience = ref(false);
const currentTab = ref<string>('profile');
const visitedTabs = ref<string[]>(['profile']);
const worker = ref<any>(null);
const commentsData = ref<any>({});

const hasDnuPermission = computed(() => {
  if (!worker.value.dnu) {
    return false;
  } else if (worker.value.dnu && isAdmin.value) {
    return false;
  }
  return true;
});

loadWorker();
if (route.query && route.query.tab) {
  currentTab.value = route.query.tab as string;
  if (!visitedTabs.value.includes(route.query.tab as string)) {
    visitedTabs.value.push(route.query.tab as string);
  }
}

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({
    path: `/recruiting/workers/${route.params.id}`,
    query: { tab: tab },
  });
}

function updateComments() {
  isLoading.value = true;
  getCommentsWorker({
    workerId: worker.value.workerId,
    size: commentSize.value,
    pageIndex: commentPageIndex.value,
  }).then((data) => {
    commentsData.value = data;
    isLoading.value = false;
  });
}

function updateExperience() {
  modalWorkExperience.value = false;
  loadWorker();
}

function changePageComments(page: number) {
  commentPageIndex.value = page;
  updateComments();
}

function loadWorker() {
  isLoading.value = true;
  getAgencyWorker(route.params.id as string)
    .then((w: any) => {
      isLoading.value = false;
      worker.value = w;
      updateComments();
    })
    .catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function toggleWorkerProfileDNU() {
  isLoading.value = true;
  updateAgencyWorkerProfileDNU(worker.value.id)
    .then(() => {
      showAlertSuccess('Updated');
      isLoading.value = false;
      loadWorker();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
      loadWorker();
    });
}

function confirmDelete(w: any) {
  showAlertConfirm(
    'Are you sure?',
    'You want to disable the worker' + '. ' + 'This worker will not be able to apply to new requests',
  )
    .then((response) => {
      if (response) {
        onUpdateApprovedToWork(w);
      }
    })
    .catch((error) => {
      showAlertError(error);
    });
}

function onUpdateApprovedToWork(w: any) {
  isLoading.value = true;
  updateApprovedToWork(w.id)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Updated');
      loadWorker();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
      loadWorker();
    });
}
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
