<template>
  <ValleyCardsSection
    eyebrow="Why Employers Choose Us"
    heading="Three reasons"
    heading-accent="your shortlist arrives faster."
    subtitle="It's not just placement. It's a partnership — built on speed, sector expertise, and a network that took years to grow."
    :reasons="REASONS"
  />
</template>

<script setup lang="ts">
import ValleyCardsSection, {
  type ValleyReason,
} from '@/components/landing/shared/ValleyCardsSection.vue'

const REASONS: readonly ValleyReason[] = [
  {
    eyebrow: 'Speed to Hire',
    title: 'Shortlists in days, not months',
    body:
      'We tap an already-vetted bench of candidates the moment your role opens — most engagements deliver a qualified shortlist inside a week, not the industry-standard quarter.',
    variant: 'blue',
  },
  {
    eyebrow: 'Sector Expertise',
    title: 'Recruiters who speak your language',
    body:
      'Our recruiters specialize by industry — they know each sector\'s titles, salary bands, and what excellent looks like. You won\'t spend the kickoff explaining your own job description.',
    variant: 'cyan',
  },
  {
    eyebrow: 'Talent Network',
    title: 'A bench you didn\'t have to build',
    body:
      'Years of relationships across professional and industrial roles means the people you need are usually one phone call away — including the passive candidates job boards never reach.',
    variant: 'red',
  },
] as const
</script>
