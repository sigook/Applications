<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <form @submit.prevent="validateAll">
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.firstName ? 'is-danger' : ''" :label="'Name'"
            :message="formErrors.firstName || ''">
            <b-input type="text" v-model="firstName" name="name" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.middleName ? 'is-danger' : ''" :label="'Middle Name'"
            :message="formErrors.middleName || ''">
            <b-input type="text" v-model="middleName" name="middle name" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.lastName ? 'is-danger' : ''" :label="'Last Name'"
            :message="formErrors.lastName || ''">
            <b-input type="text" v-model="lastName" name="lastname" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.secondLastName ? 'is-danger' : ''"
            :label="'Other Family Name'"
            :message="formErrors.secondLastName || ''">
            <b-input type="text" v-model="secondLastName" name="second lastname" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.birthDay ? 'is-danger' : ''" :label="'Date of birth'"
            :message="formErrors.birthDay || ''">
            <b-datepicker v-model="birthDay" name="birthday" :focused-date="disabledDates" :max-date="disabledDates" expanded>
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.gender ? 'is-danger' : ''" :label="'Gender'"
            :message="formErrors.gender || ''">
            <b-select v-model="gender" placeholder="Select gender" name="gender" expanded>
              <option v-for="item in genders" :key="item.id" :value="item">{{ item.value }}</option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field :label="'Do you have your own vehicle?'">
            <b-switch v-model="hasVehicle" :true-value="true" :false-value="false">
              {{ hasVehicle ? "Yes" : "No" }}
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
import * as yup from 'yup';
import { mapStores } from 'pinia';
import { useAppStore } from '@/stores/app';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import dayjs from "dayjs";
import { getGenders } from "@/api/catalogApi";
import { createWorkerBasicInformation } from '@/api/workerApi';

const schema = yup.object({
  firstName: yup.string().required('Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  middleName: yup.string().nullable().max(20, 'Max 20 characters'),
  lastName: yup.string().required('Last Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  secondLastName: yup.string().nullable().min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  birthDay: yup.mixed().required('Date of birth is required'),
  gender: yup.mixed().required('Gender is required'),
});

export default {
  props: ['data'],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        firstName: '',
        middleName: '',
        lastName: '',
        secondLastName: '',
        birthDay: null as Date | null,
        gender: null as any,
      },
    });

    function hydrate(src: any) {
      form.hydrate({
        firstName: src?.firstName || '',
        middleName: src?.middleName || '',
        lastName: src?.lastName || '',
        secondLastName: src?.secondLastName || '',
        birthDay: src?.birthDay ? new Date(src.birthDay) : null,
        gender: src?.gender || null,
      });
    }

    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrate,
    };
  },
  data() {
    return {
      isLoading: false,
      workerId: null as any,
      hasVehicle: false,
      disabledDates: null as any,
      genders: [] as any[],
    };
  },
  computed: {
    ...mapStores(useAppStore),
  },
  methods: {
    validateAll() {
      this.markInteracted(['firstName', 'middleName', 'lastName', 'secondLastName', 'birthDay', 'gender']);
      this.handleSubmit((values) => {
        this.submitWorkerBasicInformation(values);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    submitWorkerBasicInformation(values: any) {
      this.isLoading = true;
      const payload = {
        ...values,
        hasVehicle: this.hasVehicle,
      };
      createWorkerBasicInformation(this.workerId, payload)
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    }
  },
  async created() {
    if (this.data != null) {
      this.workerId = this.data.id;
      this.hasVehicle = !!this.data.hasVehicle;
      this.hydrate(this.data);
    }
    this.appStore.getCurrentDate().then(response => {
      this.disabledDates = dayjs(response).subtract(18, 'years').toDate();
    });
    this.genders = await getGenders();
  }
}
</script>
