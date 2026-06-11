<template>
  <section id="about-form" class="about-form">
    <!-- Re-use existing ContactSection (Get in Touch! header + form card).
         Bg is set via CSS variable to teal-dark. -->
    <ContactSection class="about-form__contact" />
  </section>
</template>

<script setup lang="ts">
import ContactSection from '@/components/landing/shared/ContactSection.vue'
</script>

<style scoped>
/* ============================================================
   About — Form Section (mobile-first)
   Figma node: 2537:889
   Re-uses ContactSection (Get in Touch) with teal-dark bg.
   ============================================================ */

.about-form {
  position: relative;
  width: 100%;
  /* Pass teal-dark down to ContactSection via the CSS variable it reads */
  --contact-bg: var(--c-brand-teal-dark);
}
</style>
