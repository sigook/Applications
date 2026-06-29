<template>
  <SolutionsSection
    section-id="talents-solutions"
    eyebrow="Your Career, Your Way"
    heading="Three paths."
    heading-accent="One commitment to your growth."
    subtitle="Whether you're after stability or variety, we'll match you with roles that keep moving your career forward."
    :options="OPTIONS"
  />
</template>

<script setup lang="ts">
import SolutionsSection, { type SolutionOption } from '@/components/landing/shared/SolutionsSection.vue'
import talentsSolutionsData from '@/data/landing/talentsSolutions.json'

const OPTIONS = talentsSolutionsData as readonly SolutionOption[]
</script>
