<template>
  <div class="p-3">
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field :label="title" :type="formErrors.dataModel ? 'is-danger' : ''"
          :message="formErrors.dataModel || ''">
          <b-input type="textarea" :name="title" v-model="dataModel"></b-input>
        </b-field>
      </div>
      <div class="column is-12">
        <b-button type="is-primary" @click="onSave">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { computed } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";

const props = defineProps<{ data?: string; title: string; minLength: number }>();
const emit = defineEmits<{ (e: 'updateContent', value: string): void }>();

const schema = computed(() =>
  yup.object({
    dataModel: yup
      .string()
      .required(`${props.title} is required`)
      .min(props.minLength, `Min ${props.minLength} characters`)
      .max(50000, 'Max 50000 characters'),
  })
);

const form = useStickyForm<{ dataModel: string }>({
  schema,
  initialValues: {
    dataModel: (props.data as string) || '',
  },
});

const { dataModel } = form.fields;
const formErrors = form.errors;

form.hydrate({ dataModel: props.data || '' });

function onSave() {
  form.markInteracted();
  form.handleSubmit((values) => {
    emit('updateContent', values.dataModel);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}
</script>
