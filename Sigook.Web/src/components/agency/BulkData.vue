<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title">{{ title }}</h2>
    <div v-if="!fileError">
      <b-field label="Agency">
        <b-select v-model="agencySelected" expanded>
          <option v-for="agency in agencies" :key="agency.agencyId" :value="agency.agencyId">{{ agency.name }}
          </option>
        </b-select>
      </b-field>
      <b-field :label="fileLabel">
        <b-field class="file is-primary" :class="{ 'has-name': !!bulkFile }">
          <b-upload v-model="bulkFile" class="file-label" accept=".csv" rounded>
            <span class="file-cta">
              <b-icon class="file-icon" icon="upload"></b-icon>
              <span class="file-label">{{ bulkFile ? bulkFile.name : "Click to upload" }}</span>
            </span>
          </b-upload>
        </b-field>
      </b-field>
      <b-button type="is-primary" @click="bulkUpload" :disabled="!bulkFile || !agencySelected"
        rounded>Upload</b-button>
    </div>
    <div v-else>
      <b-field>
        <b-button type="is-ghost" icon-right="file-excel"
          @click="() => downloadFile(this.fileError, errorFileName)">See Details</b-button>
      </b-field>
      <b-button type="is-primary" rounded @click="fileError = null">Try again</b-button>
    </div>
  </div>
</template>
<script>
import download from "@/mixins/downloadFileMixin";

export default {
  name: 'BulkData',
  props: ['storeAction', 'errorFileName', 'title', 'fileLabel'],
  data() {
    return {
      isLoading: false,
      bulkFile: null,
      agencySelected: null,
      fileError: null,
    }
  },
  mixins: [download],
  methods: {
    bulkUpload() {
      this.isLoading = true;
      this.$store.dispatch(this.storeAction, { agencyId: this.agencySelected, file: this.bulkFile })
        .then((file) => {
          if (file.size > 0) {
            this.fileError = file;
            this.showAlertError("Some records could not be uploaded, please check the file");
          } else {
            this.$emit("close");
          }
          this.bulkFile = null;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
  },
  computed: {
    agencies() {
      return this.$store.state.agency.personnelAgencies;
    }
  }
}
</script> 