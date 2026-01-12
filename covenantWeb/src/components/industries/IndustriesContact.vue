<template>
  <section class="ind-contact" data-aos="fade-up">
    <div class="ind-contact__wrapper">
      <!-- Bloque azul + círculo verde -->
      <div class="ind-contact__circle">
        <!-- Texto grande dentro del círculo -->
        <div class="ind-contact__headline">
          <h2 class="ind-contact__title">Get in Touch!</h2>
          <p class="ind-contact__subtitle">Your next hire starts here</p>
        </div>

        <!-- Card blanco con el formulario reutilizable -->
        <div class="ind-contact__card">
          <ContactForm
            title="REQUEST PERSONNEL FORM ~ NOTIFICATION"
            subject="Contact Request Staff"
          />
        </div>

        <div class="ind-contact__waves">
          <img
            src="@/assets/images/contact-waves.png"
            alt=""
            aria-hidden="true"
          />
        </div>

      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import ContactForm from '@/components/layout/ContactForm.vue'
</script>

<style scoped>
.ind-contact {
  background: #e6e6e8; /* gris claro como en la captura */
  padding: 0;
  margin: 0;
  box-sizing: border-box;
}

/* bloque azul oscuro donde vive el círculo verde */
.ind-contact__circle {
  position: relative;
  background: #0F2F44;
  padding:  40px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
}

/* círculo verde grande */
.ind-contact__circle::before {
  content: '';
  position: absolute;
  width: 720px;
  height: 720px;
  border-radius: 50%;
  background: #35d46b;
  top: -50px;
  left: 50%;
  transform: translateX(-50%);
}

/* --- NUEVO: CÍRCULO BLANCO (OUTLINE) --- */
/* Usamos ::after para crear el borde sin tocar el HTML */
.ind-contact__circle::after {
  content: '';
  position: absolute;
  width: 720px; /* Mismo tamaño que el verde */
  height: 720px;
  border-radius: 50%;
  border: 2px solid #ffffff; /* Borde blanco */

  /* Posicionamiento para el efecto "desfasado" */
  top: -65px;  /* Un poco más arriba que el verde (-50px) */
  left: 52%;   /* Un poco a la derecha del centro (50%) */
  transform: translateX(-50%);

  z-index:1.2; /* Detrás del círculo verde si quieres, o -1 si quieres que se crucen */
  pointer-events: none;
}

/* headline dentro del círculo (Get in Touch!) */
.ind-contact__headline {
  position: relative;
  z-index: 1;
  text-align: center;
  margin-bottom: 26px;
}

.ind-contact__title {
  font-size: 1.9rem;
  font-weight: 700;
  color: black;
  margin-bottom: 4px;
}

.ind-contact__subtitle {
  font-size: 0.95rem;
  color: black;
}

/* card blanco que contiene tu ContactForm */
.ind-contact__card {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 520px;
  background: #ffffff;
  border-radius: 18px;
  padding: 30px 26px 26px;
  box-shadow: 0 28px 60px rgba(0, 0, 0, 0.35);
}


/* === waves decorativas derechas === */
.ind-contact__waves {
  position: absolute;
  right: 0;
  top: -250px;
  z-index: 1;
  pointer-events: none;
}

.ind-contact__waves img {
  height: 860px;
  width: auto;
}

/* responsive */
@media (max-width: 768px) {
  .ind-contact__circle {
    padding-inline: 16px;
    padding-top: 110px;
  }

  .ind-contact__circle::before {
    width: 420px;
    height: 420px;
    top: 0px;
  }
  .ind-contact__circle::after {
    width: 420px;
    height: 420px;
    top: 0px;
  }

  .ind-contact__title {
    font-size: 1.6rem;
  }

  /* ocultar waves en pantallas pequeñas si molestan */
  .ind-contact__waves {
    display: none;
  }
}

@media (max-width: 480px) {
  .ind-contact__circle::before {
    width: 360px;
    height: 360px;
    top: 0px;
  }

  .ind-contact__circle::after {
    width: 360px;
    height: 360px;
    top: 0px;
  }
}
</style>
