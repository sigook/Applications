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

const OPTIONS: readonly SolutionOption[] = [
  {
    key: 'direct-hiring',
    eyebrow: 'Long-Term Fit',
    title: 'Direct Hiring',
    body:
      'Find long-term professionals who align with your company\'s culture and goals — vetted candidates, structured search, full handoff to your team.',
    benefits: [
      'Culture-matched candidates',
      'End-to-end vetting',
      'Replacement guarantee',
    ],
    ctaLabel: 'Talk to a recruiter',
    ctaTo: '#employers-contact',
    variant: 'blue',
  },
  {
    key: 'temp-to-perm',
    eyebrow: 'Try Before You Hire',
    title: 'Temp to Perm',
    body:
      'Bring talent on as a contractor first and convert to a permanent hire once they\'ve proven the fit on the job — the lowest-risk path to a long-term addition to your team.',
    benefits: [
      'On-the-job evaluation',
      'Lower hiring risk',
      'Smooth conversion to permanent',
    ],
    ctaLabel: 'Explore temp-to-perm',
    ctaTo: '#employers-contact',
    variant: 'cyan',
  },
  {
    key: 'contract',
    eyebrow: 'Flexibility & Scale',
    title: 'Contract',
    body:
      'Access skilled talent quickly with flexible contracts tailored to project needs — ramp up for a peak, fill a gap, or pilot a role before going permanent.',
    benefits: [
      'Fast deployment',
      'Project-based pricing',
      'Easy contract-to-hire',
    ],
    ctaLabel: 'Request talent',
    ctaTo: '#employers-contact',
    variant: 'red',
  },
] as const
</script>
