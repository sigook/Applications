<template>
  <form class="contact-form" @submit.prevent="handleSubmit">
    <header class="contact-form__header">
      <h2 class="contact-form__title">Your Information</h2>
      <p class="contact-form__subtitle">We’ll reach out to you for the next steps</p>
    </header>

    <div class="contact-form__group">
      <div class="contact-form__field">
        <label class="contact-form__label">Name</label>
        <input
          v-model="name"
          v-bind="nameAttrs"
          type="text"
          class="contact-form__input"
          :class="{ 'contact-form__input--error': errors.name }"
          placeholder="Name"
        />
        <span v-if="errors.name" class="contact-form__error">{{ errors.name }}</span>
      </div>
    </div>

    <div class="contact-form__group">
      <label class="contact-form__label">Email</label>
      <input
        v-model="email"
        v-bind="emailAttrs"
        type="email"
        class="contact-form__input"
        :class="{ 'contact-form__input--error': errors.email }"
        placeholder="Email"
      />
      <span v-if="errors.email" class="contact-form__error">{{ errors.email }}</span>
    </div>

    <div class="contact-form__group">
      <label class="contact-form__label">Phone</label>
      <input
        ref="phoneInput"
        v-bind="phoneAttrs"
        type="tel"
        class="contact-form__input"
        :class="{ 'contact-form__input--error': errors.phone }"
        placeholder="300 123-4567"
      />
      <span v-if="errors.phone" class="contact-form__error">{{ errors.phone }}</span>
    </div>

    <div class="contact-form__group">
      <label class="contact-form__label">Message</label>
      <textarea
        v-model="message"
        v-bind="messageAttrs"
        class="contact-form__input contact-form__textarea"
        :class="{ 'contact-form__input--error': errors.message }"
        rows="4"
        placeholder="Your message (optional)"
      ></textarea>
      <span v-if="errors.message" class="contact-form__error">{{ errors.message }}</span>
    </div>

    <div class="contact-form__recaptcha">
      <Vue3Recaptcha2
        v-if="showRecaptcha"
        :sitekey="siteKey"
        @verify="handleSuccess"
        @expire="handleExpired"
        @fail="handleError"
      />

      <p v-if="captchaError" class="error-text">
        Please verify you are human.
      </p>
    </div>

    <v-btn
      type="submit"
      class="contact-form__submit"
      :loading="isSubmitting"
      :disabled="isSubmitting"
      block
    >
      Save & Send
    </v-btn>
    <button type="button" class="contact-form__reset" @click="resetForm" :disabled="isSubmitting">Reset Information</button>
  </form>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import Vue3Recaptcha2 from 'vue3-recaptcha2';
import Cleave from 'cleave.js';
import { useForm } from 'vee-validate';
import * as yup from 'yup';
import { contactService } from '@/services/contactService';
import { useToast } from '@/composables/useToast';

// Props
const props = withDefaults(defineProps<{
  title?: string;
  subject: string;
  useDynamicTitle?: boolean;
  titleSuffix?: string;
}>(), {
  title: '',
  useDynamicTitle: false,
  titleSuffix: ''
});

const siteKey = import.meta.env.VITE_RECAPTCHA_SITE_KEY;
const { showSuccess, showError } = useToast();

// Validation schema with Yup
const validationSchema = yup.object({
  name: yup.string()
    .required('Name is required')
    .min(2, 'Name must be at least 2 characters'),
  email: yup.string()
    .required('Email is required')
    .email('Please enter a valid email address'),
  phone: yup.string()
    .required('Phone is required')
    .matches(/^\d{3} \d{3}-\d{4}$/, 'Phone format must be: ### ###-####'),
  message: yup.string()
    .max(1000, 'Message must be less than 1000 characters')
});

// VeeValidate form setup
const { errors, defineField, handleSubmit: veeHandleSubmit, resetForm: veeResetForm } = useForm({
  validationSchema,
  initialValues: {
    name: '',
    email: '',
    phone: '',
    message: ''
  }
});

// Define fields with VeeValidate
const [name, nameAttrs] = defineField('name');
const [email, emailAttrs] = defineField('email');
const [phone, phoneAttrs] = defineField('phone');
const [message, messageAttrs] = defineField('message');

// --- LÓGICA CLEAVE.JS (MÁSCARA TELÉFONO) ---
const phoneInput = ref<HTMLElement | null>(null);

onMounted(() => {
  if (phoneInput.value) {
    new Cleave(phoneInput.value, {
      blocks: [3, 3, 4],       // Formato: 123 456 7890
      delimiters: [' ', '-'],  // Separadores
      numericOnly: true,       // Solo números
      onValueChanged: (e) => {
        // Update VeeValidate model with formatted value
        phone.value = e.target.value;
      }
    });
  }
});

// --- LÓGICA RECAPTCHA ---
const captchaToken = ref<string | null>(null);
const captchaError = ref(false);
const showRecaptcha = ref(true);

// --- LOADING STATE ---
const isSubmitting = ref(false);

const handleSuccess = (token: string) => {
  console.log("Captcha verificado:", token);
  captchaToken.value = token;
  captchaError.value = false;
};

const handleExpired = () => {
  console.warn("Captcha expirado");
  captchaToken.value = null;
};

const handleError = () => {
  console.error("Error en Recaptcha");
};

// Handle form submission with VeeValidate
const handleSubmit = veeHandleSubmit(async (formValues) => {
  if (!captchaToken.value) {
    captchaError.value = true;
    return;
  }

  isSubmitting.value = true;

  try {
    // Build title: use dynamic title if enabled, otherwise use static title prop
    const emailTitle = props.useDynamicTitle
      ? `${formValues.name} ${props.titleSuffix}`
      : props.title;

    await contactService.sendContactEmail({
      title: emailTitle,
      name: formValues.name,
      email: formValues.email,
      phone: formValues.phone,
      message: formValues.message,
      subject: props.subject,
      captchaResponse: captchaToken.value,
      emailSetting: 4 // CovenantNotification
    });

    showSuccess('Message sent successfully!');
    resetForm();
  } catch (error) {
    console.error('Error sending contact form:', error);
    showError('Failed to send message. Please try again.');
  } finally {
    isSubmitting.value = false;
  }
});

const resetForm = () => {
  veeResetForm();
  captchaToken.value = null;
  captchaError.value = false;

  // Clear phone input manually (since we're not using v-model)
  if (phoneInput.value) {
    (phoneInput.value as HTMLInputElement).value = '';
  }

  // Reiniciar Recaptcha
  showRecaptcha.value = false;
  setTimeout(() => {
    showRecaptcha.value = true;
  }, 100);
};
</script>

<style scoped>
/* Tus estilos existentes se mantienen intactos */
.contact-form__recaptcha {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 15px 0;
  min-height: 78px;
}
.error-text {
  color: #e74c3c;
  font-size: 0.85rem;
  margin-top: 5px;
}

/* Validation error styles */
.contact-form__input--error,
.contact-form__textarea--error {
  border-color: #e74c3c !important;
  background-color: #fff5f5 !important;
}

.contact-form__error {
  display: block;
  color: #e74c3c;
  font-size: 0.8rem;
  margin-top: 4px;
  margin-left: 4px;
}

* {
  box-sizing: border-box;
}

.contact-form {
  width: 100%;
}

.contact-form__header {
  text-align: center;
  margin-bottom: 28px;
}

.contact-form__title {
  font-size: 1.6rem;
  font-weight: 700;
  color: #222222;
  margin-bottom: 4px;
}

.contact-form__subtitle {
  font-size: 0.9rem;
  color: #a0a0a0;
}

.contact-form__group {
  margin-bottom: 18px;
}

.contact-form__label {
  display: block;
  font-size: 0.9rem;
  font-weight: 600;
  color: #222222;
  margin-bottom: 6px;
}

.contact-form__input {
  width: 100%;
  padding: 10px 16px;
  border-radius: 999px;
  border: 1px solid #ececec;
  background-color: #fafafa;
  font-size: 0.9rem;
  outline: none;
  transition: border-color 0.18s ease, box-shadow 0.18s ease,
    background-color 0.18s ease;
  color: #333333;
}

.contact-form__input::placeholder {
  color: #c3c3c3;
}

.contact-form__textarea {
  border-radius: 18px;
  resize: vertical;
  min-height: 120px;
  padding-top: 12px;
}

.contact-form__input:focus,
.contact-form__textarea:focus {
  border-color: #32d26a;
  background-color: #ffffff;
  box-shadow: 0 0 0 2px rgba(50, 210, 106, 0.12);
}

.contact-form__submit {
  width: 100%;
  margin-top: 8px;
  padding: 12px 20px;
  border-radius: 999px;
  border: none;
  outline: none;
  background-color: #32d26a;
  color: #ffffff;
  font-weight: 700;
  font-size: 0.95rem;
  cursor: pointer;
  box-shadow: 0 14px 30px rgba(0, 0, 0, 0.25);
  transition: background-color 0.18s ease, transform 0.18s ease,
    box-shadow 0.18s ease;
}

.contact-form__submit:hover {
  background-color: #27b058;
  transform: translateY(-1px);
  box-shadow: 0 18px 36px rgba(0, 0, 0, 0.3);
}

.contact-form__reset {
  display: block;
  margin: 14px auto 0;
  border: none;
  background: none;
  color: #333333;
  font-size: 0.85rem;
  cursor: pointer;
  text-decoration: none;
}

.contact-form__reset:hover {
  text-decoration: underline;
}

@media (max-width: 480px) {
  .contact-form__title {
    font-size: 1.4rem;
  }
}
</style>
