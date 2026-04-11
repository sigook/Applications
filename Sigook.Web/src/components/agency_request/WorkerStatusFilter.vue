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
<script lang="ts">
import { WorkerRequestStatus, WorkerRequestStatusLabels } from "@/constants/enums";

export default {
  data() {
    return {
      statusList: [
        { value: WorkerRequestStatus.Booked, checked: false, display: WorkerRequestStatusLabels[WorkerRequestStatus.Booked] },
        { value: WorkerRequestStatus.Rejected, checked: false, display: WorkerRequestStatusLabels[WorkerRequestStatus.Rejected] }
      ],
      statusFilter: []
    }
  },
  methods: {
    updateStatusFilter() {
      this.statusFilter = this.statusList.filter(item => item.checked).map(item => item.value);
      this.$emit('updateFilter', this.statusFilter)
    },
    clear() {
      this.statusFilter = [];
      for (let i = 0; i < this.statusList.length; i++) {
        this.statusList[i].checked = false;
      }
    }
  }
}
</script>