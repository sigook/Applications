<template>
  <div class="d-inline-block">
    <button
      class="button-sort"
      :class="{ hidden: current === value && !isDesc }"
      @click="onClickSort(false)"
    >
      <img src="../../assets/images/up-arrow.png" alt="Up" />
    </button>
    <button
      class="button-sort"
      :class="{ hidden: current === value && isDesc }"
      @click="onClickSort(true)"
    >
      <img src="../../assets/images/down-arrow.png" alt="Down" />
    </button>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';

const props = defineProps<{ current?: any; value?: any }>();
const emit = defineEmits<{ (e: 'updateOrderBy', payload: { value: any; desc: boolean }): void }>();

const isDesc = ref(true);

function onClickSort(desc: boolean) {
  isDesc.value = desc;
  emit('updateOrderBy', { value: props.value, desc });
}
</script>
<style lang="scss">
.button-sort {
  border: 0;
  width: 10px;
  &:hover {
    transform: scale(1.2, 1.2);
  }
}
</style>
