<template>
  <div class="certified-v2-wrap">
    <!-- Back layer — same shape, sits 20px below the main section (peeks at bottom) -->
    <div class="certified-v2__back" aria-hidden="true"></div>

    <section class="certified-v2">
      <!-- Photo layers -->
      <div class="certified-v2__bg" aria-hidden="true">
        <img src="@/assets/images/v2/certified/certified-bg.jpg" alt="" class="certified-v2__photo certified-v2__photo--1" />
        <img src="@/assets/images/v2/certified/certified-bg2.jpg" alt="" class="certified-v2__photo certified-v2__photo--2" />
      </div>

      <!-- Decorative cyan glow + brand magnifier -->
      <div class="certified-v2__glow" aria-hidden="true"></div>
      <img
        src="@/assets/images/v2/branding/sigook-magnifier.png"
        alt=""
        aria-hidden="true"
        class="certified-v2__magnifier"
      />

      <div class="certified-v2__content">
        <div class="certified-v2__left">
          <span class="certified-v2__eyebrow">Our Promise</span>
          <h2 class="certified-v2__title">Smarter staffing<br />for growing teams</h2>
          <div class="certified-v2__divider" aria-hidden="true"></div>
        </div>
        <div class="certified-v2__right">
          <p class="certified-v2__body">
            We support growing businesses with flexible staffing and recruitment
            solutions, combining experienced recruiters, streamlined processes,
            and reliable talent to meet evolving workforce needs.
          </p>
          <a href="#" class="certified-v2__cta" @click.prevent>
            <span>Grow With Us</span>
            <ArrowIconV2 :width="32" :height="11" :stroke-width="1.5" />
          </a>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import ArrowIconV2 from '@/components/v2/ArrowIconV2.vue'
</script>

<style scoped>
/* ── Wrapper — carries the overlap margin and z-index ── */
.certified-v2-wrap {
  position: relative;
  z-index: 2;
  margin-top: -556px;
  /* 20px padding-top pushes the section down, exposing the back layer above it */
  padding-top: 20px;
}

/* ── Back layer — same dimensions + border-radius, 20px above section ── */
.certified-v2__back {
  position: absolute;
  top: 40px;
  left: 0;
  width: 100%;
  height: 577px;
  border-radius: 150px 0 150px 0;
  background: #1A75BB;
  opacity: 0.45;
  z-index: 0;
}

/* ── Section shell ── */
.certified-v2 {
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
.certified-v2__bg {
  position: absolute;
  inset: 0;
  overflow: hidden;
}

.certified-v2__photo {
  position: absolute;
  left: 0;
  max-width: none;
  object-fit: cover;
}

.certified-v2__photo--1 {
  width: 100%;
  height: 194.24%;
  top: -47.12%;
}

.certified-v2__photo--2 {
  width: 118.73%;
  height: 230.63%;
  top: -71.76%;
  left: -0.56%;
}

/* ── Navy gradient overlay — matches DualCta / WhyChooseUs hero veil ──── */
.certified-v2::after {
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
.certified-v2__glow {
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

/* ── Brand magnifier decoration — floats with subtle sway ──────────────── */
.certified-v2__magnifier {
  position: absolute;
  bottom: 40px;
  left: 56px;
  width: 88px;
  height: 88px;
  z-index: 1;
  pointer-events: none;
  filter: drop-shadow(0 8px 16px rgba(0, 0, 0, 0.30));
  animation: cert-magnifier-float 6.5s ease-in-out infinite;
  will-change: transform;
}

@keyframes cert-magnifier-float {
  0%, 100% { transform: translate(0, 0) rotate(-6deg); }
  25%      { transform: translate(6px, -8px) rotate(4deg); }
  50%      { transform: translate(0, -14px) rotate(8deg); }
  75%      { transform: translate(-6px, -8px) rotate(-4deg); }
}

@media (prefers-reduced-motion: reduce) {
  .certified-v2__magnifier { animation: none; }
}

/* ── Content — asymmetric split (heading left, body+CTA right) ─────────── */
.certified-v2__content {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  height: 100%;
  padding: 0 80px;
  gap: 94px;
}

.certified-v2__left {
  flex: 0 0 480px;
  text-align: right;
}

.certified-v2__eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 14px;
}

.certified-v2__title {
  font-family: var(--font-family);
  font-size: 56px;
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0;
}

.certified-v2__divider {
  width: 88px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  margin: 24px 0 0 auto;
}

.certified-v2__right {
  flex: 1;
}

.certified-v2__body {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.92);
  margin: 0 0 28px;
  max-width: 560px;
}

/* ── Glass pill CTA — matches Hero / DualCta / WhyChooseUs language ────── */
.certified-v2__cta {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  padding: 14px 32px;
  border: 1.5px solid rgba(255, 255, 255, 0.85);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.10);
  backdrop-filter: blur(12px) saturate(150%);
  -webkit-backdrop-filter: blur(12px) saturate(150%);
  color: #fff;
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 600;
  letter-spacing: 0.04em;
  text-decoration: none;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.22);
  transition: background 0.3s ease, color 0.3s ease, transform 0.3s ease;
}

.certified-v2__cta:hover {
  background: #fff;
  color: var(--c-brand-navy);
  transform: translateY(-2px);
}

/* ── Mobile ── */
@media (max-width: 1023px) {
  .certified-v2-wrap {
    margin-top: -160px; /* overlaps the 160px blue extension of WhyChooseUs — photo sits on top of the blue */
    padding-top: 0;
  }

  .certified-v2__back {
    display: none;
  }

  .certified-v2 {
    height: auto;
    min-height: 400px;
    border-radius: 80px 0 80px 0;
    padding-bottom: 80px;
  }

  .certified-v2__glow {
    width: 320px;
    height: 320px;
    right: -100px;
    top: -60px;
    filter: blur(110px);
    opacity: 0.22;
  }

  .certified-v2__magnifier {
    width: 56px;
    height: 56px;
    bottom: 20px;
    left: 20px;
  }

  .certified-v2__content {
    flex-direction: column;
    align-items: flex-start;
    padding: 80px 24px 0;
    gap: 24px;
    height: auto;
  }

  .certified-v2__left {
    flex: none;
    width: 100%;
    text-align: left;
  }

  .certified-v2__eyebrow {
    margin-bottom: 10px;
  }

  .certified-v2__title {
    font-size: 32px;
  }

  .certified-v2__divider {
    margin: 16px 0 0 0;          /* left-aligned on mobile to match text */
    width: 64px;
  }

  .certified-v2__right {
    width: 100%;
  }

  .certified-v2__body {
    font-size: 15px;
    margin-bottom: 24px;
  }

  .certified-v2__cta {
    padding: 12px 28px;
    font-size: 14px;
  }
}
</style>
