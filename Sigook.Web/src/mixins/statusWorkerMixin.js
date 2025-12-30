export default {
  methods: {
    workerColor(approvedToWork, isSubcontractor) {
      if (approvedToWork === false) {
        return "Rejected";
      } else if (isSubcontractor) {
        return "Blue";
      }
    },
  },
};
