<template>
  <div class="dual-cta-wrap">
    <div class="dual-cta__back" aria-hidden="true"></div>
    <section class="dual-cta" aria-label="Find work or find talent">
    <!-- Full-bleed background layers (fill the entire section, any width) -->
    <div class="dual-cta__bg" aria-hidden="true">
      <img :src="talentsPhoto" alt="" class="dual-cta__bg-img dual-cta__bg-img--left" loading="lazy" decoding="async" />
      <img :src="employersPhoto" alt="" class="dual-cta__bg-img dual-cta__bg-img--right" loading="lazy" decoding="async" />
    </div>

    <div class="dual-cta__veil" aria-hidden="true"></div>
    <div class="dual-cta__glow" aria-hidden="true"></div>

    <!-- Inner canvas: max-width centered, holds the cards only -->
    <div class="dual-cta__canvas">
      <PrimaryCard
        variant="navy"
        eyebrow="For Talents"
        title="Find Work"
        :show-divider="true"
        :list="talentBenefits"
        class="dual-cta__card dual-cta__card--work"
        tabindex="0"
      >
        <template #button>
          <ArrowPillCta to="/open-positions" hover-variant="navy">Browse Jobs</ArrowPillCta>
        </template>
      </PrimaryCard>

      <PrimaryCard
        variant="red"
        eyebrow="For Employers"
        title="Find Talent"
        :show-divider="true"
        :list="employerBenefits"
        class="dual-cta__card dual-cta__card--talent"
        tabindex="0"
      >
        <template #button>
          <ArrowPillCta to="#home-contact" hover-variant="red">Contact Us</ArrowPillCta>
        </template>
      </PrimaryCard>
    </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import ArrowPillCta from '@/components/landing/shared/ArrowPillCta.vue'
import PrimaryCard from '@/components/landing/shared/PrimaryCard.vue'
import talentsPhoto from '@/assets/images/v2/audience-banner/talents-worker.webp'
import employersPhoto from '@/assets/images/v2/audience-banner/employers-office.webp'

const talentBenefits: string[] = [
  'Access hundreds of jobs in skilled trades, industrial & professional fields',
  'Get matched with temporary, contract, or permanent roles that fit your skills',
  'Work across Canada and the US with top employers',
]

const employerBenefits: string[] = [
  'Hire pre-screened workers for temporary, contract, or permanent positions',
  'Fill roles fast in skilled trades, industrial, and professional sectors',
  'Trusted by leading employers across Canada and the US',
]
</script>

<style scoped>
.dual-cta-wrap {
  position: relative;
  width: 100%;
  margin-top: -140px;
  z-index: 5;
}

.dual-cta__back {
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: 0;
  border-radius: 150px 0 150px 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: var(--glass-blur-soft);
  -webkit-backdrop-filter: var(--glass-blur-soft);
  border: 1px solid var(--c-glass-border-soft);
  box-shadow: var(--sh-back);
  pointer-events: none;
}

.dual-cta {
  position: relative;
  width: 100%;
  height: 790px;
  z-index: 1;
  overflow: hidden;
  isolation: isolate;
  border-radius: 150px 0 150px 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
}

.dual-cta__canvas {
  position: relative;
  max-width: 1440px;
  width: 100%;
  height: 100%;
  margin: 0 auto;
}

.dual-cta__bg {
  position: absolute;
  inset: 0;
  display: flex;
  z-index: 1;
  overflow: hidden;
}

.dual-cta__bg-img {
  width: 50%;
  height: 100%;
  object-fit: cover;
  filter: brightness(0.62) saturate(0.92);
}

.dual-cta__bg-img--left { object-position: center 22%; }
.dual-cta__bg-img--right { object-position: center center; }

.dual-cta__veil {
  position: absolute;
  inset: 0;
  z-index: 2;
  background:
    linear-gradient(
      90deg,
      rgba(15, 47, 68, 0.55) 0%,
      rgba(15, 47, 68, 0.10) 50%,
      rgba(15, 47, 68, 0.55) 100%
    ),
    linear-gradient(
      180deg,
      rgba(15, 47, 68, 0.55) 0%,
      rgba(15, 47, 68, 0.18) 40%,
      rgba(15, 47, 68, 0.55) 100%
    );
}

.dual-cta__glow {
  position: absolute;
  z-index: 2;
  pointer-events: none;
  width: 420px;
  height: 420px;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  background: var(--c-brand-cyan);
  border-radius: 50%;
  filter: blur(120px);
  opacity: 0.32;
}

.dual-cta__card {
  position: absolute;
  width: 560px;
  z-index: 3;
  outline: none;
  cursor: pointer;
}

.dual-cta__card--work {
  left: calc(50% - 500px);
  top: 180px;
}

.dual-cta__card--talent {
  left: calc(50% - 60px);
  top: 80px;
}

.dual-cta__card--work.primary-card {
  padding-right: 130px;
}

@media (max-width: 1023px) {
  .dual-cta-wrap {
    margin-top: -100px;
  }

  .dual-cta__back {
    display: none;
  }

  .dual-cta {
    height: auto;
    padding: 90px 20px 110px;
    border-radius: 80px 0 80px 0;
    box-shadow:
      0 -22px 24px -12px rgba(0, 0, 0, 0.45),
      0  22px 24px -12px rgba(0, 0, 0, 0.45);
  }

  .dual-cta__canvas {
    height: auto;
    display: flex;
    flex-direction: column;
    gap: 28px;
    align-items: center;
  }

  .dual-cta__bg { flex-direction: column; }
  .dual-cta__bg-img { width: 100%; height: 50%; }

  .dual-cta__glow {
    width: 280px;
    height: 280px;
    filter: blur(40px);
  }

  .dual-cta__card {
    position: relative;
    width: 100%;
    max-width: 480px;
    left: auto;
    right: auto;
    top: auto;
  }

  .dual-cta__card--work.primary-card {
    padding-right: clamp(32px, 4vw, 56px);
  }
}
</style>
