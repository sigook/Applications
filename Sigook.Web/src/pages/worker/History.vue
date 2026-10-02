<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="fz1 pt-3">{{ "History" }}</h2>
    <div>
      <SigookGrid :fetch="getWorkerRequestHistory" v-model:params="serverParams" @update:loading="(value) => isLoading = value">
        <template #mobile-card="{ row }">
          <div class="rcard">
            <div class="rcard__head">
              <span class="rcard__title">{{ row.numberId }}</span>
              <div v-if="row.status && row.status !== 'None'" class="capitailized has-text-weight-bold"
                :class="row.status">
                {{ row.status }}
              </div>
            </div>
            <p class="rcard__title">{{ row.jobTitle }}</p>
            <div class="rcard__rows">
              <div class="rcard__row">
                <span class="rcard__label">Location</span>
                <span>{{ row.location }}<span v-if="row.entrance"> - {{ row.entrance }}</span></span>
              </div>
              <div class="rcard__row">
                <span class="rcard__label">Duration</span>
                <span>
                  {{ dateMonth(row.startAt) }}
                  <span v-if="row.durationTerm !== appGlobals.$longTerm"> - {{ dateMonth(row.finishAt) }}</span>
                  <span
                    v-if="(row.status === appGlobals.$statusFilled || row.status === appGlobals.$statusCancelled) && row.durationTerm === appGlobals.$longTerm">
                    - {{ dateMonth(row.finishAt) }}</span>
                  <i class="fz-2 block">{{ splitCapital(row.durationTerm) }}</i>
                </span>
              </div>
              <div class="rcard__row">
                <span class="rcard__label">Rate / Salary</span>
                <span>{{ currency(row.workerRate || row.workerSalary) }}</span>
              </div>
              <div class="rcard__row">
                <span class="rcard__label">Spots</span>
                <span>{{ row.workersQuantity }}</span>
              </div>
            </div>
          </div>
        </template>
        <b-table-column field="numberId" label="Request ID" v-slot="props">
          {{ props.row.numberId }}
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
          {{ currency(props.row.workerRate || props.row.workerSalary) }}
        </b-table-column>
        <b-table-column field="workersQuantity" label="Spots" v-slot="props">
          {{ props.row.workersQuantity }}
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
import { ref } from 'vue';
import { getWorkerRequestHistory } from '@/api/workerApi';
import type { WorkerRequestFilter } from '@/types/worker';
import { dateMonth, splitCapital, currency } from '@/utils/filters';
import { appGlobals } from '@/varaibles';
import SigookGrid from '@/components/SigookGrid.vue';

const isLoading = ref(false);
const serverParams = ref<WorkerRequestFilter>({
  isDescending: false,
});
</script>
