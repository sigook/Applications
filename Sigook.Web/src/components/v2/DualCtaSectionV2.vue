<template>
  <section class="dual-cta-v2" aria-label="Find work or find talent">
    <!-- Full-bleed background layers (fill the entire section, any width) -->
    <div class="dual-cta-v2__bg" aria-hidden="true">
      <img :src="talentsPhoto" alt="" class="dual-cta-v2__bg-img dual-cta-v2__bg-img--left" />
      <img :src="employersPhoto" alt="" class="dual-cta-v2__bg-img dual-cta-v2__bg-img--right" />
    </div>

    <div class="dual-cta-v2__veil" aria-hidden="true"></div>
    <div class="dual-cta-v2__glow" aria-hidden="true"></div>

    <!-- Inner canvas: max-width centered, holds the cards only -->
    <div class="dual-cta-v2__canvas">
      <article class="dual-cta-v2__card dual-cta-v2__card--work" tabindex="0">
        <span class="dual-cta-v2__eyebrow">For Talents</span>
        <h2 class="dual-cta-v2__title">Find Work</h2>
        <div class="dual-cta-v2__line"></div>
        <p class="dual-cta-v2__body">
          Where great talent meets great opportunities.<br />
          Browse openings and grow your career.
        </p>
        <button class="dual-cta-v2__cta" type="button">
          <span>Browse Jobs</span>
          <ArrowIconV2 :width="36" :height="14" :stroke-width="2" color="currentColor" />
        </button>
      </article>

      <article class="dual-cta-v2__card dual-cta-v2__card--talent" tabindex="0">
        <span class="dual-cta-v2__eyebrow">For Employers</span>
        <h2 class="dual-cta-v2__title">Find Talent</h2>
        <div class="dual-cta-v2__line dual-cta-v2__line--cyan"></div>
        <p class="dual-cta-v2__body">
          Find the right people for your business.<br />
          We match qualified workers to your needs, fast.
        </p>
        <button class="dual-cta-v2__cta" type="button">
          <span>Post a Job</span>
          <ArrowIconV2 :width="36" :height="14" :stroke-width="2" color="currentColor" />
        </button>
      </article>
    </div>
  </section>
</template>

<script setup lang="ts">
import ArrowIconV2 from '@/components/v2/ArrowIconV2.vue'
import talentsPhoto from '@/assets/images/v2/audience-banner/talents-worker.jpg'
import employersPhoto from '@/assets/images/v2/audience-banner/employers-office.jpg'
</script>

<style scoped>
.dual-cta-v2 {
  position: relative;
  width: 100%;
  height: 720px;
  margin-top: -140px;
  z-index: 5;
  overflow: hidden;
  isolation: isolate;
  /* Asymmetric brand shape — top-left + bottom-right curve reveal Hero/Numbers in overlap zones */
  border-radius: 150px 0 150px 0;
  /* Soft drop shadows at top and bottom — smooth transitions into Hero (above) and Numbers (below) */
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
}

.dual-cta-v2__canvas {
  position: relative;
  max-width: 1440px;
  width: 100%;
  height: 100%;
  margin: 0 auto;
}

/* Background photos split 50/50 */
.dual-cta-v2__bg {
  position: absolute;
  inset: 0;
  display: flex;
  z-index: 1;
  overflow: hidden;
}

.dual-cta-v2__bg-img {
  width: 50%;
  height: 100%;
  object-fit: cover;
  filter: brightness(0.62) saturate(0.92);
}

.dual-cta-v2__bg-img--left { object-position: center 22%; }
.dual-cta-v2__bg-img--right { object-position: center center; }

/* Veil: navy gradient that blends the two photos and gives glass the contrast it needs */
.dual-cta-v2__veil {
  position: absolute;
  inset: 0;
  z-index: 2;
  background:
    linear-gradient(
      90deg,
      rgba(15, 47, 68, 0.55) 0%,
      rgba(15, 47, 68, 0.10) 50%,
      rgba(15, 47, 68, 0.55) 100%
    ),
    linear-gradient(
      180deg,
      rgba(15, 47, 68, 0.55) 0%,
      rgba(15, 47, 68, 0.18) 40%,
      rgba(15, 47, 68, 0.55) 100%
    );
}

/* Cyan glow accent — tertiary color, sits between the two cards */
.dual-cta-v2__glow {
  position: absolute;
  z-index: 2;
  pointer-events: none;
  width: 420px;
  height: 420px;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  background: var(--c-brand-cyan);
  border-radius: 50%;
  filter: blur(120px);
  opacity: 0.32;
}

/* ── Glass cards ─────────────────────────────────────── */
.dual-cta-v2__card {
  position: absolute;
  width: 560px;
  padding: 56px 56px 52px;
  border: 1px solid rgba(255, 255, 255, 0.20);
  box-shadow: 0 24px 60px rgba(0, 0, 0, 0.30);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  color: #fff;
  z-index: 3;
  transition: transform 0.5s cubic-bezier(0.22, 1, 0.36, 1),
              box-shadow 0.5s ease;
  outline: none;
  cursor: pointer;
}

.dual-cta-v2__card:hover,
.dual-cta-v2__card:focus-visible {
  transform: translateY(-8px);
  box-shadow: 0 32px 72px rgba(0, 0, 0, 0.40);
}

/*
  Cards positioned around the canvas center so they keep a consistent ~120px
  overlap zone at ANY desktop width (1024 → 1920+).
  Work spans (center − 500px) → (center + 60px).
  Talent spans (center − 60px)  → (center + 500px).
*/

/* Find Work — navy glass, lower-left */
.dual-cta-v2__card--work {
  left: calc(50% - 500px);
  top: 200px;
  background:
    linear-gradient(135deg,
      rgba(15, 47, 68, 0.70) 0%,
      rgba(21, 117, 187, 0.45) 100%
    );
  border-radius: 24px 96px 24px 96px;
}

/* Find Talent — red glass, upper-right, overlaps the work card horizontally */
.dual-cta-v2__card--talent {
  left: calc(50% - 60px);
  top: 80px;
  background:
    linear-gradient(135deg,
      rgba(229, 45, 39, 0.62) 0%,
      rgba(229, 45, 39, 0.40) 100%
    );
  border-radius: 96px 24px 96px 24px;
}

.dual-cta-v2__eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.20em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.82);
  margin-bottom: 16px;
}

.dual-cta-v2__title {
  font-family: var(--font-family);
  font-size: 64px;
  font-weight: 700;
  line-height: 1;
  margin: 0;
  color: #fff;
  letter-spacing: -0.01em;
}

.dual-cta-v2__line {
  width: 88px;
  height: 2px;
  background: #fff;
  margin: 22px 0 24px;
}

.dual-cta-v2__line--cyan { background: var(--c-brand-cyan); }

.dual-cta-v2__body {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.92);
  margin: 0 0 36px;
}

.dual-cta-v2__cta {
  display: inline-flex;
  align-items: center;
  gap: 14px;
  padding: 14px 28px;
  border: 1.5px solid rgba(255, 255, 255, 0.85);
  border-radius: 999px;
  background: transparent;
  color: #fff;
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 600;
  letter-spacing: 0.04em;
  cursor: pointer;
  transition: background 0.3s ease, color 0.3s ease, transform 0.3s ease;
}

.dual-cta-v2__card--work .dual-cta-v2__cta:hover {
  background: #fff;
  color: var(--c-brand-navy);
  transform: translateX(4px);
}

.dual-cta-v2__card--talent .dual-cta-v2__cta:hover {
  background: #fff;
  color: var(--c-brand-red);
  transform: translateX(4px);
}

/* ── Mobile (≤ 1023px) ───────────────────────────────────── */
@media (max-width: 1023px) {
  .dual-cta-v2 {
    height: auto;
    padding: 90px 20px 110px;
    margin-top: -100px;
    border-radius: 80px 0 80px 0;
  }

  .dual-cta-v2__canvas {
    height: auto;
    display: flex;
    flex-direction: column;
    gap: 28px;
    align-items: center;
  }

  .dual-cta-v2__bg { flex-direction: column; }
  .dual-cta-v2__bg-img { width: 100%; height: 50%; }

  .dual-cta-v2__glow {
    width: 280px;
    height: 280px;
    filter: blur(90px);
  }

  .dual-cta-v2__card {
    position: relative;
    width: 100%;
    max-width: 480px;
    left: auto;
    right: auto;
    top: auto;
    padding: 40px 32px 38px;
    backdrop-filter: blur(14px) saturate(150%);
    -webkit-backdrop-filter: blur(14px) saturate(150%);
  }

  .dual-cta-v2__title { font-size: 44px; }
  .dual-cta-v2__body { font-size: 15px; }
}
</style>
