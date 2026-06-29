<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <p class="modal-card-title">Add interview</p>
    </header>
    <form @submit.prevent="submit">
      <section class="modal-card-body" style="min-width: 460px; position: relative">
        <b-loading v-model="isLoading" :is-full-page="false" />
        <b-field label="Scheduled date" required
          :type="errors.scheduledDate ? 'is-danger' : ''" :message="errors.scheduledDate">
          <b-datetimepicker v-model="scheduledDate" :mobile-native="false" placeholder="Select date and time" append-to-body />
        </b-field>
        <b-field label="Type" required :type="errors.type ? 'is-danger' : ''" :message="errors.type">
          <b-select v-model="type" expanded>
            <option v-for="t in interviewTypes" :key="t" :value="t">{{ interviewTypeLabel(t) }}</option>
          </b-select>
        </b-field>
        <b-field label="Interviewer">
          <b-input v-model="interviewer" placeholder="Interviewer name" />
        </b-field>
        <b-field label="Notes">
          <b-input v-model="notes" type="textarea" placeholder="Optional notes" />
        </b-field>
      </section>
      <footer class="modal-card-foot">
        <b-button @click="emit('close')">Cancel</b-button>
        <b-button type="is-primary" native-type="submit">Add</b-button>
      </footer>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { createRunnerInterview } from '@/api/agencyRunnerApi';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { INTERVIEW_TYPES, INTERVIEW_TYPE_LABELS, InterviewType } from '@/types/runner';

const props = defineProps<{ requestId: string; runnerId: string }>();
const emit = defineEmits<{ (e: 'updated'): void; (e: 'close'): void }>();

const interviewTypes = INTERVIEW_TYPES;
const isLoading = ref(false);

const schema = yup.object({
  scheduledDate: yup.date().nullable().required('Scheduled date is required'),
  type: yup.number().required('Type is required'),
  interviewer: yup.string().nullable(),
  notes: yup.string().nullable(),
});

const form = useStickyForm<{ scheduledDate: Date | null; type: InterviewType; interviewer: string; notes: string }>({
  schema,
  initialValues: { scheduledDate: null, type: InterviewType.Phone, interviewer: '', notes: '' },
});
const { scheduledDate, type, interviewer, notes } = form.fields;
const errors = form.errors;

function interviewTypeLabel(type: InterviewType): string {
  return INTERVIEW_TYPE_LABELS[type];
}

function submit() {
  form.markInteracted();
  form.handleSubmit(
    values => {
      isLoading.value = true;
      createRunnerInterview(props.requestId, props.runnerId, {
        scheduledDate: values.scheduledDate!.toISOString(),
        type: values.type,
        interviewer: values.interviewer || undefined,
        notes: values.notes || undefined,
      })
        .then(() => {
          showAlertSuccess('Interview added');
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
