export default {
  methods: {
    workerColor(approvedToWork: boolean, isSubcontractor: boolean): string | undefined {
      if (approvedToWork === false) {
        return "Rejected";
      } else if (isSubcontractor) {
        return "Blue";
      }
    },
  },
};
