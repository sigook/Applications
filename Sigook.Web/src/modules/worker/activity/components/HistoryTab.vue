<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid ref="grid" :fetch="getWorkerHistory" v-model:params="serverParams" focusable
      empty-text="You have no past assignments yet" @update:loading="(value) => isLoading = value">
      <template #actions>
        <b-select v-model="serverParams.status" placeholder="All statuses" aria-label="Status" @update:modelValue="onSearch">
          <option value="Past">All statuses</option>
          <option v-for="status in statuses" :key="status" :value="status">{{ status }}</option>
        </b-select>
      </template>
      <template #mobile-card="{ row }">
        <div class="rcard">
          <div class="rcard__head">
            <div>
              <p class="rcard__title">{{ row.jobTitle }}</p>
              <p class="rcard__sub">{{ row.clientName }} · #{{ row.numberId }}</p>
            </div>
            <b-tag :class="statusTagType(row.status)">{{ row.status }}</b-tag>
          </div>
          <div class="rcard__rows">
            <div class="rcard__row">
              <span class="rcard__label">Dates</span>
              <span>{{ dateMonth(row.startWorking ?? row.startAt) }} – {{ row.endedAt ? dateMonth(row.endedAt) : '—' }}</span>
            </div>
            <div class="rcard__row">
              <span class="rcard__label">Hours</span>
              <span>{{ hoursLabel(row.hoursWorked) }}</span>
            </div>
          </div>
        </div>
      </template>
      <b-table-column field="numberId" label="Request" v-slot="props">
        #{{ props.row.numberId }}
      </b-table-column>
      <b-table-column field="clientName" label="Client" v-slot="props">
        <span class="has-text-weight-medium">{{ props.row.clientName }}</span>
      </b-table-column>
      <b-table-column field="jobTitle" label="Position" v-slot="props">
        {{ props.row.jobTitle }}
      </b-table-column>
      <b-table-column field="location" label="Location" v-slot="props">
        {{ props.row.location }}
      </b-table-column>
      <b-table-column field="startWorking" label="Start" v-slot="props">
        {{ dateMonth(props.row.startWorking ?? props.row.startAt) }}
      </b-table-column>
      <b-table-column field="endedAt" label="End" v-slot="props">
        {{ props.row.endedAt ? dateMonth(props.row.endedAt) : '—' }}
      </b-table-column>
      <b-table-column field="hoursWorked" label="Hours" numeric v-slot="props">
        {{ hoursLabel(props.row.hoursWorked) }}
      </b-table-column>
      <b-table-column field="status" label="Status" v-slot="props">
        <b-tag :class="statusTagType(props.row.status)">{{ props.row.status }}</b-tag>
      </b-table-column>
    </SigookGrid>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { dateMonth } from '@/shared/format';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import { getWorkerHistory } from '@/modules/worker/activity/api';
import { hoursLabel, statusTagType } from '@/modules/worker/activity/activityFormat';
import type { WorkerHistoryFilter, WorkerHistoryStatusFilter } from '@/modules/worker/activity/types';
import type { GridHandle } from '@/shared/types/common';

const grid = useTemplateRef<GridHandle>('grid');
const isLoading = ref(false);
const statuses: WorkerHistoryStatusFilter[] = ['Completed', 'Ended'];
const serverParams = ref<WorkerHistoryFilter>({
  pageIndex: 1,
  pageSize: 30,
  status: 'Past',
});

function onSearch() {
  grid.value?.search();
}
</script>
