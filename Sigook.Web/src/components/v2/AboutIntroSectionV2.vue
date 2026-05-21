<template>
  <section class="about-intro-v2">
    <!-- Abstract bg image with gradient overlay — covers top 781px (Intro zone) -->
    <div class="about-intro-v2__abstract" aria-hidden="true">
      <img
        src="@/assets/images/v2/about/intro-bg.png"
        alt=""
        class="about-intro-v2__abstract-img"
      />
      <div class="about-intro-v2__abstract-overlay"></div>
    </div>

    <!-- Header copy + CTA — Figma frame: y=246, w=362 -->
    <div class="about-intro-v2__content">
      <h1 class="about-intro-v2__title">
        Empowering People. Strengthening
        <span class="about-intro-v2__title-accent">Businesses.</span>
      </h1>

      <p class="about-intro-v2__subtitle">
        Our success is built on values we live by: fairness, respect, and genuine care
        for those we serve. At Sigook Work Factory, we believe that trust and integrity
        are what truly connect people — and that's where lasting partnerships begin.
      </p>

      <a href="#about-form" class="btn btn--secondary about-intro-v2__cta">
        Get in Touch <span class="about-intro-v2__cta-arrow" aria-hidden="true">›</span>
      </a>
    </div>

    <!-- 2×2 numbers grid — Figma: y=824, 360×381 -->
    <NumbersGridV2 class="about-intro-v2__numbers" />
  </section>
</template>

<script setup lang="ts">
import NumbersGridV2 from '@/components/v2/NumbersGridV2.vue'
</script>

<style scoped>
/* ============================================================
   About — Intro Section (mobile-first)
   Figma node: 425:7763 + 2374:670 (Intro frame + NumbersGrid)
   Total height target: 1248px
     • 781px Intro frame (abstract bg + copy + CTA)
     • 43px gap
     • 381px Numbers grid
     • 43px bottom gap (before About Us hero card starts)
   ============================================================ */

.about-intro-v2 {
  position: relative;
  width: 100%;
  height: 1248px;
  background: var(--c-brand-teal-dark);
  overflow: hidden;
  font-family: var(--font-family);
  color: var(--c-bg);
}

/* ── Abstract bg image — top 781px, gradient fades to teal-dark ─────── */
.about-intro-v2__abstract {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 781px;
  pointer-events: none;
  z-index: 0;
}

.about-intro-v2__abstract-img {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center top;
}

.about-intro-v2__abstract-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    180deg,
    rgba(15, 47, 68, 0) 28.681%,
    var(--c-brand-teal-dark) 100%
  );
}

/* ── Copy block — Figma: x=39, y=246 ─────────────────────────────────── */
.about-intro-v2__content {
  position: absolute;
  top: 246px;
  left: 39px;
  right: 39px;
  z-index: 1;
  display: flex;
  flex-direction: column;
}

.about-intro-v2__title {
  margin: 0;
  font-size: 40px;
  font-weight: 700;
  line-height: 1.5; /* 60px per line × 4 lines = 240px (Figma height) */
  color: var(--c-bg);
  max-width: 362px;
}

.about-intro-v2__title-accent {
  color: var(--c-brand-cyan);
}

.about-intro-v2__subtitle {
  margin: 12px 0 0; /* gap before subtitle */
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: var(--c-bg);
  text-align: justify;
  max-width: 361px;
}

.about-intro-v2__cta {
  margin-top: 30px;
  align-self: flex-start;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  min-width: 196px;
  justify-content: center;
}

.about-intro-v2__cta-arrow {
  font-size: 20px;
  line-height: 1;
}

/* ── Numbers grid — Figma: x=41, y=824 (43px after intro frame) ──────── */
.about-intro-v2__numbers {
  position: absolute;
  top: 824px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 1;
}

/* ============================================================
   Desktop (≥1024px) — Figma nodes 425:6607 + 2382:4
   Total height target: 1656px
     • 781px Intro frame (abstract bg + copy + CTA at x=169/168)
     • 43px gap
     • 832px Numbers grid (834×832, centered)
   ============================================================ */
@media (min-width: 1024px) {
  .about-intro-v2 {
    /* 1656 (intro frame + numbers) + 117 gap before About Us hero */
    height: 1773px;
  }

  /* Abstract bg still covers the top 781px (full width) */

  /* ── Copy block — Figma: x=169, y=198 ──────────────────────────── */
  .about-intro-v2__content {
    top: 198px;
    left: 169px;
    right: auto;
    width: 563px; /* room for the wider subtitle */
  }

  .about-intro-v2__title {
    font-size: 50px;
    line-height: 1.5; /* 75px × 4 lines = 300px (Figma title height) */
    max-width: 409px;
  }

  /* gap 13px after title (Figma: title ends y=498, subtitle y=511) */
  .about-intro-v2__subtitle {
    margin-top: 13px;
    max-width: 563px;
  }

  /* Strict anchor at y=618 from section top (Figma) regardless of how the
     subtitle wraps. Mobile keeps the margin-top flow. */
  .about-intro-v2__cta {
    position: absolute;
    top: 420px; /* 618 (Figma section y) - 198 (content y) = 420 */
    left: 0;
    margin-top: 0;
  }

  /* Numbers grid — Figma: x=296, y=824 (centered with -7px nudge) */
  .about-intro-v2__numbers {
    top: 824px;
  }
}
</style>
