<template>
  <section class="why-v2">
    <!-- Block A — Corporate buildings photo -->
    <div class="why-v2__hero">
      <img src="@/assets/images/v2/why-bg.jpg" alt="" class="why-v2__hero-bg" aria-hidden="true" />
      <div class="why-v2__hero-overlay" aria-hidden="true"></div>
      <div class="why-v2__hero-content">
        <div class="why-v2__hero-left">
          <h2 class="v2-display why-v2__hero-title">Why Choose Us?</h2>
        </div>
        <div class="why-v2__hero-right">
          <p class="v2-body why-v2__hero-body">
            We are a Talent Management Agency focused on providing skilled professionals,
            tailored workforce solutions, and reliable support to ensure every partnership
            runs smoothly and efficiently.
          </p>
        </div>
      </div>
    </div>

    <!-- Block B — Blue content panel -->
    <div class="why-v2__panel">
      <div class="why-v2__panel-inner">

        <!-- Row 1: Map (left) + Nationwide vision (right) -->
        <div class="why-v2__row why-v2__row--top">
          <!-- Map column — pins PNG (transparent bg) floats directly on the blue panel -->
          <div class="why-v2__col why-v2__col--map">
            <img
              src="@/assets/images/v2/usa-map.png"
              alt="USA/Canada presence map"
              class="why-v2__map-img"
            />
          </div>

          <div class="why-v2__col why-v2__col--nationwide">
            <h2 class="v2-h2 why-v2__col-title">
              Nationwide vision<br>with local focus
            </h2>
            <p class="v2-body why-v2__col-body">
              We combine the reach of a nationwide recruitment network with the
              dedication and understanding of local expertise, ensuring tailored
              solutions that truly meet each client's unique needs.
            </p>
            <a href="#" class="btn btn--primary btn--sm why-v2__btn" @click.prevent>
              Learn More
              <ArrowIconV2 class="why-v2__btn-arrow" :width="32" :height="11" :stroke-width="1.5" />
            </a>
          </div>
        </div>

        <!-- Row 2: Market Leaders (left) + Recruitment Graphic (right) -->
        <div class="why-v2__row why-v2__row--bottom">
          <div class="why-v2__col why-v2__col--leaders">
            <h2 class="v2-h2 why-v2__col-title why-v2__col-title--right">
              We are Market<br>Leaders &amp; Innovators
            </h2>
            <p class="v2-body why-v2__col-body why-v2__col-body--right">
              At Sigook Work Factory, we pride ourselves on being trusted market leaders
              and forward-thinking innovators. By combining industry expertise with
              modern strategies, we create meaningful connections between businesses and
              top talent. Our commitment to excellence ensures that both employers and
              job seekers achieve success in a rapidly changing world.
            </p>
            <div class="why-v2__col-btn-right">
              <a href="#" class="btn btn--primary btn--sm why-v2__btn" @click.prevent>
                Learn More
                <ArrowIconV2 class="why-v2__btn-arrow" :width="32" :height="11" :stroke-width="1.5" />
              </a>
            </div>
          </div>

          <div class="why-v2__col why-v2__col--graphic">
            <!-- Figma asset: recruitment graphic 01 1 (node 2442:670), 480×360px -->
            <img
              src="@/assets/images/v2/recruitment-graphic.png"
              alt=""
              aria-hidden="true"
              class="why-v2__graphic-img"
            />
          </div>
        </div>

      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import ArrowIconV2 from '@/components/v2/ArrowIconV2.vue'
</script>

<style scoped>
.why-v2 {
  position: relative;
  z-index: 1;           /* above Numbers (z-index:0) */
  margin-top: -260px;   /* overlaps the bottom 260px of Numbers: Numbers ends at y=2724, WhyChooseUs starts at y=2464 */
}

/* ── Block A: Photo ── */
.why-v2__hero {
  position: relative;
  z-index: 2;       /* above the blue panel (z-index:1) — matches Figma layer order:
                       corporate buildings 01 1 (node 425:5791) is listed after
                       Rectangle 16 (the panel) in Figma, meaning it renders on top */
  width: 100%;
  height: 588px;
  border-radius: 150px 0 150px 0;
  overflow: hidden;
}

.why-v2__hero-bg {
  position: absolute;
  width: 100%;
  height: 200%;
  top: -30%;
  left: 0;
  object-fit: cover;
  object-position: center 30%;
}

.why-v2__hero-overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(180deg, rgba(16, 52, 75, 0.23) 0%, rgba(15, 47, 68, 0.65) 100%),
    linear-gradient(180deg, rgba(69, 147, 86, 0.4) 0%, rgba(102, 102, 102, 0.4) 100%);
}

.why-v2__hero-content {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  height: 100%;
  padding: 0 80px;
  gap: 94px;
}

.why-v2__hero-left {
  flex: 0 0 480px;
  text-align: right;
}

.why-v2__hero-title {
  color: #fff;
  margin: 0;
}

.why-v2__hero-right {
  flex: 1;
}

.why-v2__hero-body {
  color: #fff;
  margin: 0;
}

/* ── Block B: Blue panel ── */
.why-v2__panel {
  position: relative;
  background: var(--c-brand-blue);
  border-radius: 150px 0 150px 0;
  margin-top: -196px;
  z-index: 1;
  padding-bottom: 120px;
  overflow: hidden;
}

.why-v2__panel-inner {
  max-width: 1440px;
  margin: 0 auto;
  padding: 248px 80px 0;
}

/* ── Rows ── */
.why-v2__row {
  display: flex;
  align-items: flex-start;
  gap: 92px;
}

.why-v2__row--bottom {
  margin-top: 72px;
}

/* ── Columns ── */
.why-v2__col {
  flex: 1;
}

.why-v2__col--map {
  flex: 0 0 575px;
}

.why-v2__col--nationwide {
  padding-top: 48px;
}

.why-v2__col--leaders {
  flex: 0 0 575px;
}

.why-v2__col--graphic {
  display: flex;
  align-items: center;
  padding-top: 24px;
}

/* ── Map pins image — transparent PNG floats on blue panel background ── */
.why-v2__map-img {
  width: 100%;
  height: auto;
  display: block;
}

/* ── Recruitment graphic image — Figma node 2442:670, 480×360px ── */
.why-v2__graphic-img {
  width: 100%;
  max-width: 480px;
  height: auto;
  display: block;
}

/* ── Column text ── */
.why-v2__col-title {
  color: #fff;
  margin: 0;
}

.why-v2__col-title--right {
  text-align: right;
}

.why-v2__col-body {
  color: rgba(255, 255, 255, 0.9);
  margin: 20px 0 24px;
}

.why-v2__col-body--right {
  text-align: right;
}

.why-v2__col-btn-right {
  text-align: right;
}

/* ── Learn More buttons ── */
.why-v2__btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
}

.why-v2__btn-arrow {
  flex-shrink: 0;
}

/* ── Mobile ── */
@media (max-width: 1023px) {
  .why-v2 {
    margin-top: 0;  /* no overlap with Numbers on mobile */
  }

  .why-v2__hero {
    height: auto;
    min-height: 280px;
    border-radius: 80px 0 80px 0;
    padding: 60px 24px;
  }

  .why-v2__hero-bg {
    height: 100%;
    top: 0;
  }

  .why-v2__hero-content {
    flex-direction: column;
    align-items: flex-start;
    padding: 0;
    height: auto;
    gap: 20px;
  }

  .why-v2__hero-left {
    flex: none;
    width: 100%;
    text-align: left;
  }

  .why-v2__hero-title {
    font-size: 32px;
  }

  .why-v2__hero-right {
    width: 100%;
  }

  .why-v2__panel {
    border-radius: 80px 0 80px 0;
    margin-top: -80px;
  }

  .why-v2__panel-inner {
    padding: 120px 24px 0;
  }

  .why-v2__row {
    flex-direction: column;
    gap: 40px;
  }

  .why-v2__col--leaders,
  .why-v2__col--map {
    flex: none;
    width: 100%;
  }

  .why-v2__col--nationwide {
    padding-top: 0;
  }

  .why-v2__col-title--right,
  .why-v2__col-body--right,
  .why-v2__col-btn-right {
    text-align: left;
  }
}
</style>
