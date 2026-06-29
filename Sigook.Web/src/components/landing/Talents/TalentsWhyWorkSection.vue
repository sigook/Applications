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
} from '@/components/landing/shared/ValleyCardsSection.vue'
import talentsReasonsData from '@/data/landing/talentsReasons.json'

const REASONS = talentsReasonsData as readonly ValleyReason[]
</script>
