<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <location-address ref="addressComponent" v-model:model="worker.location" @isLoading="(value) => isLoading = value" />
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input ref="mobileComponent" :required="true" model="Mobile Number" :defaultValue="worker.mobileNumber"
          @formattedPhone="(phone) => worker.mobileNumber = phone" />
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input ref="phoneComponent" :required="false" model="Phone" :defaultValue="worker.phone"
          @formattedPhone="(phone) => worker.phone = phone" />
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">{{ "Save" }}</b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { showAlertError } from "@/utils/toast";
import { createWorkerContactInformation } from '@/api/workerApi';

export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      worker: {} as any
    }
  },
  components: {
    LocationAddress: defineAsyncComponent(() => import("@/components/Address.vue")),
    phoneInput: defineAsyncComponent(() => import("@/components/PhoneInput.vue"))
  },
  methods: {
    async validateAll() {
      const addressValid = await (this.$refs.addressComponent as any).validateAddress();
      const mobileValid = await (this.$refs.mobileComponent as any).validatePhone();
      const phoneValid = await (this.$refs.phoneComponent as any).validatePhone();
      if (addressValid && mobileValid && phoneValid) {
        this.createWorkerContactInformation();
      } else {
        showAlertError('Please make sure all required fields are filled out correctly');
      }
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
          showAlertError(error);
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
