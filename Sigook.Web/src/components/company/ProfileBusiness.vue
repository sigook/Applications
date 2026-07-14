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
          <b-autocomplete v-model="industry" :data="filteredIndustries" field="value" :placeholder="'Select industry'"
            open-on-focus @select="selectIndustry">
          </b-autocomplete>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <PhoneInput ref="phoneComponent" :required="true" model="Phone" label="Phone"
          :defaultValue="localCompanyData.phone" @formattedPhone="(phone: string) => localCompanyData.phone = phone" />
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <b-field label="Phone Ext" :type="formErrors.phoneExt ? 'is-danger' : ''" :message="formErrors.phoneExt || ''">
          <b-input type="text" v-model="phoneExt" name="phoneExt" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Website" :type="formErrors.website ? 'is-danger' : ''" :message="formErrors.website || ''">
          <b-input type="text" v-model="website" name="website" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <PhoneInput ref="faxComponent" :required="false" model="Fax" label="Fax" :defaultValue="localCompanyData.fax"
          @formattedPhone="(phone: string) => localCompanyData.fax = phone" />
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <b-field label="Fax Ext" :type="formErrors.faxExt ? 'is-danger' : ''" :message="formErrors.faxExt || ''">
          <b-input type="text" v-model="faxExt" name="faxExt" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="save">Save</b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import * as yup from 'yup';
import PhoneInput from "../../components/PhoneInput.vue";
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { getIndustries } from "@/api/catalogApi";
import { updateProfile } from "@/api/companyApi";

const props = defineProps<{ companyData: any }>();
const emit = defineEmits<{ (e: 'update:companyData', value: any): void }>();

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

const form = useStickyForm<{
  businessName: string; fullName: string; industry: string;
  phoneExt: string; faxExt: string; website: string;
}>({
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
const { businessName, fullName, industry, phoneExt, faxExt, website } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const industries = ref<any[]>([]);
const industrySelected = ref<any>(null);
const localCompanyData = ref<any>(JSON.parse(JSON.stringify(props.companyData)));
const phoneComponent = ref<any>(null);
const faxComponent = ref<any>(null);

watch(() => props.companyData, (newVal) => {
  localCompanyData.value = JSON.parse(JSON.stringify(newVal));
  form.hydrate({
    businessName: newVal?.businessName || '',
    fullName: newVal?.fullName || '',
    industry: newVal?.industry?.industry?.value || '',
    phoneExt: newVal?.phoneExt != null ? String(newVal.phoneExt) : '',
    faxExt: newVal?.faxExt != null ? String(newVal.faxExt) : '',
    website: newVal?.website || '',
  });
}, { deep: true });

const filteredIndustries = computed(() => {
  const search = (industry.value || '').toLowerCase();
  return industries.value.filter((i: any) => (i.value || '').toLowerCase().includes(search));
});

function selectIndustry(option: any) {
  industrySelected.value = option || null;
}

async function save() {
  form.markInteracted();
  const phoneValid = await phoneComponent.value?.validatePhone();
  const faxValid = await faxComponent.value?.validatePhone();
  form.handleSubmit((values) => {
    if (!phoneValid || !faxValid) return;
    isLoading.value = true;
    const updated = {
      ...localCompanyData.value,
      businessName: values.businessName,
      fullName: values.fullName,
      phoneExt: values.phoneExt ? parseInt(values.phoneExt, 10) : null,
      faxExt: values.faxExt ? parseInt(values.faxExt, 10) : null,
      website: values.website,
      industry: industrySelected.value
        ? { ...localCompanyData.value.industry, industry: industrySelected.value }
        : null,
    };
    emit('update:companyData', updated);
    updateProfile(updated.id, updated)
      .then(() => {
        isLoading.value = false;
        showAlertSuccess('Profile updated');
      })
      .catch((error: unknown) => {
        isLoading.value = false;
        showAlertError((error as { data?: unknown }).data);
      });
  })();
}

(async () => {
  industries.value = await getIndustries();
  if (localCompanyData.value.industry?.industry) {
    industrySelected.value = localCompanyData.value.industry.industry;
  }
})();
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
