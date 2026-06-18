<template>
  <section class="op-cta">
    <div class="op-cta__back" aria-hidden="true"></div>
    <div class="op-cta__card">
      <div class="op-cta__inner">
        <div class="op-cta__grid">
          <div class="op-cta__copy">
            <EyebrowPill variant="red" class="op-cta__eyebrow">
              Still Looking?
            </EyebrowPill>

            <h2 class="op-cta__heading">
              Don't see your role yet?
              <span class="op-cta__heading-accent">Send your resume anyway.</span>
            </h2>

            <div class="op-cta__accent-line"></div>

            <p class="op-cta__subtitle">
              New roles open every week and our recruiters keep a watch list.
              Drop your CV and we'll reach out the moment a fit lands.
            </p>

            <div class="op-cta__actions">
              <button type="button" class="op-cta__primary" @click="onSendResume">
                <span>Send your resume</span>
                <span class="op-cta__arrow" aria-hidden="true">→</span>
              </button>
              <router-link to="/industries" class="op-cta__secondary">
                <span>Browse industries</span>
                <span class="op-cta__arrow" aria-hidden="true">→</span>
              </router-link>
            </div>
          </div>

          <ul class="op-cta__features" aria-label="What to expect">
            <li v-for="feature in features" :key="feature" class="op-cta__feature">
              <span class="op-cta__check" aria-hidden="true">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                  <polyline points="20 6 9 17 4 12" />
                </svg>
              </span>
              <span class="op-cta__feature-text">{{ feature }}</span>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import { useCandidateApplyModal } from '@/components/landing/shared/forms/useCandidateApplyModal'

const registerModal = useCandidateApplyModal()

const features = [
  'Fresh roles posted every week',
  'Recruiters keep a personal watch list',
  'We reach out when a role fits you',
  'One short form — no account needed',
]

function onSendResume(): void {
  registerModal.open()
}
</script>

<style scoped>
.op-cta {
  position: relative;
  width: 100%;
  padding: clamp(8px, 1.5vw, 28px) 0 clamp(64px, 9vw, 120px);
  isolation: isolate;
  font-family: var(--font-family);
}

/* ── Depth back-layer — transparent glass, peeks ~16px above & below card ─ */
.op-cta__back {
  position: absolute;
  top: calc(clamp(8px, 1.5vw, 28px) - 16px);
  bottom: calc(clamp(64px, 9vw, 120px) - 16px);
  left: 0;
  right: 0;
  z-index: 0;
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

.op-cta__card {
  position: relative;
  z-index: 1;
  width: 100%;
  min-height: clamp(480px, 58vh, 740px);
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding:
    clamp(72px, 10vw, 120px)
    clamp(20px, 3vw, 64px);
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  overflow: hidden;
  background:
    linear-gradient(120deg,
      rgba(229, 45, 39, 0.3) 0%,
      rgba(229, 45, 39, 0.08) 38%,
      rgba(9, 48, 85, 0.5) 100%),
    rgba(11, 38, 64, 0.6);
  backdrop-filter: blur(24px) saturate(160%);
  -webkit-backdrop-filter: blur(24px) saturate(160%);
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0 22px 40px -12px rgba(0, 0, 0, 0.45);
}

.op-cta__card::after {
  content: '';
  position: absolute;
  pointer-events: none;
  z-index: 0;
  bottom: 0;
  right: 0;
  width: clamp(220px, 24vw, 360px);
  height: clamp(220px, 24vw, 360px);
  background: radial-gradient(circle at bottom right,
    rgba(0, 173, 239, 0.18) 0%,
    rgba(0, 173, 239, 0.05) 45%,
    transparent 72%);
}

.op-cta__inner {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: var(--container-max);
  margin: 0 auto;
}

.op-cta__grid {
  display: grid;
  grid-template-columns: 1.15fr 0.85fr;
  align-items: center;
  gap: clamp(32px, 5vw, 72px);
}

.op-cta__copy {
  text-align: left;
}

.op-cta__eyebrow {
  margin-bottom: clamp(16px, 2vw, 22px);
}

.op-cta__heading {
  font-size: clamp(26px, 3.6vw, 42px);
  font-weight: 700;
  line-height: 1.12;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.4);
}

.op-cta__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.op-cta__accent-line {
  width: 56px;
  height: 3px;
  border-radius: 2px;
  background: var(--c-brand-red);
  margin: clamp(16px, 2vw, 22px) 0;
}

.op-cta__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.82);
  margin: 0 0 clamp(24px, 3vw, 34px);
  max-width: 480px;
}

.op-cta__actions {
  display: flex;
  flex-wrap: wrap;
  gap: clamp(12px, 1.6vw, 16px);
}

.op-cta__primary,
.op-cta__secondary {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: clamp(13px, 1.4vw, 16px) clamp(24px, 2.6vw, 32px);
  border-radius: 999px;
  font-family: var(--font-family);
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
  background: var(--c-brand-red);
  border: 1px solid var(--c-brand-red);
  color: #fff;
  box-shadow: 0 10px 24px -6px rgba(229, 45, 39, 0.6);
}

.op-cta__primary:hover {
  background: var(--c-brand-crimson);
  border-color: var(--c-brand-crimson);
  transform: translateY(-1px);
  box-shadow: 0 14px 28px -6px rgba(229, 45, 39, 0.7);
}

.op-cta__secondary {
  background: rgba(255, 255, 255, 0.1);
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

.op-cta__features {
  position: relative;
  z-index: 1;
  list-style: none;
  margin: 0;
  padding: clamp(22px, 2.6vw, 32px);
  display: flex;
  flex-direction: column;
  gap: clamp(14px, 1.8vw, 20px);
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.16);
  border-radius:
    0 clamp(18px, 2vw, 26px)
    0 clamp(18px, 2vw, 26px);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
}

.op-cta__feature {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  font-size: clamp(13px, 1.1vw, 15px);
  line-height: 1.45;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.92);
}

.op-cta__check {
  flex-shrink: 0;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  background: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  margin-top: 1px;
}

.op-cta__check svg {
  width: 13px;
  height: 13px;
}

@media (max-width: 1023px) {
  .op-cta__grid {
    grid-template-columns: 1fr;
    gap: clamp(24px, 5vw, 36px);
  }

  .op-cta__copy {
    text-align: center;
  }

  .op-cta__accent-line,
  .op-cta__subtitle {
    margin-left: auto;
    margin-right: auto;
  }

  .op-cta__actions {
    justify-content: center;
  }
}
</style>
