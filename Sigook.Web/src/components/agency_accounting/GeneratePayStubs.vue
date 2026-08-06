<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" v-model:checked-rows="selectedWorkers" checkable>
      <template v-slot:empty>
        <p class="container has-text-centered">No records available</p>
      </template>
      <template>
        <b-table-column field="firstName" label="Worker" v-slot="props">
          {{ props.row.firstName }} {{ props.row.middleName }} {{ props.row.lastName }} {{ props.row.secondLastName }}
        </b-table-column>
        <b-table-column field="businessName" label="Company" v-slot="props">
          {{ props.row.businessName }}
        </b-table-column>
      </template>
    </b-table>
    <b-button type="is-primary" :disabled="selectedWorkers.length === 0" @click="submitGeneratePayStubs">Generate</b-button>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getWorkersReadyForPayStub, generatePayStubs } from "@/api/agencyPayStubApi";

const emit = defineEmits<{(e: 'pay-stubs-generated'): void}>();

const isLoading = ref(false);
const rows = ref<any[]>([]);
const selectedWorkers = ref<any[]>([]);

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
