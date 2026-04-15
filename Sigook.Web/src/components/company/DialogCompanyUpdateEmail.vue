<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <b-message type="is-warning" has-icon>
      Please note that if you change this email the worker will
      not be able to login with the previous email anymore,
      if necessary notify the worker about the change.
    </b-message>

    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field label="New Email" :type="errors.has('newEmail') ? 'is-danger' : ''"
          :message="errors.has('newEmail') ? errors.first('newEmail') : ''">
          <b-input type="email" v-model="newEmail" name="newEmail" v-validate="'required|email'" data-vv-as="newEmail"
            ref="newEmail">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field label="Confirm Email" :type="errors.has('confirmEmail') ? 'is-danger' : ''"
          :message="errors.has('confirmEmail') ? errors.first('confirmEmail') : ''">
          <b-input type="email" @paste.prevent v-model="confirmEmail" name="confirmEmail"
            v-validate="{ required: true, confirmed: newEmail }">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateAll()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">

import { showAlertError } from "@/utils/toast";
import { updateAgencyCompanyEmail } from "@/api/agencyCompanyApi";

export default {
  name: "DialogCompanyUpdateEmail",
  props: ['companyProfileId'],
  data() {
    return {
      isLoading: false,
      newEmail: "",
      confirmEmail: ""
    }
  },
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.updateEmail();
          return;
        }
        showAlertError('Please make sure all required fields are filled out correctly');
      });
    },
    updateEmail() {
      this.isLoading = true;
      updateAgencyCompanyEmail(this.companyProfileId, { newEmail: this.newEmail }).then(() => {
        this.isLoading = false;
        this.$emit('closeModal', true, this.newEmail);
      })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    }
  }
}

</script>