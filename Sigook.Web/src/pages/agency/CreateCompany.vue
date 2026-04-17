<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <h2 v-if="isUpdate" class="text-center main-title">Update Client</h2>
    <h2 v-else class="text-center main-title">Create Client</h2>
    <span class="line-orange"></span>
    <form class="form-md" @submit.prevent="validateForm">
      <div class="container-flex">
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <div class="container-image margin-10-auto">
            <upload-image @imageSelected="(img) => (company.logo.fileName = img)" :required="false"
              @onUpload="() => subscribe('file')" @finishUpload="() => unsubscribe()"></upload-image>
          </div>
        </div>
        <div v-if="isPayrollManager" class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field>
            <b-checkbox v-model="company.requiresPermissionToSeeOrders">
              Requires permission to see orders?
            </b-checkbox>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.fullName ? 'is-danger' : ''" :label="'Full name'"
            :message="formErrors.fullName || ''">
            <b-input type="text" v-model="fullName" name="full name" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.businessName ? 'is-danger' : ''" :label="'Business Name'"
            :message="formErrors.businessName || ''">
            <b-input type="text" v-model="businessName" name="business name" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="formErrors.industry ? 'is-danger' : ''" :label="'Type of industry'"
            :message="formErrors.industry || ''">
            <b-autocomplete v-model="industry" :data="filteredIndustries" open-on-focus field="value"
              name="industry" placeholder="Industry" selectable-footer
              @select="selectIndustry" @select-footer="addIndustry">
              <template #footer>
                <a><span> Add new... </span></a>
              </template>
              <template #empty>You don't have any industry created</template>
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="formErrors.companyStatus ? 'is-danger' : ''" label="Status"
            :message="formErrors.companyStatus || ''">
            <b-select v-model="companyStatus" placeholder="Select option" name="state" expanded>
              <option v-for="status in statuses" :key="status.id" :value="status.id">{{ status.value }}</option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="formErrors.salesRepresentative ? 'is-danger' : ''" label="Sales Representative"
            :message="formErrors.salesRepresentative || ''">
            <b-autocomplete :data="filteredSalesRepresentative" :placeholder="'Select'"
              v-model="salesRepresentative" open-on-focus name="salesRepresentative"
              :custom-formatter="(option) => `${option.name} - ${option.email}`"
              @select="onSalesRepresentativeSelected">
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field :type="formErrors.about ? 'is-danger' : ''" :label="'About'"
            :message="formErrors.about || ''">
            <b-input type="textarea" v-model="about" name="about" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field label="Internal Info">
            <div class="vue-trix-editor">
              <div>
                <QuillEditor theme="snow" content-type="html" v-model:content="company.internalInfo" />
              </div>
            </div>
          </b-field>
        </div>
      </div>
      <h3 class="fz1 col-padding">{{ "Contact Information" }}</h3>
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <phone-input :required="false" :defaultValue="company.phone" model="Phone"
            @formattedPhone="(phone) => (company.phone = phone)"></phone-input>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.phoneExt ? 'is-danger' : ''" label="Phone Ext"
            :message="formErrors.phoneExt || ''">
            <b-input type="text" v-model="phoneExt" name="phoneExt" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <phone-input :required="false" :defaultValue="company.fax" model="Fax"
            @formattedPhone="(phone) => (company.fax = phone)"></phone-input>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.faxExt ? 'is-danger' : ''" label="Fax Ext"
            :message="formErrors.faxExt || ''">
            <b-input type="text" v-model="faxExt" name="faxExt" />
          </b-field>
        </div>
        <div v-if="!isUpdate" class="col-sm-12 col-md-6 col-padding">
          <b-field :type="formErrors.email ? 'is-danger' : ''" :label="'Email'"
            :message="formErrors.email || ''">
            <b-input type="email" v-model="email" name="email" />
          </b-field>
        </div>
        <div v-if="displayPassword" class="col-sm-12 col-md-6 col-padding">
          <b-field :type="formErrors.password ? 'is-danger' : ''" :label="'Password'"
            :message="formErrors.password || ''">
            <b-input type="password" v-model="password" name="password" />
          </b-field>
        </div>
        <div
          :class="displayPassword ? 'col-sm-12 col-md-12 col-lg-12 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding'">
          <b-field :type="formErrors.website ? 'is-danger' : ''" :label="'Website'"
            :message="formErrors.website || ''">
            <b-input type="text" v-model="website" name="website"
              placeholder="www.example.com" />
          </b-field>
        </div>
        <div class="col-12 mt-5">
          <b-button v-if="isUpdate" type="is-primary" native-type="submit">Update</b-button>
          <b-button v-else type="is-primary" native-type="submit">{{ "Create" }}</b-button>
        </div>
      </div>
    </form>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent, ref, computed, watch } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { confirmationGuard } from '@/utils/confirmationGuard';
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { usePubSub } from '@/composables/usePubSub';
import { getIndustries, getCompanyStatus, addIndustry } from "@/api/catalogApi";
import { getAgencyPersonnel } from "@/api/agencyApi";
import { createAgencyCompany, updateAgencyCompany } from "@/api/agencyCompanyApi";

const numericExt = yup
  .string()
  .nullable()
  .transform((v) => (v === '' ? null : v))
  .matches(/^\d{1,8}$/, { message: 'Must be 1-8 digits', excludeEmptyString: true });

const urlRegex = /^((https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?)$/i;

export default {
  setup() {
    const isUpdate = ref(false);
    const companyStatusValue = ref<number | null>(null);

    const displayPassword = computed(() => !isUpdate.value && companyStatusValue.value === 5);

    const validationSchema = computed(() => {
      const shape: Record<string, any> = {
        fullName: yup.string().required('Full name is required').min(2, 'Min 2 characters').max(60, 'Max 60 characters'),
        businessName: yup.string().required('Business name is required').min(2, 'Min 2 characters').max(50, 'Max 50 characters'),
        industry: yup.string().required('Industry is required'),
        companyStatus: yup.mixed().required('Status is required'),
        salesRepresentative: yup.string().required('Sales representative is required'),
        about: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(2, 'Min 2 characters').max(5000, 'Max 5000 characters'),
        phoneExt: numericExt,
        faxExt: numericExt,
        website: yup
          .string()
          .nullable()
          .transform((v) => (v === '' ? null : v))
          .max(50, 'Max 50 characters')
          .matches(urlRegex, { message: 'Invalid URL', excludeEmptyString: true }),
      };
      if (!isUpdate.value) {
        shape.email = yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters');
      }
      if (displayPassword.value) {
        shape.password = yup.string().required('Password is required').min(6, 'Min 6 characters').max(100, 'Max 100 characters');
      }
      return yup.object(shape);
    });

    const form = useStickyForm({
      schema: validationSchema,
      initialValues: {
        fullName: '',
        businessName: '',
        industry: '',
        companyStatus: null as number | null,
        salesRepresentative: '',
        about: '',
        phoneExt: '',
        faxExt: '',
        email: '',
        password: '',
        website: '',
      },
    });

    watch(form.fields.companyStatus, (v) => {
      companyStatusValue.value = (v as number | null) ?? null;
    });

    return {
      ...useBillingAdmin(),
      ...usePubSub(),
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      setFieldValue: form.setFieldValue,
      setFieldError: form.setFieldError,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
      isUpdate,
      displayPassword,
    };
  },
  data() {
    return {
      isLoading: true,
      statuses: [] as any[],
      industryOptions: [] as any[],
      industrySelected: null as any,
      salesRepresentatives: [] as any[],
      salesRepresentativeSelected: null as any,
      company: {
        companyProfileId: null as any,
        logo: {} as any,
        industry: {
          industry: null as any,
          otherIndustry: null as any
        },
        requiresPermissionToSeeOrders: false,
      } as any,
      submitted: false,
      unsavedChanges: false,
    };
  },
  beforeRouteLeave: confirmationGuard,
  components: {
    UploadImage: defineAsyncComponent(() => import("@/components/PreviewImage.vue")),
    phoneInput: defineAsyncComponent(() => import("@/components/PhoneInput.vue")),
  },
  async created() {
    const company = this.$route.meta.company as any;
    if (company) {
      this.company = {
        ...company,
        companyProfileId: company.id,
      };
      this.statuses = this.$route.meta.companyStatuses as any[];
      this.industryOptions = this.$route.meta.industryList as any[];
      this.salesRepresentatives = this.$route.meta.agencyPersonnel as any[];
      this.isUpdate = true;

      if (company.industry?.industry) {
        this.industrySelected = company.industry.industry;
      }
      const record = this.salesRepresentatives.find((sr) => sr.id === company.salesRepresentativeId);
      if (record) {
        this.salesRepresentativeSelected = record;
      }

      this.hydrateForm({
        fullName: company.fullName || '',
        businessName: company.businessName || '',
        industry: company.industry?.industry?.value || '',
        companyStatus: company.companyStatus ?? null,
        salesRepresentative: record ? `${record.name} - ${record.email}` : '',
        about: company.about || '',
        phoneExt: company.phoneExt != null ? String(company.phoneExt) : '',
        faxExt: company.faxExt != null ? String(company.faxExt) : '',
        website: company.website || '',
      });
    } else {
      this.industryOptions = await getIndustries();
      this.statuses = await getCompanyStatus();
      this.salesRepresentatives = await getAgencyPersonnel();
    }
    this.isLoading = false;
  },
  methods: {
    validateAutocompleteSelections() {
      let valid = true;
      if (this.industry && (!this.industrySelected || (this.industrySelected.value || '').toLowerCase() !== this.industry.toLowerCase())) {
        this.setFieldError('industry', 'Please select an industry from the list');
        valid = false;
      }
      if (this.salesRepresentative && (!this.salesRepresentativeSelected ||
        `${this.salesRepresentativeSelected.name} - ${this.salesRepresentativeSelected.email}`.toLowerCase() !== this.salesRepresentative.toLowerCase())) {
        this.setFieldError('salesRepresentative', 'Please select a sales representative from the list');
        valid = false;
      }
      return valid;
    },
    validateForm() {
      this.submitted = true;
      this.markInteracted();
      this.handleSubmit((values) => {
        if (!this.validateAutocompleteSelections()) {
          showAlertError('Please make sure all required fields are filled out correctly');
          return;
        }
        const payload = {
          ...this.company,
          fullName: values.fullName,
          businessName: values.businessName,
          companyStatus: values.companyStatus,
          about: values.about,
          phoneExt: values.phoneExt ? parseInt(values.phoneExt, 10) : null,
          faxExt: values.faxExt ? parseInt(values.faxExt, 10) : null,
          website: values.website,
          salesRepresentativeId: this.salesRepresentativeSelected?.id || null,
          industry: {
            ...this.company.industry,
            industry: this.industrySelected,
          },
        };
        if (!this.isUpdate) {
          payload.email = values.email;
        }
        if (this.displayPassword) {
          payload.password = values.password;
        }
        if (this.isUpdate) {
          this.submitUpdateCompany(payload);
        } else {
          this.submitCreateCompany(payload);
        }
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    submitCreateCompany(payload: any) {
      this.isLoading = true;
      createAgencyCompany(payload)
        .then((response: any) => {
          this.isLoading = false;
          showAlertSuccess('Company created');
          this.$router.push('/agency-companies/company/' + response.id);
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error.data);
        });
    },
    submitUpdateCompany(payload: any) {
      this.isLoading = true;
      updateAgencyCompany(this.company.companyProfileId, payload)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Company updated');
          this.$router.push('/agency-companies/company/' + this.company.companyProfileId);
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error.data);
        });
    },
    onSalesRepresentativeSelected(option: any) {
      this.salesRepresentativeSelected = option || null;
    },
    selectIndustry(option: any) {
      this.industrySelected = option || null;
    },
    addIndustry() {
      (this as any).$buefy.dialog.prompt({
        message: `Industry`,
        inputAttrs: {
          placeholder: 'Industry',
          maxlength: 100,
          value: this.industry,
        },
        closeOnConfirm: false,
        confirmText: 'Add',
        onConfirm: async (value: string, dialog: any) => {
          const payload = { value };
          const newIndustry = await addIndustry(payload);
          this.industryOptions.push(newIndustry);
          this.industrySelected = newIndustry;
          this.setFieldValue('industry', newIndustry.value);
          dialog.close();
        },
      });
    },
  },
  computed: {
    filteredIndustries() {
      const search = (this.industry || '').toLowerCase();
      return this.industryOptions.filter((option) => (option.value || '').toLowerCase().includes(search));
    },
    filteredSalesRepresentative() {
      const search = (this.salesRepresentative || '').toLowerCase();
      return this.salesRepresentatives.filter((sr) => `${sr.name} - ${sr.email}`.toLowerCase().includes(search));
    },
  },
};
</script>
