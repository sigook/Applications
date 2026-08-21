<template>
  <div>
    <div class="columns is-multiline">
      <div class="column is-3-mobile is-3">
        <b-field label="Dates (From - To)" :type="formErrors.dates ? 'is-danger' : ''"
          :message="formErrors.dates">
          <b-datepicker v-model="dates" name="dates" range
            @update:modelValue="onDatesSelected" />
        </b-field>
      </div>
      <div class="column is-12">
        <b-button type="is-primary" @click="getReport" :loading="isLoading">Generate</b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import dayjs from "dayjs";
import { downloadFile } from "@/utils/downloadFile";
import { getT4Report } from "@/api/agencyReportApi";
import { useStickyForm } from '@/composables/useStickyForm';
import type { AgencyReportFilter } from '@/types/agency';

const schema = yup.object({
  dates: yup.array().of(yup.date()).min(2, 'Dates are required').required('Dates are required'),
});

const form = useStickyForm<{ dates: Date[] }>({
  schema,
  initialValues: { dates: [] },
});
const { dates } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const serverParams = ref<AgencyReportFilter>({});

function onDatesSelected() {
  if (dates.value && dates.value.length === 2) {
    serverParams.value.startDate = dayjs(dates.value[0]).format('YYYY-MM-DD');
    serverParams.value.endDate = dayjs(dates.value[1]).format('YYYY-MM-DD');
  }
}

async function getReport() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) return;
  isLoading.value = true;
  getT4Report(serverParams.value)
    .then(response => {
      isLoading.value = false;
      downloadFile(response, `T4_${serverParams.value.startDate}_${serverParams.value.endDate}`);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
