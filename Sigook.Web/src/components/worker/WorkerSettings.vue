<template>
  <div class="mt-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-2">
        <b-field label="External ID">
          <b-input v-model="localWorker.externalId" placeholder="External ID" @keypress.enter="updateExternalId">
          </b-input>
        </b-field>
      </div>
      <b-checkbox class="col-2" v-model="localWorker.isContractor" @update:modelValue="updateIsContractor">
        Is Contractor
      </b-checkbox>
      <b-checkbox class="col-2" v-model="localWorker.isSubcontractor" @update:modelValue="updateIsSubContractor">
        Is Subcontractor
      </b-checkbox>
      <span class="line-gray"></span>
      <div class="col-2">
        <b-field label="Federal Category">
          <b-select v-model="localWorker.federalTaxCategory" @update:modelValue="updateTaxCategory" expanded>
            <option :value="null">Select</option>
            <option v-for="taxCategory in taxCategories" :key="taxCategory.id" :value="taxCategory.id">
              {{ taxCategory.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-2">
        <b-field label="Provincial Category" class="mr-5">
          <b-select v-model="localWorker.provincialTaxCategory" @update:modelValue="updateTaxCategory" expanded>
            <option :value="null">Select</option>
            <option v-for="taxCategory in taxCategories" :key="taxCategory.id" :value="taxCategory.id">
              {{ taxCategory.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-2">
        <b-field label="CPP" :type="formErrors.cpp ? 'is-danger' : ''"
          :message="formErrors.cpp || ''">
          <b-numberinput v-model="cpp" name="cpp" :step="0.01" :controls="false" expanded
            @keypress.enter="updateTaxRate">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-2">
        <b-field label="EI" :type="formErrors.ei ? 'is-danger' : ''"
          :message="formErrors.ei || ''">
          <b-numberinput v-model="ei" name="ei" :step="0.01" :controls="false" expanded
            @keypress.enter="updateTaxRate">
          </b-numberinput>
        </b-field>
      </div>
      <span class="line-gray"></span>
      <div class="col-12">
        <b-button type="is-ghost" icon-left="plus" @click="addHoliday">Add Holiday</b-button>
        <b-field grouped>
          <b-field label="Holidays">
            <b-datepicker inline :selectable-dates="selectableDates" @update:modelValue="onHolidaySelected"
              :unselectable-days-of-week="[0, 1, 2, 3, 4, 5, 6]">
            </b-datepicker>
          </b-field>
          <b-field v-if="workerHolidaySelected" label="Amount to pay">
            <b-input v-model="workerHolidaySelected.statPaidWorker"></b-input>
            <b-button type="is-primary is-light" @click="addUpdateWorkerHoliday">Save</b-button>
          </b-field>
        </b-field>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { getTaxCategories } from "@/api/catalogApi";
import {
  updateWorkerProfileExternalId,
  updateAgencyWorkerContractor,
  updateAgencyWorkerSubContractor,
  updateWorkerProfileTaxCategory,
  updateWorkerProfileTaxRate,
  addNewHoliday,
  getAgencyWorkerProfileHolidays,
  addUpdateAgencyWorkerProfileHolidays,
} from "@/api/agencyWorkerApi";

const schema = yup.object({
  cpp: yup.number().nullable().transform((v, o) => o === '' || o === null ? null : v)
    .min(0, 'Must be greater than or equal to 0'),
  ei: yup.number().nullable().transform((v, o) => o === '' || o === null ? null : v)
    .min(0, 'Must be greater than or equal to 0'),
});

export default {
  props: ['worker'],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        cpp: null as number | null,
        ei: null as number | null,
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
      taxCategories: [] as any[],
      workerHolidays: [] as any[],
      workerHolidaySelected: null as any,
      localWorker: JSON.parse(JSON.stringify((this as any).worker)),
    };
  },
  watch: {
    worker: {
      handler(newVal) {
        this.localWorker = JSON.parse(JSON.stringify(newVal));
        this.hydrateForm({
          cpp: newVal?.cpp ?? null,
          ei: newVal?.ei ?? null,
        });
      },
      deep: true,
    },
  },
  methods: {
    updateExternalId() {
      this.isLoading = true;
      updateWorkerProfileExternalId(this.localWorker)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:worker', this.localWorker);
        }).catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    updateIsContractor() {
      this.isLoading = true;
      updateAgencyWorkerContractor(this.localWorker.id)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:worker', this.localWorker);
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    updateIsSubContractor() {
      this.isLoading = true;
      updateAgencyWorkerSubContractor(this.localWorker.id)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:worker', this.localWorker);
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    updateTaxCategory() {
      this.isLoading = true;
      updateWorkerProfileTaxCategory(this.localWorker)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:worker', this.localWorker);
        }).catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    updateTaxRate() {
      this.markInteracted();
      this.handleSubmit((values: any) => {
        this.isLoading = true;
        this.localWorker.cpp = values.cpp;
        this.localWorker.ei = values.ei;
        updateWorkerProfileTaxRate(this.localWorker)
          .then(() => {
            this.isLoading = false;
            this.$emit('update:worker', this.localWorker);
          }).catch((error) => {
            this.isLoading = false;
            showAlertError(error);
          });
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    async addHoliday() {
      (this as any).$buefy.dialog.prompt({
        message: `City`,
        inputAttrs: {
          type: 'date',
          placeholder: 'Date',
        },
        closeOnConfirm: false,
        confirmText: 'Add',
        onConfirm: async (value: any, dialog: any) => {
          await addNewHoliday({ workerProfileId: this.localWorker.id, date: value });
          this.workerHolidays = await getAgencyWorkerProfileHolidays(this.localWorker.id);
          dialog.close();
        },
      });
    },
    onHolidaySelected(date: Date) {
      this.workerHolidaySelected = this.workerHolidays.find(wh => new Date(wh.date).getDate() === date.getDate());
    },
    async addUpdateWorkerHoliday() {
      this.isLoading = true;
      await addUpdateAgencyWorkerProfileHolidays(this.localWorker.id, this.workerHolidaySelected);
      this.isLoading = false;
    },
  },
  async created() {
    this.taxCategories = await getTaxCategories();
    this.workerHolidays = await getAgencyWorkerProfileHolidays(this.localWorker.id);
    this.hydrateForm({
      cpp: this.localWorker?.cpp ?? null,
      ei: this.localWorker?.ei ?? null,
    });
  },
  computed: {
    selectableDates() {
      const holidays = this.workerHolidays.map((wh: any) => new Date(wh.date));
      return holidays;
    },
  },
};

</script>
