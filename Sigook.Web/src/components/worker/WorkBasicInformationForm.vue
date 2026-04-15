<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <form @submit.prevent="validateAll">
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('name') ? 'is-danger' : ''" :label="'Name'"
            :message="errors.has('name') ? errors.first('name') : ''">
            <b-input type="text" v-model="worker.firstName" name="name" v-validate="'required|max:20|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('middle name') ? 'is-danger' : ''" :label="'Middle Name'"
            :message="errors.has('middle name') ? errors.first('middle name') : ''">
            <b-input type="text" v-model="worker.middleName" name="middle name" v-validate="'max:20|min:1'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('lastname') ? 'is-danger' : ''" :label="'Last Name'"
            :message="errors.has('lastname') ? errors.first('lastname') : ''">
            <b-input type="text" v-model="worker.lastName" name="lastname" v-validate="'required|max:20|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('second lastname') ? 'is-danger' : ''"
            :label="'Other Family Name'"
            :message="errors.has('second lastname') ? errors.first('second lastname') : ''">
            <b-input type="text" v-model="worker.secondLastName" name="second lastname" v-validate="'max:20|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('birthday') ? 'is-danger' : ''" :label="'Date of birth'"
            :message="errors.has('birthday') ? errors.first('birthday') : ''">
            <b-datepicker v-model="worker.birthDay" name="birthday" :focused-date="disabledDates" :max-date="disabledDates" v-validate="'required'" expanded>
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('gender') ? 'is-danger' : ''" :label="'Gender'"
            :message="errors.has('gender') ? errors.first('gender') : ''">
            <b-select v-model="worker.gender" placeholder="Select gender" name="gender" expanded
              v-validate="'required'">
              <option v-for="item in genders" :key="item.id" :value="item">{{ item.value }}</option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field :label="'Do you have your own vehicle?'">
            <b-switch v-model="worker.hasVehicle" :true-value="true" :false-value="false">
              {{ worker.hasVehicle ? "Yes" : "No" }}
            </b-switch>
          </b-field>
        </div>
        <div class="col-12 mt-5">
          <b-button type="is-primary" native-type="submit">{{ "Save" }}</b-button>
        </div>
      </div>
    </form>
  </div>
</template>
<script lang="ts">
import { mapStores } from 'pinia';
import { useAppStore } from '@/stores/app';
import { showAlertError } from "@/utils/toast";
import dayjs from "dayjs";
import { getGenders } from "@/api/catalogApi";
import { createWorkerBasicInformation } from '@/api/workerApi';

export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      worker: {},
      disabledDates: null,
      genders: []
    }
  },
  computed: {
    ...mapStores(useAppStore),
  },
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerBasicInformation();
          return;
        }
        showAlertError('Please make sure all required fields are filled out correctly');
      });
    },
    createWorkerBasicInformation() {
      this.isLoading = true;
      createWorkerBasicInformation(this.worker.id, this.worker)
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
  async created() {
    if (this.data != null) {
      this.worker = Object.assign({}, this.data);
      this.worker.birthDay = new Date(this.worker.birthDay);
    }
    this.appStore.getCurrentDate().then(response => {
      this.disableStartDate = response;
      this.disabledDates = dayjs(response).subtract(18, 'years').toDate();
    });
    this.genders = await getGenders();
  }
}
</script>