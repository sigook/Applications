<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-4">
        <b-field :label="'Do you have any health problems / allergies?'" class="has-text-weight-normal">
          <b-switch v-model="worker.haveAnyHealthProblem" v-validate="'required'">
            {{ worker.haveAnyHealthProblem ? 'Yes' : 'No' }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-4" v-if="worker.haveAnyHealthProblem">
        <b-field :label="`${'Which'} ?`" :type="errors.has('health problem') ? 'is-danger' : ''">
          <b-input type="text" v-model="worker.healthProblem" name="health problem"
            v-validate="{ required: true, min: 2, max: 20 }">
          </b-input>
        </b-field>
      </div>
      <div class="col-4" v-if="worker.haveAnyHealthProblem">
        <b-field :label="'Other allergies'" class="has-text-weight-normal">
          <b-input type="text" v-model="worker.otherHealthProblem">
          </b-input>
        </b-field>
      </div>
      <div class="col-12">
        <h1 class="fw-700">{{ 'In case of emergency notify' }}</h1>
      </div>
      <div class="col-6">
        <b-field :label="'Name'" :type="errors.has('contact emergency') ? 'is-danger' : ''">
          <b-input type="text" v-model="worker.contactEmergencyName" name="contact emergency"
            v-validate="'required|max:20|min:2'">
          </b-input>
        </b-field>
      </div>
      <div class="col-6">
        <b-field :label="'Last Name'" class="has-text-weight-normal"
          :type="errors.has('contact emergency lastname') ? 'is-danger' : ''">
          <b-input type="text" v-model="worker.contactEmergencyLastName" name="contact emergency lastname"
            v-validate="'required|max:20|min:2'" expanded>
          </b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <phone-input :required="true" model="Contact Emergency Phone" :defaultValue="worker.contactEmergencyPhone"
          @formattedPhone="(phone) => worker.contactEmergencyPhone = phone" />
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { showAlertError } from "@/utils/toast";
import { createWorkerEmergencyInformation } from '@/api/workerApi';

export default {
  props: ['data'],
  data() {
    return {
      worker: {},
      isLoading: false
    }
  },
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerEmergencyInformation();
          return;
        }
        showAlertError('Please make sure all required fields are filled out correctly');
      });
    },
    createWorkerEmergencyInformation() {
      this.isLoading = true;
      createWorkerEmergencyInformation(this.data.id, this.worker)
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
    }
  },
  components: {
    phoneInput: defineAsyncComponent(() => import("../PhoneInput.vue"))
  }
}
</script>