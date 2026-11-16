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
import { showAlertError } from "@/shared/utils/toast";
import { getWorkersReadyForPayStub, generatePayStubs } from "@/modules/agency/accounting/paystubs/api";
import type { WorkerReadyForPayStubModel } from '@/modules/agency/accounting/paystubs/types';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const emit = defineEmits<(e: 'pay-stubs-generated') => void>();

const isLoading = ref(false);
const rows = ref<WorkerReadyForPayStubModel[]>([]);
const selectedWorkers = ref<WorkerReadyForPayStubModel[]>([]);

async function loadWorkers() {
  isLoading.value = true;
  try {
    rows.value = await getWorkersReadyForPayStub();
    isLoading.value = false;
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

async function submitGeneratePayStubs() {
  isLoading.value = true;
  const workerIds = selectedWorkers.value.map(worker => worker.workerId);
  try {
    await generatePayStubs(workerIds);
    isLoading.value = false;
    emit("pay-stubs-generated");
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
    await loadWorkers();
  }
}

void loadWorkers();
</script>
