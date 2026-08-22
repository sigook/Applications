<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <h3 class="title is-4 mb-4">Province Settings - {{ provinceName }}</h3>

    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field>
          <b-checkbox v-model="paidHolidays">
            Paid Holidays
          </b-checkbox>
        </b-field>
      </div>

      <div class="column is-12">
        <b-field label="Overtime Starts After" :type="formErrors.overtimeStartsAfter ? 'is-danger' : ''"
          :message="formErrors.overtimeStartsAfter">
          <b-input v-model="overtimeStartsAfter" name="overtimeStartsAfter" type="number" step="1"></b-input>
        </b-field>
      </div>

      <div class="column is-12">
        <b-button type="is-primary" @click="saveSettings">
          Save
        </b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { showAlertError } from '@/utils/toast';
import { useStickyForm } from '@/composables/useStickyForm';
import { updateProvinceSettings } from '@/api/locationApi';
import type { ProvinceSettings } from '@/types/common';

const props = defineProps<{
  provinceId: string;
  provinceName?: string;
  currentSettings?: ProvinceSettings | null;
}>();

const emit = defineEmits<{ (e: 'saved', settings: ProvinceSettings): void }>();

const schema = yup.object({
  overtimeStartsAfter: yup
    .number()
    .transform((v, o) => (o === '' || o === null ? null : v))
    .nullable()
    .min(0, 'Must be >= 0'),
});

const form = useStickyForm<{ overtimeStartsAfter: number | null }>({
  schema,
  initialValues: { overtimeStartsAfter: null },
});
const { overtimeStartsAfter } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const paidHolidays = ref(false);

if (props.currentSettings) {
  paidHolidays.value = !!props.currentSettings.paidHolidays;
  form.setFieldValue('overtimeStartsAfter', props.currentSettings.overtimeStartsAfter ?? null);
}

async function saveSettings() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) {
    showAlertError('Please fill in all required fields correctly');
    return;
  }
  const settings: ProvinceSettings = {
    paidHolidays: paidHolidays.value,
    overtimeStartsAfter:
      overtimeStartsAfter.value === null || String(overtimeStartsAfter.value) === ''
        ? null
        : parseFloat(String(overtimeStartsAfter.value)),
  };
  isLoading.value = true;
  try {
    await updateProvinceSettings(props.provinceId, settings);
    emit('saved', settings);
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}
</script>
