<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-card" :class="{ 'edit': !disabled }">
      <label>{{ $t('Name') }}:
        <input type="text" v-model="item.name" placeholder="Name" :name="'name' + index"
          v-validate="'required|max:50|min:3'" :class="{ 'is-danger': errors.has('name' + index) }" :disabled="disabled">
        <span v-show="errors.has('name' + index)" class="help is-danger no-margin">{{ errors.first('name') }}</span>
      </label>
      <label>{{ $t('Email') }}:
        <input type="text" v-model="item.email" placeholder="Email" :name="'email' + index"
          v-validate="'required|max:50|min:6|email'" :class="{ 'is-danger': errors.has('email' + index) }"
          :disabled="disabled">
        <span v-show="errors.has('email' + index)" class="help is-danger no-margin">{{ errors.first('email') }}</span>
      </label>

    </div>
    <div class="actions">
      <button v-if="disabled" @click="toogleEditInput()">
        <img src="../assets/images/edit-button.svg" alt="edit">
      </button>
      <button v-if="!disabled" @click="validateUpdate(item, index)">
        <img src="../assets/images/checked-accent.png" alt="edit">
      </button>
      <button @click="deleteCompanyInvoiceRecipient(item, index)">
        <img src="../assets/images/delete-icon.png" alt="edit">
      </button>
    </div>
  </div>
</template>

<script>
export default {
  props: ['index', 'item'],
  inject: ['$validator'],
  data() {
    return {
      isLoading: false,
      disabled: true
    }
  },
  methods: {
    deleteCompanyInvoiceRecipient(item, index) {
      this.isLoading = true;
      this.$store.dispatch('agency/deleteCompanyInvoiceRecipient', { companyProfileId: this.$route.params.id, id: item.id })
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
          this.updateCompanyInvoiceRecipient(item);
        }
      });
    },
    updateCompanyInvoiceRecipient(item) {
      this.isLoading = true;
      this.$store.dispatch('agency/updateCompanyInvoiceRecipient', { companyProfileId: this.$route.params.id, id: item.id, model: { name: item.name, email: item.email } })
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