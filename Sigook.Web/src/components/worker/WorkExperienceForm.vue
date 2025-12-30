<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('company') ? 'is-danger' : ''" :label="$t('Company')"
          :message="errors.has('company') ? errors.first('company') : ''">
          <b-input type="text" v-model="workExperience.company" :name="'company'"
            v-validate="'required|max:50|min:2'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('supervisor') ? 'is-danger' : ''" :label="$t('Supervisor')"
          :message="errors.has('supervisor') ? errors.first('supervisor') : ''">
          <b-input type="text" v-model="workExperience.supervisor" :name="'supervisor'"
            v-validate="'required|max:60|min:2'" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :type="errors.has('duties') ? 'is-danger' : ''" :label="$t('Duties')"
          :message="errors.has('duties') ? errors.first('duties') : ''">
          <b-input type="textarea" v-model="workExperience.duties" :name="'duties'" v-validate="'required|max:5000'" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :label="$t('CurrentJob')">
          <b-switch v-model="workExperience.isCurrentJobPosition" :name="'isCurrentJobPosition'" :true-value="true"
            :false-value="false">
            {{ workExperience.isCurrentJobPosition ? $t('Yes') : $t('No') }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('startDate') ? 'is-danger' : ''" :label="$t('StartDate')"
          :message="errors.has('startDate') ? errors.first('startDate') : ''">
          <b-datepicker v-model="workExperience.startDate" :name="'startDate'" v-validate="'required'"
            :max-date="disableStartDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding" v-if="!workExperience.isCurrentJobPosition">
        <b-field :type="errors.has('endDate') ? 'is-danger' : ''" :label="$t('EndDate')"
          :message="errors.has('endDate') ? errors.first('endDate') : ''">
          <b-datepicker v-model="workExperience.endDate" :name="'endDate'" v-validate="'required'"
            :max-date="disableStartDate" :min-date="workExperience.startDate" append-to-body position="is-top-right">
          </b-datepicker>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="validateAll">
          {{ $t("Save") }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: ['workerId'],
  data() {
    return {
      isLoading: false,
      disableStartDate: null,
      workExperience: {
        company: "",
        supervisor: "",
        duties: "",
        startDate: null,
        endDate: null,
        isCurrentJobPosition: true
      }
    }
  },
  methods: {
    validateAll() {
      this.$validator.validateAll().then(async (isValid) => {
        if (isValid) {
          this.createWorkerWorkExperience();
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    createWorkerWorkExperience() {
      this.isLoading = true;
      this.$store.dispatch("worker/createWorkerWorkExperience", { id: this.workerId, model: this.workExperience })
        .then(() => {
          this.isLoading = false;
          this.$emit("updateExperience");
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    }
  },
  created() {
    this.$store.dispatch('getCurrentDate').then(response => {
      this.disableStartDate = response;
    })
  },
}
</script>