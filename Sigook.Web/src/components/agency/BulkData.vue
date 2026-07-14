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
          @click="() => downloadFile(fileError, errorFileName)">See Details</b-button>
      </b-field>
      <b-button type="is-primary" rounded @click="fileError = null">Try again</b-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useAgencyStore } from '@/stores/agency';
import { showAlertError } from "@/utils/toast";
import { downloadFile } from "@/utils/downloadFile";

const props = defineProps<{
  uploadFn: (agencyId: any, file: any) => Promise<any>;
  errorFileName: string;
  title: string;
  fileLabel: string;
}>();

const emit = defineEmits<{ (e: 'close'): void }>();

const agencyStore = useAgencyStore();

const isLoading = ref(false);
const bulkFile = ref<any>(null);
const agencySelected = ref<any>(null);
const fileError = ref<any>(null);

const agencies = computed(() => agencyStore.personnelAgencies);

function bulkUpload() {
  isLoading.value = true;
  props.uploadFn(agencySelected.value, bulkFile.value)
    .then((file: any) => {
      if (file.size > 0) {
        fileError.value = file;
        showAlertError("Some records could not be uploaded, please check the file");
      } else {
        emit("close");
      }
      bulkFile.value = null;
      isLoading.value = false;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
