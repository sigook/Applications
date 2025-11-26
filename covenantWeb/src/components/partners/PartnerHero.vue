<template>
  <section class="partner-hero" data-aos="fade-up">
    <div class="partner-hero__wrapper">
      <!-- Texto izquierda -->
      <div class="partner-hero__content">
        <h1 class="partner-hero__title">Become a Partner</h1>
        <p class="partner-hero__text">
          If you're an independent recruiter or want to grow your staffing
          sales with the support of a company with national coverage and full
          corporate backing, contact us and become our business partner.
        </p>
      </div>

      <!-- Imagen derecha -->
      <div class="partner-hero__visual">
        <div class="partner-hero__circle">
          <!-- Cambia la ruta de la imagen por la tuya -->
          <img
            src="@/assets/images/partner-hero-man.png"
            alt="Covenant Partner"
            class="partner-hero__img"
          />
          <!-- elementos decorativos -->
          <span class="partner-hero__dot partner-hero__dot--small"></span>
          <span class="partner-hero__dot partner-hero__dot--medium"></span>
          <span class="partner-hero__dot partner-hero__dot--big"></span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup></script>

<style scoped>
.partner-hero {
  background: #f3f4f6;
  padding: 80px 0 60px;
}

.partner-hero__wrapper {
  max-width: 1180px;
  margin: 0 auto;
  padding: 0 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 40px;
}

.partner-hero__content {
  max-width: 520px;
}

.partner-hero__title {
  font-size: 3.6rem;
  line-height: 1.1;
  color: #59DC76;
  font-weight: 700;
  margin-bottom: 18px;
}

.partner-hero__text {
  font-size: 0.95rem;
  line-height: 1.6;
  color: #555b69;
}

.partner-hero__visual {
  flex: 1;
  display: flex;
  justify-content: center;
  margin-left: 150px;
}

.partner-hero__circle {
  position: relative;
  width: 460px;
  height: 460px;
  border-radius: 50%;
  background: #32d26a;
  display: flex;
  align-items: flex-end;
  justify-content: center;
  overflow: hidden;
}

.partner-hero__img {
  height: 115%;
  object-fit: contain;
}

/* dots decorativos */
.partner-hero__dot {
  position: absolute;
  border-radius: 50%;
  background: #05162d;
  opacity: 0.85;
}

.partner-hero__dot--small {
  width: 18px;
  height: 18px;
  bottom: 26px;
  left: 40px;
  background: #32d26a;
}

.partner-hero__dot--medium {
  width: 26px;
  height: 26px;
  top: 70px;
  right: 34px;
  background: #1c2940;
}

.partner-hero__dot--big {
  width: 40px;
  height: 40px;
  bottom: 52px;
  right: 70px;
  background: #05162d;
}

@media (max-width: 900px) {
  .partner-hero__wrapper {
    flex-direction: column;
    text-align: left;
  }

  .partner-hero__content {
    max-width: 100%;
  }
}
</style>
