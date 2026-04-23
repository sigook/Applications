<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <section class="container-fluid color-white">
      <div class="p-5 text-center contact-banner-bg">
        <h2 class="font-weight-bold">Get in touch</h2>
      </div>
    </section>
    <section class="px-3 py-5 px-2 px-lg-5">
      <div class="d-lg-flex p-xl-5 justify-content-center gap-2 gap-xl-5">
        <form @submit.prevent="validateContactData">
          <div class="flex-md-column">
            <div class="businesses-staff-form">
              <input type="text" class="control-borderer" placeholder="Name" v-model="name" />
              <span v-if="formErrors.name" class="help is-danger">{{ formErrors.name }}</span>
              <input type="tel" class="control-borderer" placeholder="Phone" name="phone" v-model="phone"
                @input="onPhoneInput" maxlength="12" />
              <span v-if="formErrors.phone" class="help is-danger">{{ formErrors.phone }}</span>
              <input type="email" class="control-borderer" placeholder="Email" v-model="email" />
              <span v-if="formErrors.email" class="help is-danger">{{ formErrors.email }}</span>
              <textarea placeholder="Type your message here..." rows="8" v-model="message"
                class="businesses-staff-textarea control-borderer"></textarea>
              <span v-if="formErrors.message" class="help is-danger">{{ formErrors.message }}</span>
              <vue-recaptcha ref="recaptcha" :sitekey="siteKey" @verify="onRecaptchaResponse"></vue-recaptcha>
            </div>
            <button class="bg-red-light color-white button-borderer mt-1" type="submit">
              Send Message
            </button>
            <h6 class="text-danger">{{ formError }}</h6>
          </div>
        </form>
        <div class="ml-4 ml-sm-5 mt-5 mt-lg-0 flex-lg-column flex-lx-fill">
          <h3 class="font-weight-bold mb-3">
            SIGOOK<label class="superscript">®</label>
          </h3>
          <div class="d-flex align-items-start">
            <img src="@/assets/images/icon_localization_red.png" class="mr-2" />
            <address>
              1451 West Cypress Creek Road, Suite 300 <br />
              Fort Lauderdale, FL 33309, USA
            </address>
          </div>
          <p>
            <img src="@/assets/images/icon_phone_red.png" class="mr-2" /> +1 954
            9320477
          </p>
          <p>
            <img src="@/assets/images/icon_email_red.png" class="mr-2" />
            <a href="mailto:appmanager@sigook.com">appmanager@sigook.com</a>
          </p>
          <div class="d-flex gap-4 py-4">
            <a href="https://www.facebook.com/people/sigook_work_factory/100085428074968/">
              <img src="@/assets/images/icon_facebook_red.png" />
            </a>
            <a href="https://www.linkedin.com/company/sigookworkfactory/" target="_blank">
              <img src="@/assets/images/icon_linkedin_red.png" class="ml-5" />
            </a>
            <a href="https://www.instagram.com/sigook_work_factory/" target="_blank">
              <img src="@/assets/images/icon_instagram_red.png" class="ml-5" />
            </a>
          </div>
        </div>
      </div>
    </section>
    <section class="color-white pb-sm-5 mb-sm-5">
      <div class="app-section-grid app-section-bg py-5 py-sm-0 px-5">
        <div class="ml-lg-5 px-lg-5">
          <label class="app-section-font">Download our app</label>
          <label class="app-section-font font-weight-bold">and reach your goals now</label>
          <div class="app-section-buttons-grid pt-2 pt-sm-4">
            <a class="p-0 border-0 bg-transparent" href="https://apps.apple.com/us/app/sigook/id1446736193"
              target="_blank">
              <img src="@/assets/images/appstore.png" class="w-100" alt="appstore button" />
            </a>
            <a class="p-0 border-0 ml-3 bg-transparent"
              href="https://play.google.com/store/apps/details?id=com.sigook.sigook" target="_blank">
              <img src="@/assets/images/googleplay.png" class="w-100" alt="playstore button" />
            </a>
          </div>
        </div>
        <img v-lazy="appMobile" class="w-100 grid-item-start hide-on-mobile" alt="app mobile" />
      </div>
    </section>
    <b-modal v-model="showConfirmationModal">
      <div class="modal-container modal-light overflow-initial border-radius">
        <div class="d-flex p-3 border-bottom align-items-center justify-content-between pb-5">
          <label class="fs-5 fw-semibold lh-1">Confirm contact</label>
          <button @click="showConfirmationModal = false" type="button" class="btn-close"></button>
        </div>
        <div class="p-5 border-bottom">
          By entering your information you agree to receive a call or messages
          from SIGOOK<label class="superscript">®</label> (Covenant Group
          Investors LLC) regarding job opportunities. You can opt-out of
          receiving messages at any time by replying STOP. Message and data
          rates may apply. Carriers are not liable for delayed or undelivered
          messages.
        </div>
        <div class="pt-5 px-5 pb-3 d-flex justify-content-end gap-2">
          <button type="button" class="btn btn-secondary" @click="showConfirmationModal = false">
            Stop
          </button>
          <button type="button" class="btn btn-primary" @click="sendMessage()">
            Accept
          </button>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import * as yup from 'yup';
import { recaptchaSiteKey } from '@/utils/recaptcha';
import { submitContactForm } from "@/api/websiteApi";
import appMobile from '@/assets/images/app_mobile.png';
import { phoneSchema } from '@/utils/validation';
import { formatPhone } from '@/utils/phoneFormat';
import { useStickyForm } from '@/composables/useStickyForm';

const schema = yup.object({
  name: yup.string().required('Name is required').max(50, 'Max 50 characters'),
  phone: phoneSchema(true),
  email: yup.string().required('Email is required').email('Invalid email').max(50, 'Max 50 characters'),
  message: yup.string().required('Message is required').max(255, 'Max 255 characters'),
});

const form = useStickyForm<{ name: string; phone: string; email: string; message: string }>({
  schema,
  initialValues: { name: '', phone: '', email: '', message: '' },
});
const { name, phone, email, message } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const title = "CONTACT FORM ~ NOTIFICATION";
const subject = "Contact From Sigook™";
const emailSetting = import.meta.env.VUE_APP_SIGOOK_NOTIFICATION as string;
const captchaResponse = ref<string | null>(null);
const formError = ref<string | null>(null);
const showConfirmationModal = ref(false);
const recaptcha = ref<any>(null);

const siteKey = computed(() => recaptchaSiteKey);

function onPhoneInput(e: Event) {
  const formatted = formatPhone((e.target as HTMLInputElement).value);
  form.setFieldValue('phone', formatted);
}

function onRecaptchaResponse(result: string) {
  captchaResponse.value = result;
}

async function validateContactData() {
  form.markInteracted();
  const { valid, errors } = await form.validate();
  if (!valid) {
    formError.value = errors && (errors as any).phone
      ? "Please use a valid phone number in the United States"
      : "Please check the information provided and re-submit";
    return;
  }
  if (!captchaResponse.value) {
    formError.value = "reCaptcha invalid";
    return;
  }
  formError.value = null;
  showConfirmationModal.value = true;
}

function sendMessage() {
  showConfirmationModal.value = false;
  isLoading.value = true;
  formError.value = null;
  const payload = {
    title,
    subject,
    emailSetting,
    name: name.value,
    phone: phone.value,
    email: email.value,
    message: message.value,
    captchaResponse: captchaResponse.value,
  };
  submitContactForm(payload as any)
    .then(() => {
      resetContactForm();
      isLoading.value = false;
    })
    .catch(() => {
      resetContactForm();
      isLoading.value = false;
    });
}

function resetContactForm() {
  form.resetAll();
  captchaResponse.value = null;
  recaptcha.value?.reset();
}
</script>

<style lang="scss">
@import "../../assets/scss/mixins";
@import "../../assets/scss/variables";

.contact-banner-bg {
  background: url("../../assets/images/contact_banner_bg.png") right bottom no-repeat;
  background-size: cover;

  h2 {
    font-size: 3.7rem;
  }

  @include only-mobile {
    h2 {
      font-size: 2.5rem;
    }
  }
}

.businesses-staff-textarea {
  resize: none;

  @include tablet {
    grid-column: 1/4;
  }
}
</style>
