<template>
  <section class="app-section" data-aos="fade-up" id="download">
    <div class="app-container">
      <!-- ================== BLOQUE SUPERIOR ================== -->
      <div class="app-top">
        <!-- Texto izquierda -->
        <div class="app-top__text">
          <h3 class="app-top__title">Download our APP</h3>
          <p class="app-top__subtitle">
            Smarter Staffing.<br />
            Powered by Technology
          </p>

          <a href="https://apps.apple.com/co/app/sigook/id1446736193" target="_blank" rel="noopener noreferrer" class="app-top__btn">
            <span>Download</span>
            <span class="app-top__btn-arrow">➜</span>
          </a>
        </div>

        <!-- Teléfono derecha -->
        <div class="app-top__phone-wrapper">
          <!-- círculo verde de fondo -->
          <div class="app-top__circle"></div>

          <!-- mano con el celular -->
          <img
            src="@/assets/images/phone-sigook.png"
            alt="Sigook app"
            class="app-top__phone"
          />
        </div>
      </div>

      <!-- ================== BLOQUE INFERIOR: CURVA ================== -->
      <div class="attendance-wrapper">
        <!-- curva azul del fondo -->
        <!-- curva azul del fondo -->
        <picture>
          <source
            media="(max-width: 500px)"
            srcset="@/assets/images/curva-azul-movil.png"
          />
          <img
            src="@/assets/images/blue-wave.png"
            alt=""
            class="attendance-wave attendance-wave--blue"
          />
        </picture>

        <!-- curva verde (la que ya tenías) -->
        <picture>
          <source
            media="(max-width: 500px)"
            srcset="@/assets/images/curva-verde-movil.png"
          />
          <img
            src="@/assets/images/attendance-wave.png"
            alt=""
            class="attendance-wave attendance-wave--green"
          />
        </picture>

        <!-- contenido encima de las curvas -->
        <div class="attendance-content">
          <div class="attendance-main">
            <!-- Icono izquierda -->
            <div class="attendance-icon">
              <img
                src="@/assets/images/attendance-icon.png"
                alt="Smart attendance"
                class="attendance-icon__img"
              />
            </div>

            <!-- Texto + bullets -->
            <div class="attendance-text">
              <h4 class="attendance-title">
                All-in-One Staffing &<br />Workforce Platform
              </h4>

              <ul class="attendance-list">
                <li>One platform. Total workforce visibility.</li>
                <li>Real-time attendance and supervisor-approved shifts.</li>
                <li>Instant, accurate timesheets—no manual processing.</li>
                <li>Mobile-first for workers, web-based for clients.</li>
                <li>From onboarding to invoicing and payroll, fully connected</li>
              </ul>
            </div>
          </div>

          <!-- Botones de stores centrados -->
          <div class="attendance-stores">
            <img
              src="@/assets/images/appstore.png"
              alt="App Store"
              class="attendance-store__img"
            />
            <img
              src="@/assets/images/playstore.png"
              alt="Google Play"
              class="attendance-store__img"
            />
          </div>
        </div>
      </div>

    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
.app-section {
  width: 100%;
  background: #ffffff;
  padding-top: 120px; /* Aumentado para evitar solapamiento con la sección anterior */
}

.app-container {
  max-width: 100%;
  margin: 0 auto;
  box-sizing: border-box;
}

/* ================== BLOQUE SUPERIOR ================== */

.app-top {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 40px;
  margin-bottom: 40px;
}

/* texto izquierda */
.app-top__text {
  flex: 0 0 35%;
  max-width: none;
  margin-top: 10rem;
  margin-left: 10rem;
  margin-bottom: 2rem;
}

.app-top__title {
  margin: 0 0 12px;
  font-size: 2.5rem;          /* más grande */
  font-weight: 700;
}

.app-top__subtitle {
  margin: 0 0 24px;
  font-size: 0.95rem;         /* más grande */
  line-height: 1.6;
  color: #666;
}

.app-top__btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 10px 26px;         /* botón más grande */
  border-radius: 999px;
  border: none;
  background: #35d66e;
  color: #04141f;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  text-decoration: none;
}

.app-top__btn-arrow {
  font-size: 1.2rem;
}

/* Teléfono y círculo */

.app-top__phone-wrapper {
  position: absolute;
  left: 50%;
  transform: translate(-30%,0%);
  flex: 0 0 50%;
  min-height: 380px;          /* más alto para la mano */
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 10px 0 20px;
  z-index: 3;
}

/* círculo centrado detrás del teléfono y más grande */
.app-top__circle {
  position: absolute;
  width: 560px;
  height: 560px;
  border-radius: 50%;
  background: #37d56e;
  top: 53%;
  left: 95%;
  transform: translate(-50%, -50%);
}

/* mano siempre por encima del círculo y más grande */
.app-top__phone {
  position: relative;
  z-index: 3;
  max-height: 800px;
  width: auto;
}

/* ================== BLOQUE INFERIOR: CURVA ================== */

.attendance-wrapper {
  position: relative;
  margin-top: 40px;
  min-height: 650px;          /* lo dejamos como lo tenías */
  overflow: hidden;
  width: 100vw;
  margin-left: calc(50% - 50vw);   /* rompe el ancho del contenedor central */          /* ★ NUEVO: recorta las waves dentro del wrapper */
}

/* estilo base compartido por ambas curvas */
.attendance-wave {
  position: absolute;
  left: 0;
  bottom: -100%;
  width: 100%;          /* 🚩 asegura que la imagen llene el 100% del wrapper */
  height: 200%;          /* la altura “bajita” que definimos antes */
  object-fit: cover;    /* 🚩 mejor cover aquí para que no queden huecos laterales */
  pointer-events: none;
}

/* curva azul: al fondo y desplazada un poco a la derecha */
.attendance-wave--blue {
  z-index: 0;
  transform: translateX(3%);
}

/* curva verde: por delante de la azul */
.attendance-wave--green {
  z-index: 3;
}

/* contenido encima de ambas curvas */
.attendance-content {
  position: relative;
  z-index: 4;
  padding: 250px 40px 40px;
  color: #ffffff;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  gap: 32px;
}

/* resto de estilos los dejas como ya los tienes */
.attendance-main {
  display: flex;
  align-items: flex-start;
  gap: 40px;
}

/* ★ NUEVO: Estilo para el contenedor del icono (Círculo Verde) */
.attendance-icon {
  display: flex;
  justify-content: right;
  align-items: center;
  width: 260px;       /* Tamaño del círculo */
  height: 260px;
  background-color: #37d56e; /* Mismo verde que el círculo superior */
  border-radius: 50%;
  flex-shrink: 0;     /* Evita que se aplaste */
}

/* ★ ACTUALIZADO: La imagen del icono */
.attendance-icon__img {
  width: 150px;       /* Ajustado para caber dentro del círculo de 220px */
  height: auto;
  display: block;
}


.attendance-text {
  max-width: 540px;
}

.attendance-title {
  margin: 0 0 16px;
  font-size: 2rem;
  font-weight: 700;
}

.attendance-list {
  margin: 0;
  padding-left: 22px;
  list-style: disc;
  font-size: 1.2rem;
  line-height: 1.9;
}

.attendance-stores {
  display: inline-flex;          /* se ajusta al contenido */
  justify-content: center;
  align-items: center;
  gap: 20px;

  padding: 10px 18px;            /* espacio alrededor de los iconos */
  background-color: #04141f;
  border-radius: 999px;          /* pill / cápsula */

  width: auto;                   /* asegura que no se estire */
  max-width: fit-content;        /* en navegadores modernos */
  margin: 0 auto;
}

.attendance-store__img {
  height: 34px;
  width: auto;
}

/* ================== RESPONSIVE ================== */

@media (max-width: 1024px) {
  .app-section {
    padding-bottom: 0;
    overflow-x: hidden;
  }

  /* teléfono centrado bajo el texto, como en tu captura */
  .app-top__phone-wrapper {
    margin-top: 70px;
    min-height: 400px;
    overflow: hidden;
  }

  .app-top__circle {
    width: 400px;
    height: 400px;
    top: 50%;
    left: 90%;
    transform: translate(-90%, -50%);
  }

  /* Ajustes específicos para 1024px (Tablet Landscape / Laptop pequeña) */
  
  /* Mover curva azul a la izquierda para cerrar hueco blanco */
  .attendance-wave--blue {
    transform: translateX(-1%);
  }

  /* Reducir contenido para caber en la curva verde */
  .attendance-content {
    padding-top: 320px; /* Ajuste para bajar el contenido un poco */
    padding-inline: 40px;
  }

  .attendance-icon {
    width: 180px;
    height: 180px;
  }
  
  .attendance-icon__img {
    width: 110px;
  }

  .attendance-title {
    font-size: 1.6rem;
  }
  
  .attendance-list {
    font-size: 0.95rem;
    line-height: 1.6;
  }
}

@media (max-width: 992px) {
  .app-top {
    flex-direction: column;
    align-items: flex-start;
  }

  .app-top__text {
    margin-top: 0;
  }

  .app-top__phone-wrapper {
    position: relative;
    left: auto;
    transform: none;
    width: 100%;
    align-self: center;
    flex: 0 0 auto;
  }
}

@media (max-width: 768px) {


  .app-container {
    padding: 0 20px;
  }

  .attendance-content {
    padding-inline: 20px;
  }

  .attendance-main {
    flex-direction: column;
    align-items: flex-start;
  }

  /* ★ ACTUALIZADO: Ajustes del círculo en Tablet/Móvil */
  .attendance-icon {
      width: 140px;
      height: 140px;
  }

  .attendance-icon__img {
    width: 90px;
  }

  .attendance-title {
    font-size: 1.4rem;
  }

  .attendance-list {
    font-size: 0.95rem;
  }
}

/* ====== MOBILE (≤ 768px) ====== */
@media (max-width: 768px) {
  .app-section {
    padding-bottom: 0;
    overflow-x: hidden;
    padding-top: 150px; /* Aumentar separación superior en tablets/móviles */
  }

  .app-top__text {
    margin: 0;
  }

  .app-top__title {
    font-size: 1.6rem;
  }

  .app-top__subtitle {
    font-size: 0.8rem;
    line-height: 1.5;
  }

  .app-top__btn {
    padding: 8px 20px;
    font-size: 0.85rem;
  }

  /* curva azul: al fondo y desplazada un poco a la derecha */
  .attendance-wave--blue {
    z-index: 0;
    transform: translateX(0%);
  }

  /* curva verde: reseteo en tablet/desktop si fuera necesario o ajustes leves */
  .attendance-wave--green {
    transform: translateX(0%);
  }

  /* teléfono centrado bajo el texto, como en tu captura */
  .app-top__phone-wrapper {
    margin-top: 70px;
    min-height: 400px;
    overflow: hidden;
  }

  .app-top__circle {
    width: 260px;
    height: 260px;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
  }

  .app-top__phone {
    max-height: 500px;
    padding-right: 0;
    margin-right: 0;
    margin-top: 10px;
  }

  /* BLOQUE INFERIOR: se ve como el rectángulo verde con curva y texto */
  .attendance-wrapper {
    margin-top: 0;
    min-height: 520px;
  }

  .attendance-content {
    /* Aumentamos padding superior para bajar todo el bloque (ya que quitamos el margin del icono) */
    padding: 400px 20px 20px;

  }

  /* REORGANIZACIÓN CON GRID: Título arriba, Icono + Lista abajo */
  .attendance-main {
    display: grid;
    grid-template-columns: auto 1fr; /* Columna auto para icono, resto para lista */
    grid-template-areas:
      "title title"
      "icon list";
    gap: 10px 20px; /* gap vertical 10, horizontal 20 */
    align-items: center; /* Centrar verticalmente icono y lista */
  }

  /* "Desempaquetamos" el div intermedio .attendance-text para que sus hijos sean grid items */
  .attendance-text {
    display: contents;
  }

  .attendance-title {
    grid-area: title;
    font-size: 1.5rem;
    line-height: 1.3;
    margin-bottom: 10px;
    text-align: left;
  }

  .attendance-list {
    grid-area: list;
    font-size: 0.85rem;
    line-height: 1.4;
    padding-left: 0; /* Reset default padding if needed, or adjust */
    margin: 0;
    list-style-position: inside; /* Ahorrar espacio horizontal */
  }
  
  /* Ajuste de items de lista para que no queden feos con list-style-position inside si son multilinea */
  .attendance-list li {
    margin-bottom: 6px;
  }

  /* ★ ACTUALIZADO: Icono ajustado para el grid */
  .attendance-icon {
    grid-area: icon;
    width: 110px;
    height: 110px;
    margin-top: 0; /* Eliminado margen superior gigante */
    justify-content: center;
  }

  .attendance-icon__img {
    width: 65px;
    margin-top: 0; 
  }

  .attendance-stores {
    margin-top: 24px;
    padding: 8px 16px;
  }

  .attendance-store__img {
    height: 26px;
  }
}

/* Ajuste específico para móviles (pantallas < 500px) */
@media (max-width: 425px) {
  .app-section {
    padding-top: 100px;
  }
}

/* Ajuste para móviles (coincide con el cambio de imagen) */
@media (max-width: 500px) {
  .app-top__phone {
    max-height: 400px;
  }

  .app-top__circle {
    width: 220px;
    height: 220px;
  }

  .app-top__phone-wrapper {
    min-height: 350px;
  }

  /* Ajustar posición de la curva verde en móvil */
  .attendance-wave--green {
    width: 130% !important;
    left: -15% !important;
    bottom: -62% !important; /* Subimos la imagen (original es -100%) */
  }

  /* Ajustar posición de la curva azul en móvil (detrás de la verde) */
  .attendance-wave--blue {
    width: 140% !important;
    left: -20% !important;
    bottom: -63% !important; /* Un poco más arriba/abajo para generar capas */
  }
}
</style>
