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
<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { getDialog } from '@/utils/buefyProgrammatic';
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

const props = defineProps<{ worker: any }>();
const emit = defineEmits<{ (e: 'update:worker', value: any): void }>();

const schema = yup.object({
  cpp: yup.number().nullable().transform((v, o) => o === '' || o === null ? null : v)
    .min(0, 'Must be greater than or equal to 0'),
  ei: yup.number().nullable().transform((v, o) => o === '' || o === null ? null : v)
    .min(0, 'Must be greater than or equal to 0'),
});

const form = useStickyForm<{ cpp: number | null; ei: number | null }>({
  schema,
  initialValues: {
    cpp: null,
    ei: null,
  },
});
const { cpp, ei } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const taxCategories = ref<any[]>([]);
const workerHolidays = ref<any[]>([]);
const workerHolidaySelected = ref<any>(null);
const localWorker = ref<any>(JSON.parse(JSON.stringify(props.worker)));

watch(() => props.worker, (newVal) => {
  localWorker.value = JSON.parse(JSON.stringify(newVal));
  form.hydrate({
    cpp: newVal?.cpp ?? null,
    ei: newVal?.ei ?? null,
  });
}, { deep: true });

function updateExternalId() {
  isLoading.value = true;
  updateWorkerProfileExternalId(localWorker.value)
    .then(() => {
      isLoading.value = false;
      emit('update:worker', localWorker.value);
    }).catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateIsContractor() {
  isLoading.value = true;
  updateAgencyWorkerContractor(localWorker.value.id)
    .then(() => {
      isLoading.value = false;
      emit('update:worker', localWorker.value);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateIsSubContractor() {
  isLoading.value = true;
  updateAgencyWorkerSubContractor(localWorker.value.id)
    .then(() => {
      isLoading.value = false;
      emit('update:worker', localWorker.value);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateTaxCategory() {
  isLoading.value = true;
  updateWorkerProfileTaxCategory(localWorker.value)
    .then(() => {
      isLoading.value = false;
      emit('update:worker', localWorker.value);
    }).catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateTaxRate() {
  form.markInteracted();
  form.handleSubmit((values: any) => {
    isLoading.value = true;
    localWorker.value.cpp = values.cpp;
    localWorker.value.ei = values.ei;
    updateWorkerProfileTaxRate(localWorker.value)
      .then(() => {
        isLoading.value = false;
        emit('update:worker', localWorker.value);
      }).catch((error) => {
        isLoading.value = false;
        showAlertError(error);
      });
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function addHoliday() {
  getDialog().prompt({
    message: `City`,
    inputAttrs: {
      type: 'date',
      placeholder: 'Date',
    },
    closeOnConfirm: false,
    confirmText: 'Add',
    onConfirm: async (value: any, dialog: any) => {
      await addNewHoliday({ workerProfileId: localWorker.value.id, date: value });
      workerHolidays.value = await getAgencyWorkerProfileHolidays(localWorker.value.id);
      dialog.close();
    },
  });
}

function onHolidaySelected(date: Date) {
  workerHolidaySelected.value = workerHolidays.value.find(wh => new Date(wh.date).getDate() === date.getDate());
}

async function addUpdateWorkerHoliday() {
  isLoading.value = true;
  await addUpdateAgencyWorkerProfileHolidays(localWorker.value.id, workerHolidaySelected.value);
  isLoading.value = false;
}

const selectableDates = computed(() => {
  return workerHolidays.value.map((wh: any) => new Date(wh.date));
});

(async () => {
  taxCategories.value = await getTaxCategories();
  workerHolidays.value = await getAgencyWorkerProfileHolidays(localWorker.value.id);
  form.hydrate({
    cpp: localWorker.value?.cpp ?? null,
    ei: localWorker.value?.ei ?? null,
  });
})();
</script>
