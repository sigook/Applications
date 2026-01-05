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
          <picture>
            <source
              media="(max-width: 600px)"
              srcset="@/assets/images/talents-worker-mobile.jpg"
            />

            <img
              src="@/assets/images/talents-worker.jpg"
              alt="Talent"
            />
          </picture>

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
  /* CONTEXTO GENERAL */
  .et-section {
    width: 100%;
    background: #0F2F44;
    background: linear-gradient(180deg, #0F2F44 43%, rgba(255, 255, 255, 1) 61%);
    color: #ffffff;
    padding: 0;
    margin: 0;
  }

  /* ===================== FOR EMPLOYERS ===================== */

  .et-employers-wrapper {
    position: relative;
    width: 100%;
    overflow: hidden;
    padding-top: 25px;
    border-top-right-radius: 125px;
  }

  .et-employers-wrapper-2 {
    position: relative;
    width: 100%;
    overflow: hidden;
    padding-top: 25px;
    border-top-right-radius: 130px;
  }

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

  /* BLOQUE PRINCIPAL VERDE CLARO (Employers) */
  .et-employers {
    position: relative;
    z-index: 1;
    width: 100%;
    height: 16rem;
    background: #59dc76;
    border-radius: 0 150px 0 0;
    padding: 40px 0 54px; /* Sin padding lateral para usar todo el ancho */

    /* GRID DE 2 COLUMNAS PARA CENTRADO PERFECTO */
    display: grid;
    grid-template-columns: 1fr 1fr; /* 50% y 50% */
    gap: 80px; /* Separación entre flecha y texto */
    align-items: center;
    box-sizing: border-box;
  }

  /* Columna Izquierda (Flecha): Se alinea a la derecha (al centro de la pantalla) */
  .et-employers__left {
    justify-self: end;
    display: flex;
  }

  .et-employers__arrow {
    font-size: 4rem;
    font-weight: 400;
  }

  /* Columna Derecha (Texto): Se alinea a la izquierda (al centro de la pantalla) */
  .et-employers__right {
    justify-self: start;
    display: flex;
    flex-direction: column;
    align-items: flex-start; /* Texto alineado a la izquierda */
    text-align: left;
  }

  .et-employers__title {
    margin: 0 0 12px;
    font-size: 2.8rem;
    font-weight: 700; /* GROSOR MANTENIDO EN 700 */
    letter-spacing: 0.05em;
    line-height: 1.1;
  }

  .et-employers__divider {
    width: 100px;
    height: 3px;
    background: #ffffff;
    opacity: 1;
    margin-bottom: 12px;
  }

  .et-employers__text {
    margin: 0;
    font-size: 1.1rem;
    line-height: 1.5;
  }

  /* ===================== FOR TALENTS ===================== */

  .et-talents-wrapper {
    position: relative;
    margin-top: -20px;
    width: 100%;
    padding: 60px 0 90px;
    background: linear-gradient(#59dc76 0 55%, #dedfe4 55% 100%);
    overflow: hidden;
    border-bottom-right-radius: 120px;
  }

  .et-talents-banner {
    position: relative;
    z-index: 1;
    width: 100%;
    top: 70px;
    height: 28rem;
    border-radius: 150px 0 150px 0;
    overflow: hidden;
  }

  .et-talents-img {
    position: absolute;
    inset: 0;
  }

  .et-talents-img picture,
  .et-talents-img img {
    width: 100%;
    height: 100%;
    display: block;
    object-fit: cover;
  }

  .et-talents-overlay {
    position: absolute;
    inset: 0;
    background: linear-gradient(
      to right,
      rgba(5, 10, 20, 0.9),
      rgba(5, 10, 20, 0.3)
    );
  }

  /* CONTENIDO TALENTS */
  .et-talents-content {
    position: relative;
    z-index: 1;
    height: 100%;
    padding: 32px 0;

    /* GRID DE 2 COLUMNAS PARA CENTRADO PERFECTO */
    display: grid;
    grid-template-columns: 1fr 1fr; /* 50% y 50% */
    gap: 80px; /* Separación */
    align-items: center;

    box-sizing: border-box;
    color: #ffffff;
  }

  /* Columna Izquierda (Texto): Se alinea a la derecha (hacia el centro) */
  .et-talents-text {
    justify-self: end;
    display: flex;
    flex-direction: column;
    align-items: flex-end; /* Alineamos los elementos a la derecha */
    text-align: right;     /* Texto alineado a la derecha */
    max-width: 450px;
  }

  .et-talents-title {
    margin: 0 0 12px;
    font-size: 2.8rem;
    font-weight: 700; /* GROSOR MANTENIDO EN 700 */
    letter-spacing: 0.05em;
    line-height: 1.1;
  }

  .et-talents-divider {
    display: block;
    width: 100px;
    height: 3px;
    background: #ffffff;
    opacity: 1;
    margin-bottom: 12px;
  }

  .et-talents-subtitle {
    margin: 0;
    font-size: 1.1rem;
    line-height: 1.6;
  }

  /* Columna Derecha (Flecha): Se alinea a la izquierda (hacia el centro) */
  .et-talents-arrow {
    justify-self: start;
    font-size: 4rem;
    font-weight: 400;
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
    /* Fondo general */
    .et-section {
      background: #0F2F44;
    }

    /* =================================
      1. FOR EMPLOYERS (Tarjeta Arriba)
    ================================= */

    /* Reset de bordes y padding */
    .et-employers-wrapper,
    .et-employers-wrapper-2 {
      padding-top: 0;
      border-top-right-radius: 0;
    }
    .et-employers-wrapper::before,
    .et-employers-wrapper-2::before {
      display: none;
    }

    .et-employers {
      margin-top: 0;
      width: 100%;
      /* Altura automática para ajustarse al contenido */
      height: auto;
      min-height: 24rem;
      padding: 50px 24px; /* Más padding vertical */
      border-radius: 0 80px 0 0;

      display: flex;
      flex-direction: column;
      align-items: flex-start;
      justify-content: space-between;
      gap: 30px;
    }

    /* TEXTO PRIMERO (Arriba) */
    .et-employers__right {
      order: 1;
      width: 100%;
      align-items: flex-start;
      text-align: left;
    }

    /* Título más grande */
    .et-employers__title {
      font-size: 2.4rem;
      font-weight: 700;
      letter-spacing: 0.02em;
      margin-bottom: 15px;
    }

    .et-employers__divider {
      display: block;
      margin-bottom: 20px;
      width: 80px;
      height: 3px;
      background: #ffffff;
      opacity: 1;
    }

    .et-employers__text {
      font-size: 1.1rem;
      line-height: 1.4;
    }

    /* FLECHA SEGUNDA (Abajo) */
    .et-employers__left {
      order: 2;
      align-self: center;
      margin-top: 10px;
    }

    .et-employers__arrow {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 70px;
      height: 70px;
      border-radius: 50%;
      border: 3px solid #ffffff;
      font-size: 2rem;
      font-weight: 600;
    }

    /* =================================
      2. FOR TALENTS (Tarjeta Abajo)
    ================================= */

    .et-talents-wrapper {
      margin-top: 0;
      padding: 0;
      background: linear-gradient(180deg, #59dc76 40%, #ffffff 40%);
      border-bottom-right-radius: 0;
    }

    .et-talents-banner {
      top: 0;
      width: 100%;
      height: 30rem;
      border-radius: 80px 0 80px 0;
      overflow: hidden;
      position: relative;
      background: transparent;
    }

    .et-talents-img img {
      object-position: center center;
    }

    /* Ajuste del overlay para que no oscurezca tanto la imagen */
    .et-talents-overlay {
      background: linear-gradient(
        to right,
        rgba(5, 10, 20, 0.6),
        rgba(5, 10, 20, 0.2)
      );
    }

    .et-talents-content {
      position: absolute;
      inset: 0;
      padding: 50px 24px;

      display: flex;
      flex-direction: column;
      align-items: flex-start; /* Alineado a la izquierda */
      justify-content: flex-start;
    }

    .et-talents-text {
      width: 100%;
      text-align: left;
    }

    /* Título más grande */
    .et-talents-title {
      font-size: 2.4rem; /* Aumentado */
      font-weight: 700;
      letter-spacing: 0.02em;
      margin-bottom: 15px;
    }

    .et-talents-divider {
      display: block;
      width: 80px;
      height: 3px;
      background-color: #ffffff;
      margin-bottom: 20px;
      opacity: 1;
    }

    .et-talents-subtitle {
      font-size: 1.1rem;
      line-height: 1.4;
    }

    /* Flecha abajo al centro */
    .et-talents-arrow {
      position: absolute;
      bottom: 40px;
      left: 50%;
      transform: translateX(-50%);
      display: flex;
      align-items: center;
      justify-content: center;
      width: 70px;
      height: 70px;
      border-radius: 50%;
      border: 3px solid #59dc76;
      background: transparent;
      font-size: 2rem;
      color: #59dc76;
      box-shadow: 0 4px 10px rgba(0,0,0,0.2);
    }
  }

</style>



