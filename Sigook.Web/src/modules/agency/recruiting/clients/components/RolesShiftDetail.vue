<template>
    <div ref="root" class="is-inline-block relative align-text-top">
        <span>{{ props.displayShift }}</span>
        <button v-if="props.displayShift" @click.stop="getShift" type="button" class="border-0 pl-4 pr-3">
            <b-icon :icon="showDetail ? 'chevron-up' : 'chevron-down'" size="is-small"></b-icon>
        </button>
        <shift-detail v-if="showDetail" :shift="shift" v-model:is-loading="isLoading" :anchor="root" />
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { useClickOutside } from '@/shared/composables/useClickOutside';
import { getAgencyCompanyJobPositionById } from '@/modules/agency/recruiting/clients/api';
import ShiftDetail from "@/shared/request-detail/ShiftDetail.vue";
import type { RequestShiftModel } from '@/shared/request-detail/types';

const props = defineProps<{ displayShift?: string; roleId: string; companyProfileId: string }>();

const root = ref<HTMLElement | null>(null);
const shift = ref<RequestShiftModel | null>(null);
const showDetail = ref(false);
const isLoading = ref(false);

useClickOutside(root, showDetail, () => { showDetail.value = false; });

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
