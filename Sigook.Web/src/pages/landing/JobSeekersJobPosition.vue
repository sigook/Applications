<template>
  <JobPosition solutionType="jobSeekers"></JobPosition>
</template>


<script lang="ts">

import { defineAsyncComponent } from 'vue';
export default {
  components: {
    JobPosition: defineAsyncComponent(() => import("@/components/landing/JobPosition.vue"))
  }
}

</script>