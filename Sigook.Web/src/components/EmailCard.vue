<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-card" :class="{ 'edit': !disabled }">
      <label>{{ $t('Name') }}:
        <input type="text" v-model="localItem.name" placeholder="Name" :name="'name' + index"
          v-validate="'required|max:50|min:3'" :class="{ 'is-danger': errors.has('name' + index) }" :disabled="disabled">
        <span v-show="errors.has('name' + index)" class="help is-danger no-margin">{{ errors.first('name') }}</span>
      </label>
      <label>{{ $t('Email') }}:
        <input type="text" v-model="localItem.email" placeholder="Email" :name="'email' + index"
          v-validate="'required|max:50|min:6|email'" :class="{ 'is-danger': errors.has('email' + index) }"
          :disabled="disabled">
        <span v-show="errors.has('email' + index)" class="help is-danger no-margin">{{ errors.first('email') }}</span>
      </label>

    </div>
    <div class="actions">
      <button v-if="disabled" @click="toogleEditInput()">
        <img src="../assets/images/edit-button.svg" alt="edit">
      </button>
      <button v-if="!disabled" @click="validateUpdate(localItem, index)">
        <img src="../assets/images/checked-accent.png" alt="edit">
      </button>
      <button @click="onDeleteInvoiceRecipient(localItem, index)">
        <img src="../assets/images/delete-icon.png" alt="edit">
      </button>
    </div>
  </div>
</template>

<script lang="ts">
import { deleteCompanyInvoiceRecipient, updateCompanyInvoiceRecipient } from "@/api/agencyCompanyApi";
export default {
  props: ['index', 'item'],
  inject: ['$validator'],
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
          this.$emit("updateDataEmailList", index)
        })
        .catch(error => {
          this.showAlertError(error);
          this.isLoading = false;
        });
    },
    toogleEditInput() {
      this.disabled = false;
    },
    validateUpdate(item, index) {
      let valid = true;
      Promise.all([
        this.$validator.validate('email' + index),
        this.$validator.validate('name' + index),
      ]).then(isValid => {
        isValid.forEach(function (value) {
          if (value === false) {
            valid = false;
          }
        });

        if (valid) {
          this.onUpdateInvoiceRecipient(item);
        }
      });
    },
    onUpdateInvoiceRecipient(item) {
      this.isLoading = true;
      updateCompanyInvoiceRecipient(this.$route.params.id, item.id, { name: item.name, email: item.email })
        .then(() => {
          this.disabled = true;
          this.isLoading = false;
        })
        .catch(error => {
          this.showAlertError(error);
          this.isLoading = false;
        });
    }
  }
}
</script>