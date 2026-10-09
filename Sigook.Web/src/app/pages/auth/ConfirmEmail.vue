<template>
  <div class="auth-page">
    <div class="auth-card">
      <div class="auth-card__head">
        <router-link to="/">
          <img src="@/assets/images/logo-white-v2.png" alt="Sigook" class="auth-card__logo" />
        </router-link>
        <div v-if="status === 'loading'">
          <h1 class="auth-card__title">Confirming your account</h1>
          <p class="auth-card__subtitle">This will only take a moment.</p>
        </div>
        <div v-else-if="status === 'success'">
          <h1 class="auth-card__title">Account confirmed</h1>
          <p class="auth-card__subtitle">Your email address is confirmed. You can sign in now.</p>
        </div>
        <div v-else>
          <h1 class="auth-card__title">We could not confirm your account</h1>
          <p class="auth-card__subtitle">{{ authErrorMessage(errorCode) }}</p>
        </div>
      </div>

      <b-button v-if="status === 'success'" tag="router-link" to="/login" class="auth-btn auth-btn--primary" expanded>
        Sign in
      </b-button>

      <div v-if="status === 'error'" class="auth-card__links auth-card__links--center">
        <router-link to="/login">Back to sign in</router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import { isAxiosError } from 'axios';
import { authErrorMessage } from '@/app/security/authErrors';
import { confirmEmail } from '@/app/security/authApi';
import type { PasswordResetErrorResponse } from '@/shared/types/security';

const route = useRoute();

const status = ref<'loading' | 'success' | 'error'>('loading');
const errorCode = ref('');

onMounted(async () => {
  const { id, token } = route.query;
  if (typeof id !== 'string' || typeof token !== 'string' || !id || !token) {
    errorCode.value = 'invalid_token';
    status.value = 'error';
    return;
  }
  try {
    await confirmEmail({ id, token });
    status.value = 'success';
  } catch (e) {
    errorCode.value = isAxiosError(e) && e.response?.status === 400
      ? (e.response.data as PasswordResetErrorResponse).error ?? 'invalid_token'
      : 'unknown';
    status.value = 'error';
  }
});
</script>
