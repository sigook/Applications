<template>
  <main class="apply-page">
    <header class="apply-page__hero">
      <EyebrowPill variant="red" class="apply-page__eyebrow">
        Worker Registration
      </EyebrowPill>
      <h1 class="apply-page__heading">
        Join <span class="apply-page__heading-accent">Sigook.</span><br>
        Get <span class="apply-page__heading-accent">matched</span> in days.
      </h1>
      <p class="apply-page__subtitle">
        Tell us about you, your skills, and the documents that prove them.
        Our recruiters will pair you with vetted openings across Canada and
        the US — usually within a week.
      </p>
    </header>

    <section class="apply-page__form-wrap">
      <div class="apply-page__form">
        <WorkerRegisterForm />
      </div>
    </section>
  </main>
</template>

<script setup lang="ts">
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import WorkerRegisterForm from '@/components/landing/shared/forms/WorkerRegisterForm.vue'
</script>

<style scoped>
.apply-page {
  position: relative;
  font-family: var(--font-family);
  color: #fff;
}

.apply-page__hero {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
  padding:
    clamp(120px, 14vw, 180px)
    clamp(20px, 3vw, 32px)
    clamp(48px, 6vw, 72px);
}

.apply-page__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.apply-page__heading {
  font-size: clamp(32px, 5vw, 52px);
  font-weight: 700;
  line-height: 1.1;
  letter-spacing: -0.02em;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.apply-page__heading-accent {
  color: var(--c-brand-cyan);
}

.apply-page__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.85);
  margin: 0;
  max-width: 620px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

.apply-page__form-wrap {
  position: relative;
  z-index: 2;
  padding: 0 clamp(20px, 3vw, 40px) clamp(96px, 12vw, 160px);
}

.apply-page__form {
  position: relative;
  max-width: 1080px;
  margin: 0 auto;
  padding: clamp(32px, 4vw, 56px) clamp(28px, 3.4vw, 56px);
  background: linear-gradient(180deg,
    rgba(15, 47, 68, 0.78) 0%,
    rgba(9, 48, 85, 0.78) 100%);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.20);
  border-radius:
    clamp(28px, 3.2vw, 40px) clamp(28px, 3.2vw, 40px)
    clamp(28px, 3.2vw, 40px) clamp(64px, 7vw, 96px);
  box-shadow: 0 24px 60px rgba(0, 0, 0, 0.40);
  overflow: hidden;
  isolation: isolate;
}

.apply-page__form::before {
  content: '';
  position: absolute;
  pointer-events: none;
  z-index: 0;
  bottom: 0;
  left: 0;
  width: clamp(220px, 26vw, 360px);
  height: clamp(220px, 26vw, 360px);
  background: radial-gradient(circle at bottom left,
    rgba(0, 173, 239, 0.32) 0%,
    rgba(0, 173, 239, 0.10) 40%,
    transparent 70%);
  border-radius: 0 clamp(64px, 7vw, 96px) 0 0;
}

.apply-page__form > :deep(*) {
  position: relative;
  z-index: 1;
}
</style>
