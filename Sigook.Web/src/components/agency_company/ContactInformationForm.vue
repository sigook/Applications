<template>
  <div class="p-3">
    <h2 class="text-center main-title">{{ 'Contact Information' }}</h2>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input ref="phoneComponent" :required="true" model="Phone" :defaultValue="localModel.phone"
          @formattedPhone="(phone) => localModel.phone = phone" />
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.phoneExt ? 'is-danger' : ''" label="Phone Ext"
          :message="formErrors.phoneExt || ''">
          <b-input type="text" v-model="phoneExt" name="phoneExt" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input ref="faxComponent" :required="false" model="Fax" :defaultValue="localModel.fax"
          @formattedPhone="(phone) => localModel.fax = phone" />
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.faxExt ? 'is-danger' : ''" label="Fax Ext"
          :message="formErrors.faxExt || ''">
          <b-input type="text" v-model="faxExt" name="faxExt" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :type="formErrors.website ? 'is-danger' : ''" label="Website"
          :message="formErrors.website || ''">
          <b-input type="text" v-model="website" name="website" placeholder="www.example.com" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateForm">
          {{ 'Save' }}
        </b-button>
      </div>
    </div>
  </div>

</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { updateAgencyCompanyContactInformation } from "@/api/agencyCompanyApi";
import PhoneInput from "@/components/PhoneInput.vue";

const numericExt = yup
  .string()
  .nullable()
  .transform((v) => (v === '' ? null : v))
  .matches(/^\d{1,8}$/, { message: 'Must be 1-8 digits', excludeEmptyString: true });

const urlRegex = /^((https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?)$/i;

const schema = yup.object({
  phoneExt: numericExt,
  faxExt: numericExt,
  website: yup
    .string()
    .required('Website is required')
    .max(50, 'Max 50 characters')
    .matches(urlRegex, 'Invalid URL'),
});

const props = defineProps<{ model: any }>();
const emit = defineEmits<{
  (e: 'update:model', value: any): void;
  (e: 'save'): void;
}>();

const form = useStickyForm<{ phoneExt: string; faxExt: string; website: string }>({
  schema,
  initialValues: {
    phoneExt: props.model?.phoneExt != null ? String(props.model.phoneExt) : '',
    faxExt: props.model?.faxExt != null ? String(props.model.faxExt) : '',
    website: props.model?.website || '',
  },
});
const { phoneExt, faxExt, website } = form.fields;
const formErrors = form.errors;

const phoneComponent = ref<any>(null);
const faxComponent = ref<any>(null);

const localModel = ref<any>(JSON.parse(JSON.stringify(props.model)));

watch(() => props.model, (newVal) => {
  localModel.value = JSON.parse(JSON.stringify(newVal));
  form.hydrate({
    phoneExt: newVal?.phoneExt != null ? String(newVal.phoneExt) : '',
    faxExt: newVal?.faxExt != null ? String(newVal.faxExt) : '',
    website: newVal?.website || '',
  });
}, { deep: true });

async function validateForm() {
  form.markInteracted();
  const phoneValid = await phoneComponent.value.validatePhone();
  const faxValid = await faxComponent.value.validatePhone();
  form.handleSubmit((values) => {
    if (!phoneValid || !faxValid) {
      showAlertError('Please make sure all required fields are filled out correctly');
      return;
    }
    localModel.value.phoneExt = values.phoneExt ? parseInt(values.phoneExt, 10) : null;
    localModel.value.faxExt = values.faxExt ? parseInt(values.faxExt, 10) : null;
    localModel.value.website = values.website;
    saveContactInformation();
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function saveContactInformation() {
  emit('update:model', localModel.value);
  updateAgencyCompanyContactInformation(localModel.value.id, localModel.value)
    .then(() => {
      emit('save');
      showAlertSuccess("Updated");
    })
    .catch((error: unknown) => {
      showAlertError(error);
    });
}
</script>
