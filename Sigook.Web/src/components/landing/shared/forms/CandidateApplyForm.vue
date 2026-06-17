<template>
  <div class="reg-form" :class="{ 'reg-form--loading': isLoading }">
    <header class="reg-form__head">
      <p v-if="contextLine" class="reg-form__context">
        <span class="reg-form__context-label">Applying for:</span>
        <span class="reg-form__context-value">{{ contextLine }}</span>
      </p>
      <WorkerRegisterStepNav :steps="STEPS" :current="currentStepIdx" />
    </header>

    <div v-if="isLoading" class="reg-form__loader" aria-live="polite">
      <span class="reg-form__spinner" aria-hidden="true"></span>
      <span>{{ loadingText }}</span>
    </div>

    <form v-else class="reg-form__body" @submit.prevent="onSubmit">
      <!-- ── Step 1 — Personal Information ───────────────────────────────── -->
      <section v-show="currentStep === 'personal'" class="reg-form__step">
        <h3 class="reg-form__step-title">Personal information</h3>

        <Input
          v-model="fullName"
          label="Full Name"
          placeholder="Your full name"
          :required="true"
          :maxlength="60"
          :error="errors.fullName"
          autocomplete="name"
        />

        <div class="reg-form__grid reg-form__grid--2col">
          <Input
            v-model="email"
            label="Email"
            type="email"
            placeholder="you@email.com"
            :required="true"
            :maxlength="100"
            :error="errors.email"
            autocomplete="email"
          />
          <PhoneInput
            v-model="phone"
            label="Phone"
            placeholder="305 123-4567"
            :required="true"
            :error="errors.phone"
          />
        </div>

        <div class="reg-form__grid reg-form__grid--2col">
          <Select
            v-model="selectedCountry"
            :options="countries"
            label="Country"
            placeholder="Select country"
            :required="true"
            :error="errors.countryId"
          />
          <Input
            v-model="address"
            label="City / Address"
            placeholder="City / Address"
            :required="true"
            :maxlength="100"
            :error="errors.address"
          />
        </div>

        <footer class="reg-form__nav">
          <span></span>
          <button type="button" class="reg-form__btn reg-form__btn--primary" @click="goNext">
            <span>Next</span>
            <span class="reg-form__btn-arrow" aria-hidden="true">→</span>
          </button>
        </footer>
      </section>

      <!-- ── Step 2 — Additional Details ─────────────────────────────────── -->
      <section v-show="currentStep === 'details'" class="reg-form__step">
        <h3 class="reg-form__step-title">Additional details</h3>

        <div class="reg-form__grid reg-form__grid--2col">
          <Select
            v-model="selectedStatus"
            :options="statusOptions"
            label="Immigration Status"
            placeholder="Select status"
            :required="true"
            :error="errors.status"
          />
          <Select
            v-model="selectedSource"
            :options="sources"
            label="How did you hear about us?"
            placeholder="Select an option"
          />
        </div>

        <div class="reg-form__doc-block">
          <div class="reg-form__doc-head">
            <span class="reg-form__doc-title">
              Resume / CV
              <span v-if="resumeRequired" class="reg-form__doc-required">*</span>
              <span v-else class="reg-form__doc-optional">(optional)</span>
            </span>
            <FileUpload
              label="Add resume"
              accept=".pdf,.doc,.docx"
              :disabled="!!resume"
              @file="handleResumeUpload"
            />
          </div>
          <div v-if="resume" class="reg-form__doc-card">
            <div class="reg-form__doc-card-head">
              <span class="reg-form__doc-filename">📑 {{ resume.name }}</span>
              <button type="button" class="reg-form__doc-remove" @click="deleteResume">Remove</button>
            </div>
          </div>
          <p v-if="resumeError" class="reg-form__doc-error">{{ resumeError }}</p>
        </div>

        <TagInput
          v-model="skillTags"
          :options="filteredSkills"
          label="Skills / Roles of Interest"
          placeholder="Select from suggestions or type your own"
          option-key="id"
          option-label="value"
          :allow-new="true"
          :create-tag="createSkillTag"
          @typing="onSkillsTyping"
        />

        <div class="reg-form__field-group reg-form__field-group--inline">
          <span class="reg-form__field-group-label">Transportation</span>
          <Switch v-model="hasVehicle">{{ hasVehicle ? 'Own Vehicle' : 'Public Transit' }}</Switch>
        </div>

        <footer class="reg-form__nav">
          <button type="button" class="reg-form__btn reg-form__btn--ghost" @click="goPrev">
            <span class="reg-form__btn-arrow reg-form__btn-arrow--left" aria-hidden="true">←</span>
            <span>Previous</span>
          </button>
          <button type="button" class="reg-form__btn reg-form__btn--primary" @click="goNext">
            <span>Next</span>
            <span class="reg-form__btn-arrow" aria-hidden="true">→</span>
          </button>
        </footer>
      </section>

      <!-- ── Step 3 — Review & Submit ────────────────────────────────────── -->
      <section v-show="currentStep === 'review'" class="reg-form__step">
        <h3 class="reg-form__step-title">Review &amp; submit</h3>

        <dl class="reg-form__summary">
          <div class="reg-form__summary-row">
            <dt>Name</dt>
            <dd>{{ fullName || '—' }}</dd>
          </div>
          <div class="reg-form__summary-row">
            <dt>Email</dt>
            <dd>{{ email || '—' }}</dd>
          </div>
          <div class="reg-form__summary-row">
            <dt>Phone</dt>
            <dd>{{ phone || '—' }}</dd>
          </div>
          <div class="reg-form__summary-row">
            <dt>Country</dt>
            <dd>{{ selectedCountry?.value || '—' }}</dd>
          </div>
          <div class="reg-form__summary-row">
            <dt>City / Address</dt>
            <dd>{{ address || '—' }}</dd>
          </div>
          <div class="reg-form__summary-row">
            <dt>Immigration Status</dt>
            <dd>{{ selectedStatus?.value || '—' }}</dd>
          </div>
          <div class="reg-form__summary-row">
            <dt>How did you hear about us</dt>
            <dd>{{ selectedSource?.value || '—' }}</dd>
          </div>
          <div class="reg-form__summary-row">
            <dt>Transportation</dt>
            <dd>{{ hasVehicle ? 'Own Vehicle' : 'Public Transit' }}</dd>
          </div>
          <div v-if="skillTags.length" class="reg-form__summary-row">
            <dt>Skills</dt>
            <dd>
              <span v-for="tag in skillTags" :key="tag.id || tag.value" class="reg-form__summary-chip">
                {{ tag.value }}
              </span>
            </dd>
          </div>
          <div v-if="resume" class="reg-form__summary-row">
            <dt>Resume</dt>
            <dd>{{ resume.name }}</dd>
          </div>
        </dl>

        <Checkbox v-model="termsAccepted">
          I agree to Sigook™
          <router-link to="/terms-and-conditions" target="_blank" class="reg-form__link">Terms and Conditions</router-link>
          &amp;
          <router-link to="/privacy-policy" target="_blank" class="reg-form__link">Privacy Policy</router-link>.
        </Checkbox>
        <p v-if="errors.termsAccepted" class="reg-form__field-error">{{ errors.termsAccepted }}</p>

        <footer class="reg-form__nav">
          <button type="button" class="reg-form__btn reg-form__btn--ghost" @click="goPrev">
            <span class="reg-form__btn-arrow reg-form__btn-arrow--left" aria-hidden="true">←</span>
            <span>Previous</span>
          </button>
          <button type="submit" class="reg-form__btn reg-form__btn--submit" :disabled="isSubmitting">
            <span>{{ isSubmitting ? 'Submitting…' : 'Submit Application' }}</span>
            <span class="reg-form__btn-arrow" aria-hidden="true">→</span>
          </button>
        </footer>
      </section>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { useForm, useField } from 'vee-validate'
import * as yup from 'yup'

import Input from '@/components/landing/shared/forms/Input.vue'
import Select from '@/components/landing/shared/forms/Select.vue'
import PhoneInput from '@/components/landing/shared/forms/PhoneInput.vue'
import FileUpload from '@/components/landing/shared/forms/FileUpload.vue'
import TagInput from '@/components/landing/shared/forms/TagInput.vue'
import Switch from '@/components/landing/shared/forms/Switch.vue'
import Checkbox from '@/components/landing/shared/forms/Checkbox.vue'
import WorkerRegisterStepNav, { type StepDescriptor } from '@/components/landing/shared/forms/WorkerRegisterStepNav.vue'

import { getSources, getSkills } from '@/api/catalogApi'
import { getCountries } from '@/api/locationApi'
import type { Country, Source, Skill } from '@/types/common'
import { showAlertError, showAlertSuccess } from '@/utils/toast'
import {
  RESIDENCY_STATUS,
  submitCandidateApplication,
  type CandidateFormData,
} from '@/components/landing/shared/forms/candidateApplyService'

/* ── Props / emits ──────────────────────────────────────────────────────── */

const props = withDefaults(defineProps<{
  /** Optional job title shown in the context banner ("Applying for: X"). */
  jobTitle?: string
  /** Backend Request id the application should attach to, if any. */
  requestId?: string
  /** Kept for API parity with WorkerRegisterForm; modal usage passes false. */
  redirectOnSuccess?: boolean
}>(), {
  redirectOnSuccess: false,
})

const emit = defineEmits<{
  (e: 'submitted'): void
}>()

const contextLine = computed(() => props.jobTitle || '')

/* ── Step machine ───────────────────────────────────────────────────────── */

type StepKey = 'personal' | 'details' | 'review'
const STEPS: readonly StepDescriptor[] = [
  { key: 'personal', label: 'Personal' },
  { key: 'details', label: 'Details' },
  { key: 'review', label: 'Review' },
] as const

const currentStep = ref<StepKey>('personal')
const currentStepIdx = computed(() => STEPS.findIndex((s) => s.key === currentStep.value))

/* ── Validation (mirrors Covenant useApplicationForm) ───────────────────── */

const schema = yup.object({
  fullName: yup.string().required('Full name is required').max(60, 'Full name must be at most 60 characters'),
  email: yup
    .string()
    .required('Email is required')
    .email('Invalid email format')
    .min(6, 'Email must be at least 6 characters')
    .max(100, 'Email must be at most 100 characters'),
  phone: yup
    .string()
    .required('Phone is required')
    .matches(/^\d{3} \d{3}-\d{4}$/, 'Phone format must be: ### ###-####'),
  countryId: yup.string().required('Country is required'),
  address: yup.string().required('City/Address is required').max(100, 'Address must be at most 100 characters'),
  status: yup.string().required('Immigration status is required'),
  termsAccepted: yup.boolean().oneOf([true], 'You must accept the terms and conditions'),
})

const { errors: formErrors, validateField } = useForm({
  validationSchema: schema,
  initialValues: {
    fullName: '',
    email: '',
    phone: '',
    countryId: '',
    address: '',
    status: '',
    termsAccepted: false,
  },
})

const { value: fullName } = useField<string>('fullName')
const { value: email } = useField<string>('email')
const { value: phone } = useField<string>('phone')
const { value: countryId } = useField<string>('countryId')
const { value: address } = useField<string>('address')
const { value: status } = useField<string>('status')
const { value: termsAccepted } = useField<boolean>('termsAccepted')

/* "Show error only after the user has touched the field" */
const interacted = reactive<Record<string, boolean>>({})
function markInteracted(fields: string[]): void {
  for (const f of fields) interacted[f] = true
}

const watched: Array<[string, { value: unknown }]> = [
  ['fullName', { value: fullName }],
  ['email', { value: email }],
  ['phone', { value: phone }],
  ['countryId', { value: countryId }],
  ['address', { value: address }],
  ['status', { value: status }],
  ['termsAccepted', { value: termsAccepted }],
]
for (const [name, ref_] of watched) {
  watch(() => (ref_ as { value: unknown }).value, () => { interacted[name] = true })
}

const errors = computed<Record<string, string>>(() => {
  const out: Record<string, string> = {}
  const ferrs = formErrors.value as Record<string, string | undefined>
  for (const k of Object.keys(ferrs)) {
    out[k] = interacted[k] ? (ferrs[k] || '') : ''
  }
  return out
})

/* ── Object-bound selects (sync into validated string fields) ───────────── */

interface StatusOption { id: string; value: string }
const statusOptions: StatusOption[] = RESIDENCY_STATUS.map((s) => ({ id: s, value: s }))

const countries = ref<Country[]>([])
const sources = ref<Source[]>([])

const selectedCountry = ref<Country | null>(null)
const selectedStatus = ref<StatusOption | null>(null)
const selectedSource = ref<Source | null>(null)

watch(selectedCountry, (c) => { countryId.value = c?.id ?? '' })
watch(selectedStatus, (s) => { status.value = s?.id ?? '' })

const resumeRequired = computed(() => selectedSource.value?.value === 'Linkedin')

/* ── Skills (TagInput with object tags) ─────────────────────────────────── */

const allSkills = ref<Skill[]>([])
const skillTags = ref<Skill[]>([])
const skillQuery = ref('')

const filteredSkills = computed<Skill[]>(() => {
  const q = skillQuery.value.trim().toLowerCase()
  if (!q) return allSkills.value
  return allSkills.value.filter((s) => s.value.toLowerCase().includes(q))
})

function onSkillsTyping(value: string): void {
  skillQuery.value = value
}

function createSkillTag(raw: string): Skill {
  const value = raw.slice(0, 20)
  return { id: value, value }
}

/* ── Vehicle + resume ───────────────────────────────────────────────────── */

const hasVehicle = ref(false)
const resume = ref<File | null>(null)
const resumeError = ref('')

function handleResumeUpload(file: File | null): void {
  if (!file) return
  if (file.size / 1024 > 15_500) {
    showAlertError('File exceeds 15MB limit')
    return
  }
  resume.value = file
  resumeError.value = ''
}

function deleteResume(): void {
  resume.value = null
}

/* ── Step navigation + validation ───────────────────────────────────────── */

async function validateStepPersonal(): Promise<boolean> {
  const fields = ['fullName', 'email', 'phone', 'countryId', 'address']
  markInteracted(fields)
  const results = await Promise.all(fields.map((f) => validateField(f as never)))
  return results.every((r) => r.valid)
}

async function validateStepDetails(): Promise<boolean> {
  markInteracted(['status'])
  const result = await validateField('status' as never)
  let valid = result.valid
  if (resumeRequired.value && !resume.value) {
    resumeError.value = 'Resume is required when you apply via LinkedIn'
    valid = false
  } else {
    resumeError.value = ''
  }
  return valid
}

async function validateAndAdvance(): Promise<void> {
  let valid = false
  if (currentStep.value === 'personal') valid = await validateStepPersonal()
  else if (currentStep.value === 'details') valid = await validateStepDetails()
  if (!valid) {
    showAlertError('Please make sure all required fields are filled out correctly')
    return
  }
  const idx = currentStepIdx.value
  if (idx < STEPS.length - 1) {
    currentStep.value = STEPS[idx + 1].key as StepKey
  }
}

function goNext(): void {
  void validateAndAdvance()
}

function goPrev(): void {
  const idx = currentStepIdx.value
  if (idx > 0) currentStep.value = STEPS[idx - 1].key as StepKey
}

/* ── Submit ─────────────────────────────────────────────────────────────── */

const isLoading = ref(true)
const loadingText = ref('Loading…')
const isSubmitting = ref(false)

async function onSubmit(): Promise<void> {
  markInteracted(['termsAccepted'])
  const result = await validateField('termsAccepted' as never)
  if (!result.valid) {
    showAlertError('You must accept the terms and conditions')
    return
  }
  if (resumeRequired.value && !resume.value) {
    resumeError.value = 'Resume is required when you apply via LinkedIn'
    currentStep.value = 'details'
    showAlertError('Resume is required when you apply via LinkedIn')
    return
  }

  isSubmitting.value = true

  const payload: CandidateFormData = {
    fullName: fullName.value,
    email: email.value,
    phone: phone.value,
    countryId: countryId.value,
    address: address.value,
    status: (status.value as CandidateFormData['status']) || '',
    sourceId: selectedSource.value?.id ?? null,
    skills: skillTags.value.map((s) => s.value),
    hasVehicle: hasVehicle.value,
    resume: resume.value,
    termsAccepted: termsAccepted.value,
  }

  try {
    await submitCandidateApplication(payload, props.requestId)
    showAlertSuccess('Your application has been submitted')
    emit('submitted')
  } catch (err: unknown) {
    showAlertError((err as { data?: string })?.data ?? 'Something went wrong')
  } finally {
    isSubmitting.value = false
  }
}

/* ── Catalog loading ────────────────────────────────────────────────────── */

onMounted(async () => {
  try {
    const [countryList, sourceList, skillList] = await Promise.all([
      getCountries(),
      getSources(),
      getSkills(),
    ])
    countries.value = countryList
    sources.value = sourceList
    allSkills.value = skillList
    // Default the source to the "Sigook" entry (this form lives on the Sigook site).
    const defaultSource = sourceList.find((s) => s.value === 'Sigook')
    if (defaultSource) selectedSource.value = defaultSource
  } catch (err) {
    // eslint-disable-next-line no-console
    console.warn('[CandidateApplyForm] catalog load failed:', err)
  } finally {
    isLoading.value = false
  }
})
</script>

<style scoped>
.reg-form {
  position: relative;
  width: 100%;
  font-family: var(--font-family);
  color: #fff;
}

.reg-form__head {
  display: flex;
  flex-direction: column;
  gap: clamp(14px, 2vw, 22px);
  margin-bottom: clamp(24px, 3vw, 36px);
}

.reg-form__context {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  background: rgba(0, 173, 239, 0.10);
  border: 1px solid rgba(0, 173, 239, 0.40);
  border-radius: 999px;
  font-size: 12px;
  align-self: flex-start;
  margin: 0;
}

.reg-form__context-label {
  font-weight: 700;
  letter-spacing: 0.14em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
}

.reg-form__context-value {
  color: #fff;
  font-weight: 600;
}

.reg-form__loader {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 60px 0;
  color: rgba(255, 255, 255, 0.75);
  font-size: 14px;
}

.reg-form__spinner {
  width: 22px;
  height: 22px;
  border: 2.5px solid rgba(0, 173, 239, 0.25);
  border-top-color: var(--c-brand-cyan);
  border-radius: 50%;
  animation: reg-spin 0.8s linear infinite;
}

@keyframes reg-spin {
  to { transform: rotate(360deg); }
}

.reg-form__body {
  display: flex;
  flex-direction: column;
}

.reg-form__step {
  display: flex;
  flex-direction: column;
  gap: clamp(18px, 2.4vw, 26px);
  animation: reg-step-in 0.35s ease;
}

@keyframes reg-step-in {
  from { opacity: 0; transform: translateY(8px); }
  to   { opacity: 1; transform: translateY(0); }
}

.reg-form__step-title {
  font-size: clamp(20px, 2.4vw, 26px);
  font-weight: 700;
  margin: 0 0 4px;
  letter-spacing: -0.01em;
}

.reg-form__grid {
  display: grid;
  gap: clamp(14px, 1.8vw, 20px);
}

.reg-form__grid--2col {
  grid-template-columns: 1fr 1fr;
}

.reg-form__field-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.reg-form__field-group--inline {
  gap: 12px;
}

.reg-form__field-group-label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

/* ── Document block ─────────────────────────────────────────────────────── */
.reg-form__doc-block {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.reg-form__doc-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  flex-wrap: wrap;
}

.reg-form__doc-title {
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 0.14em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.82);
}

.reg-form__doc-required {
  color: var(--c-brand-red);
  margin-left: 4px;
}

.reg-form__doc-optional {
  color: rgba(255, 255, 255, 0.45);
  font-weight: 600;
  letter-spacing: 0.06em;
  margin-left: 6px;
  text-transform: none;
}

.reg-form__doc-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.14);
  border-radius: 14px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.reg-form__doc-card-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.reg-form__doc-filename {
  font-size: 13px;
  font-weight: 600;
  color: #fff;
  word-break: break-all;
}

.reg-form__doc-remove {
  background: rgba(229, 45, 39, 0.18);
  border: 1px solid rgba(229, 45, 39, 0.45);
  color: #ff8a85;
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.10em;
  text-transform: uppercase;
  padding: 6px 12px;
  border-radius: 999px;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease;
}

.reg-form__doc-remove:hover {
  background: var(--c-brand-red);
  color: #fff;
}

.reg-form__doc-error {
  font-size: 12px;
  font-weight: 600;
  color: var(--c-brand-red);
  margin: 0;
}

/* ── Review summary ─────────────────────────────────────────────────────── */
.reg-form__summary {
  display: flex;
  flex-direction: column;
  gap: 0;
  margin: 0;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.14);
  border-radius: 16px;
  overflow: hidden;
}

.reg-form__summary-row {
  display: grid;
  grid-template-columns: minmax(140px, 0.5fr) 1fr;
  gap: 16px;
  padding: 12px 18px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.reg-form__summary-row:last-child {
  border-bottom: 0;
}

.reg-form__summary-row dt {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.55);
  align-self: center;
}

.reg-form__summary-row dd {
  margin: 0;
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 500;
  color: #fff;
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  align-items: center;
  word-break: break-word;
}

.reg-form__summary-chip {
  display: inline-flex;
  align-items: center;
  padding: 3px 10px;
  background: rgba(0, 173, 239, 0.20);
  border: 1px solid rgba(0, 173, 239, 0.50);
  border-radius: 999px;
  font-size: 12px;
  font-weight: 600;
}

/* ── Account / terms ────────────────────────────────────────────────────── */
.reg-form__link {
  color: var(--c-brand-cyan);
  text-decoration: underline;
  text-underline-offset: 2px;
}

.reg-form__field-error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  margin: 4px 0 0;
}

/* ── Nav buttons ────────────────────────────────────────────────────────── */
.reg-form__nav {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-top: clamp(14px, 2vw, 22px);
  padding-top: clamp(14px, 2vw, 22px);
  border-top: 1px solid rgba(255, 255, 255, 0.10);
}

.reg-form__btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: clamp(11px, 1.2vw, 14px) clamp(22px, 2.4vw, 30px);
  border-radius: 999px;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 700;
  letter-spacing: 0.04em;
  cursor: pointer;
  text-decoration: none;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease,
    box-shadow 0.25s ease;
}

.reg-form__btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.reg-form__btn--primary,
.reg-form__btn--submit {
  background: var(--c-brand-red);
  border: 1px solid var(--c-brand-red);
  color: #fff;
}

.reg-form__btn--primary:hover,
.reg-form__btn--submit:hover:not(:disabled) {
  background: #c92622;
  border-color: #c92622;
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(229, 45, 39, 0.35);
}

.reg-form__btn--ghost {
  background: rgba(255, 255, 255, 0.08);
  border: 1.5px solid rgba(255, 255, 255, 0.45);
  color: #fff;
}

.reg-form__btn--ghost:hover {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-navy, #0f2f44);
  transform: translateY(-1px);
}

.reg-form__btn-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.reg-form__btn-arrow--left {
  transform: scaleX(-1);
}

.reg-form__btn:hover .reg-form__btn-arrow:not(.reg-form__btn-arrow--left) {
  transform: translateX(3px);
}

.reg-form__btn:hover .reg-form__btn-arrow--left {
  transform: scaleX(-1) translateX(3px);
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 720px) {
  .reg-form__grid--2col {
    grid-template-columns: 1fr;
  }

  .reg-form__summary-row {
    grid-template-columns: 1fr;
    gap: 4px;
  }
}
</style>
