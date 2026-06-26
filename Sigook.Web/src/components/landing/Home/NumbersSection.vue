<template>
  <section
    ref="sectionRef"
    class="numbers"
    :class="{ 'is-visible': visible }"
  >
    <div class="numbers__canvas">
      <header class="numbers__header">
        <span class="numbers__eyebrow">By the Numbers</span>
        <h2 class="numbers__heading">Our impact, at scale</h2>
        <p class="numbers__sub">
          Real results from real partnerships across the United States.
        </p>
      </header>

      <div class="numbers__stats">
        <SecondaryCard
          variant="blue"
          class="numbers__stat"
          eyebrow="Network"
          :delay="0"
        >
          <span class="numbers__num">+650</span>
          <span class="numbers__lbl">Clients Served</span>
        </SecondaryCard>

        <SecondaryCard
          variant="cyan"
          class="numbers__stat"
          eyebrow="Connections"
          :delay="140"
        >
          <span class="numbers__num">+2,000</span>
          <span class="numbers__lbl">Jobs Posted</span>
        </SecondaryCard>

        <SecondaryCard
          variant="red"
          class="numbers__stat"
          eyebrow="Engagement"
          :delay="280"
        >
          <span class="numbers__num">+15,000</span>
          <span class="numbers__lbl">Applications</span>
        </SecondaryCard>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { useRevealOnScroll } from '@/composables/useRevealOnScroll'
import SecondaryCard from '@/components/landing/shared/SecondaryCard.vue'

const { el: sectionRef, visible } = useRevealOnScroll()
</script>

<style scoped>
.numbers {
  position: relative;
  margin-top: -177px;
  width: 100%;
  height: 1351px;
  overflow: hidden;
  isolation: isolate;
}

.numbers__canvas {
  position: relative;
  max-width: 1280px;
  width: 100%;
  height: 100%;
  margin: 0 auto;
  padding: 240px 40px 320px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  text-align: center;
}

.numbers__header {
  max-width: 720px;
  margin-bottom: 64px;
  opacity: 0;
  transform: translateY(20px);
  transition:
    opacity 0.7s ease-out,
    transform 0.7s var(--ease-brand);
}

.numbers.is-visible .numbers__header {
  opacity: 1;
  transform: translateY(0);
}

.numbers__eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 16px;
}

.numbers__heading {
  font-family: var(--font-family);
  font-size: 52px;
  font-weight: 700;
  line-height: 1.1;
  color: #fff;
  margin: 0 0 16px;
  letter-spacing: -0.015em;
}

.numbers__sub {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
}

.numbers__stats {
  display: flex;
  gap: 32px;
  justify-content: center;
  align-items: stretch;
  flex-wrap: wrap;
  width: 100%;
}

.numbers__stat {
  width: 280px;
  text-align: left;
}

.numbers__stat :deep(.secondary-card__body) {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 12px;
}

.numbers__num {
  display: block;
  font-family: var(--font-family);
  font-size: 64px;
  font-weight: 700;
  line-height: 1;
  color: #fff;
  letter-spacing: -0.02em;
}

.numbers__lbl {
  display: block;
  font-family: var(--font-family);
  font-size: 14px;
  font-weight: 600;
  line-height: 1.4;
  color: rgba(255, 255, 255, 0.85);
  letter-spacing: 0.10em;
  text-transform: uppercase;
}

@media (max-width: 1023px) {
  .numbers {
    height: 1280px;
    margin-top: -100px;
  }

  .numbers__canvas {
    padding: 160px 24px 160px;
  }

  .numbers__header {
    margin-bottom: 40px;
  }

  .numbers__heading {
    font-size: 32px;
  }

  .numbers__sub {
    font-size: 15px;
  }

  .numbers__stats {
    flex-direction: column;
    gap: 20px;
    align-items: center;
  }

  .numbers__stat {
    width: 100%;
    max-width: 340px;
  }

  .numbers__num {
    font-size: 48px;
  }

  .numbers__lbl {
    font-size: 12px;
  }
}
</style>
