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

<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { deleteCompanyInvoiceRecipient, updateCompanyInvoiceRecipient } from "@/api/agencyCompanyApi";

const schema = yup.object({
  name: yup.string().required('Name is required').min(3, 'Min 3 characters').max(50, 'Max 50 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
});

export default {
  props: ['index', 'item'],
  setup(props) {
    const form = useStickyForm({
      schema,
      initialValues: {
        name: (props.item && props.item.name) || '',
        email: (props.item && props.item.email) || '',
      },
    });
    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
    };
  },
  data() {
    return {
      isLoading: false,
      disabled: true,
      localItem: JSON.parse(JSON.stringify(this.item))
    }
  },
  watch: {
    item: {
      handler(newVal) {
        this.localItem = JSON.parse(JSON.stringify(newVal));
        this.hydrateForm({
          name: newVal?.name || '',
          email: newVal?.email || '',
        });
      },
      deep: true
    }
  },
  methods: {
    onDeleteInvoiceRecipient(item, index) {
      this.isLoading = true;
      deleteCompanyInvoiceRecipient(this.$route.params.id, item.id)
        .then(() => {
          this.isLoading = false;
          this.$emit("updateDataEmailList", index);
        })
        .catch(error => {
          showAlertError(error);
          this.isLoading = false;
        });
    },
    toogleEditInput() {
      this.disabled = false;
    },
    validateUpdate(_index) {
      this.markInteracted();
      this.handleSubmit((values) => {
        this.onUpdateInvoiceRecipient({ ...this.localItem, name: values.name, email: values.email });
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    onUpdateInvoiceRecipient(item) {
      this.isLoading = true;
      updateCompanyInvoiceRecipient(this.$route.params.id, item.id, { name: item.name, email: item.email })
        .then(() => {
          this.localItem = { ...item };
          this.disabled = true;
          this.isLoading = false;
        })
        .catch(error => {
          showAlertError(error);
          this.isLoading = false;
        });
    }
  }
}
</script>
