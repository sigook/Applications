<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field class="has-text-weight-normal"
          :type="formErrors.liftId ? 'is-danger' : ''"
          :message="formErrors.liftId || ''">
          <template #label>
            Can you Lift up to <span class="has-text-danger">*</span>
          </template>
          <b-select v-model="liftId" placeholder="Select option" expanded
            name="lift">
            <option v-for="item in lifts" :value="item.id" v-bind:key="item.id">
              {{ item.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { fetchLifts } from "@/api/catalogApi";
import { createWorkerOther } from '@/api/workerApi';

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const schema = yup.object({
  liftId: yup.mixed().required('Lift is required'),
});

const form = useStickyForm<{ liftId: any }>({
  schema,
  initialValues: {
    liftId: null,
  },
});
const { liftId } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const lifts = ref<any[]>([]);

function saveWorkerOther(values: any) {
  isLoading.value = true;
  const payload = { lift: { id: values.liftId } };
  createWorkerOther(props.data.id, payload)
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function validateAll() {
  form.markInteracted();
  form.handleSubmit((values: any) => {
    saveWorkerOther(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

(async () => {
  lifts.value = await fetchLifts();
  if (props.data != null && props.data.lift) {
    form.hydrate({
      liftId: props.data.lift.id || null,
    });
  }
})();
</script>
