<template>
  <JobPosition solutionType="jobSeekers"></JobPosition>
</template>


<script lang="ts">

export default {
  components: {
    JobPosition: () => import("@/components/landing/JobPosition.vue")
  }
}

</script>