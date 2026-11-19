import { describe, expect, it } from 'vitest';
import JobHeaderLocation from '@/modules/worker/requests/components/JobHeaderLocation.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerRequestDetail } from '@/modules/worker/requests/types';
import type { LocationDetailModel } from '@/shared/types/common';

function setup(request: Partial<WorkerRequestDetail>) {
  return mountWithPlugins(JobHeaderLocation, { props: { request: { id: 'r1', ...request } } });
}

function lines(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.job-header-location__line').map((l) => l.text());
}

describe('JobHeaderLocation', () => {
  it('shows the address and the coordinates', () => {
    const wrapper = setup({
      location: '1 King St',
      entrance: 'Door 4',
      jobLocation: { latitude: 43.6532, longitude: -79.3832 } as LocationDetailModel,
    });
    expect(lines(wrapper)).toEqual(['1 King St - Door 4', '43.653200, -79.383200']);
  });

  it('hides what is missing', () => {
    expect(lines(setup({ location: '1 King St', jobLocation: null }))).toEqual(['1 King St']);
    expect(lines(setup({ jobLocation: { latitude: 43.6 } as LocationDetailModel }))).toEqual([]);
  });
});
