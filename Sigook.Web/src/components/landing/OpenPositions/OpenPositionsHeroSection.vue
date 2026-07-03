<template>
  <section class="op-hero">
    <HeroBackground :image="heroImage" :image-sm="heroImageSm" focal="center 35%" />
    <DecoMagnifier class="op-hero__magnifier" />

    <div class="op-hero__content">
      <EyebrowPill variant="cyan" class="op-hero__eyebrow">
        Open Positions
      </EyebrowPill>

      <h1 class="op-hero__heading">
        Find your next <span class="op-hero__heading-accent">move.</span><br>
        Today's <span class="op-hero__heading-accent">openings.</span>
      </h1>

      <p class="op-hero__subtitle">
        Browse vetted roles across the United States — from skilled trades
        to professional positions. Updated weekly.
      </p>

      <form class="op-hero__search" @submit.prevent="onSearch">
        <div class="op-hero__field">
          <label class="op-hero__label" for="op-job-title">Job title</label>
          <input
            id="op-job-title"
            v-model="form.title"
            type="text"
            class="op-hero__input"
            placeholder="e.g. Forklift Operator"
            autocomplete="off"
          />
        </div>

        <div class="op-hero__field">
          <label class="op-hero__label" for="op-city">City</label>
          <input
            id="op-city"
            v-model="form.location"
            type="text"
            class="op-hero__input"
            placeholder="e.g. Miami, FL"
            autocomplete="off"
          />
        </div>

        <button type="submit" class="op-hero__submit">
          <span>Search</span>
          <span class="op-hero__submit-arrow" aria-hidden="true">→</span>
        </button>
      </form>
    </div>
  </section>
</template>

<script setup lang="ts">
import { reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import DecoMagnifier from '@/components/landing/shared/hero/DecoMagnifier.vue'
import HeroBackground from '@/components/landing/shared/hero/HeroBackground.vue'
import heroImage from '@/assets/images/landing/hero/open-positions.webp'
import heroImageSm from '@/assets/images/landing/hero/open-positions-960.webp'
import EyebrowPill from '@/components/landing/shared/ui/EyebrowPill.vue'
import { useJobs } from '@/composables/useJobs'

interface SearchForm {
  title: string
  location: string
}

const form = reactive<SearchForm>({
  title: '',
  location: '',
})

const route = useRoute()
const router = useRouter()
const { fetchJobs } = useJobs()

function readQueryParam(value: unknown): string {
  if (typeof value === 'string') return value
  if (Array.isArray(value)) return typeof value[0] === 'string' ? value[0] : ''
  return ''
}

onMounted(() => {
  form.title = readQueryParam(route.query.jobTitle)
  form.location = readQueryParam(route.query.location)
})

function onSearch(): void {
  const query: Record<string, string> = {}
  if (form.title) query.jobTitle = form.title
  if (form.location) query.location = form.location
  router.replace({ query })

  fetchJobs({
    jobTitle: form.title || undefined,
    location: form.location || undefined,
  })
  const target = document.getElementById('op-list')
  target?.scrollIntoView({ behavior: 'smooth', block: 'start' })
}
</script>

<style scoped>
.op-hero {
  position: relative;
  width: 100%;
  min-height: clamp(560px, 74vh, 780px);
  overflow: hidden;
  isolation: isolate;
}

.op-hero :deep(.hero-bg) {
  -webkit-mask-image: linear-gradient(180deg, #000 0%, #000 60%, transparent 92%);
  mask-image: linear-gradient(180deg, #000 0%, #000 60%, transparent 92%);
}

.op-hero :deep(.hero-bg__fade) {
  display: none;
}

.op-hero__magnifier {
  top: clamp(14%, 16vw, 22%);
  left: clamp(6%, 8vw, 12%);
}

.op-hero__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: clamp(560px, 74vh, 780px);
  padding:
    clamp(64px, 9vw, 104px)
    clamp(24px, 5vw, 40px)
    clamp(72px, 9vw, 120px);
  text-align: center;
  max-width: 1100px;
  margin: 0 auto;
  font-family: var(--font-family);
}

.op-hero__eyebrow {
  margin-bottom: clamp(20px, 3vw, 28px);
}

.op-hero__heading {
  font-size: var(--text-hero-size);
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 2vw, 20px);
  max-width: 920px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.op-hero__heading-accent {
  color: var(--c-brand-cyan);
}

.op-hero__subtitle {
  font-size: var(--text-hero-sub-size);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(24px, 3.2vw, 36px);
  max-width: 680px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

.op-hero__search {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: minmax(0, 1.4fr) minmax(0, 1.2fr) auto;
  gap: clamp(12px, 1.4vw, 16px);
  width: 100%;
  max-width: 1080px;
  padding: clamp(20px, 2.2vw, 28px);
  background: linear-gradient(180deg,
    rgba(255, 255, 255, 0.10) 0%,
    var(--c-glass-fill-soft) 100%);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid var(--c-glass-border);
  border-radius:
    clamp(20px, 2.5vw, 28px) clamp(20px, 2.5vw, 28px)
    clamp(20px, 2.5vw, 28px) clamp(40px, 5vw, 56px);
  box-shadow: 0 20px 48px rgba(0, 0, 0, 0.30);
  margin-bottom: 0;
  text-align: left;
}

.op-hero__field {
  display: flex;
  flex-direction: column;
  gap: 6px;
  min-width: 0;
}

.op-hero__label {
  font-size: clamp(10px, 0.85vw, 11px);
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

.op-hero__input {
  width: 100%;
  height: clamp(44px, 4.4vw, 50px);
  padding: 0 14px;
  background: var(--c-glass-fill);
  border: 1px solid var(--c-glass-border);
  border-radius: 12px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 500;
  outline: none;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    box-shadow 0.25s ease;
}

.op-hero__input::placeholder {
  color: rgba(255, 255, 255, 0.45);
}

.op-hero__input:hover {
  background: var(--c-glass-fill-strong);
  border-color: rgba(255, 255, 255, 0.40);
}

.op-hero__input:focus {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: var(--focus-ring-cyan);
}

.op-hero__submit {
  align-self: end;
  height: clamp(44px, 4.4vw, 50px);
  padding: 0 clamp(22px, 2.4vw, 28px);
  background: var(--c-brand-red);
  border: 1px solid var(--c-brand-red);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 700;
  letter-spacing: 0.04em;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  white-space: nowrap;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    transform 0.25s ease,
    box-shadow 0.25s ease;
}

.op-hero__submit:hover {
  background: #c92622;
  border-color: #c92622;
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(229, 45, 39, 0.35);
}

.op-hero__submit-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.op-hero__submit:hover .op-hero__submit-arrow {
  transform: translateX(3px);
}

@media (max-width: 1023px) {
  .op-hero {
    min-height: auto;
  }

  .op-hero__content {
    min-height: auto;
  }

  .op-hero__search {
    grid-template-columns: 1fr 1fr;
  }

  .op-hero__submit {
    grid-column: 1 / -1;
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 599px) {
  .op-hero__search { grid-template-columns: 1fr; }

}
</style>
