<template>
  <section class="contact-v2">
    <!-- Decorative circles -->
    <div class="contact-v2__deco-1" aria-hidden="true">
      <svg viewBox="0 0 851 851" xmlns="http://www.w3.org/2000/svg" class="contact-v2__deco-svg">
        <circle cx="425.5" cy="425.5" r="425.5" fill="#00ADEF" fill-opacity="0.35" />
      </svg>
    </div>
    <div class="contact-v2__deco-2" aria-hidden="true">
      <svg viewBox="0 0 848 848" xmlns="http://www.w3.org/2000/svg" class="contact-v2__deco-svg">
        <circle cx="424" cy="424" r="423" stroke="white" stroke-width="2" fill="none" stroke-opacity="0.4" />
      </svg>
    </div>

    <div class="contact-v2__inner">
      <h1 class="contact-v2__title">Get in Touch!</h1>
      <p class="contact-v2__lead">Your next hire starts here</p>

      <GetInTouchFormV2 />
    </div>
  </section>
</template>

<script setup lang="ts">
import GetInTouchFormV2 from '@/components/v2/GetInTouchFormV2.vue'
</script>

<style scoped>
/* ── Section shell ─────────────────────────────────────────────────────────── */
.contact-v2 {
  position: relative;
  background: var(--c-brand-blue);
  border-radius: 0 0 20px 20px;
  min-height: 1500px;
}

/* ── Decorative circles ────────────────────────────────────────────────────── */
.contact-v2__deco-1 {
  position: absolute;
  top: 40px;
  left: 50%;
  transform: translateX(-50%);
  width: 851px;
  height: 851px;
  pointer-events: none;
  z-index: 0;
}

.contact-v2__deco-2 {
  position: absolute;
  top: 30px;
  left: 50%;
  transform: translateX(-50%);
  width: 848px;
  height: 848px;
  pointer-events: none;
  z-index: 0;
}

.contact-v2__deco-svg {
  width: 100%;
  height: 100%;
}

/* ── Inner container ───────────────────────────────────────────────────────── */
.contact-v2__inner {
  position: relative;
  z-index: 1;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 80px var(--gutter-desktop) 80px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

/* ── Heading ───────────────────────────────────────────────────────────────── */
.contact-v2__title {
  color: var(--c-bg);
  font-size: 40px;
  font-weight: 700;
  line-height: 1.1;
  text-align: center;
  text-transform: none;
  margin: 0 0 8px;
}

.contact-v2__lead {
  color: var(--c-bg);
  font-size: 18px;
  font-weight: 500;
  line-height: 1.5;
  text-align: center;
  margin: 0 0 32px;
  opacity: 0.9;
}

/* ── Mobile ────────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .contact-v2 {
    min-height: unset; /* reset 1500px — let content drive height on mobile */
  }

  .contact-v2__inner {
    padding: 60px var(--gutter-mobile) 60px;
  }

  .contact-v2__deco-1,
  .contact-v2__deco-2 {
    width: 500px;
    height: 500px;
    left: -100px;
  }

  .contact-v2__title {
    font-size: 30px;
  }
}
</style>
