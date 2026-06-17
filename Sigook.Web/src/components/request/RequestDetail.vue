<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <!-- Highlight -->
    <div class="col-12 col-padding highlight-content" v-if="request">
      <div class="item">
        <span class="fw-bold">Rate / Salary</span>
        <p>{{ currency(request.workerRate || request.workerSalary) }}</p>
      </div>
      <div class="item">
        <span class="fw-bold">Term</span>
        <p>{{ DurationTermLabels[request.durationTerm] }}</p>
      </div>
      <div class="item">
        <span class="fw-bold">Employment Type</span>
        <p>{{ EmploymentTypeLabels[request.employmentType] }}</p>
      </div>
      <div class="item">
        <span class="fw-bold">Start
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
        <span class="fw-bold">Workers</span>
        <p class="hover-actions">
          <span class="me-1 fz-0">{{ request.workersQuantityWorking }} /
            {{ request.workersQuantity }}</span>
          <button v-if="request.canEdit" @click="onIncreaseWorkersQuantity()"
            class="btn-icon-sm btn-icon-circle-plus bg-transparent relative actions">
            add
          </button>
          <button @click="onReduceWorkersQuantity"
            class="btn-icon-sm btn-icon-circle-minus bg-transparent relative actions"
            v-if="request.canEdit && request.workersQuantityWorking < request.workersQuantity && request.workersQuantity !== 1">
            reduce
          </button>
        </p>
      </div>
      <div class="item">
        <span class="fw-bold">Is Asap</span>
        <p>
          <b-checkbox v-model="localRequest.isAsap" @update:modelValue="onToggleIsAsap()"></b-checkbox>
        </p>
      </div>
      <div class="item">
        <span class="fw-bold">Visible Punch Card</span>
        <p class="w-50">
          <b-checkbox v-model="localRequest.punchCardOptionEnabled" @update:modelValue="onTogglePunchCardVisibility()"></b-checkbox>
        </p>
      </div>
      <div class="item">
        <span class="fw-bold">Vaccination</span>
        <router-link :to="'/agency-companies/company/' + request.companyProfileId">
          <p>
            {{ request.vaccinationRequired ? "yes" : "No" }}
            <b-icon icon="needle" class="ms-2"></b-icon>
          </p>
        </router-link>
      </div>
    </div>

    <!-- Role -->
    <div class="col-12 col-padding">
      <span class="fw-bold me-2">Role</span>
      <span class="fw-normal">{{ request.jobPosition }}</span>
      <span v-if="request.displayShift" class="request-shift-container"><b class="fw-bold ">Shift</b>
        <agency-shift class="ms-3" :requestId="request.id" :displayShift="request.displayShift" />
      </span>
    </div>

    <!-- Skills -->
    <skills :request="request"></skills>

    <!-- Detail -->
    <section class="col-12 col-padding">
      <span class="fw-bold is-inline-block mb-2">Description</span>
      <pre class="long-description bullet-list" v-html="request.description"></pre>
    </section>

    <section class="col-12 col-padding">
      <span class="fw-bold is-inline-block mb-2">Responsibilities</span>
      <pre class="long-description bullet-list" v-html="request.responsibilities"></pre>
    </section>

    <!-- Requirements -->
    <section class="col-12 col-padding">
      <span class="fw-bold is-inline-block mb-2">Requirements</span>
      <pre class="long-description bullet-list" v-html="request.requirements"></pre>
    </section>

    <section class="col-12 col-padding">
      <span class="fw-bold is-inline-block mb-2">Internal Requirements</span>
      <pre class="long-description bullet-list" v-html="request.internalRequirements"></pre>
    </section>

    <!-- Incentive -->
    <section class="col-12 col-padding" v-if="request.incentive">
      <span class="fw-bold is-inline-block mb-2">Plus </span>
      <span class="fw-normal ms-2"> {{ currency(request.incentive) }}</span>
      <pre class="long-description">{{ request.incentiveDescription }} </pre>
    </section>

    <!-- Break -->
    <section class="col-12 col-padding">
      <span class="fw-bold me-2">Break</span>
      <span class="fw-normal">{{ request.durationBreak }}</span>
      <span v-if="request.breakIsPaid" class="fw-normal">
        | {{ "Break paid" }}</span>
    </section>
  </div>
</template>
<script setup lang="ts">
import { ref, watch } from 'vue';
import { showAlertError } from "@/utils/toast";
import { currency, dateMonth } from '@/utils/filters';
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
</style>
