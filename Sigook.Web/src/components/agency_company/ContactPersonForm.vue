<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :label="'Title *'" :type="formErrors.title ? 'is-danger' : ''"
          :message="formErrors.title || ''">
          <b-select v-model="title" name="title" expanded :placeholder="'Select'">
            <option :value="item" v-for="(item, index) in titleOptions" :key="'companyContactPersons' + index">
              {{ item }}
            </option>
          </b-select>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Name *'" :type="formErrors.firstName ? 'is-danger' : ''"
          :message="formErrors.firstName || ''">
          <b-input v-model="firstName" name="name" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Middle Name'" :type="formErrors.middleName ? 'is-danger' : ''"
          :message="formErrors.middleName || ''">
          <b-input v-model="middleName" name="middlename" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Last Name *'" :type="formErrors.lastName ? 'is-danger' : ''"
          :message="formErrors.lastName || ''">
          <b-input v-model="lastName" name="lastname" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Position *'" :type="formErrors.position ? 'is-danger' : ''"
          :message="formErrors.position || ''">
          <b-input v-model="position" name="position" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input :required="false" :model="'Office Number'" label="Office Number" :defaultValue="contactPerson.officeNumber"
          @formattedPhone="(phone) => contactPerson.officeNumber = phone">
        </phone-input>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Ext.'" :type="formErrors.officeNumberExt ? 'is-danger' : ''"
          :message="formErrors.officeNumberExt || ''">
          <b-input v-model="officeNumberExt" name="officeNumberExt" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input :required="false" :model="'Mobile Number'" label="Mobile Number" :defaultValue="contactPerson.mobileNumber"
          @formattedPhone="(phone) => contactPerson.mobileNumber = phone">
        </phone-input>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Email *'" :type="formErrors.email ? 'is-danger' : ''"
          :message="formErrors.email || ''">
          <b-input type="email" v-model="email" name="email" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateForm">
          {{ props.currentContact ? 'Save' : 'Create' }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { createAgencyCompanyContactPerson, updateAgencyCompanyContactPerson } from "@/api/agencyCompanyApi";
import PhoneInput from "../PhoneInput.vue";

const numericExt = yup
  .string()
  .nullable()
  .transform((v) => (v === '' ? null : v))
  .matches(/^\d{1,8}$/, { message: 'Must be 1-8 digits', excludeEmptyString: true });

const schema = yup.object({
  title: yup.string().required('Title is required'),
  firstName: yup.string().required('Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  middleName: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(1, 'Min 1 character').max(20, 'Max 20 characters'),
  lastName: yup.string().required('Last name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  position: yup.string().required('Position is required').min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
  officeNumberExt: numericExt,
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
});

const props = defineProps<{ currentContact?: any; profileId: any }>();
const emit = defineEmits<{ (e: 'updateContent'): void }>();

const form = useStickyForm<{
  title: string;
  firstName: string;
  middleName: string;
  lastName: string;
  position: string;
  officeNumberExt: string;
  email: string;
}>({
  schema,
  initialValues: {
    title: '',
    firstName: '',
    middleName: '',
    lastName: '',
    position: '',
    officeNumberExt: '',
    email: '',
  },
});
const { title, firstName, middleName, lastName, position, officeNumberExt, email } = form.fields;
const formErrors = form.errors;

const titleOptions = ['Mr', 'Mrs', 'Ms', 'Miss', 'Mx', 'Master', 'Madam'];

const isLoading = ref(false);
const contactPerson = ref<any>({
  mobileNumber: null,
  officeNumber: null,
});

function validateForm() {
  form.markInteracted();
  form.handleSubmit((values) => {
    const payload = {
      ...contactPerson.value,
      title: values.title,
      firstName: values.firstName,
      middleName: values.middleName,
      lastName: values.lastName,
      position: values.position,
      officeNumberExt: values.officeNumberExt ? parseInt(values.officeNumberExt, 10) : null,
      email: values.email,
    };
    if (payload.id) {
      updateContactPerson(payload, payload.id);
    } else {
      createContactPerson(payload);
    }
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function createContactPerson(payload: any) {
  isLoading.value = true;
  createAgencyCompanyContactPerson(props.profileId, payload)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Created');
      emit('updateContent');
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateContactPerson(payload: any, id: any) {
  isLoading.value = true;
  updateAgencyCompanyContactPerson(props.profileId, id, payload)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Updated');
      emit('updateContent');
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

if (props.currentContact && props.currentContact.id) {
  contactPerson.value = Object.assign({}, props.currentContact);
  form.hydrate({
    title: props.currentContact.title || '',
    firstName: props.currentContact.firstName || '',
    middleName: props.currentContact.middleName || '',
    lastName: props.currentContact.lastName || '',
    position: props.currentContact.position || '',
    officeNumberExt: props.currentContact.officeNumberExt != null ? String(props.currentContact.officeNumberExt) : '',
    email: props.currentContact.email || '',
  });
}
</script>
