<template>
  <section class="contact-v2">
    <!-- Decorative circles (inline SVG — solid cyan + white outline) -->
    <div class="contact-v2__deco-1" aria-hidden="true">
      <svg viewBox="0 0 851 851" xmlns="http://www.w3.org/2000/svg" class="contact-v2__deco-svg">
        <circle cx="425.5" cy="425.5" r="425.5" fill="#00ADEF" fill-opacity="0.35" />
      </svg>
    </div>
    <div class="contact-v2__deco-2" aria-hidden="true">
      <svg viewBox="0 0 848 848" xmlns="http://www.w3.org/2000/svg" class="contact-v2__deco-svg">
        <circle cx="424" cy="424" r="423" stroke="white" stroke-width="2" fill="none" stroke-opacity="0.4" />
      </svg>
    </div>

    <div class="contact-v2__inner">
      <!-- Above-card heading -->
      <h1 class="contact-v2__title">Get in Touch!</h1>
      <p class="contact-v2__lead">Your next hire starts here</p>

      <!-- White form card -->
      <div class="contact-v2__card">
        <h3 class="contact-v2__card-title">Your Information</h3>
        <p class="contact-v2__card-sub">We'll reach out to you for the next steps</p>

        <form class="contact-v2__form" @submit.prevent="handleFormSubmit" novalidate>
          <!-- Name: two side-by-side inputs -->
          <div class="contact-v2__field-group">
            <label class="contact-v2__label">Name</label>
            <div class="contact-v2__name-row">
              <div class="contact-v2__name-col">
                <input
                  class="contact-v2__input"
                  :class="{ 'contact-v2__input--error': errors.firstName }"
                  v-model="fields.firstName.value"
                  type="text"
                  placeholder="First Name"
                  autocomplete="given-name"
                />
                <span v-if="errors.firstName" class="contact-v2__field-error">{{ errors.firstName }}</span>
              </div>
              <div class="contact-v2__name-col">
                <input
                  class="contact-v2__input"
                  :class="{ 'contact-v2__input--error': errors.lastName }"
                  v-model="fields.lastName.value"
                  type="text"
                  placeholder="Last Name"
                  autocomplete="family-name"
                />
                <span v-if="errors.lastName" class="contact-v2__field-error">{{ errors.lastName }}</span>
              </div>
            </div>
          </div>

          <!-- Email -->
          <div class="contact-v2__field-group">
            <label class="contact-v2__label">Email</label>
            <input
              class="contact-v2__input contact-v2__input--full"
              :class="{ 'contact-v2__input--error': errors.email }"
              v-model="fields.email.value"
              type="email"
              placeholder="Your Email Address"
              autocomplete="email"
            />
            <span v-if="errors.email" class="contact-v2__field-error">{{ errors.email }}</span>
          </div>

          <!-- Industry -->
          <div class="contact-v2__field-group">
            <label class="contact-v2__label">Industry</label>
            <input
              class="contact-v2__input contact-v2__input--full"
              v-model="fields.industry.value"
              type="text"
              placeholder="Select One"
            />
          </div>

          <!-- Message -->
          <div class="contact-v2__field-group">
            <label class="contact-v2__label">Message</label>
            <textarea
              class="contact-v2__textarea"
              :class="{ 'contact-v2__input--error': errors.message }"
              v-model="fields.message.value"
              placeholder="Tell us more about the role you need to fill"
              rows="5"
            ></textarea>
            <span v-if="errors.message" class="contact-v2__field-error">{{ errors.message }}</span>
          </div>

          <!-- Feedback messages -->
          <div v-if="submitted" class="contact-v2__feedback contact-v2__feedback--success">
            Thank you! We'll reach out to you shortly.
          </div>
          <div v-if="submitError" class="contact-v2__feedback contact-v2__feedback--error">
            {{ submitError }}
          </div>

          <!-- Submit button -->
          <button
            type="submit"
            class="btn btn--primary contact-v2__submit"
            :disabled="submitting"
          >
            {{ submitting ? 'Sending…' : 'Save & Send' }}
          </button>

          <!-- Reset link -->
          <button type="button" class="contact-v2__reset" @click="handleReset">
            Reset Information
          </button>
        </form>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import * as yup from 'yup'
import { useStickyForm } from '@/composables/useStickyForm'
import { submitContactForm } from '@/api/websiteApi'

// ── Validation schema ────────────────────────────────────────────────────────
const schema = yup.object({
  firstName: yup.string().required('First name is required').max(50, 'Max 50 characters'),
  lastName:  yup.string().required('Last name is required').max(50, 'Max 50 characters'),
  email:     yup.string().required('Email is required').email('Invalid email address').max(100),
  industry:  yup.string().max(100).optional(),
  message:   yup.string().required('Message is required').max(500, 'Max 500 characters'),
})

// ── Form state ────────────────────────────────────────────────────────────────
const { fields, errors, validate, markInteracted, resetAll } = useStickyForm({
  schema,
  initialValues: { firstName: '', lastName: '', email: '', industry: '', message: '' },
})

const submitting  = ref(false)
const submitted   = ref(false)
const submitError = ref('')

// ── Handlers ──────────────────────────────────────────────────────────────────
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
/* ── Section shell ─────────────────────────────────────────────────────────── */
.contact-v2 {
  position: relative;
  overflow: hidden;
  background: var(--c-brand-blue);
  border-radius: 0 0 20px 20px;
  min-height: 960px;
}

/* ── Decorative circles ────────────────────────────────────────────────────── */
.contact-v2__deco-1 {
  position: absolute;
  top: -80px;
  left: 244px;
  width: 851px;
  height: 851px;
  pointer-events: none;
  z-index: 0;
}

.contact-v2__deco-2 {
  position: absolute;
  top: -90px;
  left: 282px;
  width: 848px;
  height: 848px;
  pointer-events: none;
  z-index: 0;
}

.contact-v2__deco-svg {
  width: 100%;
  height: 100%;
}

/* ── Inner container ───────────────────────────────────────────────────────── */
.contact-v2__inner {
  position: relative;
  z-index: 1;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 80px var(--gutter-desktop) 80px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

/* ── Above-card heading ────────────────────────────────────────────────────── */
.contact-v2__title {
  color: var(--c-bg);
  font-size: 40px;
  font-weight: 700;
  line-height: 1.1;
  text-align: center;
  text-transform: none;
  margin: 0 0 8px;
}

.contact-v2__lead {
  color: var(--c-bg);
  font-size: 18px;
  font-weight: 500;
  line-height: 1.5;
  text-align: center;
  margin: 0 0 32px;
  opacity: 0.9;
}

/* ── Form card ─────────────────────────────────────────────────────────────── */
.contact-v2__card {
  background: var(--c-bg);
  border-radius: 20px;
  box-shadow: var(--sh-2);
  width: 550px;
  max-width: 100%;
  padding: 32px 40px 40px;
}

.contact-v2__card-title {
  color: var(--c-ink);
  font-size: 22px;
  font-weight: 700;
  line-height: 1.2;
  text-align: center;
  text-transform: none;
  margin: 0 0 6px;
}

.contact-v2__card-sub {
  color: var(--c-line);
  font-size: 12px;
  line-height: 1.5;
  text-align: center;
  margin: 0 0 24px;
}

/* ── Form layout ───────────────────────────────────────────────────────────── */
.contact-v2__form {
  display: flex;
  flex-direction: column;
  gap: 0;
}

.contact-v2__field-group {
  margin-bottom: 16px;
}

.contact-v2__label {
  display: block;
  font-size: 15px;
  font-weight: 700;
  color: var(--c-ink);
  margin-bottom: 6px;
}

/* Name: side-by-side inputs */
.contact-v2__name-row {
  display: flex;
  gap: 12px;
}

.contact-v2__name-col {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
}

/* ── Input / Textarea ──────────────────────────────────────────────────────── */
.contact-v2__input {
  width: 100%;
  height: 39px;
  padding: 0 16px;
  border: 1px solid var(--c-line);
  border-radius: 20px;
  font-family: var(--font-family);
  font-size: 12px;
  color: var(--c-ink);
  background: var(--c-bg);
  outline: none;
  transition: border-color 0.2s;
  box-sizing: border-box;
}

.contact-v2__input::placeholder {
  color: var(--c-line);
}

.contact-v2__input:focus {
  border-color: var(--c-brand-blue);
}

.contact-v2__input--full {
  width: 100%;
}

.contact-v2__input--error {
  border-color: var(--c-brand-red);
}

.contact-v2__textarea {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid var(--c-line);
  border-radius: 20px;
  font-family: var(--font-family);
  font-size: 12px;
  color: var(--c-ink);
  background: var(--c-bg);
  outline: none;
  resize: none;
  transition: border-color 0.2s;
  box-sizing: border-box;
  line-height: 1.5;
}

.contact-v2__textarea::placeholder {
  color: var(--c-line);
}

.contact-v2__textarea:focus {
  border-color: var(--c-brand-blue);
}

/* ── Validation errors ─────────────────────────────────────────────────────── */
.contact-v2__field-error {
  display: block;
  color: var(--c-brand-red);
  font-size: 11px;
  margin-top: 4px;
  padding-left: 8px;
}

/* ── Feedback messages ─────────────────────────────────────────────────────── */
.contact-v2__feedback {
  font-size: 13px;
  padding: 10px 16px;
  border-radius: var(--r-md);
  margin-bottom: 12px;
  text-align: center;
}

.contact-v2__feedback--success {
  background: #e8f5e9;
  color: #2e7d32;
}

.contact-v2__feedback--error {
  background: #fdecea;
  color: var(--c-brand-red);
}

/* ── Submit button ─────────────────────────────────────────────────────────── */
.contact-v2__submit {
  width: 100%;
  margin-top: 16px;
  border-radius: 20px !important;
  height: 39px;
  font-size: 15px;
  font-weight: 700;
}

.contact-v2__submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* ── Reset link ────────────────────────────────────────────────────────────── */
.contact-v2__reset {
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
  text-decoration: none;
  line-height: 1.5;
}

.contact-v2__reset:hover {
  text-decoration: underline;
}

/* ── Mobile ────────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .contact-v2__inner {
    padding: 60px var(--gutter-mobile) 60px;
  }

  .contact-v2__deco-1,
  .contact-v2__deco-2 {
    width: 500px;
    height: 500px;
    left: -100px;
  }

  .contact-v2__card {
    width: 100%;
    padding: 24px 20px 32px;
  }

  .contact-v2__name-row {
    flex-direction: column;
    gap: 8px;
  }

  .contact-v2__title {
    font-size: 30px;
  }
}
</style>
