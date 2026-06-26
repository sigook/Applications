<template>
  <div class="numbers-grid">
    <SecondaryCard
      v-for="(stat, idx) in stats"
      :key="stat.label"
      :variant="stat.tone"
      :delay="idx * 140"
      class="numbers-grid__stat"
    >
      <span class="numbers-grid__num">
        <span class="numbers-grid__plus" aria-hidden="true">+</span>{{ stat.value }}
      </span>

      <span class="numbers-grid__lbl">{{ stat.label }}</span>
    </SecondaryCard>
  </div>
</template>

<script lang="ts">
import type { SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'

export type StatTone = SecondaryCardVariant

export interface Stat {
  readonly value: string
  readonly label: string
  readonly tone: StatTone
}
</script>

<script setup lang="ts">
import SecondaryCard from '@/components/landing/shared/SecondaryCard.vue'

defineProps<{
  stats: readonly Stat[]
}>()
</script>

<style scoped>
.numbers-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: clamp(16px, 1.8vw, 28px);
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
  font-family: var(--font-family);
}

.numbers-grid__stat {
  text-align: center;
}

.numbers-grid__stat :deep(.secondary-card__body) {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(8px, 1vw, 14px);
}

.numbers-grid__num {
  display: block;
  font-size: clamp(36px, 4.5vw, 60px);
  font-weight: 700;
  line-height: 1;
  letter-spacing: -0.02em;
  color: #fff;
  white-space: nowrap;
}

.numbers-grid__plus {
  display: inline-block;
  margin-right: clamp(2px, 0.2vw, 4px);
  font-weight: 700;
}

.numbers-grid__lbl {
  display: block;
  font-size: clamp(11px, 0.95vw, 13px);
  font-weight: 600;
  letter-spacing: 0.10em;
  text-transform: uppercase;
  line-height: 1.4;
  color: rgba(255, 255, 255, 0.86);
}

@media (max-width: 899px) {
  .numbers-grid { grid-template-columns: 1fr 1fr; }
}

@media (max-width: 480px) {
  .numbers-grid { grid-template-columns: 1fr; }
}
</style>
