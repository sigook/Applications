import roles from "@/security/roles";

export default {
  computed: {
    isPayrollManager(): boolean {
      return (this as any).$store.state.security.userRoles.some((ur: string) => ur === roles.admin || ur === roles.payroll || ur === roles.agency);
    }
  },
};
