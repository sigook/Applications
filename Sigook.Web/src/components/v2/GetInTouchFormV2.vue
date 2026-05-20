<template>
  <div class="git-card" :class="`git-card--${size}`">
    <h3 class="git-card__title">Your Information</h3>
    <p class="git-card__sub">We'll reach out to you for the next steps</p>

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
              placeholder="First Name"
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
              placeholder="Last Name"
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
          placeholder="Your Email Address"
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
          placeholder="Select One"
        />
      </div>

      <!-- Message -->
      <div class="git-card__field-group">
        <label class="git-card__label">Message</label>
        <textarea
          class="git-card__textarea"
          :class="{ 'git-card__input--error': errors.message }"
          v-model="fields.message.value"
          placeholder="Tell us more about the role you need to fill"
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
        {{ submitting ? 'Sending…' : 'Save & Send' }}
      </button>

      <!-- Reset -->
      <button type="button" class="git-card__reset" @click="handleReset">
        Reset Information
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import * as yup from 'yup'
import { useStickyForm } from '@/composables/useStickyForm'
import { submitContactForm } from '@/api/websiteApi'

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
/* ── Card shell ────────────────────────────────────────────────────────────── */
.git-card {
  background: #fff;
  border-radius: 20px;
  box-shadow: var(--sh-2);
  padding: 32px 40px 40px;
  width: 550px;
  max-width: 60%;
  box-sizing: border-box;
}

/* Compact variant — narrower card for sidebar / secondary contexts */
.git-card--compact {
  width: 392px;
  padding: 28px 32px 36px;
}

/* ── Card header ───────────────────────────────────────────────────────────── */
.git-card__title {
  color: var(--c-ink);
  font-size: 22px;
  font-weight: 700;
  line-height: 1.2;
  text-align: center;
  text-transform: none;
  margin: 0 0 6px;
}

.git-card__sub {
  color: var(--c-line);
  font-size: 12px;
  line-height: 1.5;
  text-align: center;
  margin: 0 0 24px;
}

/* ── Form layout ───────────────────────────────────────────────────────────── */
.git-card__form {
  display: flex;
  flex-direction: column;
}

.git-card__field-group {
  margin-bottom: 16px;
}

.git-card__label {
  display: block;
  font-size: 13px;
  font-weight: 700;
  color: var(--c-ink);
  margin-bottom: 6px;
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

/* ── Inputs ────────────────────────────────────────────────────────────────── */
.git-card__input {
  width: 100%;
  height: 46px;
  padding: 0 16px;
  border: 1px solid #d9d9d9;
  border-radius: 12px;
  font-family: var(--font-family);
  font-size: 13px;
  color: var(--c-ink);
  background: #fff;
  outline: none;
  transition: border-color 0.2s;
  box-sizing: border-box;
}

.git-card__input::placeholder { color: #b0b0b0; }
.git-card__input:focus        { border-color: var(--c-brand-blue); }
.git-card__input--full        { width: 100%; }
.git-card__input--error       { border-color: var(--c-brand-red); }

/* ── Textarea ──────────────────────────────────────────────────────────────── */
.git-card__textarea {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #d9d9d9;
  border-radius: 12px;
  font-family: var(--font-family);
  font-size: 13px;
  color: var(--c-ink);
  background: #fff;
  outline: none;
  resize: none;
  transition: border-color 0.2s;
  box-sizing: border-box;
  line-height: 1.5;
}

.git-card__textarea::placeholder { color: #b0b0b0; }
.git-card__textarea:focus        { border-color: var(--c-brand-blue); }

/* ── Validation errors ─────────────────────────────────────────────────────── */
.git-card__field-error {
  display: block;
  color: var(--c-brand-red);
  font-size: 11px;
  margin-top: 4px;
  padding-left: 4px;
}

/* ── Feedback ──────────────────────────────────────────────────────────────── */
.git-card__feedback {
  font-size: 13px;
  padding: 10px 16px;
  border-radius: 8px;
  margin-bottom: 12px;
  text-align: center;
}

.git-card__feedback--success { background: #e8f5e9; color: #2e7d32; }
.git-card__feedback--error   { background: #fdecea; color: var(--c-brand-red); }

/* ── Submit button — RED per Figma ─────────────────────────────────────────── */
.git-card__submit {
  width: 100%;
  height: 56px;
  margin-top: 16px;
  background: var(--c-brand-red);
  color: #fff;
  border: none;
  border-radius: 28px;
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 700;
  cursor: pointer;
  transition: opacity 0.2s;
}

.git-card__submit:hover:not(:disabled) { opacity: 0.88; }
.git-card__submit:disabled             { opacity: 0.55; cursor: not-allowed; }

/* ── Reset ─────────────────────────────────────────────────────────────────── */
.git-card__reset {
  display: block;
  width: fit-content;
  margin: 12px auto 0;
  background: none;
  border: none;
  padding: 0;
  font-family: var(--font-family);
  font-size: 12px;
  color: var(--c-ink);
  cursor: pointer;
}

.git-card__reset:hover { text-decoration: underline; }

/* ── Compact field adjustments ─────────────────────────────────────────────── */
.git-card--compact .git-card__input,
.git-card--compact .git-card__textarea {
  font-size: 12px;
}

.git-card--compact .git-card__title { font-size: 18px; }

/* ── Mobile ────────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .git-card,
  .git-card--compact {
    width: 100%;
    padding: 24px 20px 32px;
  }

  .git-card__name-row {
    flex-direction: column;
    gap: 8px;
  }
}
</style>
