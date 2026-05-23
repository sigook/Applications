<template>
  <div class="git-card" :class="`git-card--${size}`">
    <header class="git-card__header">
      <span class="git-card__eyebrow">Get Started</span>
      <h3 class="git-card__title">Your information</h3>
      <p class="git-card__sub">We'll reach out to you for the next steps.</p>
      <div class="git-card__divider" aria-hidden="true"></div>
    </header>

    <form class="git-card__form" @submit.prevent="handleFormSubmit" novalidate>
      <!-- Name row -->
      <div class="git-card__field-group">
        <label class="git-card__label">Name</label>
        <div class="git-card__name-row">
          <div class="git-card__name-col">
            <input
              class="git-card__input"
              :class="{ 'git-card__input--error': errors.firstName }"
              v-model="fields.firstName.value"
              type="text"
              placeholder="First name"
              autocomplete="given-name"
            />
            <span v-if="errors.firstName" class="git-card__field-error">{{ errors.firstName }}</span>
          </div>
          <div class="git-card__name-col">
            <input
              class="git-card__input"
              :class="{ 'git-card__input--error': errors.lastName }"
              v-model="fields.lastName.value"
              type="text"
              placeholder="Last name"
              autocomplete="family-name"
            />
            <span v-if="errors.lastName" class="git-card__field-error">{{ errors.lastName }}</span>
          </div>
        </div>
      </div>

      <!-- Email -->
      <div class="git-card__field-group">
        <label class="git-card__label">Email</label>
        <input
          class="git-card__input git-card__input--full"
          :class="{ 'git-card__input--error': errors.email }"
          v-model="fields.email.value"
          type="email"
          placeholder="your.email@company.com"
          autocomplete="email"
        />
        <span v-if="errors.email" class="git-card__field-error">{{ errors.email }}</span>
      </div>

      <!-- Industry -->
      <div class="git-card__field-group">
        <label class="git-card__label">Industry</label>
        <input
          class="git-card__input git-card__input--full"
          v-model="fields.industry.value"
          type="text"
          placeholder="Select one"
        />
      </div>

      <!-- Message -->
      <div class="git-card__field-group">
        <label class="git-card__label">Message</label>
        <textarea
          class="git-card__textarea"
          :class="{ 'git-card__input--error': errors.message }"
          v-model="fields.message.value"
          placeholder="Tell us more about the role you need to fill."
          rows="4"
        ></textarea>
        <span v-if="errors.message" class="git-card__field-error">{{ errors.message }}</span>
      </div>

      <!-- Feedback -->
      <div v-if="submitted" class="git-card__feedback git-card__feedback--success">
        Thank you! We'll reach out to you shortly.
      </div>
      <div v-if="submitError" class="git-card__feedback git-card__feedback--error">
        {{ submitError }}
      </div>

      <!-- Submit -->
      <button type="submit" class="git-card__submit" :disabled="submitting">
        <span>{{ submitting ? 'Sending…' : 'Save & Send' }}</span>
        <ArrowIconV2 v-if="!submitting" :width="32" :height="11" :stroke-width="1.5" color="#fff" />
      </button>

      <!-- Reset -->
      <button type="button" class="git-card__reset" @click="handleReset">
        Reset information
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import * as yup from 'yup'
import { useStickyForm } from '@/composables/useStickyForm'
import { submitContactForm } from '@/api/websiteApi'
import ArrowIconV2 from '@/components/v2/ArrowIconV2.vue'

withDefaults(defineProps<{ size?: 'default' | 'compact' }>(), { size: 'default' })

const schema = yup.object({
  firstName: yup.string().required('First name is required').max(50, 'Max 50 characters'),
  lastName:  yup.string().required('Last name is required').max(50, 'Max 50 characters'),
  email:     yup.string().required('Email is required').email('Invalid email address').max(100),
  industry:  yup.string().max(100).optional(),
  message:   yup.string().required('Message is required').max(500, 'Max 500 characters'),
})

const { fields, errors, validate, markInteracted, resetAll } = useStickyForm({
  schema,
  initialValues: { firstName: '', lastName: '', email: '', industry: '', message: '' },
})

const submitting  = ref(false)
const submitted   = ref(false)
const submitError = ref('')

async function handleFormSubmit() {
  markInteracted()
  const result = await validate()
  if (!result.valid) return

  submitting.value  = true
  submitError.value = ''
  submitted.value   = false

  try {
    await submitContactForm({
      title:           'Get in Touch',
      name:            `${fields.firstName.value} ${fields.lastName.value}`.trim(),
      email:           fields.email.value,
      phone:           '',
      message:         fields.message.value,
      subject:         fields.industry.value || '',
      captchaResponse: '',
    })
    submitted.value = true
    resetAll()
  } catch {
    submitError.value = 'Something went wrong. Please try again later.'
  } finally {
    submitting.value = false
  }
}

function handleReset() {
  resetAll()
  submitted.value   = false
  submitError.value = ''
}
</script>

<style scoped>
/* ── Card shell — asymmetric brand radius, deeper shadow ────────────────── */
.git-card {
  position: relative;
  background: #fff;
  border-radius: 20px 56px 20px 56px;   /* asymmetric brand shape */
  box-shadow:
    0 24px 60px rgba(15, 47, 68, 0.20),
    0 4px 12px rgba(15, 47, 68, 0.06);
  padding: 44px 48px 44px;
  width: 550px;
  max-width: 60%;
  box-sizing: border-box;
}

/* Compact variant — narrower card for sidebar / secondary contexts */
.git-card--compact {
  width: 392px;
  padding: 32px 32px 36px;
  border-radius: 16px 40px 16px 40px;
}

/* ── Card header ─────────────────────────────────────────────────────────── */
.git-card__header {
  text-align: center;
  margin-bottom: 28px;
}

.git-card__eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 12px;
}

.git-card__title {
  font-family: var(--font-family);
  font-size: 28px;
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.01em;
  color: var(--c-ink);
  text-transform: none;
  margin: 0 0 8px;
}

.git-card__sub {
  font-family: var(--font-family);
  font-size: 14px;
  font-weight: 400;
  line-height: 1.55;
  color: var(--c-ink-2, #4a5764);
  margin: 0 0 18px;
}

/* Cyan divider — closes the header, brand accent */
.git-card__divider {
  width: 56px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  margin: 0 auto;
}

/* ── Form layout ─────────────────────────────────────────────────────────── */
.git-card__form {
  display: flex;
  flex-direction: column;
}

.git-card__field-group {
  margin-bottom: 18px;
}

.git-card__label {
  display: block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.04em;
  color: var(--c-ink);
  margin-bottom: 8px;
  text-transform: none;
}

.git-card__name-row {
  display: flex;
  gap: 12px;
}

.git-card__name-col {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
}

/* ── Inputs — taller, refined borders, cyan focus accent ─────────────────── */
.git-card__input {
  width: 100%;
  height: 52px;
  padding: 0 18px;
  border: 1.5px solid #e3e8ed;
  border-radius: 14px;
  font-family: var(--font-family);
  font-size: 14px;
  color: var(--c-ink);
  background: #fff;
  outline: none;
  transition: border-color 0.25s, box-shadow 0.25s, background 0.25s;
  box-sizing: border-box;
}

.git-card__input::placeholder {
  color: #a8b3bd;
}

.git-card__input:hover {
  border-color: #c8d3dd;
}

.git-card__input:focus {
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 4px rgba(0, 173, 239, 0.12);
  background: #fbfdff;
}

.git-card__input--full {
  width: 100%;
}

.git-card__input--error {
  border-color: var(--c-brand-red);
}

.git-card__input--error:focus {
  box-shadow: 0 0 0 4px rgba(229, 45, 39, 0.14);
}

/* ── Textarea ────────────────────────────────────────────────────────────── */
.git-card__textarea {
  width: 100%;
  padding: 14px 18px;
  border: 1.5px solid #e3e8ed;
  border-radius: 14px;
  font-family: var(--font-family);
  font-size: 14px;
  color: var(--c-ink);
  background: #fff;
  outline: none;
  resize: none;
  transition: border-color 0.25s, box-shadow 0.25s, background 0.25s;
  box-sizing: border-box;
  line-height: 1.55;
  min-height: 110px;
}

.git-card__textarea::placeholder { color: #a8b3bd; }

.git-card__textarea:hover {
  border-color: #c8d3dd;
}

.git-card__textarea:focus {
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 4px rgba(0, 173, 239, 0.12);
  background: #fbfdff;
}

/* ── Validation errors ───────────────────────────────────────────────────── */
.git-card__field-error {
  display: block;
  color: var(--c-brand-red);
  font-size: 12px;
  font-weight: 500;
  margin-top: 6px;
  padding-left: 4px;
}

/* ── Feedback ────────────────────────────────────────────────────────────── */
.git-card__feedback {
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 500;
  padding: 12px 18px;
  border-radius: 10px;
  margin-bottom: 14px;
  text-align: center;
}

.git-card__feedback--success {
  background: rgba(46, 125, 50, 0.10);
  color: #2e7d32;
  border: 1px solid rgba(46, 125, 50, 0.20);
}

.git-card__feedback--error {
  background: rgba(229, 45, 39, 0.08);
  color: var(--c-brand-red);
  border: 1px solid rgba(229, 45, 39, 0.20);
}

/* ── Submit button — solid red pill with arrow icon (brand action) ───────── */
.git-card__submit {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  width: 100%;
  height: 56px;
  margin-top: 18px;
  background: var(--c-brand-red);
  color: #fff;
  border: none;
  border-radius: 999px;
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 700;
  letter-spacing: 0.04em;
  cursor: pointer;
  box-shadow: 0 12px 28px rgba(229, 45, 39, 0.28);
  transition: background 0.25s, transform 0.25s, box-shadow 0.25s;
}

.git-card__submit:hover:not(:disabled) {
  background: var(--c-brand-crimson, #c8302a);
  transform: translateY(-2px);
  box-shadow: 0 16px 36px rgba(229, 45, 39, 0.36);
}

.git-card__submit:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

/* ── Reset link ──────────────────────────────────────────────────────────── */
.git-card__reset {
  display: block;
  width: fit-content;
  margin: 14px auto 0;
  background: none;
  border: none;
  padding: 4px 8px;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 500;
  color: var(--c-ink-2, #4a5764);
  cursor: pointer;
  transition: color 0.2s;
}

.git-card__reset:hover {
  color: var(--c-brand-blue);
  text-decoration: underline;
}

/* ── Compact field adjustments ───────────────────────────────────────────── */
.git-card--compact .git-card__input,
.git-card--compact .git-card__textarea {
  font-size: 13px;
}

.git-card--compact .git-card__title { font-size: 22px; }

/* ── Mobile ──────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .git-card,
  .git-card--compact {
    width: 100%;
    max-width: 100%;
    padding: 32px 24px 32px;
    border-radius: 16px 40px 16px 40px;
  }

  .git-card__title { font-size: 24px; }
  .git-card__sub { font-size: 13px; }

  .git-card__name-row {
    flex-direction: column;
    gap: 8px;
  }

  .git-card__input {
    height: 48px;
  }
}
</style>
