import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import DatepickerModal from '@/modules/agency/recruiting/requests/components/DatepickerModal.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(startWorking?: string | Date | null) {
  return mountWithPlugins(DatepickerModal, { props: { startWorking } });
}

type Wrapper = ReturnType<typeof setup>;

function picker(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BDatepicker' });
}

async function save(wrapper: Wrapper) {
  await wrapper.find('button.is-primary').trigger('click');
}

beforeEach(() => {
  vi.useFakeTimers({ toFake: ['Date'] });
  vi.setSystemTime(new Date(2026, 10, 19, 12));
});

afterEach(() => {
  vi.useRealTimers();
});

describe('DatepickerModal', () => {
  it('defaults to today and allows dates up to one month back', async () => {
    const wrapper = setup();
    await save(wrapper);

    expect(picker(wrapper).props('minDate')).toEqual(new Date(2026, 9, 19, 12));
    expect(wrapper.emitted('update:startWorking')).toEqual([[new Date(2026, 10, 19, 12)]]);
    expect(wrapper.emitted('onSelectCalendar')).toEqual([[new Date(2026, 10, 19, 12)]]);
  });

  it('starts from the given date', async () => {
    const wrapper = setup('2026-11-25T08:00:00');
    await save(wrapper);

    expect(wrapper.emitted('onSelectCalendar')).toEqual([[new Date(2026, 10, 25, 8)]]);
  });

  it('emits the picked date', async () => {
    const wrapper = setup();
    picker(wrapper).vm.$emit('update:modelValue', new Date(2026, 10, 30));
    await nextTick();
    await save(wrapper);

    expect(wrapper.emitted('update:startWorking')).toEqual([[new Date(2026, 10, 30)]]);
  });

  it('follows changes to the start date', async () => {
    const wrapper = setup(new Date(2026, 10, 25));
    await wrapper.setProps({ startWorking: new Date(2026, 10, 27) });
    await save(wrapper);
    await wrapper.setProps({ startWorking: null });
    await save(wrapper);

    expect(wrapper.emitted('onSelectCalendar')).toEqual([[new Date(2026, 10, 27)], [new Date(2026, 10, 19, 12)]]);
  });
});
