<template>
  <section class="dual-cta" aria-label="Find work or find talent">
    <!-- Full-bleed background layers (fill the entire section, any width) -->
    <div class="dual-cta__bg" aria-hidden="true">
      <img :src="talentsPhoto" alt="" class="dual-cta__bg-img dual-cta__bg-img--left" />
      <img :src="employersPhoto" alt="" class="dual-cta__bg-img dual-cta__bg-img--right" />
    </div>

    <div class="dual-cta__veil" aria-hidden="true"></div>
    <div class="dual-cta__glow" aria-hidden="true"></div>

    <!-- Inner canvas: max-width centered, holds the cards only -->
    <div class="dual-cta__canvas">
      <PrimaryCard
        variant="navy"
        eyebrow="For Talents"
        title="Find Work"
        :show-divider="true"
        class="dual-cta__card dual-cta__card--work"
        tabindex="0"
      >
        Where great talent meets great opportunities.<br />
        Browse openings and grow your career.

        <template #button>
          <button class="dual-cta__cta dual-cta__cta--work" type="button">
            <span>Browse Jobs</span>
            <ArrowIcon :width="36" :height="14" :stroke-width="2" color="currentColor" />
          </button>
        </template>
      </PrimaryCard>

      <PrimaryCard
        variant="red"
        eyebrow="For Employers"
        title="Find Talent"
        :show-divider="true"
        class="dual-cta__card dual-cta__card--talent"
        tabindex="0"
      >
        Find the right people for your business.<br />
        We match qualified workers to your needs, fast.

        <template #button>
          <button class="dual-cta__cta dual-cta__cta--talent" type="button">
            <span>Post a Job</span>
            <ArrowIcon :width="36" :height="14" :stroke-width="2" color="currentColor" />
          </button>
        </template>
      </PrimaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
import ArrowIcon from '@/components/v2/landing/shared/ArrowIcon.vue'
import PrimaryCard from '@/components/v2/landing/shared/PrimaryCard.vue'
import talentsPhoto from '@/assets/images/v2/audience-banner/talents-worker.jpg'
import employersPhoto from '@/assets/images/v2/audience-banner/employers-office.jpg'
</script>

<style scoped>
.dual-cta {
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

.dual-cta__canvas {
  position: relative;
  max-width: 1440px;
  width: 100%;
  height: 100%;
  margin: 0 auto;
}

/* Background photos split 50/50 */
.dual-cta__bg {
  position: absolute;
  inset: 0;
  display: flex;
  z-index: 1;
  overflow: hidden;
}

.dual-cta__bg-img {
  width: 50%;
  height: 100%;
  object-fit: cover;
  filter: brightness(0.62) saturate(0.92);
}

.dual-cta__bg-img--left { object-position: center 22%; }
.dual-cta__bg-img--right { object-position: center center; }

/* Veil: navy gradient that blends the two photos and gives glass the contrast it needs */
.dual-cta__veil {
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
.dual-cta__glow {
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

/*
  Cards positioned around the canvas center so they keep a consistent ~120px
  overlap zone at ANY desktop width (1024 → 1920+). Visual treatment (bg,
  radius, padding, shadow, glass blur) lives in PrimaryCard.
*/
.dual-cta__card {
  position: absolute;
  width: 560px;
  z-index: 3;
  outline: none;
  cursor: pointer;
}

/* Find Work — navy, lower-left */
.dual-cta__card--work {
  left: calc(50% - 500px);
  top: 200px;
}

/* Find Talent — red, upper-right, overlaps the work card horizontally */
.dual-cta__card--talent {
  left: calc(50% - 60px);
  top: 80px;
}

/* ── Custom CTA button — outline pill, hover invert ─────────────────────── */
.dual-cta__cta {
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

.dual-cta__cta--work:hover {
  background: #fff;
  color: var(--c-brand-navy);
  transform: translateX(4px);
}

.dual-cta__cta--talent:hover {
  background: #fff;
  color: var(--c-brand-red);
  transform: translateX(4px);
}

/* ── Mobile (≤ 1023px) ───────────────────────────────────── */
@media (max-width: 1023px) {
  .dual-cta {
    height: auto;
    padding: 90px 20px 110px;
    margin-top: -100px;
    border-radius: 80px 0 80px 0;
  }

  .dual-cta__canvas {
    height: auto;
    display: flex;
    flex-direction: column;
    gap: 28px;
    align-items: center;
  }

  .dual-cta__bg { flex-direction: column; }
  .dual-cta__bg-img { width: 100%; height: 50%; }

  .dual-cta__glow {
    width: 280px;
    height: 280px;
    filter: blur(90px);
  }

  .dual-cta__card {
    position: relative;
    width: 100%;
    max-width: 480px;
    left: auto;
    right: auto;
    top: auto;
  }
}
</style>
