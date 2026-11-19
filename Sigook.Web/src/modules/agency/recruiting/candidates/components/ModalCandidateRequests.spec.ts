import { afterEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import ModalCandidateRequests from '@/modules/agency/recruiting/candidates/components/ModalCandidateRequests.vue';
import { getAllAgencyRequests, postAgencyRequestApplicant } from '@/modules/agency/recruiting/requests/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestApplicant, AgencyRequestListItem } from '@/modules/agency/recruiting/requests/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({ getAllAgencyRequests: vi.fn(), postAgencyRequestApplicant: vi.fn() }));

const request = { id: 'req-1', numberId: 1001, jobTitle: 'Forklift operator', companyFullName: 'Moca Logistics' } as AgencyRequestListItem;

function setup() {
  return mountWithPlugins(ModalCandidateRequests, { props: { candidateId: 77 } });
}

type Wrapper = ReturnType<typeof setup>;

function autocomplete(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BAutocomplete' });
}

async function save(wrapper: Wrapper) {
  await wrapper.find('button.is-primary').trigger('click');
  await flushPromises();
}

afterEach(() => {
  vi.useRealTimers();
});

describe('ModalCandidateRequests', () => {
  it('searches the open requests while typing', async () => {
    vi.useFakeTimers({ toFake: ['setTimeout', 'clearTimeout'] });
    const results = Promise.resolve([request]);
    vi.mocked(getAllAgencyRequests).mockReturnValue(results);
    const wrapper = setup();
    autocomplete(wrapper).vm.$emit('typing', '10');
    vi.advanceTimersByTime(300);
    await results;
    await nextTick();

    expect(getAllAgencyRequests).toHaveBeenCalledExactlyOnceWith({ filter: '10', statuses: [0, 1, 4] });
    expect(autocomplete(wrapper).props('data')).toEqual([request]);
    const formatter = autocomplete(wrapper).props('customFormatter') as (option: AgencyRequestListItem) => string;
    expect(formatter(request)).toBe('1001 | Forklift operator | Moca Logistics');
  });

  it('does nothing until a request is selected', async () => {
    const wrapper = setup();
    await save(wrapper);

    expect(postAgencyRequestApplicant).not.toHaveBeenCalled();
    expect(wrapper.emitted('onSelectRequest')).toBeUndefined();
  });

  it('applies the candidate to the selected request', async () => {
    vi.mocked(postAgencyRequestApplicant).mockResolvedValue({} as AgencyRequestApplicant);
    const wrapper = setup();
    autocomplete(wrapper).vm.$emit('select', request);
    await save(wrapper);

    expect(postAgencyRequestApplicant).toHaveBeenCalledExactlyOnceWith('req-1', { candidateId: '77' });
    expect(wrapper.emitted('onSelectRequest')).toHaveLength(1);
  });

  it('shows the error when applying fails', async () => {
    const error = new Error('apply failed');
    vi.mocked(postAgencyRequestApplicant).mockRejectedValue(error);
    const wrapper = setup();
    autocomplete(wrapper).vm.$emit('select', request);
    await save(wrapper);

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('onSelectRequest')).toBeUndefined();
  });
});
