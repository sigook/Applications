<template>
  <section class="certified-v2">
    <!-- Photo layer 1 — plant/seedling (Figma: imgOntarioFoto, node 425:5796) -->
    <div class="certified-v2__bg" aria-hidden="true">
      <img src="@/assets/images/v2/certified-bg.jpg" alt="" class="certified-v2__photo certified-v2__photo--1" />
      <!-- Photo layer 2 — second photo stacked on top (Figma: imgOntarioFoto1) -->
      <img src="@/assets/images/v2/certified-bg2.jpg" alt="" class="certified-v2__photo certified-v2__photo--2" />
    </div>

    <!--
      Blue overlay — Figma shape: 1440×577, top-left radius 150px, bottom-right radius 150px
      (path: M1440 0H150C67.1573 0 0 67.1573 0 150V577H1290C1372.84 577 1440 509.843 1440 427V0Z).
      The rect fills the full SVG area; the parent section's overflow:hidden + border-radius
      clips it to the exact Figma shape at every viewport width without distortion.
    -->
    <svg
      class="certified-v2__overlay"
      viewBox="0 0 1440 577"
      preserveAspectRatio="none"
      xmlns="http://www.w3.org/2000/svg"
      aria-hidden="true"
    >
      <rect width="1440" height="577" fill="#1A75BB" fill-opacity="0.45"/>
    </svg>

    <div class="certified-v2__content">
      <h2 class="certified-v2__title">Smarter staffing for growing teams</h2>
      <p class="certified-v2__body">
        We support growing businesses with flexible staffing and recruitment solutions,
        combining experienced recruiters, streamlined processes, and reliable talent
        to meet evolving workforce needs.
      </p>
      <div class="certified-v2__badge">Grow With Us</div>
    </div>
  </section>
</template>

<script setup lang="ts">
// Static section — no reactive state needed
</script>

<style scoped>
/* ── Section shell ── */
.certified-v2 {
  position: relative;
  z-index: 2;          /* above WhyChooseUs (z-index:1) */
  width: 100%;
  height: 577px;
  overflow: hidden;
  /* Figma screenshot: curved top-left + bottom-right → 150px 0 150px 0 */
  border-radius: 150px 0 150px 0;
  /* Figma: Certified top=4054, WhyChooseUs panel top=2856+194(hero overlap)=actual top.
     Ontario Foto 2 (photo layer) = 556px tall within the 577px section.
     We overlap 556px into the Why panel so the bottom 21px of the section
     shows only the blue container (no photo), matching Figma's exact layout. */
  margin-top: -556px;
}

/* ── Photo background ── */
.certified-v2__bg {
  position: absolute;
  /* Ontario Foto 2 (photos) = 556px, section = 577px → 21px gap at bottom
     left uncovered so the blue container shows through (matches Figma). */
  inset: 0 0 21px;
  overflow: hidden;
}

.certified-v2__photo {
  position: absolute;
  left: 0;
  max-width: none;
  object-fit: cover;
}

/* Figma: height 194.24%, top -47.12% */
.certified-v2__photo--1 {
  width: 100%;
  height: 194.24%;
  top: -47.12%;
}

/* Figma: height 230.63%, left -0.56%, top -71.76%, width 118.73% */
.certified-v2__photo--2 {
  width: 118.73%;
  height: 230.63%;
  top: -71.76%;
  left: -0.56%;
}

/* Figma SVG overlay — exact path shape with rounded tl + br corners */
.certified-v2__overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: block;
  pointer-events: none;
  z-index: 0; /* above photos (.certified-v2__bg z-index auto), below content (z-index:1) */
}

/* ── Content ── */
.certified-v2__content {
  position: relative;
  z-index: 1;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  /* Figma title at doc-y 4194, section start 4054 → section-relative 140px from top */
  padding: 122px 80px 0;
}

/* Figma: Poppins Light 30px lh-1.2, centered, max-width 643px */
.certified-v2__title {
  font-family: var(--font-family, 'Poppins', sans-serif);
  font-size: 30px;
  font-weight: 300;
  line-height: 1.2;
  color: #fff;
  margin: 0 0 56px;
  max-width: 643px;
  text-transform: none;
}

/* Figma: Poppins Regular 15px lh-1.6, centered, max-width 578px */
.certified-v2__body {
  font-family: var(--font-family, 'Poppins', sans-serif);
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: #fff;
  margin: 0 0 40px;
  max-width: 578px;
}

/* Figma: neutral/muted #99B4BF badge, 324×63px, border-radius 20px */
.certified-v2__badge {
  background: var(--c-neutral-muted, #99B4BF);
  color: #fff;
  font-family: var(--font-family, 'Poppins', sans-serif);
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  padding: 12px 60px;
  border-radius: 20px;
  white-space: nowrap;
  min-width: 324px;
  text-align: center;
}

/* ── Mobile ── */
@media (max-width: 1023px) {
  .certified-v2 {
    height: auto;
    min-height: 400px;
    border-radius: 80px 0 80px 0;
    padding-bottom: 60px;
    margin-top: 0; /* reset desktop overlap — stack normally on mobile */
  }

  .certified-v2__content {
    padding: 80px 24px 0;
  }

  .certified-v2__badge {
    min-width: 0;
    padding: 12px 40px;
  }
}
</style>
