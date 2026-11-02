<template>
  <dl ref="el" class="detail-kpis" :style="{ '--detail-kpi-columns': columns }">
    <div v-for="(kpi, index) in kpis" :key="kpi.key" class="detail-kpi"
      :class="{ 'is-row-start': index % columns === 0, 'is-wrapped': index >= columns }">
      <dt>{{ kpi.label }}</dt>
      <dd class="detail-kpi-value">
        <slot :name="`kpi-${kpi.key}`" :kpi="kpi">{{ kpi.value }}</slot>
        <progress v-if="kpi.progress !== undefined" class="detail-kpi-progress" :value="kpi.progress"
          max="100">{{ kpi.progress }}%</progress>
        <span v-if="$slots[`kpi-${kpi.key}-extra`]" class="detail-kpi-extra">
          <slot :name="`kpi-${kpi.key}-extra`" />
        </span>
      </dd>
      <dd v-if="kpi.hint" class="detail-kpi-hint">{{ kpi.hint }}</dd>
    </div>
  </dl>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useBreakpoint } from '@/shared/composables/useBreakpoint';
import { useElementSize } from '@/shared/composables/useElementSize';
import type { DetailKpi } from '@/shared/detail-page/types';

const MIN_COLUMN_WIDTH = 190;

const props = defineProps<{ kpis: DetailKpi[] }>();

const { isMobile } = useBreakpoint();
const { el, width } = useElementSize();

const columns = computed(() => {
  const count = props.kpis.length;
  if (isMobile.value) return 2;
  return count * MIN_COLUMN_WIDTH <= width.value ? count : Math.ceil(count / 2);
});
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';
@import '@/assets/scss/breakpoints';

.detail-kpis {
  display: grid;
  grid-template-columns: repeat(var(--detail-kpi-columns), minmax(0, 1fr));
  margin: 0;
  border-top: 1px solid $gray-border;

  @include mobile {
    gap: 14px 12px;
    padding: 14px;
  }
}

.detail-kpi {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;
  padding: 12px 20px;
  border-left: 1px solid $gray-border;

  &.is-row-start {
    border-left: 0;
  }

  &.is-wrapped {
    border-top: 1px solid $gray-border;
  }

  dt {
    font-size: 13.5px;
    font-weight: 600;
    color: $grey-font;
  }

  dd {
    margin: 0;
  }

  @include mobile {
    padding: 0;
    border-left: 0;

    &.is-wrapped {
      border-top: 0;
    }

    &:last-child:nth-child(odd) {
      grid-column: 1 / -1;
    }
  }
}

.detail-kpi-value {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 4px 8px;
  font-size: 16px;
  font-weight: 700;
  line-height: 1.35;
  color: $navy;
  overflow-wrap: anywhere;
}

.detail-kpi-hint {
  font-size: 13.5px;
  line-height: 1.35;
  color: $grey-font;
}

.detail-kpi-extra {
  display: inline-flex;
  gap: 4px;

  :deep(.button) {
    width: 28px;
    height: 28px;
    padding: 0;

    @include touch {
      width: 44px;
      height: 44px;
    }
  }
}

.detail-kpi-progress {
  appearance: none;
  width: 64px;
  height: 6px;
  border: 0;
  border-radius: 3px;
  background: $gray-border;
  overflow: hidden;

  &::-webkit-progress-bar {
    background: $gray-border;
  }

  &::-webkit-progress-value {
    background: $blue;
  }

  &::-moz-progress-bar {
    background: $blue;
  }
}
</style>
