<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-card" :class="{ 'edit': !disabled }">
      <label>{{ 'Name' }}:
        <input type="text" v-model="name" placeholder="Name" :name="'name' + index"
          :class="{ 'is-danger': !!formErrors.name }" :disabled="disabled">
        <span v-show="formErrors.name" class="help is-danger no-margin">{{ formErrors.name }}</span>
      </label>
      <label>{{ 'Email' }}:
        <input type="text" v-model="email" placeholder="Email" :name="'email' + index"
          :class="{ 'is-danger': !!formErrors.email }" :disabled="disabled">
        <span v-show="formErrors.email" class="help is-danger no-margin">{{ formErrors.email }}</span>
      </label>

    </div>
    <div class="actions">
      <button v-if="disabled" @click="toogleEditInput()">
        <img src="../assets/images/edit-button.svg" alt="edit">
      </button>
      <button v-if="!disabled" @click="validateUpdate(index)">
        <img src="../assets/images/checked-accent.png" alt="edit">
      </button>
      <button @click="onDeleteInvoiceRecipient(localItem, index)">
        <img src="../assets/images/delete-icon.png" alt="edit">
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { deleteCompanyInvoiceRecipient, updateCompanyInvoiceRecipient } from "@/api/agencyCompanyApi";

const schema = yup.object({
  name: yup.string().required('Name is required').min(3, 'Min 3 characters').max(50, 'Max 50 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
});

const props = defineProps<{ index?: number; item?: any }>();
const emit = defineEmits<{ (e: 'updateDataEmailList', index?: number): void }>();

const route = useRoute();

const form = useStickyForm<{ name: string; email: string }>({
  schema,
  initialValues: {
    name: (props.item && props.item.name) || '',
    email: (props.item && props.item.email) || '',
  },
});
const { name, email } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const disabled = ref(true);
const localItem = ref<any>(JSON.parse(JSON.stringify(props.item)));

watch(() => props.item, (newVal) => {
  localItem.value = JSON.parse(JSON.stringify(newVal));
  form.hydrate({
    name: newVal?.name || '',
    email: newVal?.email || '',
  });
}, { deep: true });

function onDeleteInvoiceRecipient(item: any, index?: number) {
  isLoading.value = true;
  deleteCompanyInvoiceRecipient(route.params.id as string, item.id)
    .then(() => {
      isLoading.value = false;
      emit('updateDataEmailList', index);
    })
    .catch(error => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function toogleEditInput() {
  disabled.value = false;
}

function validateUpdate(_index?: number) {
  form.markInteracted();
  form.handleSubmit((values) => {
    onUpdateInvoiceRecipient({ ...localItem.value, name: values.name, email: values.email });
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function onUpdateInvoiceRecipient(item: any) {
  isLoading.value = true;
  updateCompanyInvoiceRecipient(route.params.id as string, item.id, { name: item.name, email: item.email })
    .then(() => {
      localItem.value = { ...item };
      disabled.value = true;
      isLoading.value = false;
    })
    .catch(error => {
      showAlertError(error);
      isLoading.value = false;
    });
}
</script>
