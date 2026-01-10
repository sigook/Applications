<template>
  <section class="partner-contact" data-aos="fade-up">
    <div class="partner-contact__wrapper">
      <!-- Círculos de fondo -->
      <div
        class="partner-contact__circle partner-contact__circle--main"
      >
      <!-- Título dentro del círculo verde -->
      <div class="partner-contact__heading">
          <h2 class="partner-contact__title">Get in Touch!</h2>
          <p class="partner-contact__subtitle">
            Your next hire starts here
          </p>
        </div>
      </div>
      <div
        class="partner-contact__circle partner-contact__circle--outline"
      ></div>



      <!-- Card del formulario -->
      <div class="partner-contact__card">
        <ContactForm
          subject="Contact become a partner"
          :use-dynamic-title="true"
          title-suffix="become a partner"
        />
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import ContactForm from '@/components/layout/ContactForm.vue'
</script>

<style scoped>
.partner-contact {
  background: #0F2F44;
  padding: 80px 0 90px;
  overflow: hidden;
}

.partner-contact__wrapper {
  max-width: 1180px;
  margin: 0 auto;
  padding: 80px 24px 120px;
  position: relative;
  display: flex;
  justify-content: center;
}

/* círculos de fondo */
.partner-contact__circle {
  position: absolute;
  border-radius: 50%;
  pointer-events: none;
}

.partner-contact__circle--main {
  width: 860px;
  height: 860px;
  background: #32d26a;
  top: 10px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 0;
}

.partner-contact__circle--outline {
  width: 860px;
  height: 860px;
  border: 2px solid #ffffff;
  top: 0;
  left: 55%;
  transform: translateX(-50%);
  z-index: 0;
}

/* Título dentro del círculo */
.partner-contact__heading {
  position: absolute;
  top: 140px;               /* coloca el texto en la parte alta del círculo */
  left: 50%;
  transform: translateX(-50%);
  text-align: center;
  z-index: 1;
}

.partner-contact__title {
  font-size: 2.5rem;
  font-weight: 700;
  color: #05162d;
  margin: 0 0 6px 0;
}

.partner-contact__subtitle {
  font-size: 1.4rem;
  color: #0f2b3f;
  margin: 0;
}

/* Card blanco del form (debajo del texto, solapando el círculo) */
.partner-contact__card {
  position: relative;
  z-index: 2;
  width: 100%;
  max-width: 420px;
  background: #ffffff;
  border-radius: 18px;
  padding: 38px 34px 34px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.35);
  margin-top: 220px;        /* hace que la card “baje” y se monte sobre el círculo */
}

/* Responsive */
@media (max-width: 768px) {
  .partner-contact__wrapper {
    padding: 60px 16px 100px;
  }

  .partner-contact__circle--main {
    width: 460px;
    height: 460px;
    top: 0;
  }

  .partner-contact__circle--outline {
    width: 480px;
    height: 480px;
    top: -10px;
    left: 54%;
  }

  .partner-contact__heading {
    top: 100px;
  }

  .partner-contact__title {
    font-size: 1.4rem;
  }

  .partner-contact__card {
    margin-top: 170px;
    padding-inline: 18px;
  }
}
</style>
