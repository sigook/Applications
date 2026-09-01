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
              <ArrowPillCta variant="solid" size="lg" @click="onSendResume">
                Send your resume
              </ArrowPillCta>
              <ArrowPillCta to="/industries" hover-variant="navy" size="lg">
                Browse industries
              </ArrowPillCta>
            </div>
          </div>

          <ul class="op-cta__features" aria-label="What to expect">
            <li v-for="feature in features" :key="feature" class="op-cta__feature">
              <span class="op-cta__check" aria-hidden="true">
                <LandingIcon name="check" />
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
import LandingIcon from '@/components/landing/shared/icons/LandingIcon.vue'
import EyebrowPill from '@/components/landing/shared/ui/EyebrowPill.vue'
import { useCandidateApplyModal } from '@/composables/useCandidateApplyModal'
import ArrowPillCta from '@/components/landing/shared/ui/ArrowPillCta.vue'

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

.op-cta__back {
  position: absolute;
  top: calc(clamp(8px, 1.5vw, 28px) - 16px);
  bottom: calc(clamp(64px, 9vw, 120px) - 16px);
  left: 0;
  right: 0;
  z-index: 0;
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
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
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

.op-cta__features {
  position: relative;
  z-index: 1;
  list-style: none;
  margin: 0;
  padding: clamp(22px, 2.6vw, 32px);
  display: flex;
  flex-direction: column;
  gap: clamp(14px, 1.8vw, 20px);
  background: var(--c-glass-fill);
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
