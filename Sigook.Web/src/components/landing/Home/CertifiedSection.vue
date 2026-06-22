<template>
  <div class="certified-wrap">
    <!-- Back layer — same shape, sits 20px below the main section (peeks at bottom) -->
    <div class="certified__back" aria-hidden="true"></div>

    <section class="certified">
      <!-- Photo layers -->
      <div class="certified__bg" aria-hidden="true">
        <img src="@/assets/images/v2/certified/certified-bg.webp" alt="" class="certified__photo certified__photo--1" loading="lazy" decoding="async" />
        <img src="@/assets/images/v2/certified/certified-bg2.webp" alt="" class="certified__photo certified__photo--2" loading="lazy" decoding="async" />
      </div>

      <!-- Decorative cyan glow + brand magnifier -->
      <div class="certified__glow" aria-hidden="true"></div>
      <DecoMagnifier class="certified__magnifier" />

      <div class="certified__content">
        <div class="certified__left">
          <span class="certified__eyebrow">Our Promise</span>
          <h2 class="certified__title">Smarter staffing<br />for growing teams</h2>
          <div class="certified__divider" aria-hidden="true"></div>
        </div>
        <div class="certified__right">
          <p class="certified__body">
            We support growing businesses with flexible staffing and recruitment
            solutions, combining experienced recruiters, streamlined processes,
            and reliable talent to meet evolving workforce needs.
          </p>
          <ArrowPillCta to="#home-contact" hover-variant="navy">
            Grow With Us
          </ArrowPillCta>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import ArrowPillCta from '@/components/landing/shared/ArrowPillCta.vue'
import DecoMagnifier from '@/components/landing/shared/DecoMagnifier.vue'
</script>

<style scoped>
/* ── Wrapper — carries the overlap margin and z-index ── */
.certified-wrap {
  position: relative;
  z-index: 2;
  margin-top: -556px;
  /* 20px padding-top pushes the section down, exposing the back layer above it */
  padding-top: 20px;
}

/* ── Back layer — same dimensions + border-radius, 20px above section ── */
.certified__back {
  position: absolute;
  top: 4px;
  left: 0;
  width: 100%;
  height: 609px;
  border-radius: 150px 0 150px 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: blur(10px) saturate(120%);
  -webkit-backdrop-filter: blur(10px) saturate(120%);
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow: 0 18px 40px -20px rgba(0, 0, 0, 0.4);
  z-index: 0;
}

/* ── Section shell ── */
.certified {
  position: relative;
  z-index: 1;
  width: 100%;
  height: 577px;
  overflow: hidden;
  border-radius: 150px 0 150px 0;
  /* Cast a soft shadow downward to reinforce the overlap onto AppDownload */
  box-shadow: 0 22px 40px -12px rgba(0, 0, 0, 0.45);
}

/* ── Photo background ── */
.certified__bg {
  position: absolute;
  inset: 0;
  overflow: hidden;
}

.certified__photo {
  position: absolute;
  left: 0;
  max-width: none;
  object-fit: cover;
}

.certified__photo--1 {
  width: 100%;
  height: 194.24%;
  top: -47.12%;
}

.certified__photo--2 {
  width: 118.73%;
  height: 230.63%;
  top: -71.76%;
  left: -0.56%;
}

/* ── Navy gradient overlay — matches DualCta / WhyChooseUs hero veil ──── */
.certified::after {
  content: '';
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(15, 47, 68, 0.78) 0%, rgba(15, 47, 68, 0.45) 100%),
    linear-gradient(180deg, rgba(9, 48, 85, 0.20) 0%, rgba(15, 47, 68, 0.55) 100%);
  pointer-events: none;
  z-index: 0;
}

/* ── Decorative cyan glow — Hero language ──────────────────────────────── */
.certified__glow {
  position: absolute;
  top: -80px;
  right: -120px;
  width: 520px;
  height: 520px;
  background: var(--c-brand-cyan);
  border-radius: 50%;
  filter: blur(160px);
  opacity: 0.26;
  z-index: 1;
  pointer-events: none;
}

/* ── Brand magnifier — position only (size/float/shadow from DecoMagnifier) ─ */
.certified__magnifier {
  bottom: 40px;
  left: 56px;
}

/* ── Content — asymmetric split (heading left, body+CTA right) ─────────── */
.certified__content {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  height: 100%;
  padding: 0 80px;
  gap: 94px;
}

.certified__left {
  flex: 0 0 480px;
  text-align: right;
}

.certified__eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 14px;
}

.certified__title {
  font-family: var(--font-family);
  font-size: 56px;
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0;
}

.certified__divider {
  width: 88px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  margin: 24px 0 0 auto;
}

.certified__right {
  flex: 1;
}

.certified__body {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.92);
  margin: 0 0 28px;
  max-width: 560px;
}

/* ── Mobile ── */
@media (max-width: 1023px) {
  .certified-wrap {
    margin-top: -160px; /* overlaps the 160px blue extension of WhyChooseUs — photo sits on top of the blue */
    padding-top: 0;
  }

  .certified__back {
    display: none;
  }

  .certified {
    height: auto;
    min-height: 400px;
    border-radius: 80px 0 80px 0;
    padding-bottom: 80px;
    box-shadow: 0 22px 24px -12px rgba(0, 0, 0, 0.5);
  }

  .certified__glow {
    width: 320px;
    height: 320px;
    right: -100px;
    top: -60px;
    filter: blur(40px);
    opacity: 0.22;
  }

  .certified__magnifier {
    bottom: 20px;
    left: 20px;
  }

  .certified__content {
    flex-direction: column;
    align-items: flex-start;
    padding: 80px 24px 0;
    gap: 24px;
    height: auto;
  }

  .certified__left {
    flex: none;
    width: 100%;
    text-align: left;
  }

  .certified__eyebrow {
    margin-bottom: 10px;
  }

  .certified__title {
    font-size: 32px;
  }

  .certified__divider {
    margin: 16px 0 0 0;          /* left-aligned on mobile to match text */
    width: 64px;
  }

  .certified__right {
    width: 100%;
  }

  .certified__body {
    font-size: 15px;
    margin-bottom: 24px;
  }
}
</style>
