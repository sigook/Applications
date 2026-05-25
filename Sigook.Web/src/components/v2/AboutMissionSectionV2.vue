<template>
  <section class="about-mission-v2">
    <!-- Atmospheric magnifier decoration — bottom-left anchor (varies from Hero/Intro placements) -->
    <DecoMagnifierV2 class="about-mission-v2__magnifier" />

    <header class="about-mission-v2__header">
      <EyebrowPillV2 variant="white" class="about-mission-v2__eyebrow">
        Why We Exist
      </EyebrowPillV2>

      <h2 class="about-mission-v2__heading">
        Built on purpose.
        <span class="about-mission-v2__heading-accent">Driven by people.</span>
      </h2>

      <p class="about-mission-v2__intro">
        Two questions guide everything we build — what we're here to do, and the
        future we're trying to make real.
      </p>
    </header>

    <div class="about-mission-v2__cards">
      <!-- Card 01 — Mission (cyan accent) -->
      <article class="about-mission-v2__card about-mission-v2__card--mission">
        <span class="about-mission-v2__index" aria-hidden="true">01</span>

        <span class="about-mission-v2__card-eyebrow about-mission-v2__card-eyebrow--cyan">
          Our Mission
        </span>

        <h3 class="about-mission-v2__card-heading">
          Connect great people with great opportunities — fairly, transparently,
          and at the pace today's businesses move.
        </h3>

        <p class="about-mission-v2__card-body">
          We exist to remove the friction between talent and the companies that
          need them — replacing slow, opaque hiring with technology that respects
          everyone's time and treats every match as a relationship, not a
          transaction.
        </p>
      </article>

      <!-- Card 02 — Vision (red accent, staggered down) -->
      <article class="about-mission-v2__card about-mission-v2__card--vision">
        <span class="about-mission-v2__index" aria-hidden="true">02</span>

        <span class="about-mission-v2__card-eyebrow about-mission-v2__card-eyebrow--red">
          Our Vision
        </span>

        <h3 class="about-mission-v2__card-heading">
          A North American workforce where every connection sparks real growth —
          for workers, for companies, and for the communities they share.
        </h3>

        <p class="about-mission-v2__card-body">
          We see a future where staffing is built on trust, not transactions, and
          where opportunity travels as freely as the people who pursue it —
          across borders, industries, and generations.
        </p>
      </article>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * About — Mission & Vision section.
 *
 * Replaces the legacy AboutHeroSectionV2 which duplicated intro content.
 * Editorial declarative layout: a header on top, two glass cards staggered
 * vertically with large background indices (01 / 02) — visual rhythm closer
 * to a magazine spread than the action-oriented DualCta dual cards.
 *
 * Composition rules:
 *  • Mission card pulls cyan; Vision card pulls red — keeps brand duality
 *  • Cards have mirrored asymmetric border-radius (signature brand shape)
 *  • Card 02 sits lower via margin-top to break the symmetry of DualCta
 *  • No CTA — this is a declaration of purpose, not an action prompt
 */
import DecoMagnifierV2 from '@/components/v2/shared/DecoMagnifierV2.vue'
import EyebrowPillV2 from '@/components/v2/shared/EyebrowPillV2.vue'
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.about-mission-v2 {
  position: relative;
  width: 100%;
  padding:
    clamp(72px, 10vw, 140px)
    clamp(20px, 3vw, 40px)
    clamp(96px, 12vw, 180px);
  isolation: isolate;
  overflow: hidden;
}

/* ── Decorative magnifier — bottom-left anchor (differs from Intro/Hero) ── */
.about-mission-v2__magnifier {
  bottom: clamp(8%, 10vw, 14%);
  left: clamp(5%, 6vw, 8%);
}

/* ── Header block — editorial intro ─────────────────────────────────────── */
.about-mission-v2__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto clamp(48px, 6vw, 80px);
}

.about-mission-v2__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.about-mission-v2__heading {
  font-family: var(--font-family);
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.1;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.about-mission-v2__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.about-mission-v2__intro {
  font-family: var(--font-family);
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.75);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Cards grid — staggered dual layout ─────────────────────────────────── */
.about-mission-v2__cards {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: clamp(24px, 3.5vw, 48px);
  align-items: start;
  max-width: 1180px;
  margin: 0 auto;
}

/* ── Card shell — glass surface ─────────────────────────────────────────── */
.about-mission-v2__card {
  position: relative;
  padding:
    clamp(32px, 4vw, 56px)
    clamp(28px, 3.5vw, 48px);
  background: rgba(255, 255, 255, 0.045);
  border: 1px solid rgba(255, 255, 255, 0.10);
  backdrop-filter: blur(14px) saturate(140%);
  -webkit-backdrop-filter: blur(14px) saturate(140%);
  overflow: hidden;
  isolation: isolate;
  transition: transform 0.35s cubic-bezier(0.22, 1, 0.36, 1),
              border-color 0.35s ease,
              background 0.35s ease;
}

.about-mission-v2__card:hover {
  background: rgba(255, 255, 255, 0.07);
  border-color: rgba(255, 255, 255, 0.18);
  transform: translateY(-4px);
}

/* Mirrored asymmetric brand radius — Mission rounds TL+BR, Vision rounds TR+BL */
.about-mission-v2__card--mission {
  border-radius: clamp(48px, 7vw, 96px) 0 clamp(48px, 7vw, 96px) 0;
}

.about-mission-v2__card--vision {
  border-radius: 0 clamp(48px, 7vw, 96px) 0 clamp(48px, 7vw, 96px);
  /* Stagger down — visual rhythm, breaks DualCta-style symmetry */
  margin-top: clamp(40px, 6vw, 96px);
}

/* ── Background index 01/02 — magazine-style ghost numeral ──────────────── */
.about-mission-v2__index {
  position: absolute;
  top: clamp(-4px, -0.5vw, 4px);
  right: clamp(12px, 2vw, 28px);
  z-index: 0;
  font-family: var(--font-family);
  font-size: clamp(120px, 16vw, 220px);
  font-weight: 800;
  line-height: 0.85;
  letter-spacing: -0.04em;
  color: rgba(255, 255, 255, 0.07);
  pointer-events: none;
  user-select: none;
}

/* Tint the ghost numeral per-card to echo each card's accent */
.about-mission-v2__card--mission .about-mission-v2__index {
  color: rgba(0, 173, 239, 0.10);
}

.about-mission-v2__card--vision .about-mission-v2__index {
  color: rgba(229, 45, 39, 0.10);
}

/* ── Card eyebrow — small uppercase tracked ─────────────────────────────── */
.about-mission-v2__card-eyebrow {
  position: relative;
  z-index: 1;
  display: inline-block;
  font-family: var(--font-family);
  font-size: clamp(10px, 0.85vw, 12px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  margin-bottom: clamp(16px, 2vw, 24px);
}

.about-mission-v2__card-eyebrow--cyan { color: var(--c-brand-cyan); }
.about-mission-v2__card-eyebrow--red  { color: var(--c-brand-red);  }

/* ── Card heading — primary statement (medium editorial) ────────────────── */
.about-mission-v2__card-heading {
  position: relative;
  z-index: 1;
  font-family: var(--font-family);
  font-size: clamp(20px, 2.2vw, 28px);
  font-weight: 700;
  line-height: 1.25;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  text-shadow: 0 2px 14px rgba(0, 0, 0, 0.30);
}

/* ── Card body — supporting paragraph ───────────────────────────────────── */
.about-mission-v2__card-body {
  position: relative;
  z-index: 1;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.15vw, 15px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
}

/* ── Mobile-only behaviors (conditional layout shifts) ──────────────────── */
@media (max-width: 899px) {
  .about-mission-v2__cards {
    grid-template-columns: 1fr;
  }

  /* Stagger only makes sense in 2-col layout — drop it on stack */
  .about-mission-v2__card--vision {
    margin-top: 0;
  }

  /* Magnifier moves to keep clear of cards stack */
  .about-mission-v2__magnifier {
    bottom: 6%;
    left: 6%;
  }
}
</style>
