<template>
  <section class="partner-types" data-aos="fade-up">
    <!-- Switch -->
    <div class="partner-types__switch">
      <button
        class="partner-types__btn"
        :class="{ 'partner-types__btn--active': activeTab === 'recruiter' }"
        @click="activeTab = 'recruiter'"
      >
        Recruiter<br />
        Partner
      </button>
      <button
        class="partner-types__btn"
        :class="{ 'partner-types__btn--active': activeTab === 'business' }"
        @click="activeTab = 'business'"
      >
        Business<br />
        Partner
      </button>
    </div>

    <!-- Contenido dinámico -->
    <div v-if="activeTab === 'business'" class="partner-types__content">
      <h2 class="partner-types__title">Business Partner</h2>

      <div class="partner-types__grid">
        <!-- Columna izquierda -->
        <article class="partner-types__card">
          <div class="partner-types__image-circle">
            <!-- Cambia la imagen -->
            <img
              src="@/assets/images/partner-toronto.jpg"
              alt="City skyline"
            />
          </div>

          <p class="partner-types__text">
            As a Business Partner, you remain independent, managing your own
            relationships and pace, while
            <strong>Covenant handles contracts, compliance, payroll, and
              client support.</strong>
            Once a client begins working with us, you participate in the
            ongoing revenue from each successful placement or project.
          </p>
        </article>

        <!-- Columna derecha -->
        <article class="partner-types__card">
          <div class="partner-types__image-circle">
            <!-- Cambia la imagen -->
            <img
              src="@/assets/images/partner-business-woman.jpg"
              alt="Business partner"
            />
          </div>

          <p class="partner-types__text">
            This model is ideal for
            <strong>business developers, industry professionals, or
              consultants</strong>
            who want to expand their reach and income by partnering with a
            licensed, established staffing and talent management company.
            Together, we build success through trust, fairness, and shared
            results.
          </p>
        </article>
      </div>

      <div class="partner-types__cta-wrapper">
        <button class="partner-types__cta">Start Here</button>
      </div>
    </div>

    <!-- Contenido Recruiter (placeholder simple) -->
    <div v-else class="partner-types__content">
      <h2 class="partner-types__title">Recruiter Partner</h2>

      <div class="partner-types__grid partner-types__grid--single">
        <article class="partner-types__card">
          <p class="partner-types__text">
            As a Recruiter Partner, you focus on what you do best:
            <strong>finding and qualifying talent.</strong> Covenant supports
            you with client relationships, contracts, and back-office
            operations so you can turn every successful placement into
            recurring income.
          </p>
        </article>
      </div>

      <div class="partner-types__cta-wrapper">
        <button class="partner-types__cta">Start Here</button>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref } from 'vue'

const activeTab = ref('business') // opción inicial como en el diseño
</script>

<style scoped>
.partner-types {
  background: #ffffff;
  padding: 40px 0 70px;
}

/* switch */
.partner-types__switch {
  max-width: 360px;
  margin: 0 auto 40px;
  background: #32d26a;
  border-radius: 999px;
  display: flex;
  padding: 4px;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.15);
}

.partner-types__btn {
  flex: 1;
  border: none;
  background: transparent;
  font-size: 0.8rem;
  color: #ffffff;
  padding: 10px 0;
  border-radius: 999px;
  cursor: pointer;
  line-height: 1.3;
  transition: background 0.2s ease, color 0.2s ease;
}

.partner-types__btn--active {
  background: #ffffff;
  color: #05162d;
}

/* contenido */
.partner-types__content {
  max-width: 1180px;
  margin: 0 auto;
  padding: 0 24px;
}

.partner-types__title {
  text-align: center;
  font-size: 1.6rem;
  color: #0b1a33;
  margin-bottom: 40px;
}

.partner-types__grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 40px;
  align-items: center;
}

.partner-types__grid--single {
  grid-template-columns: minmax(0, 1fr);
}

.partner-types__card {
  text-align: center;
}

.partner-types__image-circle {
  width: 260px;
  height: 260px;
  border-radius: 50%;
  margin: 0 auto 22px;
  position: relative;
  overflow: hidden;
  background: #32d26a;
}

.partner-types__image-circle img {
  width: 115%;
  height: 115%;
  object-fit: cover;
}

.partner-types__text {
  font-size: 0.9rem;
  line-height: 1.7;
  color: #4c5664;
  max-width: 380px;
  margin: 0 auto;
}

.partner-types__cta-wrapper {
  display: flex;
  justify-content: center;
  margin-top: 40px;
}

.partner-types__cta {
  border: none;
  padding: 14px 60px;
  border-radius: 999px;
  background: #32d26a;
  color: #ffffff;
  font-weight: 600;
  font-size: 0.95rem;
  cursor: pointer;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.2);
  transition: transform 0.18s ease, box-shadow 0.18s ease, background 0.18s ease;
}

.partner-types__cta:hover {
  transform: translateY(-2px);
  background: #24a654;
  box-shadow: 0 18px 40px rgba(0, 0, 0, 0.3);
}

@media (max-width: 900px) {
  .partner-types__grid {
    grid-template-columns: minmax(0, 1fr);
  }

  .partner-types__text {
    max-width: 100%;
  }
}
</style>
