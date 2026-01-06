<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> Role </h2>
    <div class="container-flex">
      <div class="col-12">
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
      <div class="col-12">
        <b-field :type="errors.has('message') ? 'is-danger' : ''" label="Message"
          :message="errors.has('message') ? errors.first('message') : ''">
          <b-input type="textarea" v-model="model.message" name="message" v-validate="{ max: 1000 }" />
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateForm">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ["profileId"],
  data() {
    return {
      isLoading: false,
      model: {
        id: null,
        jobPosition: null,
        message: ''
      },
      jobPositionList: [],
      jobPosition: ''
    }
  },
  methods: {
    async validateForm() {
      this.model.jobPosition = this.jobPosition;
      const result = await this.$validator.validateAll();
      if (result) {
        this.requestAgencyJobPosition();
      }
      else {
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      }
    },
    requestAgencyJobPosition() {
      this.isLoading = true;
      this.$store.dispatch('agency/petitionAgencyCompanyJobPosition', { profileId: this.profileId, model: this.model })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal');
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    }
  },
  async created() {
    this.isLoading = true;
    this.jobPositionList = await this.$store.dispatch('getJobPositions');
    this.isLoading = false;
  },
  computed: {
    filteredPositions() {
      const position = this.jobPositionList.filter(jpl => jpl.value.toLowerCase().includes(this.jobPosition.toLowerCase()));
      return position;
    }
  }
}
</script>