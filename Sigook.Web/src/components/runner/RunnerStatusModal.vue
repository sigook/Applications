<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <p class="modal-card-title">Change status</p>
    </header>
    <form @submit.prevent="submit">
      <section class="modal-card-body" style="min-width: 460px; position: relative">
        <b-loading v-model="isLoading" :is-full-page="false" />
        <b-message v-if="isCandidate" type="is-warning" size="is-small" has-icon>
          This runner is a candidate. Convert them to a worker before hiring, otherwise they will not appear in attendance review.
        </b-message>
        <b-field label="Status" :type="errors.status ? 'is-danger' : ''" :message="errors.status">
          <b-select v-model="status" placeholder="Select a status" expanded>
            <option v-for="s in statuses" :key="s" :value="s">{{ statusLabel(s) }}</option>
          </b-select>
        </b-field>
        <b-field v-if="status === RunnerStatus.Hired" label="Start date" required
          :type="errors.startDate ? 'is-danger' : ''" :message="errors.startDate">
          <b-datepicker v-model="startDate" :mobile-native="false" placeholder="Select a date" append-to-body />
        </b-field>
        <b-field label="Comments">
          <b-input v-model="comments" type="textarea" placeholder="Optional comments" />
        </b-field>
      </section>
      <footer class="modal-card-foot">
        <b-button @click="emit('close')">Cancel</b-button>
        <b-button type="is-primary" native-type="submit" :disabled="status === currentStatus">
          Update status
        </b-button>
      </footer>
    </form>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { changeRunnerStatus } from '@/api/agencyRunnerApi';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { RUNNER_STATUSES, RUNNER_STATUS_LABELS, RunnerStatus } from '@/types/runner';

const props = defineProps<{ requestId: string; runnerId: string; currentStatus: RunnerStatus; isCandidate?: boolean }>();
const emit = defineEmits<{ (e: 'updated'): void; (e: 'close'): void }>();

// 'Interview rescheduled' is assigned automatically when an interview is
// rescheduled, so it is not selectable here (kept only if it's the current one).
// A candidate cannot be hired until converted to a worker.
const statuses = computed(() =>
  RUNNER_STATUSES.filter(
    s =>
      (s !== RunnerStatus.InterviewRescheduled || s === props.currentStatus) &&
      (s !== RunnerStatus.Hired || !props.isCandidate),
  ),
);
const isCandidate = computed(() => !!props.isCandidate);
const isLoading = ref(false);

const schema = yup.object({
  status: yup.number().required(),
  startDate: yup
    .date()
    .nullable()
    .when('status', {
      is: RunnerStatus.Hired,
      then: s => s.required('Start date is required to hire a runner'),
    }),
  comments: yup.string().nullable(),
});

const form = useStickyForm<{ status: RunnerStatus; startDate: Date | null; comments: string }>({
  schema,
  initialValues: { status: props.currentStatus, startDate: null, comments: '' },
});
const { status, startDate, comments } = form.fields;
const errors = form.errors;

function statusLabel(status: RunnerStatus): string {
  return RUNNER_STATUS_LABELS[status];
}

function submit() {
  form.markInteracted();
  form.handleSubmit(
    values => {
      isLoading.value = true;
      changeRunnerStatus(props.requestId, props.runnerId, {
        status: values.status,
        comments: values.comments || undefined,
        startDate: values.status === RunnerStatus.Hired && values.startDate ? values.startDate.toISOString() : undefined,
      })
        .then(() => {
          showAlertSuccess('Status updated');
          emit('updated');
          emit('close');
        })
        .catch(err => {
          isLoading.value = false;
          showAlertError(err);
        });
    },
    () => showAlertError('Please make sure all required fields are filled out correctly'),
  )();
}
</script>
