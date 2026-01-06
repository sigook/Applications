export default {
  computed: {
    isDirectHiring() {
      return this.request && this.request.workerSalary;
    },
  },
};
