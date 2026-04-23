<template>
    <div class="d-inline-block relative v-top" @mouseleave="showDetail = false">
        <span>{{ displayShift }}</span>
        <button v-if="displayShift" @click.stop="getRequestShift" class="no-border" :class="{ 'up': showDetail }">
            <img src="../../assets/images/arrow-down.svg" alt="button" type="button" width="10px" class="ml-2">
        </button>
        <shift-detail v-if="showDetail" :shift="shift" v-model:is-loading="isLoading" />
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { fetchRequestShift } from "@/api/requestApi";
import ShiftDetail from '../request/ShiftDetail.vue';

const props = defineProps<{ displayShift?: string; requestId: any }>();

const shift = ref<any>(null);
const showDetail = ref(false);
const isLoading = ref(false);

function getRequestShift() {
    if (!showDetail.value) {
        isLoading.value = true;
        showDetail.value = true;
        fetchRequestShift(props.requestId)
            .then(response => {
                isLoading.value = false;
                shift.value = response;
            })
            .catch(error => {
                isLoading.value = false;
                showAlertError(error);
            });
    } else {
        showDetail.value = false;
    }
}
</script>
