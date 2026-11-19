import { afterEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ManageApplicantsModal from '@/modules/agency/recruiting/requests/components/ManageApplicantsModal.vue';
import { searchAgencyRequestApplicants } from '@/modules/agency/recruiting/requests/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { ApplicantSearchResult } from '@/modules/agency/recruiting/requests/types';

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'route-req' } }),
}));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({ searchAgencyRequestApplicants: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const worker: ApplicantSearchResult = { workerProfileId: 'wp-1', candidateId: null, numberId: 5, name: 'Ana Lopez', email: 'ana@mail.com', type: 'Worker', approvedToWork: true };
const candidate: ApplicantSearchResult = { workerProfileId: null, candidateId: 'cand-1', numberId: 9, name: 'Leo Park', type: 'Candidate', approvedToWork: false };

function setup(requestId?: string | null) {
  return mountWithPlugins(ManageApplicantsModal, { props: { requestId } });
}

type Wrapper = ReturnType<typeof setup>;

function autocomplete(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BAutocomplete' });
}

function label(item: ApplicantSearchResult) {
  return `#${item.numberId} | ${item.name} | ${item.email ?? 'No Email'} | ${item.type}`;
}

async function settle() {
  await new Promise((resolve) => { setTimeout(resolve, 20); });
  await flushPromises();
}

async function type(wrapper: Wrapper, text: string) {
  autocomplete(wrapper).vm.$emit('update:modelValue', text);
  await settle();
}

async function choose(wrapper: Wrapper, item: ApplicantSearchResult) {
  autocomplete(wrapper).vm.$emit('select', item);
  await type(wrapper, label(item));
}

async function save(wrapper: Wrapper) {
  await wrapper.find('button.is-primary').trigger('click');
}

afterEach(() => {
  vi.useRealTimers();
});

describe('ManageApplicantsModal', () => {
  it('requires an applicant', async () => {
    const wrapper = setup('req-1');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Applicant is required'); });
    expect(wrapper.emitted('updateApplicants')).toBeUndefined();
  });

  it('adds the selected worker', async () => {
    const wrapper = setup('req-1');
    await choose(wrapper, worker);
    await save(wrapper);

    await vi.waitFor(() => {
      expect(wrapper.emitted('updateApplicants')).toEqual([[{ model: { workerProfileId: 'wp-1', candidateId: null, comments: null } }]]);
    });
  });

  it('adds the selected candidate', async () => {
    const wrapper = setup('req-1');
    await choose(wrapper, candidate);
    await save(wrapper);

    await vi.waitFor(() => {
      expect(wrapper.emitted('updateApplicants')).toEqual([[{ model: { workerProfileId: null, candidateId: 'cand-1', comments: null } }]]);
    });
  });

  it('requires picking an applicant from the list', async () => {
    const wrapper = setup('req-1');
    await type(wrapper, 'Ana Lopez');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Select an applicant from the list'); });
    expect(wrapper.emitted('updateApplicants')).toBeUndefined();
  });

  it('drops the selection when the text is edited after picking', async () => {
    const wrapper = setup('req-1');
    await choose(wrapper, worker);
    await type(wrapper, 'Ana Lo');
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Select an applicant from the list'); });
    expect(wrapper.emitted('updateApplicants')).toBeUndefined();
  });

  it('drops the selection when the autocomplete clears it', async () => {
    const wrapper = setup('req-1');
    await choose(wrapper, worker);
    autocomplete(wrapper).vm.$emit('select', null);
    await settle();
    await save(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Select an applicant from the list'); });
    expect(wrapper.emitted('updateApplicants')).toBeUndefined();
  });

  it('adds the applicant picked after a previous one', async () => {
    const wrapper = setup('req-1');
    await choose(wrapper, worker);
    await type(wrapper, 'Leo');
    await choose(wrapper, candidate);
    await save(wrapper);

    await vi.waitFor(() => {
      expect(wrapper.emitted('updateApplicants')).toEqual([[{ model: { workerProfileId: null, candidateId: 'cand-1', comments: null } }]]);
    });
  });

  it('formats the options with their type', () => {
    const wrapper = setup('req-1');
    const formatter = autocomplete(wrapper).props('customFormatter') as (o: ApplicantSearchResult) => string;

    expect(formatter(worker)).toBe('#5 | Ana Lopez | ana@mail.com | Worker');
    expect(formatter(candidate)).toBe('#9 | Leo Park | No Email | Candidate');
  });

  it('searches in the given request', () => {
    vi.useFakeTimers();
    vi.mocked(searchAgencyRequestApplicants).mockResolvedValue([worker]);
    const wrapper = setup('req-1');
    autocomplete(wrapper).vm.$emit('typing', 'ana');
    vi.advanceTimersByTime(400);

    expect(searchAgencyRequestApplicants).toHaveBeenCalledExactlyOnceWith('req-1', 'ana');
  });

  it('falls back to the route request', () => {
    vi.useFakeTimers();
    vi.mocked(searchAgencyRequestApplicants).mockResolvedValue([]);
    const wrapper = setup();
    autocomplete(wrapper).vm.$emit('typing', 'leo');
    vi.advanceTimersByTime(400);

    expect(searchAgencyRequestApplicants).toHaveBeenCalledExactlyOnceWith('route-req', 'leo');
  });
});
