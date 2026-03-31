<template>
  <b-field grouped position="is-right">
    <slot name="actions"></slot>
    <b-dropdown aria-role="list" position="is-bottom-left">
      <template #trigger>
        <b-button type="is-ghost" icon-right="chevron-down" icon-left="dots-vertical">Actions</b-button>
      </template>
      <slot name="dropdown-actions"></slot>
      <b-dropdown-item aria-role="listitem" @click="downloadReport">
        <b-icon icon="file-excel"></b-icon>
        <span>Export</span>
      </b-dropdown-item>
    </b-dropdown>
  </b-field>
</template>
<script lang="ts">
import download from "@/mixins/downloadFileMixin";
export default {
  props: ["url", "params", "fileName"],
  mixins: [download],
  methods: {
    downloadReport() {
      this.$emit("onDataLoading", true);
      this.$store.dispatch("agency/getAgencyReport", { filter: this.params, url: this.url })
        .then(file => {
          this.$emit("onDataLoading", false);
          this.downloadFile(file, `${this.fileName}_${new Date().toLocaleDateString()}`)
        })
        .catch(() => this.$emit("onDataLoading", false));
    }
  }
}
</script>
