<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.company ? 'is-danger' : ''" :label="'Company'"
          :message="formErrors.company || ''">
          <b-input type="text" v-model="company" :name="'company'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.supervisor ? 'is-danger' : ''" :label="'Supervisor'"
          :message="formErrors.supervisor || ''">
          <b-input type="text" v-model="supervisor" :name="'supervisor'" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :type="formErrors.duties ? 'is-danger' : ''" :label="'Duties'"
          :message="formErrors.duties || ''">
          <b-input type="textarea" v-model="duties" :name="'duties'" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :label="'Current Job'">
          <b-switch v-model="workExperience.isCurrentJobPosition" :name="'isCurrentJobPosition'" :true-value="true"
            :false-value="false">
            {{ workExperience.isCurrentJobPosition ? 'Yes' : 'No' }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.startDate ? 'is-danger' : ''" :label="'Start date'"
          :message="formErrors.startDate || ''">
          <b-datepicker v-model="startDate" :name="'startDate'"
            :max-date="disableStartDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding" v-if="!workExperience.isCurrentJobPosition">
        <b-field :type="formErrors.endDate ? 'is-danger' : ''" :label="'End date'"
          :message="formErrors.endDate || ''">
          <b-datepicker v-model="endDate" :name="'endDate'"
            :max-date="disableStartDate" :min-date="startDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="validateAll">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import * as yup from 'yup';
import { mapStores } from 'pinia';
import { useAppStore } from '@/stores/app';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { createWorkerWorkExperience, editWorkerWorkExperience } from '@/api/workerApi';

const schema = yup.object({
  company: yup.string().required('Company is required')
    .min(2, 'Min 2 characters').max(50, 'Max 50 characters'),
  supervisor: yup.string().required('Supervisor is required')
    .min(2, 'Min 2 characters').max(60, 'Max 60 characters'),
  duties: yup.string().required('Duties is required').max(5000, 'Max 5000 characters'),
  startDate: yup.mixed().required('Start date is required'),
  endDate: yup.mixed().nullable(),
});

export default {
  props: ['workerId', 'data'],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        company: '',
        supervisor: '',
        duties: '',
        startDate: null as Date | null,
        endDate: null as Date | null,
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
      disableStartDate: null as Date | null,
      workExperience: {
        isCurrentJobPosition: true,
      } as any,
    };
  },
  computed: {
    ...mapStores(useAppStore),
  },
  methods: {
    validateAll() {
      this.markInteracted();
      this.handleSubmit((values: any) => {
        if (!this.workExperience.isCurrentJobPosition && !values.endDate) {
          showAlertError('Please make sure all required fields are filled out correctly');
          return;
        }
        const payload = {
          ...this.workExperience,
          company: values.company,
          supervisor: values.supervisor,
          duties: values.duties,
          startDate: values.startDate,
          endDate: this.workExperience.isCurrentJobPosition ? null : values.endDate,
        };
        if ((this as any).data) {
          this.editWorkerWorkExperience(payload);
        } else {
          this.createWorkerWorkExperience(payload);
        }
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    createWorkerWorkExperience(payload: any) {
      this.isLoading = true;
      createWorkerWorkExperience((this as any).workerId, payload)
        .then(() => {
          this.isLoading = false;
          this.$emit("updateExperience");
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    editWorkerWorkExperience(payload: any) {
      this.isLoading = true;
      editWorkerWorkExperience((this as any).workerId, (this as any).data.id, payload)
        .then(() => {
          this.isLoading = false;
          this.$emit("updateExperience");
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    updateData() {
      const src = (this as any).data;
      this.workExperience = Object.assign({}, src);
      const startDate = new Date(src.startDate);
      const endDate = src.endDate ? new Date(src.endDate) : null;
      this.hydrateForm({
        company: src.company || '',
        supervisor: src.supervisor || '',
        duties: src.duties || '',
        startDate,
        endDate,
      });
    },
  },
  created() {
    this.appStore.getCurrentDate().then((response: any) => {
      this.disableStartDate = response;
    });
    if ((this as any).data) {
      this.updateData();
    }
  },
};
</script>
