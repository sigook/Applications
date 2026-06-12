<template>
  <section id="about-form" class="about-form">
    <!-- Re-use existing ContactSection (Get in Touch! header + form card).
         Bg is set via CSS variable to teal-dark. -->
    <ContactSection class="about-form__contact" />

    <!-- Closing "connect" strip — follow Sigook on social. -->
    <div class="about-form__connect">
      <span class="about-form__connect-label">Connect with us</span>
      <SocialLinks />
    </div>
  </section>
</template>

<script setup lang="ts">
import ContactSection from '@/components/landing/shared/ContactSection.vue'
import SocialLinks from '@/components/landing/shared/SocialLinks.vue'
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

/* ── Closing connect strip ──────────────────────────────────────────────── */
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
