import { afterEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import ClientInteractionsModal from '@/modules/agency/sales/dashboard/components/ClientInteractionsModal.vue';
import { getCompanyInteractions } from '@/modules/agency/sales/clients/api';
import { CompanyInteractionSortBy } from '@/modules/agency/sales/clients/types';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createInteraction, createRecentClient } from '@/testing/salesFixtures';
import type { CompanyInteraction } from '@/modules/agency/sales/clients/types';
import type { SalesRecentClient } from '@/modules/agency/sales/dashboard/types';

vi.mock('@/modules/agency/sales/clients/api', () => ({ getCompanyInteractions: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/sales/clients/components/InteractionModal.vue', () => ({
  default: {
    name: 'InteractionModal',
    props: { modelValue: Boolean, interaction: Object, client: Object },
    emits: ['saved', 'update:modelValue'],
    template: '<div class="interaction-modal-stub" />',
  },
}));

const client = createRecentClient();
const first = createInteraction();
const second = createInteraction({ id: 'int-2', description: 'Follow-up email' });

async function setup(items: CompanyInteraction[] = [first, second], selected: SalesRecentClient | null = client) {
  vi.mocked(getCompanyInteractions).mockResolvedValue({ items, totalItems: items.length, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(ClientInteractionsModal, { props: { modelValue: false, client: selected } });
  await wrapper.setProps({ modelValue: true });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function interactionModal(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'InteractionModal' });
}

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAllComponents({ name: 'BButton' }).find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

afterEach(() => {
  vi.useRealTimers();
});

describe('ClientInteractionsModal', () => {
  it('loads the latest 50 interactions of the client when opened', async () => {
    vi.useFakeTimers({ now: new Date('2026-11-19T10:00:00'), toFake: ['Date'] });
    const wrapper = await setup();
    const modal = wrapper.findComponent({ name: 'BModal' });

    expect(getCompanyInteractions).toHaveBeenCalledExactlyOnceWith('company-1', { pageSize: 50, isDescending: true, sortBy: CompanyInteractionSortBy.CreatedAt });
    expect(modal.find('h2').text()).toBe('Acme Logistics');
    expect(modal.text()).toContain('ap@acme.test · 2 interactions');
    expect(modal.findAll('.sd-interaction-row').map((r) => r.find('.sd-interaction-time').text())).toEqual(['1h', '1h']);
  });

  it('says 1 interaction and omits a missing email', async () => {
    const wrapper = await setup([first], createRecentClient({ email: null }));

    expect(wrapper.findComponent({ name: 'BModal' }).find('h2 + p').text()).toBe('1 interaction');
  });

  it('does not load while closed or without a client', async () => {
    vi.mocked(getCompanyInteractions).mockResolvedValue({ items: [], totalItems: 0, pageIndex: 1, totalPages: 1 });
    const wrapper = mountWithPlugins(ClientInteractionsModal, { props: { modelValue: false, client: null } });
    await wrapper.setProps({ modelValue: true });
    await flushPromises();

    expect(getCompanyInteractions).not.toHaveBeenCalled();
    await wrapper.setProps({ modelValue: false });
    expect(getCompanyInteractions).not.toHaveBeenCalled();
  });

  it('logs a new interaction for the client and reloads when saved', async () => {
    const wrapper = await setup();
    await button(wrapper, 'Log interaction').trigger('click');

    expect(interactionModal(wrapper).props()).toEqual({ modelValue: true, interaction: null, client });
    expect(wrapper.findComponent({ name: 'BModal' }).props('canCancel')).toBe(false);

    interactionModal(wrapper).vm.$emit('saved');
    await flushPromises();
    expect(wrapper.emitted('saved')).toHaveLength(1);
    expect(getCompanyInteractions).toHaveBeenCalledTimes(2);
  });

  it('edits an interaction from the list', async () => {
    const wrapper = await setup();
    await wrapper.findAll('.sd-interaction-row')[1].trigger('click');

    expect(interactionModal(wrapper).props('interaction')).toEqual(second);
    interactionModal(wrapper).vm.$emit('update:modelValue', false);
    await nextTick();
    expect(wrapper.findComponent({ name: 'BModal' }).props('canCancel')).toEqual(['escape', 'x', 'outside']);
  });

  it('closes', async () => {
    const wrapper = await setup();
    await button(wrapper, 'Close').trigger('click');

    expect(wrapper.emitted('update:modelValue')).toEqual([[false]]);
  });

  it('shows the error when the interactions cannot load', async () => {
    const error = new Error('load');
    vi.mocked(getCompanyInteractions).mockRejectedValue(error);
    const wrapper = mountWithPlugins(ClientInteractionsModal, { props: { modelValue: false, client } });
    await wrapper.setProps({ modelValue: true });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
