<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <form @submit.prevent="validateAll">
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('name') ? 'is-danger' : ''" :label="$t('WorkerName')"
            :message="errors.has('name') ? errors.first('name') : ''">
            <b-input type="text" v-model="worker.firstName" name="name" v-validate="'required|max:20|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('middle name') ? 'is-danger' : ''" :label="$t('WorkerMiddleName')"
            :message="errors.has('middle name') ? errors.first('middle name') : ''">
            <b-input type="text" v-model="worker.middleName" name="middle name" v-validate="'max:20|min:1'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('lastname') ? 'is-danger' : ''" :label="$t('WorkerLastName')"
            :message="errors.has('lastname') ? errors.first('lastname') : ''">
            <b-input type="text" v-model="worker.lastName" name="lastname" v-validate="'required|max:20|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('second lastname') ? 'is-danger' : ''"
            :label="$t('WorkerSecondLastName')"
            :message="errors.has('second lastname') ? errors.first('second lastname') : ''">
            <b-input type="text" v-model="worker.secondLastName" name="second lastname" v-validate="'max:20|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('birthday') ? 'is-danger' : ''" :label="$t('WorkerBirthday')"
            :message="errors.has('birthday') ? errors.first('birthday') : ''">
            <b-datepicker v-model="worker.birthDay" name="birthday" :focused-date="disabledDates" :max-date="disabledDates" v-validate="'required'" expanded>
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('gender') ? 'is-danger' : ''" :label="$t('WorkerGender')"
            :message="errors.has('gender') ? errors.first('gender') : ''">
            <b-select v-model="worker.gender" placeholder="Select gender" name="gender" expanded
              v-validate="'required'">
              <option v-for="item in genders" :key="item.id" :value="item">{{ item.value }}</option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field :label="$t('WorkerHasVehicle')">
            <b-switch v-model="worker.hasVehicle" :true-value="true" :false-value="false">
              {{ worker.hasVehicle ? $t("Yes") : $t("No") }}
            </b-switch>
          </b-field>
        </div>
        <div class="col-12 mt-5">
          <b-button type="is-primary" native-type="submit">{{ $t("Save") }}</b-button>
        </div>
      </div>
    </form>
  </div>
</template>
<script>
import dayjs from "dayjs";

export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      worker: {},
      disabledDates: null,
    }
  },
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerBasicInformation();
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    createWorkerBasicInformation() {
      this.isLoading = true;
      this.$store.dispatch('worker/createWorkerBasicInformation', { profileId: this.worker.id, model: this.worker })
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
  async created() {
    if (this.data != null) {
      this.worker = Object.assign({}, this.data);
      this.worker.birthDay = new Date(this.worker.birthDay);
    }
    this.$store.dispatch('getCurrentDate').then(response => {
      this.disableStartDate = response;
      this.disabledDates = dayjs(response).subtract(18, 'years').toDate();
    });
    await this.$store.dispatch('getGenders');
  },
  computed: {
    genders() {
      return this.$store.state.catalog.genders;
    }
  }
}
</script>