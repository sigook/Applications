<template>
  <div>
    <b-loading :model-value="isLoading"></b-loading>
    <!-- If is not approved to work -->
    <b-message type="is-warning" v-if="isProfileLoaded && !currentUser.approvedToWork" :closable="false">
      You are not approved to work
    </b-message>
    <b-message type="is-warning" v-if="missingRequirements.length" :closable="false">
      <p><b>Your Profile is Incomplete</b></p>
      <p class="fz-1">Please update the following information to be able to work with us.</p>
      <ul class="normal-list fz-1">
        <li v-for="requirement in missingRequirements" :key="requirement">
          <router-link :to="{ path: '/worker-profile', query: { section: workerRequirementSection(requirement) } }">
            {{ workerRequirementLabel(requirement) }}
          </router-link>
        </li>
      </ul>
      <b-button tag="router-link" to="/worker-profile" type="is-danger" class="mtop-5">
        OPEN MY PROFILE
      </b-button>
    </b-message>
    <h2 class="fz1 pt-3">Jobs</h2>
    <div>
      <SigookGrid :fetch="getJobs" v-model:params="serverParams" @update:loading="(value) => isGridLoading = value" @click="onRowClick">
        <template #mobile-card="{ row }">
          <JobCard :job="row" clickable @click="onRowClick(row)" />
        </template>
        <b-table-column field="numberId" label="Request ID" v-slot="props">
          {{ props.row.numberId }}
          <p v-if="props.row.isAsap" class="asap">{{ "Asap" }}</p>
        </b-table-column>
        <b-table-column field="jobTitle" label="Position" v-slot="props">
          {{ props.row.jobTitle }}
        </b-table-column>
        <b-table-column field="location" label="Location" v-slot="props">
          {{ props.row.location }}
          <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
        </b-table-column>
        <b-table-column field="startAt">
          <template v-slot:header>
            <p class="has-text-weight-semibold">Duration</p>
            <p class="has-text-weight-semibold">(Start - End)</p>
          </template>
          <template v-slot="props">
            {{ dateMonth(props.row.startAt) }}
            <span v-if="props.row.durationTerm !== appGlobals.$longTerm">
              - {{ dateMonth(props.row.finishAt) }}
            </span>
            <span
              v-if="(props.row.status === appGlobals.$statusFilled || props.row.status === appGlobals.$statusCancelled) && props.row.durationTerm === appGlobals.$longTerm">
              - {{ dateMonth(props.row.finishAt) }}
            </span>
            <i class="fz-2 block">{{ splitCapital(props.row.durationTerm) }}</i>
          </template>
        </b-table-column>
        <b-table-column field="workerRate" label="Rate / Salary" v-slot="props">
          {{ currency(props.row.workerRate) }}
        </b-table-column>
        <b-table-column field="status" v-slot="props">
          <div v-if="props.row.status && props.row.status !== 'None'" class="capitailized has-text-weight-bold has-text-centered"
            :class="props.row.status">
            {{ props.row.status }}
          </div>
        </b-table-column>
      </SigookGrid>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useWorkerStore } from '@/modules/worker/store';
import { getJobs } from '@/modules/worker/requests/api';
import { getMyProfile } from '@/modules/worker/profile/api';
import { showAlertError } from '@/shared/utils/toast';
import type { WorkerRequestFilter, WorkerRequestListItem } from '@/modules/worker/requests/types';
import { dateMonth, splitCapital, currency } from '@/shared/format';
import { workerRequirementLabel, workerRequirementSection } from '@/shared/worker-profile/useWorkerProfileStatus';
import { appGlobals } from '@/app/globals';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import JobCard from '@/modules/worker/requests/components/JobCard.vue';

const router = useRouter();
const workerStore = useWorkerStore();

const isGridLoading = ref(true);
const serverParams = ref<WorkerRequestFilter>({
  isDescending: false,
});

const currentUser = computed(() => workerStore.workerProfile);
const isProfileLoaded = computed(() => !!currentUser.value.id);
const isProfileLoading = ref(!isProfileLoaded.value);
const isLoading = computed(() => isGridLoading.value || isProfileLoading.value);
const missingRequirements = computed(() => currentUser.value.missingRequirements ?? []);

if (isProfileLoading.value) {
  getMyProfile()
    .then((data) => workerStore.setWorkerProfile(data))
    .catch((error) => showAlertError(error))
    .finally(() => { isProfileLoading.value = false; });
}

function onRowClick(row: WorkerRequestListItem) {
  switch (row.status) {
    case appGlobals.$statusApply:
    case appGlobals.$statusBook:
      router.push({ path: `/worker-requests/applied/${row.id}` });
      break;
    default:
      router.push({ path: `/worker-requests/${row.id}` });
  }
}
</script>
