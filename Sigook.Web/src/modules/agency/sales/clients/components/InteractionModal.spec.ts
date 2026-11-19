import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import InteractionModal from '@/modules/agency/sales/clients/components/InteractionModal.vue';
import { InteractionPurpose, InteractionStatus, InteractionType } from '@/modules/agency/sales/clients/types';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyInteraction } from '@/modules/agency/sales/clients/types';

const { submit } = vi.hoisted(() => ({ submit: vi.fn<() => Promise<boolean>>() }));

vi.mock('@/modules/agency/sales/clients/components/InteractionForm.vue', () => ({
  default: {
    name: 'InteractionForm',
    props: { interaction: Object, initialClient: Object },
    setup(_: unknown, { expose }: { expose: (exposed: Record<string, unknown>) => void }) {
      expose({ submit });
    },
    template: '<div class="interaction-form-stub" />',
  },
}));

const interaction = {
  id: 'int-1',
  companyProfileId: 'company-1',
  companyName: 'Acme',
  ownerId: 'u-1',
  description: 'Intro call',
  interactionPurpose: InteractionPurpose.Intro,
  interactionType: InteractionType.Call,
  interactionStatus: InteractionStatus.Completed,
  createdAt: '2026-11-10T12:00:00',
  updatedAt: '2026-11-10T12:00:00',
} satisfies CompanyInteraction;

function setup(props: { interaction?: CompanyInteraction | null } = {}) {
  return mountWithPlugins(InteractionModal, { props: { modelValue: true, client: { id: 'company-1', fullName: 'Acme' }, ...props } });
}

function button(wrapper: ReturnType<typeof setup>, text: string) {
  const found = wrapper.findAllComponents({ name: 'BButton' }).find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

describe('InteractionModal', () => {
  it('titles a new interaction and passes the client to the form', () => {
    const wrapper = setup();

    expect(wrapper.findComponent({ name: 'BModal' }).text()).toContain('Log interaction');
    expect(wrapper.findComponent({ name: 'InteractionForm' }).props('initialClient')).toEqual({ id: 'company-1', fullName: 'Acme' });
  });

  it('titles an edited interaction', () => {
    const wrapper = setup({ interaction });

    expect(wrapper.findComponent({ name: 'BModal' }).text()).toContain('Edit interaction');
    expect(wrapper.findComponent({ name: 'InteractionForm' }).props('interaction')).toEqual(interaction);
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
