<template>
  <v-dialog v-model="isOpen" max-width="600px">
    <v-card>
      <!-- HEADER -->
      <v-toolbar color="primary" dark>
        <v-toolbar-title>
          {{ dialogTitle }}
        </v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn icon @click="handleClose">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-toolbar>

      <!-- TABS (shown only when applying to a job) -->
      <v-card-text class="pa-2 dialog-content">
        <v-tabs v-if="selectedJob" v-model="activeTab" centered class="mb-2">
          <v-tab value="new">New Applicant</v-tab>
          <v-tab value="registered">Already Registered</v-tab>
        </v-tabs>

        <!-- NEW APPLICANT TAB -->
        <div v-if="!selectedJob || activeTab === 'new'">
          <!-- STEPPER -->
          <v-stepper v-model="currentStep" :items="stepItems" alt-labels>

            <!-- STEP 1: Personal Info -->
            <template v-slot:item.1>
              <v-card flat class="pa-2">
                <v-card-title class="text-h6 mb-2">Personal Information</v-card-title>
                <v-form>
                  <v-row>
                    <v-col cols="12" md="6">
                      <v-text-field v-model="fields.fullName.value" v-bind="fields.fullNameProps" label="Full Name *"
                        :error-messages="errors.fullName" counter="60" variant="outlined"></v-text-field>
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-text-field v-model="fields.email.value" v-bind="fields.emailProps" label="Email *" type="email"
                        :error-messages="errors.email" variant="outlined"></v-text-field>
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-text-field v-model="phoneFormatted" label="Phone *" placeholder="305 123-4567"
                        :error-messages="errors.phone" hint="Format: ### ###-####" persistent-hint variant="outlined"
                        @input="handlePhoneInput"></v-text-field>
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-select v-model="fields.countryId.value" v-bind="fields.countryIdProps" label="Country *"
                        :items="countries" item-title="value" item-value="id" :error-messages="errors.countryId"
                        :loading="loadingCountries" variant="outlined"></v-select>
                    </v-col>
                    <v-col cols="12">
                      <v-text-field v-model="fields.address.value" v-bind="fields.addressProps" label="City / Address *"
                        :error-messages="errors.address" counter="100" variant="outlined"></v-text-field>
                    </v-col>
                  </v-row>
                </v-form>
              </v-card>
            </template>

            <!-- STEP 2: Additional Details -->
            <template v-slot:item.2>
              <v-card flat class="pa-2">
                <v-card-title class="text-h6 mb-2">Additional Details</v-card-title>
                <v-form>
                  <v-row>
                    <v-col cols="12" md="6">
                      <v-select v-model="(fields.status.value as any)" v-bind="fields.statusProps" label="Immigration Status *"
                        :items="RESIDENCY_STATUS" :error-messages="errors.status" variant="outlined"></v-select>
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-switch v-model="fields.hasVehicle.value" v-bind="fields.hasVehicleProps"
                        :label="fields.hasVehicle.value ? 'Own Vehicle' : 'Public Transit'"
                        :error-messages="errors.hasVehicle" color="primary" hide-details="auto"></v-switch>
                    </v-col>
                    <v-col cols="12">
                      <v-combobox v-model="fields.skills.value" v-bind="fields.skillsProps"
                        label="Skills / Roles of Interest" :items="suggestedSkills" multiple chips closable-chips
                        :error-messages="errors.skills"
                        hint="Select from suggestions or type your own (Press Enter to add, max 20 characters each)"
                        persistent-hint variant="outlined" :loading="loadingSkills"></v-combobox>
                    </v-col>
                  </v-row>
                </v-form>
              </v-card>
            </template>

            <!-- STEP 3: Resume & Terms -->
            <template v-slot:item.3>
              <v-card flat class="pa-2">
                <v-card-title class="text-h6 mb-2">Resume & Terms</v-card-title>
                <v-form>
                  <v-row>
                    <v-col cols="12">
                      <v-file-input v-model="resumeFiles" label="Resume / CV (Optional)" accept=".pdf,.doc,.docx"
                        prepend-icon="mdi-file-document" :error-messages="errors.resume" show-size variant="outlined"
                        @update:model-value="handleResumeChange"></v-file-input>
                    </v-col>
                    <v-col cols="12">
                      <v-checkbox v-model="fields.termsAccepted.value" v-bind="fields.termsAcceptedProps"
                        :error-messages="errors.termsAccepted" color="primary">
                        <template v-slot:label>
                          <span>
                            I accept the
                            <a href="/terms" target="_blank" @click.stop>Terms and Conditions</a>
                            *
                          </span>
                        </template>
                      </v-checkbox>
                    </v-col>

                    <!-- Summary Review -->
                    <v-col cols="12">
                      <v-card variant="outlined" class="pa-2">
                        <v-card-title class="text-subtitle-1 mb-1">Application Summary</v-card-title>
                        <v-card-text class="pa-2">
                          <div class="mb-1 text-body-2">
                            <strong>Name:</strong> {{ values.fullName || 'Not provided' }}
                          </div>
                          <div class="mb-1 text-body-2">
                            <strong>Email:</strong> {{ values.email || 'Not provided' }}
                          </div>
                          <div class="mb-1 text-body-2">
                            <strong>Phone:</strong> {{ values.phone || 'Not provided' }}
                          </div>
                          <div class="mb-1 text-body-2">
                            <strong>Country:</strong> {{ selectedCountryName || 'Not selected' }}
                          </div>
                          <div class="mb-1 text-body-2">
                            <strong>City/Address:</strong> {{ values.address || 'Not provided' }}
                          </div>
                          <div class="mb-1 text-body-2">
                            <strong>Immigration Status:</strong> {{ values.status || 'Not selected' }}
                          </div>
                          <div class="mb-1 text-body-2">
                            <strong>Transportation:</strong> {{ values.hasVehicle ? 'Own Vehicle' : 'Public Transit' }}
                          </div>
                          <div v-if="values.skills && values.skills.length > 0" class="mb-1 text-body-2">
                            <strong>Skills:</strong>
                            <v-chip v-for="skill in values.skills" :key="skill" size="small" class="ma-1">
                              {{ skill }}
                            </v-chip>
                          </div>
                          <div v-if="values.resume" class="mb-1 text-body-2">
                            <strong>Resume:</strong> {{ values.resume.name }}
                          </div>
                        </v-card-text>
                      </v-card>
                    </v-col>
                  </v-row>
                </v-form>
              </v-card>
            </template>

            <!-- ACTIONS -->
            <template v-slot:actions="{ prev, next }">
              <div class="d-flex align-center justify-space-between w-100 ga-2 stepper-actions">
                <div>
                  <v-btn v-if="currentStep > 1" @click="prev" :disabled="submitting">
                    Back
                  </v-btn>
                </div>
                <div>
                  <v-btn v-if="currentStep < 3" color="primary" @click="handleNext(next)" :disabled="submitting">
                    Next
                  </v-btn>
                  <v-btn v-else color="success" @click="handleSubmit" :loading="submitting">
                    Submit Application
                  </v-btn>
                </div>
              </div>
            </template>

          </v-stepper>
        </div>

        <!-- ALREADY REGISTERED TAB -->
        <div v-else-if="activeTab === 'registered'">
          <v-card flat class="pa-2">
            <v-card-title class="text-h6 mb-2">Already Registered</v-card-title>
            <v-form>
              <v-row>
                <v-col cols="12">
                  <v-text-field v-model="registeredEmail" label="Email *" type="email"
                    :error-messages="registeredEmailError" variant="outlined"
                    hint="Enter the email you used to register with us" persistent-hint></v-text-field>
                </v-col>
              </v-row>
            </v-form>

            <!-- Actions for Already Registered -->
            <div class="d-flex justify-end w-100 mt-4">
              <v-btn color="success" @click="handleSubmitRegistered" :loading="submitting" :disabled="!registeredEmail">
                Submit Application
              </v-btn>
            </div>
          </v-card>
        </div>
      </v-card-text>
    </v-card>
  </v-dialog>

  <!-- Error Snackbar -->
  <v-snackbar v-model="errorSnackbar" color="error" location="top" :timeout="5000">
    {{ errorMessage }}
  </v-snackbar>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import Cleave from 'cleave.js'
import { useApplicationForm } from '@/composables/useApplicationForm'
import { applicationService } from '@/services/applicationService'
import { locationService } from '@/services/locationService'
import { RESIDENCY_STATUS } from '@/services/types/application.types'
import type { Job } from '@/services/types/job.types'
import type { Country } from '@/services/types/application.types'

const props = defineProps<{
  modelValue: boolean
  selectedJob?: Job | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'application-submitted': []
}>()

const isOpen = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const { currentStep, values, errors, fields, validateCurrentStep, resetForm, setFieldValue } =
  useApplicationForm()

const activeTab = ref('new')
const countries = ref<Country[]>([])
const loadingCountries = ref(false)
const suggestedSkills = ref<string[]>([])
const loadingSkills = ref(false)
const submitting = ref(false)
const phoneFormatted = ref('')
const resumeFiles = ref<File[]>([])
const registeredEmail = ref('')
const registeredEmailError = ref('')
const errorSnackbar = ref(false)
const errorMessage = ref('')

const stepItems = ['Personal Information', 'Additional Details', 'Resume & Terms']

const dialogTitle = computed(() => {
  if (props.selectedJob) {
    return `Apply for ${props.selectedJob.title}`
  }
  return 'Register with Us'
})

const selectedCountryName = computed(() => {
  const country = countries.value.find((c) => c.id === values.countryId)
  return country?.value || ''
})

// Cleave.js instance for phone formatting
let cleaveInstance: Cleave | null = null

onMounted(async () => {
  // Load countries
  loadingCountries.value = true
  try {
    countries.value = await locationService.getCountries()
  } catch (error) {
    console.error('Failed to load countries:', error)
  } finally {
    loadingCountries.value = false
  }

  // Load suggested skills
  loadingSkills.value = true
  try {
    suggestedSkills.value = await locationService.getSkills()
  } catch (error) {
    console.error('Failed to load skills:', error)
  } finally {
    loadingSkills.value = false
  }
})

function handlePhoneInput(event: Event) {
  const target = event.target as HTMLInputElement

  if (!cleaveInstance) {
    cleaveInstance = new Cleave(target, {
      delimiters: [' ', '-'],
      blocks: [3, 3, 4],
      numericOnly: true
    })
  }

  phoneFormatted.value = target.value
  setFieldValue('phone', target.value)
}

function handleResumeChange(files: File | File[] | null) {
  if (files) {
    // Handle both single File and File array
    const file = Array.isArray(files) ? files[0] : files
    setFieldValue('resume', file)
  } else {
    setFieldValue('resume', null)
  }
}

async function handleNext(next: () => void) {
  const isValid = await validateCurrentStep()
  if (isValid) {
    next()
  }
}

async function handleSubmit() {
  const isValid = await validateCurrentStep()
  if (!isValid) return

  submitting.value = true
  try {
    await applicationService.submitApplication(values as any, props.selectedJob?.requestId)
    emit('application-submitted')
    resetForm()
    phoneFormatted.value = ''
    resumeFiles.value = []
    registeredEmail.value = ''
  } catch (error) {
    console.error('Failed to submit application:', error)
    errorMessage.value = 'Failed to submit application. Please try again.'
    errorSnackbar.value = true
  } finally {
    submitting.value = false
  }
}

async function handleSubmitRegistered() {
  // Validate email
  if (!registeredEmail.value) {
    registeredEmailError.value = 'Email is required'
    return
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(registeredEmail.value)) {
    registeredEmailError.value = 'Invalid email format'
    return
  }

  registeredEmailError.value = ''
  submitting.value = true

  try {
    // Submit just email with requestId (for already registered users)
    await applicationService.submitRegisteredApplication(
      registeredEmail.value,
      props.selectedJob?.requestId
    )
    emit('application-submitted')
    registeredEmail.value = ''
  } catch (error) {
    console.error('Failed to submit application:', error)
    errorMessage.value = 'Failed to submit application. Please try again.'
    errorSnackbar.value = true
  } finally {
    submitting.value = false
  }
}

function handleClose() {
  if (!submitting.value) {
    isOpen.value = false
  }
}

// Reset form when dialog reopens
watch(isOpen, (newVal) => {
  if (newVal) {
    currentStep.value = 1
    activeTab.value = 'new'
    registeredEmail.value = ''
    registeredEmailError.value = ''
  } else {
    // Clean up Cleave instance when dialog closes
    if (cleaveInstance) {
      cleaveInstance.destroy()
      cleaveInstance = null
    }
  }
})

// Clear email error when user types
watch(registeredEmail, () => {
  if (registeredEmailError.value) {
    registeredEmailError.value = ''
  }
})
</script>

<style scoped>
.dialog-content {
  /* Let dialog size naturally to content */
}

.stepper-actions {
  padding: 16px;
}
</style>
