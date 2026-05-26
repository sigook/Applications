<template>
  <section class="op-cta">
    <div class="op-cta__inner">
      <EyebrowPill variant="red" class="op-cta__eyebrow">
        Still Looking?
      </EyebrowPill>

      <h2 class="op-cta__heading">
        Don't see your role yet?
        <span class="op-cta__heading-accent">Send your resume anyway.</span>
      </h2>

      <p class="op-cta__subtitle">
        New roles open every week — and our recruiters keep a watch list.
        Drop your CV and we'll let you know the moment a fit lands.
      </p>

      <div class="op-cta__actions">
        <a href="#openpos-contact" class="op-cta__primary">
          <span>Send your resume</span>
          <span class="op-cta__arrow" aria-hidden="true">→</span>
        </a>
        <router-link to="/v2/industries" class="op-cta__secondary">
          <span>Browse industries</span>
          <span class="op-cta__arrow" aria-hidden="true">→</span>
        </router-link>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * OpenPositionsCtaSection — bridge between the job list and Contact.
 *
 * Window-style closing CTA for users who didn't find a fit in the list.
 * Two actions:
 *  • Primary: scroll to contact form to send a CV
 *  • Secondary: pivot to the industries page for sector-level browsing
 */
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
</script>

<style scoped>
/* ── Window shell — transparent ─────────────────────────────────────────── */
.op-cta {
  position: relative;
  width: 100%;
  padding:
    clamp(72px, 10vw, 140px)
    clamp(20px, 3vw, 40px);
  display: flex;
  flex-direction: column;
  align-items: center;
  isolation: isolate;
  overflow: hidden;
  font-family: var(--font-family);
}

/* Inner editorial card — anchors the CTA against the GlobalBackground */
.op-cta__inner {
  position: relative;
  z-index: 2;
  width: 100%;
  max-width: 880px;
  margin: 0 auto;
  padding: clamp(36px, 4.5vw, 60px) clamp(28px, 3.6vw, 56px);
  border-radius:
    clamp(28px, 3.4vw, 44px) clamp(28px, 3.4vw, 44px)
    clamp(28px, 3.4vw, 44px) clamp(64px, 7.5vw, 100px);
  background: linear-gradient(180deg,
    rgba(229, 45, 39, 0.18) 0%,
    rgba(229, 45, 39, 0.04) 100%);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid rgba(229, 45, 39, 0.35);
  box-shadow: 0 22px 50px rgba(0, 0, 0, 0.30);
  text-align: center;
}

/* Red glow accent at the asymmetric corner */
.op-cta__inner::before {
  content: '';
  position: absolute;
  pointer-events: none;
  z-index: 0;
  bottom: 0;
  left: 0;
  width: clamp(180px, 20vw, 280px);
  height: clamp(180px, 20vw, 280px);
  background: radial-gradient(circle at bottom left,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.18) 40%,
    transparent 70%);
  border-radius: 0 clamp(64px, 7.5vw, 100px) 0 0;
}

/* ── Content ────────────────────────────────────────────────────────────── */
.op-cta__eyebrow {
  position: relative;
  z-index: 1;
  margin-bottom: clamp(18px, 2.2vw, 24px);
}

.op-cta__heading {
  position: relative;
  z-index: 1;
  font-size: clamp(26px, 4vw, 42px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 20px);
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.op-cta__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.op-cta__subtitle {
  position: relative;
  z-index: 1;
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 auto clamp(28px, 3.4vw, 40px);
  max-width: 560px;
}

/* ── Actions ────────────────────────────────────────────────────────────── */
.op-cta__actions {
  position: relative;
  z-index: 1;
  display: inline-flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: clamp(12px, 1.6vw, 18px);
}

.op-cta__primary,
.op-cta__secondary {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: clamp(13px, 1.4vw, 16px) clamp(26px, 2.8vw, 36px);
  border-radius: 999px;
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 700;
  letter-spacing: 0.04em;
  text-decoration: none;
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease,
    box-shadow 0.25s ease;
}

.op-cta__primary {
  background: #fff;
  border: 1px solid #fff;
  color: var(--c-brand-red);
}

.op-cta__primary:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(0, 173, 239, 0.35);
}

.op-cta__secondary {
  background: rgba(255, 255, 255, 0.10);
  border: 1.5px solid rgba(255, 255, 255, 0.55);
  color: #fff;
}

.op-cta__secondary:hover {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-navy);
  transform: translateY(-1px);
}

.op-cta__arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.op-cta__primary:hover .op-cta__arrow,
.op-cta__secondary:hover .op-cta__arrow {
  transform: translateX(3px);
}
</style>
