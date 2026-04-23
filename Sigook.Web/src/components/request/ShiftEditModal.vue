<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <shift-form v-if="shift" :current-shift="shift" :is-update="true" @updateModel="(val) => shift = val" />
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="saveShift(shift)">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError } from "@/utils/toast";
import { fetchRequestShift } from "@/api/requestApi";
import { updateAgencyRequestShift } from "@/api/agencyRequestApi";
import ShiftForm from "../../components/request/ShiftsForm.vue";

const emit = defineEmits<{ (e: 'onUpdateShift', displayShift: any): void }>();

const route = useRoute();

const isLoading = ref(false);
const shift = ref<any>(null);
const requestId = route.params.id;

function getRequestShift() {
  isLoading.value = true;
  fetchRequestShift(requestId as any)
    .then(response => {
      isLoading.value = false;
      shift.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function saveShift(model: any) {
  isLoading.value = true;
  updateAgencyRequestShift(requestId as any, model)
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
