<template>
  <section
    ref="sectionRef"
    class="numbers-v2"
    :class="{ 'is-visible': visible }"
  >
    <!-- Floating decorative geometry — drift continuously, varied sizes/blur -->
    <!-- Soft blurred glows (atmospheric) -->
    <span class="numbers-v2__deco numbers-v2__deco--cyan-lg" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--navy-md" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--cyan-md" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--red-sm" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--navy-sm" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--navy-huge" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--red-md" aria-hidden="true"></span>
    <!-- Outlined rings (sharp, geometric structure) -->
    <span class="numbers-v2__deco numbers-v2__deco--cyan-ring" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--white-ring" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--cyan-ring-xl" aria-hidden="true"></span>
    <!-- Solid dots (sharp accents, varied sizes 4px → 44px) -->
    <span class="numbers-v2__deco numbers-v2__deco--cyan-dot-tiny" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--cyan-dot-a" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--cyan-dot-b" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--red-dot" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--red-dot-big" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--white-dot" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--navy-dot" aria-hidden="true"></span>

    <!-- Mid-sized circles (88-132px, 2-3× the biggest filled dot) — mix of rings + fills -->
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--cyan-ring-a" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--cyan-fill-a" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--red-ring-a" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--red-fill-a" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--white-ring-a" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--navy-ring-a" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--cyan-fill-b" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--cyan-ring-b" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--red-fill-b" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--red-ring-b" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--white-ring-b" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--navy-fill-a" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--cyan-ring-c" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--cyan-fill-c" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--red-ring-c" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--white-ring-c" aria-hidden="true"></span>
    <span class="numbers-v2__deco numbers-v2__deco--mid mid--navy-ring-b" aria-hidden="true"></span>

    <!-- Centered content -->
    <div class="numbers-v2__canvas">
      <header class="numbers-v2__header">
        <span class="numbers-v2__eyebrow">By the Numbers</span>
        <h2 class="numbers-v2__heading">Our impact, at scale</h2>
        <p class="numbers-v2__sub">
          Real results from real partnerships across North America.
        </p>
      </header>

      <div class="numbers-v2__stats">
        <article class="numbers-v2__stat numbers-v2__stat--blue">
          <span class="numbers-v2__num">+330</span>
          <span class="numbers-v2__lbl">Clients Served</span>
        </article>

        <article class="numbers-v2__stat numbers-v2__stat--cyan">
          <span class="numbers-v2__num">+1,700</span>
          <span class="numbers-v2__lbl">Jobs Posted</span>
        </article>

        <article class="numbers-v2__stat numbers-v2__stat--red">
          <span class="numbers-v2__num">+5,000</span>
          <span class="numbers-v2__lbl">Applications</span>
        </article>
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
  // Continuous observer — fade in when entering viewport, fade out when leaving
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
/* ── Section shell — gradient navy → brand-blue ──────────────────────────── */
.numbers-v2 {
  position: relative;
  margin-top: -177px;
  width: 100%;
  height: 1351px;
  overflow: hidden;
  isolation: isolate;
  background:
    linear-gradient(180deg,
      #0f2f44 0%,
      #093055 22%,
      #0d4063 48%,
      #135b8c 76%,
      #1575bb 100%
    );
}

/* ── Floating decorative circles ─────────────────────────────────────────── */
.numbers-v2__deco {
  position: absolute;
  z-index: 1;
  pointer-events: none;
  border-radius: 50%;
  opacity: 0;
  transition: opacity 1.2s ease;
  will-change: transform;
}

.numbers-v2.is-visible .numbers-v2__deco--cyan-lg       { opacity: 0.28; }
.numbers-v2.is-visible .numbers-v2__deco--navy-md       { opacity: 0.55; }
.numbers-v2.is-visible .numbers-v2__deco--cyan-md       { opacity: 0.22; }
.numbers-v2.is-visible .numbers-v2__deco--red-sm        { opacity: 0.45; }
.numbers-v2.is-visible .numbers-v2__deco--navy-sm       { opacity: 0.40; }
.numbers-v2.is-visible .numbers-v2__deco--navy-huge     { opacity: 0.30; }
.numbers-v2.is-visible .numbers-v2__deco--red-md        { opacity: 0.32; }
.numbers-v2.is-visible .numbers-v2__deco--cyan-ring     { opacity: 0.55; }
.numbers-v2.is-visible .numbers-v2__deco--cyan-ring-xl  { opacity: 0.42; }
.numbers-v2.is-visible .numbers-v2__deco--white-ring    { opacity: 0.18; }
.numbers-v2.is-visible .numbers-v2__deco--cyan-dot-tiny { opacity: 0.95; }
.numbers-v2.is-visible .numbers-v2__deco--cyan-dot-a    { opacity: 0.85; }
.numbers-v2.is-visible .numbers-v2__deco--cyan-dot-b    { opacity: 0.70; }
.numbers-v2.is-visible .numbers-v2__deco--red-dot       { opacity: 0.90; }
.numbers-v2.is-visible .numbers-v2__deco--red-dot-big   { opacity: 0.80; }
.numbers-v2.is-visible .numbers-v2__deco--white-dot     { opacity: 0.55; }
.numbers-v2.is-visible .numbers-v2__deco--navy-dot      { opacity: 0.65; }

/* Mid circles — shared opacity rule, then per-variant tweaks below */
.numbers-v2.is-visible .numbers-v2__deco--mid          { opacity: 0.35; }
/* Cyan fills (blurred soft glows) */
.numbers-v2.is-visible .mid--cyan-fill-a,
.numbers-v2.is-visible .mid--cyan-fill-b,
.numbers-v2.is-visible .mid--cyan-fill-c,
.numbers-v2.is-visible .mid--cyan-ring-a               { opacity: 0.28; }
/* Cyan SHARP solid (no blur) — slightly brighter to read as accent */
.numbers-v2.is-visible .mid--cyan-ring-b               { opacity: 0.55; }
/* Red fills (blurred warm glows) */
.numbers-v2.is-visible .mid--red-fill-a,
.numbers-v2.is-visible .mid--red-fill-b,
.numbers-v2.is-visible .mid--red-ring-a,
.numbers-v2.is-visible .mid--red-ring-c                { opacity: 0.50; }
/* Navy fills (blurred dark voids) */
.numbers-v2.is-visible .mid--navy-fill-a,
.numbers-v2.is-visible .mid--navy-ring-a,
.numbers-v2.is-visible .mid--navy-ring-b               { opacity: 0.50; }
/* White solid blurred — softer than navy/red */
.numbers-v2.is-visible .mid--white-ring-c              { opacity: 0.35; }
/* True outlined white rings (still rings) */
.numbers-v2.is-visible .mid--white-ring-a,
.numbers-v2.is-visible .mid--white-ring-b              { opacity: 0.22; }

/* Large cyan glow — top-right, slow horizontal drift */
.numbers-v2__deco--cyan-lg {
  width: 560px;
  height: 560px;
  top: 8%;
  right: -120px;
  background: var(--c-brand-cyan);
  filter: blur(120px);
  animation: drift-a 48s ease-in-out infinite;
}

/* Medium navy — mid-left, multi-step drift */
.numbers-v2__deco--navy-md {
  width: 420px;
  height: 420px;
  top: 38%;
  left: -100px;
  background: #062a44;
  filter: blur(110px);
  animation: drift-b 56s ease-in-out infinite;
}

/* Medium cyan — bottom-right, diagonal drift */
.numbers-v2__deco--cyan-md {
  width: 320px;
  height: 320px;
  bottom: 14%;
  right: 12%;
  background: var(--c-brand-cyan);
  filter: blur(90px);
  animation: drift-c 42s ease-in-out infinite;
  animation-delay: -6s;
}

/* Small red — top accent, subtle motion (sharper, less blur) */
.numbers-v2__deco--red-sm {
  width: 160px;
  height: 160px;
  top: 22%;
  right: 18%;
  background: var(--c-brand-red);
  filter: blur(30px);
  animation: drift-d 36s ease-in-out infinite;
  animation-delay: -3s;
}

/* Small navy — bottom-center, drift A with offset */
.numbers-v2__deco--navy-sm {
  width: 240px;
  height: 240px;
  bottom: 22%;
  left: 26%;
  background: #062a44;
  filter: blur(80px);
  animation: drift-a 52s ease-in-out infinite;
  animation-delay: -12s;
}

/* Huge navy backdrop blur — bottom-right corner, atmospheric depth */
.numbers-v2__deco--navy-huge {
  width: 720px;
  height: 720px;
  bottom: -180px;
  right: -160px;
  background: #051f33;
  filter: blur(200px);
  animation: drift-e 70s ease-in-out infinite;
}

/* Medium red blur — left side, adds warmth */
.numbers-v2__deco--red-md {
  width: 340px;
  height: 340px;
  top: 58%;
  left: 18%;
  background: var(--c-brand-red);
  filter: blur(140px);
  animation: drift-h 64s ease-in-out infinite;
  animation-delay: -18s;
}

/* Extra-large ring — blue dominant fill mixing blue + red, blue border (520px) */
.numbers-v2__deco--cyan-ring-xl {
  width: 520px;
  height: 520px;
  top: 22%;
  left: 42%;
  background:
    linear-gradient(135deg,
      rgba(21, 117, 187, 0.30) 0%,
      rgba(21, 117, 187, 0.10) 50%,
      rgba(229, 45, 39, 0.28) 100%
    );
  border: 4px solid rgba(21, 117, 187, 0.78);
  animation: drift-h 72s ease-in-out infinite;
}

/* Large ring — red dominant fill mixing red + blue, red border (300px) */
.numbers-v2__deco--cyan-ring {
  width: 300px;
  height: 300px;
  top: 46%;
  right: 6%;
  background:
    linear-gradient(135deg,
      rgba(229, 45, 39, 0.32) 0%,
      rgba(229, 45, 39, 0.10) 50%,
      rgba(21, 117, 187, 0.30) 100%
    );
  border: 4px solid rgba(229, 45, 39, 0.78);
  animation: drift-f 60s ease-in-out infinite;
}

/* White outlined ring — smaller, top-left subtle */
.numbers-v2__deco--white-ring {
  width: 180px;
  height: 180px;
  top: 12%;
  left: 8%;
  background: transparent;
  border: 3px solid rgba(255, 255, 255, 0.6);
  animation: drift-g 54s ease-in-out infinite;
  animation-delay: -8s;
}

/* Sharp solid dots — varied sizes 4px → 44px, all drift continuously */
.numbers-v2__deco--cyan-dot-tiny {
  width: 4px;
  height: 4px;
  top: 36%;
  left: 48%;
  background: var(--c-brand-cyan);
  box-shadow: 0 0 10px var(--c-brand-cyan);
  animation: drift-h 28s ease-in-out infinite;
}

.numbers-v2__deco--cyan-dot-a {
  width: 24px;
  height: 24px;
  top: 32%;
  left: 18%;
  background: var(--c-brand-cyan);
  animation: drift-f 38s ease-in-out infinite;
}

.numbers-v2__deco--cyan-dot-b {
  width: 8px;
  height: 8px;
  bottom: 28%;
  right: 24%;
  background: var(--c-brand-cyan);
  animation: drift-g 44s ease-in-out infinite;
  animation-delay: -5s;
}

.numbers-v2__deco--red-dot {
  width: 14px;
  height: 14px;
  top: 18%;
  left: 38%;
  background: var(--c-brand-red);
  animation: drift-e 32s ease-in-out infinite;
}

.numbers-v2__deco--red-dot-big {
  width: 44px;
  height: 44px;
  bottom: 12%;
  left: 36%;
  background: var(--c-brand-red);
  animation: drift-d 40s ease-in-out infinite;
  animation-delay: -9s;
}

.numbers-v2__deco--white-dot {
  width: 18px;
  height: 18px;
  bottom: 18%;
  left: 12%;
  background: rgba(255, 255, 255, 0.85);
  animation: drift-f 50s ease-in-out infinite;
  animation-delay: -15s;
}

.numbers-v2__deco--navy-dot {
  width: 32px;
  height: 32px;
  top: 8%;
  left: 32%;
  background: #062a44;
  animation: drift-a 46s ease-in-out infinite;
  animation-delay: -7s;
}

/* ── Mid-sized circles (88–132px) — mix of rings + solid fills ───────────── */

/* TOP-LEFT zone — converted to solid */
.mid--cyan-ring-a {
  width: 110px; height: 110px;
  top: 6%; left: 14%;
  background: var(--c-brand-cyan);
  filter: blur(16px);
  animation: drift-a 58s ease-in-out infinite;
}

.mid--cyan-fill-a {
  width: 100px; height: 100px;
  top: 12%; right: 6%;
  background: var(--c-brand-cyan);
  filter: blur(22px);
  animation: drift-b 64s ease-in-out infinite;
  animation-delay: -4s;
}

.mid--red-ring-a {
  width: 115px; height: 115px;
  top: 30%; left: 4%;
  background: var(--c-brand-red);
  filter: blur(20px);
  animation: drift-c 52s ease-in-out infinite;
  animation-delay: -10s;
}

.mid--red-fill-a {
  width: 95px; height: 95px;
  top: 8%; right: 32%;
  background: var(--c-brand-red);
  filter: blur(18px);
  animation: drift-d 46s ease-in-out infinite;
}

.mid--white-ring-a {
  width: 92px; height: 92px;
  top: 24%; left: 64%;
  background: transparent;
  border: 3px solid rgba(255, 255, 255, 0.55);
  animation: drift-e 50s ease-in-out infinite;
  animation-delay: -7s;
}

.mid--navy-ring-a {
  width: 128px; height: 128px;
  top: 4%; left: 38%;
  background: #062a44;
  filter: blur(30px);
  animation: drift-f 62s ease-in-out infinite;
  animation-delay: -14s;
}

/* MID/EDGE zones */
.mid--cyan-fill-b {
  width: 120px; height: 120px;
  top: 42%; right: -30px;
  background: var(--c-brand-cyan);
  filter: blur(28px);
  animation: drift-g 56s ease-in-out infinite;
  animation-delay: -3s;
}

.mid--cyan-ring-b {
  width: 105px; height: 105px;
  bottom: 32%; right: 2%;
  background: var(--c-brand-cyan);
  /* No blur — sharp solid for geometric variety */
  animation: drift-h 48s ease-in-out infinite;
  animation-delay: -16s;
}

.mid--cyan-ring-c {
  width: 96px; height: 96px;
  top: 56%; left: -20px;
  background: transparent;
  border: 3px solid var(--c-brand-cyan);
  animation: drift-a 44s ease-in-out infinite;
  animation-delay: -22s;
}

.mid--cyan-fill-c {
  width: 108px; height: 108px;
  top: 38%; left: 4%;
  background: var(--c-brand-cyan);
  filter: blur(26px);
  animation: drift-b 60s ease-in-out infinite;
  animation-delay: -11s;
}

/* BOTTOM zone */
.mid--red-fill-b {
  width: 88px; height: 88px;
  bottom: 12%; left: 22%;
  background: var(--c-brand-red);
  filter: blur(16px);
  animation: drift-c 42s ease-in-out infinite;
  animation-delay: -19s;
}

.mid--red-ring-b {
  width: 130px; height: 130px;
  bottom: 6%; right: 18%;
  background: transparent;
  border: 3px solid var(--c-brand-red);
  animation: drift-d 66s ease-in-out infinite;
  animation-delay: -8s;
}

.mid--red-ring-c {
  width: 122px; height: 122px;
  top: 64%; right: 4%;
  background: var(--c-brand-red);
  filter: blur(22px);
  animation: drift-e 54s ease-in-out infinite;
  animation-delay: -25s;
}

.mid--white-ring-b {
  width: 100px; height: 100px;
  bottom: 22%; right: 36%;
  background: transparent;
  border: 3px solid rgba(255, 255, 255, 0.55);
  animation: drift-f 70s ease-in-out infinite;
  animation-delay: -12s;
}

.mid--white-ring-c {
  width: 100px; height: 100px;
  bottom: 38%; left: 16%;
  background: rgba(255, 255, 255, 0.85);
  filter: blur(18px);
  animation: drift-g 58s ease-in-out infinite;
  animation-delay: -28s;
}

.mid--navy-fill-a {
  width: 110px; height: 110px;
  bottom: 8%; left: 8%;
  background: #062a44;
  filter: blur(34px);
  animation: drift-h 64s ease-in-out infinite;
  animation-delay: -17s;
}

.mid--navy-ring-b {
  width: 130px; height: 130px;
  bottom: 4%; left: 42%;
  background: #062a44;
  filter: blur(28px);
  animation: drift-a 68s ease-in-out infinite;
  animation-delay: -33s;
}

/* ── Drift keyframes — bigger movement ranges so the motion is clearly visible ─ */
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

/* ── Canvas — centers content in visible middle zone ─────────────────────── */
.numbers-v2__canvas {
  position: relative;
  z-index: 3;
  max-width: 1280px;
  width: 100%;
  height: 100%;
  margin: 0 auto;
  padding: 240px 40px 320px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  text-align: center;
}

/* ── Header — fades with section ─────────────────────────────────────────── */
.numbers-v2__header {
  max-width: 720px;
  margin-bottom: 64px;
  opacity: 0;
  transform: translateY(20px);
  transition:
    opacity 0.7s ease-out,
    transform 0.7s cubic-bezier(0.22, 1, 0.36, 1);
}

.numbers-v2.is-visible .numbers-v2__header {
  opacity: 1;
  transform: translateY(0);
}

.numbers-v2__eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 16px;
}

.numbers-v2__heading {
  font-family: var(--font-family);
  font-size: 52px;
  font-weight: 700;
  line-height: 1.1;
  color: #fff;
  margin: 0 0 16px;
  letter-spacing: -0.015em;
}

.numbers-v2__sub {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
}

/* ── Stat cards row ──────────────────────────────────────────────────────── */
.numbers-v2__stats {
  display: flex;
  gap: 32px;
  justify-content: center;
  align-items: stretch;
  flex-wrap: wrap;
  width: 100%;
}

.numbers-v2__stat {
  position: relative;
  width: 280px;
  padding: 56px 32px 48px;
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.22);
  box-shadow: 0 18px 44px rgba(0, 0, 0, 0.30);
  text-align: center;

  opacity: 0;
  transform: translateY(40px) scale(0.92);
  transition:
    opacity 0.65s ease-out,
    transform 0.65s cubic-bezier(0.22, 1, 0.36, 1);
}

.numbers-v2.is-visible .numbers-v2__stat {
  opacity: 1;
  transform: translateY(0) scale(1);
}

/* Staggered ENTRY delays — only apply when entering (is-visible).
   On exit (class removed), no delay → cards fade out together cleanly. */
.numbers-v2.is-visible .numbers-v2__stat--blue { transition-delay: 0.22s; }
.numbers-v2.is-visible .numbers-v2__stat--cyan { transition-delay: 0.36s; }
.numbers-v2.is-visible .numbers-v2__stat--red  { transition-delay: 0.50s; }

/* ── Card color variants — each card plays with red + blue (+ cyan) ─────── */

/* Shared radial corner glow — highlights the larger asymmetric corner */
.numbers-v2__stat::before {
  content: '';
  position: absolute;
  pointer-events: none;
  width: 160px;
  height: 160px;
}

/* Card 1 — Blue dominant, red accent on bottom-right diagonal */
.numbers-v2__stat--blue {
  background:
    linear-gradient(135deg,
      rgba(21, 117, 187, 0.42) 0%,
      rgba(21, 117, 187, 0.14) 50%,
      rgba(229, 45, 39, 0.32) 100%
    );
  border-radius: 24px 64px 24px 64px;
}

/* Red corner glow follows the rounded top-right */
.numbers-v2__stat--blue::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.18) 35%,
    transparent 70%);
  border-radius: 0 64px 0 0;
}

/* Card 2 — Balanced cyan → blue → red triple gradient */
.numbers-v2__stat--cyan {
  background:
    linear-gradient(135deg,
      rgba(0, 173, 239, 0.34) 0%,
      rgba(21, 117, 187, 0.22) 50%,
      rgba(229, 45, 39, 0.30) 100%
    );
  border-radius: 64px 24px 64px 24px;
}

/* Cyan corner glow follows the rounded top-left (mirrored asymmetric) */
.numbers-v2__stat--cyan::before {
  top: 0;
  left: 0;
  background: radial-gradient(circle at top left,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.18) 35%,
    transparent 70%);
  border-radius: 64px 0 0 0;
}

/* Card 3 — Red dominant, blue accent on bottom-right diagonal */
.numbers-v2__stat--red {
  background:
    linear-gradient(135deg,
      rgba(229, 45, 39, 0.42) 0%,
      rgba(229, 45, 39, 0.14) 50%,
      rgba(21, 117, 187, 0.32) 100%
    );
  border-radius: 24px 64px 24px 64px;
}

/* Blue corner glow follows the rounded top-right */
.numbers-v2__stat--red::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(21, 117, 187, 0.60) 0%,
    rgba(21, 117, 187, 0.20) 35%,
    transparent 70%);
  border-radius: 0 64px 0 0;
}

/* Number + label */
.numbers-v2__num {
  display: block;
  font-family: var(--font-family);
  font-size: 64px;
  font-weight: 700;
  line-height: 1;
  color: #fff;
  margin-bottom: 12px;
  letter-spacing: -0.02em;
}

.numbers-v2__lbl {
  display: block;
  font-family: var(--font-family);
  font-size: 14px;
  font-weight: 600;
  line-height: 1.4;
  color: rgba(255, 255, 255, 0.88);
  letter-spacing: 0.10em;
  text-transform: uppercase;
}

/* ── Reduced motion ──────────────────────────────────────────────────────── */
@media (prefers-reduced-motion: reduce) {
  .numbers-v2__deco {
    animation: none !important;
  }
}

/* ── Mobile (≤ 1023px) ───────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .numbers-v2 {
    height: 1280px;
    margin-top: -100px;
  }

  .numbers-v2__canvas {
    padding: 160px 24px 160px;
  }

  .numbers-v2__header {
    margin-bottom: 40px;
  }

  .numbers-v2__heading {
    font-size: 32px;
  }

  .numbers-v2__sub {
    font-size: 15px;
  }

  /* Scale down deco circles for mobile, keep the floating animation */
  .numbers-v2__deco--cyan-lg {
    width: 360px;
    height: 360px;
    right: -100px;
    top: 6%;
    filter: blur(90px);
  }

  .numbers-v2__deco--navy-md {
    width: 280px;
    height: 280px;
    left: -80px;
    top: 32%;
    filter: blur(80px);
  }

  .numbers-v2__deco--cyan-md {
    width: 220px;
    height: 220px;
    right: -40px;
    bottom: 16%;
    filter: blur(70px);
  }

  .numbers-v2__deco--red-sm {
    width: 110px;
    height: 110px;
    top: 18%;
    right: 8%;
    filter: blur(24px);
  }

  .numbers-v2__deco--navy-sm {
    width: 180px;
    height: 180px;
    left: -50px;
    bottom: 12%;
    filter: blur(60px);
  }

  .numbers-v2__deco--navy-huge {
    width: 460px;
    height: 460px;
    bottom: -120px;
    right: -120px;
    filter: blur(140px);
  }

  .numbers-v2__deco--cyan-ring {
    width: 180px;
    height: 180px;
    top: 50%;
    right: -40px;
  }

  .numbers-v2__deco--white-ring {
    display: none;          /* too cluttered for small viewports */
  }

  .numbers-v2__deco--cyan-dot-a {
    width: 12px;
    height: 12px;
    top: 26%;
    left: 10%;
  }

  .numbers-v2__deco--cyan-dot-b {
    width: 10px;
    height: 10px;
    bottom: 24%;
    right: 12%;
  }

  .numbers-v2__deco--red-dot {
    width: 10px;
    height: 10px;
    top: 14%;
    left: 28%;
  }

  .numbers-v2__deco--red-dot-big {
    width: 28px;
    height: 28px;
    bottom: 10%;
    left: 28%;
  }

  .numbers-v2__deco--navy-dot {
    width: 20px;
    height: 20px;
    top: 6%;
    left: 22%;
  }

  .numbers-v2__deco--cyan-dot-tiny {
    width: 4px;
    height: 4px;
    top: 50%;
    left: 38%;
  }

  .numbers-v2__deco--red-md {
    width: 240px;
    height: 240px;
    top: 60%;
    left: -60px;
    filter: blur(100px);
  }

  .numbers-v2__deco--cyan-ring-xl {
    width: 320px;
    height: 320px;
    top: 35%;
    left: -60px;
  }

  .numbers-v2__deco--white-dot {
    display: none;
  }

  /* Mobile — hide most mid circles, keep a curated handful for visual rhythm */
  .numbers-v2__deco--mid { display: none; }

  .mid--cyan-ring-a,
  .mid--red-fill-a,
  .mid--cyan-fill-b,
  .mid--red-ring-b,
  .mid--cyan-ring-c,
  .mid--navy-fill-a {
    display: block;
  }

  /* Scale and reposition the few mobile-visible mid circles */
  .mid--cyan-ring-a {
    width: 80px; height: 80px;
    top: 6%; left: 6%;
    border-width: 2.5px;
  }
  .mid--red-fill-a {
    width: 70px; height: 70px;
    top: 8%; right: 10%;
    filter: blur(14px);
  }
  .mid--cyan-fill-b {
    width: 90px; height: 90px;
    top: 46%; right: -20px;
    filter: blur(22px);
  }
  .mid--red-ring-b {
    width: 88px; height: 88px;
    bottom: 8%; right: 6%;
    border-width: 2.5px;
  }
  .mid--cyan-ring-c {
    width: 70px; height: 70px;
    top: 60%; left: -15px;
    border-width: 2.5px;
  }
  .mid--navy-fill-a {
    width: 80px; height: 80px;
    bottom: 14%; left: 6%;
    filter: blur(24px);
  }

  .numbers-v2__stats {
    flex-direction: column;
    gap: 20px;
    align-items: center;
  }

  .numbers-v2__stat {
    width: 100%;
    max-width: 340px;
    padding: 36px 28px 32px;
    backdrop-filter: blur(14px) saturate(150%);
    -webkit-backdrop-filter: blur(14px) saturate(150%);
  }

  .numbers-v2__num {
    font-size: 48px;
  }

  .numbers-v2__lbl {
    font-size: 12px;
  }
}
</style>
