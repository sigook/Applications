<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message type="is-warning" has-icon>
      Please note that if you change this email the worker will
      not be able to login with the previous email anymore,
      if necessary notify the worker about the change.
    </b-message>
    <div class="container-flex">
      <div class="col-12">
        <b-field label="New Email" :type="errors.has('newEmail') ? 'is-danger' : ''"
          :message="errors.has('newEmail') ? errors.first('newEmail') : ''">
          <b-input type="email" v-model="newEmail" name="newEmail" v-validate="'required|email'" data-vv-as="newEmail"
            ref="newEmail">
          </b-input>
        </b-field>
      </div>
      <div class="col-12">
        <b-field label="Confirm Email" :type="errors.has('confirmEmail') ? 'is-danger' : ''"
          :message="errors.has('confirmEmail') ? errors.first('confirmEmail') : ''">
          <b-input type="email" @paste.prevent v-model="confirmEmail" name="confirmEmail"
            v-validate="{ required: true, confirmed: newEmail }">
          </b-input>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">
          {{ $t("Save") }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script>

export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      newEmail: "",
      confirmEmail: "",
      worker: {}
    }
  },
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.updateWorkerEmail();
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    updateWorkerEmail() {
      this.isLoading = true;
      this.$store.dispatch('agency/updateAgencyWorkerEmail', { workerProfileId: this.worker.id, model: { newEmail: this.newEmail } })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    }
  },
  created() {
    if (this.data != null) {
      this.worker = Object.assign({}, this.data);
    }
  }
}
</script>