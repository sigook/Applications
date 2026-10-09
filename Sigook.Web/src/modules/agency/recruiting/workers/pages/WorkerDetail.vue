<template>
  <div class="contain-worker has-menu-bottom" v-if="worker">
    <b-loading v-model="isLoading"></b-loading>

    <Breadcrumbs :crumbs="crumbs" back-to="/recruiting/workers" />
    <worker-profile-header class="worker-card worker-profile-header" :worker="worker"
      :number-class="workerColor(worker.approvedToWork, worker.isSubcontractor)" @updateProfile="loadWorker">
      <template #chips>
        <div class="worker-chips">
          <span v-if="worker.approvedToWork" class="worker-chip is-success">Approved to work</span>
          <span v-else class="worker-chip is-danger">Not approved</span>
          <span v-if="worker.isSubcontractor" class="worker-chip is-info">Subcontractor</span>
          <span v-if="worker.isContractor" class="worker-chip">Contractor</span>
          <span v-if="worker.dnu" class="worker-chip is-danger">DNU</span>
          <b-button v-if="attentionItems.length" size="is-small" rounded class="worker-chip-button"
            @click="scrollToWorkerSection(attentionItems[0].sectionId)">
            {{ attentionItems.length }} {{ attentionItems.length === 1 ? 'item needs' : 'items need' }} attention
          </b-button>
        </div>
      </template>
      <template #actions>
        <b-button icon-left="comment-outline" @click="openCommentDialog">Add comment</b-button>
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
          <template #trigger>
            <b-button icon-right="dots-vertical" aria-label="More actions" />
          </template>
          <b-dropdown-item aria-role="listitem" v-if="!worker.approvedToWork" @click="onUpdateApprovedToWork(worker)">
            Approve to work
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" v-if="worker.approvedToWork" @click="confirmDelete(worker)">
            Reject to work
          </b-dropdown-item>
        </b-dropdown>
      </template>
    </worker-profile-header>
    <b-tabs v-model="currentTab" @update:modelValue="changeTab">
      <b-tab-item label="Profile" value="profile">
        <div v-if="visitedTabs.includes('profile')" class="worker-profile-layout">
          <profile-index class="worker-profile-layout-index" :sections="sections" :completeness="completeness"
            :missing-labels="missingLabels" :active-id="activeSectionId" @select="selectSection" />

          <div class="worker-profile-layout-content">
            <personal-card :id="workerSectionAnchor('personal')" class="worker-card worker-section" :worker="worker"
              :update-email="updateAgencyWorkerEmail" @updateProfile="loadWorker" />
            <contact-card :id="workerSectionAnchor('contact')" class="worker-card worker-section" :worker="worker"
              @updateProfile="loadWorker" />
            <documents-card :id="workerSectionAnchor('documents')" class="worker-card worker-section" :worker="worker"
              @updateProfile="loadWorker" @loading="(value) => (isLoading = value)" />
            <preferences-card :id="workerSectionAnchor('preferences')" class="worker-card worker-section" :worker="worker"
              @updateProfile="loadWorker" />
            <skills-card :id="workerSectionAnchor('skills')" class="worker-card worker-section" :worker="worker"
              @updateProfile="loadWorker" />
            <experience-card :id="workerSectionAnchor('experience')" class="worker-card worker-section" :worker="worker"
              @updateProfile="loadWorker" @loading="(value) => (isLoading = value)" />

            <comments-card v-if="commentsData" ref="commentsCard" :id="workerSectionAnchor('comments')"
              class="worker-card worker-section" :worker-profile-id="worker.id" :comments="commentsData"
              :page-index="commentPageIndex" :page-size="commentSize" :create-comment="agencyCommentWorker" @commentCreated="updateComments"
              @changePage="changePageComments" @loading="(value) => (isLoading = value)" />
          </div>

          <aside class="worker-profile-layout-rail">
            <needs-attention :items="attentionItems" @select="selectSection" />
            <div class="worker-card">
              <notes />
            </div>
            <div class="worker-card">
              <div class="worker-card-header">
                <h3>Flags</h3>
              </div>
              <b-checkbox v-model="worker.dnu" @update:modelValue="toggleWorkerProfileDNU" :disabled="hasDnuPermission">
                Do not use (DNU)
              </b-checkbox>
            </div>
          </aside>
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
import { ref, computed, nextTick } from 'vue';
import { useRoute, useRouter } from 'vue-router';

import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { useAdmin } from '@/shared/composables/useAdmin';
import { useModuleBase } from '@/modules/agency/shared/useModuleBase';
import Breadcrumbs from '@/shared/ui/Breadcrumbs.vue';
import type { PageBreadcrumb } from '@/shared/types/common';
import { workerColor } from '@/modules/agency/recruiting/workers/workerStatus';
import { agencyCommentWorker, getAgencyWorker, getAgencyWorkerComments, updateAgencyWorkerEmail, updateAgencyWorkerProfileDNU, updateApprovedToWork } from '@/modules/agency/recruiting/workers/api';
import workerSettings from '@/modules/agency/recruiting/workers/components/WorkerSettings.vue';
import wageHistory from '@/modules/agency/recruiting/workers/components/WorkWageHistory.vue';
import requestHistory from '@/modules/agency/recruiting/workers/components/AgencyWorkerRequestHistory.vue';
import timeSheetHistory from '@/modules/agency/recruiting/workers/components/TimeSheetHistory.vue';
import notes from '@/modules/agency/recruiting/workers/components/Notes.vue';
import WorkerProfileHeader from '@/shared/worker-profile/cards/WorkerProfileHeader.vue';
import ProfileIndex from '@/shared/worker-profile/cards/ProfileIndex.vue';
import NeedsAttention from '@/shared/worker-profile/cards/NeedsAttention.vue';
import PersonalCard from '@/shared/worker-profile/cards/PersonalCard.vue';
import ContactCard from '@/shared/worker-profile/cards/ContactCard.vue';
import DocumentsCard from '@/shared/worker-profile/cards/DocumentsCard.vue';
import PreferencesCard from '@/shared/worker-profile/cards/PreferencesCard.vue';
import SkillsCard from '@/shared/worker-profile/cards/SkillsCard.vue';
import ExperienceCard from '@/shared/worker-profile/cards/ExperienceCard.vue';
import CommentsCard from '@/shared/worker-profile/cards/CommentsCard.vue';
import { scrollToWorkerSection, useActiveWorkerSection, useWorkerProfileStatus, workerSectionAnchor } from '@/shared/worker-profile/useWorkerProfileStatus';
import type { WorkerCommentList, WorkerProfileDetail } from '@/shared/worker-profile/types';

const route = useRoute();
const router = useRouter();
const { moduleCrumbs } = useModuleBase();
const crumbs = computed<PageBreadcrumb[]>(() => [...moduleCrumbs.value, { label: 'Workers', to: '/recruiting/workers' }]);
const { isAdmin } = useAdmin();

const isLoading = ref(true);
const commentSize = ref(10);
const commentPageIndex = ref(1);
const currentTab = ref<string>('profile');
const visitedTabs = ref<string[]>(['profile']);
const worker = ref<WorkerProfileDetail | null>(null);
const commentsData = ref<WorkerCommentList | null>(null);
const commentsCard = ref<InstanceType<typeof CommentsCard> | null>(null);

const { attentionItems, sections, completeness, missingLabels } = useWorkerProfileStatus(worker);
const { activeSectionId, selectSection, observeSections } = useActiveWorkerSection(sections);

const hasDnuPermission = computed(() => {
  if (!worker.value?.dnu) {
    return false;
  } else if (isAdmin.value) {
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
  getAgencyWorkerComments(worker.value!.id, {
    size: commentSize.value,
    pageIndex: commentPageIndex.value,
  })
    .then((data) => {
      commentsData.value = data;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function openCommentDialog() {
  scrollToWorkerSection('comments');
  commentsCard.value?.open();
}

function changePageComments(page: number) {
  commentPageIndex.value = page;
  updateComments();
}

function loadWorker() {
  isLoading.value = true;
  getAgencyWorker(route.params.id as string)
    .then((w) => {
      isLoading.value = false;
      worker.value = w;
      updateComments();
      nextTick(observeSections);
    })
    .catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function toggleWorkerProfileDNU() {
  isLoading.value = true;
  updateAgencyWorkerProfileDNU(worker.value!.id)
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

function confirmDelete(w: WorkerProfileDetail) {
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

function onUpdateApprovedToWork(w: WorkerProfileDetail) {
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
@import "@/assets/scss/worker-profile-layout";

.contain-worker {
  padding: 20px;

  @media (max-width: 970px) {
    padding: 0 50px;
  }

  @media (max-width: 767px) {
    padding: 15px;
  }
}

:where(.contain-worker) :deep(h2) {
  margin: 0;
}

:where(.contain-worker) :deep(h3) {
  margin: 15px 0;
  font-size: 1.15em;
}

:where(.contain-worker) :deep(section) {
  margin-bottom: 25px;

  @media (max-width: 767px) {
    margin-top: 10px;
    margin-bottom: 10px;
  }
}

:where(.contain-worker) :deep(.line-gray) {
  margin-bottom: 10px;
}

.worker-profile-header {
  margin-bottom: 16px;
}
</style>
