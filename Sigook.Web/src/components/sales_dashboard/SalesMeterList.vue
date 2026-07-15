<template>
  <div class="sd-meters">
    <p class="sd-meters__title">{{ title }}</p>
    <div class="sd-meters__list">
      <div
        v-for="item in items"
        :key="item.label"
        class="sd-meter"
        role="img"
        :aria-label="`${item.label}: ${item.count}`"
      >
        <span class="sd-meter__label">{{ item.label }}</span>
        <div class="sd-meter__track">
          <div
            class="sd-meter__fill"
            :style="{ width: ratioOf(item.count, max) * 100 + '%', backgroundColor: item.color }"
          ></div>
        </div>
        <span class="sd-meter__count">{{ item.count }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { SalesMeter } from '@/types/salesDashboard';
import { ratioOf } from '@/utils/salesDashboardFormat';

const props = defineProps<{ title: string; items: readonly SalesMeter[] }>();

const max = computed(() =>
  props.items.length > 0 ? Math.max(...props.items.map((item) => item.count)) : 0
);
</script>

<style scoped lang="scss">
.sd-meters__title {
  font-size: 0.75rem;
  font-weight: 600;
  color: #555;
  margin-bottom: 0.56rem;
}

.sd-meters__list {
  display: flex;
  flex-direction: column;
  gap: 0.44rem;
}

.sd-meter {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.sd-meter__label {
  width: 74px;
  flex: none;
  font-size: 0.72rem;
  color: #777;
}

.sd-meter__track {
  flex: 1;
  height: 15px;
  background: #f1f2f4;
  border-radius: 5px;
  overflow: hidden;
}

.sd-meter__fill {
  height: 100%;
  border-radius: 5px;
  transition: width 0.35s ease;
}

.sd-meter__count {
  width: 22px;
  flex: none;
  text-align: right;
  font-size: 0.72rem;
  color: #555;
}
</style>
