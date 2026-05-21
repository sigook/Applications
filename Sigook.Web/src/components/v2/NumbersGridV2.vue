<template>
  <div class="numbers-grid-v2">
    <div class="numbers-grid-v2__row">
      <div class="numbers-grid-v2__card numbers-grid-v2__card--red numbers-grid-v2__card--tl">
        <span class="numbers-grid-v2__num numbers-grid-v2__num--lg">+10</span>
        <span class="numbers-grid-v2__lbl">Years of<br />Experience</span>
      </div>
      <div class="numbers-grid-v2__card numbers-grid-v2__card--blue numbers-grid-v2__card--tr">
        <span class="numbers-grid-v2__num">+330</span>
        <span class="numbers-grid-v2__lbl">Clients<br />Served</span>
      </div>
    </div>
    <div class="numbers-grid-v2__row">
      <div class="numbers-grid-v2__card numbers-grid-v2__card--blue numbers-grid-v2__card--bl">
        <span class="numbers-grid-v2__num">+1,700</span>
        <span class="numbers-grid-v2__lbl">Jobs<br />Posted</span>
      </div>
      <div class="numbers-grid-v2__card numbers-grid-v2__card--red numbers-grid-v2__card--br">
        <span class="numbers-grid-v2__num">+5,000</span>
        <span class="numbers-grid-v2__lbl">Applications<br />Filed</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Decorative numbers grid — 2×2 cards with asymmetric pill corners.
// Figma node: 2374:670 (Mobile NumbersGrid 360×381)
</script>

<style scoped>
/* Figma mobile: 360×381 — 2×2 grid, gap 20px between rows & cols */
.numbers-grid-v2 {
  width: 360px;
  max-width: 100%;
  height: 381px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  font-family: var(--font-family);
}

.numbers-grid-v2__row {
  display: flex;
  gap: 20px;
  flex: 1 0 0;
  min-height: 0;
}

.numbers-grid-v2__card {
  flex: 1 0 0;
  min-width: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 16px 12px;
  color: #fff;
  text-align: center;
}

.numbers-grid-v2__card--red { background-color: var(--c-brand-red); }
.numbers-grid-v2__card--blue { background-color: var(--c-brand-blue); }

/* Asymmetric pill corners — one corner sharp (the one pointing inward),
   three rounded — creates the "petal" cluster effect */
.numbers-grid-v2__card--tl { border-radius: 300px 300px 0 300px; }
.numbers-grid-v2__card--tr { border-radius: 300px 300px 300px 0; }
.numbers-grid-v2__card--bl { border-radius: 300px 0 300px 300px; }
.numbers-grid-v2__card--br { border-radius: 0 300px 300px 300px; }

.numbers-grid-v2__num {
  font-size: 36px;
  font-weight: 700;
  line-height: 1.1;
  white-space: nowrap;
}

.numbers-grid-v2__num--lg {
  font-size: 42px;
}

.numbers-grid-v2__lbl {
  font-size: 14px;
  font-weight: 400;
  line-height: 1.2;
}

/* ============================================================
   Desktop (≥1024px) — Figma node 2381:14 (834×832)
   ============================================================ */
@media (min-width: 1024px) {
  .numbers-grid-v2 {
    width: 834px;
    height: 832px;
    gap: 46px; /* row gap */
  }

  .numbers-grid-v2__row {
    gap: 48px; /* col gap */
  }

  /* Border-radius scales up to 300px per Figma (already 300, but make sure) */
  .numbers-grid-v2__card--tl { border-radius: 300px 300px 0 300px; }
  .numbers-grid-v2__card--tr { border-radius: 300px 300px 300px 0; }
  .numbers-grid-v2__card--bl { border-radius: 300px 0 300px 300px; }
  .numbers-grid-v2__card--br { border-radius: 0 300px 300px 300px; }

  .numbers-grid-v2__num {
    font-size: 60px;
  }

  .numbers-grid-v2__num--lg {
    font-size: 80px;
  }

  .numbers-grid-v2__lbl {
    font-size: 20px;
  }
}
</style>
