<template>
  <div class="p-3">
    <div class="modal-overflow" v-if="candidate">
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
            <b-field label="Full Name" :type="formErrors.name ? 'is-danger' : ''"
              :message="formErrors.name">
              <b-input type="text" v-model="name" name="name" />
            </b-field>
          </div>
          <div class="col-12 mb-1">
            <b-field :type="formErrors.email ? 'is-danger' : ''" label="Email"
              :message="formErrors.email">
              <b-input type="email" v-model="email" name="email" />
            </b-field>
          </div>
          <div class="col-12 mb-3">
            <b-field :type="formErrors.address ? 'is-danger' : ''" label="Address"
              :message="formErrors.address">
              <b-input type="text" v-model="address" name="address" />
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
          <div class="col-12">
            <b-field label="Has Vehicle">
              <b-switch v-model="candidate.hasVehicle" :true-value="true" :false-value="false">
                {{ candidate.hasVehicle ? "Yes" : "No" }}
              </b-switch>
            </b-field>
          </div>
        </div>
        <b-button type="is-primary" rounded native-type="submit">Update</b-button>
      </form>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed } from 'vue';
import * as yup from 'yup';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { getGenders } from "@/api/catalogApi";
import { residencyList } from "@/constants/catalog";
import { getAgencyCandidate, updateAgencyCandidate } from "@/api/agencyCandidateApi";
import { useStickyForm } from '@/composables/useStickyForm';

const props = defineProps<{ candidateId: number | string }>();
const emit = defineEmits<{ (e: 'onUpdateWorker', value: boolean): void }>();

const billingAdmin = useBillingAdmin();

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

const genders = computed(() => genderList.value);
const hasDnuPermission = computed(() => {
  if (!candidate.value || !candidate.value.dnu) {
    return false;
  } else if (candidate.value.dnu && billingAdmin.isPayrollManager) {
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
  if (!valid) {
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

getGenders()
  .then((result) => {
    genderList.value = result;
    loadCandidate();
  })
  .catch(error => {
    showAlertError(error);
  });
</script>
