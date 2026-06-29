<template>
  <section id="partner-types" class="partner-types">
    <div class="partner-types__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      eyebrow="Two Tracks"
      heading="Two partner tracks."
      heading-accent="One platform behind you."
      subtitle="Pick the model that fits your strength — sourcing talent or sourcing clients. We'll handle the rest, whichever side you bring."
      subtitle-max-width="580px"
    />

    <div class="partner-types__cards">
      <PrimaryCard
        v-for="(track, idx) in TRACKS"
        :key="track.key"
        :variant="track.variant"
        :eyebrow="track.eyebrow"
        :title="track.title"
        :list="track.benefits"
        :show-divider="true"
        :delay="idx * 200"
        class="partner-types__card"
      >
        {{ track.body }}

        <template #button>
          <ArrowPillCta
            :href="track.ctaHref"
            :hover-variant="track.variant"
            size="lg"
          >
            {{ track.ctaLabel }}
          </ArrowPillCta>
        </template>
      </PrimaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
import LandingSectionHeader from '@/components/landing/shared/sections/LandingSectionHeader.vue'
import ArrowPillCta from '@/components/landing/shared/ui/ArrowPillCta.vue'
import PrimaryCard, { type PrimaryCardVariant } from '@/components/landing/shared/cards/PrimaryCard.vue'
import partnerTracksData from '@/data/landing/partnerTracks.json'

interface PartnerTrack {
  readonly key: string
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly benefits: readonly string[]
  readonly ctaLabel: string
  readonly ctaHref: string
  readonly variant: PrimaryCardVariant
}

const TRACKS = partnerTracksData as readonly PartnerTrack[]
</script>

<style scoped>
.partner-types {
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

.partner-types::before {
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

.partner-types__surface {
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

.partner-types__cards {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: clamp(24px, 3.2vw, 40px);
  align-items: stretch;
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

.partner-types__card {
  display: flex;
  flex-direction: column;
}

@media (max-width: 899px) {
  .partner-types__cards { grid-template-columns: 1fr; }
}
</style>
