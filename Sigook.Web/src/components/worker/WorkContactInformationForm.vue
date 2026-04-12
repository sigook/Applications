<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <location-address v-model:model="worker.location" @isLoading="(value) => isLoading = value" />
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input :required="true" model="Mobile Number" :defaultValue="worker.mobileNumber"
          @formattedPhone="(phone) => worker.mobileNumber = phone" />
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input :required="false" model="Phone" :defaultValue="worker.phone"
          @formattedPhone="(phone) => worker.phone = phone" />
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">{{ $t("Save") }}</b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { createWorkerContactInformation } from '@/api/workerApi';

export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      worker: {}
    }
  },
  components: {
    LocationAddress: () => import("@/components/Address.vue"),
    phoneInput: () => import("@/components/PhoneInput.vue")
  },
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerContactInformation();
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    createWorkerContactInformation() {
      this.isLoading = true;
      createWorkerContactInformation(this.worker.id, this.worker)
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
      this.worker.location = Object.assign({}, this.data.location);
    }
  }
}
</script>