import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import ClientModal from '@/modules/agency/sales/dashboard/components/ClientModal.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

const { submit } = vi.hoisted(() => ({ submit: vi.fn<() => Promise<boolean>>() }));

vi.mock('@/modules/agency/sales/dashboard/components/ClientForm.vue', () => ({
  default: {
    name: 'ClientForm',
    props: {},
    setup(_: unknown, { expose }: { expose: (exposed: Record<string, unknown>) => void }) {
      expose({ submit });
    },
    template: '<div class="client-form-stub" />',
  },
}));

function setup() {
  return mountWithPlugins(ClientModal, { props: { modelValue: true } });
}

function button(wrapper: ReturnType<typeof setup>, text: string) {
  const found = wrapper.findAllComponents({ name: 'BButton' }).find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

describe('ClientModal', () => {
  it('shows the client form', () => {
    const wrapper = setup();

    expect(wrapper.findComponent({ name: 'BModal' }).text()).toContain('New client');
    expect(wrapper.findComponent({ name: 'ClientForm' }).exists()).toBe(true);
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
