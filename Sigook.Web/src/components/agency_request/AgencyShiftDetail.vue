<template>
    <div ref="root" class="is-inline-block relative align-text-top" @mouseleave="showDetail = false">
        <span>{{ displayShift }}</span>
        <button v-if="displayShift" @click.stop="getRequestShift" class="border-0" :class="{ 'up': showDetail }">
            <img src="../../assets/images/arrow-down.svg" alt="button" type="button" width="10px" class="ml-2">
        </button>
        <shift-detail v-if="showDetail" :shift="shift" v-model:is-loading="isLoading" />
    </div>
</template>

<script setup lang="ts">
import { ref, watch, onScopeDispose } from 'vue';
import { showAlertError } from "@/utils/toast";
import type { RequestShiftModel } from '@/types/agency';
import ShiftDetail from '../request/ShiftDetail.vue';

const props = defineProps<{
    displayShift?: string;
    requestId: string;
    fetchShift: (requestId: string) => Promise<RequestShiftModel>;
}>();

const root = ref<HTMLElement | null>(null);
const shift = ref<RequestShiftModel | null>(null);
const showDetail = ref(false);
const isLoading = ref(false);

function onDocumentClick(event: MouseEvent) {
    if (root.value && !root.value.contains(event.target as Node)) {
        showDetail.value = false;
    }
}

watch(showDetail, (open) => {
    if (open) {
        document.addEventListener('click', onDocumentClick, true);
    } else {
        document.removeEventListener('click', onDocumentClick, true);
    }
});

onScopeDispose(() => document.removeEventListener('click', onDocumentClick, true));

function getRequestShift() {
    if (!showDetail.value) {
        isLoading.value = true;
        showDetail.value = true;
        props.fetchShift(props.requestId)
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
