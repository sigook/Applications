<template>
  <section class="et-section">
    <!-- FOR EMPLOYERS -->
    <div class="et-employers-wrapper">
      <div class="et-employers-wrapper-2">
        <div id="for-employers" class="et-employers" data-aos="fade-up">
          <div class="et-employers__left">
            <span class="et-employers__arrow">➜</span>
          </div>

          <div class="et-employers__right">
            <p class="et-employers__title">FOR EMPLOYERS</p>
            <span class="et-employers__divider"></span>
            <p class="et-employers__text">
              Find the right people<br />
              for your business
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- FOR TALENTS -->
    <section class="et-talents-wrapper" data-aos="fade-up" data-aos-delay="150">
      <div class="et-talents-banner">
        <div class="et-talents-img">
          <img src="@/assets/images/talents-worker.jpg" alt="Talent" />
          <div class="et-talents-overlay"></div>
        </div>

        <div class="et-talents-content">
          <div class="et-talents-text">
            <p class="et-talents-title">FOR TALENTS</p>
            <span class="et-talents-divider"></span>
            <p class="et-talents-subtitle">
              Where great talent<br />
              meets great opportunities
            </p>
          </div>

          <span class="et-talents-arrow">➜</span>
        </div>
      </div>
    </section>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
  /* CONTEXTO GENERAL (fondo azul que viene del hero) */
  .et-section {
    width: 100%;
    background: #0f2f44;
    background: linear-gradient(180deg, rgb(5, 23, 35) 43%, rgba(255, 255, 255, 1) 61%);
    color: #ffffff;
    padding: 0;
    margin: 0;
  }

  /* ===================== FOR EMPLOYERS ===================== */

  .et-employers-wrapper {
    position: relative;
    width: 100%;
    overflow: hidden;
    padding-top: 25px; /* altura de la franja verde oscura */
    border-top-right-radius: 125px;
  }

  .et-employers-wrapper-2 {
    position: relative;
    width: 100%;
    overflow: hidden;
    padding-top: 25px;
    border-top-right-radius: 130px;
  }

  /* franja verde oscura superior curvada */
  .et-employers-wrapper::before {
    content: "";
    position: absolute;
    top: 0;
    right: -10%;
    width: 140%;
    height: 210px;
    background: #3c7d4a;
    border-top-right-radius: 150px;
  }

  .et-employers-wrapper-2::before {
    content: "";
    position: absolute;
    top: 0;
    right: -10%;
    width: 140%;
    height: 210px;
    background: #50c069;
    border-top-right-radius: 150px;
  }

  /* bloque principal verde claro */
  .et-employers {
    position: relative;
    z-index: 1;
    width: 100%;
    height: 14rem;
    background: #59dc76;
    border-radius: 0 150px 0 0;
    padding: 40px 9vw 54px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 40px;
    box-sizing: border-box;
  }

  /* flecha grande a la izquierda */
  .et-employers__left {
    flex: 0 0 auto;
  }

  .et-employers__arrow {
    font-size: 3.1rem;
    font-weight: 600;
  }

  /* texto alineado a la derecha */
  .et-employers__right {
    flex: 1;
    display: flex;
    flex-direction: column;
    align-items: flex-end;
    text-align: right;
  }

  .et-employers__title {
    margin: 0 0 8px;
    font-size: 1.7rem;
    font-weight: 700;
    letter-spacing: 0.09em;
  }

  .et-employers__divider {
    width: 80px;
    height: 2px;
    background: #ffffff;
    opacity: 0.8;
    margin-bottom: 10px;
  }

  .et-employers__text {
    margin: 0;
    font-size: 0.96rem;
    line-height: 1.5;
  }

  /* ===================== FOR TALENTS ===================== */

  /* Wrapper que mantiene continuidad del verde y crea la base gris */
  .et-talents-wrapper {
    position: relative;
    margin-top: -20px; /* pequeño solape con la sección de arriba */
    width: 100%;
    padding: 60px 0 90px;
    background: linear-gradient(#59dc76 0 55%, #dedfe4 55% 100%);
    overflow: hidden;
    border-bottom-right-radius: 120px;
  }

  /* Banner con la imagen (ocupa 100% del ancho) */
  .et-talents-banner {
    position: relative;
    z-index: 1; /* por encima de las formas de fondo */
    width: 100%;
    top: 70px;
    height: 25rem;
    border-radius: 150px 0 150px 0;
    overflow: hidden;
  }

  /* imagen que llena todo el banner */
  .et-talents-img {
    position: absolute;
    inset: 0;
  }

  .et-talents-img img {
    width: 100%;
    height: 100%;
    object-fit: cover;
    display: block;
  }

  /* overlay para oscurecer un poco */
  .et-talents-overlay {
    position: absolute;
    inset: 0;
    background: linear-gradient(
      to right,
      rgba(5, 10, 20, 0.9),
      rgba(5, 10, 20, 0.3)
    );
  }

  /* contenido encima de la imagen */
  .et-talents-content {
    position: relative;
    z-index: 1;
    height: 100%;
    padding: 32px 60px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    box-sizing: border-box;
    color: #ffffff;
  }

  .et-talents-text {
    max-width: 360px;
  }

  .et-talents-title {
    margin: 0 0 8px;
    font-size: 1.6rem;
    font-weight: 700;
    letter-spacing: 0.1em;
  }

  .et-talents-divider {
    width: 80px;
    height: 2px;
    background: #ffffff;
    opacity: 0.9;
    margin-bottom: 10px;
  }

  .et-talents-subtitle {
    margin: 0;
    font-size: 0.9rem;
    line-height: 1.6;
  }

  .et-talents-arrow {
    font-size: 2.3rem;
    font-weight: 600;
    color: #59dc76;
  }

  /* ===================== RESPONSIVE ===================== */

  /* Tablets / pantallas medianas */
  @media (max-width: 960px) {
    .et-employers {
      height: auto;
      padding: 36px 32px 48px;
      gap: 20px;
      border-radius: 0 120px 0 0;
    }

    .et-employers__title {
      font-size: 1.5rem;
    }

    .et-employers__text {
      font-size: 0.9rem;
    }

    .et-talents-wrapper {
      padding: 70px 0 80px;
    }

    .et-talents-banner {
      top: 40px;
      height: 22rem;
      border-radius: 120px 0 120px 0;
    }

    .et-talents-content {
      padding: 26px 30px;
    }
  }

  @media (max-width: 600px) {
  /* Fondo general más simple en mobile */
  .et-section {
    background: #04141f;
  }

  /* ---------- FOR EMPLOYERS ---------- */

  /* quitamos las franjas curvas de fondo en mobile */
  .et-employers-wrapper,
  .et-employers-wrapper-2 {
    padding-top: 0;
    border-top-right-radius: 0;
  }

  .et-employers-wrapper::before,
  .et-employers-wrapper-2::before {
    display: none;
  }

  /* tarjeta verde principal */
  .et-employers {
    margin-top: 0;
    width: 100%;
    height: 24rem;
    padding: 32px 20px 44px;
    border-radius: 0 80px 0 0;
    flex-direction: column;
    align-items: center;
    gap: 16px;
  }

  .et-employers__right {
    align-items: flex-start;
    text-align: left;
  }

  .et-employers__title {
    font-size: 1.7rem;
    letter-spacing: 0.08em;
  }

  .et-employers__text {
    font-size: 0.9rem;
    line-height: 1.5;
  }

  /* flecha en círculo centrada */
  .et-employers__left {
    align-self: center;
    margin-top: 20px;
  }

  .et-employers__arrow {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 74px;
    height: 74px;
    border-radius: 50%;
    border: 3px solid #ffffff;
    font-size: 2rem;
    font-weight: 600;
  }

  /* ---------- FOR TALENTS ---------- */

  /* quitamos gradiente raro y dejamos fondo liso */
  .et-talents-wrapper {
    margin-top: 0;
    padding: 0;
    background: #59DC76;
    background: linear-gradient(180deg,rgba(89, 220, 118, 1) 52%, rgba(255, 255, 255, 1) 56%);
    border-bottom-right-radius: 0;
  }

  /* tarjeta de talentos: se pega al bloque verde anterior */
  .et-talents-banner {
    top: 0;
    width: 100%;
    height: 24rem;
    border-radius: 80px 0 80px 0;
    overflow: hidden;
  }

  .et-talents-img img {
    object-fit: cover;
    object-position: center;
  }

  .et-talents-content {
    padding: 22px 22px 80px; /* espacio extra abajo para la flecha */
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 14px;
  }

  .et-talents-title {
    font-size: 1.7rem;
    letter-spacing: 0.09em;
  }

  .et-talents-subtitle {
    font-size: 0.9rem;
    line-height: 1.5;
  }

  /* flecha en círculo centrada al final de la tarjeta */
  .et-talents-arrow {
    position: absolute;
    bottom: 24px;
    left: 50%;
    transform: translateX(-50%);
    display: flex;
    align-items: center;
    justify-content: center;
    width: 74px;
    height: 74px;
    border-radius: 50%;
    border: 3px solid #59dc76;
    font-size: 2rem;
    color: #59dc76;
  }
}

</style>



