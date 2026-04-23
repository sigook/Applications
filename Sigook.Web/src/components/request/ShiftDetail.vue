<template>
    <div>
        <b-loading v-model="localIsLoading"></b-loading>
        <ul class="table-shift-detail" v-if="shift">
            <li v-if="shift.sunday">
                Sun {{ hourminutes(shift.sundayStart) }} to {{ hourminutes(shift.sundayFinish) }}
            </li>
            <li v-if="shift.monday">
                Mon {{ hourminutes(shift.mondayStart) }} to {{ hourminutes(shift.mondayFinish) }}
            </li>
            <li v-if="shift.tuesday">
                Tue {{ hourminutes(shift.tuesdayStart) }} to {{ hourminutes(shift.tuesdayFinish) }}
            </li>
            <li v-if="shift.wednesday">
                Wed {{ hourminutes(shift.wednesdayStart) }} to {{ hourminutes(shift.wednesdayFinish) }}
            </li>
            <li v-if="shift.thursday">
                Thu {{ hourminutes(shift.thursdayStart) }} to {{ hourminutes(shift.thursdayFinish) }}
            </li>
            <li v-if="shift.friday">
                Fri {{ hourminutes(shift.fridayStart) }} to {{ hourminutes(shift.fridayFinish) }}
            </li>
            <li v-if="shift.saturday">
                Sat {{ hourminutes(shift.saturdayStart) }} to {{ hourminutes(shift.saturdayFinish) }}
            </li>
            <li class="fz-1" v-if="shift.comments"><i>*{{ shift.comments }}</i></li>
        </ul>
    </div>
</template>
<script setup lang="ts">
import { ref, watch } from 'vue';
import { hourminutes } from '@/utils/filters';

const props = defineProps<{ shift?: any; isLoading?: boolean }>();

const localIsLoading = ref<boolean>(!!props.isLoading);

watch(() => props.isLoading, (newVal) => {
  localIsLoading.value = !!newVal;
});
</script>
<style lang="scss">
.table-shift-detail {
    border: 5px solid #f3f3f3;
    border-radius: 5px;
    margin-top: 8px;
    background: #ffffff;
    position: absolute;
    left: -5px;
    top: 12px;
    z-index: 1;
    width: 100%;
    min-width: 200px;

    li {
        padding: 5px;
        border-bottom: 1px solid #f3f3f3;

        &:nth-last-child(1) {
            border-bottom: 0;
        }
    }
}

button.up img {
    transform: rotate(180deg);
}
</style>
