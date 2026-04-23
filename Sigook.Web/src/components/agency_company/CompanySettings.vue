<template>
  <div class="mt-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <b-checkbox class="col-2" v-model="localCompany.requiresPermissionToSeeOrders" @update:modelValue="updateRequiresPermissionToSee">
        Requires permission to see orders?
      </b-checkbox>
      <b-checkbox class="col-2" v-model="localCompany.paidHolidays" @update:modelValue="updatePaidHolidaysHandler">
        Paid Holidays?
      </b-checkbox>
      <span class="line-gray"></span>
      <div class="col-4">
        <b-field label="Overtime Starts After">
          <b-input v-model="localCompany.overtimeStartsAfter"></b-input>
          <b-button type="is-primary is-light" @click="updateOvertimeHandler">Save</b-button>
        </b-field>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, watch } from 'vue';
import { showAlertError } from "@/utils/toast";
import {
  updatePermissionToSeeOrders,
  updatePaidHolidays,
  updateOvertime
} from "@/api/agencyCompanyApi";

const apiActions: Record<string, (id: any, model: any) => Promise<any>> = {
  updatePermissionToSeeOrders,
  updatePaidHolidays,
  updateOvertime
};

const props = defineProps<{ company: any }>();
const emit = defineEmits<{ (e: 'update:company', value: any): void }>();

const isLoading = ref(false);
const localCompany = ref<any>(JSON.parse(JSON.stringify(props.company)));

watch(() => props.company, (newVal) => {
  localCompany.value = JSON.parse(JSON.stringify(newVal));
}, { deep: true });

function update(action: string) {
  isLoading.value = true;
  emit('update:company', localCompany.value);
  apiActions[action](localCompany.value.id, localCompany.value)
    .then(() => { isLoading.value = false; })
    .catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function updateRequiresPermissionToSee() {
  update("updatePermissionToSeeOrders");
}

function updatePaidHolidaysHandler() {
  update("updatePaidHolidays");
}

function updateOvertimeHandler() {
  update("updateOvertime");
}
</script>
