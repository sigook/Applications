<template>
  <div class="worker-profile-page">
    <b-loading v-model="isLoading"></b-loading>

    <template v-if="worker">
      <worker-profile-header class="worker-card worker-profile-header" :worker="worker" @updateProfile="loadProfile">
        <template #chips>
          <div class="worker-chips">
            <span v-if="worker.approvedToWork" class="worker-chip is-success">Approved to work</span>
            <span v-else class="worker-chip is-warning">Pending approval</span>
            <b-button v-if="attentionItems.length" size="is-small" rounded class="worker-chip-button"
              @click="selectSection(attentionItems[0].sectionId)">
              {{ attentionItems.length }} {{ attentionItems.length === 1 ? 'item needs' : 'items need' }} attention
            </b-button>
          </div>
        </template>
      </worker-profile-header>

      <b-tabs v-model="currentTab" @update:modelValue="changeTab">
        <b-tab-item label="Profile" value="profile">
          <div class="worker-profile-layout">
            <profile-index class="worker-profile-layout-index" :sections="sections" :completeness="completeness"
              :missing-labels="missingLabels" :active-id="activeSectionId" @select="selectSection" />

            <div class="worker-profile-layout-content">
              <personal-card :id="workerSectionAnchor('personal')" class="worker-card worker-section" :worker="worker"
                :show-login-email="false" @updateProfile="loadProfile" />
              <contact-card :id="workerSectionAnchor('contact')" class="worker-card worker-section" :worker="worker"
                @updateProfile="loadProfile" />
              <documents-card :id="workerSectionAnchor('documents')" class="worker-card worker-section" :worker="worker"
                @updateProfile="loadProfile" @loading="(value) => (isLoading = value)" />
              <preferences-card :id="workerSectionAnchor('preferences')" class="worker-card worker-section"
                :worker="worker" @updateProfile="loadProfile" />
              <skills-card :id="workerSectionAnchor('skills')" class="worker-card worker-section" :worker="worker"
                @updateProfile="loadProfile" />
              <experience-card :id="workerSectionAnchor('experience')" class="worker-card worker-section"
                :worker="worker" @updateProfile="loadProfile" @loading="(value) => (isLoading = value)" />
              <comments-card v-if="commentsData" :id="workerSectionAnchor('comments')" class="worker-card worker-section"
                :worker-profile-id="worker.id" :comments="commentsData" :page-index="commentPageIndex"
                :page-size="commentSize" readonly title="Comments from your agency" @changePage="changePageComments" />
            </div>

            <aside class="worker-profile-layout-rail">
              <approval-status-card class="worker-card" :approved="worker.approvedToWork" />
              <needs-attention :items="attentionItems" @select="selectSection" />
            </aside>
          </div>
        </b-tab-item>

        <b-tab-item label="Account" value="account">
          <worker-account-security v-if="visitedTabs.includes('account')" />
        </b-tab-item>
      </b-tabs>
    </template>
  </div>
</template>

<script setup lang="ts">
import { nextTick, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useWorkerStore } from '@/modules/worker/store';
import { showAlertError } from '@/shared/utils/toast';
import { getMyComments, getMyProfile } from '@/modules/worker/profile/api';
import { useActiveWorkerSection, useWorkerProfileStatus, workerSectionAnchor } from '@/shared/worker-profile/useWorkerProfileStatus';
import type { WorkerCommentList, WorkerProfileDetail } from '@/shared/worker-profile/types';
import WorkerProfileHeader from '@/shared/worker-profile/cards/WorkerProfileHeader.vue';
import ProfileIndex from '@/shared/worker-profile/cards/ProfileIndex.vue';
import NeedsAttention from '@/shared/worker-profile/cards/NeedsAttention.vue';
import ApprovalStatusCard from '@/modules/worker/profile/components/ApprovalStatusCard.vue';
import PersonalCard from '@/shared/worker-profile/cards/PersonalCard.vue';
import ContactCard from '@/shared/worker-profile/cards/ContactCard.vue';
import DocumentsCard from '@/shared/worker-profile/cards/DocumentsCard.vue';
import PreferencesCard from '@/shared/worker-profile/cards/PreferencesCard.vue';
import SkillsCard from '@/shared/worker-profile/cards/SkillsCard.vue';
import ExperienceCard from '@/shared/worker-profile/cards/ExperienceCard.vue';
import CommentsCard from '@/shared/worker-profile/cards/CommentsCard.vue';
import WorkerAccountSecurity from '@/modules/worker/profile/components/WorkerAccountSecurity.vue';

type WorkerProfileTab = 'profile' | 'account';

const route = useRoute();
const router = useRouter();
const workerStore = useWorkerStore();

const isLoading = ref(true);
const commentSize = 10;
const commentPageIndex = ref(1);
const worker = ref<WorkerProfileDetail | null>(null);
const commentsData = ref<WorkerCommentList | null>(null);
const currentTab = ref<WorkerProfileTab>(route.query.tab === 'account' ? 'account' : 'profile');
const visitedTabs = ref<WorkerProfileTab[]>([currentTab.value]);

const { attentionItems, sections, completeness, missingLabels } = useWorkerProfileStatus(worker);
const { activeSectionId, selectSection, observeSections } = useActiveWorkerSection(sections);

function changeTab(tab: WorkerProfileTab) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({ path: '/worker-profile', query: { tab } });
}

function loadComments() {
  getMyComments({ size: commentSize, pageIndex: commentPageIndex.value })
    .then((data) => {
      commentsData.value = data;
    })
    .catch((error) => showAlertError(error));
}

function changePageComments(page: number) {
  commentPageIndex.value = page;
  loadComments();
}

function loadProfile() {
  isLoading.value = true;
  getMyProfile()
    .then((data) => {
      worker.value = data;
      workerStore.setWorkerProfile(data);
      isLoading.value = false;
      nextTick(observeSections);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

loadProfile();
loadComments();
</script>

<style lang="scss" scoped>
@import '@/assets/scss/worker-profile-layout';

.worker-profile-page {
  padding: 20px;

  @include mobile {
    padding: 16px;
  }
}

.worker-profile-header {
  margin-bottom: 16px;
}

.worker-profile-layout-rail {
  @include touch {
    order: -1;
  }
}
</style>
