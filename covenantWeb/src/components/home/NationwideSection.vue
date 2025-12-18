<template>
  <section class="nw-section" data-aos="fade-up">
    <div class="nw-container">
      <!-- ROW 1: MAPA + TEXTO -->
      <div class="nw-row">
        <div class="nw-col nw-col--image">
          <img
            src="@/assets/images/map-canada.png"
            alt="Map of Canada"
            class="nw-image nw-image--map"
          />
        </div>

        <div class="nw-col nw-col--text">
          <h3 class="nw-title">
            Nationwide vision<br />
            <span class="nw-title__accent">with local focus</span>
          </h3>

          <p class="nw-text">
            We combine the reach of a nationwide recruitment network with the
            dedication and understanding of local expertise, ensuring tailored
            solutions that truly meet each client's unique needs.
          </p>

          <button class="nw-btn">
            <span>Learn More</span>
            <span class="nw-btn__arrow">➜</span>
          </button>
        </div>
      </div>

      <!-- ROW 2: TEXTO + ICONO -->
      <div class="nw-row nw-row--reverse">
        <div class="nw-col nw-col--text nw-col--text-center btn-end">
          <h3 class="nw-title-2">
            We are Market<br />
            <span class="nw-title__accent">Leaders &amp; Innovators</span>
          </h3>

          <p class="nw-text-2">
            At Covenant Group, we pride ourselves on being trusted market leaders and
            forward-thinking innovators. By combining industry expertise with modern
            strategies, we create meaningful connections between businesses and top
            talent. Our commitment to excellence ensures that both employers and job
            seekers achieve success in a rapidly changing world.
          </p>

          <button class="nw-btn">
            <span>Learn More</span>
            <span class="nw-btn__arrow">➜</span>
          </button>
        </div>

        <div class="nw-col nw-col--image">
          <img
            src="@/assets/images/icon-talent-search.png"
            alt="Talent search"
            class="nw-image nw-image--icon"
          />
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
.nw-section {
  width: 100%;
  background: #062338; /* azul oscuro del diseño */
  padding: 80px 0 90px;
  color: #ffffff;
}

.nw-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 32px;
  box-sizing: border-box;
}

/* ================= ROWS ================= */

.nw-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 60px;
  margin-bottom: 80px;
}

.nw-row:last-child {
  margin-bottom: 0;
}

.nw-row--reverse {
  flex-direction: row;
}

/* columnas */

.nw-col {
  flex: 1;
}

/* imagen */
.nw-col--image {
  display: flex;
  justify-content: center;
  align-items: center;
}

.nw-image {
  max-width: 100%;
  height: auto;
  display: block;
}

.nw-image--map {
  max-width: 420px;
}

.nw-image--icon {
  max-width: 360px;
}

/* texto */
.nw-col--text {
  max-width: 480px;
}

.nw-col--text-center {
  text-align: left;
}

/* ================= TIPOGRAFÍA ================= */

.nw-title {
  margin: 0 0 18px;
  font-size: 2rem;
  font-weight: 700;
  line-height: 1.25;
}

.nw-title__accent {
  /* mismo tamaño, solo sirve para agrupar si luego querés estilos distintos */
}

.nw-text {
  margin: 0 0 26px;
  font-size: 0.96rem;
  line-height: 1.7;
  color: #e3edf5;
}

.nw-title-2 {
  text-align: end;
  margin: 0 0 18px;
  font-size: 2rem;
  font-weight: 700;
  line-height: 1.25;
}

.nw-text-2 {
  text-align: end;
  margin: 0 0 26px;
  font-size: 0.96rem;
  line-height: 1.7;
  color: #e3edf5;
}

/* ================= BOTÓN ================= */
/* La columna de texto se comporta como un flex vertical */
.btn-end {
  display: flex;
  flex-direction: column;
}

/* El botón se empuja hacia abajo y queda alineado con el texto */
.btn-end .nw-btn {
  margin-top: auto;        /* lo manda al final del contenedor */
  align-self: flex-end;  /* misma alineación horizontal que el texto */
}


.nw-btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 10px 26px;
  border-radius: 999px;
  border: none;
  cursor: pointer;
  background: #35d66d;
  color: #062338;
  font-size: 0.88rem;
  font-weight: 600;
  text-transform: none;
}



.nw-btn__arrow {
  font-size: 1.1rem;
  transform: translateY(1px);
}

/* ================= RESPONSIVE ================= */

@media (max-width: 992px) {
  .nw-row {
    gap: 40px;
  }

  .nw-image--map {
    max-width: 360px;
  }

  .nw-image--icon {
    max-width: 320px;
  }

  .nw-title {
    font-size: 1.8rem;
  }
}

@media (max-width: 768px) {
  .nw-section {
    padding: 60px 0 70px;
  }

  .nw-container {
    padding: 0 24px;
  }

  .nw-row {
    flex-direction: column;
    margin-bottom: 60px;
    text-align: left;
  }

  .nw-row--reverse {
    flex-direction: column;
  }

  .nw-col--text,
  .nw-col--text-center {
    max-width: none;
  }

  .nw-col--image {
    order: -1; /* imagen arriba en mobile, si no te gusta quita esta línea */
  }

  .nw-image--map,
  .nw-image--icon {
    max-width: 260px;
  }
}

@media (max-width: 480px) {
  .nw-title {
    font-size: 1.6rem;
  }

  .nw-text {
    font-size: 0.9rem;
  }

  .nw-btn {
    padding-inline: 20px;
  }
}
</style>
