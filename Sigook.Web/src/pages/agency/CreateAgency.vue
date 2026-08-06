<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title is-flex is-flex-wrap-wrap mb-5">
      <h2 class="fz1 pt-3">Create Agency</h2>
    </div>
    <form @submit.prevent="validateForm">
      <div class="columns is-multiline">
        <div class="column is-6 is-3-desktop">
          <b-field label="Full Name" :type="formErrors.fullName ? 'is-danger' : ''"
            :message="formErrors.fullName || ''">
            <b-input type="text" v-model="fullName" name="full name" />
          </b-field>
        </div>
        <div class="column is-6 is-3-desktop">
          <b-field :type="formErrors.email ? 'is-danger' : ''" label="Email"
            :message="formErrors.email || ''">
            <b-input type="email" v-model="email" name="email" />
          </b-field>
        </div>
        <div class="column is-6 is-3-desktop">
          <phone-input ref="phoneComponent" :required="true" model="Phone" :defaultValue="phoneNumber"
            @formattedPhone="(phone) => phoneNumber = phone"></phone-input>
        </div>
        <div class="column is-6 is-3-desktop">
          <b-field :type="formErrors.agencyType ? 'is-danger' : ''" label="Agency Type"
            :message="formErrors.agencyType || ''">
            <b-select v-model="agencyType" name="agency type" placeholder="Select agency type" expanded>
              <option v-for="type in agencyTypes" :key="type.value" :value="type.value">
                {{ type.label }}
              </option>
            </b-select>
          </b-field>
        </div>
        <div class="column is-6 is-3-desktop">
          <b-field :type="formErrors.password ? 'is-danger' : ''" label="Password"
            :message="formErrors.password || ''">
            <b-input type="password" v-model="password" name="password" password-reveal />
          </b-field>
        </div>
        <div class="column is-12">
          <b-button type="is-primary" native-type="submit">{{ 'Create' }}</b-button>
        </div>
      </div>
    </form>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { createAgency } from '@/api/agencyApi';
import { appGlobals } from '@/varaibles';
import PhoneInput from '@/components/PhoneInput.vue';

const router = useRouter();

const schema = yup.object({
  fullName: yup.string().required('Full name is required').min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
  agencyType: yup.mixed().required('Agency type is required'),
  password: yup.string().required('Password is required').min(6, 'Min 6 characters').max(100, 'Max 100 characters'),
});

const form = useStickyForm({
  schema,
  initialValues: {
    fullName: '',
    email: '',
    agencyType: null as number | null,
    password: '',
  },
});
const { fullName, email, agencyType, password } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const phoneNumber = ref('');
const agencyTypes = appGlobals.$agencyTypes;
const phoneComponent = ref<any>(null);

async function validateForm() {
  form.markInteracted();
  const phoneValid = await phoneComponent.value?.validatePhone();
  form.handleSubmit((values) => {
    if (!phoneValid) {
      showAlertError('Please make sure all required fields are filled out correctly');
      return;
    }
    submitAgency(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function submitAgency(values: any) {
  isLoading.value = true;
  const payload = {
    fullName: values.fullName,
    email: values.email,
    agencyType: values.agencyType,
    password: values.password,
    phonePrincipal: phoneNumber.value,
  };
  createAgency(payload)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Agency created successfully');
      router.push('/sales/agencies');
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
