<template>
  <section class="industries-cta">
    <div class="container industries-cta__content">
      <h2 class="industries-cta__title">Industries</h2>
      <p class="industries-cta__text">
        We proudly serve a wide range of industries, including corporate, professional, manufacturing, and logistics sectors. Our tailored recruitment solutions ensure that businesses across diverse fields find the right talent to achieve lasting success.
      </p>
      <button class="industries-cta__btn" @click="$emit('scrollToContact')">
        Contact Us
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"></line><polyline points="12 5 19 12 12 19"></polyline></svg>
      </button>
    </div>
  </section>
</template>

<script setup lang="ts">
defineEmits(['scrollToContact'])
</script>

<style scoped>
.industries-cta {
  background-color: #cccccc; /* Gris de la imagen 1 */
  padding: 120px 20px;
  text-align: center;
  color: #333;
  /* Curva inferior */
  border-bottom-right-radius: 160px;
  position: relative;
  z-index: 1; /* Por encima del siguiente componente */
}

.industries-cta__content {
  max-width: 700px;
  margin: 0 auto;
}

.industries-cta__title {
  color: #ffffff;
  font-size: 2.2rem;
  font-weight: 700;
  margin-bottom: 20px;
  text-align: left;
}

.industries-cta__text {
  color: #ffffff;
  line-height: 1.6;
  margin-bottom: 35px;
  font-size: 1.1rem;
  text-align: left;
}

.industries-cta__btn {
  background: #32d26a; /* Verde corporativo */
  color: #333;
  border: none;
  padding: 14px 45px;
  border-radius: 999px;
  font-weight: 700;
  font-size: 1rem;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: background-color 0.3s ease, transform 0.2s ease;
  box-shadow: 0 10px 20px rgba(50, 210, 106, 0.3);
}

.industries-cta__btn:hover {
  background: #28a755;
  transform: translateY(-3px);
}
</style>
