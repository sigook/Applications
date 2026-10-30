<template>
  <div class="p-4">
    <h2 class="has-text-centered fz1 mb-2">End assignment</h2>
    <p class="has-text-centered mb-4">{{ subject }}</p>
    <p class="mb-4">The assignment ended normally and shows as completed in the worker's history. To remove a worker for cause, use Reject.</p>
    <form @submit.prevent="submit">
      <div class="columns is-multiline">
        <div class="column is-6">
          <b-field label="Last day worked *" :type="errors.lastDay ? 'is-danger' : ''" :message="errors.lastDay">
            <b-datepicker v-model="lastDay" :mobile-native="false" :min-date="minDate" :max-date="today"
              placeholder="Select a date" append-to-body />
          </b-field>
        </div>
        <div class="column is-6">
          <b-field label="Reason *" :type="errors.reason ? 'is-danger' : ''" :message="errors.reason">
            <b-select v-model="reason" expanded placeholder="Select">
              <option v-for="option in reasons" :key="option.id" :value="option.id">{{ option.value }}</option>
            </b-select>
          </b-field>
        </div>
        <div class="column is-12">
          <b-field label="Comments" :type="errors.comments ? 'is-danger' : ''" :message="errors.comments">
            <b-input v-model="comments" type="textarea" maxlength="500" rows="3" />
          </b-field>
        </div>
        <div class="column is-12">
          <p class="fz-1">Hours can still be added until one month after the last day.</p>
        </div>
      </div>
      <div class="mt-4">
        <b-button @click="emit('close')">Cancel</b-button>
        <b-button type="is-primary" native-type="submit" class="ml-2">End assignment</b-button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import dayjs from 'dayjs';
import * as yup from 'yup';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { WorkerRequestFinishReason, WorkerRequestFinishReasonLabels } from '@/shared/constants/enums';
import type { FinishAssignmentModel } from '@/shared/request-detail/types';

const props = defineProps<{
  subject: string;
  startWorking?: string | null;
}>();

const emit = defineEmits<{
  (e: 'submit', model: FinishAssignmentModel): void;
  (e: 'close'): void;
}>();

const today = new Date();
const minDate = props.startWorking ? dayjs(props.startWorking).startOf('day').toDate() : undefined;
const reasons = Object.values(WorkerRequestFinishReason)
  .filter((value): value is WorkerRequestFinishReason => typeof value === 'number')
  .map(id => ({ id, value: WorkerRequestFinishReasonLabels[id] }));

const schema = yup.object({
  lastDay: yup.date().required('Last day is required'),
  reason: yup.number().required('Reason is required'),
  comments: yup.string().max(500),
});

const form = useStickyForm<{ lastDay: Date | null; reason: WorkerRequestFinishReason | null; comments: string }>({
  schema,
  initialValues: { lastDay: today, reason: WorkerRequestFinishReason.ContractCompleted, comments: '' },
});
const { lastDay, reason, comments } = form.fields;
const errors = form.errors;

function submit() {
  form.markInteracted();
  form.handleSubmit(values => {
    emit('submit', {
      lastDay: dayjs(values.lastDay).startOf('day').toISOString(),
      reason: values.reason as WorkerRequestFinishReason,
      comments: values.comments || undefined,
    });
  })();
}
</script>
