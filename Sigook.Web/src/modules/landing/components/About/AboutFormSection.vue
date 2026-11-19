<template>
  <section id="about-form" class="about-form">
    <ContactSection class="about-form__contact" />

    <div class="about-form__connect">
      <span class="about-form__connect-label">Connect with us</span>
      <SocialLinks />
    </div>
  </section>
</template>

<script setup lang="ts">
import ContactSection from '@/modules/landing/components/shared/sections/ContactSection.vue'
import SocialLinks from '@/modules/landing/components/shared/ui/SocialLinks.vue'
</script>

<style scoped>
.about-form {
  position: relative;
  width: 100%;
  --contact-bg: var(--c-brand-teal-dark);
}

.about-form__connect {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
  padding: 0 24px clamp(64px, 9vw, 104px);
  text-align: center;
}

.about-form__connect-label {
  font-family: var(--font-family);
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
}
</style>
