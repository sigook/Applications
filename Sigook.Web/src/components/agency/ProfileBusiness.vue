<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Agency Name" :type="formErrors.fullName ? 'is-danger' : ''"
          :message="formErrors.fullName || ''">
          <b-input v-model="fullName" name="agency name" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Agency HST Number" :type="formErrors.hstNumber ? 'is-danger' : ''"
          :message="formErrors.hstNumber || ''">
          <b-input v-model="hstNumber" name="hts #" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="BN/EIN" :type="formErrors.businessNumber ? 'is-danger' : ''"
          :message="formErrors.businessNumber || ''">
          <b-input v-model="businessNumber" name="business #" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-3 col-lg-3 col-padding">
        <phone-input ref="phoneComponent" :required="true" :defaultValue="localAgencyData.phonePrincipal"
          model="Phone Principal" @formattedPhone="(phone) => (localAgencyData.phonePrincipal = phone)">
        </phone-input>
      </div>
      <div class="col-sm-12 col-md-3 col-lg-3 col-padding">
        <b-field label="Ext" :type="formErrors.phonePrincipalExt ? 'is-danger' : ''"
          :message="formErrors.phonePrincipalExt || ''">
          <b-input v-model="phonePrincipalExt" name="phonePrincipalExt" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Web Page" :type="formErrors.webPage ? 'is-danger' : ''"
          :message="formErrors.webPage || ''">
          <b-input v-model="webPage" name="web page" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-3 col-lg-3 col-padding">
        <phone-input ref="faxComponent" :required="false" :defaultValue="localAgencyData.fax" model="Fax Number"
          @formattedPhone="(phone) => (localAgencyData.fax = phone)">
        </phone-input>
      </div>
      <div class="col-sm-12 col-md-3 col-lg-3 col-padding">
        <b-field label="Ext" :type="formErrors.faxExt ? 'is-danger' : ''"
          :message="formErrors.faxExt || ''">
          <b-input v-model="faxExt" name="faxExt" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="WSIB Group" :type="formErrors.wsibGroup ? 'is-danger' : ''"
          :message="formErrors.wsibGroup || ''">
          <b-taginput v-model="localAgencyData.wsibGroup" autocomplete :data="wsibGroups" open-on-focus field="value"
            icon="label" :placeholder="'Click here to add more'" :before-adding="beforeWsibBeingSelected">
          </b-taginput>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="validateForm">SAVE</b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertSuccess } from "@/utils/toast";
import { getWsibGroups } from "@/api/catalogApi";
import { updateAgency } from "@/api/agencyApi";

const numericExt = yup
  .string()
  .nullable()
  .transform((v) => (v === '' ? null : v))
  .matches(/^\d{1,8}$/, { message: 'Must be 1-8 digits', excludeEmptyString: true });

const urlRegex = /^((https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?)$/i;

const schema = yup.object({
  fullName: yup.string().required('Agency name is required').min(2, 'Min 2 characters').max(50, 'Max 50 characters'),
  hstNumber: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(15, 'Min 15 characters').max(20, 'Max 20 characters'),
  businessNumber: yup.string().required('Business number is required').min(9, 'Min 9 characters').max(15, 'Max 15 characters'),
  phonePrincipalExt: numericExt,
  faxExt: numericExt,
  webPage: yup
    .string()
    .nullable()
    .transform((v) => (v === '' ? null : v))
    .max(50, 'Max 50 characters')
    .matches(urlRegex, { message: 'Invalid URL', excludeEmptyString: true }),
});

export default {
  props: ["agencyData"],
  setup(props: any) {
    const form = useStickyForm({
      schema,
      initialValues: {
        fullName: props.agencyData?.fullName || '',
        hstNumber: props.agencyData?.hstNumber || '',
        businessNumber: props.agencyData?.businessNumber || '',
        phonePrincipalExt: props.agencyData?.phonePrincipalExt != null ? String(props.agencyData.phonePrincipalExt) : '',
        faxExt: props.agencyData?.faxExt != null ? String(props.agencyData.faxExt) : '',
        webPage: props.agencyData?.webPage || '',
      },
    });

    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
    };
  },
  data() {
    return {
      isLoading: false,
      localAgencyData: JSON.parse(JSON.stringify((this as any).agencyData)),
      wsibGroups: [] as any[],
    };
  },
  watch: {
    agencyData: {
      handler(newVal: any) {
        this.localAgencyData = JSON.parse(JSON.stringify(newVal));
        this.hydrateForm({
          fullName: newVal?.fullName || '',
          hstNumber: newVal?.hstNumber || '',
          businessNumber: newVal?.businessNumber || '',
          phonePrincipalExt: newVal?.phonePrincipalExt != null ? String(newVal.phonePrincipalExt) : '',
          faxExt: newVal?.faxExt != null ? String(newVal.faxExt) : '',
          webPage: newVal?.webPage || '',
        });
      },
      deep: true,
    },
  },
  components: {
    phoneInput: defineAsyncComponent(() => import("@/components/PhoneInput.vue")),
  },
  methods: {
    async validateForm() {
      this.markInteracted();
      const phoneValid = await (this.$refs.phoneComponent as any).validatePhone();
      const faxValid = await (this.$refs.faxComponent as any).validatePhone();
      this.handleSubmit((values) => {
        if (!phoneValid || !faxValid) return;
        this.isLoading = true;
        const updated = {
          ...this.localAgencyData,
          fullName: values.fullName,
          hstNumber: values.hstNumber,
          businessNumber: values.businessNumber,
          phonePrincipalExt: values.phonePrincipalExt ? parseInt(values.phonePrincipalExt, 10) : null,
          faxExt: values.faxExt ? parseInt(values.faxExt, 10) : null,
          webPage: values.webPage,
        };
        this.$emit('update:agencyData', updated);
        updateAgency(updated)
          .then(() => {
            this.isLoading = false;
            showAlertSuccess("Updated");
          })
          .catch(() => {
            this.isLoading = false;
          });
      })();
    },
    beforeWsibBeingSelected(tag: any) {
      return !this.localAgencyData.wsibGroup.some((item: any) => item.value === tag.value);
    },
  },
  created() {
    this.isLoading = true;
    getWsibGroups()
      .then((response: any) => {
        this.isLoading = false;
        this.wsibGroups = response;
      })
      .catch(() => {
        this.isLoading = false;
      });
  },
};
</script>
