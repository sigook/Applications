<template>
  <section class="valley-cards">
    <LandingSectionHeader
      :eyebrow="eyebrow"
      :heading="heading"
      :heading-accent="headingAccent"
      :subtitle="subtitle"
    />

    <div class="valley-cards__grid">
      <template v-for="(reason, idx) in reasons" :key="reason.title">
        <SecondaryCard
          v-if="reason.body !== undefined"
          :variant="reason.variant"
          :eyebrow="reason.eyebrow"
          :title="reason.title"
          :delay="idx * 140"
          :class="{ 'valley-cards__card--offset': idx === 1 }"
        >
          {{ reason.body }}
        </SecondaryCard>

        <SecondaryCard
          v-else
          :variant="reason.variant"
          :eyebrow="reason.eyebrow"
          :title="reason.title"
          :list="reason.bullets"
          :delay="idx * 140"
          :class="{ 'valley-cards__card--offset': idx === 1 }"
        />
      </template>
    </div>
  </section>
</template>

<script setup lang="ts">
import LandingSectionHeader from '@/components/landing/shared/sections/LandingSectionHeader.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/cards/SecondaryCard.vue'

export interface ValleyReason {
  readonly eyebrow: string
  readonly title: string
  readonly variant: SecondaryCardVariant
  readonly bullets?: readonly string[]
  readonly body?: string
}

defineProps<{
  eyebrow: string
  heading: string
  headingAccent: string
  subtitle: string
  reasons: readonly ValleyReason[]
}>()
</script>

<style scoped>
.valley-cards {
  position: relative;
  width: 100%;
  padding:
    var(--section-pad-y)
    clamp(20px, 3vw, 40px)
    clamp(180px, 18vw, 300px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  isolation: isolate;
  overflow: hidden;
  font-family: var(--font-family);
}

.valley-cards__grid {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(20px, 2.4vw, 32px);
  align-items: start;
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

.valley-cards__card--offset {
  margin-top: clamp(36px, 5vw, 72px);
}

@media (max-width: 899px) {
  .valley-cards__grid { grid-template-columns: 1fr; }

  .valley-cards__card--offset { margin-top: 0; }
}
</style>
