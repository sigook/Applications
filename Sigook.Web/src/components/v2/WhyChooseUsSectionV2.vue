<template>
  <section
    ref="sectionRef"
    class="why-v2"
    :class="{ 'is-visible': visible }"
  >
    <!-- Back layer — same shape as the photo, sits 20px above so it peeks out at the top -->
    <div class="why-v2__hero-back" aria-hidden="true"></div>

    <!-- Block A — Corporate buildings photo -->
    <div class="why-v2__hero">
      <img src="@/assets/images/v2/why-choose-us/why-bg.jpg" alt="" class="why-v2__hero-bg" aria-hidden="true" />
      <div class="why-v2__hero-overlay" aria-hidden="true"></div>

      <!-- Decorative cyan glow + brand magnifier -->
      <div class="why-v2__hero-glow" aria-hidden="true"></div>
      <img
        src="@/assets/images/v2/branding/sigook-magnifier.png"
        alt=""
        aria-hidden="true"
        class="why-v2__hero-magnifier"
      />

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

        <!-- Decorative floating elements (matches Numbers vocabulary) -->
        <span class="why-v2__deco why-v2__deco--cyan-glow" aria-hidden="true"></span>
        <span class="why-v2__deco why-v2__deco--navy-glow" aria-hidden="true"></span>
        <span class="why-v2__deco why-v2__deco--red-glow" aria-hidden="true"></span>
        <span class="why-v2__deco why-v2__deco--blue-glow" aria-hidden="true"></span>
        <span class="why-v2__deco why-v2__deco--red-glow-sm" aria-hidden="true"></span>
        <span class="why-v2__deco why-v2__deco--ring-lg" aria-hidden="true"></span>
        <span class="why-v2__deco why-v2__deco--ring-sm" aria-hidden="true"></span>
        <span class="why-v2__deco why-v2__deco--dot-a" aria-hidden="true"></span>
        <span class="why-v2__deco why-v2__deco--dot-b" aria-hidden="true"></span>

        <!-- Top zone: asymmetric editorial — heading left, map right -->
        <div class="why-v2__top">
          <header class="why-v2__panel-header">
            <span class="why-v2__panel-eyebrow">What Sets Us Apart</span>
            <h3 class="why-v2__panel-heading">Nationwide reach,<br />local focus</h3>
            <p class="why-v2__panel-sub">
              A network that scales coast-to-coast while staying personally
              invested in every local market we serve.
            </p>
            <div class="why-v2__panel-divider" aria-hidden="true"></div>
          </header>

          <div class="why-v2__map-wrap">
            <span class="why-v2__map-halo" aria-hidden="true"></span>
            <img
              src="@/assets/images/v2/why-choose-us/usa-map.png"
              alt="USA/Canada presence map"
              class="why-v2__map-img"
            />
          </div>
        </div>

        <!-- Feature cards — glass, asymmetric brand radii -->
        <div class="why-v2__features">
          <article class="why-v2__feature why-v2__feature--blue">
            <span class="why-v2__feature-eyebrow">Network</span>
            <h4 class="why-v2__feature-title">Coast-to-coast reach</h4>
            <p class="why-v2__feature-body">
              A nationwide recruitment network with reliable coverage across every
              major North American market.
            </p>
          </article>

          <article class="why-v2__feature why-v2__feature--cyan">
            <span class="why-v2__feature-eyebrow">Local Focus</span>
            <h4 class="why-v2__feature-title">Tailored solutions</h4>
            <p class="why-v2__feature-body">
              National scale paired with the dedication of local expertise — every
              client gets a fit, never a template.
            </p>
          </article>

          <article class="why-v2__feature why-v2__feature--red">
            <span class="why-v2__feature-eyebrow">Leadership</span>
            <h4 class="why-v2__feature-title">Trusted innovators</h4>
            <p class="why-v2__feature-body">
              Industry expertise meets modern strategies — meaningful connections
              that drive success on both sides of the table.
            </p>
          </article>
        </div>

      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const sectionRef = ref<HTMLElement | null>(null)
const visible = ref(false)
let observer: IntersectionObserver | null = null

onMounted(() => {
  if (!sectionRef.value) return
  // Continuous observer — fade in entering viewport, fade out leaving (matches Numbers)
  observer = new IntersectionObserver(
    (entries) => {
      visible.value = entries[0].isIntersecting
    },
    { threshold: 0.15, rootMargin: '0px 0px -10% 0px' }
  )
  observer.observe(sectionRef.value)
})

onUnmounted(() => observer?.disconnect())
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

/* Brand magnifier decoration — floats with subtle sway */
.why-v2__hero-magnifier {
  position: absolute;
  bottom: 48px;
  left: 56px;
  width: 88px;
  height: 88px;
  z-index: 1;
  pointer-events: none;
  filter: drop-shadow(0 8px 16px rgba(0, 0, 0, 0.30));
  animation: why-magnifier-float 6.5s ease-in-out infinite;
  will-change: transform;
}

@keyframes why-magnifier-float {
  0%, 100% { transform: translate(0, 0) rotate(-6deg); }
  25%      { transform: translate(6px, -8px) rotate(4deg); }
  50%      { transform: translate(0, -14px) rotate(8deg); }
  75%      { transform: translate(-6px, -8px) rotate(-4deg); }
}

@media (prefers-reduced-motion: reduce) {
  .why-v2__hero-magnifier { animation: none; }
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
  /* Same gradient as Numbers section — flows navy → brand-blue from top to bottom */
  background:
    linear-gradient(180deg,
      #0f2f44 0%,
      #093055 22%,
      #0d4063 48%,
      #135b8c 76%,
      #1575bb 100%
    );
  border-radius: 150px 0 150px 0;
  margin-top: -196px;
  z-index: 1;
  padding-bottom: 160px;
  overflow: hidden;
  /* Sized so feature cards clear Certified's -556px overlap with a small ~100px buffer */
  min-height: 1720px;
}

.why-v2__panel-inner {
  position: relative;
  max-width: 1280px;
  margin: 0 auto;
  padding: 280px 80px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  z-index: 1;
}

/* ── Floating decorative elements (drift continuously like Numbers) ────── */
.why-v2__deco {
  position: absolute;
  pointer-events: none;
  border-radius: 50%;
  z-index: 0;
  will-change: transform;
}

.why-v2__deco--cyan-glow {
  width: 640px;
  height: 640px;
  top: 180px;
  right: -180px;
  background: var(--c-brand-cyan);
  filter: blur(180px);
  opacity: 0.32;
  animation: why-drift-a 62s ease-in-out infinite;
}

.why-v2__deco--navy-glow {
  width: 520px;
  height: 520px;
  top: 60%;
  left: -160px;
  background: #062a44;
  filter: blur(160px);
  opacity: 0.45;
  animation: why-drift-b 70s ease-in-out infinite;
  animation-delay: -8s;
}

/* Big red blur — visible warmth, mid-left area */
.why-v2__deco--red-glow {
  width: 520px;
  height: 520px;
  top: 40%;
  left: 24%;
  background: var(--c-brand-red);
  filter: blur(170px);
  opacity: 0.32;
  animation: why-drift-c 56s ease-in-out infinite;
  animation-delay: -14s;
}

/* Big blue blur — visible cool tone, lower-right */
.why-v2__deco--blue-glow {
  width: 580px;
  height: 580px;
  top: 50%;
  right: 8%;
  background: var(--c-brand-blue);
  filter: blur(180px);
  opacity: 0.40;
  animation: why-drift-d 66s ease-in-out infinite;
  animation-delay: -22s;
}

/* Medium red blur — bottom, near feature cards */
.why-v2__deco--red-glow-sm {
  width: 380px;
  height: 380px;
  bottom: 280px;
  left: 20%;
  background: var(--c-brand-red);
  filter: blur(140px);
  opacity: 0.30;
  animation: why-drift-e 50s ease-in-out infinite;
  animation-delay: -6s;
}

.why-v2__deco--ring-lg {
  width: 220px;
  height: 220px;
  top: 22%;
  left: -50px;
  background: transparent;
  border: 3px solid rgba(255, 255, 255, 0.22);
  animation: why-drift-f 54s ease-in-out infinite;
}

.why-v2__deco--ring-sm {
  width: 130px;
  height: 130px;
  bottom: 280px;
  right: 60px;
  background: transparent;
  border: 3px solid rgba(0, 173, 239, 0.55);
  animation: why-drift-g 46s ease-in-out infinite;
  animation-delay: -12s;
}

.why-v2__deco--dot-a {
  width: 22px;
  height: 22px;
  top: 32%;
  right: 8%;
  background: var(--c-brand-cyan);
  opacity: 0.85;
  animation: why-drift-h 38s ease-in-out infinite;
}

.why-v2__deco--dot-b {
  width: 14px;
  height: 14px;
  bottom: 18%;
  left: 18%;
  background: var(--c-brand-red);
  opacity: 0.85;
  animation: why-drift-a 42s ease-in-out infinite;
  animation-delay: -18s;
}

/* ── Drift keyframes — slow ambient motion (same vocabulary as Numbers) ── */
@keyframes why-drift-a {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(140px, -110px); }
}

@keyframes why-drift-b {
  0%, 100% { transform: translate(0, 0); }
  33%      { transform: translate(-90px, 80px); }
  66%      { transform: translate(80px, -120px); }
}

@keyframes why-drift-c {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50%      { transform: translate(-110px, 90px) scale(1.10); }
}

@keyframes why-drift-d {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50%      { transform: translate(70px, -80px) scale(0.90); }
}

@keyframes why-drift-e {
  0%, 100% { transform: translate(0, 0); }
  25%      { transform: translate(80px, -60px); }
  50%      { transform: translate(50px, 100px); }
  75%      { transform: translate(-80px, 40px); }
}

@keyframes why-drift-f {
  0%, 100% { transform: translate(0, 0) scale(1); }
  20%      { transform: translate(90px, -40px) scale(1.08); }
  40%      { transform: translate(-70px, -80px) scale(0.92); }
  60%      { transform: translate(-80px, 70px) scale(1.06); }
  80%      { transform: translate(100px, 60px) scale(0.94); }
}

@keyframes why-drift-g {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(-130px, 150px); }
}

@keyframes why-drift-h {
  0%, 100% { transform: translate(0, 0) scale(1); }
  33%      { transform: translate(120px, 70px) scale(1.05); }
  66%      { transform: translate(-90px, 110px) scale(0.95); }
}

@media (prefers-reduced-motion: reduce) {
  .why-v2__deco {
    animation: none !important;
  }
}

/* ── Top zone — asymmetric editorial split (heading left, map right) ─── */
.why-v2__top {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  gap: 80px;
  width: 100%;
  margin-bottom: 96px;
}

/* Panel header — text block on the left */
.why-v2__panel-header {
  flex: 1;
  position: relative;
  z-index: 2;
  text-align: left;
  max-width: 520px;
  margin: 0;
}

.why-v2__panel-eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 16px;
}

.why-v2__panel-heading {
  font-family: var(--font-family);
  font-size: 52px;
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0 0 20px;
}

.why-v2__panel-sub {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 28px;
  max-width: 460px;
}

/* Cyan divider under sub — closes the heading block, matches Hero divider language */
.why-v2__panel-divider {
  width: 88px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
}

/* ── USA Map — right side of the split ───────────────────────────────── */
.why-v2__map-wrap {
  flex: 1;
  position: relative;
  z-index: 2;
  display: flex;
  justify-content: center;
  align-items: center;
  margin: 0;
  /* Fade in/out with viewport (matches Numbers behavior) */
  opacity: 0;
  transform: translateY(30px) scale(0.96);
  transition:
    opacity 0.8s ease-out,
    transform 0.8s cubic-bezier(0.22, 1, 0.36, 1);
}

.why-v2.is-visible .why-v2__map-wrap {
  opacity: 1;
  transform: translateY(0) scale(1);
  transition-delay: 0.10s;
}

.why-v2__map-halo {
  position: absolute;
  width: 100%;
  height: 100%;
  background: radial-gradient(ellipse at center,
    rgba(0, 173, 239, 0.45) 0%,
    rgba(0, 173, 239, 0.12) 40%,
    transparent 70%);
  filter: blur(60px);
  pointer-events: none;
  z-index: -1;
}

.why-v2__map-img {
  position: relative;
  width: 100%;
  max-width: 600px;
  height: auto;
  display: block;
  filter: drop-shadow(0 24px 48px rgba(0, 0, 0, 0.30));
}

/* ── Feature cards row (matches Numbers + DualCta glass card pattern) ── */
.why-v2__features {
  position: relative;
  z-index: 2;
  display: flex;
  gap: 28px;
  justify-content: center;
  align-items: stretch;
  flex-wrap: wrap;
  width: 100%;
}

.why-v2__feature {
  flex: 1 1 280px;
  max-width: 360px;
  padding: 40px 30px 36px;
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.20);
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.22);
  /* Fade in/out with viewport — same as Numbers stat cards */
  opacity: 0;
  transform: translateY(40px) scale(0.92);
  transition:
    opacity 0.7s ease-out,
    transform 0.7s cubic-bezier(0.22, 1, 0.36, 1);
}

.why-v2.is-visible .why-v2__feature {
  opacity: 1;
  transform: translateY(0) scale(1);
}

/* Staggered ENTRY delays — only apply when visible; on exit, no delay → cards fade out together */
.why-v2.is-visible .why-v2__feature--blue { transition-delay: 0.30s; }
.why-v2.is-visible .why-v2__feature--cyan { transition-delay: 0.44s; }
.why-v2.is-visible .why-v2__feature--red  { transition-delay: 0.58s; }

/* Shared radial corner glow — highlights the larger asymmetric corner (matches Numbers) */
.why-v2__feature::before {
  content: '';
  position: absolute;
  pointer-events: none;
  width: 160px;
  height: 160px;
}

/* Card 1 — Blue dominant gradient with red accent on bottom-right (mirrors Numbers --blue) */
.why-v2__feature--blue {
  position: relative;
  background: linear-gradient(135deg,
    rgba(21, 117, 187, 0.42) 0%,
    rgba(21, 117, 187, 0.14) 50%,
    rgba(229, 45, 39, 0.32) 100%
  );
  border-radius: 20px 56px 20px 56px;
}

.why-v2__feature--blue::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.18) 35%,
    transparent 70%);
  border-radius: 0 56px 0 0;
}

/* Card 2 — Balanced cyan → blue → red triple gradient (mirrors Numbers --cyan) */
.why-v2__feature--cyan {
  position: relative;
  background: linear-gradient(135deg,
    rgba(0, 173, 239, 0.34) 0%,
    rgba(21, 117, 187, 0.22) 50%,
    rgba(229, 45, 39, 0.30) 100%
  );
  border-radius: 56px 20px 56px 20px;
}

.why-v2__feature--cyan::before {
  top: 0;
  left: 0;
  background: radial-gradient(circle at top left,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.18) 35%,
    transparent 70%);
  border-radius: 56px 0 0 0;
}

/* Card 3 — Red dominant gradient with blue accent on bottom-right (mirrors Numbers --red) */
.why-v2__feature--red {
  position: relative;
  background: linear-gradient(135deg,
    rgba(229, 45, 39, 0.42) 0%,
    rgba(229, 45, 39, 0.14) 50%,
    rgba(21, 117, 187, 0.32) 100%
  );
  border-radius: 20px 56px 20px 56px;
}

.why-v2__feature--red::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(21, 117, 187, 0.60) 0%,
    rgba(21, 117, 187, 0.20) 35%,
    transparent 70%);
  border-radius: 0 56px 0 0;
}

.why-v2__feature-eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 14px;
}

.why-v2__feature--red .why-v2__feature-eyebrow {
  color: #fff;
  opacity: 0.85;
}

.why-v2__feature-title {
  font-family: var(--font-family);
  font-size: 22px;
  font-weight: 600;
  line-height: 1.2;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0 0 14px;
}

.why-v2__feature-body {
  font-family: var(--font-family);
  font-size: 14px;
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.86);
  margin: 0;
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

  .why-v2__hero-magnifier {
    width: 56px;
    height: 56px;
    bottom: 20px;
    left: 20px;
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
    padding: 140px 24px 0;
  }

  /* Mobile deco scale-down — keep glows, hide some rings/dots */
  .why-v2__deco--cyan-glow {
    width: 360px;
    height: 360px;
    right: -100px;
    top: 120px;
    filter: blur(120px);
    opacity: 0.28;
  }

  .why-v2__deco--navy-glow {
    width: 320px;
    height: 320px;
    left: -100px;
    filter: blur(110px);
  }

  .why-v2__deco--red-glow {
    width: 320px;
    height: 320px;
    top: 36%;
    left: -60px;
    filter: blur(130px);
    opacity: 0.28;
  }

  .why-v2__deco--blue-glow {
    width: 360px;
    height: 360px;
    top: 52%;
    right: -100px;
    filter: blur(140px);
    opacity: 0.35;
  }

  .why-v2__deco--red-glow-sm {
    width: 260px;
    height: 260px;
    bottom: 220px;
    left: 8%;
    filter: blur(110px);
    opacity: 0.26;
  }

  .why-v2__deco--ring-lg {
    width: 120px;
    height: 120px;
    top: 14%;
    left: -40px;
  }

  .why-v2__deco--ring-sm,
  .why-v2__deco--dot-a {
    display: none;
  }

  .why-v2__deco--dot-b {
    width: 10px;
    height: 10px;
    bottom: 10%;
    left: 10%;
  }

  /* Top zone — stack vertically on mobile */
  .why-v2__top {
    flex-direction: column;
    gap: 40px;
    margin-bottom: 56px;
  }

  .why-v2__panel-header {
    text-align: center;
    max-width: 100%;
  }

  .why-v2__panel-heading {
    font-size: 32px;
  }

  .why-v2__panel-sub {
    font-size: 15px;
    margin-left: auto;
    margin-right: auto;
  }

  .why-v2__panel-divider {
    margin: 0 auto;
  }

  /* Map */
  .why-v2__map-wrap {
    width: 100%;
  }

  .why-v2__map-halo {
    width: 110%;
    height: 80%;
  }

  /* Feature cards stack on mobile */
  .why-v2__features {
    flex-direction: column;
    gap: 18px;
    align-items: center;
  }

  .why-v2__feature {
    width: 100%;
    max-width: 360px;
    padding: 32px 26px 28px;
    backdrop-filter: blur(14px) saturate(150%);
    -webkit-backdrop-filter: blur(14px) saturate(150%);
  }

  .why-v2__feature-title {
    font-size: 20px;
  }
}
</style>
