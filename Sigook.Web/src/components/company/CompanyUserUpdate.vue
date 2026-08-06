<template>
  <div class="columns is-multiline">
    <div class="column is-6">
      <b-field :type="formErrors.name ? 'is-danger' : ''" label="Name"
        :message="formErrors.name">
        <b-input type="text" v-model="name" name="name" />
      </b-field>
    </div>
    <div class="column is-6">
      <b-field :type="formErrors.lastname ? 'is-danger' : ''" label="Last Name"
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
      <PhoneInput :required="false" model="Mobile Number" :default-value="mobileNumber"
        @formattedPhone="(phone: string) => mobileNumber = phone" />
    </div>
    <div class="column is-12 mt-5">
      <b-button type="is-primary" @click="update">Save</b-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import * as yup from 'yup';
import { showAlertSuccess } from "@/utils/toast";
import PhoneInput from "@/components/PhoneInput.vue";
import { updateCompanyUser } from "@/api/companyApi";
import type { CompanyUserModel } from "@/types/company";
import { useStickyForm } from '@/composables/useStickyForm';

const props = defineProps<{ user: CompanyUserModel }>();
const emit = defineEmits<{ (e: 'update:user', payload: any): void }>();

const schema = yup.object({
  name: yup.string().required('Name is required').min(2, 'Min 2 characters').max(50, 'Max 50 characters'),
  lastname: yup.string().required('Last Name is required').min(2, 'Min 2 characters').max(50, 'Max 50 characters'),
  position: yup.string().nullable().transform(v => v || null)
    .min(2, 'Min 2 characters').max(50, 'Max 50 characters'),
});

const form = useStickyForm<{ name: string; lastname: string; position: string }>({
  schema,
  initialValues: { name: '', lastname: '', position: '' },
});
const { name, lastname, position } = form.fields;
const formErrors = form.errors;

const userId = ref<any>(null);
const mobileNumber = ref<string | null>(null);

function hydrateFromUser(u: any) {
  if (!u) return;
  userId.value = u.id;
  mobileNumber.value = u.mobileNumber || null;
  form.hydrate({
    name: u.name || '',
    lastname: u.lastname || '',
    position: u.position || '',
  });
}

hydrateFromUser(props.user);

watch(() => props.user, (newVal) => {
  hydrateFromUser(newVal);
}, { deep: true });

async function update() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) return;
  const payload: CompanyUserModel = {
    ...props.user,
    name: name.value,
    lastname: lastname.value,
    position: position.value || '',
    mobileNumber: mobileNumber.value || '',
  };
  emit('update:user', payload);
  updateCompanyUser(userId.value, payload)
    .then(() => {
      showAlertSuccess("Updated");
    });
}
</script>
