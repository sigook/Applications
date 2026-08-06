<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="w-100 highlight-content" v-if="request">
      <div class="item">
        <span class="has-text-weight-bold">Rate / Salary</span>
        <p>{{ currency(request.workerRate || request.workerSalary) }}</p>
      </div>
      <div class="item">
        <span class="has-text-weight-bold">Term</span>
        <p>{{ DurationTermLabels[request.durationTerm] }}</p>
      </div>
      <div class="item">
        <span class="has-text-weight-bold">Employment Type</span>
        <p>{{ EmploymentTypeLabels[request.employmentType] }}</p>
      </div>
      <div class="item">
        <span class="has-text-weight-bold">Start
          <span
            v-if="((request.status === RequestStatus.Filled || request.status === RequestStatus.Cancelled) && request.durationTerm === DurationTerm.LongTerm) || request.durationTerm === DurationTerm.ShortTerm">
            / Finish</span>
        </span>
        <p>
          {{ dateMonth(request.startAt) }}
          <span class="fz-0" v-if="request.durationTerm !== DurationTerm.LongTerm">
            / {{ dateMonth(request.finishAt) }}</span>
          <span class="fz-0"
            v-if="(request.status === RequestStatus.Filled || request.status === RequestStatus.Cancelled) && request.durationTerm === DurationTerm.LongTerm">
            / {{ dateMonth(request.finishAt) }}
          </span>
        </p>
      </div>
      <div class="item worker-options">
        <span class="has-text-weight-bold">Workers</span>
        <p class="hover-actions">
          <span class="mr-1 fz-0">{{ request.workersQuantityWorking }} /
            {{ request.workersQuantity }}</span>
          <b-button v-if="request.canEdit" type="is-ghost" icon-left="plus-circle"
            class="worker-qty-btn worker-qty-plus" @click="onIncreaseWorkersQuantity()" />
          <b-button type="is-ghost" icon-left="minus-circle" class="worker-qty-btn worker-qty-minus"
            v-if="request.canEdit && request.workersQuantityWorking < request.workersQuantity && request.workersQuantity !== 1"
            @click="onReduceWorkersQuantity" />
        </p>
      </div>
      <div class="item">
        <span class="has-text-weight-bold">Is Asap</span>
        <p>
          <b-checkbox v-model="localRequest.isAsap" @update:modelValue="onToggleIsAsap()"></b-checkbox>
        </p>
      </div>
      <div class="item">
        <span class="has-text-weight-bold">Visible Punch Card</span>
        <p class="w-50">
          <b-checkbox v-model="localRequest.punchCardOptionEnabled"
            @update:modelValue="onTogglePunchCardVisibility()"></b-checkbox>
        </p>
      </div>
      <div class="item">
        <span class="has-text-weight-bold">Vaccination</span>
        <router-link :to="companyBase + '/' + request.companyProfileId">
          <p>
            {{ request.vaccinationRequired ? "yes" : "No" }}
            <b-icon icon="needle" class="ml-2"></b-icon>
          </p>
        </router-link>
      </div>
    </div>

    <div class="">
      <span class="has-text-weight-bold mr-2">Role</span>
      <span class="has-text-weight-normal">{{ request.jobPosition }}</span>
      <span v-if="request.displayShift" class="request-shift-container"><b class="has-text-weight-bold ">Shift</b>
        <agency-shift class="ml-3" :requestId="request.id" :displayShift="request.displayShift" />
      </span>
    </div>

    <skills :request="request"></skills>

    <section class="">
      <span class="has-text-weight-bold is-inline-block mb-2">Description</span>
      <pre class="long-description bullet-list" v-html="request.description"></pre>
    </section>

    <section class="">
      <span class="has-text-weight-bold is-inline-block mb-2">Responsibilities</span>
      <pre class="long-description bullet-list" v-html="request.responsibilities"></pre>
    </section>

    <section class="">
      <span class="has-text-weight-bold is-inline-block mb-2">Requirements</span>
      <pre class="long-description bullet-list" v-html="request.requirements"></pre>
    </section>

    <section class="">
      <span class="has-text-weight-bold is-inline-block mb-2">Internal Requirements</span>
      <pre class="long-description bullet-list" v-html="request.internalRequirements"></pre>
    </section>

    <section class="" v-if="request.incentive">
      <span class="has-text-weight-bold is-inline-block mb-2">Plus </span>
      <span class="has-text-weight-normal ml-2"> {{ currency(request.incentive) }}</span>
      <pre class="long-description">{{ request.incentiveDescription }} </pre>
    </section>

    <section class="">
      <span class="has-text-weight-bold mr-2">Break</span>
      <span class="has-text-weight-normal">{{ request.durationBreak }}</span>
      <span v-if="request.breakIsPaid" class="has-text-weight-normal">
        | {{ "Break paid" }}</span>
    </section>
  </div>
</template>
<script setup lang="ts">
import { ref, watch } from 'vue';
import { showAlertError } from "@/utils/toast";
import { currency, dateMonth } from '@/utils/filters';
import { useModuleBase } from '@/composables/useModuleBase';
import {
  increaseWorkersQuantityByOne,
  reduceWorkersQuantityByOne,
  updateAgencyRequestIsAsap,
  updateAgencyPunchCardVisibilityStatusInApp
} from "@/api/agencyRequestApi";
import {
  DurationTerm,
  DurationTermLabels,
  EmploymentTypeLabels,
  RequestStatus
} from "@/constants/enums";
import Skills from "../agency_request/AgencyRequestSkills.vue";
import AgencyShift from "../agency_request/AgencyShiftDetail.vue";

const props = defineProps<{ request?: any }>();
const emit = defineEmits<{ (e: 'refreshRequest'): void }>();

const { companyBase } = useModuleBase();

const isLoading = ref(false);
const localRequest = ref<any>(JSON.parse(JSON.stringify(props.request)));

watch(() => props.request, (newVal) => {
  localRequest.value = JSON.parse(JSON.stringify(newVal));
}, { deep: true });

function onIncreaseWorkersQuantity() {
  isLoading.value = true;
  increaseWorkersQuantityByOne(props.request.id)
    .then(() => {
      isLoading.value = false;
      emit('refreshRequest');
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onReduceWorkersQuantity() {
  isLoading.value = true;
  reduceWorkersQuantityByOne(props.request.id)
    .then(() => {
      isLoading.value = false;
      emit('refreshRequest');
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onToggleIsAsap() {
  isLoading.value = true;
  updateAgencyRequestIsAsap(props.request.id)
    .then(() => {
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onTogglePunchCardVisibility() {
  isLoading.value = true;
  updateAgencyPunchCardVisibilityStatusInApp(props.request.id)
    .then(() => {
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
<style lang="scss">
.bullet-list {
  &>ul {
    list-style: inside;
  }
}

.worker-options {
  .hover-actions {
    display: flex;
    align-items: center;
  }

  .worker-qty-btn {
    height: auto;
    min-height: 0;
    padding: 0;
    border: 0;
    margin-left: 4px;
    line-height: 1;
    background: transparent;

    &:hover,
    &:focus {
      background: transparent;
      text-decoration: none;
    }

    .icon {
      width: auto;
      height: auto;
      margin: 0 !important;
      font-size: 18px;
    }
  }
}
</style>
