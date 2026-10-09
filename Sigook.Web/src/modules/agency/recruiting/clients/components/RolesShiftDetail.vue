<template>
    <div class="is-inline-block relative align-text-top" @mouseleave="showDetail = false">
        <span>{{ props.displayShift }}</span>
        <button v-if="props.displayShift" @click="getShift" class="border-0 pl-4 pr-3" :class="{ 'up': showDetail }">
            <img src="@/assets/images/arrow-down.svg" alt="button" type="button" width="10px">
        </button>
        <shift-detail v-if="showDetail" :shift="shift" v-model:is-loading="isLoading" />
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { getAgencyCompanyJobPositionById } from '@/modules/agency/recruiting/clients/api';
import ShiftDetail from "@/shared/request-detail/ShiftDetail.vue";
import type { RequestShiftModel } from '@/shared/request-detail/types';

const props = defineProps<{ displayShift?: string; roleId: string; companyProfileId: string }>();

const shift = ref<RequestShiftModel | null>(null);
const showDetail = ref(false);
const isLoading = ref(false);

function getShift() {
    if (!showDetail.value) {
        isLoading.value = true;
        showDetail.value = true;
        getAgencyCompanyJobPositionById(props.companyProfileId, props.roleId)
            .then(response => {
                isLoading.value = false;
                shift.value = response.shift;
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
