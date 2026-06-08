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
          :delay="0"
        >
          <span class="numbers__num">+330</span>
          <span class="numbers__lbl">Clients Served</span>
        </SecondaryCard>

        <SecondaryCard
          variant="cyan"
          class="numbers__stat"
          :delay="140"
        >
          <span class="numbers__num">+1,700</span>
          <span class="numbers__lbl">Jobs Posted</span>
        </SecondaryCard>

        <SecondaryCard
          variant="red"
          class="numbers__stat"
          :delay="280"
        >
          <span class="numbers__num">+5,000</span>
          <span class="numbers__lbl">Applications</span>
        </SecondaryCard>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import SecondaryCard from '@/components/v2/landing/shared/SecondaryCard.vue'

const sectionRef = ref<HTMLElement | null>(null)
const visible = ref(false)
let observer: IntersectionObserver | null = null

onMounted(() => {
  if (!sectionRef.value) return
  observer = new IntersectionObserver(
    (entries) => { visible.value = entries[0].isIntersecting },
    { threshold: 0.15, rootMargin: '0px 0px -10% 0px' }
  )
  observer.observe(sectionRef.value)
})

onUnmounted(() => observer?.disconnect())
</script>

<style scoped>
/* ── Section shell — transparent (background lives in GlobalBackground) ─── */
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

/* ── Header ──────────────────────────────────────────────────────────────── */
.numbers__header {
  max-width: 720px;
  margin-bottom: 64px;
  opacity: 0;
  transform: translateY(20px);
  transition:
    opacity 0.7s ease-out,
    transform 0.7s cubic-bezier(0.22, 1, 0.36, 1);
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

/* ── Stat cards row — SecondaryCard instances laid out in a flex row ───── */
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
  /* Center-align content inside the card body for stat layout */
  text-align: center;
}

/* Number + label — typography lives at the section level so the stat
   slot stays content-agnostic in the canonical SecondaryCard. */
.numbers__stat :deep(.secondary-card__body) {
  display: flex;
  flex-direction: column;
  align-items: center;
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

/* ── Mobile (≤ 1023px) ───────────────────────────────────────────────────── */
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
