<template>
    <div ref="root" class="shift-trigger is-inline-block align-text-top">
        <span>{{ displayShift }}</span>
        <b-button v-if="displayShift" type="is-ghost" size="is-small" class="shift-toggle"
            :icon-left="showDetail ? 'chevron-up' : 'chevron-down'"
            :aria-label="showDetail ? 'Hide shift detail' : 'Show shift detail'" :aria-expanded="showDetail"
            @click.stop="getRequestShift" />
        <shift-detail v-if="showDetail" :shift="shift" v-model:is-loading="isLoading" :anchor="root" />
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { useClickOutside } from '@/shared/composables/useClickOutside';
import type { RequestShiftModel } from '@/shared/request-detail/types';
import ShiftDetail from '@/shared/request-detail/ShiftDetail.vue';

const props = defineProps<{
    displayShift?: string;
    requestId: string;
    fetchShift: (requestId: string) => Promise<RequestShiftModel>;
}>();

const root = ref<HTMLElement | null>(null);
const shift = ref<RequestShiftModel | null>(null);
const showDetail = ref(false);
const isLoading = ref(false);

useClickOutside(root, showDetail, () => { showDetail.value = false; });

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

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.shift-trigger {
    position: relative;
}

.button.shift-toggle {
    width: 24px;
    height: 24px;
    margin-left: 4px;
    padding: 0;
    vertical-align: middle;
    color: $grey-font;

    &:hover,
    &:focus {
        background: $gray-bg;
        color: $navy;
        text-decoration: none;
    }
}
</style>
