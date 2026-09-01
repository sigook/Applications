<template>
  <div class="reg-form landing-form">
    <p v-if="contextLine" class="reg-form__context">
      <span class="reg-form__context-label">Applying for:</span>
      <span class="reg-form__context-value">{{ contextLine }}</span>
    </p>

    <div v-if="isLoading" class="reg-form__loader" aria-live="polite">
      <span class="reg-form__spinner" aria-hidden="true"></span>
      <span>{{ loadingText }}</span>
    </div>

    <form v-else @submit.prevent="onSubmit">
      <b-steps v-model="activeStep" animated mobile-mode="compact" :has-navigation="false">
        <b-step-item step="1" label="Personal" :clickable="false">
          <h3 class="title">Personal information</h3>

          <div class="columns is-multiline">
            <div class="column is-12">
              <b-field :type="errors.fullName ? 'is-danger' : ''" :message="errors.fullName || ''">
                <template #label>
                  Full Name <span class="has-text-danger">*</span>
                </template>
                <b-input type="text" v-model="fullName" name="fullName" placeholder="Your full name"
                  :maxlength="60" :has-counter="false" autocomplete="name" />
              </b-field>
            </div>
            <div class="column is-6">
              <b-field :type="errors.email ? 'is-danger' : ''" :message="errors.email || ''">
                <template #label>
                  Email <span class="has-text-danger">*</span>
                </template>
                <b-input type="email" v-model="email" name="email" placeholder="you@email.com"
                  :maxlength="100" :has-counter="false" autocomplete="email" />
              </b-field>
            </div>
            <div class="column is-6">
              <PhoneInput ref="phoneComponent" label="Phone" :required="true" placeholder="305 123-4567"
                @formattedPhone="(value) => (phone = value || '')" />
            </div>
            <div class="column is-6">
              <b-field :type="errors.countryId ? 'is-danger' : ''" :message="errors.countryId || ''">
                <template #label>
                  Country <span class="has-text-danger">*</span>
                </template>
                <b-select v-model="selectedCountry" name="country" placeholder="Select country" expanded>
                  <option v-for="item in countries" :key="item.id" :value="item">{{ item.value }}</option>
                </b-select>
              </b-field>
            </div>
            <div class="column is-6">
              <b-field :type="errors.address ? 'is-danger' : ''" :message="errors.address || ''">
                <template #label>
                  City / Address <span class="has-text-danger">*</span>
                </template>
                <b-input type="text" v-model="address" name="address" placeholder="City / Address"
                  :maxlength="100" :has-counter="false" />
              </b-field>
            </div>
          </div>

          <div class="step-navigation-buttons">
            <span></span>
            <b-button type="is-primary" @click="goNext">Next</b-button>
          </div>
        </b-step-item>

        <b-step-item step="2" label="Details" :clickable="false">
          <h3 class="title">Additional details</h3>

          <div class="columns is-multiline">
            <div class="column is-6">
              <b-field :type="errors.status ? 'is-danger' : ''" :message="errors.status || ''">
                <template #label>
                  Immigration Status <span class="has-text-danger">*</span>
                </template>
                <b-select v-model="selectedStatus" name="status" placeholder="Select status" expanded>
                  <option v-for="item in statusOptions" :key="item.id" :value="item">{{ item.value }}</option>
                </b-select>
              </b-field>
            </div>
            <div class="column is-6">
              <b-field label="How did you hear about us?">
                <b-select v-model="selectedSource" name="source" placeholder="Select an option" expanded>
                  <option v-for="item in sources" :key="item.id" :value="item">{{ item.value }}</option>
                </b-select>
              </b-field>
            </div>

            <div class="column is-12">
              <div class="columns is-multiline document-section-header">
                <div class="column is-6">
                  <label class="fz1 has-text-weight-semibold section-label">
                    Resume / CV
                    <span v-if="resumeRequired" class="has-text-danger">*</span>
                    <i v-else>(optional)</i>
                  </label>
                </div>
                <div class="column is-6 upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{
                    'has-name': !!selectedResumeFile,
                    'upload-disabled': !!resume
                  }">
                    <b-upload v-model="selectedResumeFile" accept=".pdf,.doc,.docx"
                      @update:modelValue="onResumeUpload" :disabled="!!resume" class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">Add resume</span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="container-files">
                <div v-if="resume" class="document-card">
                  <div class="columns is-multiline document-card-header">
                    <div class="column is-10-mobile is-10 no-padding">
                      <div class="document-icon-title">
                        <b-icon icon="file-account" size="is-small" class="document-icon"></b-icon>
                        <h4 class="has-text-weight-semibold document-filename">{{ resume.name }}</h4>
                      </div>
                    </div>
                    <div class="column is-2-mobile is-2 document-delete-container no-padding">
                      <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                        <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                          @click="deleteResume">
                        </b-button>
                      </b-tooltip>
                    </div>
                  </div>
                </div>
              </div>
              <span v-show="resumeError" class="help is-danger">{{ resumeError }}</span>
            </div>

            <div class="column is-12">
              <b-field label="Skills / Roles of Interest">
                <b-taginput v-model="skillTags" autocomplete :data="filteredSkills" open-on-focus field="skill"
                  placeholder="Select from suggestions or type your own" :maxlength="20" :has-counter="false"
                  allow-new @typing="onSkillsTyping" :create-tag="createSkillTag">
                </b-taginput>
              </b-field>
            </div>
            <div class="column is-12">
              <b-field label="Transportation">
                <b-switch v-model="hasVehicle" :true-value="true" :false-value="false">
                  {{ hasVehicle ? 'Own Vehicle' : 'Public Transit' }}
                </b-switch>
              </b-field>
            </div>
          </div>

          <div class="step-navigation-buttons">
            <b-button @click="goPrev">Previous</b-button>
            <b-button type="is-primary" @click="goNext">Next</b-button>
          </div>
        </b-step-item>

        <b-step-item step="3" label="Review" :clickable="false">
          <h3 class="title">Review &amp; submit</h3>

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
                <span v-for="tag in skillTags" :key="tag.skill" class="reg-form__summary-chip">
                  {{ tag.skill }}
                </span>
              </dd>
            </div>
            <div v-if="resume" class="reg-form__summary-row">
              <dt>Resume</dt>
              <dd>{{ resume.name }}</dd>
            </div>
          </dl>

          <b-field class="reg-form__terms">
            <b-checkbox v-model="termsAccepted" name="agree terms">
              I agree to Sigook™
              <router-link to="/terms-and-conditions" target="_blank">
                <u class="color-primary">Terms and Conditions</u>
              </router-link>
              &amp;
              <router-link to="/privacy-policy" target="_blank">
                <u class="color-primary">Privacy Policy.</u>
              </router-link>
            </b-checkbox>
          </b-field>
          <span v-show="errors.termsAccepted" class="help is-danger">{{ errors.termsAccepted || '' }}</span>

          <div class="step-navigation-buttons">
            <b-button @click="goPrev">Previous</b-button>
            <b-button type="is-primary" native-type="submit" :disabled="isSubmitting">
              {{ isSubmitting ? 'Submitting…' : 'Submit Application' }}
            </b-button>
          </div>
        </b-step-item>
      </b-steps>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { useForm, useField } from 'vee-validate'
import * as yup from 'yup'

import PhoneInput from '@/components/PhoneInput.vue'

import { getSources, getSkills } from '@/api/catalogApi'
import { getCountries } from '@/api/locationApi'
import { submitCandidateApplication } from '@/api/websiteApi'
import { residencyList } from '@/constants/catalog'
import type { Country, Source, Skill } from '@/types/common'
import type { CandidateFormData } from '@/types/website'
import { showAlertError, showAlertSuccess } from '@/utils/toast'


const props = withDefaults(defineProps<{
  jobTitle?: string
  requestId?: string
  redirectOnSuccess?: boolean
}>(), {
  redirectOnSuccess: false,
})

const emit = defineEmits<{
  (e: 'submitted'): void
}>()

const contextLine = computed(() => props.jobTitle || '')

const activeStep = ref(0)
const LAST_STEP = 2

const schema = yup.object({
  fullName: yup.string().required('Full name is required').max(60, 'Full name must be at most 60 characters'),
  email: yup
    .string()
    .required('Email is required')
    .email('Invalid email format')
    .min(6, 'Email must be at least 6 characters')
    .max(100, 'Email must be at most 100 characters'),
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
    countryId: '',
    address: '',
    status: '',
    termsAccepted: false,
  },
})

const { value: fullName } = useField<string>('fullName')
const { value: email } = useField<string>('email')
const { value: countryId } = useField<string>('countryId')
const { value: address } = useField<string>('address')
const { value: status } = useField<string>('status')
const { value: termsAccepted } = useField<boolean>('termsAccepted')

const phone = ref('')
const phoneComponent = ref<InstanceType<typeof PhoneInput> | null>(null)

const interacted = reactive<Record<string, boolean>>({})
function markInteracted(fields: string[]): void {
  for (const f of fields) interacted[f] = true
}

const watched: Array<[string, { value: unknown }]> = [
  ['fullName', { value: fullName }],
  ['email', { value: email }],
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


interface StatusOption { id: string; value: string }
const statusOptions: StatusOption[] = residencyList.map((s) => ({ id: s, value: s }))

const countries = ref<Country[]>([])
const sources = ref<Source[]>([])

const selectedCountry = ref<Country | null>(null)
const selectedStatus = ref<StatusOption | null>(null)
const selectedSource = ref<Source | null>(null)

watch(selectedCountry, (c) => { countryId.value = c?.id ?? '' })
watch(selectedStatus, (s) => { status.value = s?.id ?? '' })

const resumeRequired = computed(() => selectedSource.value?.value === 'Linkedin')


const allSkills = ref<Skill[]>([])
const skillTags = ref<Skill[]>([])
const skillQuery = ref('')

const filteredSkills = computed<Skill[]>(() => {
  const q = skillQuery.value.trim().toLowerCase()
  if (!q) return allSkills.value
  return allSkills.value.filter((s) => s.skill.toLowerCase().includes(q))
})

function onSkillsTyping(value: string): void {
  skillQuery.value = value
}

function createSkillTag(raw: string): Skill {
  return { skill: raw.slice(0, 20) }
}

const hasVehicle = ref(false)
const resume = ref<File | null>(null)
const selectedResumeFile = ref<File | null>(null)
const resumeError = ref('')

function onResumeUpload(file: File | null): void {
  if (!file) return
  if (file.size / 1024 > 15_500) {
    showAlertError('File exceeds 15MB limit')
    return
  }
  resume.value = file
  resumeError.value = ''
  selectedResumeFile.value = null
}

function deleteResume(): void {
  resume.value = null
}

async function validateStepPersonal(): Promise<boolean> {
  const fields = ['fullName', 'email', 'countryId', 'address']
  markInteracted(fields)
  const results = await Promise.all(fields.map((f) => validateField(f as never)))
  const phoneValid = await phoneComponent.value?.validatePhone()
  return results.every((r) => r.valid) && !!phoneValid
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
  if (activeStep.value === 0) valid = await validateStepPersonal()
  else if (activeStep.value === 1) valid = await validateStepDetails()
  if (!valid) {
    showAlertError('Please make sure all required fields are filled out correctly')
    return
  }
  if (activeStep.value < LAST_STEP) {
    activeStep.value++
  }
}

function goNext(): void {
  void validateAndAdvance()
}

function goPrev(): void {
  if (activeStep.value > 0) activeStep.value--
}

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
    activeStep.value = 1
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
    skills: skillTags.value.map((s) => s.skill),
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
    const defaultSource = sourceList.find((s) => s.value === 'Sigook')
    if (defaultSource) selectedSource.value = defaultSource
  } catch (err) {
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
}

.reg-form__context {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  margin: 0 0 clamp(20px, 2.6vw, 30px);
  background: rgba(0, 173, 239, 0.10);
  border: 1px solid rgba(0, 173, 239, 0.40);
  border-radius: 999px;
  font-size: 12px;
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

.reg-form__summary {
  display: flex;
  flex-direction: column;
  margin: 0 0 clamp(16px, 2vw, 22px);
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid var(--c-glass-border-soft);
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

.reg-form__terms {
  margin-bottom: 0;
}

.document-section-header {
  align-items: center;
  margin-bottom: 15px;
}

.section-label {
  margin-bottom: 0;
}

.upload-button-container {
  text-align: right;
}

.upload-field {
  margin-bottom: 0;
}

.container-files:empty {
  display: none;
}

.document-card-header {
  align-items: center;
}

.document-icon-title {
  display: flex;
  align-items: center;
}

.document-icon {
  margin-right: 10px;
}

.document-filename {
  margin: 0;
}

.document-delete-container {
  text-align: right;
}

.no-padding {
  padding: 0;
}

.upload-disabled {
  opacity: 0.5;
  cursor: not-allowed;
  pointer-events: none;
}

.step-navigation-buttons {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 20px;
  padding-top: 20px;
}

@media (max-width: 900px) {
  .reg-form__summary-row {
    grid-template-columns: 1fr;
    gap: 4px;
  }
}
</style>
