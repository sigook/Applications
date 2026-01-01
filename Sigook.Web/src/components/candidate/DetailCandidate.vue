<template>
  <div class="p-3">
    <div class="white-container-mobile modal-overflow" v-if="candidate">
      <b-loading v-model="isLoading"></b-loading>
      <h2 class="text-center main-title">{{ candidate.name }}</h2>
      <form @submit.prevent="validateForm">
        <div class="container-flex">
          <div class="col-12">
            <b-checkbox v-model="candidate.dnu" :disabled="hasDnuPermission">
              {{ $t("DNU") }}
            </b-checkbox>
          </div>
          <div class="col-12">
            <b-field label="Full Name" :type="errors.has('full name') ? 'is-danger' : ''">
              <b-input type="text" v-model="candidate.name" name="full name" v-validate="'required|max:60|min:2'" />
            </b-field>
            <span v-show="errors.has('full name')" class="help is-danger no-margin">
              {{ errors.first('full name') }}
            </span>
          </div>
          <div class="col-12 mb-1">
            <b-field :type="errors.has('email') ? 'is-danger' : ''" label="Email"
              :message="errors.has('email') ? errors.first('email') : ''">
              <b-input type="email" v-model="candidate.email" name="email" v-validate="'required|email|max:50|min:6'" />
            </b-field>
          </div>
          <div class="col-12 mb-3">
            <b-field :type="errors.has('address') ? 'is-danger' : ''" label="Address"
              :message="errors.has('address') ? errors.first('address') : ''">
              <b-input type="text" v-model="candidate.address" name="address" v-validate="'required|max:100|min:2'" />
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Status">
              <b-select v-model="candidate.residencyStatus" expanded placeholder="Select a residency status">
                <option v-for="(item, index) in residencyList" :key="index" :value="item">{{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Gender">
              <b-select v-model="candidate.gender" expanded placeholder="Select a gender">
                <option v-for="item in genders" v-bind:key="item.id" :value="item">{{ item.value }}</option>
              </b-select>
            </b-field>
          </div>
          <div class="col-12">
            <b-field label="Has Vehicle">
              <b-switch v-model="candidate.hasVehicle" :true-value="true" :false-value="false">
                {{ candidate.hasVehicle ? $t("Yes") : $t("No") }}
              </b-switch>
            </b-field>
          </div>
        </div>
        <button class="background-btn create-btn orange-button btn-radius" type="submit">{{ $t('Update') }}</button>
      </form>
    </div>
  </div>
</template>
<script>
import billingAdminMixin from "@/mixins/billingAdminMixin";

export default {
  props: ['candidateId'],
  data() {
    return {
      isLoading: false,
      candidate: {},
      showPostalCode: false
    }
  },
  methods: {
    getAgencyCandidate() {
      this.isLoading = true;
      this.$store.dispatch('agency/getAgencyCandidate', this.candidateId)
        .then(response => {
          this.isLoading = false;
          this.candidate = response
          this.showPostalCode = true;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    validateForm() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.updateCandidate();
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    updateCandidate() {
      this.isLoading = true;
      this.$store.dispatch('agency/updateCandidate', { candidateId: this.candidateId, model: this.candidate })
        .then(() => {
          this.isLoading = false
          this.showAlertSuccess('Updated');
          this.$emit('onUpdateWorker', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    }
  },
  created() {
    this.$store.dispatch('getGenders')
      .then(() => {
        this.getAgencyCandidate();
      })
      .catch(error => {
        this.showAlertError(error);
      })
  },
  mixins: [billingAdminMixin],
  computed: {
    genders() {
      return this.$store.state.catalog.genders;
    },
    residencyList() {
      return this.$store.state.catalog.residencyList
    },
    hasDnuPermission() {
      if (!this.candidate.dnu) {
        return false;
      } else if (
        this.candidate.dnu && this.isPayrollManager
      ) {
        return false;
      } else {
        return true;
      }
    },
  }
}
</script>