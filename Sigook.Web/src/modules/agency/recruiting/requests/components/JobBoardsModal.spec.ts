import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import JobBoardsModal from '@/modules/agency/recruiting/requests/components/JobBoardsModal.vue';
import { setAgencyRequestSources } from '@/modules/agency/recruiting/requests/api';
import { getSourcesForRequests } from '@/shared/api/catalogApi';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestJobBoard } from '@/modules/agency/recruiting/requests/types';
import type { Source } from '@/shared/types/common';

vi.mock('@/modules/agency/recruiting/requests/api', () => ({ setAgencyRequestSources: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({ getSourcesForRequests: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const indeed: Source = { id: 'src-1', value: 'Indeed' };
const kijiji: Source = { id: 'src-2', value: 'Kijiji' };

async function setup(currentBoards: RequestJobBoard[] = [], sources: Source[] = [indeed, kijiji]) {
  vi.mocked(getSourcesForRequests).mockResolvedValue(sources);
  const wrapper = mountWithPlugins(JobBoardsModal, { props: { requestId: 'req-1', numberId: 42, currentBoards } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('.board-row');
}

async function save(wrapper: Wrapper) {
  await wrapper.find('footer button').trigger('click');
  await flushPromises();
}

describe('JobBoardsModal', () => {
  it('shows an empty message when no boards exist', async () => {
    const wrapper = await setup([], []);

    expect(wrapper.find('.card-header-title').text()).toBe('Job Boards — Request #42');
    expect(wrapper.text()).toContain('No job boards available.');
  });

  it('lists the boards with the current ones active', async () => {
    const wrapper = await setup([{ sourceId: 'src-2', value: 'Kijiji', publishedAt: '2026-11-10T12:00:00', externalUrl: 'https://kijiji.ca/1' }]);

    expect(rows(wrapper).map((r) => r.find('.board-row__switch').text())).toEqual(['Indeed', 'Kijiji']);
    expect(rows(wrapper)[0].find('.board-row__inputs').exists()).toBe(false);
    expect((rows(wrapper)[1].find('.board-row__url input').element as HTMLInputElement).value).toBe('https://kijiji.ca/1');
  });

  it('saves only the active boards with their details', async () => {
    vi.mocked(setAgencyRequestSources).mockResolvedValue();
    const published = '2026-11-10T12:00:00';
    const wrapper = await setup([{ sourceId: 'src-2', value: 'Kijiji', publishedAt: published, externalUrl: 'https://kijiji.ca/1' }]);
    await rows(wrapper)[0].find('input[type="checkbox"]').setValue(true);
    await nextTick();
    await rows(wrapper)[0].find('.board-row__url input').setValue('   ');
    await save(wrapper);

    expect(setAgencyRequestSources).toHaveBeenCalledExactlyOnceWith('req-1', [
      { sourceId: 'src-1', publishedAt: null, externalUrl: null },
      { sourceId: 'src-2', publishedAt: new Date(published).toISOString(), externalUrl: 'https://kijiji.ca/1' },
    ]);
    expect(showAlertSuccess).toHaveBeenCalledWith('Job boards updated');
    expect(wrapper.emitted('saved')).toHaveLength(1);
    expect(wrapper.emitted('close')).toHaveLength(1);
  });

  it('deactivates a board', async () => {
    vi.mocked(setAgencyRequestSources).mockResolvedValue();
    const wrapper = await setup([{ sourceId: 'src-1', value: 'Indeed' }]);
    await rows(wrapper)[0].find('input[type="checkbox"]').setValue(false);
    await save(wrapper);

    expect(setAgencyRequestSources).toHaveBeenCalledWith('req-1', []);
  });

  it('rehydrates when the current boards change', async () => {
    const wrapper = await setup();
    await wrapper.setProps({ currentBoards: [{ sourceId: 'src-1', value: 'Indeed', externalUrl: 'https://indeed.com/9' }] });

    expect((rows(wrapper)[0].find('.board-row__url input').element as HTMLInputElement).value).toBe('https://indeed.com/9');
  });

  it('shows the error when the boards fail to load', async () => {
    const error = new Error('load failed');
    vi.mocked(getSourcesForRequests).mockRejectedValue(error);
    const wrapper = mountWithPlugins(JobBoardsModal, { props: { requestId: 'req-1', numberId: 42, currentBoards: [] } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
    expect(wrapper.text()).toContain('No job boards available.');
    expect(wrapper.find('.loading-overlay').exists()).toBe(false);
  });

  it('shows the error and stays open when saving fails', async () => {
    const error = new Error('boom');
    vi.mocked(setAgencyRequestSources).mockRejectedValue(error);
    const wrapper = await setup();
    await save(wrapper);

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('close')).toBeUndefined();
  });
});
