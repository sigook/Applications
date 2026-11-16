<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message v-if="!user" type="is-info" has-icon>
      The user will receive an email to confirm and create a password. If you don't receive an email,
      please check your spam or junk mail folder.
    </b-message>
    <b-message v-else-if="emailEditable" type="is-warning" has-icon>
      If you change the email the user will not be able to login with the previous one anymore,
      if necessary notify the user about the change.
    </b-message>
    <div class="columns is-multiline">
      <div class="column is-6">
        <b-field :type="formErrors.name ? 'is-danger' : ''" label="Name *"
          :message="formErrors.name">
          <b-input type="text" v-model="name" name="name" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field :type="formErrors.lastname ? 'is-danger' : ''" label="Last Name *"
          :message="formErrors.lastname">
          <b-input type="text" v-model="lastname" name="lastname" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field :type="formErrors.position ? 'is-danger' : ''" label="Position"
          :message="formErrors.position">
          <b-input type="text" v-model="position" name="position" />
        </b-field>
      </div>
      <div class="column is-6">
        <phone-input :required="false" :model="'Mobile'" :default-value="user?.mobileNumber"
          @formattedPhone="(phone) => mobileNumber = phone"></phone-input>
      </div>
      <div class="column is-6">
        <b-field :type="formErrors.email ? 'is-danger' : ''" label="Email *"
          :message="formErrors.email">
          <b-input type="email" v-model="email" name="email" :disabled="!isEmailEditable" />
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="validateForm">{{ user ? 'Save' : 'Create' }}</b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import * as yup from 'yup';
import { showAlertError } from "@/shared/utils/toast";
import type { CompanyUserModel, CreateCompanyUserModel } from '@/shared/company-profile/types';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import PhoneInput from "@/shared/ui/PhoneInput.vue";

const schema = yup.object({
  name: yup.string().required('Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  lastname: yup.string().required('Last Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  position: yup.string().nullable().transform(v => v || null)
    .min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
});

const { save, user = null, emailEditable = false } = defineProps<{
  save: (model: CreateCompanyUserModel) => Promise<unknown>;
  user?: CompanyUserModel | null;
  emailEditable?: boolean;
}>();
const emit = defineEmits<(e: 'updateUsers') => void>();

const form = useStickyForm<{ name: string; lastname: string; position: string; email: string }>({
  schema,
  initialValues: {
    name: user?.name ?? '',
    lastname: user?.lastname ?? '',
    position: user?.position ?? '',
    email: user?.email ?? '',
  },
});
const { name, lastname, position, email } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const mobileNumber = ref<string | null>(user?.mobileNumber || null);
const isEmailEditable = computed(() => !user || emailEditable);

async function validateForm() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) {
    showAlertError('Please make sure all required fields are filled out correctly');
    return;
  }
  await onSave();
}

async function onSave() {
  isLoading.value = true;
  const model = {
    name: name.value,
    lastname: lastname.value,
    position: position.value || null,
    email: email.value,
    mobileNumber: mobileNumber.value,
  };
  try {
    await save(model);
    isLoading.value = false;
    emit('updateUsers');
  } catch (error) {
    isLoading.value = false;
    showAlertError((error as { data?: unknown }).data);
  }
}
</script>
