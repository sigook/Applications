<template>
  <div class="coming">
    <div class="coming__card">
      <p class="coming__label">{{ pageTitle }}</p>
      <h1 class="coming__title">This page is coming soon</h1>
      <p class="coming__sub">We're working hard to bring this to you. Check back soon.</p>
      <router-link to="/" class="btn btn--primary btn--md">Back to Home</router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const pageTitle = computed(() => (route.meta.title as string) ?? '');
</script>

<style scoped>
.coming {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f4f7fb;
  padding: 40px 24px;
}

.coming__card {
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
  max-width: 480px;
}

.coming__label {
  font-family: var(--font-family, 'Poppins', sans-serif);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 1.5px;
  text-transform: uppercase;
  color: var(--c-brand-blue, #2c5eba);
  margin: 0;
}

.coming__title {
  font-family: var(--font-family, 'Poppins', sans-serif);
  font-size: 36px;
  font-weight: 700;
  line-height: 1.15;
  color: var(--c-ink, #111827);
  margin: 0;
}

.coming__sub {
  font-family: var(--font-family, 'Poppins', sans-serif);
  font-size: 15px;
  font-weight: 400;
  color: var(--c-ink-2, #6b7280);
  margin: 0 0 8px;
  line-height: 1.6;
}
</style>
