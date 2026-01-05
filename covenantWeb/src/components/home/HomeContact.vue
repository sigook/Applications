<template>
  <section class="hc-section" id="contact" data-aos="fade-up">
    <div class="hc-decor-quote">”</div>

    <div class="hc-container">

      <div class="hc-title-wrapper">
        <h2 class="hc-section-title">
          What Our<br />
          Clients Say
        </h2>
        <div class="hc-title-line"></div>
      </div>

      <div class="hc-hero">
        <div class="hc-hero__content">
          <div class="hc-hero__card">
            <p class="hc-hero__quote">
              “{{ currentTestimonial.text }}”
            </p>
            <p class="hc-hero__author">
              {{ currentTestimonial.author }}
            </p>
            <p class="hc-hero__role">
              {{ currentTestimonial.role }}
            </p>

            <div class="hc-hero__dots">
              <button
                v-for="(t, index) in testimonials"
                :key="index"
                class="hc-hero__dot"
                :class="{ 'hc-hero__dot--active': index === currentIndex }"
                @click="goTo(index)"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="hc-circle-area">
      <div class="hc-contact-circle">
        <div class="hc-circle__content">
          <h2 class="hc-circle__title">Get in Touch!</h2>
          <p class="hc-circle__subtitle">
            Your next hire starts here
          </p>

          <div class="hc-circle__form-placeholder">
            <ContactForm />
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
  import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
  import ContactForm from '@/components/layout/ContactForm.vue'

  interface Testimonial {
    text: string
    author: string
    role: string
  }

  const testimonials = ref<Testimonial[]>([
    {
      text: 'Thank you so much for your assistance. Wouldn’t have done it without you!',
      author: 'Business Owner, Flower Boutique',
      role: 'Ontario, Canada'
    },
    {
      text: 'They quickly understood our needs and found the right talent for our projects.',
      author: 'HR Manager, Tech Company',
      role: 'Toronto, Canada'
    },
    {
      text: 'Reliable, professional and always available when we need them.',
      author: 'Operations Director, Logistics Firm',
      role: 'Vancouver, Canada'
    }
  ])

  const currentIndex = ref<number>(0)
  const intervalId = ref<ReturnType<typeof setInterval> | null>(null)

  const currentTestimonial = computed(
    () => testimonials.value[currentIndex.value]
  )

  const goTo = (index: number): void => {
    currentIndex.value = index
  }

  const next = (): void => {
    currentIndex.value = (currentIndex.value + 1) % testimonials.value.length
  }

  onMounted((): void => {
    intervalId.value = setInterval(next, 6000)
  })

  onBeforeUnmount((): void => {
    if (intervalId.value) {
      clearInterval(intervalId.value)
    }
  })
</script>

<style scoped>
  .hc-section {
    position: relative;
    width: 100%;
    padding: 80px 0 400px;
    color: #ffffff;
    overflow: hidden;

    background:
      linear-gradient(
        to bottom,
        rgba(4, 28, 44, 0.45) 0%, /* Un poco más oscuro para leer las letras blancas */
        #0F2F44 60%,
        #0F2F44 100%
      ),
      url("@/assets/images/home-contact-bg.png");

    background-position: center top; /* Enfoca las caras */
    background-size: 100% auto;
    background-repeat: no-repeat;
  }

  /* Contenedor general para alinear título y contenido */
  .hc-container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 40px;
    position: relative;
  }

  /* ================== 3. COMILLAS GIGANTES ================== */
  .hc-decor-quote {
    position: absolute;
    top: -60px; /* Sube para que sobresalga */
    right: 80px; /* Pegado a la derecha */
    font-family: sans-serif; /* O una fuente serif si prefieres */
    font-size: 20rem; /* Gigante */
    line-height: 1;
    color: black; /* Verde */
    opacity: 1;
    pointer-events: none;
    z-index: 1;
  }

  /* ================== 1. TÍTULO IZQUIERDA ================== */
  .hc-title-wrapper {
    margin-bottom: 40px;
    text-align: left;
    position: relative;
    z-index: 2;
    margin-top: 40px;
  }

  .hc-section-title {
    font-size: 2.5rem;
    font-weight: 700;
    line-height: 1.2;
    margin-bottom: 12px;
  }

  .hc-title-line {
    width: 60px;
    height: 4px;
    background-color: rgba(255, 255, 255, 0.5); /* La línea pequeña debajo */
    border-radius: 2px;
  }

  /* ================== HERO TESTIMONIALS ================== */

  .hc-hero {
    width: 100%;
    display: flex;
    justify-content: center; /* Mantiene la tarjeta centrada */
    margin-bottom: 80px;
    position: relative;
    z-index: 2;
  }

  .hc-hero__content {
    text-align: center;
    max-width: 600px; /* Un poco más ancho */
    width: 100%;
  }

  .hc-hero__card {
    border-radius: 18px;
    padding: 0 20px;

    width: 100%;
    min-height: 150px;
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .hc-hero__quote {
    font-size: 1.1rem; /* Texto un poco más grande */
    font-weight: 500;
    line-height: 1.6;
    margin: 0 0 20px;
    text-align: center;
  }

  .hc-hero__author {
    font-weight: 800;
    font-size: 0.95rem;
    margin: 0 0 2px;
  }

  .hc-hero__role {
    font-size: 0.8rem;
    opacity: 0.8;
    margin: 0 0 20px;
  }

  /* dots */
  .hc-hero__dots {
    display: flex;
    justify-content: center;
    gap: 8px;
    margin-top: auto;
    background: rgba(255,255,255,0.2); /* Fondo suave para los dots */
    padding: 6px 12px;
    border-radius: 20px;
  }

  .hc-hero__dot {
    width: 10px;
    height: 10px;
    border-radius: 999px;
    border: none;
    background: #ffffff; /* Blanco inactivo */
    opacity: 0.5;
    cursor: pointer;
    padding: 0;
    transition: all 0.2s ease;
  }

  .hc-hero__dot--active {
    width: 10px; /* Mismo tamaño */
    background: #ffffff;
    opacity: 1; /* Blanco activo total */
  }

  /* ================== CÍRCULO + FORM ================== */

  .hc-circle-area {
    width: 100%;
    display: flex;
    justify-content: center;
    padding: 0 20px;
    position: relative;
    /* Aseguramos que esté por encima del fondo */
    z-index: 5;
  }

  /* NUEVO: El círculo principal verde */
  .hc-contact-circle {
    position: relative; /* Necesario para que el ::before se posicione respecto a este */
    width: 780px;       /* Tamaño fijo del círculo verde */
    height: 780px;
    border-radius: 50%;
    background: #45d86e; /* Verde brillante */
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 2; /* Asegura que el verde esté delante del blanco */
  }

  /* NUEVO: El borde blanco desplazado usando pseudo-elemento */
  .hc-contact-circle::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    border-radius: 50%;
    border: 2px solid rgba(255, 255, 255, 0.6);
    transform: translateX(50px);

    z-index: -1;
    pointer-events: none;
  }


  .hc-circle__content {
    width: 80%;
    text-align: center;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: flex-start;
    padding-top: 60px; /* Más espacio arriba */
    box-sizing: border-box;
  }

  /* título dentro del círculo */
  .hc-circle__title {
    color: #04141f; /* Texto oscuro */
    margin: 0 0 4px;
    font-size: 2.2rem;
    font-weight: 700;
  }

  .hc-circle__subtitle {
    color: #04141f;
    margin: 0;
    font-size: 1rem;
    opacity: 0.8;
    font-weight: 500;
  }

  .hc-circle__form-placeholder {
    background: #ffffff;
    border-radius: 20px;
    padding: 22px 20px 26px;
    color: #04141f;
    font-size: 0.85rem;
    box-shadow: 0 18px 36px rgba(0, 0, 0, 0.25);
    margin-top: 20px;
    width: 100%;
    max-width: 400px;
    /* Posicionamiento para que se salga un poco del círculo abajo */
    position: relative;
    top: 20px;
  }

  /* ============ AJUSTES ContactForm ============ */

  .hc-circle__form-placeholder :deep(.contact-form) {
    width: 100%;
  }
  .hc-circle__form-placeholder :deep(.contact-form__header) {
    margin-bottom: 18px;
  }
  .hc-circle__form-placeholder :deep(.contact-form__input) {
    padding: 10px 14px;
    border: 1px solid #eee;
    background: #fdfdfd;
  }
  .hc-circle__form-placeholder :deep(.contact-form__submit) {
    background: #45d86e; /* Botón verde */
    color: #fff;
    font-weight: 700;
  }

  /* ================== RESPONSIVE ================== */

  @media (max-width: 768px) {
    .hc-section {
      background: url("@/assets/images/home-contact-bg-movile.png");
      padding-bottom: 400px;
      background-size: 100% auto;
      background-position: top center;
      background-color: #0F2F44;
      background-repeat: no-repeat;
    }

    .hc-container {
      padding: 0 20px;
    }

    .hc-title-wrapper {
      text-align: left;
      margin-top: 60px;
    }

    .hc-title-line {
      margin: 0;
    }

    .hc-decor-quote {
      font-size: 8rem;
      top: -10px;
      right: 0px;
    }

    .hc-contact-circle {
      width: 450px;
      height: 450px;
    }

    .hc-contact-circle::before {
      transform: translateX(30px);
    }

    .hc-circle__title {
      font-size: 1.8rem;
    }

    .hc-circle__form-placeholder {
      padding: 20px 16px 22px;
      top: 10px;
      max-width: 340px;
    }
  }

  @media (max-width: 480px) {
    .hc-section{
      padding-bottom: 600px;
    }

    .hc-decor-quote{
      right: 40px;
    }

    .hc-contact-circle {
      width: 460px;
      height: 460px;
    }
    .hc-contact-circle::before {
       transform: translateX(20px);
    }

    .hc-circle__title {
       font-size: 1.5rem;
    }

    .hc-circle__form-placeholder {
      max-width: 300px;
    }
  }
</style>
