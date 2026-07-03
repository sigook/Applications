<template>
  <ValleyCardsSection
    eyebrow="Why Work With Us"
    heading="Three things"
    heading-accent="you'll feel from day one."
    subtitle="It's not just placement. It's a relationship — built on local presence, genuine care, and a recruiter who actually knows your industry."
    :reasons="REASONS"
  />
</template>

<script setup lang="ts">
import ValleyCardsSection, {
  type ValleyReason,
} from '@/components/landing/shared/sections/ValleyCardsSection.vue'

const REASONS: readonly ValleyReason[] = [
  {
    eyebrow: 'Local Reach',
    title: 'Working where you are',
    bullets: [
      'Roles available across major US states and metro areas',
      'Local recruiters who know your market and the employers in it',
      'Placements that grow into lasting careers, not just one-time gigs',
    ],
    variant: 'blue',
  },
  {
    eyebrow: 'Human-First',
    title: 'We care about you',
    bullets: [
      'We listen first — your goals shape every role we recommend',
      'A real person in your corner from first conversation to first day',
      "We don't stop at placement — we check in to make sure it's working",
    ],
    variant: 'cyan',
  },
  {
    eyebrow: 'Sector Expertise',
    title: 'Focused on your experience',
    bullets: [
      'Recruiters who specialize in skilled trades, industrial, and professional sectors',
      'Deep knowledge of what great looks like in your specific field',
      'Matched to roles where your skills are genuinely valued and put to use',
    ],
    variant: 'red',
  },
]
</script>
