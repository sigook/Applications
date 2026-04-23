<template>
  <div class="apply-container">
    <b-loading v-model="isLoading"></b-loading>
    <p class="alert-warning-red text-center" v-if="errorMessage" v-html="errorMessage"></p>
    <p class="alert-success text-center" v-if="successMessage">
      {{ successMessage }}
    </p>
    <div>
      <button v-on:click="redirectToHome" type="button" class="background-btn create-btn primary-button btn-radius">
        OK
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { getErrorMessage } from '@/utils/toast';
import { workerRequestApply } from '@/api/workerApi';

const route = useRoute();

const isLoading = ref(false);
const errorMessage = ref<string | null>(null);
const successMessage = ref<string | null>(null);
const defaultSuccessMessage = 'Thank you, one of our recruiters will contact you soon.';

function redirectToHome() {
  window.location.href = '/';
}

function apply() {
  const workerId = route.query.w;
  const requestId = route.query.r;
  if (!workerId || !requestId) {
    redirectToHome();
    return;
  }

  const key = `${workerId}${requestId}`;
  const alreadyApplied = window.sessionStorage.getItem(key);
  if (alreadyApplied) {
    successMessage.value = defaultSuccessMessage;
    return;
  }

  isLoading.value = true;
  workerRequestApply(workerId as string, requestId as string, {})
    .then(() => {
      isLoading.value = false;
      successMessage.value = defaultSuccessMessage;
      window.sessionStorage.setItem(key, '1');
    })
    .catch((error: any) => {
      isLoading.value = false;
      errorMessage.value = getErrorMessage(error);
      window.sessionStorage.setItem(key, '1');
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
