import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import GeneratePayStubs from '@/modules/agency/accounting/paystubs/components/GeneratePayStubs.vue';
import { generatePayStubs, getWorkersReadyForPayStub } from '@/modules/agency/accounting/paystubs/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/accounting/paystubs/api', () => ({ generatePayStubs: vi.fn(), getWorkersReadyForPayStub: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const workers = [
  { workerId: 'w-1', firstName: 'John', middleName: 'Paul', lastName: 'Smith', secondLastName: 'Doe', businessName: 'Acme' },
  { workerId: 'w-2', firstName: 'Ana', lastName: 'Lopez', businessName: 'Globex' },
];

async function setup() {
  const wrapper = mountWithPlugins(GeneratePayStubs);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function generate(wrapper: Wrapper) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Generate');
  if (!button) throw new Error('Generate not found');
  return button;
}

async function check(wrapper: Wrapper, index: number) {
  await wrapper.findAll('tbody .checkbox-cell label')[index].trigger('click');
}

describe('GeneratePayStubs', () => {
  it('lists the workers ready for a pay stub', async () => {
    vi.mocked(getWorkersReadyForPayStub).mockResolvedValue(workers);
    const wrapper = await setup();

    const rows = wrapper.findAll('tbody tr').map((r) => r.findAll('td').slice(1).map((c) => c.text().replace(/\s+/g, ' ')));
    expect(rows).toEqual([['John Paul Smith Doe', 'Acme'], ['Ana Lopez', 'Globex']]);
    expect(generate(wrapper).attributes('disabled')).toBeDefined();
  });

  it('generates pay stubs for the checked workers', async () => {
    vi.mocked(getWorkersReadyForPayStub).mockResolvedValue(workers);
    vi.mocked(generatePayStubs).mockResolvedValue();
    const wrapper = await setup();
    await check(wrapper, 1);
    await generate(wrapper).trigger('click');
    await flushPromises();

    expect(generatePayStubs).toHaveBeenCalledExactlyOnceWith(['w-2']);
    expect(wrapper.emitted('pay-stubs-generated')).toHaveLength(1);
  });

  it('shows the error and reloads the workers when generation fails', async () => {
    const error = new Error('locked');
    vi.mocked(getWorkersReadyForPayStub).mockResolvedValue(workers);
    vi.mocked(generatePayStubs).mockRejectedValue(error);
    const wrapper = await setup();
    await check(wrapper, 0);
    await generate(wrapper).trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getWorkersReadyForPayStub).toHaveBeenCalledTimes(2);
    expect(wrapper.emitted('pay-stubs-generated')).toBeUndefined();
  });

  it('shows the error when the workers cannot load', async () => {
    const error = new Error('offline');
    vi.mocked(getWorkersReadyForPayStub).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
