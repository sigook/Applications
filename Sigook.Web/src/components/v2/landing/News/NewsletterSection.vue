<template>
  <section class="news-newsletter">
    <div class="news-newsletter__inner">
      <header class="news-newsletter__header">
        <EyebrowPill variant="cyan" class="news-newsletter__eyebrow">
          Newsletter
        </EyebrowPill>

        <h2 class="news-newsletter__heading">
          Stay in the loop.
          <span class="news-newsletter__heading-accent">
            One digest, every Friday.
          </span>
        </h2>

        <p class="news-newsletter__subtitle">
          The week's stories — compliance, hiring trends, and Sigook moves —
          condensed into a five-minute read. No filler, no sales pitches.
        </p>
      </header>

      <form class="news-newsletter__form" @submit.prevent="onSubmit">
        <div class="news-newsletter__field">
          <label class="news-newsletter__label" for="newsletter-email">
            Work email
          </label>
          <input
            id="newsletter-email"
            v-model="email"
            type="email"
            required
            placeholder="you@company.com"
            class="news-newsletter__input"
            :disabled="submitted"
            autocomplete="email"
          />
        </div>

        <button
          type="submit"
          class="news-newsletter__submit"
          :disabled="submitted"
        >
          <span v-if="!submitted">
            <span>Subscribe</span>
            <span class="news-newsletter__submit-arrow" aria-hidden="true">→</span>
          </span>
          <span v-else>Subscribed ✓</span>
        </button>
      </form>

      <p class="news-newsletter__footnote">
        We send one email per week. Unsubscribe with a single click. See our
        <a href="/v2/privacy-policy">privacy policy</a>.
      </p>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * NewsletterSection — window-style subscription block that closes the page.
 *
 * The form is currently UI-only (logs to console, flips a "Subscribed" state
 * for visual feedback). Hooking it to the backend mailing-list endpoint is a
 * follow-up task.
 *
 * Validation: `type="email" required` covers basic format + presence; the
 * browser surfaces native error UI on submit.
 */
import { ref } from 'vue'
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'

const email = ref('')
const submitted = ref(false)

function onSubmit(): void {
  if (!email.value) return
  // Placeholder — replace with a POST to the newsletter API when available.
  console.info('[newsletter] subscribe:', email.value)
  submitted.value = true
}
</script>

<style scoped>
/* ── Window shell — transparent (GlobalBackground shows through) ────────── */
.news-newsletter {
  position: relative;
  width: 100%;
  padding:
    clamp(72px, 10vw, 140px)
    clamp(20px, 3vw, 40px)
    clamp(96px, 12vw, 180px);
  display: flex;
  flex-direction: column;
  align-items: center;
  isolation: isolate;
  overflow: hidden;
  font-family: var(--font-family);
}

/* Inner editorial card — keeps the form scannable */
.news-newsletter__inner {
  position: relative;
  z-index: 2;
  width: 100%;
  max-width: 760px;
  margin: 0 auto;
  padding: clamp(32px, 4vw, 56px) clamp(28px, 3.6vw, 56px);
  border-radius:
    clamp(28px, 3.2vw, 40px) clamp(28px, 3.2vw, 40px)
    clamp(28px, 3.2vw, 40px) clamp(60px, 7vw, 96px);
  background: linear-gradient(180deg,
    rgba(255, 255, 255, 0.10) 0%,
    rgba(255, 255, 255, 0.04) 100%);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.20);
  box-shadow: 0 20px 48px rgba(0, 0, 0, 0.30);
  text-align: center;
}

/* Cyan glow accent in the asymmetric corner */
.news-newsletter__inner::before {
  content: '';
  position: absolute;
  pointer-events: none;
  z-index: 0;
  bottom: 0;
  left: 0;
  width: clamp(160px, 18vw, 240px);
  height: clamp(160px, 18vw, 240px);
  background: radial-gradient(circle at bottom left,
    rgba(0, 173, 239, 0.45) 0%,
    rgba(0, 173, 239, 0.14) 40%,
    transparent 70%);
  border-radius: 0 clamp(60px, 7vw, 96px) 0 0;
}

/* ── Header ─────────────────────────────────────────────────────────────── */
.news-newsletter__header {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: clamp(28px, 3.4vw, 40px);
}

.news-newsletter__eyebrow {
  margin-bottom: clamp(16px, 2vw, 22px);
}

.news-newsletter__heading {
  font-size: clamp(26px, 3.8vw, 40px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(12px, 1.6vw, 18px);
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.news-newsletter__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.news-newsletter__subtitle {
  font-size: clamp(13px, 1.2vw, 15px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.82);
  margin: 0;
  max-width: 540px;
}

/* ── Form — email input + submit on one row ─────────────────────────────── */
.news-newsletter__form {
  position: relative;
  z-index: 1;
  display: grid;
  grid-template-columns: 1fr auto;
  gap: clamp(10px, 1.2vw, 14px);
  align-items: end;
  margin: 0 auto;
  max-width: 560px;
}

.news-newsletter__field {
  display: flex;
  flex-direction: column;
  text-align: left;
}

.news-newsletter__label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.65);
  margin-bottom: 6px;
}

.news-newsletter__input {
  height: clamp(46px, 4.6vw, 54px);
  padding: 0 18px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.25);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.15vw, 15px);
  font-weight: 500;
  outline: none;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    box-shadow 0.25s ease;
}

.news-newsletter__input::placeholder {
  color: rgba(255, 255, 255, 0.45);
}

.news-newsletter__input:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.40);
}

.news-newsletter__input:focus {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 4px rgba(0, 173, 239, 0.20);
}

.news-newsletter__input:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

/* Submit button */
.news-newsletter__submit {
  height: clamp(46px, 4.6vw, 54px);
  padding: 0 clamp(20px, 2.2vw, 28px);
  background: var(--c-brand-cyan);
  border: 1px solid var(--c-brand-cyan);
  border-radius: 999px;
  color: var(--c-brand-navy);
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 700;
  letter-spacing: 0.04em;
  cursor: pointer;
  white-space: nowrap;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease,
    box-shadow 0.25s ease;
}

.news-newsletter__submit span {
  display: inline-flex;
  align-items: center;
  gap: 10px;
}

.news-newsletter__submit-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.news-newsletter__submit:hover:not(:disabled) {
  background: #0098d6;
  border-color: #0098d6;
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(0, 173, 239, 0.40);
}

.news-newsletter__submit:hover:not(:disabled) .news-newsletter__submit-arrow {
  transform: translateX(3px);
}

.news-newsletter__submit:disabled {
  background: rgba(0, 173, 239, 0.30);
  border-color: rgba(0, 173, 239, 0.30);
  cursor: default;
}

/* ── Footnote ───────────────────────────────────────────────────────────── */
.news-newsletter__footnote {
  position: relative;
  z-index: 1;
  margin: clamp(20px, 2.4vw, 28px) auto 0;
  max-width: 520px;
  font-size: clamp(11px, 1vw, 12px);
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.55);
}

.news-newsletter__footnote a {
  color: rgba(255, 255, 255, 0.75);
  text-decoration: underline;
  text-underline-offset: 2px;
}

.news-newsletter__footnote a:hover {
  color: var(--c-brand-cyan);
}

/* ── Responsive — stack form on small screens ───────────────────────────── */
@media (max-width: 599px) {
  .news-newsletter__form { grid-template-columns: 1fr; }
}
</style>
