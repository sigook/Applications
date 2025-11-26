<template>
  <section class="our-numbers" data-aos="fade-up">
    <div class="our-numbers__inner">
      <img
        src="@/assets/our-numbers.svg"
        alt="Our numbers"
        class="our-numbers__img"
      />
    </div>
  </section>
</template>

<script setup>
</script>

<style scoped>
.our-numbers {
  width: 100%;
  background: #ffffff;
  padding: 20px 0;
  box-sizing: border-box;
}

/* contenedor que recorta los bordes extra */
.our-numbers__inner {
  width: 100%;
  margin: 0 auto;
  overflow: hidden;         /* recorta lo que se sale de la imagen */
}

.our-numbers__img {
  display: block;
  width: 100%;
  max-width: none;
  height: auto;
  transform: translateX(0%);
}

/* En pantallas pequeñas podemos bajar un poco el zoom */
@media (max-width: 768px) {
  .our-numbers {
    padding: 60px 0;
  }

  .our-numbers__img {
    width: 130%;
    transform: translateX(-15%);
  }
}
</style>

