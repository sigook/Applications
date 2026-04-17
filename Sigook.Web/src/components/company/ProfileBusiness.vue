<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Company Business Name" :type="formErrors.businessName ? 'is-danger' : ''"
          :message="formErrors.businessName || ''">
          <b-input v-model="businessName" name="business name" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Company Full Name" :type="formErrors.fullName ? 'is-danger' : ''"
          :message="formErrors.fullName || ''">
          <b-input v-model="fullName" name="full name" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Industry">
          <b-autocomplete v-model="industry" :data="filteredIndustries" field="value"
            :placeholder="'Select industry'" open-on-focus @select="selectIndustry">
          </b-autocomplete>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <phone-input ref="phoneComponent" :required="true" model="Phone" :defaultValue="localCompanyData.phone"
          @formattedPhone="(phone) => localCompanyData.phone = phone"></phone-input>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <b-field label="Phone Ext" :type="formErrors.phoneExt ? 'is-danger' : ''"
          :message="formErrors.phoneExt || ''">
          <b-input type="text" v-model="phoneExt" name="phoneExt" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Website" :type="formErrors.website ? 'is-danger' : ''"
          :message="formErrors.website || ''">
          <b-input type="text" v-model="website" name="website" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <phone-input ref="faxComponent" :required="false" model="Fax" :defaultValue="localCompanyData.fax"
          @formattedPhone="(phone) => localCompanyData.fax = phone"></phone-input>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <b-field label="Fax Ext" :type="formErrors.faxExt ? 'is-danger' : ''"
          :message="formErrors.faxExt || ''">
          <b-input type="text" v-model="faxExt" name="faxExt" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="save">Save</b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { getIndustries } from "@/api/catalogApi";
import { updateProfile } from "@/api/companyApi";

const numericExt = yup
  .string()
  .nullable()
  .transform((v) => (v === '' ? null : v))
  .matches(/^\d{1,8}$/, { message: 'Must be 1-8 digits', excludeEmptyString: true });

const urlRegex = /^((https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?)$/i;

const schema = yup.object({
  businessName: yup.string().required('Business name is required').min(2).max(50, 'Max 50 characters'),
  fullName: yup.string().required('Full name is required').min(2).max(50, 'Max 50 characters'),
  industry: yup.string().nullable(),
  phoneExt: numericExt,
  faxExt: numericExt,
  website: yup
    .string()
    .nullable()
    .transform((v) => (v === '' ? null : v))
    .max(50, 'Max 50 characters')
    .matches(urlRegex, { message: 'Invalid URL', excludeEmptyString: true }),
});

export default {
  props: ['companyData'],
  setup(props: any) {
    const form = useStickyForm({
      schema,
      initialValues: {
        businessName: props.companyData?.businessName || '',
        fullName: props.companyData?.fullName || '',
        industry: props.companyData?.industry?.industry?.value || '',
        phoneExt: props.companyData?.phoneExt != null ? String(props.companyData.phoneExt) : '',
        faxExt: props.companyData?.faxExt != null ? String(props.companyData.faxExt) : '',
        website: props.companyData?.website || '',
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
      industries: [] as any[],
      industrySelected: null as any,
      localCompanyData: JSON.parse(JSON.stringify((this as any).companyData)),
    };
  },
  watch: {
    companyData: {
      handler(newVal) {
        this.localCompanyData = JSON.parse(JSON.stringify(newVal));
        this.hydrateForm({
          businessName: newVal?.businessName || '',
          fullName: newVal?.fullName || '',
          industry: newVal?.industry?.industry?.value || '',
          phoneExt: newVal?.phoneExt != null ? String(newVal.phoneExt) : '',
          faxExt: newVal?.faxExt != null ? String(newVal.faxExt) : '',
          website: newVal?.website || '',
        });
      },
      deep: true,
    },
  },
  components: {
    phoneInput: defineAsyncComponent(() => import("../../components/PhoneInput.vue")),
  },
  methods: {
    selectIndustry(option: any) {
      this.industrySelected = option || null;
    },
    async save() {
      this.markInteracted();
      const phoneValid = await (this.$refs.phoneComponent as any).validatePhone();
      const faxValid = await (this.$refs.faxComponent as any).validatePhone();
      this.handleSubmit((values) => {
        if (!phoneValid || !faxValid) return;
        this.isLoading = true;
        const updated = {
          ...this.localCompanyData,
          businessName: values.businessName,
          fullName: values.fullName,
          phoneExt: values.phoneExt ? parseInt(values.phoneExt, 10) : null,
          faxExt: values.faxExt ? parseInt(values.faxExt, 10) : null,
          website: values.website,
          industry: this.industrySelected
            ? { ...this.localCompanyData.industry, industry: this.industrySelected }
            : null,
        };
        this.$emit('update:companyData', updated);
        updateProfile(updated.id, updated)
          .then(() => {
            this.isLoading = false;
            showAlertSuccess('Profile updated');
          })
          .catch((error: any) => {
            this.isLoading = false;
            showAlertError(error.data);
          });
      })();
    },
  },
  async created() {
    this.industries = await getIndustries();
    if (this.localCompanyData.industry?.industry) {
      this.industrySelected = this.localCompanyData.industry.industry;
    }
  },
  computed: {
    filteredIndustries() {
      const search = ((this as any).industry || '').toLowerCase();
      return this.industries.filter((industry: any) => (industry.value || '').toLowerCase().includes(search));
    },
  },
};
</script>

<style lang="scss">
.profile-information>div.profile-100 {
  display: block;

  b,
  p {
    display: block;
    width: 100%;
  }

  input {
    width: 100%;
    max-width: 100px;

  }

  input[disabled="disabled"] {
    padding: 0;
  }

  .job-rates {
    width: 100%;
    margin-top: 10px;

    table {
      width: 100%;
      border-collapse: collapse;

      tr {
        transition: .3s all ease;
      }

      tr:hover {
        background-color: #f7f7f7;

      }

      td {
        padding: 5px 15px;
        font-size: 14px;
      }
    }
  }
}

.padding-left span {

  padding-left: 5px;
}
</style>
