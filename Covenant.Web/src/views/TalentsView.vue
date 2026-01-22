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
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import RolesTalents from '@/components/talents/RolesTalents.vue'
import WhyWork from '@/components/talents/WhyWork.vue'
import TalentsMainSection from '@/components/talents/TalentsMainSection.vue'
</script>

<style scoped>
.employers-page {
  background-color: #ffffff;
  min-height: 100vh;
}
</style>
