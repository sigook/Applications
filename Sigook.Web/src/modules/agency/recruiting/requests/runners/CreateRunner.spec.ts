import { afterEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import CreateRunner from '@/modules/agency/recruiting/requests/runners/CreateRunner.vue';
import { searchAgencyRunnerProspects } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { ApplicantSearchResult } from '@/modules/agency/recruiting/requests/types';

vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ searchAgencyRunnerProspects: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const prospect: ApplicantSearchResult = { workerProfileId: 'wp-1', numberId: 12, name: 'Ana Lopez', email: 'ana@mail.com', type: 'Worker', approvedToWork: true };

function setup(isSaving = false) {
  return mountWithPlugins(CreateRunner, { props: { requestId: 'req-1', isSaving } });
}

type Wrapper = ReturnType<typeof setup>;

function autocomplete(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BAutocomplete' });
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('form').trigger('submit');
}

afterEach(() => {
  vi.useRealTimers();
});

describe('CreateRunner', () => {
  it('requires a worker', async () => {
    const wrapper = setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Select a worker'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(wrapper.emitted('create')).toBeUndefined();
  });

  it('searches prospects for the request after the debounce', async () => {
    vi.useFakeTimers();
    let resolve: (value: ApplicantSearchResult[]) => void = () => undefined;
    vi.mocked(searchAgencyRunnerProspects).mockReturnValue(new Promise((r) => { resolve = r; }));
    const wrapper = setup();
    autocomplete(wrapper).vm.$emit('typing', 'an');
    vi.advanceTimersByTime(400);
    expect(searchAgencyRunnerProspects).not.toHaveBeenCalled();

    autocomplete(wrapper).vm.$emit('typing', 'ana');
    vi.advanceTimersByTime(400);
    expect(searchAgencyRunnerProspects).toHaveBeenCalledExactlyOnceWith('req-1', 'ana');
    resolve([prospect]);
    await Promise.resolve();
    await nextTick();

    expect(autocomplete(wrapper).props('data')).toEqual([prospect]);
  });

  it('formats options with number, name and email', () => {
    const wrapper = setup();
    const formatter = autocomplete(wrapper).props('customFormatter') as (o: ApplicantSearchResult) => string;

    expect(formatter(prospect)).toBe('#12 | Ana Lopez | ana@mail.com');
    expect(formatter({ ...prospect, email: '' })).toBe('#12 | Ana Lopez | No Email');
  });

  it('emits the selected worker with the chosen type', async () => {
    const wrapper = setup();
    await wrapper.findAll('input[type="radio"]')[1].setValue(true);
    autocomplete(wrapper).vm.$emit('select', prospect);
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('create')).toEqual([[{ workerProfileId: 'wp-1', type: RunnerType.Passive }]]); });
  });

  it('clears the selection when the user types again', async () => {
    const wrapper = setup();
    autocomplete(wrapper).vm.$emit('select', prospect);
    autocomplete(wrapper).vm.$emit('typing', 'x');
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalled(); });
    expect(wrapper.emitted('create')).toBeUndefined();
  });

  it('clears the selection when it is removed', async () => {
    const wrapper = setup();
    autocomplete(wrapper).vm.$emit('select', prospect);
    autocomplete(wrapper).vm.$emit('select', null);
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalled(); });
    expect(wrapper.emitted('create')).toBeUndefined();
  });

  it('shows the saving state and closes on cancel', async () => {
    const wrapper = setup(true);

    expect(wrapper.find('footer button.is-primary').classes()).toContain('is-loading');
    await wrapper.find('footer button').trigger('click');
    expect(wrapper.emitted('close')).toHaveLength(1);
  });
});
