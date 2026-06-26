<template>
  <div class="social-links">
    <a
      v-for="social in socials"
      :key="social.label"
      :href="social.href"
      class="social-links__item"
      target="_blank"
      rel="noopener noreferrer"
      :aria-label="`Sigook on ${social.label}`"
    >
      <LandingIcon :name="social.icon" />
    </a>
  </div>
</template>

<script setup lang="ts">
import LandingIcon from '@/components/landing/shared/LandingIcon.vue'
import type { LandingIconName } from '@/components/landing/shared/icons'

interface Social {
  label: string
  href: string
  icon: LandingIconName
}

const socials: Social[] = [
  {
    label: 'Instagram',
    href: 'https://www.instagram.com/sigook_work_factory/',
    icon: 'instagram',
  },
  {
    label: 'Facebook',
    href: 'https://www.facebook.com/sigook.work.factory/',
    icon: 'facebook',
  },
  {
    label: 'LinkedIn',
    href: 'https://www.linkedin.com/company/sigookworkfactory/',
    icon: 'linkedin',
  },
]
</script>

<style scoped>
.social-links {
  display: flex;
  align-items: center;
  gap: 12px;
}

.social-links__item {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.18);
  color: rgba(255, 255, 255, 0.80);
  transition:
    color 0.25s ease,
    background 0.25s ease,
    border-color 0.25s ease,
    transform 0.25s ease;
}

.social-links__item:hover {
  color: var(--c-brand-navy);
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  transform: translateY(-2px);
}

.social-links__item svg {
  width: 18px;
  height: 18px;
}
</style>
