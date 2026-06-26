<template>
  <section :id="sectionId" class="solutions-section">
    <div class="solutions-section__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      :eyebrow="eyebrow"
      :heading="heading"
      :heading-accent="headingAccent"
      :subtitle="subtitle"
    />

    <div
      class="solutions-section__cards"
      :class="{ 'solutions-section__cards--detail': tempExpanded }"
    >
      <SecondaryCard
        v-for="(option, idx) in options"
        :key="option.key"
        :variant="option.key === 'temp-to-perm' && tempExpanded ? 'red' : option.variant"
        :eyebrow="option.eyebrow"
        :title="option.title"
        :list="option.benefits"
        :delay="idx * 160"
        :expanded="option.key === 'temp-to-perm' && tempExpanded"
        class="solutions-section__card"
        :class="{
          'solutions-section__card--popped': option.key === 'temp-to-perm' && tempExpanded,
          'solutions-section__card--dimmed': option.key !== 'temp-to-perm' && tempExpanded,
        }"
      >
        <template v-if="option.body">
          {{ option.body }}
        </template>

        <template v-if="option.key === 'temp-to-perm'" #expanded>
          <TempToPermDetail />
        </template>

        <template #button>
          <ArrowPillCta
            v-if="option.key !== 'temp-to-perm'"
            :to="option.ctaTo"
            :hover-variant="option.variant === 'red' ? 'red' : 'cyan'"
          >
            {{ option.ctaLabel }}
          </ArrowPillCta>
          <ArrowPillCta
            v-else
            :hover-variant="tempExpanded ? 'red' : 'cyan'"
            :show-arrow="false"
            @click="tempExpanded = !tempExpanded"
          >
            {{ tempExpanded ? 'Show less' : 'View details' }}
          </ArrowPillCta>
        </template>
      </SecondaryCard>
    </div>
  </section>
</template>

<script lang="ts">
import { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'

export interface SolutionOption {
  readonly key: string
  readonly eyebrow: string
  readonly title: string
  readonly body?: string
  readonly benefits: readonly string[]
  readonly ctaLabel: string
  readonly ctaTo: string
  readonly variant: SecondaryCardVariant
}
</script>

<script setup lang="ts">
import { ref } from 'vue'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import ArrowPillCta from '@/components/landing/shared/ArrowPillCta.vue'
import SecondaryCard from '@/components/landing/shared/SecondaryCard.vue'
import TempToPermDetail from '@/components/landing/shared/TempToPermDetail.vue'

defineProps<{
  sectionId: string
  eyebrow: string
  heading: string
  headingAccent: string
  subtitle: string
  options: readonly SolutionOption[]
}>()

const tempExpanded = ref(false)
</script>

<style scoped>
.solutions-section {
  position: relative;
  width: 100%;
  margin-top: var(--panel-overlap);
  padding:
    var(--section-pad-y-lg)
    clamp(20px, 3vw, 64px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  z-index: 5;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  isolation: isolate;
  font-family: var(--font-family);
}

.solutions-section::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: var(--glass-blur-soft);
  -webkit-backdrop-filter: var(--glass-blur-soft);
  border: 1px solid var(--c-glass-border-soft);
  box-shadow: var(--sh-back);
  pointer-events: none;
}

.solutions-section__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

.solutions-section__cards {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  align-items: stretch;
  gap: clamp(20px, 2.4vw, 32px);
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

.solutions-section__card {
  position: relative;
  display: flex;
  flex-direction: column;
  transition:
    transform 0.55s cubic-bezier(0.34, 1.56, 0.64, 1),
    opacity 0.4s ease,
    filter 0.4s ease,
    box-shadow 0.5s ease;
}

.solutions-section__cards--detail {
  align-items: start;
}

.solutions-section__cards--detail .solutions-section__card--popped {
  z-index: 20;
  opacity: 1;
  transform: scale(1.06) translateY(-6px);
  box-shadow:
    0 44px 90px -28px rgba(0, 0, 0, 0.65),
    0 0 0 1px rgba(255, 255, 255, 0.12);
}

.solutions-section__cards--detail .solutions-section__card--dimmed {
  opacity: 0.22;
  filter: blur(2px) saturate(0.85);
  transform: scale(0.95);
  pointer-events: none;
}

@media (max-width: 899px) {
  .solutions-section__cards {
    grid-template-columns: 1fr;
  }

  .solutions-section__cards--detail .solutions-section__card--popped {
    transform: none;
    box-shadow: 0 24px 50px -20px rgba(0, 0, 0, 0.55);
  }

  .solutions-section__cards--detail .solutions-section__card--dimmed {
    opacity: 1;
    filter: none;
    transform: none;
    pointer-events: auto;
  }
}
</style>
