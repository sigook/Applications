<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3">Create Agency</h2>
    </div>
    <form @submit.prevent="validateForm">
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field label="Full Name" :type="formErrors.fullName ? 'is-danger' : ''"
            :message="formErrors.fullName || ''">
            <b-input type="text" v-model="fullName" name="full name" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field :type="formErrors.email ? 'is-danger' : ''" label="Email"
            :message="formErrors.email || ''">
            <b-input type="email" v-model="email" name="email" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <phone-input ref="phoneComponent" :required="true" model="Phone" :defaultValue="phoneNumber"
            @formattedPhone="(phone) => phoneNumber = phone"></phone-input>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field :type="formErrors.agencyType ? 'is-danger' : ''" label="Agency Type"
            :message="formErrors.agencyType || ''">
            <b-select v-model="agencyType" name="agency type" placeholder="Select agency type" expanded>
              <option v-for="type in agencyTypes" :key="type.value" :value="type.value">
                {{ type.label }}
              </option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field :type="formErrors.password ? 'is-danger' : ''" label="Password"
            :message="formErrors.password || ''">
            <b-input type="password" v-model="password" name="password" password-reveal />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-button type="is-primary" native-type="submit">{{ 'Create' }}</b-button>
        </div>
      </div>
    </form>
  </div>
</template>
<script lang="ts">
import { defineAsyncComponent } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { createAgency } from "@/api/agencyApi";

const schema = yup.object({
  fullName: yup.string().required('Full name is required').min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
  agencyType: yup.mixed().required('Agency type is required'),
  password: yup.string().required('Password is required').min(6, 'Min 6 characters').max(100, 'Max 100 characters'),
});

export default {
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        fullName: '',
        email: '',
        agencyType: null as any,
        password: '',
      },
    });

    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
    };
  },
  data() {
    return {
      isLoading: false,
      phoneNumber: "",
      agencyTypes: (this as any).$agencyTypes,
    };
  },
  components: {
    phoneInput: defineAsyncComponent(() => import("@/components/PhoneInput.vue")),
  },
  methods: {
    async validateForm() {
      this.markInteracted();
      const phoneValid = await (this.$refs.phoneComponent as any).validatePhone();
      this.handleSubmit((values) => {
        if (!phoneValid) {
          showAlertError('Please make sure all required fields are filled out correctly');
          return;
        }
        this.submitAgency(values);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    submitAgency(values: any) {
      this.isLoading = true;
      const payload = {
        fullName: values.fullName,
        email: values.email,
        agencyType: values.agencyType,
        password: values.password,
        phonePrincipal: this.phoneNumber,
      };
      createAgency(payload)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess("Agency created successfully");
          this.$router.push('/agency-agencies');
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
  },
};
</script>
