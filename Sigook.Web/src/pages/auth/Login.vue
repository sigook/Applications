<template>
  <div class="auth-page">
    <div class="auth-card">
      <div class="auth-card__head">
        <router-link to="/">
          <img src="@/assets/images/logo-white-v2.png" alt="Sigook" class="auth-card__logo" />
        </router-link>
        <div>
          <h1 class="auth-card__title">Welcome back</h1>
          <p class="auth-card__subtitle">Sign in to your account</p>
        </div>
      </div>
      <form novalidate @submit.prevent="onSubmit">
        <b-field label="Email" :type="errors.email ? 'is-danger' : ''" :message="errors.email">
          <b-input v-model="email" type="email" placeholder="you@example.com" autocomplete="username" />
        </b-field>
        <b-field label="Password" :type="errors.password ? 'is-danger' : ''" :message="errors.password">
          <b-input v-model="password" type="password" placeholder="••••••••" autocomplete="current-password" password-reveal />
        </b-field>
        <div v-if="loginError" class="auth-card__error">
          <p>{{ loginError }}</p>
          <a v-if="showResend" href="#" @click.prevent="resend">Resend confirmation email</a>
        </div>
        <b-button native-type="submit" class="auth-btn auth-btn--primary" expanded :loading="isLoading">
          Sign In
        </b-button>
      </form>
      <div class="auth-card__divider"><span>or</span></div>
      <b-button class="auth-btn auth-btn--ghost" expanded @click="onMicrosoft">
        <img src="@/assets/images/office.svg" alt="" class="auth-btn__icon" />
        <span>Continue with Microsoft 365</span>
      </b-button>
      <div class="auth-card__links">
        <router-link to="/forgot-password">Forgot password?</router-link>
        <router-link to="/register-worker">Create an account</router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useField, useForm } from 'vee-validate';
import * as yup from 'yup';
import { isAxiosError } from 'axios';
import { useSecurityStore } from '@/stores/security';
import menu from '@/security/menu';
import { authErrorMessage } from '@/security/authErrors';
import { resendConfirmationLink } from '@/api/authApi';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import type { TokenErrorResponse } from '@/types/security';

const route = useRoute();
const router = useRouter();
const securityStore = useSecurityStore();

const validationSchema = yup.object({
  email: yup.string().required('Email is required').email('Invalid email'),
  password: yup.string().required('Password is required'),
});

const { handleSubmit, errors } = useForm({
  validationSchema,
  initialValues: { email: '', password: '' },
});
const { value: email } = useField<string>('email');
const { value: password } = useField<string>('password');

const isLoading = ref(false);
const errorCode = ref('');
const loginError = computed(() => (errorCode.value ? authErrorMessage(errorCode.value) : ''));
const showResend = computed(() => errorCode.value === 'email_not_confirmed');

function targetUrl(): string {
  const returnUrl = route.query.returnUrl;
  if (typeof returnUrl === 'string' && returnUrl.startsWith('/')) return returnUrl;
  return menu.getDefaultHomePageUrlBaseOnRoles(securityStore.userRoles);
}

if (securityStore.user) {
  router.replace(targetUrl());
}

const onSubmit = handleSubmit(async (values) => {
  isLoading.value = true;
  errorCode.value = '';
  try {
    await securityStore.signInWithPassword(values.email, values.password);
    router.push(targetUrl());
  } catch (e) {
    if (isAxiosError(e) && e.response?.status === 400) {
      const data = e.response.data as TokenErrorResponse;
      errorCode.value = data.error_description ?? data.error ?? 'unknown';
    } else {
      errorCode.value = 'unknown';
    }
  } finally {
    isLoading.value = false;
  }
});

async function resend(): Promise<void> {
  try {
    await resendConfirmationLink(email.value);
    showAlertSuccess('Confirmation email sent. Check your inbox.');
  } catch {
    await showAlertError('Could not send the confirmation email. Try again later.');
  }
}

async function onMicrosoft(): Promise<void> {
  await securityStore.signInWithMicrosoft();
}
</script>
