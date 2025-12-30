<template>
  <b-field grouped position="is-right">
    <slot name="actions"></slot>
    <b-button type="is-ghost" icon-right="file-excel" @click="downloadReport">Export</b-button>
  </b-field>
</template>
<script>
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