<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field label="Position" :type="formErrors.jobPosition ? 'is-danger' : ''"
          :message="formErrors.jobPosition || ''">
          <b-autocomplete :data="filteredPositions" placeholder="Position" v-model="jobPosition" field="value"
            open-on-focus name="positions">
            <template v-slot="props">
              <span class="fz-0">{{ props.option.value }}</span>
              <span v-if="props.option.industry" class="fz-2 d-block">Industry: {{ props.option.industry }}</span>
            </template>
          </b-autocomplete>
        </b-field>
      </div>
      <div v-if="isPayrollManager" class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="'Agency Rate'" :type="formErrors.rate ? 'is-danger' : ''"
          :message="formErrors.rate || ''">
          <b-numberinput v-model="rate" name="rate" controls-alignment="right" step="0.01" placeholder="10.00">
          </b-numberinput>
        </b-field>
      </div>
      <div
        :class="isPayrollManager ? 'col-sm-12 col-md-4 col-lg-4 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding'">
        <b-field :label="'Worker Rate'" :type="formErrors.workerRate ? 'is-danger' : ''"
          :message="formErrors.workerRate || ''">
          <b-numberinput v-model="workerRate" :disabled="!isPayrollManager" name="workerRate"
            controls-alignment="right" step="0.01" placeholder="10.00">
          </b-numberinput>
        </b-field>
      </div>
      <div
        :class="isPayrollManager ? 'col-sm-12 col-md-4 col-lg-4 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding'">
        <b-field :label="'Worker Rate Max'" :type="formErrors.workerRateMax ? 'is-danger' : ''"
          :message="formErrors.workerRateMax || ''">
          <b-numberinput v-model="workerRateMax" name="workerRateMax" controls-alignment="right"
            step="0.01" placeholder="10.00">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :label="'Description'" :type="formErrors.description ? 'is-danger' : ''"
          :message="formErrors.description || ''">
          <b-input type="textarea" v-model="description" name="description">
          </b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding" v-if="currentPosition">
        <shift v-if="model.shift" :is-update="true" :current-shift="model.shift"
          @updateModel="(shift) => model.shift = shift" />
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding" v-else>
        <shift :is-update="false" @updateModel="(shift) => model.shift = shift" />
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateForm">
          {{ currentPosition ? 'Save' : 'Create' }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineAsyncComponent, computed, ref, watch } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { getJobPositions } from "@/api/catalogApi";
import { createAgencyCompanyJobPosition, updateAgencyCompanyJobPosition, getAgencyCompanyJobPositionById } from "@/api/agencyCompanyApi";

export default {
  props: ['currentPosition', 'profileId'],
  setup() {
    const rateRef = ref<number | null>(null);
    const workerRateRef = ref<number | null>(null);

    const schema = computed(() => {
      return yup.object({
        jobPosition: yup.string().required('Position is required'),
        rate: yup.number().typeError('Rate is required').required('Rate is required').min(0.1, 'Minimum is 0.1').max(999999),
        workerRate: yup
          .number()
          .typeError('Worker rate is required')
          .required('Worker rate is required')
          .min(0.1, 'Minimum is 0.1')
          .max(rateRef.value && rateRef.value > 0 ? rateRef.value : 50, 'Cannot exceed agency rate'),
        workerRateMax: yup
          .number()
          .nullable()
          .transform((v, o) => (o === '' || o === null || o === undefined ? null : v))
          .min(workerRateRef.value && workerRateRef.value > 0 ? workerRateRef.value : 0, 'Must be >= worker rate')
          .max(rateRef.value && rateRef.value > 0 ? rateRef.value : 50, 'Cannot exceed agency rate'),
        description: yup.string().nullable().transform((v) => (v === '' ? null : v)).max(1000, 'Max 1000 characters'),
      });
    });

    const form = useStickyForm({
      schema,
      initialValues: {
        jobPosition: '',
        rate: null as number | null,
        workerRate: null as number | null,
        workerRateMax: null as number | null,
        description: '',
      },
    });

    watch(form.fields.rate, (v) => { rateRef.value = (v as number | null) ?? null; });
    watch(form.fields.workerRate, (v) => { workerRateRef.value = (v as number | null) ?? null; });

    return {
      ...useBillingAdmin(),
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
    };
  },
  data() {
    return {
      isLoading: false,
      model: {
        id: null,
        jobPosition: null,
        rate: null,
        otherJobPosition: null,
        workerRate: null,
        description: null,
        workerRateMin: null,
        workerRateMax: null,
        shift: null,
      } as any,
      jobPositionList: [] as any[],
    };
  },
  components: {
    Shift: defineAsyncComponent(() => import("../request/ShiftsForm.vue")),
  },
  methods: {
    validateForm() {
      this.markInteracted();
      this.handleSubmit((values) => {
        const existingJobPosition = this.jobPositionList.find((jpl: any) => jpl.value === values.jobPosition);
        const payload = {
          ...this.model,
          rate: values.rate,
          workerRate: values.workerRate,
          workerRateMin: values.workerRate,
          workerRateMax: values.workerRateMax,
          description: values.description,
          jobPosition: existingJobPosition || null,
          otherJobPosition: existingJobPosition ? null : values.jobPosition,
        };
        if ((this as any).currentPosition) {
          this.updateJobPosition(payload, (this as any).currentPosition.id);
        } else {
          this.createJobPosition(payload);
        }
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    createJobPosition(payload: any) {
      this.isLoading = true;
      createAgencyCompanyJobPosition((this as any).profileId, payload)
        .then(() => {
          this.isLoading = false;
          this.$emit('updateContent');
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    updateJobPosition(payload: any, id: any) {
      this.isLoading = true;
      updateAgencyCompanyJobPosition((this as any).profileId, id, payload)
        .then(() => {
          this.isLoading = false;
          this.$emit('updateContent');
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    loadJobPositionById(id: any) {
      this.isLoading = true;
      getAgencyCompanyJobPositionById((this as any).profileId, id)
        .then((response: any) => {
          this.model = response;
          this.hydrateForm({
            jobPosition: response.value || '',
            rate: response.rate ?? null,
            workerRate: response.workerRate ?? null,
            workerRateMax: response.workerRateMax ?? null,
            description: response.description || '',
          });
          this.isLoading = false;
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
  },
  async created() {
    this.isLoading = true;
    this.jobPositionList = await getJobPositions();
    const currentPosition = (this as any).currentPosition;
    if (currentPosition && currentPosition.id) {
      this.loadJobPositionById(currentPosition.id);
    }
    this.isLoading = false;
  },
  computed: {
    filteredPositions() {
      const search = ((this as any).jobPosition || '').toLowerCase();
      return this.jobPositionList.filter((jpl: any) => jpl.value.toLowerCase().includes(search));
    },
  },
};
</script>
