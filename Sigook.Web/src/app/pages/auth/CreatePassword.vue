<template>
  <div class="auth-page">
    <div class="auth-card">
      <div class="auth-card__head">
        <router-link to="/">
          <img src="@/assets/images/logo-white-v2.png" alt="Sigook" class="auth-card__logo" />
        </router-link>
        <div v-if="!completed">
          <h1 class="auth-card__title">Create your password</h1>
          <p class="auth-card__subtitle">Choose a password to confirm your account.</p>
        </div>
        <div v-else>
          <h1 class="auth-card__title">You are all set</h1>
          <p class="auth-card__subtitle">Your password was saved and your account is confirmed.</p>
        </div>
      </div>

      <form v-if="!completed" novalidate @submit.prevent="onSubmit">
        <b-field label="Password" :type="errors.password ? 'is-danger' : ''" :message="errors.password">
          <b-input v-model="password" type="password" placeholder="••••••••" autocomplete="new-password" password-reveal />
        </b-field>
        <b-field
          label="Confirm password"
          :type="errors.confirmPassword ? 'is-danger' : ''"
          :message="errors.confirmPassword"
        >
          <b-input v-model="confirmPassword" type="password" placeholder="••••••••" autocomplete="new-password" />
        </b-field>
        <div v-if="submitError" class="auth-card__error">
          <p>{{ submitError }}</p>
          <ul v-if="policyMessages.length">
            <li v-for="message in policyMessages" :key="message">{{ message }}</li>
          </ul>
          <router-link v-if="errorCode === 'invalid_token'" to="/forgot-password">Request a new link</router-link>
        </div>
        <b-button native-type="submit" class="auth-btn auth-btn--primary" expanded :loading="isLoading">
          Save password
        </b-button>
      </form>

      <b-button v-else tag="router-link" to="/login" class="auth-btn auth-btn--primary" expanded>
        Sign in
      </b-button>

      <div v-if="!completed" class="auth-card__links auth-card__links--center">
        <router-link to="/login">Back to sign in</router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';
import { useField, useForm } from 'vee-validate';
import * as yup from 'yup';
import { isAxiosError } from 'axios';
import { authErrorMessage } from '@/app/security/authErrors';
import { createPassword } from '@/app/security/authApi';
import type { PasswordResetErrorResponse } from '@/shared/types/security';

const route = useRoute();

const id = typeof route.query.id === 'string' ? route.query.id : '';
const token = typeof route.query.token === 'string' ? route.query.token : '';

const isLoading = ref(false);
const completed = ref(false);
const errorCode = ref(id && token ? '' : 'invalid_token');
const policyMessages = ref<string[]>([]);
const submitError = computed(() => (errorCode.value ? authErrorMessage(errorCode.value) : ''));

const validationSchema = yup.object({
  password: yup.string().required('Password is required').min(6, 'Min 6 characters'),
  confirmPassword: yup
    .string()
    .required('Confirm password is required')
    .oneOf([yup.ref('password')], 'Passwords must match'),
});

const { handleSubmit, errors } = useForm({
  validationSchema,
  initialValues: { password: '', confirmPassword: '' },
});
const { value: password } = useField<string>('password');
const { value: confirmPassword } = useField<string>('confirmPassword');

const onSubmit = handleSubmit(async (values) => {
  isLoading.value = true;
  errorCode.value = '';
  policyMessages.value = [];
  try {
    await createPassword({ id, token, password: values.password });
    completed.value = true;
  } catch (e) {
    if (isAxiosError(e) && e.response?.status === 400) {
      const data = e.response.data as PasswordResetErrorResponse;
      errorCode.value = data.error ?? 'invalid_token';
      if (data.error === 'password_policy') policyMessages.value = data.messages ?? [];
    } else {
      errorCode.value = 'unknown';
    }
  } finally {
    isLoading.value = false;
  }
});
</script>
