<template>
  <section class="contact-cta" data-aos="fade-up">
    <!-- CÍRCULO VERDE CON TEXTO -->
    <div class="contact-cta__circle">
      <h2 class="contact-cta__title">Get in Touch!</h2>
      <p class="contact-cta__subtitle">Your next hire starts here</p>
    </div>

    <!-- BANDA AZUL + TARJETA DEL FORMULARIO -->
    <div class="contact-cta__blue">
      <div class="contact-cta__card">
        <ContactForm />
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
  import ContactForm from '@/components/layout/ContactForm.vue'
</script>

<style scoped>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

.contact-cta {
  position: relative;
  width: 100%;
  background-color: #ffffff;
  padding: 160px 0 0;
  overflow: visible;
}

/* CÍRCULO VERDE CENTRADO HORIZONTAL Y COMPLETO */
.contact-cta__circle {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
  top: 40px;               /* controla cuánto sube el círculo */
  width: 720px;
  height: 720px;
  border-radius: 50%;
  background-color: #32d26a;
  box-shadow: 0 0 0 10px rgba(255, 255, 255, 0.15);
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 40px 40px 80px; /* mueve el texto un poco hacia arriba */
  z-index: 10;
}

/* Texto dentro del círculo */
.contact-cta__title {
  font-size: 2.5rem;
  font-weight: 700;
  color: #171717;
  margin-bottom: 8px;
  margin-top: 3rem;
}

.contact-cta__subtitle {
  font-size: 1.1rem;
  color: #0b3b28;
}

/* BANDA AZUL DE FONDO */
.contact-cta__blue {
  position: relative;
  background-color: #020d1e;
  padding: 260px 0 140px;  /* deja espacio para que el círculo se meta dentro */
  margin-top: 180px;       /* baja la banda azul para que el círculo sobresalga por arriba */
}

/* TARJETA BLANCA DEL FORMULARIO */
.contact-cta__card {
  position: relative;
  z-index: 20;
  width: 100%;
  max-width: 520px;
  margin: 0 auto;
  margin-top: -300px;      /* hace que la tarjeta se sobreponga al círculo */
  background-color: #ffffff;
  border-radius: 18px;
  box-shadow: 0 26px 60px rgba(0, 0, 0, 0.35);
  padding: 32px 32px 40px;
}

/* Placeholder del formulario (lo reemplazas luego) */
.contact-cta__form-placeholder {
  border-radius: 12px;
  border: 1px dashed #d0d0d0;
  padding: 40px 20px;
  text-align: center;
}

.contact-cta__placeholder-text {
  font-size: 0.95rem;
  color: #777777;
}

/* ========= RESPONSIVE ========= */

@media (max-width: 900px) {
  .contact-cta {
    padding: 160px 0 0;
  }

  .contact-cta__circle {
    width: 440px;
    height: 440px;
    top: 140px;
    padding: 32px 28px 64px;
  }

  .contact-cta__title {
    font-size: 1.8rem;
  }

  .contact-cta__subtitle {
    font-size: 0.95rem;
  }

  .contact-cta__blue {
    margin-top: 170px;
    padding: 230px 0 120px;
  }

  .contact-cta__card {
    max-width: 460px;
    margin-top: -180px;
    padding: 28px 24px 36px;
  }
}

@media (max-width: 600px) {
  .contact-cta {
    padding: 130px 0 0;
  }

  .contact-cta__circle {
    width: 360px;
    height: 360px;
    top: 140px;
    padding: 28px 22px 52px;
  }

  .contact-cta__title {
    font-size: 1.6rem;
  }

  .contact-cta__subtitle {
    font-size: 0.9rem;
  }

  .contact-cta__blue {
    margin-top: 150px;
    padding: 210px 0 110px;
  }

  .contact-cta__card {
    max-width: 100%;
    margin-top: -170px;
    padding: 24px 18px 32px;
  }
}
</style>

