<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-6">
        <b-field label="Agency Name" :type="formErrors.fullName ? 'is-danger' : ''"
          :message="formErrors.fullName || ''">
          <b-input v-model="fullName" name="agency name" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Agency HST Number" :type="formErrors.hstNumber ? 'is-danger' : ''"
          :message="formErrors.hstNumber || ''">
          <b-input v-model="hstNumber" name="hts #" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="BN/EIN" :type="formErrors.businessNumber ? 'is-danger' : ''"
          :message="formErrors.businessNumber || ''">
          <b-input v-model="businessNumber" name="business #" />
        </b-field>
      </div>
      <div class="column is-3">
        <phone-input ref="phoneComponent" :required="true" :defaultValue="localAgencyData.phonePrincipal"
          model="Phone Principal" @formattedPhone="(phone) => (localAgencyData.phonePrincipal = phone)">
        </phone-input>
      </div>
      <div class="column is-3">
        <b-field label="Ext" :type="formErrors.phonePrincipalExt ? 'is-danger' : ''"
          :message="formErrors.phonePrincipalExt || ''">
          <b-input v-model="phonePrincipalExt" name="phonePrincipalExt" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Web Page" :type="formErrors.webPage ? 'is-danger' : ''"
          :message="formErrors.webPage || ''">
          <b-input v-model="webPage" name="web page" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="WSIB Group" :type="formErrors.wsibGroup ? 'is-danger' : ''"
          :message="formErrors.wsibGroup || ''">
          <b-taginput v-model="localAgencyData.wsibGroup" autocomplete :data="wsibGroups" open-on-focus field="value"
            icon="label" :placeholder="'Click here to add more'" :before-adding="beforeWsibBeingSelected">
          </b-taginput>
        </b-field>
      </div>
      <div class="column is-12">
        <b-button type="is-primary" @click="validateForm">SAVE</b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { showAlertSuccess } from "@/shared/utils/toast";
import { getWsibGroups } from "@/shared/api/catalogApi";
import { updateAgency } from '@/modules/agency/sales/agencies/api';
import phoneInput from "@/shared/ui/PhoneInput.vue";
import type { AgencyDetail } from '@/modules/agency/profile/types';
import type { WsibGroup } from '@/shared/types/common';

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
  webPage: yup
    .string()
    .nullable()
    .transform((v) => (v === '' ? null : v))
    .max(50, 'Max 50 characters')
    .matches(urlRegex, { message: 'Invalid URL', excludeEmptyString: true }),
});

const props = defineProps<{ agencyData: AgencyDetail }>();
const emit = defineEmits<{ (e: 'update:agencyData', data: AgencyDetail): void }>();

const form = useStickyForm({
  schema,
  initialValues: {
    fullName: props.agencyData?.fullName || '',
    hstNumber: props.agencyData?.hstNumber || '',
    businessNumber: props.agencyData?.businessNumber || '',
    phonePrincipalExt: props.agencyData?.phonePrincipalExt != null ? String(props.agencyData.phonePrincipalExt) : '',
    webPage: props.agencyData?.webPage || '',
  },
});
const { fullName, hstNumber, businessNumber, phonePrincipalExt, webPage } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const localAgencyData = ref<AgencyDetail>(JSON.parse(JSON.stringify(props.agencyData)));
const wsibGroups = ref<WsibGroup[]>([]);
const phoneComponent = ref<InstanceType<typeof phoneInput> | null>(null);

watch(
  () => props.agencyData,
  (newVal) => {
    localAgencyData.value = JSON.parse(JSON.stringify(newVal));
    form.hydrate({
      fullName: newVal?.fullName || '',
      hstNumber: newVal?.hstNumber || '',
      businessNumber: newVal?.businessNumber || '',
      phonePrincipalExt: newVal?.phonePrincipalExt != null ? String(newVal.phonePrincipalExt) : '',
      webPage: newVal?.webPage || '',
    });
  },
  { deep: true }
);

async function validateForm() {
  form.markInteracted();
  const phoneValid = await phoneComponent.value.validatePhone();
  form.handleSubmit((values) => {
    if (!phoneValid) return;
    isLoading.value = true;
    const updated = {
      ...localAgencyData.value,
      fullName: values.fullName,
      hstNumber: values.hstNumber,
      businessNumber: values.businessNumber,
      phonePrincipalExt: values.phonePrincipalExt ? parseInt(values.phonePrincipalExt, 10) : null,
      webPage: values.webPage,
    };
    emit('update:agencyData', updated);
    updateAgency(updated)
      .then(() => {
        isLoading.value = false;
        showAlertSuccess("Updated");
      })
      .catch(() => {
        isLoading.value = false;
      });
  })();
}

function beforeWsibBeingSelected(tag: WsibGroup) {
  return !localAgencyData.value.wsibGroup.some((item) => item.value === tag.value);
}

isLoading.value = true;
getWsibGroups()
  .then((response) => {
    isLoading.value = false;
    wsibGroups.value = response;
  })
  .catch(() => {
    isLoading.value = false;
  });
</script>
