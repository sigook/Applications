<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field label="Languages">
          <b-taginput v-model="worker.languages" autocomplete :data="filteredLanguages" open-on-focus field="value"
            icon="label" placeholder="Select Languages" @typing="getFilteredLanguages" append-to-body>
          </b-taginput>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="saveWorkerLanguages()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/utils/toast";
import { fetchLanguages } from "@/api/catalogApi";
import { createWorkerLanguages } from '@/api/workerApi';

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const languages = ref<any[]>([]);
const filteredLanguages = ref<any[]>([]);
const worker = reactive<{ languages: any[] }>({ languages: [] });

function saveWorkerLanguages() {
  isLoading.value = true;
  createWorkerLanguages(props.data.id, worker.languages)
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function getFilteredLanguages(text: string) {
  filteredLanguages.value = languages.value.filter((option) =>
    option.value.toLowerCase().includes(text.toLowerCase())
  );
}

(async () => {
  languages.value = await fetchLanguages();
  filteredLanguages.value = languages.value;
  if (props.data != null) {
    worker.languages = props.data.languages;
  }
})();
</script>
