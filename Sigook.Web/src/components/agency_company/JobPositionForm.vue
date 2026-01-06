<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field label="Position">
          <b-autocomplete :data="filteredPositions" placeholder="Position" v-model="jobPosition" field="value"
            open-on-focus name="positions" v-validate="'required'">
            <template v-slot="props">
              <span class="fz-0">{{ props.option.value }}</span>
              <span v-if="props.option.industry" class="fz-2 d-block">Industry: {{ props.option.industry }}</span>
            </template>
          </b-autocomplete>
        </b-field>
      </div>
      <div v-if="isPayrollManager" class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field :label="'Agency Rate'" :type="errors.has('rate') ? 'is-danger' : ''"
          :message="errors.has('rate') ? errors.first('rate') : ''">
          <b-numberinput v-model="model.rate" name="rate" controls-alignment="right"
            v-validate="{ required: true, max_value: 999999, min_value: 0.1 }" step="0.01" placeholder="10.00">
          </b-numberinput>
        </b-field>
      </div>
      <div
        :class="isPayrollManager ? 'col-sm-12 col-md-4 col-lg-4 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding'">
        <b-field :label="'Worker Rate'" :type="errors.has('workerRate') ? 'is-danger' : ''"
          :message="errors.has('workerRate') ? errors.first('workerRate') : ''">
          <b-numberinput v-model="model.workerRate" :disabled="!isPayrollManager" name="workerRate"
            controls-alignment="right"
            v-validate="{ required: true, max_value: model.rate > 0 ? model.rate : 50, min_value: 0.1 }" step="0.01"
            placeholder="10.00">
          </b-numberinput>
        </b-field>
      </div>
      <div
        :class="isPayrollManager ? 'col-sm-12 col-md-4 col-lg-4 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding'">
        <b-field :label="'Worker Rate Max'" :type="errors.has('workerRateMax') ? 'is-danger' : ''"
          :message="errors.has('workerRateMax') ? errors.first('workerRateMax') : ''">
          <b-numberinput v-model="model.workerRateMax" name="workerRateMax" controls-alignment="right"
            v-validate="{ required: false, max_value: model.rate > 0 ? model.rate : 50, min_value: model.workerRate > 0 ? model.workerRate : 0 }"
            step="0.01" placeholder="10.00">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :label="'Description'" :type="errors.has('description') ? 'is-danger' : ''"
          :message="errors.has('description') ? errors.first('description') : ''">
          <b-input type="textarea" v-model="model.description" name="description" v-validate="{ max: 1000 }">
          </b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding" v-if="currentPosition">
        <!-- UPDATE -->
        <shift v-if="model.shift" :is-update="true" :current-shift="model.shift"
          @updateModel="(shift) => model.shift = shift" />
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding" v-else>
        <!-- CREATE -->
        <shift :is-update="false" @updateModel="(shift) => model.shift = shift" />
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateForm">
          {{ currentPosition ? $t('Save') : $t('Create') }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script>
import billingAdminMixin from "@/mixins/billingAdminMixin";
export default {
  props: ['currentPosition', 'profileId'],
  data() {
    return {
      isLoading: false,
      model: {
        id: null,
        jobPosition: null,
        rate: null,
        otherJobPosition: null,
        workerRate: null,
        description: null,
        workerRateMin: null,
        workerRateMax: null,
        shift: null,
      },
      jobPositionList: [],
      jobPosition: ''
    }
  },
  mixins: [billingAdminMixin],
  components: {
    Shift: () => import("../request/ShiftsForm")
  },
  methods: {
    async validateForm() {
      const existingJobPosition = this.jobPositionList.find(jpl => jpl.value === this.jobPosition);
      if (existingJobPosition) {
        this.model.jobPosition = existingJobPosition;
      } else {
        this.model.otherJobPosition = this.jobPosition;
      }
      this.model.workerRateMin = this.model.workerRate;
      const result = await this.$validator.validateAll();
      if (result) {
        if (this.currentPosition) {
          this.updateAgencyCompanyJobPosition(this.currentPosition.id);
        } else {
          this.createAgencyCompanyJobPosition();
        }
      } else {
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      }
    },
    createAgencyCompanyJobPosition() {
      this.isLoading = true;
      this.$store.dispatch('agency/createAgencyCompanyJobPosition', { profileId: this.profileId, model: this.model })
        .then(response => {
          this.isLoading = false;
          this.$emit('updateContent');
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    updateAgencyCompanyJobPosition(id) {
      this.isLoading = true;
      this.$store.dispatch('agency/updateAgencyCompanyJobPosition', { profileId: this.profileId, id: id, model: this.model })
        .then(() => {
          this.isLoading = false;
          this.$emit('updateContent');
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    getAgencyCompanyJobPositionById(id) {
      this.isLoading = true;
      this.$store.dispatch('agency/getAgencyCompanyJobPositionById', { profileId: this.profileId, id: id })
        .then(response => {
          this.model = response;
          this.jobPosition = response.value;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
  },
  async created() {
    this.isLoading = true;
    this.jobPositionList = await this.$store.dispatch('getJobPositions')
    if (this.currentPosition && this.currentPosition.id) {
      this.getAgencyCompanyJobPositionById(this.currentPosition.id);
    }
    this.isLoading = false;
  },
  computed: {
    filteredPositions() {
      const position = this.jobPositionList.filter(jpl => jpl.value.toLowerCase().includes(this.jobPosition.toLowerCase()));
      return position
    }
  }
}
</script>