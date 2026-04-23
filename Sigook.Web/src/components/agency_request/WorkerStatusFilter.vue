<template>
  <div>
    <label v-for="(status, index) in statusList" :key="'status' + index"
           :class="'status-'+status.display.toLowerCase()"
           class="d-inline-block filter-status-tag">
      <input type="checkbox" v-model="status.checked" @change="updateStatusFilter">
      <span>{{ status.display }}</span>
    </label>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { WorkerRequestStatus, WorkerRequestStatusLabels } from "@/constants/enums";

const emit = defineEmits<{ (e: 'updateFilter', value: any[]): void }>();

const statusList = ref([
  { value: WorkerRequestStatus.Booked, checked: false, display: WorkerRequestStatusLabels[WorkerRequestStatus.Booked] },
  { value: WorkerRequestStatus.Rejected, checked: false, display: WorkerRequestStatusLabels[WorkerRequestStatus.Rejected] }
]);
const statusFilter = ref<any[]>([]);

function updateStatusFilter() {
  statusFilter.value = statusList.value.filter(item => item.checked).map(item => item.value);
  emit('updateFilter', statusFilter.value);
}

function clear() {
  statusFilter.value = [];
  for (let i = 0; i < statusList.value.length; i++) {
    statusList.value[i].checked = false;
  }
}

defineExpose({ clear });
</script>
