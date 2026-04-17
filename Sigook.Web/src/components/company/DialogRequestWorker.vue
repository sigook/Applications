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

<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";

const schema = yup.object({
  comment: yup.string().required('Comment is required'),
});

export default {
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: { comment: '' },
    });
    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
    };
  },
  methods: {
    send() {
      this.markInteracted();
      this.handleSubmit((values) => {
        this.$emit('sendAnotherWorker', values.comment);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    }
  }
}
</script>
