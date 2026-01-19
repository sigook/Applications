<template>
  <section class="hc-section" id="contact" data-aos="fade-up">
    <!-- BACKGROUND LAYERS FOR SMOOTH TRANSITION -->
    <div class="hc-bg-container">
      <div class="hc-bg-layer layer-1" :class="{ 'bg-active': currentIndex === 0 }"></div>
      <div class="hc-bg-layer layer-2" :class="{ 'bg-active': currentIndex === 1 }"></div>
    </div>

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

          <div class="hc-circle__form-placeholder" id="contact-form">
            <ContactForm
              title="CONTACT FORM ~ NOTIFICATION"
              subject="Contact From Covenant"
            />
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
      text: 'I recommend Covenant Group Ltd. as an exceptional and reliable employment agency. Our company has been partnering with them since July 2020, and their service has been consistently outstanding.',
      author: 'HR Manager, manufacturer',
      role: 'Mississauga, Ontario'
    },
    {
      text: 'Our organization has been working continuously with Covenant Group Ltd. since 2022...Their services are competitively priced, and billing has always been accurate and transparent.',
      author: 'Operations Manager, 4PL Company',
      role: 'Ottawa, Ontario'
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
    padding: 80px 0 250px;
    color: #ffffff;
    background-color: #0F2F44; /* Base color */
    border-radius: 0 200px 0 0;
  }

  /* BACKGROUND CONTAINER & LAYERS */
  .hc-bg-container {
    position: absolute;
    inset: 0;
    width: 100%;
    height: 100%;
    z-index: 0;
    border-radius: 0 200px 0 0; 
    overflow: hidden; 
  }

  .hc-bg-layer {
    position: absolute;
    inset: 0;
    width: 100%;
    height: 100%;
    background-position: center top;
    background-size: 100% auto;
    background-repeat: no-repeat;
    opacity: 0;
    transition: opacity 0.8s ease-in-out;
  }

  .hc-bg-layer.bg-active {
    opacity: 1;
  }

  /* Layer 1 Styles (Desktop) */
  .layer-1 {
    background-image:
      linear-gradient(
        to bottom,
        rgba(4, 28, 44, 0.45) 0%,
        #0F2F44 60%,
        #0F2F44 100%
      ),
      url("@/assets/images/home-contact-bg.png");
  }

  /* Layer 2 Styles (Desktop) */
  .layer-2 {
    background-image:
      linear-gradient(
        to bottom,
        rgba(4, 28, 44, 0.45) 0%,
        #0F2F44 60%,
        #0F2F44 100%
      ),
      url("@/assets/images/home-contact-bg-2.png");
  }

  /* Contenedor general para alinear título y contenido */
  .hc-container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 40px;
    position: relative;
    z-index: 2; /* Content above background */
  }

  /* ================== 3. COMILLAS GIGANTES ================== */
  .hc-decor-quote {
    position: absolute;
    top: -20px; 
    right: 80px; 
    font-family: serif; 
    font-size: 10rem;
    line-height: 1;
    color: #000000; 
    opacity: 1;
    pointer-events: none;
    z-index: 1000;
    font-weight: bold;
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
    background-color: rgba(255, 255, 255, 0.5);
    border-radius: 2px;
  }

  /* ================== HERO TESTIMONIALS ================== */

  .hc-hero {
    width: 100%;
    display: flex;
    justify-content: center;
    margin-bottom: 80px;
    position: relative;
    z-index: 2;
  }

  .hc-hero__content {
    text-align: center;
    max-width: 600px;
    width: 100%;
  }

  .hc-hero__card {
    border-radius: 18px;
    padding: 0 20px;

    width: 100%;
    height: 300px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 30px;
  }

  .hc-hero__quote {
    font-size: 1.1rem;
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
    background: rgba(255,255,255,0.2);
    padding: 6px 12px;
    border-radius: 20px;
  }

  .hc-hero__dot {
    width: 10px;
    height: 10px;
    border-radius: 999px;
    border: none;
    background: #ffffff;
    opacity: 0.5;
    cursor: pointer;
    padding: 0;
    transition: all 0.2s ease;
  }

  .hc-hero__dot--active {
    width: 10px;
    background: #0F2F44;
    opacity: 1;
  }

  /* ================== CÍRCULO + FORM ================== */

  .hc-circle-area {
    width: 100%;
    display: flex;
    justify-content: center;
    padding: 0 20px;
    position: relative;
    z-index: 5;
  }

  .hc-contact-circle {
    position: relative;
    width: 920px;
    height: 920px;
    border-radius: 50%;
    background: #45d86e;
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 2;
  }

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
    padding-top: 60px;
    box-sizing: border-box;
  }

  .hc-circle__title {
    color: #04141f;
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
    max-width: 620px;
    position: relative;
    top: 40px;
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
    background: #45d86e;
    color: #fff;
    font-weight: 700;
  }

  /* ================== RESPONSIVE ================== */

  @media (max-width: 768px) {
    .hc-section {
      padding-bottom: 500px;
      /* El fondo se maneja en .hc-bg-layer */
      border-radius: 0 150px 0 0; /* Radio de borde reducido en móviles */
    }

    /* Ajustar el radio de borde del contenedor de fondo para coincidir */
    .hc-bg-container {
      border-radius: 0 150px 0 0;
    }

    /* Adjust layers for mobile */
    .hc-bg-layer {
      background-size: 100% auto;
      background-position: top center;
    }
    
    .layer-1 {
      background-image: url("@/assets/images/home-contact-bg-movile.png");
    }

    .layer-2 {
      background-image: url("@/assets/images/home-contact-bg-movile-2.png");
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
      top: -20px;
      right: 0px;
    }

    .hc-contact-circle {
      width: 460px;
      height: 460px;
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
      padding-bottom: 450px;
    }

    /* Ajuste título "What Our Clients Say" más pequeño y arriba */
    .hc-title-wrapper {
      margin-top: -40px; 
      margin-bottom: 80px;
    }
    
    .hc-section-title {
      font-size: 1.5rem; 
    }

    .hc-decor-quote{
      font-size: 6rem;
      top: -10px;
      right: 20px;
    }

    /* CÍRCULO: Forzar redondez */
    .hc-contact-circle {
      width: 600px;
      min-width: 600px; /* Evita que se aplaste */
      height: 600px;
      flex-shrink: 0;
    }
    .hc-contact-circle::before {
       transform: translateX(20px);
    }

    .hc-circle__title {
       font-size: 1.5rem;
       margin-top: 20px;
    }

    .hc-circle__form-placeholder {
      max-width: 90%;
      width: 360px;
    }
  }
</style>
