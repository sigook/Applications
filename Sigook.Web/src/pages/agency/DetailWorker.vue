<template>
  <div class="contain-worker has-menu-bottom" v-if="worker">
    <b-loading v-model="isLoading"></b-loading>

    <Breadcrumbs :crumbs="crumbs" back-to="/recruiting/workers" />
    <section class="worker-card worker-header">
      <image-detail class="worker-header-photo" :data="worker" @updateProfile="() => loadWorker()" />
      <div class="worker-header-main">
        <h2 class="worker-header-name">
          {{ lowercase(worker.firstName) }}
          {{ lowercase(worker.middleName) }}
          {{ lowercase(worker.lastName) }}
          {{ lowercase(worker.secondLastName) }}
          <span class="worker-header-number" :class="workerColor(worker.approvedToWork, worker.isSubcontractor)">
            #{{ worker.numberId }}
          </span>
        </h2>
        <div class="worker-chips">
          <span v-if="worker.approvedToWork" class="worker-chip is-success">Approved to work</span>
          <span v-else class="worker-chip is-danger">Not approved</span>
          <span v-if="worker.isSubcontractor" class="worker-chip is-info">Subcontractor</span>
          <span v-if="worker.isContractor" class="worker-chip">Contractor</span>
          <span v-if="worker.dnu" class="worker-chip is-danger">DNU</span>
          <button v-if="attentionItems.length" type="button" class="worker-chip is-warning"
            @click="scrollToWorkerSection(attentionItems[0].sectionId)">
            {{ attentionItems.length }} {{ attentionItems.length === 1 ? 'item needs' : 'items need' }} attention
          </button>
        </div>
        <div class="worker-header-contact">
          <span v-if="worker.mobileNumber"><b-icon icon="cellphone" size="is-small" />{{ worker.mobileNumber }}</span>
          <a v-if="worker.email" :href="`mailto:${worker.email}`"><b-icon icon="email-outline" size="is-small" />{{ worker.email }}</a>
          <span v-if="worker.location?.city"><b-icon icon="map-marker-outline" size="is-small" />{{ headerCity }}</span>
        </div>
      </div>
      <div class="worker-header-actions">
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
      </div>
    </section>
    <b-tabs v-model="currentTab" @update:modelValue="changeTab">
      <b-tab-item label="Profile" value="profile">
        <div v-if="visitedTabs.includes('profile')" class="worker-profile-layout">
          <profile-index class="worker-profile-layout-index" :sections="sections" :completeness="completeness"
            :missing-labels="missingLabels" :active-id="activeSectionId" @select="selectSection" />

          <div class="worker-profile-layout-content">
            <personal-card :id="workerSectionAnchor('personal')" class="worker-card worker-section" :worker="worker"
              @updateProfile="loadWorker" />
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
              :page-index="commentPageIndex" :page-size="commentSize" @commentCreated="updateComments"
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
import { ref, computed, nextTick, onBeforeUnmount } from 'vue';
import { useRoute, useRouter } from 'vue-router';

import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
import { useAdmin } from '@/composables/useAdmin';
import { useModuleBase } from '@/composables/useModuleBase';
import Breadcrumbs from '@/components/Breadcrumbs.vue';
import type { PageBreadcrumb } from '@/types/common';
import { workerColor } from '@/utils/workerStatus';
import { getAgencyWorker, getAgencyWorkerComments, updateAgencyWorkerProfileDNU, updateApprovedToWork } from '@/api/agencyWorkerApi';
import { lowercase } from '@/utils/filters';
import imageDetail from '@/components/worker/WorkImageDetail.vue';
import workerSettings from '@/components/worker/WorkerSettings.vue';
import wageHistory from '@/components/worker/WorkWageHistory.vue';
import requestHistory from '@/components/agency/AgencyWorkerRequestHistory.vue';
import timeSheetHistory from '@/components/worker/TimeSheetHistory.vue';
import notes from '@/components/worker/Notes.vue';
import ProfileIndex from '@/components/agency_worker/ProfileIndex.vue';
import NeedsAttention from '@/components/agency_worker/NeedsAttention.vue';
import PersonalCard from '@/components/agency_worker/PersonalCard.vue';
import ContactCard from '@/components/agency_worker/ContactCard.vue';
import DocumentsCard from '@/components/agency_worker/DocumentsCard.vue';
import PreferencesCard from '@/components/agency_worker/PreferencesCard.vue';
import SkillsCard from '@/components/agency_worker/SkillsCard.vue';
import ExperienceCard from '@/components/agency_worker/ExperienceCard.vue';
import CommentsCard from '@/components/agency_worker/CommentsCard.vue';
import { scrollToWorkerSection, useWorkerProfileStatus, workerSectionAnchor } from '@/composables/useWorkerProfileStatus';
import type { WorkerCommentList, WorkerProfileDetail, WorkerProfileSectionId } from '@/types/worker';

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
const activeSectionId = ref<WorkerProfileSectionId | null>('personal');
let sectionObserver: IntersectionObserver | null = null;

const { attentionItems, sections, completeness, missingLabels } = useWorkerProfileStatus(worker);

const headerCity = computed(() => {
  const city = worker.value?.location?.city;
  if (!city) {
    return '';
  }
  return city.province?.code ? `${city.value}, ${city.province.code}` : city.value;
});

const hasDnuPermission = computed(() => {
  if (!worker.value?.dnu) {
    return false;
  } else if (isAdmin.value) {
    return false;
  }
  return true;
});

function selectSection(id: WorkerProfileSectionId) {
  activeSectionId.value = id;
  scrollToWorkerSection(id);
}

function observeSections() {
  if (sectionObserver || typeof IntersectionObserver === 'undefined') {
    return;
  }
  sectionObserver = new IntersectionObserver(
    (entries) => {
      const visible = entries.find((e) => e.isIntersecting);
      if (visible) {
        const id = sections.value.find((s) => workerSectionAnchor(s.id) === visible.target.id)?.id;
        if (id) {
          activeSectionId.value = id;
        }
      }
    },
    { rootMargin: '-80px 0px -70% 0px' },
  );
  sections.value.forEach((s) => {
    const el = document.getElementById(workerSectionAnchor(s.id));
    if (el) {
      sectionObserver?.observe(el);
    }
  });
}

onBeforeUnmount(() => sectionObserver?.disconnect());

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
@import "../../assets/scss/variables";
@import "../../assets/scss/breakpoints";

.worker-card {
  background: $white;
  border: 1px solid $gray-border;
  border-radius: 12px;
  padding: 20px 24px;
  margin: 0;
}

.worker-profile-index.worker-card {
  padding: 12px;
}

.worker-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 12px;

  h3 {
    margin: 0;
    font-weight: 700;
  }
}

.worker-header {
  display: flex;
  align-items: flex-start;
  gap: 20px;

  &.worker-card {
    margin-bottom: 16px;
  }

  @include mobile {
    flex-wrap: wrap;
  }
}

.worker-header-photo {
  flex-shrink: 0;
}

.worker-header-main {
  flex-grow: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.worker-header-name {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  line-height: 1.25;
}

.worker-header-number {
  margin-left: 6px;
  font-size: 1rem;
  font-weight: 400;
  color: $grey-font;
}

.worker-header-contact {
  display: flex;
  flex-wrap: wrap;
  gap: 6px 24px;
  font-size: 0.9rem;
  color: $grey-font;

  span,
  a {
    display: inline-flex;
    align-items: center;
    gap: 4px;
  }

  a {
    color: $blue;
  }
}

.worker-header-actions {
  display: flex;
  gap: 8px;
  flex-shrink: 0;

  @include mobile {
    width: 100%;
  }
}

.worker-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.worker-chip {
  display: inline-flex;
  align-items: center;
  padding: 2px 10px;
  border: 0;
  border-radius: 999px;
  font: inherit;
  font-size: 0.75rem;
  font-weight: 600;
  background: $gray-bg;
  color: $grey-font;

  &.is-success {
    background: rgba($green, 0.18);
    color: $green-text;
  }

  &.is-danger {
    background: rgba($danger, 0.1);
    color: $danger-hover;
  }

  &.is-warning {
    background: rgba($accent, 0.16);
    color: $accent-text;
  }

  &.is-info {
    background: rgba($blue, 0.1);
    color: $blue-dark;
  }
}

button.worker-chip {
  cursor: pointer;
}

.worker-profile-layout {
  display: grid;
  grid-template-columns: 220px minmax(0, 1fr) 300px;
  gap: 24px;
  align-items: start;

  @include compact-desktop {
    grid-template-columns: 190px minmax(0, 1fr) 260px;
    gap: 16px;
  }

  @include touch {
    grid-template-columns: minmax(0, 1fr);
  }
}

.worker-profile-layout-index {
  position: sticky;
  top: 68px;

  @include touch {
    display: none;
  }
}

.worker-profile-layout-content,
.worker-profile-layout-rail {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.worker-section {
  scroll-margin-top: 68px;
}

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
