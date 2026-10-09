<template>
  <detail-card title="Settings">
    <template #badge>
      <span class="detail-tag">Admin</span>
    </template>
    <div class="company-settings">
      <b-checkbox :model-value="company.requiresPermissionToSeeRequests"
        @update:modelValue="(value: boolean) => update('requiresPermissionToSeeRequests', value)">
        Requires permission to see requests
      </b-checkbox>
      <b-checkbox :model-value="company.paidHolidays"
        @update:modelValue="(value: boolean) => update('paidHolidays', value)">
        Paid holidays
      </b-checkbox>
      <b-field label="Overtime starts after (hours / week)" class="company-settings-overtime">
        <b-input v-model="overtime" type="number" min="0" step="0.5" />
        <p class="control">
          <b-button :loading="isLoading" @click="update('overtimeStartsAfter', Number(overtime))">Save</b-button>
        </p>
      </b-field>
    </div>
  </detail-card>
</template>

<script setup lang="ts" generic="T extends CompanyProfileDetail">
import { ref, watch } from 'vue';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { updateOvertime, updatePaidHolidays, updatePermissionToSeeRequests } from '@/modules/agency/recruiting/clients/api';
import type { CompanyProfileDetail } from '@/shared/company-profile/types';
import type { CompanyProfileSettingsUpdate } from '@/modules/agency/recruiting/clients/types';
import DetailCard from '@/shared/detail-page/DetailCard.vue';

type SettingKey = keyof CompanyProfileSettingsUpdate;

const props = defineProps<{ company: T }>();
const emit = defineEmits<{ (e: 'update:company', value: T): void }>();

const actions: Record<SettingKey, (id: string, model: CompanyProfileSettingsUpdate) => Promise<void>> = {
  requiresPermissionToSeeRequests: updatePermissionToSeeRequests,
  paidHolidays: updatePaidHolidays,
  overtimeStartsAfter: updateOvertime,
};

const isLoading = ref(false);
const overtime = ref(String(props.company.overtimeStartsAfter ?? ''));

watch(() => props.company.overtimeStartsAfter, (value) => {
  overtime.value = String(value ?? '');
});

function update<K extends SettingKey>(key: K, value: CompanyProfileSettingsUpdate[K]) {
  const previous = props.company;
  const updated = { ...props.company, [key]: value };
  emit('update:company', updated);
  isLoading.value = true;
  actions[key](props.company.id, updated)
    .then(() => showAlertSuccess('Updated'))
    .catch((error) => {
      emit('update:company', previous);
      showAlertError(error);
    })
    .finally(() => {
      isLoading.value = false;
    });
}
</script>

<style lang="scss" scoped>
.company-settings {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-end;
  gap: 16px 32px;
}

.company-settings-overtime {
  margin-bottom: 0;

  :deep(.input) {
    max-width: 120px;
  }
}
</style>
