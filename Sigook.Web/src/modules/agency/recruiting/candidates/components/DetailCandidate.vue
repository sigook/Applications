<template>
  <div class="p-3">
    <div v-if="candidate">
      <b-loading v-model="isLoading"></b-loading>
      <h2 class="has-text-centered main-title">{{ candidate.name }}</h2>
      <form @submit.prevent="validateForm">
        <div class="columns is-multiline">
          <div class="column is-12">
            <b-checkbox v-model="candidate.dnu" :disabled="hasDnuPermission">
              DNU
            </b-checkbox>
          </div>
          <div class="column is-12">
            <b-field :type="formErrors.name ? 'is-danger' : ''" :message="formErrors.name">
              <template #label>
                Full Name <span class="has-text-danger">*</span>
              </template>
              <b-input type="text" v-model="name" name="name" />
            </b-field>
          </div>
          <div class="column is-12 mb-1">
            <b-field :type="formErrors.email ? 'is-danger' : ''" :message="formErrors.email">
              <template #label>
                Email <span class="has-text-danger">*</span>
              </template>
              <b-input type="email" v-model="email" name="email" />
            </b-field>
          </div>
          <div class="column is-12">
            <PhoneInput ref="phoneComponent" model="Phone" :required="true" :defaultValue="phoneNumber"
              @formattedPhone="(phone: string) => phoneNumber = phone" />
          </div>
          <div class="column is-12 mb-3">
            <AddressComponent ref="addressComponent" v-model:model="location" :require-postal-code="false"
              @isLoading="(value: boolean) => isLoading = value" />
          </div>
          <div class="column is-12">
            <b-field :type="sourceError ? 'is-danger' : ''" :message="sourceError">
              <template #label>
                Source <span class="has-text-danger">*</span>
              </template>
              <b-select v-model="candidate.sourceId" expanded placeholder="Select a source">
                <option v-for="item in sourceList" :key="item.id" :value="item.id">{{ item.value }}</option>
              </b-select>
            </b-field>
          </div>
          <div class="column is-12">
            <b-field label="Status">
              <b-select v-model="candidate.residencyStatus" expanded placeholder="Select a residency status">
                <option v-for="(item, index) in residencyList" :key="index" :value="item">{{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="column is-12">
            <b-field label="Gender">
              <b-select v-model="candidate.gender" expanded placeholder="Select a gender">
                <option v-for="item in genders" v-bind:key="item.id" :value="item">{{ item.value }}</option>
              </b-select>
            </b-field>
          </div>
          <div class="column is-12 mb-3">
            <b-field label="Has Vehicle">
              <b-switch v-model="candidate.hasVehicle" :true-value="true" :false-value="false">
                {{ candidate.hasVehicle ? "Yes" : "No" }}
              </b-switch>
            </b-field>
          </div>
          <div class="column is-12">
            <b-button type="is-primary" rounded native-type="submit">Update</b-button>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import * as yup from 'yup';
import { showAlertError, showAlertSuccess } from "@/shared/utils/toast";
import { useAdmin } from '@/shared/composables/useAdmin';
import { getGenders, getSources } from "@/shared/api/catalogApi";
import { residencyList } from "@/shared/constants/catalog";
import { getAgencyCandidate, updateAgencyCandidate } from "@/modules/agency/recruiting/candidates/api";
import { useStickyForm } from '@/shared/composables/useStickyForm';
import PhoneInput from "@/shared/ui/PhoneInput.vue";
import AddressComponent from '@/shared/ui/Address.vue';
import type { CandidateDetail } from '@/modules/agency/recruiting/candidates/types';
import type { Gender, LocationModel, Source } from "@/shared/types/common";

const props = defineProps<{ candidateId: number | string }>();
const emit = defineEmits<{ (e: 'onUpdateWorker', value: boolean): void }>();

const { isAdmin } = useAdmin();

const schema = yup.object({
  name: yup.string().required('Full Name is required').min(2, 'Min 2 characters').max(60, 'Max 60 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
});

const form = useStickyForm<{ name: string; email: string }>({
  schema,
  initialValues: { name: '', email: '' },
});
const { name, email } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const candidate = ref<CandidateDetail | null>(null);
const genderList = ref<Gender[]>([]);
const sourceList = ref<Source[]>([]);
const sourceError = ref('');
const phoneNumber = ref('');
const phoneComponent = ref<InstanceType<typeof PhoneInput> | null>(null);
const addressComponent = ref<InstanceType<typeof AddressComponent> | null>(null);
const location = ref<LocationModel>({ address: '', postalCode: '', city: null });

const genders = computed(() => genderList.value);
const hasDnuPermission = computed(() => {
  if (!candidate.value || !candidate.value.dnu) {
    return false;
  } else if (candidate.value.dnu && isAdmin.value) {
    return false;
  } else {
    return true;
  }
});

function loadCandidate() {
  isLoading.value = true;
  getAgencyCandidate(String(props.candidateId))
    .then(response => {
      isLoading.value = false;
      location.value = {
        address: response.address || '',
        postalCode: response.postalCode || '',
        city: response.city ?? null,
      };
      candidate.value = response;
      phoneNumber.value = response.phoneNumbers?.[0]?.phoneNumber || '';
      form.hydrate({
        name: response.name || '',
        email: response.email || '',
      });
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

async function validateForm() {
  form.markInteracted();
  const { valid } = await form.validate();
  const phoneValid = phoneComponent.value ? await phoneComponent.value.validatePhone() : false;
  const addressValid = addressComponent.value ? await addressComponent.value.validateAddress() : false;
  sourceError.value = candidate.value.sourceId ? '' : 'Source is required';
  if (!valid || !phoneValid || !addressValid || sourceError.value) {
    showAlertError('Please make sure all required fields are filled out correctly');
    return;
  }
  submitCandidate();
}

function submitCandidate() {
  isLoading.value = true;
  const payload = {
    ...candidate.value,
    name: name.value,
    email: email.value,
    address: location.value.address,
    postalCode: location.value.postalCode || undefined,
    city: location.value.city ? { id: location.value.city.id, value: location.value.city.value } : null,
    phoneNumbers: [{ phoneNumber: phoneNumber.value }],
  };
  updateAgencyCandidate(String(props.candidateId), payload)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Updated');
      emit('onUpdateWorker', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

watch(() => candidate.value?.sourceId, () => { sourceError.value = ''; });

Promise.all([getGenders(), getSources()])
  .then(([genderResult, sourceResult]) => {
    genderList.value = genderResult;
    sourceList.value = sourceResult;
    loadCandidate();
  })
  .catch(error => {
    showAlertError(error);
  });
</script>
