<template>
  <section id="about-numbers" class="about-numbers">
    <!-- Inner glass surface — materializes the panel against GlobalBackground,
         keeps shape consistency with DualCtaSection from Home. -->
    <div class="about-numbers__surface" aria-hidden="true"></div>

    <header class="about-numbers__header">
      <EyebrowPill variant="cyan" class="about-numbers__eyebrow">
        By the Numbers
      </EyebrowPill>

      <h2 class="about-numbers__heading">
        A track record built
        <span class="about-numbers__heading-accent">connection by connection.</span>
      </h2>

      <p class="about-numbers__subtitle">
        Seventeen years of bringing people and opportunities together — and the
        numbers behind every story.
      </p>
    </header>

    <NumbersGrid :stats="ABOUT_STATS" class="about-numbers__grid" />
  </section>
</template>

<script setup lang="ts">
/**
 * About — "By the numbers" section.
 *
 * Adopts the Home page panel vocabulary established by DualCtaSection:
 * asymmetric brand radius (TL + BR), dual top/bottom drop shadows, negative
 * margin-top to overlap the previous section, and an inner glass surface
 * that materializes the panel on top of the GlobalBackground.
 *
 * Stats data lives here (page-owned content); NumbersGrid is a pure
 * layout atom.
 */
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
import NumbersGrid, { type Stat } from '@/components/v2/landing/shared/NumbersGrid.vue'

const ABOUT_STATS: readonly Stat[] = [
  { value: '10',    label: 'Years of Experience', tone: 'blue' },
  { value: '780',   label: 'Clients Served',      tone: 'cyan' },
  { value: '1,700', label: 'Jobs Posted',         tone: 'red'  },
  { value: '5,000', label: 'Applications Filed',  tone: 'cyan' },
] as const
</script>

<style scoped>
/* ── Panel shell — adopts DualCta vocabulary (TL+BR radius, dual shadow) ── */
.about-numbers {
  position: relative;
  width: 100%;
  /* Overlap previous section (Intro) — matches Home DualCta -140px vibe */
  margin-top: clamp(-180px, -10vw, -80px);
  padding:
    clamp(140px, 14vw, 200px)
    clamp(20px, 3vw, 64px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(36px, 5vw, 64px);
  z-index: 5;
  /* Asymmetric brand shape — TL + BR rounded, mirrors DualCta */
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  /* Dual drop shadows top + bottom for smooth transitions into neighbours */
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  overflow: hidden;
  isolation: isolate;
}

/* ── Inner glass surface — semi-opaque navy lets GlobalBg breathe through ── */
.about-numbers__surface {
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

/* ── Header block — editorial intro for the grid ────────────────────────── */
.about-numbers__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.about-numbers__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.about-numbers__heading {
  font-family: var(--font-family);
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(16px, 2vw, 24px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.about-numbers__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.about-numbers__subtitle {
  font-family: var(--font-family);
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Grid wrapper — atom owns internal sizing, we just center + raise z ─── */
.about-numbers__grid {
  position: relative;
  z-index: 2;
  margin: 0 auto;
}
</style>
