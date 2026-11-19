import { beforeEach, describe, expect, it, vi } from 'vitest';
import { useJobs } from '@/modules/landing/composables/useJobs';
import { getJobs } from '@/modules/landing/api';
import type { JobViewModel } from '@/modules/landing/types';

vi.mock('@/modules/landing/api', () => ({ getJobs: vi.fn() }));

function job(id: string): JobViewModel {
  return {
    id,
    requestId: `request-${id}`,
    numberId: id,
    title: `Job ${id}`,
    salary: '$20.00',
    location: 'Toronto, ON',
    type: 'Full time',
    createdAt: '2026-11-01T00:00:00Z',
    description: '',
    requirements: '',
    responsibilities: '',
    shift: 'Day',
    createdBy: 'Agency',
    agencyId: null,
  };
}

function deferred<T>() {
  let resolve!: (value: T) => void;
  let reject!: (reason: unknown) => void;
  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });
  return { promise, resolve, reject };
}

describe('useJobs', () => {
  beforeEach(async () => {
    vi.mocked(getJobs).mockResolvedValue([]);
    await useJobs().fetchJobs();
    vi.mocked(getJobs).mockReset();
  });

  it('loads jobs with the given filter and toggles loading', async () => {
    const pending = deferred<JobViewModel[]>();
    vi.mocked(getJobs).mockReturnValue(pending.promise);
    const { jobs, loading, hasJobs, jobsCount, fetchJobs } = useJobs();

    const request = fetchJobs({ jobTitle: 'Driver' });
    expect(getJobs).toHaveBeenCalledWith({ jobTitle: 'Driver' });
    expect(loading.value).toBe(true);

    pending.resolve([job('1'), job('2')]);
    await request;
    expect(loading.value).toBe(false);
    expect(jobs.value.map((j) => j.id)).toEqual(['1', '2']);
    expect(hasJobs.value).toBe(true);
    expect(jobsCount.value).toBe(2);
  });

  it('sends an empty filter by default', async () => {
    vi.mocked(getJobs).mockResolvedValue([]);
    const { fetchJobs, hasJobs } = useJobs();
    await fetchJobs();
    expect(getJobs).toHaveBeenCalledWith({});
    expect(hasJobs.value).toBe(false);
  });

  it('sets an error and clears jobs when the request fails', async () => {
    vi.mocked(getJobs).mockResolvedValueOnce([job('1')]).mockRejectedValueOnce(new Error('boom'));
    const { jobs, error, loading, fetchJobs } = useJobs();
    await fetchJobs();
    await fetchJobs();
    expect(error.value).toBe('We could not load the open positions. Please try again.');
    expect(jobs.value).toEqual([]);
    expect(loading.value).toBe(false);
  });

  it('clears a previous error when fetching again and via clearError', async () => {
    vi.mocked(getJobs).mockRejectedValueOnce(new Error('boom')).mockResolvedValueOnce([]);
    const { error, fetchJobs, clearError } = useJobs();
    await fetchJobs();
    expect(error.value).not.toBeNull();
    const request = fetchJobs();
    expect(error.value).toBeNull();
    await request;

    vi.mocked(getJobs).mockRejectedValueOnce(new Error('boom'));
    await fetchJobs();
    clearError();
    expect(error.value).toBeNull();
  });

  it('ignores a stale success that resolves after a newer request', async () => {
    const first = deferred<JobViewModel[]>();
    const second = deferred<JobViewModel[]>();
    vi.mocked(getJobs).mockReturnValueOnce(first.promise).mockReturnValueOnce(second.promise);
    const { jobs, loading, fetchJobs } = useJobs();

    const firstRequest = fetchJobs({ jobTitle: 'a' });
    const secondRequest = fetchJobs({ jobTitle: 'b' });
    first.resolve([job('stale')]);
    await firstRequest;
    expect(loading.value).toBe(true);
    expect(jobs.value).toEqual([]);

    second.resolve([job('fresh')]);
    await secondRequest;
    expect(jobs.value.map((j) => j.id)).toEqual(['fresh']);
    expect(loading.value).toBe(false);
  });

  it('ignores a stale failure that rejects after a newer request', async () => {
    const first = deferred<JobViewModel[]>();
    vi.mocked(getJobs).mockReturnValueOnce(first.promise).mockResolvedValueOnce([job('fresh')]);
    const { jobs, error, fetchJobs } = useJobs();

    const firstRequest = fetchJobs();
    await fetchJobs();
    first.reject(new Error('late'));
    await firstRequest;

    expect(error.value).toBeNull();
    expect(jobs.value.map((j) => j.id)).toEqual(['fresh']);
  });

  it('shares state between callers', async () => {
    vi.mocked(getJobs).mockResolvedValue([job('1')]);
    await useJobs().fetchJobs();
    expect(useJobs().jobsCount.value).toBe(1);
  });
});
