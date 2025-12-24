<template>
  <section class="hc-section" id="contact" data-aos="fade-up">
    <!-- ================== TESTIMONIALS / HERO ================== -->
    <div class="hc-hero">
      <div class="hc-hero__content">
        <p class="hc-hero__label">What Our Clients Say</p>

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

          <!-- bullets -->
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

    <!-- ================== CÍRCULO + FORMULARIO ================== -->
    <div class="hc-circle-area">
      <div class="hc-circle hc-circle--outer">
        <div class="hc-circle hc-circle--inner">
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
  padding: 80px 0 190px;
  color: #ffffff;
  overflow: hidden;

  /* Fondo = imagen + overlay azul en una sola declaración */
  background:
    linear-gradient(
      to bottom,
      rgba(4, 28, 44, 0.25) 0%,
      #041c2c 55%,
      #041c2c 100%
    ),
    url("@/assets/images/home-contact-bg.jpg");
  background-position: center top;
  background-size: cover;
  background-repeat: no-repeat;
}

/* ================== HERO TESTIMONIALS ================== */

.hc-hero {
  width: 100%;
  display: flex;
  justify-content: center;
  margin-bottom: 80px;
}

.hc-hero__content {
  text-align: center;
  max-width: 520px;
  padding: 0 20px;
}

.hc-hero__label {
  text-align: center;
  font-size: 2rem;
  font-family: Poppins, sans-serif;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  margin-bottom: 24px;
  opacity: 0.9;
}

.hc-hero__card {
  border-radius: 18px;
  padding: 28px 24px 22px;
  backdrop-filter: blur(2px);

  width: 100%;
  max-width: 520px;
  min-height: 150px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.hc-hero__quote {
  font-size: 0.95rem;
  line-height: 1.6;
  margin: 0 0 14px;
  flex: 1;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
}

.hc-hero__author {
  font-weight: 700;
  font-size: 0.9rem;
  margin: 0 0 2px;
}

.hc-hero__role {
  font-size: 0.8rem;
  opacity: 0.8;
  margin: 0 0 10px;
}

/* dots */
.hc-hero__dots {
  display: flex;
  justify-content: center;
  gap: 8px;
  margin-top: auto;
}

.hc-hero__dot {
  width: 10px;
  height: 10px;
  border-radius: 999px;
  border: none;
  background: rgba(255, 255, 255, 0.4);
  cursor: pointer;
  padding: 0;
  transition: all 0.2s ease;
}

.hc-hero__dot--active {
  width: 20px;
  background: #35d66e;
}

/* ================== CÍRCULO + FORM ================== */

.hc-circle-area {
  width: 100%;
  display: flex;
  justify-content: center;
  padding: 0 20px;
}

/* círculo exterior */
.hc-circle--outer {
  width: 560px;
  height: 560px;
  border-radius: 50%;
  border: 4px solid #45f08a;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* círculo interior verde */
.hc-circle--inner {
  width: 94%;
  height: 94%;
  border-radius: 50%;
  background: #45d86e;
  display: flex;
  align-items: center;
  justify-content: center;
}

.hc-circle__content {
  width: 80%;
  text-align: center;

  /* 🔹 clave para posicionar título arriba y form abajo */
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-start;
  padding-top: 40px;
  box-sizing: border-box;
}

/* título dentro del círculo */
.hc-circle__title {
  color: black;
  margin: 0 0 4px;
  font-size: 1.9rem;
  font-weight: 700;
}

.hc-circle__subtitle {
  color: black;
  margin: 0;
  font-size: 0.95rem;
  opacity: 0.9;
}

.hc-circle__form-placeholder {
  background: #ffffff;
  border-radius: 20px;
  padding: 22px 20px 26px;
  color: #04141f;
  font-size: 0.85rem;
  box-shadow: 0 18px 36px rgba(0, 0, 0, 0.25);
  margin-top: 0;
  transform: translateY(5%);

}

/* ============ AJUSTES SOLO PARA ContactForm AQUÍ ============ */
/* Usamos :deep para no tocar otros ContactForm en otras vistas */

.hc-circle__form-placeholder :deep(.contact-form) {
  width: 100%;
}

/* header más compacto dentro del círculo */
.hc-circle__form-placeholder :deep(.contact-form__header) {
  margin-bottom: 18px;
}

.hc-circle__form-placeholder :deep(.contact-form__title) {
  font-size: 1.2rem;
}

.hc-circle__form-placeholder :deep(.contact-form__subtitle) {
  font-size: 0.8rem;
}

/* inputs un poco más pequeños para que todo quepa cómodo */
.hc-circle__form-placeholder :deep(.contact-form__input) {
  padding: 8px 14px;
  font-size: 0.85rem;
}

.hc-circle__form-placeholder :deep(.contact-form__textarea) {
  min-height: 90px;
}

.hc-circle__form-placeholder :deep(.contact-form__group) {
  margin-bottom: 12px;
}

/* botón también ligeramente más compacto */
.hc-circle__form-placeholder :deep(.contact-form__submit) {
  padding: 10px 18px;
  font-size: 0.9rem;
}

.hc-circle__form-placeholder :deep(.contact-form__reset) {
  font-size: 0.78rem;
}

/* ================== RESPONSIVE ================== */

@media (max-width: 768px) {
  .hc-section {
    padding: 30px 0;
    padding-bottom: 440px;
  }

  .hc-circle--outer {
    width: 420px;
    height: 420px;
  }

  .hc-circle--inner {
    width: 94%;
    height: 94%;
  }

  .hc-circle__content {
    width: 100%;
  }

  .hc-circle__title {
    font-size: 1.55rem;
  }

  .hc-circle__subtitle {
    font-size: 0.9rem;
  }

  .hc-circle__form-placeholder {
    padding: 20px 16px 22px;
    transform: translateY(5%);
  }

  .hc-circle-area{
    padding: 0;
    margin: 0;
    width: 100%;
  }

  .hc-hero__card{
    padding: 0;
  }
}

@media (max-width: 480px) {
  .hc-circle--outer {
    width: 340px;
    height: 340px;
  }

  .hc-circle__form-placeholder {
    padding: 18px 32px;
    transform: translateY(3%);
  }

  .hc-circle__title {
    font-size: 1.4rem;
  }
}
</style>
