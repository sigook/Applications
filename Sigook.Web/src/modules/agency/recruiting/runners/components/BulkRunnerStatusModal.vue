<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <p class="modal-card-title">Change status · {{ count }} runners</p>
    </header>
    <form @submit.prevent="submit">
      <section class="modal-card-body" style="min-width: 460px">
        <b-field label="Status" :type="errors.status ? 'is-danger' : ''" :message="errors.status">
          <b-select v-model="status" placeholder="Select a status" expanded>
            <option v-for="s in statuses" :key="s" :value="s">{{ RUNNER_STATUS_LABELS[s] }}</option>
          </b-select>
        </b-field>
        <b-field v-if="status === RunnerStatus.Hired" label="Start date" required
          :type="errors.startDate ? 'is-danger' : ''" :message="errors.startDate">
          <b-datepicker v-model="startDate" :mobile-native="false" placeholder="Select a date" append-to-body />
        </b-field>
        <b-field label="Comments" message="Saved on every runner's history entry">
          <b-input v-model="comments" type="textarea" placeholder="Optional comments" />
        </b-field>
        <b-message type="is-warning" size="is-small">
          Runners that cannot take the status (already hired, or on a request without runners) stay as they are and are
          listed afterwards with the reason.
        </b-message>
      </section>
      <footer class="modal-card-foot">
        <b-button @click="emit('close')">Cancel</b-button>
        <b-button type="is-primary" native-type="submit" :loading="isSaving">Update {{ count }} runners</b-button>
      </footer>
    </form>
  </div>
</template>

<script setup lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { showAlertError } from '@/shared/utils/toast';
import { RUNNER_STATUSES, RUNNER_STATUS_LABELS, RunnerStatus } from '@/modules/agency/recruiting/requests/runners/types';

export interface BulkRunnerStatus {
  status: RunnerStatus;
  comments?: string;
  startDate?: string | null;
}

defineProps<{ count: number; isSaving?: boolean }>();
const emit = defineEmits<{ (e: 'submit', value: BulkRunnerStatus): void; (e: 'close'): void }>();

// 'Interview rescheduled' is assigned automatically when an interview is
// rescheduled, so it is never a target here.
const statuses = RUNNER_STATUSES.filter((s) => s !== RunnerStatus.InterviewRescheduled);

const schema = yup.object({
  status: yup.number().required('Select a status'),
  startDate: yup
    .date()
    .nullable()
    .when('status', {
      is: RunnerStatus.Hired,
      then: (s) => s.required('Start date is required to hire a runner'),
    }),
  comments: yup.string().nullable(),
});

const form = useStickyForm<{ status: RunnerStatus | null; startDate: Date | null; comments: string }>({
  schema,
  initialValues: { status: null, startDate: null, comments: '' },
});
const { status, startDate, comments } = form.fields;
const errors = form.errors;

function submit() {
  form.markInteracted();
  form.handleSubmit(
    (values) => {
      if (values.status === null) return;
      emit('submit', {
        status: values.status,
        comments: values.comments || undefined,
        startDate: values.status === RunnerStatus.Hired && values.startDate ? values.startDate.toISOString() : undefined,
      });
    },
    () => showAlertError('Please make sure all required fields are filled out correctly'),
  )();
}
</script>
