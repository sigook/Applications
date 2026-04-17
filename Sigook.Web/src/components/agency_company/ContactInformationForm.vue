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

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { updateAgencyCompanyContactInformation } from "@/api/agencyCompanyApi";

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

export default {
  name: 'ContactInformationForm',
  props: ["model"],
  setup(props: any) {
    const form = useStickyForm({
      schema,
      initialValues: {
        phoneExt: props.model?.phoneExt != null ? String(props.model.phoneExt) : '',
        faxExt: props.model?.faxExt != null ? String(props.model.faxExt) : '',
        website: props.model?.website || '',
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
      localModel: JSON.parse(JSON.stringify((this as any).model)),
    };
  },
  watch: {
    model: {
      handler(newVal: any) {
        this.localModel = JSON.parse(JSON.stringify(newVal));
        this.hydrateForm({
          phoneExt: newVal?.phoneExt != null ? String(newVal.phoneExt) : '',
          faxExt: newVal?.faxExt != null ? String(newVal.faxExt) : '',
          website: newVal?.website || '',
        });
      },
      deep: true,
    },
  },
  methods: {
    async validateForm() {
      this.markInteracted();
      const phoneValid = await (this.$refs.phoneComponent as any).validatePhone();
      const faxValid = await (this.$refs.faxComponent as any).validatePhone();
      this.handleSubmit((values) => {
        if (!phoneValid || !faxValid) {
          showAlertError('Please make sure all required fields are filled out correctly');
          return;
        }
        this.localModel.phoneExt = values.phoneExt ? parseInt(values.phoneExt, 10) : null;
        this.localModel.faxExt = values.faxExt ? parseInt(values.faxExt, 10) : null;
        this.localModel.website = values.website;
        this.saveContactInformation();
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    saveContactInformation() {
      this.$emit('update:model', this.localModel);
      updateAgencyCompanyContactInformation(this.localModel.id, this.localModel)
        .then(() => {
          this.$emit('save');
          showAlertSuccess("Updated");
        })
        .catch((error: any) => {
          showAlertError(error);
        });
    },
  },
  components: {
    phoneInput: defineAsyncComponent(() => import("@/components/PhoneInput.vue")),
  },
};
</script>
