<template>
  <section class="our-history">
    <div class="our-history__surface" aria-hidden="true"></div>
    <div class="our-history__glow" aria-hidden="true"></div>

    <div class="our-history__inner">
      <header class="our-history__header">
        <EyebrowPill variant="white" class="our-history__eyebrow">
          Our History
        </EyebrowPill>

        <h2 class="our-history__heading">
          Connecting people, companies, and opportunities.
          <span class="our-history__heading-accent">Since 2008.</span>
        </h2>

        <p class="our-history__lead">
          From a Toronto consulting startup to a multi-state workforce partner —
          here's how the journey unfolded.
        </p>
      </header>

      <ol ref="timeline" class="our-history__timeline">
        <li
          v-for="(m, i) in MILESTONES"
          :key="m.year"
          class="our-history__milestone"
          :class="{ 'is-revealed': revealed }"
          :style="{ transitionDelay: `${i * 100}ms` }"
        >
          <span class="our-history__node" aria-hidden="true"></span>
          <span class="our-history__year">{{ m.year }}</span>
          <h3 class="our-history__milestone-title">{{ m.title }}</h3>
          <p class="our-history__milestone-text">{{ m.body }}</p>
        </li>
      </ol>

      <div class="our-history__meaning">
        <span class="our-history__meaning-eyebrow">The meaning behind the name</span>
        <div class="our-history__letters">
          <div class="our-history__letter">
            <span class="our-history__letter-mark">SI</span>
            <span class="our-history__letter-text">"Yes" in Spanish — saying yes to opportunity.</span>
          </div>
          <div class="our-history__letter">
            <span class="our-history__letter-mark">GO</span>
            <span class="our-history__letter-text">Action and movement — moving forward with purpose.</span>
          </div>
          <div class="our-history__letter">
            <span class="our-history__letter-mark">OK</span>
            <span class="our-history__letter-text">Confirmation and reliability — dependable outcomes.</span>
          </div>
        </div>
      </div>

      <div class="our-history__next">
        <h3 class="our-history__next-title">Building the future of work</h3>
        <p class="our-history__next-text">
          Beyond staffing, SIGOOK is developing workforce-readiness solutions so
          clients in logistics, warehousing, and manufacturing gain not only
          reliable talent, but practical support adapting to new tools and risks:
        </p>
        <ul class="our-history__chips">
          <li>Cybersecurity awareness</li>
          <li>Responsible AI adoption</li>
          <li>Supervisor productivity</li>
          <li>Technology-enabled support</li>
        </ul>
      </div>

      <div class="our-history__closing">
        <p class="our-history__closing-text">
          Our goal remains the same as it was from the beginning: to connect
          people, companies, and opportunities. What has evolved is the way we do
          it — by helping employers build teams that are not only available to
          work, but safer, smarter, and better prepared for the future of work.
        </p>
        <span class="our-history__tag">
          <span class="our-history__tag-star" aria-hidden="true">★</span>
          Founder-led · Majority woman-owned
        </span>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'

interface Milestone {
  readonly year: string
  readonly title: string
  readonly body: string
}

const MILESTONES: readonly Milestone[] = [
  {
    year: '2008',
    title: 'A Toronto beginning',
    body: 'Covenant Group Ltd. was incorporated in Toronto by husband-and-wife team Andrea and David — created to provide sales and marketing consulting for international companies entering the Canadian market, and Canadian organizations expanding into Latin America.',
  },
  {
    year: '2016',
    title: 'From consulting to staffing',
    body: 'Their mission evolved into something broader: helping the growing community of immigrants arriving in Canada access meaningful work. The company shifted to staffing for the industrial, manufacturing, and logistics sectors — quickly creating thousands of jobs across Canada and becoming a trusted partner for high-volume operations.',
  },
  {
    year: '2020',
    title: 'Recognized as essential',
    body: 'Recognized as an essential workplace during the public health emergency, Covenant Group grew significantly — supporting essential operations and connecting workers with opportunities at a critical time.',
  },
  {
    year: '2021',
    title: 'SIGOOK launches in the U.S.',
    body: 'Andrea and David expanded into the United States, incorporating Covenant Group Investors LLC, operating under the DBA SIGOOK.',
  },
  {
    year: 'Today',
    title: 'A multi-state growth story',
    body: 'SIGOOK has grown beyond Florida to serve clients across multiple states — employing approximately 1,500 workers in under four years. As part of the Covenant Group family, it delivers flexible staffing, recruitment, and workforce support, continually expanding to build a more prepared, productive, and resilient workforce.',
  },
] as const

const timeline = ref<HTMLElement | null>(null)
const revealed = ref(false)
let observer: IntersectionObserver | null = null

onMounted(() => {
  if (!timeline.value) return
  observer = new IntersectionObserver(
    (entries) => {
      if (entries[0].isIntersecting) {
        revealed.value = true
        observer?.disconnect()
      }
    },
    { threshold: 0.12 },
  )
  observer.observe(timeline.value)
})

onUnmounted(() => observer?.disconnect())
</script>

<style scoped>
.our-history {
  position: relative;
  width: 100%;
  margin-top: clamp(-180px, -10vw, -80px);
  padding:
    clamp(140px, 14vw, 200px)
    clamp(20px, 3vw, 64px)
    clamp(96px, 12vw, 160px);
  display: flex;
  flex-direction: column;
  align-items: center;
  z-index: 5;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0 22px 40px -12px rgba(0, 0, 0, 0.45);
  isolation: isolate;
  font-family: var(--font-family);
}

.our-history::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: blur(10px) saturate(120%);
  -webkit-backdrop-filter: blur(10px) saturate(120%);
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow: 0 18px 40px -20px rgba(0, 0, 0, 0.4);
  pointer-events: none;
}

.our-history__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

.our-history__glow {
  position: absolute;
  z-index: 0;
  pointer-events: none;
  bottom: 0;
  right: 0;
  width: clamp(220px, 26vw, 400px);
  height: clamp(220px, 26vw, 400px);
  background: radial-gradient(circle at bottom right,
    rgba(0, 173, 239, 0.18) 0%,
    rgba(0, 173, 239, 0.05) 45%,
    transparent 72%);
}

.our-history__inner {
  position: relative;
  z-index: 2;
  width: 100%;
  max-width: 820px;
  margin: 0 auto;
}

.our-history__header {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  margin-bottom: clamp(40px, 5.5vw, 64px);
}

.our-history__eyebrow {
  margin-bottom: clamp(18px, 2.4vw, 26px);
}

.our-history__heading {
  font-size: clamp(26px, 4vw, 44px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0;
  max-width: 700px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.4);
}

.our-history__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.our-history__lead {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: clamp(14px, 1.8vw, 20px) 0 0;
  max-width: 560px;
}

/* ── Timeline ───────────────────────────────────────────────────────────── */
.our-history__timeline {
  --rail: clamp(38px, 4.6vw, 52px);
  --line-x: clamp(11px, 1.3vw, 15px);
  --node: 16px;
  position: relative;
  list-style: none;
  margin: 0;
  padding: 0 0 0 var(--rail);
}

.our-history__timeline::before {
  content: '';
  position: absolute;
  left: var(--line-x);
  top: 6px;
  bottom: 8px;
  width: 2px;
  border-radius: 2px;
  background: linear-gradient(180deg,
    var(--c-brand-cyan) 0%,
    rgba(0, 173, 239, 0.6) 55%,
    rgba(0, 173, 239, 0.12) 100%);
}

.our-history__milestone {
  position: relative;
  padding-bottom: clamp(26px, 3.6vw, 42px);
  opacity: 0;
  transform: translateY(18px);
  transition:
    opacity 0.6s ease,
    transform 0.6s cubic-bezier(0.22, 1, 0.36, 1);
}

.our-history__milestone:last-child {
  padding-bottom: 0;
}

.our-history__milestone.is-revealed {
  opacity: 1;
  transform: none;
}

.our-history__node {
  position: absolute;
  top: 3px;
  left: calc(var(--line-x) + 1px - var(--rail) - (var(--node) / 2));
  width: var(--node);
  height: var(--node);
  border-radius: 50%;
  background: var(--c-brand-cyan);
  box-shadow:
    0 0 0 4px rgba(0, 173, 239, 0.16),
    0 0 16px rgba(0, 173, 239, 0.55);
}

.our-history__year {
  display: inline-block;
  font-size: clamp(17px, 1.9vw, 23px);
  font-weight: 800;
  letter-spacing: -0.01em;
  line-height: 1;
  color: var(--c-brand-cyan);
}

.our-history__milestone-title {
  font-size: clamp(15px, 1.5vw, 19px);
  font-weight: 700;
  line-height: 1.25;
  color: #fff;
  margin: clamp(6px, 0.9vw, 10px) 0 clamp(6px, 0.8vw, 9px);
}

.our-history__milestone-text {
  font-size: clamp(13.5px, 1.1vw, 15px);
  line-height: 1.7;
  color: rgba(255, 255, 255, 0.8);
  margin: 0;
}

/* ── SIGOOK meaning callout ─────────────────────────────────────────────── */
.our-history__meaning {
  margin-top: clamp(40px, 5.5vw, 64px);
  padding: clamp(24px, 3.2vw, 40px);
  border-radius:
    0 clamp(20px, 2.6vw, 32px)
    0 clamp(20px, 2.6vw, 32px);
  background: rgba(0, 173, 239, 0.07);
  border: 1px solid rgba(0, 173, 239, 0.22);
}

.our-history__meaning-eyebrow {
  display: block;
  text-align: center;
  font-size: clamp(10px, 0.85vw, 11px);
  font-weight: 700;
  letter-spacing: 0.2em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: clamp(18px, 2.4vw, 26px);
}

.our-history__letters {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(16px, 2.4vw, 28px);
}

.our-history__letter {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: clamp(6px, 0.9vw, 10px);
}

.our-history__letter-mark {
  font-size: clamp(32px, 4.4vw, 50px);
  font-weight: 800;
  letter-spacing: 0.02em;
  line-height: 1;
  color: #fff;
  text-shadow: 0 4px 18px rgba(0, 173, 239, 0.4);
}

.our-history__letter-text {
  font-size: clamp(12px, 1vw, 13.5px);
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.82);
}

/* ── What's next ────────────────────────────────────────────────────────── */
.our-history__next {
  margin-top: clamp(36px, 5vw, 56px);
}

.our-history__next-title {
  font-size: clamp(16px, 1.7vw, 20px);
  font-weight: 700;
  color: #fff;
  margin: 0 0 clamp(8px, 1vw, 12px);
}

.our-history__next-text {
  font-size: clamp(13.5px, 1.1vw, 15px);
  line-height: 1.7;
  color: rgba(255, 255, 255, 0.8);
  margin: 0 0 clamp(14px, 1.8vw, 20px);
}

.our-history__chips {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-wrap: wrap;
  gap: clamp(8px, 1vw, 12px);
}

.our-history__chips li {
  padding: clamp(8px, 1vw, 10px) clamp(14px, 1.6vw, 20px);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.18);
  color: rgba(255, 255, 255, 0.88);
  font-size: clamp(12px, 1vw, 13.5px);
  font-weight: 500;
}

/* ── Closing ────────────────────────────────────────────────────────────── */
.our-history__closing {
  margin-top: clamp(36px, 5vw, 56px);
  padding-top: clamp(26px, 3.2vw, 38px);
  border-top: 1px solid rgba(255, 255, 255, 0.12);
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: clamp(16px, 2vw, 22px);
}

.our-history__closing-text {
  font-size: clamp(14px, 1.2vw, 17px);
  font-weight: 500;
  line-height: 1.7;
  color: rgba(255, 255, 255, 0.92);
  margin: 0;
}

.our-history__tag {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: clamp(8px, 1vw, 10px) clamp(16px, 1.8vw, 20px);
  border-radius: 999px;
  background: rgba(229, 45, 39, 0.12);
  border: 1px solid rgba(229, 45, 39, 0.4);
  color: #fff;
  font-size: clamp(11px, 0.95vw, 13px);
  font-weight: 700;
  letter-spacing: 0.02em;
}

.our-history__tag-star {
  color: var(--c-brand-red);
  font-size: 1.1em;
  line-height: 1;
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 560px) {
  .our-history__letters {
    grid-template-columns: 1fr;
    gap: clamp(14px, 4vw, 20px);
  }
}

@media (prefers-reduced-motion: reduce) {
  .our-history__milestone {
    opacity: 1;
    transform: none;
    transition: none;
  }
}
</style>
