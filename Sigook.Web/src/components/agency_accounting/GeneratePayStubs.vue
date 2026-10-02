<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="rows" v-model:checked-rows="selectedWorkers" checkable :paginated="false"
      :fit-viewport="false">
      <b-table-column field="firstName" label="Worker" v-slot="props">
        {{ props.row.firstName }} {{ props.row.middleName }} {{ props.row.lastName }} {{ props.row.secondLastName }}
      </b-table-column>
      <b-table-column field="businessName" label="Company" v-slot="props">
        {{ props.row.businessName }}
      </b-table-column>
    </SigookGrid>
    <b-button type="is-primary" :disabled="selectedWorkers.length === 0" @click="submitGeneratePayStubs">Generate</b-button>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getWorkersReadyForPayStub, generatePayStubs } from "@/api/agencyPayStubApi";
import type { WorkerReadyForPayStubModel } from '@/types/accounting';
import SigookGrid from '@/components/SigookGrid.vue';

const emit = defineEmits<{(e: 'pay-stubs-generated'): void}>();

const isLoading = ref(false);
const rows = ref<WorkerReadyForPayStubModel[]>([]);
const selectedWorkers = ref<WorkerReadyForPayStubModel[]>([]);

function loadWorkers() {
  isLoading.value = true;
  getWorkersReadyForPayStub()
    .then((response) => {
      rows.value = response;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function submitGeneratePayStubs() {
  isLoading.value = true;
  const workerIds = selectedWorkers.value.map(worker => worker.workerId);
  generatePayStubs(workerIds)
    .then(() => {
      isLoading.value = false;
      emit("pay-stubs-generated");
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
      loadWorkers();
    });
}

loadWorkers();
</script>
