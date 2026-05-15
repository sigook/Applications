<template>
  <div class="p-3 white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <form @submit.prevent="submit">
      <b-steps v-model="activeStep" animated mobile-mode="compact" :has-navigation="false">
        <b-step-item step="1" label="Basic" :clickable="false">
          <div class="container-flex">
            <div class="col-12">
              <b-field :type="errors.name ? 'is-danger' : ''" :message="errors.name">
                <template #label>
                  Full Name <span class="has-text-danger">*</span>
                </template>
                <b-input type="text" v-model="name" />
              </b-field>
            </div>
            <div class="col-12">
              <b-field :type="errors.email ? 'is-danger' : ''" :message="errors.email">
                <template #label>
                  Email <span class="has-text-danger">*</span>
                </template>
                <b-input type="email" v-model="email" />
              </b-field>
            </div>
            <div class="col-12">
              <PhoneInput ref="phoneComponent" model="Phone" :required="true" :defaultValue="phoneNumber"
                @formattedPhone="(phone: string) => phoneNumber = phone" />
            </div>
            <div class="col-12">
              <b-field :type="errors.address ? 'is-danger' : ''" :message="errors.address">
                <template #label>
                  Address <span class="has-text-danger">*</span>
                </template>
                <b-input type="text" v-model="address" />
              </b-field>
            </div>
            <div class="col-12">
              <b-field :type="sourceError ? 'is-danger' : ''" :message="sourceError">
                <template #label>
                  Source <span class="has-text-danger">*</span>
                </template>
                <b-select v-model="candidate.source" expanded placeholder="Select a source">
                  <option v-for="(item, index) in sourceList" :value="item" :key="'sourceItem' + index">{{ item }}
                  </option>
                </b-select>
              </b-field>
            </div>
            <div class="col-12 mt-5">
              <b-field class="file is-primary" :class="{ 'has-name': !!file }">
                <b-upload v-model="file" class="file-label" accept=".pdf,.doc,.docx" @update:modelValue="uploadResume" rounded>
                  <span class="file-cta">
                    <b-icon class="file-icon" icon="upload"></b-icon>
                    <span class="file-label">{{ file ? file.name : "Click to upload" }}</span>
                  </span>
                </b-upload>
              </b-field>
            </div>
          </div>
          <div class="step-navigation-buttons">
            <span></span>
            <b-button type="is-primary" @click="validateAndGoToStep(1)">Next</b-button>
          </div>
        </b-step-item>
        <b-step-item step="2" label="Additionals" :clickable="false">
          <div class="col-12">
            <b-field label="Skills (Press enter to add)">
              <b-taginput v-model="candidate.skills" :maxlength="20" open-on-focus icon="label" placeholder="Add Skills" allow-new>
              </b-taginput>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Status">
              <b-select v-model="candidate.residencyStatus" expanded placeholder="Select a residency status">
                <option v-for="(item, index) in residencyList" :key="'residency' + index" :value="item">{{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Gender">
              <b-select v-model="candidate.gender" expanded placeholder="Select a gender">
                <option v-for="item in genders" v-bind:key="item.id" :value="item">{{ item.value }}</option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Has Vehicle">
              <b-switch v-model="candidate.hasVehicle" :true-value="true" :false-value="false">
                {{ candidate.hasVehicle ? "Yes" : "No" }}
              </b-switch>
            </b-field>
          </div>
          <div class="step-navigation-buttons">
            <b-button @click="goToPreviousStep()">Previous</b-button>
            <b-button type="is-primary" native-type="submit">Create</b-button>
          </div>
        </b-step-item>
      </b-steps>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue';
import { useForm, useField } from 'vee-validate';
import * as yup from 'yup';
import PhoneInput from "@/components/PhoneInput.vue";
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { uploadFile } from "@/utils/fileUpload";
import { getGenders, getSources } from "@/api/catalogApi";
import { residencyList } from "@/constants/catalog";
import { createAgencyCandidate } from "@/api/agencyCandidateApi";

const emit = defineEmits<{ (e: 'onClose', value: boolean): void }>();

const schema = yup.object({
  name: yup.string().required('Full name is required').min(2).max(60),
  email: yup.string().required('Email is required').email('Invalid email').min(6).max(50),
  address: yup.string().required('Address is required').min(2).max(100),
});

const { errors: formErrors, validateField, handleSubmit } = useForm({
  validationSchema: schema,
  initialValues: { name: '', email: '', address: '' },
});

const { value: name } = useField<string>('name');
const { value: email } = useField<string>('email');
const { value: address } = useField<string>('address');

const interacted = reactive<Record<string, boolean>>({});
watch(name, () => { interacted.name = true; });
watch(email, () => { interacted.email = true; });
watch(address, () => { interacted.address = true; });

const errors = computed(() => {
  const out: Record<string, string> = {};
  for (const key of Object.keys(formErrors.value)) {
    out[key] = interacted[key] ? (formErrors.value[key] || '') : '';
  }
  return out;
});

function markInteracted(fields: string[]) {
  for (const f of fields) interacted[f] = true;
}

const activeStep = ref(0);
const isLoading = ref(false);
const genders = ref<Array<{ id: number; value: string }>>([]);
const sourceList = ref<string[]>([]);
const sourceError = ref('');
const phoneNumber = ref('');
const phoneComponent = ref<any>(null);
const candidate = reactive<{
  phoneNumbers: Array<{ phoneNumber: string }>;
  skills: string[];
  source: string | null;
  gender: { id: number; value: string } | null;
  hasVehicle: boolean;
  residencyStatus: string | null;
  fileName: string | null;
}>({
  phoneNumbers: [],
  skills: [],
  source: null,
  gender: null,
  hasVehicle: false,
  residencyStatus: null,
  fileName: null,
});
const file = ref<File | null>(null);

watch(() => candidate.source, () => { sourceError.value = ''; });

async function validateStep1(): Promise<boolean> {
  const fields = ['name', 'email', 'address'];
  markInteracted(fields);
  const results = await Promise.all(fields.map((f) => validateField(f as any)));
  const fieldsValid = results.every((r: any) => r.valid);
  const phoneValid = phoneComponent.value ? await phoneComponent.value.validatePhone() : false;
  sourceError.value = candidate.source ? '' : 'Source is required';
  return fieldsValid && phoneValid && !sourceError.value;
}

async function validateAndGoToStep(currentStep: number) {
  let valid = false;
  if (currentStep === 1) {
    valid = await validateStep1();
  }
  if (valid) {
    activeStep.value++;
  } else {
    showAlertError('Please make sure all required fields are filled out correctly');
  }
}

function goToPreviousStep() {
  if (activeStep.value > 0) activeStep.value--;
}

function submit() {
  handleSubmit(async (values: any) => {
    const phoneValid = phoneComponent.value ? await phoneComponent.value.validatePhone() : false;
    if (!phoneValid) {
      activeStep.value = 0;
      showAlertError('Please make sure all required fields are filled out correctly');
      return;
    }
    submitCandidate(values);
  })();
}

function submitCandidate(values: any) {
  isLoading.value = true;
  const payload: any = {
    name: values.name,
    email: values.email,
    address: values.address,
    skills: candidate.skills.map((s: string) => ({ skill: s })),
    phoneNumbers: phoneNumber.value ? [{ phoneNumber: phoneNumber.value }] : [],
    source: candidate.source,
    gender: candidate.gender,
    hasVehicle: candidate.hasVehicle,
    residencyStatus: candidate.residencyStatus,
    fileName: candidate.fileName,
  };
  createAgencyCandidate(payload)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess("Created");
      emit('onClose', true);
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

async function uploadResume(f: File) {
  isLoading.value = true;
  const response = await uploadFile(f, 'document', 'Resume_');
  candidate.fileName = response;
  isLoading.value = false;
}

(async () => {
  [genders.value, sourceList.value] = await Promise.all([getGenders(), getSources()]);
})();
</script>

<style lang="scss" scoped>
.step-navigation-buttons {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #e0e0e0;
}
</style>
