<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <form @submit.prevent="createCandidate" class="p-3">
      <b-tabs v-model="activeTab" position="is-centered">
        <b-tab-item label="New Applicant">
          <b-steps v-model="activeStep" animated mobile-mode="compact" :has-navigation="false">
            <b-step-item step="1" label="Personal Info" :clickable="false">
              <div class="container-flex">
                <div class="col-12 col-padding">
                  <b-field :type="formErrors.fullName ? 'is-danger' : ''"
                    :message="formErrors.fullName">
                    <template #label>
                      Full Name <span class="has-text-danger">*</span>
                    </template>
                    <b-input v-model="fullName" name="fullName" has-counter maxLength="50"></b-input>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <b-field :type="formErrors.email ? 'is-danger' : ''"
                    :message="formErrors.email">
                    <template #label>
                      Email <span class="has-text-danger">*</span>
                    </template>
                    <b-input type="email" v-model="email" has-counter name="email" maxLength="50"></b-input>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <phone-input ref="phoneComponent" model="Phone" :required="isNewApplicantTab" :defaultValue="phoneValue"
                    @formattedPhone="(p) => phoneValue = p"></phone-input>
                </div>
                <div class="col-12 col-padding">
                  <b-field :type="formErrors.countryId ? 'is-danger' : ''"
                    :message="formErrors.countryId">
                    <template #label>
                      Country <span class="has-text-danger">*</span>
                    </template>
                    <b-select v-model="countryId" expanded name="country">
                      <option v-for="country in countries" :key="country.id" :value="country.id">{{ country.value }}
                      </option>
                    </b-select>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <b-field :type="formErrors.address ? 'is-danger' : ''"
                    :message="formErrors.address">
                    <template #label>
                      City <span class="has-text-danger">*</span>
                    </template>
                    <b-input v-model="address" name="address" maxLength="50" has-counter></b-input>
                  </b-field>
                </div>
              </div>
              <div class="step-navigation-buttons">
                <span></span>
                <b-button type="is-primary" @click="validateAndGoToStep(1)">Next</b-button>
              </div>
            </b-step-item>

            <b-step-item step="2" label="Additional Details" :clickable="false">
              <div class="container-flex">
                <div class="col-12 col-padding">
                  <b-field :type="formErrors.status ? 'is-danger' : ''"
                    :message="formErrors.status">
                    <template #label>
                      Immigration Status <span class="has-text-danger">*</span>
                    </template>
                    <b-select v-model="status" name="status" expanded>
                      <option value="Citizen">Citizen</option>
                      <option value="Work Permit">Work Permit</option>
                      <option value="Student">Student</option>
                      <option value="Permanent Resident">Permanent Resident</option>
                    </b-select>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <b-field label="Resume (Optional)">
                    <b-field class="file is-primary" :class="{ 'has-name': !!file }">
                      <b-upload v-model="file" class="file-label" accept=".pdf,.doc,.docx" rounded>
                        <span class="file-cta">
                          <b-icon class="file-icon" icon="upload"></b-icon>
                          <span class="file-label">{{ file ? file.name : "Click to upload" }}</span>
                        </span>
                      </b-upload>
                    </b-field>
                  </b-field>
                </div>
                <div class="col-12 col-padding">
                  <b-field label="Roles of Interest (you may type more than one)">
                    <b-taginput v-model="skills" open-on-focus icon="label" :maxlength="20" ellipsis
                      placeholder="Select or Add Skill" allow-new name="skills">
                    </b-taginput>
                  </b-field>
                </div>

                <div class="col-12 col-padding">
                  <b-field label="Transportation">
                    <b-switch v-model="hasVehicle" :true-value="true" :false-value="false">
                      {{ hasVehicle ? "Own Vehicle" : "Public Transit" }}
                    </b-switch>
                  </b-field>
                </div>
              </div>
              <div class="step-navigation-buttons">
                <b-button @click="goToPreviousStep()">Previous</b-button>
                <b-button type="is-primary" @click="validateAndGoToStep(2)">Next</b-button>
              </div>
            </b-step-item>

            <b-step-item step="3" label="Review & Submit" :clickable="false">
              <div class="container-flex">
                <div class="col-12 col-padding">
                  <b-field :type="formErrors.termsAndConditions ? 'is-danger' : ''"
                    :message="formErrors.termsAndConditions">
                    <b-checkbox v-model="termsAndConditions" name="termsAndConditions">
                      I agree to the
                      <router-link to="/terms-and-conditions" target="_blank">Terms and Conditions</router-link>
                      &
                      <router-link to="/privacy-policy" target="_blank">Privacy Policy</router-link>
                    </b-checkbox>
                  </b-field>
                </div>

                <div class="col-12 col-padding">
                  <div class="box">
                    <h3 class="title is-5 mb-3">Application Summary</h3>
                    <div class="content">
                      <p><strong>Full Name:</strong> {{ fullName || 'Not provided' }}</p>
                      <p><strong>Email:</strong> {{ email || 'Not provided' }}</p>
                      <p><strong>Phone:</strong> {{ phoneValue || 'Not provided' }}</p>
                      <p><strong>Country:</strong> {{ selectedCountryName || 'Not selected' }}</p>
                      <p><strong>City:</strong> {{ address || 'Not provided' }}</p>
                      <p><strong>Immigration Status:</strong> {{ status || 'Not selected' }}</p>
                      <p><strong>Transportation:</strong> {{ hasVehicle ? 'Own Vehicle' : 'Public Transit' }}</p>
                      <p v-if="skills && skills.length > 0">
                        <strong>Skills:</strong>
                        <span v-for="(skill, index) in skills" :key="index">
                          <b-tag type="is-info" class="ml-1">{{ skill }}</b-tag>
                        </span>
                      </p>
                      <p v-if="file"><strong>Resume:</strong> {{ file.name }}</p>
                    </div>
                  </div>
                </div>
              </div>
              <div class="step-navigation-buttons">
                <b-button @click="goToPreviousStep()">Previous</b-button>
                <b-button type="is-primary" native-type="submit" :disabled="!termsAndConditions">Submit Application</b-button>
              </div>
            </b-step-item>
          </b-steps>
        </b-tab-item>
        <b-tab-item label="Already Registered" :visible="jobToApply">
          <div class="container-flex">
            <div class="col-12 col-padding">
              <b-field :type="formErrors.email ? 'is-danger' : ''" label="Email"
                :message="formErrors.email">
                <b-input type="email" v-model="email" maxlength="50" has-counter name="emailRegistered"></b-input>
              </b-field>
            </div>
          </div>
        </b-tab-item>
      </b-tabs>

    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import { generateFileName } from "@/utils/buildWorkerFormData";
import { getCountries } from "@/api/locationApi";
import { submitCandidate } from "@/api/websiteApi";
import { useStickyForm } from '@/composables/useStickyForm';
import PhoneInput from "@/components/PhoneInput.vue";

type ApplyFields = {
  fullName: string;
  email: string;
  countryId: string | null;
  address: string;
  status: string;
  termsAndConditions: boolean;
};

const props = defineProps<{ jobToApply?: any }>();
const emit = defineEmits<{ (e: 'candidateCreated'): void }>();

const activeTab = ref(0);
const isNewApplicantTab = computed(() => activeTab.value === 0);

const schema = computed(() => yup.object({
  fullName: isNewApplicantTab.value
    ? yup.string().required('Full Name is required').max(50, 'Max 50 characters')
    : yup.string().nullable().transform(v => v || null).max(50, 'Max 50 characters'),
  email: yup.string().required('Email is required').email('Invalid email').max(50, 'Max 50 characters'),
  countryId: isNewApplicantTab.value
    ? yup.string().nullable().required('Country is required')
    : yup.string().nullable(),
  address: isNewApplicantTab.value
    ? yup.string().required('City is required').max(50, 'Max 50 characters')
    : yup.string().nullable().transform(v => v || null),
  status: isNewApplicantTab.value
    ? yup.string().required('Immigration Status is required')
    : yup.string().nullable().transform(v => v || null),
  termsAndConditions: isNewApplicantTab.value
    ? yup.boolean().oneOf([true], 'You must accept the Terms and Conditions').required()
    : yup.boolean(),
}));

const form = useStickyForm<ApplyFields>({
  schema: schema as any,
  initialValues: {
    fullName: '',
    email: '',
    countryId: null,
    address: '',
    status: '',
    termsAndConditions: false,
  },
});
const { fullName, email, countryId, address, status, termsAndConditions } = form.fields;
const formErrors = form.errors;

watch(isNewApplicantTab, (newVal) => {
  form.setFieldValue('termsAndConditions', !newVal as any);
});

const activeStep = ref(0);
const isLoading = ref(false);
const countries = ref<any[]>([]);
const file = ref<File | null>(null);
const skills = ref<string[]>([]);
const hasVehicle = ref(false);
const phoneValue = ref<string | null>(null);
const requestId = ref<any>(null);
const phoneComponent = ref<any>(null);

const selectedCountryName = computed(() => {
  const country = countries.value.find((c: any) => c.id === countryId.value);
  return country ? country.value : '';
});

(async () => {
  countries.value = await getCountries();
})();

function goToPreviousStep() {
  if (activeStep.value > 0) {
    activeStep.value--;
  }
}

async function validateAndGoToStep(currentStep: number) {
  let valid = false;
  if (currentStep === 1) {
    valid = await validateStep1();
  } else if (currentStep === 2) {
    valid = await validateStep2();
  }
  if (valid) {
    activeStep.value++;
  } else {
    showAlertError("Please verify that the required fields are correctly filled in.");
  }
}

async function validateStep1() {
  const fields = ['fullName', 'email', 'countryId', 'address'];
  form.markInteracted(fields);
  const results = await Promise.all(fields.map((f: any) => form.validateField(f)));
  const phoneValid = await phoneComponent.value.validatePhone();
  return results.every((r: any) => r.valid) && phoneValid;
}

async function validateStep2() {
  form.markInteracted(['status']);
  const r = await form.validateField('status');
  return r.valid;
}

async function createCandidate() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) return;
  isLoading.value = true;

  if (props.jobToApply) {
    requestId.value = props.jobToApply.requestId;
  }

  let formData: FormData;
  if (isNewApplicantTab.value) {
    formData = new FormData();
    let fileName: string | null = null;
    if (file.value) {
      fileName = generateFileName('Resume', file.value.name);
    }
    const candidateData = {
      fullName: fullName.value,
      email: email.value,
      phone: phoneValue.value,
      skills: skills.value,
      status: status.value || '',
      countryId: countryId.value,
      address: address.value,
      fileName,
      hasVehicle: hasVehicle.value,
      requestId: requestId.value,
    };
    formData.append('data', JSON.stringify(candidateData));
    if (file.value && fileName) {
      formData.append(fileName, file.value, fileName);
    }
  } else {
    formData = new FormData();
    const candidateData = {
      email: email.value,
      requestId: requestId.value,
    };
    formData.append('data', JSON.stringify(candidateData));
  }

  await submitCandidate(formData);
  emit('candidateCreated');
  isLoading.value = false;
}
</script>

<style scoped>
.step-navigation-buttons {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #e0e0e0;
}
</style>
