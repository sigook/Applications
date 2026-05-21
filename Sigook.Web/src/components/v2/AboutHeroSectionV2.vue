<template>
  <section class="about-hero-v2">
    <!-- Photo bg with rounded top-left + gradient overlay -->
    <div class="about-hero-v2__bg-layer" aria-hidden="true">
      <img
        src="@/assets/images/v2/about/hero-bg.png"
        alt=""
        class="about-hero-v2__bg-img"
      />
      <div class="about-hero-v2__overlay"></div>
    </div>

    <h2 class="about-hero-v2__title">
      <span class="about-hero-v2__title-line">About</span>
      <span class="about-hero-v2__title-line about-hero-v2__title-accent">Us</span>
    </h2>

    <div class="about-hero-v2__divider" aria-hidden="true"></div>

    <p class="about-hero-v2__paragraph">
      Since our beginnings in Florida in 2008, Sigook Work Factory has grown with one
      clear purpose — to bring people and opportunities together. Active in Talent
      Management since 2016, we've built lasting relationships across many industries.
      Behind every connection, there's a story — of people striving for better careers,
      of companies growing stronger, and of a team committed to making it happen.
    </p>
  </section>
</template>

<script setup lang="ts">
// About Us hero card — Figma node 425:7775
// Mobile dimensions: 440×595 with rounded top-left 150px (brand signature)
</script>

<style scoped>
/* ============================================================
   About Us hero card (mobile-first)
   Figma node: 425:7775
   ============================================================ */

.about-hero-v2 {
  position: relative;
  width: 100%;
  height: 595px;
  background: var(--c-brand-teal-dark);
  font-family: var(--font-family);
  color: var(--c-bg);
  overflow: hidden;
}

/* ── Photo bg layer with rounded top-left ───────────────────── */
.about-hero-v2__bg-layer {
  position: absolute;
  inset: 0;
  border-radius: var(--r-brand) 0 0 0;
  overflow: hidden;
  pointer-events: none;
  z-index: 0;
}

.about-hero-v2__bg-img {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center center;
}

/* Gradient overlay — fades from transparent (top 23%) to navy bottom */
.about-hero-v2__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    180deg,
    rgba(28, 73, 108, 0) 23.445%,
    var(--c-brand-navy) 100%
  );
}

/* ── Title — Figma: x=37, y=94 (in card coords), 40px Poppins Bold ─── */
.about-hero-v2__title {
  position: absolute;
  top: 94px;
  left: 37px;
  z-index: 1;
  margin: 0;
  font-size: 40px;
  font-weight: 700;
  line-height: 1.5; /* 60px × 2 lines = 120px (Figma title height) */
  white-space: nowrap;
}

.about-hero-v2__title-line {
  display: block;
}

.about-hero-v2__title-accent {
  color: var(--c-brand-cyan);
}

/* ── Divider — Figma: x=39, y=253, w=103, h=2 (Vector inset 1px) ──── */
.about-hero-v2__divider {
  position: absolute;
  top: 253px;
  left: 39px;
  z-index: 1;
  width: 103px;
  height: 2px;
  background: var(--c-bg);
  border-radius: 1px;
}

/* ── Paragraph — Figma: x=39, y=310, max-w=361 ───────────────────── */
.about-hero-v2__paragraph {
  position: absolute;
  top: 310px;
  left: 39px;
  right: 39px;
  z-index: 1;
  margin: 0;
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: var(--c-bg);
  text-align: justify;
  max-width: 361px;
}

/* ============================================================
   Desktop (≥1024px) — Figma node 425:6638 (1440×539)
   Horizontal layout: title left column, paragraph right column,
   divider tucked under the title on the left.
   ============================================================ */
@media (min-width: 1024px) {
  .about-hero-v2 {
    height: 539px;
  }

  /* Title — Figma: x=195, y=190 (section-relative) */
  .about-hero-v2__title {
    top: 190px;
    left: 195px;
  }

  /* Divider — Figma: x=197, y=315 (under title on the left) */
  .about-hero-v2__divider {
    top: 315px;
    left: 197px;
  }

  /* Paragraph — Figma: x=484, y=196, w=471 (right column, ~horizontally
     aligned with the title row) */
  .about-hero-v2__paragraph {
    top: 196px;
    left: 484px;
    right: auto;
    width: 471px;
    max-width: 471px;
  }
}
</style>
