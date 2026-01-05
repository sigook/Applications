<template>
  <div>
    <label v-for="(status, index) in statusList" :key="'status' + index"
           :class="'status-'+status.name.toLowerCase()"
           class="d-inline-block filter-status-tag">
      <input type="checkbox" v-model="status.checked" @change="updateStatusFilter">
      <span>{{ status.display }}</span>
    </label>
  </div>
</template>
<script>
export default {
  data() {
    return {
      statusList: [
        {name: this.$statusBook, checked: false, display: this.$statusDisplayBook},
        {name: this.$statusReject, checked: false, display: this.$statusDisplayReject}
      ],
      statusFilter: []
    }
  },
  methods: {
    updateStatusFilter() {
      this.statusFilter = this.statusList.filter(item => item.checked).map(item => item.name);
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