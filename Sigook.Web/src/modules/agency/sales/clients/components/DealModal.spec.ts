import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import DealModal from '@/modules/agency/sales/clients/components/DealModal.vue';
import { DealStatus, DealType } from '@/modules/agency/sales/clients/types';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { Deal } from '@/modules/agency/sales/clients/types';

const { submit } = vi.hoisted(() => ({ submit: vi.fn<() => Promise<boolean>>() }));

vi.mock('@/modules/agency/sales/clients/components/DealForm.vue', () => ({
  default: {
    name: 'DealForm',
    props: { deal: Object, initialClient: Object },
    setup(_: unknown, { expose }: { expose: (exposed: Record<string, unknown>) => void }) {
      expose({ submit });
    },
    template: '<div class="deal-form-stub" />',
  },
}));

const deal = {
  id: 'deal-1',
  title: 'Warehouse',
  companyProfileId: 'company-1',
  companyName: 'Acme',
  ownerId: 'u-1',
  date: '2026-11-10T12:00:00',
  value: 10,
  type: DealType.Temporal,
  status: DealStatus.Sent,
  createdAt: '2026-11-10T12:00:00',
  updatedAt: '2026-11-10T12:00:00',
} satisfies Deal;

function setup(props: { deal?: Deal | null } = {}) {
  return mountWithPlugins(DealModal, { props: { modelValue: true, client: { id: 'company-1', fullName: 'Acme' }, ...props } });
}

function button(wrapper: ReturnType<typeof setup>, text: string) {
  const found = wrapper.findAllComponents({ name: 'BButton' }).find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

describe('DealModal', () => {
  it('titles a new deal and passes the client to the form', () => {
    const wrapper = setup();

    expect(wrapper.findComponent({ name: 'BModal' }).text()).toContain('New deal');
    expect(wrapper.findComponent({ name: 'DealForm' }).props('initialClient')).toEqual({ id: 'company-1', fullName: 'Acme' });
  });

  it('titles an edited deal', () => {
    const wrapper = setup({ deal });

    expect(wrapper.findComponent({ name: 'BModal' }).text()).toContain('Edit deal');
    expect(wrapper.findComponent({ name: 'DealForm' }).props('deal')).toEqual(deal);
  });

  it('emits saved and closes when the form saves', async () => {
    submit.mockResolvedValue(true);
    const wrapper = setup();
    await button(wrapper, 'Save').trigger('click');
    await flushPromises();

    expect(wrapper.emitted('saved')).toHaveLength(1);
    expect(wrapper.emitted('update:modelValue')).toEqual([[false]]);
  });

  it('stays open when the form does not save', async () => {
    submit.mockResolvedValue(false);
    const wrapper = setup();
    await button(wrapper, 'Save').trigger('click');
    await flushPromises();

    expect(wrapper.emitted('saved')).toBeUndefined();
    expect(wrapper.emitted('update:modelValue')).toBeUndefined();
    expect(button(wrapper, 'Save').props('loading')).toBe(false);
  });

  it('closes on cancel', async () => {
    const wrapper = setup();
    await button(wrapper, 'Cancel').trigger('click');

    expect(wrapper.emitted('update:modelValue')).toEqual([[false]]);
  });
});
