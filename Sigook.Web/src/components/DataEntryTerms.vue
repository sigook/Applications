<template>
  <div>
    <u class="color-primary" @click="expandOrColapse">Data Entry Terms</u>
    <div v-if="expanded">
      <br />
      <div v-if="isCompanyProfile">
        <h1 class="capitalize fz1 fw-700">Business Owner Data Entry</h1>
        <p>
          If you are a Business Owner, by clicking the submit button, you
          acknowledge your receipt and approval of the time report. You
          understand that a misstatement of fact may cause SIGOOK™ to
          discontinue providing services or may result in the removal of your
          account from the System.
        </p>
      </div>
      <div v-else-if="isWorkerProfile">
        <h1 class="capitalize fz1 fw-700">Worker Data Entry</h1>
        <p>
          If you are a Worker, you certify that the days and hours entered
          on the time report through the System are correct, complete, and were
          worked by you. You understand that a misstatement of fact may cause
          you to lose your employment or contract with SIGOOK™ or may result in
          the removal of your account from the System.
        </p>
      </div>
      <h1 class="capitalize fz1 fw-700">Data Entry Terms</h1>
        <p>
          By submitting your hours, comments, data and/or approval of time
          report information to the System (the "Time Report Information"), you
          hereby grant SIGOOK™ a perpetual, non-exclusive, irrevocable,
          royalty-free, worldwide license and right to use, copy, modify,
          display, distribute, download, store, reproduce, transmit, publish,
          transfer, adapt, create derivative works in any manner, by any means,
          in entirety or a portion of, of your Time Report Information.
        </p>
        <p>
          You acknowledge and accept that SIGOOK™ will share the Time Report
          Information with third parties for SIGOOK™’s business purposes,
          including but not limited to SIGOOK™’s business clients, advisors,
          agents, and consultants. SIGOOK™ will not sell your Time Report
          Information to any third party for marketing purposes.
        </p>
    </div>
  </div>
</template>
<script>

import roles from "@/security/roles";

export default {
  data() {
    return {
      expanded: false,
    };
  },
  methods: {
    expandOrColapse() {
      this.expanded = !this.expanded;
    },
  },
  computed: {
    isCompanyProfile() {
      return this.$store.state.security.userRoles.some((ur) =>
        ur.includes(roles.company)
      );
    },
    isWorkerProfile() {
      return this.$store.state.security.userRoles.some((ur) =>
        ur.includes(roles.worker)
      );
    },
  },
};
</script>
<style lang="scss" scoped>
u {
  cursor: pointer;
}
</style>