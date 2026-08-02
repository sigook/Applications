<template>
  <div class="p-3">
    <div v-if="candidate">
      <b-loading v-model="isLoading"></b-loading>
      <h2 class="text-center main-title">{{ candidate.name }}</h2>
      <form @submit.prevent="validateForm">
        <div class="container-flex">
          <div class="col-12">
            <b-checkbox v-model="candidate.dnu" :disabled="hasDnuPermission">
              DNU
            </b-checkbox>
          </div>
          <div class="col-12">
            <b-field :type="formErrors.name ? 'is-danger' : ''" :message="formErrors.name">
              <template #label>
                Full Name <span class="has-text-danger">*</span>
              </template>
              <b-input type="text" v-model="name" name="name" />
            </b-field>
          </div>
          <div class="col-12 mb-1">
            <b-field :type="formErrors.email ? 'is-danger' : ''" :message="formErrors.email">
              <template #label>
                Email <span class="has-text-danger">*</span>
              </template>
              <b-input type="email" v-model="email" name="email" />
            </b-field>
          </div>
          <div class="col-12">
            <PhoneInput ref="phoneComponent" model="Phone" :required="true" :defaultValue="phoneNumber"
              @formattedPhone="(phone: string) => phoneNumber = phone" />
          </div>
          <div class="col-12 mb-3">
            <b-field :type="formErrors.address ? 'is-danger' : ''" :message="formErrors.address">
              <template #label>
                Address <span class="has-text-danger">*</span>
              </template>
              <b-input type="text" v-model="address" name="address" />
            </b-field>
          </div>
          <div class="col-12">
            <b-field :type="sourceError ? 'is-danger' : ''" :message="sourceError">
              <template #label>
                Source <span class="has-text-danger">*</span>
              </template>
              <b-select v-model="candidate.sourceId" expanded placeholder="Select a source">
                <option v-for="item in sourceList" :key="item.id" :value="item.id">{{ item.value }}</option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Status">
              <b-select v-model="candidate.residencyStatus" expanded placeholder="Select a residency status">
                <option v-for="(item, index) in residencyList" :key="index" :value="item">{{ item }}
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
          <div class="col-12 mb-3">
            <b-field label="Has Vehicle">
              <b-switch v-model="candidate.hasVehicle" :true-value="true" :false-value="false">
                {{ candidate.hasVehicle ? "Yes" : "No" }}
              </b-switch>
            </b-field>
          </div>
          <div class="col-12">
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
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { useAdmin } from '@/composables/useAdmin';
import { getGenders, getSources } from "@/api/catalogApi";
import { residencyList } from "@/constants/catalog";
import { getAgencyCandidate, updateAgencyCandidate } from "@/api/agencyCandidateApi";
import { useStickyForm } from '@/composables/useStickyForm';
import PhoneInput from "@/components/PhoneInput.vue";
import type { Source } from "@/types/common";

const props = defineProps<{ candidateId: number | string }>();
const emit = defineEmits<{ (e: 'onUpdateWorker', value: boolean): void }>();

const { isAdmin } = useAdmin();

const schema = yup.object({
  name: yup.string().required('Full Name is required').min(2, 'Min 2 characters').max(60, 'Max 60 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
  address: yup.string().required('Address is required').min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
});

const form = useStickyForm<{ name: string; email: string; address: string }>({
  schema,
  initialValues: { name: '', email: '', address: '' },
});
const { name, email, address } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const candidate = ref<any>(null);
const showPostalCode = ref(false);
const genderList = ref<any[]>([]);
const sourceList = ref<Source[]>([]);
const sourceError = ref('');
const phoneNumber = ref('');
const phoneComponent = ref<any>(null);

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
      candidate.value = response;
      showPostalCode.value = true;
      phoneNumber.value = response.phoneNumbers?.[0]?.phoneNumber || '';
      form.hydrate({
        name: response.name || '',
        email: response.email || '',
        address: response.address || '',
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
  sourceError.value = candidate.value.sourceId ? '' : 'Source is required';
  if (!valid || !phoneValid || sourceError.value) {
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
    address: address.value,
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
