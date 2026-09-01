<template>
  <div class="apply-container">
    <b-loading v-model="isLoading"></b-loading>
    <p class="alert-warning-red has-text-centered" v-if="errorMessage" v-html="errorMessage"></p>
    <p class="alert-success has-text-centered" v-if="successMessage">
      {{ successMessage }}
    </p>
    <div>
      <b-button type="is-primary" rounded @click="redirectToHome">OK</b-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute, type LocationQueryValue } from 'vue-router';
import { getErrorMessage } from '@/utils/toast';
import { workerRequestApply } from '@/api/workerApi';
import { requestApplyByEmail } from '@/api/websiteApi';

const route = useRoute();

const isLoading = ref(false);
const errorMessage = ref<string | null>(null);
const successMessage = ref<string | null>(null);
const defaultSuccessMessage = 'Thank you, one of our recruiters will contact you soon.';
const defaultErrorMessage = 'We could not process your application, please contact the agency.';

function redirectToHome() {
  window.location.href = '/';
}

function firstParam(value: LocationQueryValue | LocationQueryValue[]): string | null {
  const single = Array.isArray(value) ? value[0] : value;
  return typeof single === 'string' && single ? single : null;
}

function apply() {
  const numberIdParam = firstParam(route.query.n);
  const email = firstParam(route.query.e);
  const workerId = firstParam(route.query.w);
  const requestId = firstParam(route.query.r);

  const numberId = numberIdParam ? Number(numberIdParam) : NaN;
  const byEmail = Number.isInteger(numberId) && numberId > 0 && !!email;
  if (!byEmail && (!workerId || !requestId)) {
    redirectToHome();
    return;
  }

  const key = byEmail ? `${numberId}|${email}` : `${workerId}${requestId}`;
  const alreadyApplied = window.sessionStorage.getItem(key);
  if (alreadyApplied) {
    successMessage.value = defaultSuccessMessage;
    return;
  }

  isLoading.value = true;
  const applyRequest = byEmail
    ? requestApplyByEmail(numberId, email as string)
    : workerRequestApply(workerId as string, requestId as string, {});
  applyRequest
    .then(() => {
      isLoading.value = false;
      successMessage.value = defaultSuccessMessage;
      window.sessionStorage.setItem(key, '1');
    })
    .catch(async (error: unknown) => {
      isLoading.value = false;
      errorMessage.value = (await getErrorMessage(error)) || defaultErrorMessage;
    });
}

apply();
</script>

<style scoped>
.apply-container {
  width: 100%;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}
</style>
