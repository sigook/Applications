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
import employersReasonsData from '@/data/landing/employersReasons.json'

const REASONS = employersReasonsData as readonly ValleyReason[]
</script>
