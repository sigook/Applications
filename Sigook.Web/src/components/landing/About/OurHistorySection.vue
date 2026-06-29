<template>
  <section class="our-history">
    <div class="our-history__surface" aria-hidden="true"></div>
    <div class="our-history__glow" aria-hidden="true"></div>

    <div class="our-history__inner">
      <LandingSectionHeader
        eyebrow="Our History"
        heading="Connecting people, companies, and opportunities."
        heading-accent="Since 2008."
        subtitle="From a Toronto consulting startup to a multi-state workforce partner — here's how the journey unfolded."
        margin-bottom="clamp(40px, 5.5vw, 64px)"
        eyebrow-margin-bottom="clamp(18px, 2.4vw, 26px)"
        heading-size="clamp(26px, 4vw, 44px)"
        heading-margin-bottom="0"
        heading-max-width="700px"
        subtitle-text-shadow="none"
        subtitle-margin-top="clamp(14px, 1.8vw, 20px)"
      />

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
import { useRevealOnScroll } from '@/composables/useRevealOnScroll'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import historyMilestonesData from '@/data/landing/historyMilestones.json'

interface Milestone {
  readonly year: string
  readonly title: string
  readonly body: string
}

const MILESTONES = historyMilestonesData as readonly Milestone[]

const { el: timeline, visible: revealed } = useRevealOnScroll({
  threshold: 0.12,
  rootMargin: '0px',
  once: true,
})
</script>

<style scoped>
.our-history {
  position: relative;
  width: 100%;
  margin-top: var(--panel-overlap);
  padding:
    var(--section-pad-y-lg)
    clamp(20px, 3vw, 64px)
    clamp(96px, 12vw, 160px);
  display: flex;
  flex-direction: column;
  align-items: center;
  z-index: 5;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
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
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: var(--glass-blur-soft);
  -webkit-backdrop-filter: var(--glass-blur-soft);
  border: 1px solid var(--c-glass-border-soft);
  box-shadow: var(--sh-back);
  pointer-events: none;
}

.our-history__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
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
    transform 0.6s var(--ease-brand);
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
