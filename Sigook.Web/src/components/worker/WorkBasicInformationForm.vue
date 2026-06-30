<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <form @submit.prevent="validateAll">
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.firstName ? 'is-danger' : ''"
            :message="formErrors.firstName || ''">
            <template #label>
              Name <span class="has-text-danger">*</span>
            </template>
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
          <b-field :type="formErrors.lastName ? 'is-danger' : ''"
            :message="formErrors.lastName || ''">
            <template #label>
              Last Name <span class="has-text-danger">*</span>
            </template>
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
          <b-field :type="formErrors.birthDay ? 'is-danger' : ''"
            :message="formErrors.birthDay || ''">
            <template #label>
              Date of birth <span class="has-text-danger">*</span>
            </template>
            <b-datepicker v-model="birthDay" name="birthday" :focused-date="disabledDates" :max-date="disabledDates" expanded>
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.gender ? 'is-danger' : ''"
            :message="formErrors.gender || ''">
            <template #label>
              Gender <span class="has-text-danger">*</span>
            </template>
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
<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useAppStore } from '@/stores/app';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import dayjs from "dayjs";
import { getGenders } from "@/api/catalogApi";
import { createWorkerBasicInformation } from '@/api/workerApi';

interface BasicInfoForm {
  firstName: string;
  middleName: string;
  lastName: string;
  secondLastName: string;
  birthDay: Date | null;
  gender: any;
}

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const schema = yup.object({
  firstName: yup.string().required('Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  middleName: yup.string().nullable().max(20, 'Max 20 characters'),
  lastName: yup.string().required('Last Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  secondLastName: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  birthDay: yup.mixed().required('Date of birth is required'),
  gender: yup.mixed().required('Gender is required'),
});

const form = useStickyForm<BasicInfoForm>({
  schema,
  initialValues: {
    firstName: '',
    middleName: '',
    lastName: '',
    secondLastName: '',
    birthDay: null,
    gender: null,
  },
});
const { firstName, middleName, lastName, secondLastName, birthDay, gender } = form.fields;
const formErrors = form.errors;

const appStore = useAppStore();

const isLoading = ref(false);
const workerId = ref<any>(null);
const hasVehicle = ref(false);
const disabledDates = ref<any>(null);
const genders = ref<any[]>([]);

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

function submitWorkerBasicInformation(values: any) {
  isLoading.value = true;
  const payload = {
    ...values,
    hasVehicle: hasVehicle.value,
  };
  createWorkerBasicInformation(workerId.value, payload)
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function validateAll() {
  form.markInteracted(['firstName', 'middleName', 'lastName', 'secondLastName', 'birthDay', 'gender']);
  form.handleSubmit((values) => {
    submitWorkerBasicInformation(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

if (props.data != null) {
  workerId.value = props.data.id;
  hasVehicle.value = !!props.data.hasVehicle;
  hydrate(props.data);
}
appStore.getCurrentDate().then(response => {
  disabledDates.value = dayjs(response).subtract(18, 'years').toDate();
});
(async () => {
  genders.value = await getGenders();
})();
</script>
