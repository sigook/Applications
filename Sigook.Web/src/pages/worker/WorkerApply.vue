<template>
  <main class="worker-apply-page">
    <header class="worker-apply-page__hero">
      <EyebrowPill variant="red" class="worker-apply-page__eyebrow">
        Application
      </EyebrowPill>
      <h1 class="worker-apply-page__heading">
        You are applying with <span class="worker-apply-page__heading-accent">Sigook</span>.<br>
        Nothing else to <span class="worker-apply-page__heading-accent">fill in</span>.
      </h1>
      <p class="worker-apply-page__subtitle">
        We are matching the invitation we emailed you to the position it was sent for.
        This page takes care of the rest.
      </p>
    </header>

    <section class="worker-apply-page__form-wrap">
      <div class="worker-apply-page__form landing-form">
        <span class="worker-apply-page__form-glow" aria-hidden="true"></span>

        <div class="worker-apply-page__status" :class="`worker-apply-page__status--${status}`">
          <span class="worker-apply-page__status-icon" aria-hidden="true">
            <svg v-if="status === 'success'" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round">
              <path d="M20 6 9 17l-5-5" />
            </svg>
            <svg v-else-if="status === 'error'" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round">
              <path d="M12 8v5" />
              <path d="M12 17h.01" />
              <circle cx="12" cy="12" r="9" />
            </svg>
            <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.4" stroke-linecap="round">
              <path d="M12 3a9 9 0 1 0 9 9" />
            </svg>
          </span>

          <h2 class="worker-apply-page__status-title">{{ statusTitle }}</h2>

          <p v-if="status === 'error'" class="worker-apply-page__status-text" v-html="errorMessage"></p>
          <p v-else class="worker-apply-page__status-text">{{ statusText }}</p>

          <button
            v-if="status !== 'loading'"
            type="button"
            class="btn btn--primary worker-apply-page__action"
            @click="redirectToHome"
          >
            Back to Sigook
          </button>
        </div>
      </div>
    </section>
  </main>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRoute, type LocationQueryValue } from 'vue-router';
import { getErrorMessage } from '@/utils/toast';
import { requestApplyByEmail } from '@/api/workerApi';
import EyebrowPill from '@/components/landing/shared/ui/EyebrowPill.vue';

type ApplyStatus = 'loading' | 'success' | 'error';

const route = useRoute();

const status = ref<ApplyStatus>('loading');
const errorMessage = ref<string | null>(null);
const defaultSuccessMessage = 'Thank you, one of our recruiters will contact you soon.';
const defaultErrorMessage = 'We could not process your application, please contact the agency.';

const statusTitle = computed(() => {
  if (status.value === 'success') return 'Application sent';
  if (status.value === 'error') return 'We could not send your application';
  return 'Sending your application';
});

const statusText = computed(() =>
  status.value === 'success' ? defaultSuccessMessage : 'This only takes a moment.');

function redirectToHome() {
  window.location.href = '/';
}

function firstParam(value: LocationQueryValue | LocationQueryValue[]): string | null {
  const single = Array.isArray(value) ? value[0] : value;
  return typeof single === 'string' && single ? single : null;
}

function apply() {
  const numberIdParam = firstParam(route.query.n);
  const email = firstParam(route.query.e);

  const numberId = numberIdParam ? Number(numberIdParam) : NaN;
  if (!Number.isInteger(numberId) || numberId <= 0 || !email) {
    redirectToHome();
    return;
  }

  const key = `${numberId}|${email}`;
  const alreadyApplied = window.sessionStorage.getItem(key);
  if (alreadyApplied) {
    status.value = 'success';
    return;
  }

  requestApplyByEmail(numberId, email)
    .then(() => {
      status.value = 'success';
      window.sessionStorage.setItem(key, '1');
    })
    .catch(async (error: unknown) => {
      errorMessage.value = (await getErrorMessage(error)) || defaultErrorMessage;
      status.value = 'error';
    });
}

apply();
</script>

<style scoped>
.worker-apply-page {
  position: relative;
  font-family: var(--font-family);
  color: #fff;
}

.worker-apply-page__hero {
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

.worker-apply-page__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.worker-apply-page__heading {
  font-size: clamp(32px, 5vw, 52px);
  font-weight: 700;
  line-height: 1.1;
  letter-spacing: -0.02em;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  text-shadow: var(--sh-text-heading);
}

.worker-apply-page__heading-accent {
  color: var(--c-brand-cyan);
}

.worker-apply-page__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.85);
  margin: 0;
  max-width: 620px;
  text-shadow: var(--sh-text-sub);
}

.worker-apply-page__form-wrap {
  position: relative;
  z-index: 2;
  padding: 0 clamp(20px, 3vw, 40px) clamp(96px, 12vw, 160px);
}

.worker-apply-page__form {
  position: relative;
  max-width: 620px;
  margin: 0 auto;
  padding: clamp(32px, 4vw, 56px) clamp(28px, 3.4vw, 56px);
  background: linear-gradient(180deg,
    rgba(15, 47, 68, 0.78) 0%,
    rgba(9, 48, 85, 0.78) 100%);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid var(--c-glass-border);
  border-radius:
    clamp(28px, 3.2vw, 40px) clamp(28px, 3.2vw, 40px)
    clamp(28px, 3.2vw, 40px) clamp(64px, 7vw, 96px);
  box-shadow: 0 24px 60px rgba(0, 0, 0, 0.40);
  isolation: isolate;
}

.worker-apply-page__form > :not(.worker-apply-page__form-glow) {
  position: relative;
  z-index: 1;
}

.worker-apply-page__form-glow {
  position: absolute;
  inset: 0;
  z-index: 0;
  pointer-events: none;
  border-radius: inherit;
  overflow: hidden;
}

.worker-apply-page__form-glow::before {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: clamp(220px, 26vw, 360px);
  height: clamp(220px, 26vw, 360px);
  background: radial-gradient(circle at bottom left,
    rgba(0, 173, 239, 0.32) 0%,
    rgba(0, 173, 239, 0.10) 40%,
    transparent 70%);
}

.worker-apply-page__status {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.worker-apply-page__status-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 64px;
  height: 64px;
  margin-bottom: 24px;
  border-radius: 999px;
  background: rgba(var(--c-brand-cyan-rgb), 0.12);
  border: 1px solid rgba(var(--c-brand-cyan-rgb), 0.40);
  color: var(--c-brand-cyan);
}

.worker-apply-page__status-icon svg {
  width: 30px;
  height: 30px;
}

.worker-apply-page__status--loading .worker-apply-page__status-icon svg {
  animation: worker-apply-spin 0.9s linear infinite;
}

.worker-apply-page__status--error .worker-apply-page__status-icon {
  background: rgba(var(--c-brand-red-rgb), 0.12);
  border-color: rgba(var(--c-brand-red-rgb), 0.40);
  color: var(--c-brand-red);
}

.worker-apply-page__status-title {
  font-size: clamp(20px, 2.4vw, 26px);
  font-weight: 700;
  line-height: 1.2;
  letter-spacing: -0.01em;
  text-transform: none;
  margin: 0 0 12px;
  color: #fff;
}

.worker-apply-page__status-text {
  font-size: 15px;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.75);
  margin: 0;
  max-width: 420px;
}

.worker-apply-page__action {
  margin-top: 32px;
}

@keyframes worker-apply-spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
