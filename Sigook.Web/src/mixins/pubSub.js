export default {
  data() {
    return {
      subscribers: [],
    };
  },
  methods: {
    subscribe(event) {
      if (!this.subscribers[event]) {
        this.subscribers.push(event);
      }
    },
    unsubscribe() {
      this.subscribers.splice(0, 1);
    },
  },
  computed: {
    isLoadingFiles() {
      return this.subscribers.length !== 0;
    },
  },
};
