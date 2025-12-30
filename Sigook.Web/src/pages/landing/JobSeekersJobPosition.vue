<template>
  <JobPosition solutionType="jobSeekers"></JobPosition>
</template>


<script>

export default {
  components: {
    JobPosition: () => import("@/components/landing/JobPosition.vue")
  }
}

</script>