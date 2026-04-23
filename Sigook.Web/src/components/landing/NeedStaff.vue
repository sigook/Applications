
<template>
  <form :class="cssClass" @submit.prevent="sendMessage">
    <input type="text" class="control-borderer" placeholder="Company Name" v-model="companyName" />
    <span v-if="formErrors.companyName" class="help is-danger">{{ formErrors.companyName }}</span>
    <input type="text" class="control-borderer" placeholder="Address" v-model="address" />
    <span v-if="formErrors.address" class="help is-danger">{{ formErrors.address }}</span>
    <input type="tel" class="control-borderer" placeholder="Phone" name="phone" v-model="phone"
      @input="onPhoneInput" maxlength="12" />
    <span v-if="formErrors.phone" class="help is-danger">{{ formErrors.phone }}</span>
    <input type="email" class="control-borderer" placeholder="Email" v-model="email" />
    <span v-if="formErrors.email" class="help is-danger">{{ formErrors.email }}</span>
    <br />
    <vue-recaptcha ref="recaptcha" :sitekey="siteKey" @verify="onRecaptchaResponse"></vue-recaptcha>
    <div></div>
    <div></div>
    <div></div>
    <button class="bg-red-light color-white button-borderer mt-1" type="submit">Send Message</button>
    <div></div>
    <div></div>
    <h6 class="text-danger">{{ formError }}</h6>
  </form>
</template>
<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import * as yup from 'yup';
import { recaptchaSiteKey } from '@/utils/recaptcha';
import { submitContactForm } from '@/api/websiteApi';
import { phoneSchema } from '@/utils/validation';
import { formatPhone } from '@/utils/phoneFormat';
import { useStickyForm } from '@/composables/useStickyForm';

defineProps<{ cssClass?: string; isLoading?: boolean }>();
const emit = defineEmits<{ (e: 'onLoading', loading: boolean): void }>();

const schema = yup.object({
  companyName: yup.string().required('Company Name is required').max(50, 'Max 50 characters'),
  address: yup.string().required('Address is required').max(50, 'Max 50 characters'),
  phone: phoneSchema(true),
  email: yup.string().required('Email is required').email('Invalid email').max(50, 'Max 50 characters'),
});

const form = useStickyForm<{ companyName: string; address: string; phone: string; email: string }>({
  schema,
  initialValues: { companyName: '', address: '', phone: '', email: '' },
});
const { companyName, address, phone, email } = form.fields;
const formErrors = form.errors;

const title = 'REQUEST PERSONNEL FORM ~ NOTIFICATION';
const subject = 'Contact Request Staff';
const emailSetting = import.meta.env.VUE_APP_SIGOOK_NOTIFICATION as string;

const siteKey = recaptchaSiteKey;
const captchaResponse = ref<string | null>(null);
const formError = ref<string | null>(null);
const recaptcha = useTemplateRef<{ reset: () => void }>('recaptcha');

function onPhoneInput(e: Event) {
  const formatted = formatPhone((e.target as HTMLInputElement).value);
  form.setFieldValue('phone', formatted);
}

function onRecaptchaResponse(result: string) {
  captchaResponse.value = result;
}

async function sendMessage() {
  form.markInteracted();
  const { valid, errors } = await form.validate();
  if (!valid) {
    formError.value = errors && (errors as any).phone
      ? 'Please use a valid phone number in the United States'
      : 'Please check the information provided and re-submit';
    return;
  }
  if (!captchaResponse.value) {
    formError.value = 'reCaptcha invalid';
    return;
  }
  emit('onLoading', true);
  formError.value = null;
  const payload = {
    title,
    subject,
    emailSetting,
    companyName: companyName.value,
    address: address.value,
    phone: phone.value,
    email: email.value,
    captchaResponse: captchaResponse.value,
  };
  submitContactForm(payload as any)
    .then(() => {
      resetContactForm();
      emit('onLoading', false);
    })
    .catch(() => {
      resetContactForm();
      emit('onLoading', false);
    });
}

function resetContactForm() {
  form.resetAll();
  captchaResponse.value = null;
  recaptcha.value?.reset();
}
</script>
