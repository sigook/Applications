<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field :label="'Can you Lift up to'" class="has-text-weight-normal"
          :type="formErrors.liftId ? 'is-danger' : ''"
          :message="formErrors.liftId || ''">
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
<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { fetchLifts } from "@/api/catalogApi";
import { createWorkerOther } from '@/api/workerApi';

const schema = yup.object({
  liftId: yup.mixed().required('Lift is required'),
});

export default {
  props: ['data'],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        liftId: null as any,
      },
    });

    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
      resetForm: form.resetAll,
    };
  },
  data() {
    return {
      isLoading: false,
      lifts: [] as any[],
    };
  },
  methods: {
    validateAll() {
      this.markInteracted();
      this.handleSubmit((values: any) => {
        this.createWorkerOther(values);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    createWorkerOther(values: any) {
      this.isLoading = true;
      const payload = { lift: { id: values.liftId } };
      createWorkerOther((this as any).data.id, payload)
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
  },
  async created() {
    this.lifts = await fetchLifts();
    if ((this as any).data != null && (this as any).data.lift) {
      this.hydrateForm({
        liftId: (this as any).data.lift.id || null,
      });
    }
  },
};
</script>
