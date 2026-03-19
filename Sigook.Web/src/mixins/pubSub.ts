export default {
  data() {
    return {
      subscribers: [] as string[],
    };
  },
  methods: {
    subscribe(event: string): void {
      if (!(this as any).subscribers[event]) {
        (this as any).subscribers.push(event);
      }
    },
    unsubscribe(): void {
      (this as any).subscribers.splice(0, 1);
    },
  },
  computed: {
    isLoadingFiles(): boolean {
      return (this as any).subscribers.length !== 0;
    },
  },
};
