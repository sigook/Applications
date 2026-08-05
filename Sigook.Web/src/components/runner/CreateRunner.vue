<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <p class="modal-card-title">Add Runner</p>
    </header>
    <form @submit.prevent="submit">
      <section class="modal-card-body" style="min-width: 560px">
        <b-field label="Type">
          <b-radio-button v-model="type" :native-value="RunnerType.Active" type="is-primary">Active</b-radio-button>
          <b-radio-button v-model="type" :native-value="RunnerType.Passive" type="is-primary">Passive</b-radio-button>
        </b-field>

        <b-field label="Worker" :type="errors.workerProfileId ? 'is-danger' : ''"
          :message="errors.workerProfileId || 'Type at least 3 characters to search'">
          <b-autocomplete v-model="searchText" :data="results" placeholder="Search by name, email or ID..." append-to-body
            name="worker" :loading="isSearching" :custom-formatter="formatOption" @typing="onSearchInput" @select="onSelect">
            <template v-slot="props">
              <div class="is-flex is-justify-content-space-between is-align-items-center">
                <div>
                  <strong>#{{ props.option.numberId }}</strong>
                  <span class="ml-2">{{ props.option.name }}</span>
                  <small v-if="props.option.email" class="ml-2 color-gray-light">{{ props.option.email }}</small>
                </div>
              </div>
            </template>
          </b-autocomplete>
        </b-field>
      </section>
      <footer class="modal-card-foot">
        <b-button @click="emit('close')">Cancel</b-button>
        <b-button type="is-primary" native-type="submit" :loading="props.isSaving">Add Runner</b-button>
      </footer>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { searchAgencyRunnerProspects } from '@/api/agencyRunnerApi';
import { showAlertError } from '@/utils/toast';
import type { ApplicantSearchResult } from '@/types/agency';
import { RunnerType } from '@/types/runner';
import type { CreateRunnerModel } from '@/types/runner';

const props = defineProps<{ requestId: string; isSaving?: boolean }>();
const emit = defineEmits<{ (e: 'create', model: CreateRunnerModel): void; (e: 'close'): void }>();

const searchText = ref('');
const results = ref<ApplicantSearchResult[]>([]);
const isSearching = ref(false);

const schema = yup.object({
  type: yup.number().required(),
  workerProfileId: yup.string().required('Select a worker'),
});

const form = useStickyForm<{ type: RunnerType; workerProfileId: string }>({
  schema,
  initialValues: { type: RunnerType.Active, workerProfileId: '' },
});
const { type, workerProfileId } = form.fields;
const errors = form.errors;

function formatOption(option: ApplicantSearchResult): string {
  return `#${option.numberId} | ${option.name} | ${option.email || 'No Email'}`;
}

function onSearchInput(text: string) {
  workerProfileId.value = '';
  if (text.length < 3) {
    results.value = [];
    return;
  }
  isSearching.value = true;
  searchAgencyRunnerProspects(props.requestId, text)
    .then(res => {
      results.value = res;
    })
    .catch(err => showAlertError(err))
    .finally(() => {
      isSearching.value = false;
    });
}

function onSelect(item: ApplicantSearchResult | null) {
  workerProfileId.value = item?.workerProfileId ?? '';
}

function submit() {
  form.markInteracted();
  form.handleSubmit(
    values => {
      emit('create', {
        workerProfileId: values.workerProfileId,
        type: values.type,
      });
    },
    () => showAlertError('Please make sure all required fields are filled out correctly'),
  )();
}
</script>
