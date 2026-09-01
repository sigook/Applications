<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message v-if="!isEdit" type="is-info" has-icon>
      The user will receive an email to confirm and create a password. If you don't receive an email,
      please check your spam or junk mail folder.
    </b-message>
    <b-message v-else type="is-warning" has-icon>
      The role is global: changing it also changes it in every agency this user belongs to, and it
      takes effect the next time they sign in.
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
        <b-field :type="formErrors.role ? 'is-danger' : ''" label="Role *"
          :message="formErrors.role || (isOwnUser ? 'You cannot change your own role' : '')">
          <b-select v-model="role" name="role" placeholder="Select a role" expanded :disabled="isOwnUser">
            <option v-for="option in assignableRoles" :key="option" :value="option">
              {{ roleLabels[option] || option }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="validateForm">{{ isEdit ? 'Save' : 'Create' }}</b-button>
      </div>
    </div>
  </div>
</template>


<script setup lang="ts">
import { computed, ref } from 'vue';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import { createAgencyPersonnel, updateAgencyPersonnel, getAssignableRoles } from "@/api/agencyApi";
import { roleLabels } from "@/security/roles";
import { useSecurityStore } from "@/stores/security";
import { useStickyForm } from '@/composables/useStickyForm';
import type { AgencyPersonnelListItem } from '@/types/agency';

const schema = yup.object({
  name: yup.string().required('Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
  role: yup.string().required('Role is required'),
});

const props = defineProps<{ personnel?: AgencyPersonnelListItem | null }>();
const emit = defineEmits<{ (e: 'updateUsers'): void }>();

const securityStore = useSecurityStore();

const form = useStickyForm<{ name: string; email: string; role: string }>({
  schema,
  initialValues: { name: '', email: '', role: '' },
});
const { name, email, role } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const assignableRoles = ref<string[]>([]);

const isEdit = computed(() => !!props.personnel);
const isOwnUser = computed(() => !!props.personnel && props.personnel.userId === securityStore.user?.profile.sub);

if (props.personnel) {
  form.hydrate({
    name: props.personnel.name || '',
    email: props.personnel.email || '',
    role: props.personnel.role || '',
  });
}

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
  saveUser();
}

function saveUser() {
  const model = { name: name.value, email: email.value, role: role.value };
  isLoading.value = true;
  const request = props.personnel
    ? updateAgencyPersonnel(props.personnel.id, model)
    : createAgencyPersonnel(model);
  request
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
