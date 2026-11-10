import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { effectScope } from 'vue';
import { useDebouncedSearch, SEARCH_DEBOUNCE_MS } from '@/shared/composables/useDebouncedSearch';
import { showAlertError } from '@/shared/utils/toast';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

function deferred<T>() {
  let resolve!: (value: T) => void;
  let reject!: (reason: unknown) => void;
  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });
  return { promise, resolve, reject };
}

function setup<T>(fetch: (term: string) => Promise<T[]>) {
  const scope = effectScope();
  const api = scope.run(() => useDebouncedSearch(fetch));
  if (!api) throw new Error('Scope did not run');
  return { ...api, scope };
}

describe('useDebouncedSearch', () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('waits for the debounce delay before fetching the trimmed term', async () => {
    const fetch = vi.fn().mockResolvedValue(['Toronto']);
    const { search, results, isSearching } = setup(fetch);

    search('  tor  ');
    expect(fetch).not.toHaveBeenCalled();

    vi.advanceTimersByTime(SEARCH_DEBOUNCE_MS);
    expect(fetch).toHaveBeenCalledWith('tor');
    expect(isSearching.value).toBe(true);

    await vi.runAllTimersAsync();
    expect(results.value).toEqual(['Toronto']);
    expect(isSearching.value).toBe(false);
  });

  it('only fetches the last term typed within the delay', () => {
    const fetch = vi.fn().mockResolvedValue([]);
    const { search } = setup(fetch);

    search('tor');
    vi.advanceTimersByTime(100);
    search('toro');
    vi.advanceTimersByTime(SEARCH_DEBOUNCE_MS);

    expect(fetch).toHaveBeenCalledTimes(1);
    expect(fetch).toHaveBeenCalledWith('toro');
  });

  it('clears results without fetching when the term is too short', async () => {
    const fetch = vi.fn().mockResolvedValue(['Toronto']);
    const { search, results } = setup(fetch);

    search('toronto');
    await vi.runAllTimersAsync();
    expect(results.value).toEqual(['Toronto']);

    search('to');
    vi.advanceTimersByTime(SEARCH_DEBOUNCE_MS);
    expect(fetch).toHaveBeenCalledTimes(1);
    expect(results.value).toEqual([]);
  });

  it('ignores responses from stale requests', async () => {
    const first = deferred<string[]>();
    const second = deferred<string[]>();
    const fetch = vi.fn().mockReturnValueOnce(first.promise).mockReturnValueOnce(second.promise);
    const { search, results } = setup(fetch);

    search('tor');
    vi.advanceTimersByTime(SEARCH_DEBOUNCE_MS);
    search('toronto');
    vi.advanceTimersByTime(SEARCH_DEBOUNCE_MS);

    second.resolve(['Toronto']);
    await vi.runAllTimersAsync();
    first.resolve(['Tory']);
    await vi.runAllTimersAsync();

    expect(results.value).toEqual(['Toronto']);
  });

  it('shows an error toast when the fetch fails', async () => {
    const error = new Error('boom');
    const { search, isSearching } = setup(vi.fn().mockRejectedValue(error));

    search('toronto');
    await vi.runAllTimersAsync();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(isSearching.value).toBe(false);
  });

  it('cancels the pending fetch when the scope is disposed', () => {
    const fetch = vi.fn().mockResolvedValue([]);
    const { search, scope } = setup(fetch);

    search('toronto');
    scope.stop();
    vi.advanceTimersByTime(SEARCH_DEBOUNCE_MS);

    expect(fetch).not.toHaveBeenCalled();
  });
});
