export default {
  computed: {
    isDirectHiring(): boolean {
      return (this as any).request && (this as any).request.workerSalary;
    },
  },
};
