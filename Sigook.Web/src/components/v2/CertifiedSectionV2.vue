<template>
  <section class="certified-v2">
    <div class="certified-v2__bg">
      <img src="@/assets/images/v2/certified-bg.jpg" alt="" class="certified-v2__photo" />
      <div class="certified-v2__overlay"></div>
    </div>
    <div class="certified-v2__content">
      <h2 class="certified-v2__title">Smarter staffing for growing teams</h2>
      <p class="certified-v2__body">
        We support growing businesses with flexible staffing and recruitment solutions, combining experienced recruiters, streamlined processes, and reliable talent to meet evolving workforce needs.
      </p>
      <div class="certified-v2__badge">Grow With Us</div>
    </div>
  </section>
</template>

<script setup lang="ts">
// Static certified section — no reactive state needed
</script>

<style scoped>
.certified-v2 {
  position: relative;
  width: 100%;
  height: 577px;
  overflow: hidden;
  border-radius: 0 150px 0 150px;
}

.certified-v2__bg {
  position: absolute;
  inset: 0;
}

.certified-v2__photo {
  position: absolute;
  width: 100%;
  height: 200%;
  top: -47%;
  left: 0;
  object-fit: cover;
  object-position: center 40%;
}

.certified-v2__overlay {
  position: absolute;
  inset: 0;
  background: rgba(26, 117, 187, 0.45);
}

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
  font-family: 'Poppins', sans-serif;
  font-size: 30px;
  font-weight: 300;
  line-height: 1.2;
  color: #fff;
  margin: 0 0 56px;
  max-width: 643px;
  text-transform: none;
}

.certified-v2__body {
  font-family: 'Poppins', sans-serif;
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: #fff;
  margin: 0 0 40px;
  max-width: 578px;
}

.certified-v2__badge {
  background: var(--neutral-muted, #99B4BF);
  color: #fff;
  font-family: 'Poppins', sans-serif;
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  padding: 12px 60px;
  border-radius: 20px;
  letter-spacing: 0.02em;
}
</style>
