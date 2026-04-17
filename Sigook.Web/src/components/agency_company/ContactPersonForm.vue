<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :label="'Title'" :type="formErrors.title ? 'is-danger' : ''"
          :message="formErrors.title || ''">
          <b-select v-model="title" name="title" expanded :placeholder="'Select'">
            <option :value="item" v-for="(item, index) in titleOptions" :key="'companyContactPersons' + index">
              {{ item }}
            </option>
          </b-select>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Name'" :type="formErrors.firstName ? 'is-danger' : ''"
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
        <b-field :label="'Last Name'" :type="formErrors.lastName ? 'is-danger' : ''"
          :message="formErrors.lastName || ''">
          <b-input v-model="lastName" name="lastname" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Position'" :type="formErrors.position ? 'is-danger' : ''"
          :message="formErrors.position || ''">
          <b-input v-model="position" name="position" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input :required="false" :model="'Office Number'" :defaultValue="contactPerson.officeNumber"
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
        <phone-input :required="false" :model="'Mobile Number'" :defaultValue="contactPerson.mobileNumber"
          @formattedPhone="(phone) => contactPerson.mobileNumber = phone">
        </phone-input>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Email'" :type="formErrors.email ? 'is-danger' : ''"
          :message="formErrors.email || ''">
          <b-input type="email" v-model="email" name="email" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateForm">
          {{ currentContact ? 'Save' : 'Create' }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { createAgencyCompanyContactPerson, updateAgencyCompanyContactPerson } from "@/api/agencyCompanyApi";

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

export default {
  props: ['currentContact', 'profileId'],
  setup() {
    const form = useStickyForm({
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
    const titleOptions = ['Mr', 'Mrs', 'Ms', 'Miss', 'Mx', 'Master', 'Madam'];
    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
      titleOptions,
    };
  },
  data() {
    return {
      isLoading: false,
      contactPerson: {
        mobileNumber: null as any,
        officeNumber: null as any,
      } as any,
    };
  },
  components: {
    phoneInput: defineAsyncComponent(() => import("../PhoneInput.vue")),
  },
  methods: {
    validateForm() {
      this.markInteracted();
      this.handleSubmit((values) => {
        const payload = {
          ...this.contactPerson,
          title: values.title,
          firstName: values.firstName,
          middleName: values.middleName,
          lastName: values.lastName,
          position: values.position,
          officeNumberExt: values.officeNumberExt ? parseInt(values.officeNumberExt, 10) : null,
          email: values.email,
        };
        if (payload.id) {
          this.updateContactPerson(payload, payload.id);
        } else {
          this.createContactPerson(payload);
        }
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    createContactPerson(payload: any) {
      this.isLoading = true;
      createAgencyCompanyContactPerson((this as any).profileId, payload)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Created');
          this.$emit('updateContent');
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    updateContactPerson(payload: any, id: any) {
      this.isLoading = true;
      updateAgencyCompanyContactPerson((this as any).profileId, id, payload)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Updated');
          this.$emit('updateContent');
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
  },
  created() {
    const currentContact = (this as any).currentContact;
    if (currentContact && currentContact.id) {
      this.contactPerson = Object.assign({}, currentContact);
      this.hydrateForm({
        title: currentContact.title || '',
        firstName: currentContact.firstName || '',
        middleName: currentContact.middleName || '',
        lastName: currentContact.lastName || '',
        position: currentContact.position || '',
        officeNumberExt: currentContact.officeNumberExt != null ? String(currentContact.officeNumberExt) : '',
        email: currentContact.email || '',
      });
    }
  },
};
</script>
