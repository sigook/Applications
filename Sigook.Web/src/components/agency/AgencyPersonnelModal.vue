<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message type="is-info" has-icon>
      The user will receive an email to confirm and create a password. If you don't receive an email,
      please check your spam or junk mail folder.
    </b-message>
    <div class="columns is-multiline">
      <div class="column is-6">
        <b-field :type="formErrors.name ? 'is-danger' : ''" label="Name *"
          :message="formErrors.name">
          <b-input type="text" v-model="name" name="name" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field :type="formErrors.email ? 'is-danger' : ''" label="Email *"
          :message="formErrors.email">
          <b-input type="email" v-model="email" name="email" />
        </b-field>
      </div>
      <div class="column is-12">
        <b-field :type="formErrors.role ? 'is-danger' : ''" label="Role *" :message="formErrors.role">
          <b-select v-model="role" name="role" placeholder="Select a role" expanded>
            <option v-for="option in assignableRoles" :key="option" :value="option">
              {{ roleLabels[option] || option }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="validateForm">Create</b-button>
      </div>
    </div>
  </div>
</template>


<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import { createAgencyPersonnel, getAssignableRoles } from "@/api/agencyApi";
import { roleLabels } from "@/security/roles";
import { useStickyForm } from '@/composables/useStickyForm';

const schema = yup.object({
  name: yup.string().required('Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
  role: yup.string().required('Role is required'),
});

const emit = defineEmits<{ (e: 'updateUsers'): void }>();

const form = useStickyForm<{ name: string; email: string; role: string }>({
  schema,
  initialValues: { name: '', email: '', role: '' },
});
const { name, email, role } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const assignableRoles = ref<string[]>([]);

loadAssignableRoles();

function loadAssignableRoles() {
  isLoading.value = true;
  getAssignableRoles()
    .then((response) => {
      isLoading.value = false;
      assignableRoles.value = response;
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
  createUser();
}

function createUser() {
  isLoading.value = true;
  createAgencyPersonnel({ name: name.value, email: email.value, role: role.value })
    .then(() => {
      isLoading.value = false;
      emit("updateUsers");
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
