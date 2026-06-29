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

        <b-field label="Worker/Candidate" :type="errors.applicantId ? 'is-danger' : ''"
          :message="errors.applicantId || 'Type at least 3 characters to search'">
          <b-autocomplete v-model="searchText" :data="results" placeholder="Search by name, email or ID..." append-to-body
            name="applicant" :loading="isSearching" :custom-formatter="formatOption" @typing="onSearchInput" @select="onSelect">
            <template v-slot="props">
              <div class="d-flex justify-content-between align-items-center">
                <div>
                  <strong>#{{ props.option.numberId }}</strong>
                  <span class="ms-2">{{ props.option.name }}</span>
                  <small v-if="props.option.email" class="ms-2 color-gray-light">{{ props.option.email }}</small>
                </div>
                <span class="tag-sm-gray ms-2">{{ props.option.type }}</span>
              </div>
            </template>
          </b-autocomplete>
        </b-field>
      </section>
      <footer class="modal-card-foot">
        <b-button @click="emit('close')">Cancel</b-button>
        <b-button type="is-primary" native-type="submit">Add Runner</b-button>
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

const props = defineProps<{ requestId: string }>();
const emit = defineEmits<{ (e: 'create', model: CreateRunnerModel): void; (e: 'close'): void }>();

const searchText = ref('');
const results = ref<ApplicantSearchResult[]>([]);
const selected = ref<ApplicantSearchResult | null>(null);
const isSearching = ref(false);

const schema = yup.object({
  type: yup.number().required(),
  applicantId: yup.string().required('Select a worker or candidate'),
});

const form = useStickyForm<{ type: RunnerType; applicantId: string }>({
  schema,
  initialValues: { type: RunnerType.Active, applicantId: '' },
});
const { type, applicantId } = form.fields;
const errors = form.errors;

function formatOption(option: ApplicantSearchResult): string {
  return `#${option.numberId} | ${option.name} | ${option.email || 'No Email'} | ${option.type}`;
}

function onSearchInput(text: string) {
  selected.value = null;
  applicantId.value = '';
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
  selected.value = item;
  applicantId.value = item ? (item.workerProfileId ?? item.candidateId ?? '') : '';
}

function submit() {
  form.markInteracted();
  form.handleSubmit(
    values => {
      if (!selected.value) return;
      emit('create', {
        workerProfileId: selected.value.workerProfileId ?? null,
        candidateId: selected.value.candidateId ?? null,
        type: values.type,
      });
    },
    () => showAlertError('Please make sure all required fields are filled out correctly'),
  )();
}
</script>
