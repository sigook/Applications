<template>
  <section class="why-v2">
    <!-- Back layer — same shape as the photo, sits 20px above so it peeks out at the top -->
    <div class="why-v2__hero-back" aria-hidden="true"></div>

    <!-- Block A — Corporate buildings photo -->
    <div class="why-v2__hero">
      <img src="@/assets/images/v2/why-choose-us/why-bg.jpg" alt="" class="why-v2__hero-bg" aria-hidden="true" />
      <div class="why-v2__hero-overlay" aria-hidden="true"></div>

      <!-- Decorative cyan glow + thin lines (Hero language) -->
      <div class="why-v2__hero-glow" aria-hidden="true"></div>
      <div class="why-v2__hero-lines" aria-hidden="true">
        <span class="why-v2__hero-line"></span>
        <span class="why-v2__hero-line"></span>
      </div>

      <div class="why-v2__hero-content">
        <div class="why-v2__hero-left">
          <span class="why-v2__hero-eyebrow">Our Approach</span>
          <h2 class="why-v2__hero-title">Why Choose Us?</h2>
          <div class="why-v2__hero-divider" aria-hidden="true"></div>
        </div>
        <div class="why-v2__hero-right">
          <p class="why-v2__hero-body">
            We are a Talent Management Agency focused on providing skilled professionals,
            tailored workforce solutions, and reliable support to ensure every partnership
            runs smoothly and efficiently.
          </p>
        </div>
      </div>
    </div>

    <!-- Block B — Blue content panel -->
    <div class="why-v2__panel">
      <div class="why-v2__panel-inner">

        <!-- Row 1: Map (left) + Nationwide vision (right) -->
        <div class="why-v2__row why-v2__row--top">
          <!-- Map column — pins PNG (transparent bg) floats directly on the blue panel -->
          <div class="why-v2__col why-v2__col--map">
            <img
              src="@/assets/images/v2/why-choose-us/usa-map.png"
              alt="USA/Canada presence map"
              class="why-v2__map-img"
            />
          </div>

          <div class="why-v2__col why-v2__col--nationwide">
            <h2 class="v2-h2 why-v2__col-title">
              Nationwide vision<br>with local focus
            </h2>
            <p class="v2-body why-v2__col-body">
              We combine the reach of a nationwide recruitment network with the
              dedication and understanding of local expertise, ensuring tailored
              solutions that truly meet each client's unique needs.
            </p>
            <a href="#" class="btn btn--primary btn--sm why-v2__btn" @click.prevent>
              Learn More
              <ArrowIconV2 class="why-v2__btn-arrow" :width="32" :height="11" :stroke-width="1.5" />
            </a>
          </div>
        </div>

        <!-- Row 2: Market Leaders (left) + Recruitment Graphic (right) -->
        <div class="why-v2__row why-v2__row--bottom">
          <div class="why-v2__col why-v2__col--leaders">
            <h2 class="v2-h2 why-v2__col-title why-v2__col-title--right">
              We are Market<br>Leaders &amp; Innovators
            </h2>
            <p class="v2-body why-v2__col-body why-v2__col-body--right">
              At Sigook Work Factory, we pride ourselves on being trusted market leaders
              and forward-thinking innovators. By combining industry expertise with
              modern strategies, we create meaningful connections between businesses and
              top talent. Our commitment to excellence ensures that both employers and
              job seekers achieve success in a rapidly changing world.
            </p>
            <div class="why-v2__col-btn-right">
              <a href="#" class="btn btn--primary btn--sm why-v2__btn" @click.prevent>
                Learn More
                <ArrowIconV2 class="why-v2__btn-arrow" :width="32" :height="11" :stroke-width="1.5" />
              </a>
            </div>
          </div>

          <div class="why-v2__col why-v2__col--graphic">
            <!-- Figma asset: recruitment graphic 01 1 (node 2442:670), 480×360px -->
            <img
              src="@/assets/images/v2/why-choose-us/recruitment-graphic.png"
              alt=""
              aria-hidden="true"
              class="why-v2__graphic-img"
            />
          </div>
        </div>

      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import ArrowIconV2 from '@/components/v2/ArrowIconV2.vue'
</script>

<style scoped>
.why-v2 {
  position: relative;
  z-index: 1;           /* above Numbers (z-index:0) */
  margin-top: -260px;   /* overlaps the bottom 260px of Numbers: Numbers ends at y=2724, WhyChooseUs starts at y=2464 */
}

/* ── Back layer for the hero — peeks 20px above & sides, same shape ─────── */
.why-v2__hero-back {
  position: absolute;
  top: -20px;
  left: 0;
  width: 100%;
  height: 588px;
  border-radius: 150px 0 150px 0;
  background: rgba(26, 117, 187, 0.45);
  z-index: 1;
  pointer-events: none;
}

/* ── Block A: Photo ── */
.why-v2__hero {
  position: relative;
  z-index: 2;       /* above the blue panel (z-index:1) — matches Figma layer order:
                       corporate buildings 01 1 (node 425:5791) is listed after
                       Rectangle 16 (the panel) in Figma, meaning it renders on top */
  width: 100%;
  height: 588px;
  border-radius: 150px 0 150px 0;
  overflow: hidden;
  /* Soft drop shadows top + bottom — smooth transitions with Numbers (above) and panel (below) */
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
}

.why-v2__hero-bg {
  position: absolute;
  width: 100%;
  height: 200%;
  top: -30%;
  left: 0;
  object-fit: cover;
  object-position: center 30%;
}

/* Navy gradient overlay — replaces the green-tinted one, matches DualCta veil language */
.why-v2__hero-overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(15, 47, 68, 0.78) 0%, rgba(15, 47, 68, 0.45) 100%),
    linear-gradient(180deg, rgba(9, 48, 85, 0.20) 0%, rgba(15, 47, 68, 0.55) 100%);
}

/* Cyan glow accent — tertiary color, top-right corner */
.why-v2__hero-glow {
  position: absolute;
  top: -80px;
  right: -120px;
  width: 540px;
  height: 540px;
  background: var(--c-brand-cyan);
  border-radius: 50%;
  filter: blur(160px);
  opacity: 0.24;
  z-index: 1;
  pointer-events: none;
}

/* Thin geometric line accents — bottom-left, echoes Hero deco lines */
.why-v2__hero-lines {
  position: absolute;
  bottom: 56px;
  left: 60px;
  display: flex;
  flex-direction: column;
  gap: 6px;
  z-index: 1;
  pointer-events: none;
}

.why-v2__hero-line {
  display: block;
  width: 96px;
  height: 2px;
  background: rgba(255, 255, 255, 0.40);
  border-radius: 2px;
}

.why-v2__hero-line:nth-child(2) {
  width: 64px;
  margin-left: 18px;
}

.why-v2__hero-content {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  height: 100%;
  padding: 0 80px;
  gap: 94px;
}

.why-v2__hero-left {
  flex: 0 0 480px;
  text-align: right;
}

.why-v2__hero-eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 14px;
}

.why-v2__hero-title {
  font-family: var(--font-family);
  font-size: 60px;
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0;
}

/* Cyan divider line under title — right-aligned to match text */
.why-v2__hero-divider {
  width: 88px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  margin: 24px 0 0 auto;
}

.why-v2__hero-right {
  flex: 1;
}

.why-v2__hero-body {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.92);
  margin: 0;
  max-width: 560px;
}

/* ── Block B: Blue panel ── */
.why-v2__panel {
  position: relative;
  background: var(--c-brand-blue);
  border-radius: 150px 0 150px 0;
  margin-top: -196px;
  z-index: 1;
  padding-bottom: 160px;
  overflow: hidden;
  /* Figma: Rectangle 16 (node 425:5688) height = 1754px */
  min-height: 1900px;
}

.why-v2__panel-inner {
  max-width: 1440px;
  margin: 0 auto;
  padding: 248px 80px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
}

/* ── Rows ── */
.why-v2__row {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 92px;
  width: 100%;
  max-width: 1100px;
}

.why-v2__row--bottom {
  margin-top: 120px;
}

/* ── Columns ── */
.why-v2__col {
  flex: 1;
}

.why-v2__col--map {
  flex: 1;
}

.why-v2__col--nationwide {
  flex: 1;
}

.why-v2__col--leaders {
  flex: 1;
}

.why-v2__col--graphic {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* ── Map pins image — transparent PNG floats on blue panel background ── */
.why-v2__map-img {
  width: 100%;
  height: auto;
  display: block;
}

/* ── Recruitment graphic image — Figma node 2442:670, 480×360px ── */
.why-v2__graphic-img {
  width: 100%;
  max-width: 480px;
  height: auto;
  display: block;
  margin: 0 auto;
}

/* ── Column text ── */
.why-v2__col-title {
  color: #fff;
  margin: 0;
}

.why-v2__col-title--right {
  text-align: right;
}

.why-v2__col-body {
  color: rgba(255, 255, 255, 0.9);
  margin: 20px 0 24px;
}

.why-v2__col-body--right {
  text-align: right;
}

.why-v2__col-btn-right {
  text-align: right;
}

/* ── Learn More buttons ── */
.why-v2__btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
}

.why-v2__btn-arrow {
  flex-shrink: 0;
}

/* ── Mobile ── */
@media (max-width: 1023px) {
  .why-v2 {
    margin-top: -100px;  /* overlaps bottom 100px of Numbers background (z-index:1 already set) */
  }

  .why-v2__hero-back {
    display: none;       /* skip on mobile — overlap math + smaller card make it noisy */
  }

  .why-v2__hero {
    height: auto;
    min-height: 280px;
    border-radius: 80px 0 80px 0;
    padding: 60px 24px;
  }

  .why-v2__hero-bg {
    height: 100%;
    top: 0;
  }

  .why-v2__hero-content {
    flex-direction: column;
    align-items: flex-start;
    padding: 0;
    height: auto;
    gap: 20px;
  }

  .why-v2__hero-glow {
    width: 320px;
    height: 320px;
    top: -60px;
    right: -100px;
    filter: blur(110px);
    opacity: 0.20;
  }

  .why-v2__hero-lines {
    bottom: 24px;
    left: 24px;
  }

  .why-v2__hero-line {
    width: 56px;
  }

  .why-v2__hero-line:nth-child(2) {
    width: 36px;
    margin-left: 10px;
  }

  .why-v2__hero-left {
    flex: none;
    width: 100%;
    text-align: left;
  }

  .why-v2__hero-eyebrow {
    margin-bottom: 10px;
  }

  .why-v2__hero-title {
    font-size: 36px;
  }

  .why-v2__hero-divider {
    margin: 16px 0 0 0;       /* left-aligned on mobile to match left-aligned text */
    width: 64px;
  }

  .why-v2__hero-right {
    width: 100%;
  }

  .why-v2__hero-body {
    font-size: 15px;
  }

  .why-v2__panel {
    border-radius: 80px 0 80px 0;
    margin-top: -80px;
    min-height: 0; /* reset desktop min-height — mobile stacks naturally */
    padding-bottom: 220px; /* extends blue downward — certified section overlaps this extra space */
  }

  .why-v2__panel-inner {
    padding: 120px 24px 0;
  }

  .why-v2__row {
    flex-direction: column;
    gap: 40px;
    max-width: 100%;
  }

  .why-v2__row--bottom {
    margin-top: 48px; /* reduce from 120px on desktop */
  }

  .why-v2__col--leaders,
  .why-v2__col--map {
    flex: none;
    width: 100%;
  }

  .why-v2__col--nationwide {
    padding-top: 0;
  }

  .why-v2__col-title--right,
  .why-v2__col-body--right,
  .why-v2__col-btn-right {
    text-align: left;
  }
}
</style>
