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
          <div class="why-v2__col why-v2__col--map">
            <div class="why-v2__map-wrap">
              <div class="why-v2__map-container">
                <!-- Figma asset: Canada Map (node 425:5692), 574×551px frame -->
                <img
                  src="@/assets/images/v2/canada-map.png"
                  alt="USA/Canada presence map"
                  class="why-v2__map-img"
                />
              </div>
              <div class="why-v2__hawaii-wrap">
                <!-- Figma asset: hawaii 1 (node 425:5695), 172×129px display size -->
                <img
                  src="@/assets/images/v2/hawaii-map.png"
                  alt=""
                  aria-hidden="true"
                  class="why-v2__hawaii-img"
                />
              </div>
            </div>
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
            <svg class="why-v2__graphic-svg" viewBox="0 0 480 360" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
              <!-- Magnifying glass -->
              <circle cx="280" cy="165" r="130" fill="none" stroke="white" stroke-width="12" opacity="0.9"/>
              <line x1="375" y1="265" x2="440" y2="330" stroke="white" stroke-width="16" stroke-linecap="round" opacity="0.9"/>
              <!-- Person inside circle -->
              <circle cx="280" cy="140" r="28" fill="white" opacity="0.9"/>
              <path d="M220 210 Q220 185 280 185 Q340 185 340 210" fill="white" opacity="0.9"/>
              <!-- Smaller person left-1 -->
              <circle cx="90" cy="155" r="20" fill="white" opacity="0.75"/>
              <path d="M48 210 Q48 193 90 193 Q132 193 132 210" fill="white" opacity="0.75"/>
              <!-- Smaller person left-2 -->
              <circle cx="145" cy="175" r="18" fill="white" opacity="0.65"/>
              <path d="M108 225 Q108 210 145 210 Q182 210 182 225" fill="white" opacity="0.65"/>
              <!-- Sparkle stars -->
              <g transform="translate(420,60)" stroke="white" stroke-width="2.5" stroke-linecap="round" opacity="0.8">
                <line x1="0" y1="-14" x2="0" y2="14"/><line x1="-14" y1="0" x2="14" y2="0"/>
                <line x1="-8" y1="-8" x2="8" y2="8"/><line x1="8" y1="-8" x2="-8" y2="8"/>
              </g>
              <g transform="translate(240,20)" stroke="white" stroke-width="2" stroke-linecap="round" opacity="0.6">
                <line x1="0" y1="-10" x2="0" y2="10"/><line x1="-10" y1="0" x2="10" y2="0"/>
                <line x1="-6" y1="-6" x2="6" y2="6"/><line x1="6" y1="-6" x2="-6" y2="6"/>
              </g>
              <g transform="translate(30,90)" stroke="white" stroke-width="2" stroke-linecap="round" opacity="0.6">
                <line x1="0" y1="-10" x2="0" y2="10"/><line x1="-10" y1="0" x2="10" y2="0"/>
                <line x1="-6" y1="-6" x2="6" y2="6"/><line x1="6" y1="-6" x2="-6" y2="6"/>
              </g>
              <g transform="translate(60,300)" stroke="white" stroke-width="2" stroke-linecap="round" opacity="0.5">
                <line x1="0" y1="-8" x2="0" y2="8"/><line x1="-8" y1="0" x2="8" y2="0"/>
                <line x1="-5" y1="-5" x2="5" y2="5"/><line x1="5" y1="-5" x2="-5" y2="5"/>
              </g>
            </svg>
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

/* ── Map ── */
.why-v2__map-wrap {
  position: relative;
}

.why-v2__map-container {
  position: relative;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.18);
}

/* Canada Map actual image — Figma node 425:5692, natural 4:3 displayed stretched */
.why-v2__map-img {
  width: 100%;
  height: auto;
  display: block;
  object-fit: cover;
  object-position: bottom;  /* matches Figma object-bottom spec */
}

/* Hawaii inset — Figma node 425:5695, 172×129 display size */
.why-v2__hawaii-wrap {
  display: inline-block;
  border-radius: 4px;
  border: 1px solid rgba(255, 255, 255, 0.35);
  margin-top: 10px;
  overflow: hidden;
  width: 172px;
}

.why-v2__hawaii-img {
  width: 100%;
  height: auto;
  display: block;
}

/* ── Recruitment graphic SVG ── */
.why-v2__graphic-svg {
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

  .why-v2__col--map,
  .why-v2__col--leaders {
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
