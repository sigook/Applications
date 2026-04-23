<template>
  <div class="unsubscribe-container">
    <b-loading v-model="isLoading"></b-loading>
    <h3 class="text-center">Would you like to unsubscribe from these emails?</h3>
    <p class="alert-warning-red text-center" v-if="errorMessage" v-html="errorMessage"></p>
    <div>
      <button v-on:click="redirectToHome" type="button" class="background-btn create-btn btn-radius">Cancel
      </button>
      <button v-on:click="onUnsubscribe" type="button" class="background-btn create-btn red-button btn-radius">Yes
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { getErrorMessage } from '@/utils/toast';
import { unsubscribe } from '@/api/sharedApi';

const route = useRoute();
const isLoading = ref(false);
const errorMessage = ref<string | null>(null);

function redirectToHome() {
  window.location.href = '/';
}

async function onUnsubscribe() {
  const typeOfUser = route.query.u;
  const userId = route.query.id;
  const subscriptionType = route.query.t;

  if (!typeOfUser || !userId || !subscriptionType) {
    redirectToHome();
    return;
  }

  const key = `${typeOfUser}${userId}${subscriptionType}`;
  const alreadyUnsubscribe = window.sessionStorage.getItem(key);
  if (alreadyUnsubscribe) {
    redirectToHome();
    return;
  }

  isLoading.value = true;
  try {
    await unsubscribe({ userId: userId as string, typeId: subscriptionType as string });
    isLoading.value = false;
    window.sessionStorage.setItem(key, '1');
    redirectToHome();
  } catch (error) {
    isLoading.value = false;
    errorMessage.value = getErrorMessage(error);
    window.sessionStorage.setItem(key, '1');
  }
}
</script>

<style scoped>
.unsubscribe-container {
  width: 100%;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}
</style>
