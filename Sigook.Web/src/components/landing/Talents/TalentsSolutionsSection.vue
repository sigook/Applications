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

const OPTIONS: readonly SolutionOption[] = [
  {
    key: 'direct-hiring',
    eyebrow: 'Long-Term Career',
    title: 'Direct Hiring',
    benefits: [
      'Permanent roles with competitive pay and full US benefits',
      'Join companies that invest in your development and long-term growth',
      'Build real career momentum with stability and a team behind you',
    ],
    ctaLabel: 'Browse direct hires',
    ctaTo: '/open-positions',
    variant: 'blue',
  },
  {
    key: 'temp-to-perm',
    eyebrow: 'Try Before You Commit',
    title: 'Temp to Perm',
    benefits: [
      'Start on a contract with a clear path to a permanent role',
      'Test the team and the day-to-day before committing long-term',
      'Convert to full-time once it\'s the right fit for both sides',
    ],
    ctaLabel: 'Browse temp-to-perm',
    ctaTo: '/open-positions',
    variant: 'cyan',
  },
  {
    key: 'contract',
    eyebrow: 'Flexibility & Reach',
    title: 'Contract',
    benefits: [
      'Pick up project-based roles across multiple industries and locations',
      'Grow your skillset faster by working in diverse environments',
      'No long-term commitment — work on your terms, at your pace',
    ],
    ctaLabel: 'Browse contracts',
    ctaTo: '/open-positions',
    variant: 'red',
  },
] as const
</script>
