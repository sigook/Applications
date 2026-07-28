<template>
  <form class="sd-form" @submit.prevent>
    <div class="sd-form__logo">
      <UploadImage
        :required="false"
        @imageSelected="(img) => (company.logo.fileName = img)"
        @onUpload="() => pubSub.subscribe('file')"
        @finishUpload="() => pubSub.unsubscribe()"
      />
    </div>

    <b-field v-if="isAccountingManager" class="sd-form__check">
      <b-checkbox v-model="company.requiresPermissionToSeeRequests">
        Requires permission to see requests?
      </b-checkbox>
    </b-field>

    <b-field :type="formErrors.fullName ? 'is-danger' : ''" :message="formErrors.fullName || ''">
      <template #label>Full name <span class="has-text-danger">*</span></template>
      <b-input v-model="fullName" name="full name" placeholder="e.g. Acme Logistics"></b-input>
    </b-field>

    <b-field :type="formErrors.industry ? 'is-danger' : ''" :message="formErrors.industry || ''">
      <template #label>Type of industry <span class="has-text-danger">*</span></template>
      <b-autocomplete
        v-model="industry"
        :data="filteredIndustries"
        open-on-focus
        field="value"
        name="industry"
        placeholder="Industry"
        selectable-footer
        @select="selectIndustry"
        @select-footer="onAddIndustry"
      >
        <template #footer>
          <a><span> Add new... </span></a>
        </template>
        <template #empty>You don't have any industry created</template>
      </b-autocomplete>
    </b-field>

    <div class="sd-form__row">
      <b-field
        class="sd-form__col"
        :type="formErrors.companyStatus ? 'is-danger' : ''"
        :message="formErrors.companyStatus || ''"
      >
        <template #label>Status <span class="has-text-danger">*</span></template>
        <b-select v-model="companyStatus" placeholder="Select option" name="state" expanded>
          <option v-for="status in statuses" :key="status.id" :value="status.id">{{ status.value }}</option>
        </b-select>
      </b-field>

      <b-field
        class="sd-form__col"
        :type="formErrors.salesRepresentative ? 'is-danger' : ''"
        :message="formErrors.salesRepresentative || ''"
      >
        <template #label>Sales Representative <span class="has-text-danger">*</span></template>
        <b-autocomplete
          v-model="salesRepresentative"
          :data="filteredSalesRepresentative"
          open-on-focus
          name="salesRepresentative"
          placeholder="Select"
          :custom-formatter="(option) => `${option.name} - ${option.email}`"
          @select="onSalesRepresentativeSelected"
        ></b-autocomplete>
      </b-field>
    </div>

    <b-field :type="formErrors.about ? 'is-danger' : ''" label="About" :message="formErrors.about || ''">
      <b-input type="textarea" v-model="about" name="about"></b-input>
    </b-field>

    <b-field label="Internal Info">
      <div class="sd-form__editor">
        <QuillEditor theme="snow" content-type="html" v-model:content="company.internalInfo" />
      </div>
    </b-field>

    <div class="sd-form__row">
      <div class="sd-form__col">
        <PhoneInput
          :required="false"
          :defaultValue="company.phone"
          label="Phone"
          @formattedPhone="(phone) => (company.phone = phone)"
        ></PhoneInput>
      </div>
      <b-field class="sd-form__col" :type="formErrors.phoneExt ? 'is-danger' : ''" label="Phone Ext" :message="formErrors.phoneExt || ''">
        <b-input v-model="phoneExt" name="phoneExt"></b-input>
      </b-field>
    </div>

    <div class="sd-form__row">
      <div class="sd-form__col">
        <PhoneInput
          :required="false"
          :defaultValue="company.fax"
          label="Fax"
          @formattedPhone="(fax) => (company.fax = fax)"
        ></PhoneInput>
      </div>
      <b-field class="sd-form__col" :type="formErrors.faxExt ? 'is-danger' : ''" label="Fax Ext" :message="formErrors.faxExt || ''">
        <b-input v-model="faxExt" name="faxExt"></b-input>
      </b-field>
    </div>

    <div class="sd-form__row">
      <b-field class="sd-form__col" :type="formErrors.email ? 'is-danger' : ''" :message="formErrors.email || ''">
        <template #label>Email <span class="has-text-danger">*</span></template>
        <b-input type="email" v-model="email" name="email" placeholder="name@company.com"></b-input>
      </b-field>
      <b-field
        v-if="displayPassword"
        class="sd-form__col"
        :type="formErrors.password ? 'is-danger' : ''"
        :message="formErrors.password || ''"
      >
        <template #label>Password <span class="has-text-danger">*</span></template>
        <b-input type="password" v-model="password" name="password"></b-input>
      </b-field>
    </div>

    <b-field :type="formErrors.website ? 'is-danger' : ''" label="Website" :message="formErrors.website || ''">
      <b-input v-model="website" name="website" placeholder="www.example.com"></b-input>
    </b-field>
  </form>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import * as yup from 'yup';
import UploadImage from '@/components/PreviewImage.vue';
import PhoneInput from '@/components/PhoneInput.vue';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { useAccountingAdmin } from '@/composables/useAccountingAdmin';
import { usePubSub } from '@/composables/usePubSub';
import { getDialog } from '@/utils/buefyProgrammatic';
import { getIndustries, getCompanyStatus, addIndustry as addIndustryApi } from '@/api/catalogApi';
import { getAgencyPersonnel } from '@/api/agencyApi';
import { createAgencyCompany } from '@/api/agencyCompanyApi';

const { isAccountingManager } = useAccountingAdmin();
const pubSub = usePubSub();

const numericExt = yup
  .string()
  .nullable()
  .transform((v) => (v === '' ? null : v))
  .matches(/^\d{1,8}$/, { message: 'Must be 1-8 digits', excludeEmptyString: true });

const urlRegex = /^((https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?)$/i;

const companyStatusValue = ref<number | null>(null);
const displayPassword = computed(() => companyStatusValue.value === 5);

const validationSchema = computed(() => {
  const shape: Record<string, any> = {
    fullName: yup.string().required('Full name is required').min(2, 'Min 2 characters').max(50, 'Max 50 characters'),
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
    email: yup.string().required('Email is required').email('Invalid email').min(6, 'Min 6 characters').max(50, 'Max 50 characters'),
  };
  if (displayPassword.value) {
    shape.password = yup.string().required('Password is required').min(6, 'Min 6 characters').max(100, 'Max 100 characters');
  }
  return yup.object(shape);
});

const form = useStickyForm({
  schema: validationSchema,
  initialValues: {
    fullName: '',
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
  fullName, industry, companyStatus, salesRepresentative,
  about, phoneExt, faxExt, email, password, website,
} = form.fields;
const formErrors = form.errors;

watch(form.fields.companyStatus, (v) => {
  companyStatusValue.value = (v as number | null) ?? null;
});

const statuses = ref<any[]>([]);
const industryOptions = ref<any[]>([]);
const industrySelected = ref<any>(null);
const salesRepresentatives = ref<any[]>([]);
const salesRepresentativeSelected = ref<any>(null);
const company = ref<any>({
  logo: {},
  industry: { industry: null, otherIndustry: null },
  requiresPermissionToSeeRequests: false,
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
  industryOptions.value = await getIndustries();
  statuses.value = await getCompanyStatus();
  salesRepresentatives.value = await getAgencyPersonnel();
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

function submit(): Promise<boolean> {
  form.markInteracted();
  return new Promise<boolean>((resolve) => {
    form.handleSubmit(
      async (values) => {
        if (!validateAutocompleteSelections()) {
          showAlertError('Please make sure all required fields are filled out correctly');
          resolve(false);
          return;
        }
        const payload: any = {
          ...company.value,
          fullName: values.fullName,
          companyStatus: values.companyStatus,
          about: values.about,
          phoneExt: values.phoneExt ? parseInt(values.phoneExt, 10) : null,
          faxExt: values.faxExt ? parseInt(values.faxExt, 10) : null,
          website: values.website,
          salesRepresentativeId: salesRepresentativeSelected.value?.id || null,
          industry: { ...company.value.industry, industry: industrySelected.value },
          email: values.email,
        };
        if (displayPassword.value) payload.password = values.password;
        try {
          await createAgencyCompany(payload);
          showAlertSuccess('Company created');
          resolve(true);
        } catch (error) {
          showAlertError((error as { data?: unknown }).data);
          resolve(false);
        }
      },
      () => {
        showAlertError('Please make sure all required fields are filled out correctly');
        resolve(false);
      }
    )();
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

defineExpose({ submit });
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-form {
  display: flex;
  flex-direction: column;
  gap: 0.2rem;

  :deep(.label) {
    font-size: 0.75rem;
    font-weight: 600;
    color: #777;
    margin-bottom: 0.35rem;
  }

  :deep(.input),
  :deep(.textarea),
  :deep(.select select) {
    font-size: 0.82rem;
    border-color: $gray-border;
    box-shadow: none;
    color: #333;

    &:focus,
    &:active {
      border-color: $primary;
      box-shadow: 0 0 0 2px rgba(33, 183, 255, 0.15);
    }
  }

  :deep(.textarea) {
    min-height: 5.5rem;
  }
}

.sd-form__logo {
  display: flex;
  justify-content: center;
  margin-bottom: 0.4rem;
}

.sd-form__check {
  margin-bottom: 0.2rem;
}

.sd-form__row {
  display: flex;
  gap: 0.7rem;
}

.sd-form__col {
  flex: 1;
  min-width: 0;
}

.sd-form__editor {
  width: 100%;

  :deep(.ql-container) {
    font-size: 0.82rem;
  }

  :deep(.ql-editor) {
    min-height: 5rem;
  }
}
</style>
