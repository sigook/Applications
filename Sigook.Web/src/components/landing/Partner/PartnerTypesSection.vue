<template>
  <section id="partner-types" class="partner-types">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="partner-types__surface" aria-hidden="true"></div>

    <header class="partner-types__header">
      <EyebrowPill variant="white" class="partner-types__eyebrow">
        Two Tracks
      </EyebrowPill>

      <h2 class="partner-types__heading">
        Two partner tracks.
        <span class="partner-types__heading-accent">
          One platform behind you.
        </span>
      </h2>

      <p class="partner-types__subtitle">
        Pick the model that fits your strength — sourcing talent or
        sourcing clients. We'll handle the rest, whichever side you bring.
      </p>
    </header>

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
          <a
            :href="track.ctaHref"
            class="partner-types__cta"
            :class="`partner-types__cta--${track.variant}`"
          >
            <span>{{ track.ctaLabel }}</span>
            <span class="partner-types__cta-arrow" aria-hidden="true">→</span>
          </a>
        </template>
      </PrimaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Partner — "Two Tracks" section.
 *
 * Replaces the Figma toggle (Recruiter / Business) with two side-by-side
 * PrimaryCards — both visible at once, no clicking required. This gives the
 * Sigook brand the same "audience picker" language already used on Home
 * (Find Work / Find Talent) and Employers/Talents Solutions sections.
 *
 * Recruiter Partner = navy variant (the steady, structured track).
 * Business Partner  = red variant (the entrepreneurial, revenue-share track).
 */
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import PrimaryCard, { type PrimaryCardVariant } from '@/components/landing/shared/PrimaryCard.vue'

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

const TRACKS: readonly PartnerTrack[] = [
  {
    key: 'recruiter',
    eyebrow: 'Independent Recruiter',
    title: 'Recruiter Partner',
    body:
      'Source and screen candidates for the industries you know best — engineering, logistics, construction, IT, or professional services. You stay independent; Sigook provides the compliance, payroll, and client onboarding behind you.',
    benefits: [
      'Full back-office support',
      'Compliance + payroll handled',
      'Client onboarding structure',
      'Your brand stays yours',
    ],
    ctaLabel: 'Apply as recruiter',
    ctaHref: '#partner-contact',
    variant: 'navy',
  },
  {
    key: 'business',
    eyebrow: 'Business Developer',
    title: 'Business Partner',
    body:
      'Bring your client relationships and let Sigook handle delivery. You share the ongoing revenue from every placement or project, while contracts, compliance, and client success live with us.',
    benefits: [
      'Ongoing revenue share',
      'Sigook handles delivery',
      'Keep your relationships',
      'Ideal for consultants',
    ],
    ctaLabel: 'Apply as business partner',
    ctaHref: '#partner-contact',
    variant: 'red',
  },
] as const
</script>

<style scoped>
/* ── Panel shell ────────────────────────────────────────────────────────── */
.partner-types {
  position: relative;
  width: 100%;
  margin-top: clamp(-180px, -10vw, -80px);
  padding:
    clamp(140px, 14vw, 200px)
    clamp(20px, 3vw, 64px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  z-index: 5;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  overflow: hidden;
  isolation: isolate;
  font-family: var(--font-family);
}

.partner-types__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

/* ── Header ─────────────────────────────────────────────────────────────── */
.partner-types__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.partner-types__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.partner-types__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.partner-types__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.partner-types__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 580px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Cards grid — 2 cols desktop, stack mobile ──────────────────────────── */
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

/* ── CTA pill in #button slot ───────────────────────────────────────────── */
.partner-types__cta {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: clamp(13px, 1.4vw, 16px) clamp(26px, 2.8vw, 36px);
  background: rgba(255, 255, 255, 0.12);
  border: 1.5px solid rgba(255, 255, 255, 0.55);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(14px, 1.15vw, 15px);
  font-weight: 700;
  letter-spacing: 0.04em;
  text-decoration: none;
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease;
}

.partner-types__cta--navy:hover {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-navy);
  transform: translateX(4px);
}

.partner-types__cta--red:hover {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-red);
  transform: translateX(4px);
}

.partner-types__cta-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.partner-types__cta:hover .partner-types__cta-arrow {
  transform: translateX(3px);
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .partner-types__cards { grid-template-columns: 1fr; }
}
</style>
