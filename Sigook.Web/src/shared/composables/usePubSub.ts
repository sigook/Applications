import { ref, computed, Ref, ComputedRef } from 'vue';

export interface PubSub {
  subscribers: Ref<string[]>;
  subscribe: (event: string) => void;
  unsubscribe: () => void;
  isLoadingFiles: ComputedRef<boolean>;
}

export function usePubSub(): PubSub {
  const subscribers = ref<string[]>([]);

  function subscribe(event: string): void {
    if (!(subscribers.value as unknown as Record<string, boolean>)[event]) {
      subscribers.value.push(event);
    }
  }

  function unsubscribe(): void {
    subscribers.value.splice(0, 1);
  }

  const isLoadingFiles = computed(() => subscribers.value.length !== 0);

  return { subscribers, subscribe, unsubscribe, isLoadingFiles };
}
