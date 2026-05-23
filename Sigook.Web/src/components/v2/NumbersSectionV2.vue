<template>
  <section
    ref="sectionRef"
    class="numbers-v2"
    :class="{ 'is-visible': visible }"
  >
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
  observer = new IntersectionObserver(
    (entries) => { visible.value = entries[0].isIntersecting },
    { threshold: 0.15, rootMargin: '0px 0px -10% 0px' }
  )
  observer.observe(sectionRef.value)
})

onUnmounted(() => observer?.disconnect())
</script>

<style scoped>
/* ── Section shell — transparent (background lives in GlobalBackgroundV2) ─── */
.numbers-v2 {
  position: relative;
  margin-top: -177px;
  width: 100%;
  height: 1351px;
  overflow: hidden;
  isolation: isolate;
}

.numbers-v2__canvas {
  position: relative;
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

/* ── Header ──────────────────────────────────────────────────────────────── */
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

.numbers-v2.is-visible .numbers-v2__stat--blue { transition-delay: 0.22s; }
.numbers-v2.is-visible .numbers-v2__stat--cyan { transition-delay: 0.36s; }
.numbers-v2.is-visible .numbers-v2__stat--red  { transition-delay: 0.50s; }

/* ── Card color variants — each plays with red + blue (+ cyan) ───────────── */

/* Shared radial corner glow */
.numbers-v2__stat::before {
  content: '';
  position: absolute;
  pointer-events: none;
  width: 160px;
  height: 160px;
}

/* Card 1 — Blue dominant gradient with red accent */
.numbers-v2__stat--blue {
  background: linear-gradient(135deg,
    rgba(21, 117, 187, 0.42) 0%,
    rgba(21, 117, 187, 0.14) 50%,
    rgba(229, 45, 39, 0.32) 100%
  );
  border-radius: 24px 64px 24px 64px;
}
.numbers-v2__stat--blue::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.18) 35%,
    transparent 70%);
  border-radius: 0 64px 0 0;
}

/* Card 2 — Cyan → blue → red triple gradient */
.numbers-v2__stat--cyan {
  background: linear-gradient(135deg,
    rgba(0, 173, 239, 0.34) 0%,
    rgba(21, 117, 187, 0.22) 50%,
    rgba(229, 45, 39, 0.30) 100%
  );
  border-radius: 64px 24px 64px 24px;
}
.numbers-v2__stat--cyan::before {
  top: 0;
  left: 0;
  background: radial-gradient(circle at top left,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.18) 35%,
    transparent 70%);
  border-radius: 64px 0 0 0;
}

/* Card 3 — Red dominant gradient with blue accent */
.numbers-v2__stat--red {
  background: linear-gradient(135deg,
    rgba(229, 45, 39, 0.42) 0%,
    rgba(229, 45, 39, 0.14) 50%,
    rgba(21, 117, 187, 0.32) 100%
  );
  border-radius: 24px 64px 24px 64px;
}
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
  color: rgba(255, 255, 255, 0.85);
  letter-spacing: 0.10em;
  text-transform: uppercase;
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
