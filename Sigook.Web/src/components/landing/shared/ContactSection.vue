<template>
  <section class="contact">
    <div class="contact__inner">
      <h1 class="contact__title">Get in Touch!</h1>
      <p class="contact__lead">Your next hire starts here</p>

      <GetInTouchForm />
    </div>
  </section>
</template>

<script setup lang="ts">
import GetInTouchForm from '@/components/landing/shared/GetInTouchForm.vue'
</script>

<style scoped>
.contact {
  position: relative;
  border-radius: 0 0 20px 20px;
  overflow: hidden;
  font-family: var(--font-family);
  color: #fff;
  margin-top: -200px;
  padding-top: 200px;
  z-index: 2;
}

.contact__inner {
  position: relative;
  z-index: 1;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 90px var(--gutter-desktop) 100px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.contact__title {
  color: #fff;
  font-family: var(--font-family);
  font-size: 48px;
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.015em;
  text-align: center;
  text-transform: none;
  margin: 0 0 12px;
}

.contact__lead {
  color: rgba(255, 255, 255, 0.85);
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 500;
  line-height: 1.5;
  text-align: center;
  margin: 0 0 40px;
}

@media (max-width: 1023px) {
  .contact {
    min-height: unset;
    margin-top: -120px;
    padding-top: 120px;
  }

  .contact__inner {
    padding: 64px var(--gutter-mobile) 72px;
  }

  .contact__title {
    font-size: 32px;
  }

  .contact__lead {
    font-size: 15px;
    margin-bottom: 32px;
  }
}
</style>
