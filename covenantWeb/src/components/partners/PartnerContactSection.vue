<template>
  <section class="partner-contact" data-aos="fade-up">
    <div class="partner-contact__wrapper">
      <div class="partner-contact__circle">
        <div class="partner-contact__card">
          <h2 class="partner-contact__title">Get in Touch!</h2>
          <p class="partner-contact__subtitle">
            Your next hire starts here
          </p>

          <!-- Formulario reutilizable -->
          <ContactForm />
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import ContactForm from '@/components/layout/ContactForm.vue'
</script>

<style scoped>
.partner-contact {
  background: #05162d;
  padding: 80px 0 90px;
}

.partner-contact__wrapper {
  max-width: 1180px;
  margin: 0 auto;
  padding: 0 24px;
  display: flex;
  justify-content: center;
}

/* círculo verde de fondo */
.partner-contact__circle {
  position: relative;
  width: 520px;
  max-width: 100%;
  padding: 70px 40px;
  border-radius: 260px;
  background: radial-gradient(circle at 50% 50%, #32d26a, #28b05a);
  display: flex;
  align-items: center;
  justify-content: center;
}

/* card blanco del form */
.partner-contact__card {
  width: 100%;
  max-width: 420px;
  background: #ffffff;
  border-radius: 18px;
  padding: 28px 24px 24px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.35);
}

.partner-contact__title {
  font-size: 1.3rem;
  font-weight: 700;
  color: #05162d;
  text-align: center;
  margin-bottom: 4px;
}

.partner-contact__subtitle {
  font-size: 0.85rem;
  color: #7a8494;
  text-align: center;
  margin-bottom: 18px;
}

@media (max-width: 720px) {
  .partner-contact__circle {
    padding: 40px 16px;
    border-radius: 40px;
  }
}
</style>
