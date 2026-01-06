import roles from "@/security/roles";

export default {
  computed: {
    isPayrollManager() {
      return this.$store.state.security.userRoles.some(ur => ur === roles.admin || ur === roles.payroll || ur === roles.agency);
    }
  },
};
