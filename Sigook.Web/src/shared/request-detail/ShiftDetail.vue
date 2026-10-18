<template>
    <Teleport to="body" :disabled="!anchor">
        <div ref="panel" class="table-shift-detail" :class="{ 'is-floating': !!anchor }" :style="floatingStyle">
            <b-loading v-model="localIsLoading" :is-full-page="false"></b-loading>
            <ul v-if="shift">
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
    </Teleport>
</template>
<script setup lang="ts">
import { ref, watch, computed, onMounted, onScopeDispose } from 'vue';
import { hourminutes } from '@/shared/format';
import type { RequestShiftModel } from '@/shared/request-detail/types';

const GUTTER = 8;

const props = defineProps<{
  shift?: RequestShiftModel | null;
  isLoading?: boolean;
  anchor?: HTMLElement | null;
}>();

const panel = ref<HTMLElement | null>(null);
const localIsLoading = ref<boolean>(!!props.isLoading);
const top = ref(0);
const left = ref(0);

const floatingStyle = computed(() =>
  props.anchor ? { top: `${top.value}px`, left: `${left.value}px` } : undefined,
);

function reposition(): void {
  if (!props.anchor) return;
  const rect = props.anchor.getBoundingClientRect();
  const width = panel.value?.offsetWidth ?? 0;
  top.value = rect.bottom + 4;
  left.value = Math.max(GUTTER, Math.min(rect.left, window.innerWidth - width - GUTTER));
}

watch(() => props.isLoading, (newVal) => {
  localIsLoading.value = !!newVal;
});

watch(() => props.shift, () => requestAnimationFrame(reposition));

onMounted(() => {
  if (!props.anchor) return;
  reposition();
  window.addEventListener('scroll', reposition, true);
  window.addEventListener('resize', reposition);
});

onScopeDispose(() => {
  window.removeEventListener('scroll', reposition, true);
  window.removeEventListener('resize', reposition);
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
    z-index: 30;
    width: max-content;
    min-width: 200px;
    font-weight: 400;

    &.is-floating {
        position: fixed;
        margin-top: 0;
        z-index: 1000;
    }

    li {
        padding: 5px;
        border-bottom: 1px solid #f3f3f3;

        &:nth-last-child(1) {
            border-bottom: 0;
        }
    }
}
</style>
