<template>
  <div class="reg-form" :class="{ 'reg-form--loading': isLoading }">
    <header class="reg-form__head">
      <p v-if="contextLine" class="reg-form__context">
        <span class="reg-form__context-label">Applying for:</span>
        <span class="reg-form__context-value">{{ contextLine }}</span>
      </p>
      <WorkerRegisterStepNav :steps="visibleSteps" :current="currentStepIdx" />
    </header>

    <div v-if="isLoading" class="reg-form__loader" aria-live="polite">
      <span class="reg-form__spinner" aria-hidden="true"></span>
      <span>{{ loadingText }}</span>
    </div>

    <form v-else class="reg-form__body" @submit.prevent="onSubmit">
      <section v-show="currentStep === 'basic'" class="reg-form__step">
        <h3 class="reg-form__step-title">Basic information</h3>

        <div class="reg-form__photo-wrap">
          <ImageUpload
            hint="Plain white or off-white background works best."
            @file="onPhotoSelected"
          />
        </div>

        <div class="reg-form__grid reg-form__grid--2col">
          <Input v-model="firstName" label="Name" :required="true" :error="errors.firstName" />
          <Input v-model="lastName" label="Last Name" :required="true" :error="errors.lastName" />
          <DatePicker
            v-model="birthDay"
            label="Date of birth"
            :required="true"
            :error="errors.birthDay"
            :max="maxBirthDate"
          />
          <Select
            v-model="gender"
            :options="genders"
            label="Gender"
            placeholder="Select gender"
            :required="true"
            :error="errors.gender"
          />
        </div>

        <div class="reg-form__section-title">Address</div>
        <AddressFields
          ref="addressRef"
          v-model="addressModel"
        />

        <div class="reg-form__row">
          <PhoneInput
            v-model="mobileNumber"
            label="Mobile Number"
            :required="true"
            :error="errors.mobileNumber"
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

      <section v-show="currentStep === 'preferences'" class="reg-form__step">
        <h3 class="reg-form__step-title">Preferences</h3>

        <div class="reg-form__field-group">
          <span class="reg-form__field-group-label">Availability</span>
          <div class="reg-form__check-grid">
            <Checkbox
              v-for="item in availabilities"
              :key="item.id"
              v-model="worker.availabilities"
              :native-value="item"
            >{{ item.value }}</Checkbox>
          </div>
        </div>

        <div class="reg-form__field-group">
          <span class="reg-form__field-group-label">Available time</span>
          <div class="reg-form__check-grid">
            <Checkbox
              v-for="t in availabilityTimes"
              :key="t.id"
              v-model="worker.availabilityTimes"
              :native-value="t"
            >{{ t.value }}</Checkbox>
          </div>
        </div>

        <div class="reg-form__field-group">
          <span class="reg-form__field-group-label">Available days</span>
          <div class="reg-form__check-grid">
            <Checkbox v-model="allDaysSelected" @update:model-value="changeDaysSelected">
              All days
            </Checkbox>
            <Checkbox
              v-for="day in days"
              :key="day.id"
              v-model="worker.availabilityDays"
              :native-value="day"
              @update:model-value="changeAllDays"
            >{{ day.value }}</Checkbox>
          </div>
        </div>

        <div class="reg-form__grid reg-form__grid--2col">
          <Select
            v-model="lift"
            :options="lifts"
            label="Can you lift up to"
            placeholder="Select option"
          />
          <div class="reg-form__field-group reg-form__field-group--inline">
            <span class="reg-form__field-group-label">Do you have your own vehicle?</span>
            <Switch v-model="hasVehicle">{{ hasVehicle ? 'Yes' : 'No' }}</Switch>
          </div>
        </div>

        <TagInput
          v-model="languagesModel"
          :options="filteredLanguages"
          label="Languages"
          placeholder="Select languages"
          option-key="id"
          option-label="value"
          @typing="getFilteredLanguages"
        />

        <TagInput
          v-model="skillsModel"
          :options="filteredSkills"
          label="Skills"
          placeholder="Select or add skills"
          option-key="id"
          option-label="skill"
          :allow-new="true"
          :create-tag="(raw) => ({ skill: raw } as unknown as { id: number; skill: string })"
          @typing="getFilteredSkills"
        />

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

      <section v-show="currentStep === 'documents'" class="reg-form__step">
        <h3 class="reg-form__step-title">Documents</h3>

        <div class="reg-form__doc-block">
          <div class="reg-form__doc-head">
            <span class="reg-form__doc-title">
              Identification <span class="reg-form__doc-required">*</span>
            </span>
            <FileUpload
              label="Add file"
              :disabled="!!worker.identificationType1File && !!worker.identificationType2File"
              @file="handleIdentificationUpload"
            />
          </div>

          <div v-if="worker.identificationType1File" class="reg-form__doc-card">
            <div class="reg-form__doc-card-head">
              <span class="reg-form__doc-filename">📄 {{ filename(worker.identificationType1File.fileName) }}</span>
              <button type="button" class="reg-form__doc-remove" @click="deleteDocument(worker.identificationType1File)">Remove</button>
            </div>
            <div class="reg-form__grid reg-form__grid--2col">
              <Select
                v-model="identificationType1"
                :options="identificationTypes"
                label="Identification type"
                placeholder="Select type"
                :required="true"
                :error="errors.identificationType1"
              />
              <Input
                v-model="identificationNumber1"
                label="Identification number"
                :required="true"
                :error="errors.identificationNumber1"
              />
            </div>
          </div>

          <div v-if="worker.identificationType2File" class="reg-form__doc-card">
            <div class="reg-form__doc-card-head">
              <span class="reg-form__doc-filename">📄 {{ filename(worker.identificationType2File.fileName) }}</span>
              <button type="button" class="reg-form__doc-remove" @click="deleteDocument(worker.identificationType2File)">Remove</button>
            </div>
            <div class="reg-form__grid reg-form__grid--2col">
              <Select
                v-model="identificationType2"
                :options="identificationTypes"
                label="Identification type"
                placeholder="Select type"
                :required="true"
                :error="errors.identificationType2"
              />
              <Input
                v-model="identificationNumber2"
                label="Identification number"
                :required="true"
                :error="errors.identificationNumber2"
              />
            </div>
          </div>

          <p v-if="documentsError" class="reg-form__doc-error">
            At least one identification document is required.
          </p>
        </div>

        <div class="reg-form__doc-block">
          <div class="reg-form__doc-head">
            <span class="reg-form__doc-title">Licenses</span>
            <FileUpload label="Add license" @file="handleLicenseUpload" />
          </div>
          <div
            v-for="(item, idx) in worker.licenses"
            :key="`lic-${idx}`"
            class="reg-form__doc-card"
          >
            <div class="reg-form__doc-card-head">
              <span class="reg-form__doc-filename">🪪 {{ filename(item.license.fileName) }}</span>
              <button type="button" class="reg-form__doc-remove" @click="deleteLicense(idx)">Remove</button>
            </div>
            <div class="reg-form__grid reg-form__grid--licence">
              <Input
                v-model="item.license.description"
                label="Description"
                :required="true"
                :error="itemErrors['description' + idx]"
              />
              <DatePicker
                v-model="item.expires"
                label="Expires"
                :required="true"
                :error="itemErrors['licenseExpires' + idx]"
              />
            </div>
          </div>
        </div>

        <div class="reg-form__doc-block">
          <div class="reg-form__doc-head">
            <span class="reg-form__doc-title">Certificates</span>
            <FileUpload label="Add certificate" @file="handleCertificateUpload" />
          </div>
          <div
            v-for="(item, idx) in worker.certificates"
            :key="`cer-${idx}`"
            class="reg-form__doc-card"
          >
            <div class="reg-form__doc-card-head">
              <span class="reg-form__doc-filename">🎓 {{ filename(item.fileName) }}</span>
              <button type="button" class="reg-form__doc-remove" @click="deleteCertificate(idx)">Remove</button>
            </div>
            <Input
              v-model="item.description"
              label="Description"
              :required="true"
              :error="itemErrors['descriptioncer' + idx]"
            />
          </div>
        </div>

        <div class="reg-form__doc-block">
          <div class="reg-form__doc-head">
            <span class="reg-form__doc-title">Resume</span>
            <FileUpload
              label="Add resume"
              :disabled="!!worker.resume"
              @file="handleResumeUpload"
            />
          </div>
          <div v-if="worker.resume" class="reg-form__doc-card">
            <div class="reg-form__doc-card-head">
              <span class="reg-form__doc-filename">📑 {{ filename(worker.resume.fileName) }}</span>
              <button type="button" class="reg-form__doc-remove" @click="deleteResume()">Remove</button>
            </div>
          </div>
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

      <section v-show="currentStep === 'account'" class="reg-form__step">
        <h3 class="reg-form__step-title">Account</h3>

        <Input
          v-model="email"
          label="Email"
          type="email"
          :required="true"
          :error="errors.email"
          autocomplete="email"
        />

        <div class="reg-form__grid reg-form__grid--2col">
          <Input
            v-model="password"
            label="Password"
            type="password"
            :required="true"
            :error="errors.password"
            autocomplete="new-password"
          />
          <Input
            v-model="confirmPassword"
            label="Confirm password"
            type="password"
            :required="true"
            :error="errors.confirmPassword"
            autocomplete="new-password"
          />
        </div>

        <Checkbox v-if="!isLogin" v-model="agreeTermsAndConditions">
          I agree to Sigook™
          <router-link to="/terms-and-conditions" target="_blank" class="reg-form__link">Terms and Conditions</router-link>
          &amp;
          <router-link to="/privacy-policy" target="_blank" class="reg-form__link">Privacy Policy</router-link>.
        </Checkbox>
        <p v-if="!isLogin && errors.agreeTermsAndConditions" class="reg-form__field-error">
          {{ errors.agreeTermsAndConditions }}
        </p>

        <footer class="reg-form__nav">
          <button type="button" class="reg-form__btn reg-form__btn--ghost" @click="goPrev">
            <span class="reg-form__btn-arrow reg-form__btn-arrow--left" aria-hidden="true">←</span>
            <span>Previous</span>
          </button>
          <button type="submit" class="reg-form__btn reg-form__btn--submit" :disabled="isSubmitting">
            <span>Register</span>
            <span class="reg-form__btn-arrow" aria-hidden="true">→</span>
          </button>
        </footer>
      </section>
    </form>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useForm, useField } from 'vee-validate'
import * as yup from 'yup'
import dayjs from 'dayjs'

import Input from '@/components/landing/shared/forms/Input.vue'
import Select from '@/components/landing/shared/forms/Select.vue'
import DatePicker from '@/components/landing/shared/forms/DatePicker.vue'
import Checkbox from '@/components/landing/shared/forms/Checkbox.vue'
import Switch from '@/components/landing/shared/forms/Switch.vue'
import TagInput from '@/components/landing/shared/forms/TagInput.vue'
import FileUpload from '@/components/landing/shared/forms/FileUpload.vue'
import ImageUpload from '@/components/landing/shared/forms/ImageUpload.vue'
import PhoneInput from '@/components/landing/shared/forms/PhoneInput.vue'
import AddressFields, { type AddressModel } from '@/components/landing/shared/forms/AddressFields.vue'
import WorkerRegisterStepNav, { type StepDescriptor } from '@/components/landing/shared/forms/WorkerRegisterStepNav.vue'

import { useCreateWorker } from '@/composables/useCreateWorker'
import { registerWorker } from '@/api/workerApi'
import { createMultipartFormData } from '@/utils/buildWorkerFormData'
import { useSecurityStore } from '@/stores/security'
import { useAppStore } from '@/stores/app'
import { showAlertError, showAlertSuccess } from '@/utils/toast'
import { filename } from '@/utils/filters'


const props = withDefaults(defineProps<{
  jobTitle?: string
  redirectOnSuccess?: boolean
}>(), {
  redirectOnSuccess: true,
})

const emit = defineEmits<{
  (e: 'submitted', workerId: string): void
}>()

const router = useRouter()
const securityStore = useSecurityStore()
const appStore = useAppStore()
const create = useCreateWorker()

const {
  skills,
  filteredSkills,
  filteredLanguages,
  genders,
  identificationTypes,
  availabilities,
  availabilityTimes,
  days,
  lifts,
  worker,
  allDaysSelected,
  loadCatalogs,
  changeDaysSelected,
  changeAllDays,
  getFilteredSkills,
  getFilteredLanguages,
} = create

const isLogin = computed(() => !!securityStore.user)

const contextLine = computed(() => props.jobTitle || '')

type StepKey = 'basic' | 'preferences' | 'documents' | 'account'
const ALL_STEPS: readonly StepDescriptor[] = [
  { key: 'basic',       label: 'Basic' },
  { key: 'preferences', label: 'Preferences' },
  { key: 'documents',   label: 'Documents' },
  { key: 'account',     label: 'Account' },
] as const

const visibleSteps = computed<readonly StepDescriptor[]>(() =>
  isLogin.value ? ALL_STEPS.filter((s) => s.key !== 'preferences') : ALL_STEPS,
)

const currentStep = ref<StepKey>('basic')
const currentStepIdx = computed(() =>
  visibleSteps.value.findIndex((s) => s.key === currentStep.value),
)

function goNext(): void {
  void validateAndAdvance()
}

function goPrev(): void {
  const idx = currentStepIdx.value
  if (idx > 0) currentStep.value = visibleSteps.value[idx - 1].key as StepKey
}

const hasSecondId = computed(() => !!worker.identificationType2File)

const validationSchema = computed(() => {
  const shape: Record<string, yup.AnySchema> = {
    firstName: yup.string().required('Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
    lastName: yup.string().required('Last name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
    birthDay: yup.mixed().required('Date of birth is required'),
    gender: yup.mixed().required('Gender is required'),
    mobileNumber: yup
      .string()
      .required('Mobile number is required')
      .matches(/^\d{3}\s\d{3}-\d{4}$/, 'Invalid mobile format'),
    identificationType1: yup.mixed().required('Identification type is required'),
    identificationNumber1: yup
      .string()
      .required('Identification number is required')
      .min(5, 'Min 5 characters')
      .max(15, 'Max 15 characters'),
    email: yup.string().required('Email is required').email('Invalid email').min(6).max(50),
    password: yup.string().required('Password is required').min(6, 'Min 6 characters').max(100, 'Max 100 characters'),
    confirmPassword: yup
      .string()
      .required('Confirm password is required')
      .oneOf([yup.ref('password')], 'Passwords must match'),
  }
  if (hasSecondId.value) {
    shape.identificationType2 = yup.mixed().required('Identification type is required')
    shape.identificationNumber2 = yup
      .string()
      .required('Identification number is required')
      .min(5, 'Min 5 characters')
      .max(15, 'Max 15 characters')
  }
  if (!isLogin.value) {
    shape.agreeTermsAndConditions = yup
      .boolean()
      .oneOf([true], 'You must accept the Terms & Conditions to continue')
  }
  return yup.object(shape)
})

const { errors: formErrors, validateField } = useForm({
  validationSchema,
  initialValues: {
    firstName: '',
    lastName: '',
    birthDay: null,
    gender: null,
    mobileNumber: '',
    identificationType1: null,
    identificationNumber1: '',
    identificationType2: null,
    identificationNumber2: '',
    email: '',
    password: '',
    confirmPassword: '',
    agreeTermsAndConditions: false,
  },
})

const { value: firstName } = useField<string>('firstName')
const { value: lastName } = useField<string>('lastName')
const { value: birthDay } = useField<Date | null>('birthDay')
const { value: gender } = useField<unknown>('gender')
const { value: mobileNumber } = useField<string>('mobileNumber')
const { value: identificationType1 } = useField<unknown>('identificationType1')
const { value: identificationNumber1 } = useField<string>('identificationNumber1')
const { value: identificationType2 } = useField<unknown>('identificationType2')
const { value: identificationNumber2 } = useField<string>('identificationNumber2')
const { value: email } = useField<string>('email')
const { value: password } = useField<string>('password')
const { value: confirmPassword } = useField<string>('confirmPassword')
const { value: agreeTermsAndConditions } = useField<boolean>('agreeTermsAndConditions')

const interacted = reactive<Record<string, boolean>>({})
const watchedFields: Array<[string, { value: unknown }]> = [
  ['firstName', { value: firstName }],
  ['lastName', { value: lastName }],
  ['birthDay', { value: birthDay }],
  ['gender', { value: gender }],
  ['mobileNumber', { value: mobileNumber }],
  ['identificationType1', { value: identificationType1 }],
  ['identificationNumber1', { value: identificationNumber1 }],
  ['identificationType2', { value: identificationType2 }],
  ['identificationNumber2', { value: identificationNumber2 }],
  ['email', { value: email }],
  ['password', { value: password }],
  ['confirmPassword', { value: confirmPassword }],
  ['agreeTermsAndConditions', { value: agreeTermsAndConditions }],
]
for (const [name, ref_] of watchedFields) {
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

const itemErrors = reactive<Record<string, string>>({})

function markInteracted(fields: string[]): void {
  for (const f of fields) interacted[f] = true
}

const addressModel = ref<AddressModel>({
  country: null,
  province: null,
  city: null,
  address: '',
  postalCode: '',
})
const addressRef = ref<{ validate: () => boolean } | null>(null)

const lift = computed({
  get: () => worker.lift as unknown,
  set: (v: unknown) => { worker.lift = v as never },
})
const hasVehicle = computed({
  get: () => Boolean(worker.hasVehicle),
  set: (v: boolean) => { worker.hasVehicle = v as never },
})

const languagesModel = computed({
  get: () => worker.languages,
  set: (v) => { worker.languages = v as never },
})
const skillsModel = computed({
  get: () => worker.skills,
  set: (v) => { worker.skills = v as never },
})

const documentsError = ref(false)
const fileObjects = reactive({
  profileImage: null as File | null,
  identificationType1: null as File | null,
  identificationType2: null as File | null,
  licenses: [] as File[],
  certificates: [] as File[],
  resume: null as File | null,
  otherDocuments: [] as File[],
})

function validateFileSize(file: File | null, maxKB = 15_500): boolean {
  if (!file) return false
  if (file.size / 1024 > maxKB) {
    showAlertError('File exceeds 15MB limit')
    return false
  }
  return true
}

function onPhotoSelected(file: File | null): void {
  if (!file) return
  fileObjects.profileImage = file
  if (!worker.profileImage) {
    worker.profileImage = { fileName: file.name } as never
  } else {
    ;(worker.profileImage as { fileName: string }).fileName = file.name
  }
}

function handleIdentificationUpload(file: File | null): void {
  if (!file || !validateFileSize(file)) return
  if (!worker.identificationType1File) {
    fileObjects.identificationType1 = file
    worker.identificationType1File = { fileName: file.name, description: '' }
  } else {
    fileObjects.identificationType2 = file
    worker.identificationType2File = { fileName: file.name, description: '' }
  }
  documentsError.value = false
}

function handleLicenseUpload(file: File | null): void {
  if (!file || !validateFileSize(file)) return
  fileObjects.licenses.push(file)
  worker.licenses.push({ license: { fileName: file.name, description: '' } })
}

function handleCertificateUpload(file: File | null): void {
  if (!file || !validateFileSize(file)) return
  fileObjects.certificates.push(file)
  worker.certificates.push({ fileName: file.name, description: '' })
}

function handleResumeUpload(file: File | null): void {
  if (!file || !validateFileSize(file)) return
  fileObjects.resume = file
  worker.resume = { fileName: file.name }
}

function deleteDocument(file: { fileName: string }): void {
  if (!worker.identificationType1File) return
  const isFirst = worker.identificationType1File.fileName === file.fileName
  if (isFirst && worker.identificationType2File) {
    worker.identificationType1File = { ...worker.identificationType2File }
    fileObjects.identificationType1 = fileObjects.identificationType2
    identificationType1.value = identificationType2.value
    identificationNumber1.value = identificationNumber2.value
    fileObjects.identificationType2 = null
    worker.identificationType2File = null
    identificationType2.value = null
    identificationNumber2.value = ''
  } else if (isFirst) {
    fileObjects.identificationType1 = null
    worker.identificationType1File = null
    identificationType1.value = null
    identificationNumber1.value = ''
  } else {
    fileObjects.identificationType2 = null
    worker.identificationType2File = null
    identificationType2.value = null
    identificationNumber2.value = ''
  }
}

function deleteLicense(idx: number): void {
  fileObjects.licenses.splice(idx, 1)
  worker.licenses.splice(idx, 1)
}

function deleteCertificate(idx: number): void {
  fileObjects.certificates.splice(idx, 1)
  worker.certificates.splice(idx, 1)
}

function deleteResume(): void {
  fileObjects.resume = null
  worker.resume = null
}

async function validateStepBasic(): Promise<boolean> {
  const fields = ['firstName', 'lastName', 'birthDay', 'gender', 'mobileNumber']
  markInteracted(fields)
  const results = await Promise.all(fields.map((f) => validateField(f as never)))
  const fieldsValid = results.every((r) => r.valid)
  const addressValid = addressRef.value?.validate() ?? false
  return fieldsValid && addressValid
}

async function validateStepDocuments(): Promise<boolean> {
  documentsError.value = !worker.identificationType1File
  if (documentsError.value) return false

  const fields = ['identificationType1', 'identificationNumber1']
  if (worker.identificationType2File) {
    fields.push('identificationType2', 'identificationNumber2')
  }
  markInteracted(fields)
  const results = await Promise.all(fields.map((f) => validateField(f as never)))
  let valid = results.every((r) => r.valid)

  const next: Record<string, string> = {}
  for (let i = 0; i < worker.licenses.length; i++) {
    const item = worker.licenses[i] as { license: { description: string }; expires?: Date }
    const desc = item.license.description || ''
    if (!desc) { next['description' + i] = 'Description is required'; valid = false }
    else if (desc.length > 100) { next['description' + i] = 'Max 100 characters'; valid = false }
    if (!item.expires) { next['licenseExpires' + i] = 'Expiration date is required'; valid = false }
  }
  for (let i = 0; i < worker.certificates.length; i++) {
    const item = worker.certificates[i] as { description: string }
    if (!item.description) { next['descriptioncer' + i] = 'Description is required'; valid = false }
  }
  for (let i = 0; i < worker.otherDocuments.length; i++) {
    const item = worker.otherDocuments[i] as { description: string }
    if (!item.description) { next['descriptionOther' + i] = 'Description is required'; valid = false }
  }
  Object.keys(itemErrors).forEach((k) => delete itemErrors[k])
  Object.assign(itemErrors, next)
  return valid
}

async function validateAndAdvance(): Promise<void> {
  let valid = false
  if (currentStep.value === 'basic') valid = await validateStepBasic()
  else if (currentStep.value === 'preferences') valid = true
  else if (currentStep.value === 'documents') valid = await validateStepDocuments()
  if (!valid) {
    showAlertError('Please make sure all required fields are filled out correctly')
    return
  }
  const idx = currentStepIdx.value
  if (idx < visibleSteps.value.length - 1) {
    currentStep.value = visibleSteps.value[idx + 1].key as StepKey
  }
}

const isLoading = ref(true)
const loadingText = ref('Loading…')
const isSubmitting = ref(false)

async function onSubmit(): Promise<void> {
  if (isSubmitting.value) return
  isSubmitting.value = true
  const fields = ['email', 'password', 'confirmPassword']
  if (!isLogin.value) fields.push('agreeTermsAndConditions')
  markInteracted(fields)
  const results = await Promise.all(fields.map((f) => validateField(f as never)))
  if (!results.every((r) => r.valid)) {
    showAlertError('Please make sure all required fields are filled out correctly')
    isSubmitting.value = false
    return
  }

  isLoading.value = true
  loadingText.value = 'Submitting…'

  worker.firstName = firstName.value as never
  worker.lastName = lastName.value as never
  worker.birthDay = birthDay.value as never
  worker.mobileNumber = mobileNumber.value as never
  worker.identificationType1 = identificationType1.value as never
  worker.identificationNumber1 = identificationNumber1.value as never
  worker.identificationType2 = identificationType2.value as never
  worker.identificationNumber2 = identificationNumber2.value as never
  worker.email = email.value as never
  worker.password = password.value as never
  worker.confirmPassword = confirmPassword.value as never
  worker.agreeTermsAndConditions = agreeTermsAndConditions.value as never
  const genderObj = gender.value as { id: string } | null
  worker.gender = (genderObj ? { id: genderObj.id } : null) as never
  worker.location = addressModel.value.city
    ? {
        address: addressModel.value.address,
        postalCode: addressModel.value.postalCode,
        city: {
          ...addressModel.value.city,
          province: {
            ...addressModel.value.province,
            country: addressModel.value.country,
          },
        },
      }
    : {}

  try {
    const formData = await createMultipartFormData(worker, fileObjects)
    const id = await registerWorker(formData)
    showAlertSuccess('Your account has been created')
    emit('submitted', id)
    if (props.redirectOnSuccess) {
      const route = isLogin.value ? `/agency-workers/worker/${id}` : '/'
      router.push(route)
    }
  } catch (err: unknown) {
    showAlertError((err as { data?: string })?.data ?? 'Something went wrong')
  } finally {
    isLoading.value = false
    isSubmitting.value = false
  }
}

const maxBirthDate = ref<Date | null>(null)

onMounted(async () => {
  try {
    await loadCatalogs()
  } catch (err) {
    console.warn('[WorkerRegisterForm] catalog load failed:', err)
  }
  try {
    const today = await appStore.getCurrentDate()
    maxBirthDate.value = dayjs(today).subtract(18, 'years').toDate()
  } catch {
    maxBirthDate.value = dayjs().subtract(18, 'years').toDate()
  }
  isLoading.value = false
  loadingText.value = 'Loading…'
})

void skills
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

.reg-form__section-title {
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-top: clamp(4px, 0.8vw, 8px);
}

.reg-form__photo-wrap {
  display: flex;
  justify-content: center;
  margin: 0 auto;
}

.reg-form__grid {
  display: grid;
  gap: clamp(14px, 1.8vw, 20px);
}

.reg-form__grid--2col {
  grid-template-columns: 1fr 1fr;
}

.reg-form__grid--licence {
  grid-template-columns: 2fr 1fr;
}

.reg-form__row {
  display: grid;
  grid-template-columns: 1fr;
  gap: clamp(14px, 1.8vw, 20px);
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

.reg-form__check-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px;
}

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

.reg-form__doc-hint {
  font-size: 12px;
  color: rgba(255, 255, 255, 0.55);
  margin: 4px 0 0;
}

.reg-form__doc-links {
  display: flex;
  gap: 18px;
  flex-wrap: wrap;
  margin: 0 0 6px;
}

.reg-form__doc-links a {
  color: var(--c-brand-cyan);
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.06em;
  text-decoration: none;
}

.reg-form__doc-links a:hover {
  text-decoration: underline;
}

.reg-form__doc-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid var(--c-glass-border-soft);
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

.reg-form__btn--primary,
.reg-form__btn--submit {
  background: var(--c-brand-red);
  border: 1px solid var(--c-brand-red);
  color: #fff;
}

.reg-form__btn--primary:hover,
.reg-form__btn--submit:hover {
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
  color: var(--c-brand-navy);
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

@media (max-width: 720px) {
  .reg-form__grid--2col,
  .reg-form__grid--licence {
    grid-template-columns: 1fr;
  }

  .reg-form__check-grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 479px) {
  .reg-form__check-grid {
    grid-template-columns: 1fr;
  }
}
</style>
