<template>
  <div class="auth-page">
    <div class="auth-card">
      <div class="auth-card__head">
        <router-link to="/">
          <img src="@/assets/images/logo-white-v2.png" alt="Sigook" class="auth-card__logo" />
        </router-link>
        <div v-if="step === 1">
          <h1 class="auth-card__title">Reset your password</h1>
          <p class="auth-card__subtitle">Enter your account email and we will send you a 6-digit code.</p>
        </div>
        <div v-else>
          <h1 class="auth-card__title">Check your inbox</h1>
          <p class="auth-card__subtitle">
            If an account exists for <strong>{{ email }}</strong>, we sent it a 6-digit code. It expires in 15 minutes.
          </p>
        </div>
      </div>

      <form v-if="step === 1" novalidate @submit.prevent="onRequestCode">
        <b-field label="Email" :type="errors.email ? 'is-danger' : ''" :message="errors.email">
          <b-input v-model="email" type="email" placeholder="you@example.com" autocomplete="username" />
        </b-field>
        <b-button native-type="submit" class="auth-btn auth-btn--primary" expanded :loading="isLoading">
          Send code
        </b-button>
      </form>

      <form v-else novalidate @submit.prevent="onReset">
        <b-field label="Code" :type="errors.code ? 'is-danger' : ''" :message="errors.code">
          <b-input v-model="code" placeholder="123456" maxlength="6" inputmode="numeric" autocomplete="one-time-code" />
        </b-field>
        <b-field label="New password" :type="errors.newPassword ? 'is-danger' : ''" :message="errors.newPassword">
          <b-input v-model="newPassword" type="password" placeholder="••••••••" autocomplete="new-password" />
        </b-field>
        <b-field
          label="Confirm new password"
          :type="errors.confirmPassword ? 'is-danger' : ''"
          :message="errors.confirmPassword"
        >
          <b-input v-model="confirmPassword" type="password" placeholder="••••••••" autocomplete="new-password" />
        </b-field>
        <div v-if="resetError" class="auth-card__error">
          <p>{{ resetError }}</p>
          <ul v-if="policyMessages.length">
            <li v-for="message in policyMessages" :key="message">{{ message }}</li>
          </ul>
        </div>
        <b-button native-type="submit" class="auth-btn auth-btn--primary" expanded :loading="isLoading">
          Reset password
        </b-button>
      </form>

      <div class="auth-card__links" :class="{ 'auth-card__links--center': step === 1 }">
        <a
          v-if="step === 2"
          href="#"
          :class="{ 'auth-card__link--disabled': cooldown > 0 }"
          @click.prevent="onResend"
        >
          {{ cooldown > 0 ? `Resend code (${cooldown}s)` : 'Resend code' }}
        </a>
        <router-link to="/login">Back to sign in</router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, nextTick, onUnmounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useField, useForm } from 'vee-validate';
import * as yup from 'yup';
import { isAxiosError } from 'axios';
import { authErrorMessage } from '@/security/authErrors';
import { requestPasswordResetCode, resetPasswordWithCode } from '@/api/authApi';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import type { PasswordResetErrorResponse } from '@/types/security';

const RESEND_COOLDOWN_SECONDS = 60;

const router = useRouter();

const step = ref<1 | 2>(1);
const isLoading = ref(false);
const errorCode = ref('');
const policyMessages = ref<string[]>([]);
const resetError = computed(() => (errorCode.value ? authErrorMessage(errorCode.value) : ''));

const cooldown = ref(0);
let cooldownIntervalId: ReturnType<typeof setInterval> | null = null;

const validationSchema = computed(() => {
  if (step.value === 1) {
    return yup.object({
      email: yup.string().required('Email is required').email('Invalid email'),
    });
  }
  return yup.object({
    code: yup.string().required('Code is required').matches(/^\d{6}$/, 'Code must be 6 digits'),
    newPassword: yup.string().required('New password is required').min(6, 'Min 6 characters'),
    confirmPassword: yup
      .string()
      .required('Confirm password is required')
      .oneOf([yup.ref('newPassword')], 'Passwords must match'),
  });
});

const { handleSubmit, resetForm, errors } = useForm({
  validationSchema,
  initialValues: { email: '', code: '', newPassword: '', confirmPassword: '' },
});
const { value: email } = useField<string>('email');
const { value: code } = useField<string>('code');
const { value: newPassword } = useField<string>('newPassword');
const { value: confirmPassword } = useField<string>('confirmPassword');

function startCooldown(): void {
  cooldown.value = RESEND_COOLDOWN_SECONDS;
  if (cooldownIntervalId !== null) clearInterval(cooldownIntervalId);
  cooldownIntervalId = setInterval(() => {
    cooldown.value -= 1;
    if (cooldown.value <= 0 && cooldownIntervalId !== null) {
      clearInterval(cooldownIntervalId);
      cooldownIntervalId = null;
    }
  }, 1000);
}

const onRequestCode = handleSubmit(async () => {
  isLoading.value = true;
  try {
    await requestPasswordResetCode(email.value);
    step.value = 2;
    await nextTick();
    resetForm({ values: { email: email.value, code: '', newPassword: '', confirmPassword: '' } });
    startCooldown();
  } catch {
    await showAlertError('Could not send the code. Try again later.');
  } finally {
    isLoading.value = false;
  }
});

async function onResend(): Promise<void> {
  if (cooldown.value > 0) return;
  try {
    await requestPasswordResetCode(email.value);
    startCooldown();
    showAlertSuccess('A new code was sent. Check your inbox.');
  } catch {
    await showAlertError('Could not send the code. Try again later.');
  }
}

const onReset = handleSubmit(async () => {
  isLoading.value = true;
  errorCode.value = '';
  policyMessages.value = [];
  try {
    await resetPasswordWithCode({
      email: email.value,
      code: code.value,
      newPassword: newPassword.value,
    });
    showAlertSuccess('Password updated. Sign in with your new password.');
    router.push('/login');
  } catch (e) {
    if (isAxiosError(e) && e.response?.status === 400) {
      const data = e.response.data as PasswordResetErrorResponse;
      errorCode.value = data.error ?? 'unknown';
      if (data.error === 'password_policy') policyMessages.value = data.messages ?? [];
    } else {
      errorCode.value = 'unknown';
    }
  } finally {
    isLoading.value = false;
  }
});

onUnmounted(() => {
  if (cooldownIntervalId !== null) clearInterval(cooldownIntervalId);
});
</script>
