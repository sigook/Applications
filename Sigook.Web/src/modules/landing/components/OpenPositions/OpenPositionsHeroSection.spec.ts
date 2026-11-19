import { describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import OpenPositionsHeroSection from '@/modules/landing/components/OpenPositions/OpenPositionsHeroSection.vue';
import { getJobs } from '@/modules/landing/api';
import { createTestRouter } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/api', () => ({ getJobs: vi.fn() }));

async function setup(path = '/open-positions') {
  const router = createTestRouter();
  await router.push(path);
  await router.isReady();
  const replace = vi.spyOn(router, 'replace');
  const wrapper = mount(OpenPositionsHeroSection, { attachTo: document.body, global: { plugins: [Buefy, router] } });
  await flushPromises();
  return { wrapper, replace };
}

type Wrapper = Awaited<ReturnType<typeof setup>>['wrapper'];

function value(wrapper: Wrapper, id: string) {
  return (wrapper.find(`#${id}`).element as HTMLInputElement).value;
}

describe('OpenPositionsHeroSection', () => {
  it('prefills the search from the url', async () => {
    const { wrapper } = await setup('/open-positions?jobTitle=Welder&location=Miami');

    expect(value(wrapper, 'op-job-title')).toBe('Welder');
    expect(value(wrapper, 'op-city')).toBe('Miami');
  });

  it('uses the first value of repeated query params', async () => {
    const { wrapper } = await setup('/open-positions?jobTitle=Welder&jobTitle=Driver');

    expect(value(wrapper, 'op-job-title')).toBe('Welder');
    expect(value(wrapper, 'op-city')).toBe('');
  });

  it('searches, updates the url and scrolls to the list', async () => {
    vi.mocked(getJobs).mockResolvedValue([]);
    const list = document.createElement('section');
    list.id = 'op-list';
    const scroll = vi.fn();
    list.scrollIntoView = scroll;
    document.body.appendChild(list);
    const { wrapper, replace } = await setup();
    await wrapper.find('#op-job-title').setValue('Forklift');
    await wrapper.find('#op-city').setValue('Tampa');
    await wrapper.find('form').trigger('submit');

    expect(replace).toHaveBeenCalledWith({ path: '/open-positions', query: { jobTitle: 'Forklift', location: 'Tampa' } });
    expect(getJobs).toHaveBeenCalledWith({ jobTitle: 'Forklift', location: 'Tampa' });
    expect(scroll).toHaveBeenCalledWith({ behavior: 'smooth', block: 'start' });
  });

  it('searches everything with an empty form', async () => {
    vi.mocked(getJobs).mockResolvedValue([]);
    const { wrapper, replace } = await setup();
    await wrapper.find('form').trigger('submit');

    expect(replace).toHaveBeenCalledWith({ path: '/open-positions', query: {} });
    expect(getJobs).toHaveBeenCalledWith({ jobTitle: undefined, location: undefined });
  });
});
