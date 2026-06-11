<template>
  <div class="global-bg" aria-hidden="true">
    <!-- Soft blurred glows (atmospheric depth) -->
    <span class="global-bg__deco glow-cyan-lg"></span>
    <span class="global-bg__deco glow-navy-lg"></span>
    <span class="global-bg__deco glow-red-md"></span>
    <span class="global-bg__deco glow-blue-md"></span>
    <span class="global-bg__deco glow-red-sm"></span>
    <span class="global-bg__deco glow-cyan-md"></span>

    <!-- Outlined rings (geometric structure) -->
    <span class="global-bg__deco ring-lg-white"></span>
    <span class="global-bg__deco ring-md-cyan"></span>

    <!-- Solid dots (sharp accents) -->
    <span class="global-bg__deco dot-cyan-a"></span>
    <span class="global-bg__deco dot-red-a"></span>
    <span class="global-bg__deco dot-cyan-b"></span>
    <span class="global-bg__deco dot-red-b"></span>
  </div>
</template>

<script setup lang="ts">
/**
 * Page-wide animated background — fixed to viewport, sits behind everything.
 *
 * Vocabulary mirrors WhyChooseUs panel: navy → brand-blue gradient + radial
 * cyan glow + the brand palette of soft glows (cyan/navy/red/blue), outlined
 * rings (white/cyan), and solid dots (cyan/red). All elements drift
 * independently.
 *
 * Renders inside App.vue's v2 layout branch so every v2 route inherits the
 * same backdrop. Sections layer on top with their own photos / content /
 * transparent containers.
 */
</script>

<style scoped>
.global-bg {
  position: fixed;
  inset: 0;
  z-index: 0;             /* below all section content */
  overflow: hidden;
  pointer-events: none;
  background:
    radial-gradient(circle at 80% 20%, rgba(0, 173, 239, 0.15) 0%, transparent 45%),
    linear-gradient(
      180deg,
      #0f2f44 0%,
      #093055 22%,
      #0d4063 48%,
      #135b8c 76%,
      #1575bb 100%
    );
}

.global-bg__deco {
  position: absolute;
  border-radius: 50%;
  will-change: transform;
}

/* ── Soft blurred glows ───────────────────────────────────────────────────── */
/* All blurred glows use white per design decision — they read as soft
   atmospheric halos against the navy gradient backdrop rather than
   colored accents. Opacities are tuned down where the previous color
   already had high saturation (e.g. navy was 0.55, but white at that
   level reads as harsh light; lowered to 0.18). */
.glow-cyan-lg {
  width: 640px; height: 640px;
  top: -100px; right: -100px;
  background: #fff;
  filter: blur(180px);
  opacity: 0.18;
  animation: drift-a 62s ease-in-out infinite;
}

.glow-navy-lg {
  width: 520px; height: 520px;
  bottom: -120px; left: -120px;
  background: #fff;
  filter: blur(160px);
  opacity: 0.14;
  animation: drift-b 70s ease-in-out infinite;
  animation-delay: -8s;
}

.glow-red-md {
  width: 480px; height: 480px;
  top: 30%; left: 18%;
  background: #fff;
  filter: blur(170px);
  opacity: 0.16;
  animation: drift-c 56s ease-in-out infinite;
  animation-delay: -14s;
}

.glow-blue-md {
  width: 540px; height: 540px;
  top: 50%; right: 12%;
  background: #fff;
  filter: blur(180px);
  opacity: 0.18;
  animation: drift-d 66s ease-in-out infinite;
  animation-delay: -22s;
}

.glow-red-sm {
  width: 320px; height: 320px;
  bottom: 14%; left: 32%;
  background: #fff;
  filter: blur(140px);
  opacity: 0.15;
  animation: drift-e 50s ease-in-out infinite;
  animation-delay: -6s;
}

.glow-cyan-md {
  width: 380px; height: 380px;
  bottom: 10%; right: -50px;
  background: #fff;
  filter: blur(150px);
  opacity: 0.14;
  animation: drift-f 60s ease-in-out infinite;
  animation-delay: -32s;
}

/* ── Outlined rings ───────────────────────────────────────────────────────── */
.ring-lg-white {
  width: 220px; height: 220px;
  top: 12%; left: 8%;
  background: transparent;
  border: 3px solid rgba(255, 255, 255, 0.22);
  animation: drift-g 54s ease-in-out infinite;
}

.ring-md-cyan {
  width: 130px; height: 130px;
  bottom: 22%; right: 8%;
  background: transparent;
  border: 3px solid rgba(0, 173, 239, 0.55);
  animation: drift-h 46s ease-in-out infinite;
  animation-delay: -12s;
}

/* ── Solid dots ───────────────────────────────────────────────────────────── */
.dot-cyan-a {
  width: 22px; height: 22px;
  top: 28%; right: 8%;
  background: var(--c-brand-cyan);
  opacity: 0.85;
  animation: drift-c 38s ease-in-out infinite;
}

.dot-red-a {
  width: 14px; height: 14px;
  bottom: 24%; left: 18%;
  background: var(--c-brand-red);
  opacity: 0.85;
  animation: drift-a 42s ease-in-out infinite;
  animation-delay: -18s;
}

.dot-cyan-b {
  width: 12px; height: 12px;
  top: 62%; left: 6%;
  background: var(--c-brand-cyan);
  opacity: 0.70;
  animation: drift-e 44s ease-in-out infinite;
  animation-delay: -10s;
}

.dot-red-b {
  width: 18px; height: 18px;
  top: 18%; left: 48%;
  background: var(--c-brand-red);
  opacity: 0.75;
  animation: drift-b 36s ease-in-out infinite;
  animation-delay: -22s;
}

/* ── Drift keyframes (same vocabulary as section-level drifts) ────────────── */
@keyframes drift-a {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(140px, -110px); }
}

@keyframes drift-b {
  0%, 100% { transform: translate(0, 0); }
  33%      { transform: translate(-90px, 80px); }
  66%      { transform: translate(80px, -120px); }
}

@keyframes drift-c {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50%      { transform: translate(-110px, 90px) scale(1.10); }
}

@keyframes drift-d {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50%      { transform: translate(70px, -80px) scale(0.90); }
}

@keyframes drift-e {
  0%, 100% { transform: translate(0, 0); }
  25%      { transform: translate(80px, -60px); }
  50%      { transform: translate(50px, 100px); }
  75%      { transform: translate(-80px, 40px); }
}

@keyframes drift-f {
  0%, 100% { transform: translate(0, 0) scale(1); }
  20%      { transform: translate(90px, -40px) scale(1.08); }
  40%      { transform: translate(-70px, -80px) scale(0.92); }
  60%      { transform: translate(-80px, 70px) scale(1.06); }
  80%      { transform: translate(100px, 60px) scale(0.94); }
}

@keyframes drift-g {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(-130px, 150px); }
}

@keyframes drift-h {
  0%, 100% { transform: translate(0, 0) scale(1); }
  33%      { transform: translate(120px, 70px) scale(1.05); }
  66%      { transform: translate(-90px, 110px) scale(0.95); }
}

@media (prefers-reduced-motion: reduce) {
  .global-bg__deco { animation: none !important; }
}

/* ── Mobile — scaled-down shapes ──────────────────────────────────────────── */
@media (max-width: 1023px) {
  .glow-cyan-lg { width: 360px; height: 360px; filter: blur(120px); right: -80px; top: -60px; }
  .glow-navy-lg { width: 320px; height: 320px; filter: blur(110px); }
  .glow-red-md  { width: 260px; height: 260px; filter: blur(120px); left: 5%; }
  .glow-blue-md { width: 320px; height: 320px; filter: blur(130px); right: -40px; }
  .glow-red-sm  { width: 200px; height: 200px; filter: blur(100px); left: 20%; }
  .glow-cyan-md { width: 240px; height: 240px; filter: blur(110px); right: -60px; }
  .ring-lg-white { width: 140px; height: 140px; }
  .ring-md-cyan  { width: 80px;  height: 80px;  }
}
</style>
