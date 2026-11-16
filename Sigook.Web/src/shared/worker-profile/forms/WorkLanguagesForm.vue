<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="Languages">
          <b-taginput v-model="worker.languages" autocomplete :data="filteredLanguages" open-on-focus field="value"
            icon="label" placeholder="Select Languages" @typing="getFilteredLanguages" append-to-body>
          </b-taginput>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="saveWorkerLanguages()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { fetchLanguages } from "@/shared/api/catalogApi";
import { createWorkerLanguages } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import type { CatalogItem } from '@/shared/types/common';

const { data } = defineProps<{ data: WorkerProfileDetail }>();
const emit = defineEmits<(e: 'closeModal', value: boolean) => void>();

const isLoading = ref(false);
const languages = ref<CatalogItem[]>([]);
const filteredLanguages = ref<CatalogItem[]>([]);
const worker = reactive<{ languages: CatalogItem[] }>({ languages: [] });

async function saveWorkerLanguages() {
  isLoading.value = true;
  try {
    await createWorkerLanguages(data.id, worker.languages);
    isLoading.value = false;
    emit('closeModal', true);
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

function getFilteredLanguages(text: string) {
  filteredLanguages.value = languages.value.filter((option) =>
    option.value.toLowerCase().includes(text.toLowerCase())
  );
}

void (async () => {
  languages.value = await fetchLanguages();
  filteredLanguages.value = languages.value;
  worker.languages = data.languages;
})();
</script>
