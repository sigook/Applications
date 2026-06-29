<template>
  <SolutionsSection
    section-id="employers-solutions"
    eyebrow="Professional Hiring Options"
    heading="Three ways to hire."
    heading-accent="One commitment to fit."
    subtitle="Build a permanent team or scale on demand — choose the model that matches your timeline, budget, and growth plan."
    :options="OPTIONS"
  />
</template>

<script setup lang="ts">
import SolutionsSection, { type SolutionOption } from '@/components/landing/shared/SolutionsSection.vue'
import employersSolutionsData from '@/data/landing/employersSolutions.json'

const OPTIONS = employersSolutionsData as readonly SolutionOption[]
</script>
