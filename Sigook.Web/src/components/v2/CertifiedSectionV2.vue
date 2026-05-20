<template>
  <div class="certified-v2-wrap">
    <!-- Back layer — same shape, sits 20px above the main section -->
    <div class="certified-v2__back" aria-hidden="true"></div>

    <section class="certified-v2">
      <!-- Photo layer 1 -->
      <div class="certified-v2__bg" aria-hidden="true">
        <img src="@/assets/images/v2/certified/certified-bg.jpg" alt="" class="certified-v2__photo certified-v2__photo--1" />
        <!-- Photo layer 2 -->
        <img src="@/assets/images/v2/certified/certified-bg2.jpg" alt="" class="certified-v2__photo certified-v2__photo--2" />
      </div>

      <div class="certified-v2__content">
        <h2 class="certified-v2__title">Smarter staffing for growing teams</h2>
        <p class="certified-v2__body">
          We support growing businesses with flexible staffing and recruitment solutions,
          combining experienced recruiters, streamlined processes, and reliable talent
          to meet evolving workforce needs.
        </p>
        <div class="certified-v2__badge">Grow With Us</div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
// Static section — no reactive state needed
</script>

<style scoped>
/* ── Wrapper — carries the overlap margin and z-index ── */
.certified-v2-wrap {
  position: relative;
  z-index: 2;
  margin-top: -556px;
  /* 20px padding-top pushes the section down, exposing the back layer above it */
  padding-top: 20px;
}

/* ── Back layer — same dimensions + border-radius, 20px above section ── */
.certified-v2__back {
  position: absolute;
  top: 40px;
  left: 0;
  width: 100%;
  height: 577px;
  border-radius: 150px 0 150px 0;
  background: #1A75BB;
  opacity: 0.45;
  z-index: 0;
}

/* ── Section shell ── */
.certified-v2 {
  position: relative;
  z-index: 1;
  width: 100%;
  height: 577px;
  overflow: hidden;
  border-radius: 150px 0 150px 0;
  /* Cast a soft shadow downward to reinforce the overlap onto AppDownload */
  box-shadow: 0 22px 40px -12px rgba(0, 0, 0, 0.45);
}

/* ── Photo background ── */
.certified-v2__bg {
  position: absolute;
  inset: 0;
  overflow: hidden;
}

.certified-v2__photo {
  position: absolute;
  left: 0;
  max-width: none;
  object-fit: cover;
}

.certified-v2__photo--1 {
  width: 100%;
  height: 194.24%;
  top: -47.12%;
}

.certified-v2__photo--2 {
  width: 118.73%;
  height: 230.63%;
  top: -71.76%;
  left: -0.56%;
}

/* ── Blue overlay ── */
.certified-v2::after {
  content: '';
  position: absolute;
  inset: 0;
  background: rgba(26, 117, 187, 0.45);
  pointer-events: none;
  z-index: 0;
}

/* ── Content ── */
.certified-v2__content {
  position: relative;
  z-index: 1;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 122px 80px 0;
}

.certified-v2__title {
  font-family: var(--font-family, 'Poppins', sans-serif);
  font-size: 30px;
  font-weight: 300;
  line-height: 1.2;
  color: #fff;
  margin: 0 0 56px;
  max-width: 643px;
  text-transform: none;
}

.certified-v2__body {
  font-family: var(--font-family, 'Poppins', sans-serif);
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: #fff;
  margin: 0 0 40px;
  max-width: 578px;
}

.certified-v2__badge {
  background: var(--c-neutral-muted, #99B4BF);
  color: #fff;
  font-family: var(--font-family, 'Poppins', sans-serif);
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  padding: 12px 60px;
  border-radius: 20px;
  white-space: nowrap;
  min-width: 324px;
  text-align: center;
}

/* ── Mobile ── */
@media (max-width: 1023px) {
  .certified-v2-wrap {
    margin-top: -160px; /* overlaps the 160px blue extension of WhyChooseUs — photo sits on top of the blue */
    padding-top: 0;
  }

  .certified-v2__back {
    display: none;
  }

  .certified-v2 {
    height: auto;
    min-height: 400px;
    border-radius: 80px 0 80px 0;
    padding-bottom: 100px;
  }

  .certified-v2__content {
    padding: 80px 24px 0;
  }

  .certified-v2__badge {
    min-width: 0;
    padding: 12px 40px;
  }
}
</style>
