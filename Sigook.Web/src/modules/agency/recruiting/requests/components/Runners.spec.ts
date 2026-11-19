import { describe, expect, it, vi } from 'vitest';
import { defineComponent, nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import Runners from '@/modules/agency/recruiting/requests/components/Runners.vue';
import { createAgencyRunner, getAgencyRunners } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerSortBy, RunnerType, type AgencyRunnerFilter } from '@/modules/agency/recruiting/requests/runners/types';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestDetail } from '@/modules/agency/recruiting/requests/types';

const { reload } = vi.hoisted(() => ({ reload: vi.fn() }));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'req-1' } }),
}));
vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ getAgencyRunners: vi.fn(), createAgencyRunner: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/recruiting/requests/runners/CreateRunner.vue', () => ({
  default: {
    name: 'CreateRunner',
    props: { requestId: String, isSaving: Boolean },
    emits: ['create', 'close'],
    template: '<div class="create-runner-stub" />',
  },
}));
vi.mock('@/modules/agency/recruiting/runners/components/RunnersGrid.vue', async () => {
  const { defineComponent: define } = await import('vue');
  return {
    default: define({
      name: 'RunnersGrid',
      props: { fetch: Function, params: Object, sortMap: Object, canEdit: Boolean },
      emits: ['update:loading', 'updated', 'update:params'],
      setup(_, { expose }) {
        expose({ reload, search: vi.fn() });
      },
      template: '<div class="grid-stub"><slot name="actions" /></div>',
    }),
  };
});

const Host = defineComponent({ components: { Runners }, props: { request: Object }, template: '<Runners :request="request" />' });

function setup(usesRunners: boolean) {
  const request = { usesRunners } as Pick<AgencyRequestDetail, 'usesRunners'>;
  return mountWithPlugins(Runners, { props: { request: request as AgencyRequestDetail } });
}

type Wrapper = ReturnType<typeof setup>;

function grid(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'RunnersGrid' });
}

async function openCreate(wrapper: Wrapper) {
  await wrapper.find('.grid-stub button').trigger('click');
  await nextTick();
  return wrapper.findComponent({ name: 'CreateRunner' });
}

describe('Runners', () => {
  it('configures the grid for the request runners', async () => {
    vi.mocked(getAgencyRunners).mockResolvedValue({ items: [], totalItems: 0 } as unknown as Awaited<ReturnType<typeof getAgencyRunners>>);
    const wrapper = setup(true);

    expect(grid(wrapper).props('params')).toEqual({ requestId: 'req-1', isDescending: true, sortBy: RunnerSortBy.CreatedAt });
    expect(grid(wrapper).props('sortMap')).toEqual({ name: 0, status: 1, type: 2, createdAt: 3 });
    expect(grid(wrapper).props('canEdit')).toBe(true);
    const filter: AgencyRunnerFilter = { pageIndex: 2 };
    await (grid(wrapper).props('fetch') as (p: AgencyRunnerFilter) => Promise<unknown>)(filter);
    expect(getAgencyRunners).toHaveBeenCalledWith('req-1', filter);
    expect(wrapper.find('.message').exists()).toBe(false);
  });

  it('is read-only when the request does not use runners', () => {
    const wrapper = setup(false);

    expect(wrapper.find('.message').text()).toContain('This request does not use runners');
    expect(grid(wrapper).props('canEdit')).toBe(false);
    expect(wrapper.find('.grid-stub button').exists()).toBe(false);
  });

  it('is read-only without a request', () => {
    const wrapper = mountWithPlugins(Host);

    expect(wrapper.findComponent({ name: 'RunnersGrid' }).props('canEdit')).toBe(false);
  });

  it('mirrors the grid loading and forwards updates', async () => {
    const wrapper = setup(true);
    grid(wrapper).vm.$emit('update:loading', true);
    grid(wrapper).vm.$emit('updated');
    await nextTick();

    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(true);
    expect(wrapper.emitted('changed')).toHaveLength(1);
  });

  it('creates a runner, reloads the grid and closes the modal', async () => {
    vi.mocked(createAgencyRunner).mockResolvedValue('run-1');
    const wrapper = setup(true);
    const create = await openCreate(wrapper);
    expect(create.props('requestId')).toBe('req-1');
    create.vm.$emit('create', { workerProfileId: 'wp-1', type: RunnerType.Passive });
    await flushPromises();

    expect(createAgencyRunner).toHaveBeenCalledExactlyOnceWith('req-1', { workerProfileId: 'wp-1', type: RunnerType.Passive });
    expect(reload).toHaveBeenCalledOnce();
    expect(wrapper.emitted('changed')).toHaveLength(1);
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });

  it('keeps the modal open and shows the error when creation fails', async () => {
    const error = new Error('boom');
    vi.mocked(createAgencyRunner).mockRejectedValue(error);
    const wrapper = setup(true);
    const create = await openCreate(wrapper);
    create.vm.$emit('create', { workerProfileId: 'wp-1', type: RunnerType.Active });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('changed')).toBeUndefined();
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(true);
    expect(wrapper.findComponent({ name: 'CreateRunner' }).props('isSaving')).toBe(false);
  });

  it('closes the create modal on cancel', async () => {
    const wrapper = setup(true);
    const create = await openCreate(wrapper);
    create.vm.$emit('close');
    await nextTick();

    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });
});
