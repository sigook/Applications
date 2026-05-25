<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <h2 v-if="isUpdate" class="text-center main-title">Update Client</h2>
    <h2 v-else class="text-center main-title">Create Client</h2>
    <span class="line-orange"></span>
    <form class="form-md" @submit.prevent="validateForm">
      <div class="container-flex">
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <div class="container-image margin-10-auto">
            <UploadImage @imageSelected="(img) => (company.logo.fileName = img)" :required="false"
              @onUpload="() => pubSub.subscribe('file')" @finishUpload="() => pubSub.unsubscribe()"></UploadImage>
          </div>
        </div>
        <div v-if="billingAdmin.isPayrollManager" class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field>
            <b-checkbox v-model="company.requiresPermissionToSeeOrders">
              Requires permission to see orders?
            </b-checkbox>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.fullName ? 'is-danger' : ''"
            :message="formErrors.fullName || ''">
            <template #label>Full name <span class="has-text-danger">*</span></template>
            <b-input type="text" v-model="fullName" name="full name" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.businessName ? 'is-danger' : ''"
            :message="formErrors.businessName || ''">
            <template #label>Business Name <span class="has-text-danger">*</span></template>
            <b-input type="text" v-model="businessName" name="business name" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="formErrors.industry ? 'is-danger' : ''"
            :message="formErrors.industry || ''">
            <template #label>Type of industry <span class="has-text-danger">*</span></template>
            <b-autocomplete v-model="industry" :data="filteredIndustries" open-on-focus field="value"
              name="industry" placeholder="Industry" selectable-footer
              @select="selectIndustry" @select-footer="onAddIndustry">
              <template #footer>
                <a><span> Add new... </span></a>
              </template>
              <template #empty>You don't have any industry created</template>
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="formErrors.companyStatus ? 'is-danger' : ''"
            :message="formErrors.companyStatus || ''">
            <template #label>Status <span class="has-text-danger">*</span></template>
            <b-select v-model="companyStatus" placeholder="Select option" name="state" expanded>
              <option v-for="status in statuses" :key="status.id" :value="status.id">{{ status.value }}</option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="formErrors.salesRepresentative ? 'is-danger' : ''"
            :message="formErrors.salesRepresentative || ''">
            <template #label>Sales Representative <span class="has-text-danger">*</span></template>
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
      <h3 class="fz1 col-padding">Contact Information</h3>
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <PhoneInput :required="false" :defaultValue="company.phone" label="Phone"
            @formattedPhone="(phone) => (company.phone = phone)"></PhoneInput>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.phoneExt ? 'is-danger' : ''" label="Phone Ext"
            :message="formErrors.phoneExt || ''">
            <b-input type="text" v-model="phoneExt" name="phoneExt" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <PhoneInput :required="false" :defaultValue="company.fax" label="Fax"
            @formattedPhone="(phone) => (company.fax = phone)"></PhoneInput>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="formErrors.faxExt ? 'is-danger' : ''" label="Fax Ext"
            :message="formErrors.faxExt || ''">
            <b-input type="text" v-model="faxExt" name="faxExt" />
          </b-field>
        </div>
        <div v-if="!isUpdate" class="col-sm-12 col-md-6 col-padding">
          <b-field :type="formErrors.email ? 'is-danger' : ''"
            :message="formErrors.email || ''">
            <template #label>Email <span class="has-text-danger">*</span></template>
            <b-input type="email" v-model="email" name="email" />
          </b-field>
        </div>
        <div v-if="displayPassword" class="col-sm-12 col-md-6 col-padding">
          <b-field :type="formErrors.password ? 'is-danger' : ''"
            :message="formErrors.password || ''">
            <template #label>Password <span class="has-text-danger">*</span></template>
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
          <b-button v-else type="is-primary" native-type="submit">Create</b-button>
        </div>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import * as yup from 'yup';
import UploadImage from '@/components/PreviewImage.vue';
import PhoneInput from '@/components/PhoneInput.vue';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { usePubSub } from '@/composables/usePubSub';
import { getDialog } from '@/utils/buefyProgrammatic';
import { getIndustries, getCompanyStatus, addIndustry as addIndustryApi } from '@/api/catalogApi';
import { getAgencyPersonnel } from '@/api/agencyApi';
import { createAgencyCompany, updateAgencyCompany } from '@/api/agencyCompanyApi';

const route = useRoute();
const router = useRouter();
const billingAdmin = useBillingAdmin();
const pubSub = usePubSub();

const numericExt = yup
  .string()
  .nullable()
  .transform((v) => (v === '' ? null : v))
  .matches(/^\d{1,8}$/, { message: 'Must be 1-8 digits', excludeEmptyString: true });

const urlRegex = /^((https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?)$/i;

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
const {
  fullName, businessName, industry, companyStatus, salesRepresentative,
  about, phoneExt, faxExt, email, password, website,
} = form.fields;
const formErrors = form.errors;

watch(form.fields.companyStatus, (v) => {
  companyStatusValue.value = (v as number | null) ?? null;
});

const isLoading = ref(true);
const statuses = ref<any[]>([]);
const industryOptions = ref<any[]>([]);
const industrySelected = ref<any>(null);
const salesRepresentatives = ref<any[]>([]);
const salesRepresentativeSelected = ref<any>(null);
const company = ref<any>({
  companyProfileId: null,
  logo: {},
  industry: { industry: null, otherIndustry: null },
  requiresPermissionToSeeOrders: false,
});

const filteredIndustries = computed(() => {
  const search = (industry.value || '').toLowerCase();
  return industryOptions.value.filter((option) => (option.value || '').toLowerCase().includes(search));
});

const filteredSalesRepresentative = computed(() => {
  const search = (salesRepresentative.value || '').toLowerCase();
  return salesRepresentatives.value.filter((sr) => `${sr.name} - ${sr.email}`.toLowerCase().includes(search));
});

async function init() {
  const meta = route.meta as any;
  const existing = meta.company;
  if (existing) {
    company.value = { ...existing, companyProfileId: existing.id };
    statuses.value = meta.companyStatuses as any[];
    industryOptions.value = meta.industryList as any[];
    salesRepresentatives.value = meta.agencyPersonnel as any[];
    isUpdate.value = true;

    if (existing.industry?.industry) {
      industrySelected.value = existing.industry.industry;
    }
    const record = salesRepresentatives.value.find((sr) => sr.id === existing.salesRepresentativeId);
    if (record) salesRepresentativeSelected.value = record;

    form.hydrate({
      fullName: existing.fullName || '',
      businessName: existing.businessName || '',
      industry: existing.industry?.industry?.value || '',
      companyStatus: existing.companyStatus ?? null,
      salesRepresentative: record ? `${record.name} - ${record.email}` : '',
      about: existing.about || '',
      phoneExt: existing.phoneExt != null ? String(existing.phoneExt) : '',
      faxExt: existing.faxExt != null ? String(existing.faxExt) : '',
      website: existing.website || '',
    });
  } else {
    industryOptions.value = await getIndustries();
    statuses.value = await getCompanyStatus();
    salesRepresentatives.value = await getAgencyPersonnel();
  }
  isLoading.value = false;
}
init();

function validateAutocompleteSelections(): boolean {
  let valid = true;
  if (industry.value && (!industrySelected.value || (industrySelected.value.value || '').toLowerCase() !== industry.value.toLowerCase())) {
    form.setFieldError('industry', 'Please select an industry from the list');
    valid = false;
  }
  if (salesRepresentative.value && (!salesRepresentativeSelected.value ||
    `${salesRepresentativeSelected.value.name} - ${salesRepresentativeSelected.value.email}`.toLowerCase() !== salesRepresentative.value.toLowerCase())) {
    form.setFieldError('salesRepresentative', 'Please select a sales representative from the list');
    valid = false;
  }
  return valid;
}

function validateForm() {
  form.markInteracted();
  form.handleSubmit((values) => {
    if (!validateAutocompleteSelections()) {
      showAlertError('Please make sure all required fields are filled out correctly');
      return;
    }
    const payload: any = {
      ...company.value,
      fullName: values.fullName,
      businessName: values.businessName,
      companyStatus: values.companyStatus,
      about: values.about,
      phoneExt: values.phoneExt ? parseInt(values.phoneExt, 10) : null,
      faxExt: values.faxExt ? parseInt(values.faxExt, 10) : null,
      website: values.website,
      salesRepresentativeId: salesRepresentativeSelected.value?.id || null,
      industry: { ...company.value.industry, industry: industrySelected.value },
    };
    if (!isUpdate.value) payload.email = values.email;
    if (displayPassword.value) payload.password = values.password;

    if (isUpdate.value) submitUpdateCompany(payload);
    else submitCreateCompany(payload);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function submitCreateCompany(payload: any) {
  isLoading.value = true;
  createAgencyCompany(payload)
    .then((response: any) => {
      isLoading.value = false;
      showAlertSuccess('Company created');
      router.push('/agency-companies/company/' + response.id);
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function submitUpdateCompany(payload: any) {
  isLoading.value = true;
  updateAgencyCompany(company.value.companyProfileId, payload)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Company updated');
      router.push('/agency-companies/company/' + company.value.companyProfileId);
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function onSalesRepresentativeSelected(option: any) {
  salesRepresentativeSelected.value = option || null;
}

function selectIndustry(option: any) {
  industrySelected.value = option || null;
}

function onAddIndustry() {
  getDialog().prompt({
    message: 'Industry',
    inputAttrs: {
      placeholder: 'Industry',
      maxlength: 100,
      value: industry.value,
    },
    closeOnConfirm: false,
    confirmText: 'Add',
    onConfirm: async (value: string, dialog: any) => {
      const newIndustry = await addIndustryApi({ value });
      industryOptions.value.push(newIndustry);
      industrySelected.value = newIndustry;
      form.setFieldValue('industry', newIndustry.value);
      dialog.close();
    },
  });
}
</script>
