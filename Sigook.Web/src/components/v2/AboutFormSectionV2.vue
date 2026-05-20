<template>
  <section id="about-form" class="about-form-v2">
    <!-- Re-use existing ContactSectionV2 (Get in Touch! header + form card).
         Bg is set via CSS variable to teal-dark. -->
    <ContactSectionV2 class="about-form-v2__contact" />
  </section>
</template>

<script setup lang="ts">
import ContactSectionV2 from '@/components/v2/ContactSectionV2.vue'
</script>

<style scoped>
/* ============================================================
   About — Form Section (mobile-first)
   Figma node: 2537:889
   Re-uses ContactSectionV2 (Get in Touch) with teal-dark bg.
   ============================================================ */

.about-form-v2 {
  position: relative;
  width: 100%;
  /* Pass teal-dark down to ContactSectionV2 via the CSS variable it reads */
  --contact-v2-bg: var(--c-brand-teal-dark);
}
</style>
