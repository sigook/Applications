<template>
  <div class="employers-page">
    <MainNavbar />

    <TalentsMainSection />

    <WhyWork />

    <RolesTalents />

    <MainFooter />
  </div>
</template>

<script setup lang="ts">
import { useSeoMeta } from '@unhead/vue'
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import RolesTalents from '@/components/talents/RolesTalents.vue'
import WhyWork from '@/components/talents/WhyWork.vue'
import TalentsMainSection from '@/components/talents/TalentsMainSection.vue'

useSeoMeta({
  title: 'For Talents - Covenant Group',
  description: 'Looking for work? Join Covenant Group to access job opportunities in skilled trades, industrial, and professional sectors across Canada.',
  ogTitle: 'For Talents - Covenant Group',
  ogDescription: 'Access job opportunities in skilled trades, industrial, and professional sectors across Canada.',
})
</script>

<style scoped>
.employers-page {
  background-color: #ffffff;
  min-height: 100vh;
}
</style>
