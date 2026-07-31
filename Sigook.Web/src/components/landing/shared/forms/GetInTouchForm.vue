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
            <b-input
              class="git-card__input"
              :class="{ 'git-card__input--error': errors.firstName }"
              v-model="fields.firstName.value"
              type="text"
              placeholder="First name"
              autocomplete="given-name"
              :has-counter="false"
            />
            <span v-if="errors.firstName" class="git-card__field-error">{{ errors.firstName }}</span>
          </div>
          <div class="git-card__name-col">
            <b-input
              class="git-card__input"
              :class="{ 'git-card__input--error': errors.lastName }"
              v-model="fields.lastName.value"
              type="text"
              placeholder="Last name"
              autocomplete="family-name"
              :has-counter="false"
            />
            <span v-if="errors.lastName" class="git-card__field-error">{{ errors.lastName }}</span>
          </div>
        </div>
      </div>

      <div class="git-card__field-group">
        <label class="git-card__label">Email</label>
        <b-input
          class="git-card__input git-card__input--full"
          :class="{ 'git-card__input--error': errors.email }"
          v-model="fields.email.value"
          type="email"
          placeholder="your.email@company.com"
          autocomplete="email"
          :has-counter="false"
        />
        <span v-if="errors.email" class="git-card__field-error">{{ errors.email }}</span>
      </div>

      <div class="git-card__field-group">
        <div class="git-card__name-row">
          <div class="git-card__name-col">
            <label class="git-card__label">Company</label>
            <b-input
              class="git-card__input"
              :class="{ 'git-card__input--error': errors.company }"
              v-model="fields.company.value"
              type="text"
              placeholder="Your company"
              autocomplete="organization"
              :has-counter="false"
            />
            <span v-if="errors.company" class="git-card__field-error">{{ errors.company }}</span>
          </div>
          <div class="git-card__name-col">
            <label class="git-card__label">Location</label>
            <b-select
              class="git-card__input git-card__select"
              :class="{
                'git-card__input--error': errors.state,
                'git-card__select--placeholder': !fields.state.value,
              }"
              v-model="fields.state.value"
              expanded
            >
              <option value="" disabled>Select a state</option>
              <option v-for="state in states" :key="state.id" :value="state.value">
                {{ state.value }}
              </option>
            </b-select>
            <span v-if="errors.state" class="git-card__field-error">{{ errors.state }}</span>
          </div>
        </div>
      </div>

      <div class="git-card__field-group">
        <label class="git-card__label">Industry</label>
        <b-input
          class="git-card__input git-card__input--full"
          v-model="fields.industry.value"
          type="text"
          placeholder="Select one"
          :has-counter="false"
        />
      </div>

      <div class="git-card__field-group">
        <label class="git-card__label">Message</label>
        <b-input
          class="git-card__textarea"
          :class="{ 'git-card__input--error': errors.message }"
          v-model="fields.message.value"
          type="textarea"
          placeholder="Tell us more about the role you need to fill."
          :has-counter="false"
        />
        <span v-if="errors.message" class="git-card__field-error">{{ errors.message }}</span>
      </div>

      <div class="git-card__recaptcha">
        <Vue3Recaptcha2
          v-if="showRecaptcha"
          :sitekey="siteKey"
          @verify="handleCaptchaVerify"
          @expire="handleCaptchaExpired"
          @fail="handleCaptchaError"
        />
        <span v-if="captchaError" class="git-card__field-error">Please verify you are human.</span>
      </div>

      <div v-if="submitted" class="git-card__feedback git-card__feedback--success">
        Thank you! We'll reach out to you shortly.
      </div>
      <div v-if="submitError" class="git-card__feedback git-card__feedback--error">
        {{ submitError }}
      </div>

      <b-button native-type="submit" class="git-card__submit" :disabled="submitting">
        <span>{{ submitting ? 'Sending…' : 'Save & Send' }}</span>
        <ArrowIcon v-if="!submitting" :width="32" :height="11" :stroke-width="1.5" color="#fff" />
      </b-button>

      <b-button native-type="button" class="git-card__reset" @click="handleReset">
        Reset information
      </b-button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Vue3Recaptcha2 from 'vue3-recaptcha2'
import * as yup from 'yup'
import { useStickyForm } from '@/composables/useStickyForm'
import { submitContactForm } from '@/api/websiteApi'
import { getCountries, getProvinces } from '@/api/locationApi'
import type { Province } from '@/types/common'
import ArrowIcon from '@/components/landing/shared/icons/ArrowIcon.vue'

withDefaults(defineProps<{ size?: 'default' | 'compact' }>(), { size: 'default' })

const schema = yup.object({
  firstName: yup.string().required('First name is required').max(50, 'Max 50 characters'),
  lastName:  yup.string().required('Last name is required').max(50, 'Max 50 characters'),
  email:     yup.string().required('Email is required').email('Invalid email address').max(100),
  company:   yup.string().required('Company is required').max(100, 'Max 100 characters'),
  industry:  yup.string().max(100).optional(),
  state:     yup.string().required('State is required'),
  message:   yup.string().required('Message is required').max(500, 'Max 500 characters'),
})

const { fields, errors, validate, markInteracted, resetAll } = useStickyForm({
  schema,
  initialValues: { firstName: '', lastName: '', email: '', company: '', industry: '', state: '', message: '' },
})

const submitting  = ref(false)
const submitted   = ref(false)
const submitError = ref('')

const siteKey       = import.meta.env.VUE_APP_RE_CAPTCHA_SITE_KEY
const captchaToken  = ref<string | null>(null)
const captchaError  = ref(false)
const showRecaptcha = ref(true)

const states = ref<Province[]>([])

onMounted(async () => {
  try {
    const countries = await getCountries()
    const usa = countries.find((c) => c.code === 'USA')
    if (usa) states.value = await getProvinces(String(usa.id))
  } catch {
    states.value = []
  }
})

function handleCaptchaVerify(token: string) {
  captchaToken.value = token
  captchaError.value = false
}

function handleCaptchaExpired() {
  captchaToken.value = null
}

function handleCaptchaError() {
  captchaToken.value = null
  captchaError.value = true
}

function resetCaptcha() {
  captchaToken.value  = null
  captchaError.value  = false
  showRecaptcha.value = false
  setTimeout(() => { showRecaptcha.value = true }, 100)
}

async function handleFormSubmit() {
  markInteracted()
  const result = await validate()
  if (!result.valid) return

  if (!captchaToken.value) {
    captchaError.value = true
    return
  }

  submitting.value  = true
  submitError.value = ''
  submitted.value   = false

  try {
    await submitContactForm({
      title:           'Get in Touch',
      name:            `${fields.firstName.value} ${fields.lastName.value}`.trim(),
      email:           fields.email.value,
      phone:           '',
      company:         fields.company.value,
      location:        fields.state.value,
      message:         fields.message.value,
      subject:         fields.industry.value || '',
      captchaResponse: captchaToken.value,
    })
    submitted.value = true
    resetAll()
  } catch {
    submitError.value = 'Something went wrong. Please try again later.'
  } finally {
    resetCaptcha()
    submitting.value = false
  }
}

function handleReset() {
  resetAll()
  resetCaptcha()
  submitted.value   = false
  submitError.value = ''
}
</script>

<style scoped>
.git-card {
  position: relative;
  background: #fff;
  border-radius: 20px 56px 20px 56px;
  box-shadow:
    0 24px 60px rgba(15, 47, 68, 0.20),
    0 4px 12px rgba(15, 47, 68, 0.06);
  padding: 44px 48px 44px;
  width: 550px;
  max-width: 60%;
  box-sizing: border-box;
}

.git-card--compact {
  width: 392px;
  padding: 32px 32px 36px;
  border-radius: 16px 40px 16px 40px;
}

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

.git-card__divider {
  width: 56px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  margin: 0 auto;
}

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

.git-card__input :deep(.input),
.git-card__select :deep(.select select),
.git-card__textarea :deep(.textarea) {
  width: 100%;
  max-width: 100%;
  border: 1.5px solid var(--c-input-border);
  border-radius: 14px;
  font-family: var(--font-family);
  font-size: 14px;
  color: var(--c-ink);
  background-color: #fff;
  outline: none;
  box-shadow: none;
  transition: border-color 0.25s, box-shadow 0.25s, background-color 0.25s;
  box-sizing: border-box;
}

.git-card__input :deep(.input),
.git-card__select :deep(.select select) {
  height: 52px;
  padding: 0 18px;
}

.git-card__input :deep(.control),
.git-card__select :deep(.control),
.git-card__select :deep(.select),
.git-card__textarea :deep(.control) {
  width: 100%;
  max-width: 100%;
  height: auto;
}

.git-card__input :deep(.input)::placeholder,
.git-card__textarea :deep(.textarea)::placeholder {
  color: var(--c-input-placeholder);
}

.git-card__input :deep(.input):hover,
.git-card__select :deep(.select select:hover),
.git-card__textarea :deep(.textarea:hover) {
  border-color: var(--c-input-border-hover);
}

.git-card__input :deep(.input):focus,
.git-card__select :deep(.select select:focus),
.git-card__textarea :deep(.textarea:focus) {
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 4px rgba(0, 173, 239, 0.12);
  background-color: var(--c-input-focus-bg);
}

.git-card__input--error :deep(.input),
.git-card__input--error :deep(.select select),
.git-card__input--error :deep(.textarea) {
  border-color: var(--c-brand-red);
}

.git-card__input--error :deep(.input):focus,
.git-card__input--error :deep(.select select:focus),
.git-card__input--error :deep(.textarea:focus) {
  box-shadow: 0 0 0 4px rgba(229, 45, 39, 0.14);
}

.git-card__select :deep(.select:not(.is-multiple):not(.is-loading)::after) {
  display: none;
}

.git-card__select :deep(.select select) {
  appearance: none;
  cursor: pointer;
  padding-right: 42px;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='8' viewBox='0 0 12 8'%3E%3Cpath fill='none' stroke='%234a5764' stroke-width='1.6' stroke-linecap='round' stroke-linejoin='round' d='M1 1.5 6 6.5 11 1.5'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 16px center;
}

.git-card__select--placeholder :deep(.select select) {
  color: var(--c-input-placeholder);
}

.git-card__textarea :deep(.textarea) {
  padding: 14px 18px;
  resize: none;
  line-height: 1.55;
  min-height: 110px;
  height: auto;
}

.git-card__field-error {
  display: block;
  color: var(--c-brand-red);
  font-size: 12px;
  font-weight: 500;
  margin-top: 6px;
  padding-left: 4px;
}

.git-card__recaptcha {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 18px;
}

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

.git-card__submit {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  width: 100%;
  height: 56px;
  margin-top: 18px;
  padding: 0;
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

.git-card__submit > :deep(span) {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
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

.git-card__reset {
  display: block;
  width: fit-content;
  height: auto;
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

.git-card--compact .git-card__input :deep(.input),
.git-card--compact .git-card__textarea :deep(.textarea) {
  font-size: 13px;
}

.git-card--compact .git-card__title { font-size: 22px; }

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

  .git-card__input :deep(.input),
  .git-card__select :deep(.select select) {
    height: 48px;
  }
}
</style>
