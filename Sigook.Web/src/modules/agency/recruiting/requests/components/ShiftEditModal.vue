<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <shift-form v-if="shift" :current-shift="shift" :is-update="true" @updateModel="(val) => shift = val" />
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="saveShift(shift)">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError } from "@/shared/utils/toast";
import { getAgencyRequestShift, updateAgencyRequestShift } from "@/modules/agency/recruiting/requests/api";
import ShiftForm from "@/modules/agency/recruiting/requests/components/ShiftsForm.vue";
import type { RequestShiftModel } from '@/shared/request-detail/types';

const emit = defineEmits<{ (e: 'onUpdateShift', displayShift: string | undefined): void }>();

const route = useRoute();

const isLoading = ref(false);
const shift = ref<RequestShiftModel | null>(null);
const requestId = route.params.id;

function getRequestShift() {
  isLoading.value = true;
  getAgencyRequestShift(requestId as string)
    .then(response => {
      isLoading.value = false;
      shift.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function saveShift(model: RequestShiftModel) {
  isLoading.value = true;
  updateAgencyRequestShift(requestId as string, model)
    .then(response => {
      isLoading.value = false;
      emit('onUpdateShift', response.displayShift);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

getRequestShift();
</script>
