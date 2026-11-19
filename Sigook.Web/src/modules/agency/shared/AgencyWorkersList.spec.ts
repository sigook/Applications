import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import AgencyWorkersList from '@/modules/agency/shared/AgencyWorkersList.vue';
import { getAgencyWorkersDropdown } from '@/modules/agency/recruiting/workers/api';
import { bookAgencyRequestWorker } from '@/modules/agency/recruiting/requests/api';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyWorkerDropdownItem } from '@/modules/agency/recruiting/workers/types';

interface PromptConfig {
  onConfirm: (value: string, dialog: { close: () => void }) => Promise<void>;
}

const { prompt } = vi.hoisted(() => ({ prompt: vi.fn<(config: PromptConfig) => void>() }));

vi.mock('@/modules/agency/recruiting/workers/api', () => ({ getAgencyWorkersDropdown: vi.fn() }));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({ bookAgencyRequestWorker: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => ({ getDialog: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const approved: AgencyWorkerDropdownItem = { id: 'w1', workerProfileId: 'wp1', fullName: 'Jane Doe', email: 'jane@x.com', approvedToWork: true };

function setup() {
  return mountWithPlugins(AgencyWorkersList, { props: { requestId: 'request-1' } });
}

function bookButton(wrapper: ReturnType<typeof setup>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Book Worker');
  if (!button) throw new Error('Book Worker not found');
  return button;
}

async function selectWorker(wrapper: ReturnType<typeof setup>, worker: AgencyWorkerDropdownItem | null) {
  wrapper.findComponent({ name: 'BAutocomplete' }).vm.$emit('select', worker);
  await nextTick();
}

function promptConfig(): PromptConfig {
  const config = prompt.mock.lastCall?.[0];
  if (!config) throw new Error('prompt not opened');
  return config;
}

beforeEach(() => {
  vi.mocked(getDialog).mockReturnValue({ prompt } as unknown as ReturnType<typeof getDialog>);
});

afterEach(() => {
  vi.useRealTimers();
});

describe('AgencyWorkersList', () => {
  it('searches workers and labels their approval', async () => {
    vi.useFakeTimers();
    vi.mocked(getAgencyWorkersDropdown).mockResolvedValue([approved, { ...approved, id: 'w2', fullName: 'Tom', approvedToWork: false }]);
    const wrapper = setup();
    const autocomplete = wrapper.findComponent({ name: 'BAutocomplete' });
    autocomplete.vm.$emit('typing', 'Jan');
    await vi.runAllTimersAsync();

    expect(getAgencyWorkersDropdown).toHaveBeenCalledWith({ searchTerm: 'Jan' });
    const formatter = autocomplete.props('customFormatter') as (option: AgencyWorkerDropdownItem) => string;
    expect(formatter(approved)).toBe('Jane Doe | Approved');
    expect(formatter({ ...approved, approvedToWork: false })).toBe('Jane Doe | Not Approved');
  });

  it('enables booking only with a selected worker', async () => {
    const wrapper = setup();
    expect(bookButton(wrapper).attributes('disabled')).toBeDefined();
    await selectWorker(wrapper, approved);
    expect(bookButton(wrapper).attributes('disabled')).toBeUndefined();
    await selectWorker(wrapper, null);
    expect(bookButton(wrapper).attributes('disabled')).toBeDefined();
  });

  it('books the worker from the chosen start date', async () => {
    vi.mocked(bookAgencyRequestWorker).mockResolvedValue({ id: 'b1' });
    const wrapper = setup();
    await selectWorker(wrapper, approved);
    await bookButton(wrapper).trigger('click');

    expect(promptConfig()).toMatchObject({ message: 'Starting Date', confirmText: 'Book', inputAttrs: { type: 'date' } });
    const close = vi.fn();
    await promptConfig().onConfirm('2026-06-10', { close });

    expect(bookAgencyRequestWorker).toHaveBeenCalledWith('request-1', 'wp1', { startWorking: '2026-06-10' });
    expect(showAlertSuccess).toHaveBeenCalledWith('Booked');
    expect(wrapper.emitted('workerBooked')).toHaveLength(1);
    expect(close).toHaveBeenCalledOnce();
  });

  it('reports a failed booking and keeps the dialog open', async () => {
    vi.mocked(bookAgencyRequestWorker).mockRejectedValue('Already booked');
    const wrapper = setup();
    await selectWorker(wrapper, approved);
    await bookButton(wrapper).trigger('click');
    const close = vi.fn();
    await promptConfig().onConfirm('2026-06-10', { close });

    expect(showAlertError).toHaveBeenCalledWith('Already booked');
    expect(close).not.toHaveBeenCalled();
    expect(wrapper.emitted('workerBooked')).toBeUndefined();
  });

  it('does not book once the worker was cleared', async () => {
    const wrapper = setup();
    await selectWorker(wrapper, approved);
    await bookButton(wrapper).trigger('click');
    await selectWorker(wrapper, null);
    await promptConfig().onConfirm('2026-06-10', { close: vi.fn() });
    expect(bookAgencyRequestWorker).not.toHaveBeenCalled();
  });
});
