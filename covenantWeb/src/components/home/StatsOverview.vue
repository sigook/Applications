<template>
  <section class="our-numbers" data-aos="fade-up">
    <div class="our-numbers__inner">
      <!-- 👇 Imagen responsive: mobile + desktop -->
      <picture class="our-numbers__picture">
        <!-- 🔹 Imagen para móviles -->
        <source
          srcset="@/assets/images/Section-OurNumbers.png"
          media="(max-width: 768px)"
        />
        <!-- 🔹 Imagen por defecto (desktop / tablet) -->
        <img
          src="@/assets/our-numbers.svg"
          alt="Our numbers"
          class="our-numbers__img"
        />
      </picture>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
.our-numbers {
  width: 100%;
  background: #ffffff;
  padding: 20px 0;
  box-sizing: border-box;
}

/* contenedor que recorta bordes extra si la imagen los tuviera */
.our-numbers__inner {
  width: 100%;
  margin: 0 auto;
  overflow: hidden;
}

/* picture como bloque */
.our-numbers__picture {
  display: block;
  width: 100%;
}

/* imagen (desktop o mobile) siempre ajustada al ancho */
.our-numbers__img {
  display: block;
  width: 100%;
  max-width: none;
  height: auto;
}

/* Puedes ajustar solo el padding en mobile si quieres más aire */
@media (max-width: 768px) {
  .our-numbers {
    padding: 40px 0;
  }
}
</style>


