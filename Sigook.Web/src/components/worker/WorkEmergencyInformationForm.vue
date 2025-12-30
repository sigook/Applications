<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-4">
        <b-field :label="$t('WorkerDoYouHaveAnyHealthProblemsAllergies')" class="has-text-weight-normal">
          <b-switch v-model="worker.haveAnyHealthProblem" v-validate="'required'">
            {{ worker.haveAnyHealthProblem ? $t('Yes') : $t('No') }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-4" v-if="worker.haveAnyHealthProblem">
        <b-field :label="`${$t('WorkerWhich')} ?`" :type="errors.has('health problem') ? 'is-danger' : ''">
          <b-input type="text" v-model="worker.healthProblem" name="health problem"
            v-validate="{ required: true, min: 2, max: 20 }">
          </b-input>
        </b-field>
      </div>
      <div class="col-4" v-if="worker.haveAnyHealthProblem">
        <b-field :label="$t('WorkerOtherAllergies')" class="has-text-weight-normal">
          <b-input type="text" v-model="worker.otherHealthProblem">
          </b-input>
        </b-field>
      </div>
      <div class="col-12">
        <h1 class="fw-700">{{ $t('WorkerInCaseOfEmergencyNotify') }}</h1>
      </div>
      <div class="col-6">
        <b-field :label="$t('Name')" :type="errors.has('contact emergency') ? 'is-danger' : ''">
          <b-input type="text" v-model="worker.contactEmergencyName" name="contact emergency"
            v-validate="'required|max:20|min:2'">
          </b-input>
        </b-field>
      </div>
      <div class="col-6">
        <b-field :label="$t('LastName')" class="has-text-weight-normal"
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
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    createWorkerEmergencyInformation() {
      this.isLoading = true;
      this.$store.dispatch('worker/createWorkerEmergencyInformation', { profileId: this.data.id, model: this.worker })
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
  },
  components: {
    phoneInput: () => import("../PhoneInput")
  }
}
</script>