<template>
  <!-- Employers variant -->
  <section v-if="variant === 'employers'" class="employers-v2" role="button" tabindex="0">
    <!-- Photo layer (fades IN on hover) -->
    <div class="employers-v2__photo-layer" aria-hidden="true">
      <img :src="employersPhoto" alt="" class="employers-v2__photo-img" />
      <div class="employers-v2__photo-overlay"></div>
    </div>

    <!-- Solid color layers (fade OUT on hover) -->
    <div class="employers-v2__bg-1"></div>
    <div class="employers-v2__bg-2"></div>
    <div class="employers-v2__bg-3"></div>

    <div class="employers-v2__arrow" aria-hidden="true">
      <ArrowIconV2 :width="52" :height="20" :stroke-width="2" />
    </div>

    <div class="employers-v2__text">
      <h1 class="v2-h1">FOR EMPLOYERS</h1>
      <div class="sect-line"></div>
      <p class="v2-body">
        Find the right people<br>for your business
      </p>
    </div>
  </section>

  <!-- Talents variant -->
  <section v-else class="talents-v2" role="button" tabindex="0">
    <!-- Photo layer (always present, photo revealed on hover) -->
    <div class="talents-v2__photo-layer" aria-hidden="true">
      <img :src="talentsPhoto" alt="" class="talents-v2__photo-img" />
      <div class="talents-v2__photo-overlay"></div>
    </div>

    <!-- Solid red cover (fades OUT on hover, revealing the photo) -->
    <div class="talents-v2__solid-bg"></div>

    <div class="talents-v2__arrow" aria-hidden="true">
      <ArrowIconV2 :width="52" :height="20" :stroke-width="2" />
    </div>

    <div class="talents-v2__text">
      <h1 class="v2-h1">FOR TALENTS</h1>
      <div class="sect-line sect-line--right"></div>
      <p class="v2-body">
        Where great talent<br>meets great opportunities
      </p>
    </div>
  </section>
</template>

<script setup lang="ts">
import ArrowIconV2 from '@/components/v2/ArrowIconV2.vue'
import employersPhoto from '@/assets/images/v2/employers-office.jpg'
import talentsPhoto from '@/assets/images/v2/talents-worker.jpg'

const props = defineProps<{
  variant: 'employers' | 'talents'
}>()
</script>

<style scoped>
/* ── Shared decorative line ── */
.sect-line {
  width: 113px;
  height: 2px;
  background: #fff;
  margin: 16px 0 20px;
}

.sect-line--right {
  margin-left: auto;
}

/* ─────────────────────────────────────────────
   Employers
───────────────────────────────────────────── */
.employers-v2 {
  position: relative;
  width: 100%;
  height: 659px;
  background: var(--c-brand-navy);
  overflow: hidden;
  cursor: pointer;
  outline: none;
}

/* Photo layer — hidden by default, fades in on hover */
.employers-v2__photo-layer {
  position: absolute;
  inset: 0;
  border-radius: 0 125px 150px 0;
  overflow: hidden;
  opacity: 0;
  transition: opacity 0.7s ease;
  z-index: 1;
}

.employers-v2:hover .employers-v2__photo-layer,
.employers-v2:focus-visible .employers-v2__photo-layer {
  opacity: 1;
}

.employers-v2__photo-img {
  position: absolute;
  width: 100%;
  height: 184%;
  top: -32%;
  left: 0;
  object-fit: cover;
  object-position: center top;
}

.employers-v2__photo-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    180deg,
    rgba(21, 117, 187, 0.6) 0%,
    rgba(15, 47, 68, 0.82) 100%
  );
}

/* Solid color layers — visible by default, fade out on hover */
.employers-v2__bg-1 {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 659px;
  background: #13629e;
  border-radius: 0 125px 150px 0;
  z-index: 2;
  transition: opacity 0.7s ease;
}

.employers-v2__bg-2 {
  position: absolute;
  left: 0;
  top: 29px;
  width: 100%;
  height: 630px;
  background: var(--c-brand-blue);
  border-radius: 0 130px 150px 0;
  z-index: 2;
  transition: opacity 0.7s ease;
}

.employers-v2__bg-3 {
  position: absolute;
  left: 0;
  top: 57px;
  width: 100%;
  height: 602px;
  background: var(--c-brand-cyan);
  border-radius: 0 130px 150px 0;
  z-index: 2;
  transition: opacity 0.7s ease;
}

.employers-v2:hover .employers-v2__bg-1,
.employers-v2:hover .employers-v2__bg-2,
.employers-v2:hover .employers-v2__bg-3,
.employers-v2:focus-visible .employers-v2__bg-1,
.employers-v2:focus-visible .employers-v2__bg-2,
.employers-v2:focus-visible .employers-v2__bg-3 {
  opacity: 0;
}

/* Content — always on top */
.employers-v2__arrow {
  position: absolute;
  left: 703px;
  top: 178px;
  transform: translate(-50%, -50%);
  z-index: 3;
  display: flex;
}

.employers-v2__text {
  position: absolute;
  left: 939px;
  top: 105px;
  z-index: 3;
  color: #fff;
}

.employers-v2__text .v2-h1 {
  color: #fff;
  margin: 0;
  white-space: nowrap;
}

.employers-v2__text .v2-body {
  color: #fff;
  margin: 0;
}

/* ─────────────────────────────────────────────
   Talents
───────────────────────────────────────────── */
.talents-v2 {
  position: relative;
  width: 100%;
  height: 354px;
  background: var(--c-brand-red);
  border-radius: 150px 0 150px 0;
  margin-top: -312px;
  z-index: 3;
  overflow: hidden;
  cursor: pointer;
  outline: none;
}

/* Photo layer — always present, photo revealed when solid cover fades */
.talents-v2__photo-layer {
  position: absolute;
  inset: 0;
  z-index: 1;
}

.talents-v2__photo-img {
  position: absolute;
  height: 356%;
  left: -7.59%;
  top: -48.23%;
  width: 108.04%;
  object-fit: cover;
}

/* Semi-transparent red overlay that stays on top of photo at all times */
.talents-v2__photo-overlay {
  position: absolute;
  inset: 0;
  background: rgba(229, 45, 39, 0.55);
}

/* Solid red cover — fades out on hover, revealing the photo+overlay */
.talents-v2__solid-bg {
  position: absolute;
  inset: 0;
  background: var(--c-brand-red);
  z-index: 2;
  transition: opacity 0.7s ease;
}

.talents-v2:hover .talents-v2__solid-bg,
.talents-v2:focus-visible .talents-v2__solid-bg {
  opacity: 0;
}

/* Content — always on top */
.talents-v2__arrow {
  position: absolute;
  left: 692px;
  top: 162px;
  transform: translate(-50%, -50%);
  z-index: 4;
  display: flex;
}

.talents-v2__text {
  position: absolute;
  left: 259px;
  width: 261px;
  top: 83px;
  text-align: right;
  z-index: 4;
  color: #fff;
}

.talents-v2__text .v2-h1 {
  color: #fff;
  margin: 0;
  white-space: nowrap;
}

.talents-v2__text .v2-body {
  color: #fff;
  margin: 0;
}

/* ── Mobile ── */
@media (max-width: 1023px) {
  .employers-v2 {
    height: auto;
    min-height: 420px;
    padding-bottom: 48px;
  }

  .employers-v2__photo-layer { height: 100%; border-radius: 0 80px 80px 0; }
  .employers-v2__bg-1 { height: 100%; border-radius: 0 80px 80px 0; }
  .employers-v2__bg-2 { height: calc(100% - 29px); border-radius: 0 80px 80px 0; }
  .employers-v2__bg-3 { height: calc(100% - 57px); border-radius: 0 80px 80px 0; }

  .employers-v2__arrow {
    left: 50%;
    top: 56px;
  }

  .employers-v2__text {
    left: 24px;
    right: 24px;
    top: 120px;
  }

  .talents-v2 {
    height: auto;
    min-height: 300px;
    padding: 80px 24px 48px;
    margin-top: -80px;
    border-radius: 80px 0 80px 0;
  }

  .talents-v2__text {
    position: relative;
    left: auto;
    width: 100%;
    top: auto;
    text-align: left;
  }

  .talents-v2__text .sect-line--right {
    margin-left: 0;
  }

  .talents-v2__arrow {
    left: 50%;
    top: 40px;
  }
}
</style>
