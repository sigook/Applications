<template>
    <div ref="root" class="is-inline-block relative align-text-top">
        <span>{{ displayShift }}</span>
        <button v-if="displayShift" @click.stop="getShift" type="button" class="border-0 pl-4 pr-3">
            <b-icon :icon="showDetail ? 'chevron-up' : 'chevron-down'" size="is-small"></b-icon>
        </button>
        <shift-detail v-if="showDetail" :shift="shift" v-model:is-loading="isLoading" :anchor="root" />
    </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { useClickOutside } from '@/shared/composables/useClickOutside';
import { getAgencyCompanyJobPositionById } from '@/modules/agency/recruiting/clients/api';
import ShiftDetail from "@/shared/request-detail/ShiftDetail.vue";
import type { RequestShiftModel } from '@/shared/request-detail/types';

const { displayShift, roleId, companyProfileId } = defineProps<{ displayShift?: string; roleId: string; companyProfileId: string }>();

const root = useTemplateRef<HTMLElement>('root');
const shift = ref<RequestShiftModel | null>(null);
const showDetail = ref(false);
const isLoading = ref(false);

useClickOutside(root, showDetail, () => { showDetail.value = false; });

async function getShift() {
    if (!showDetail.value) {
        isLoading.value = true;
        showDetail.value = true;
        try {
            const response = await getAgencyCompanyJobPositionById(companyProfileId, roleId);
            isLoading.value = false;
            shift.value = response.shift ?? null;
        } catch (error) {
            isLoading.value = false;
            showAlertError(error);
        }
    } else {
        showDetail.value = false;
    }
}
</script>
