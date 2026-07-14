<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message type="is-info" has-icon>
      The user will receive an email to confirm and create a password. If you don't receive an email,
      please check your spam or junk mail folder.
    </b-message>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.name ? 'is-danger' : ''" label="Name *"
          :message="formErrors.name">
          <b-input type="text" v-model="name" name="name" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.lastname ? 'is-danger' : ''" label="Last Name *"
          :message="formErrors.lastname">
          <b-input type="text" v-model="lastname" name="lastname" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.position ? 'is-danger' : ''" label="Position"
          :message="formErrors.position">
          <b-input type="text" v-model="position" name="position" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input :required="false" :model="'Mobile'"
          @formattedPhone="(phone) => mobileNumber = phone"></phone-input>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.email ? 'is-danger' : ''" label="Email *"
          :message="formErrors.email">
          <b-input type="email" v-model="email" name="email" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding mt-5">
        <b-button type="is-primary" @click="validateForm">Create</b-button>
      </div>
    </div>
  </div>
</template>


<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import { createCompanyUser } from '@/api/companyApi';
import { createCompanyProfileUser } from '@/api/agencyCompanyApi';
import { useStickyForm } from '@/composables/useStickyForm';
import PhoneInput from "@/components/PhoneInput.vue";

const schema = yup.object({
  name: yup.string().required('Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  lastname: yup.string().required('Last Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  position: yup.string().nullable().transform(v => v || null)
    .min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
});

const props = defineProps<{ profileId?: string }>();
const emit = defineEmits<{ (e: 'updateUsers'): void }>();

const form = useStickyForm<{ name: string; lastname: string; position: string; email: string }>({
  schema,
  initialValues: { name: '', lastname: '', position: '', email: '' },
});
const { name, lastname, position, email } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const mobileNumber = ref<string | null>(null);

async function validateForm() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) {
    showAlertError('Please make sure all required fields are filled out correctly');
    return;
  }
  onCreateUser();
}

function onCreateUser() {
  isLoading.value = true;
  const user = {
    name: name.value,
    lastname: lastname.value,
    position: position.value || null,
    email: email.value,
    mobileNumber: mobileNumber.value,
  };
  const action = props.profileId ?
    createCompanyProfileUser(props.profileId, user) :
    createCompanyUser(user);
  action
    .then(() => {
      isLoading.value = false;
      emit('updateUsers');
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}
</script>
