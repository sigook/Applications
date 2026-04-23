<template>
  <div class="p-3">
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field label="Comment" :type="formErrors.comment ? 'is-danger' : ''"
          :message="formErrors.comment || ''">
          <b-input type="textarea" v-model="comment"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="send">{{ "Send" }}</b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";

const emit = defineEmits<{ (e: 'sendAnotherWorker', comment: string): void }>();

const schema = yup.object({
  comment: yup.string().required('Comment is required'),
});

const form = useStickyForm<{ comment: string }>({
  schema,
  initialValues: { comment: '' },
});
const { comment } = form.fields;
const formErrors = form.errors;

function send() {
  form.markInteracted();
  form.handleSubmit((values) => {
    emit('sendAnotherWorker', values.comment);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}
</script>
