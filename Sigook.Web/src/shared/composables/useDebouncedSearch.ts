import { onScopeDispose, ref, type Ref } from 'vue';
import { showAlertError } from '@/shared/utils/toast';

export const SEARCH_DEBOUNCE_MS = 300;
export const SEARCH_MIN_LENGTH = 3;

interface DebouncedSearchOptions {
  minLength?: number;
  delay?: number;
}

export function useDebouncedSearch<T>(
  fetch: (term: string) => Promise<T[]>,
  { minLength = SEARCH_MIN_LENGTH, delay = SEARCH_DEBOUNCE_MS }: DebouncedSearchOptions = {},
): { results: Ref<T[]>; isSearching: Ref<boolean>; search: (text: string | null | undefined) => void } {
  const results = ref<T[]>([]) as Ref<T[]>;
  const isSearching = ref(false);
  let timer: ReturnType<typeof setTimeout> | undefined;
  let latestRequest = 0;

  function search(text: string | null | undefined): void {
    clearTimeout(timer);
    const term = (text ?? '').trim();
    if (term.length < minLength) {
      latestRequest++;
      results.value = [];
      isSearching.value = false;
      return;
    }
    timer = setTimeout(() => { void run(term); }, delay);
  }

  async function run(term: string): Promise<void> {
    const request = ++latestRequest;
    isSearching.value = true;
    try {
      const response = await fetch(term);
      if (request === latestRequest) results.value = response;
    } catch (error) {
      if (request === latestRequest) showAlertError(error);
    } finally {
      if (request === latestRequest) isSearching.value = false;
    }
  }

  onScopeDispose(() => { clearTimeout(timer); });

  return { results, isSearching, search };
}
