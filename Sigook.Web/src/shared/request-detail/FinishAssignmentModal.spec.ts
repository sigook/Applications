import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import FinishAssignmentModal from '@/shared/request-detail/FinishAssignmentModal.vue';
import { WorkerRequestFinishReason } from '@/shared/constants/enums';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(startWorking?: string | null) {
  return mountWithPlugins(FinishAssignmentModal, { props: { subject: 'John Smith · Forklift operator', startWorking } });
}

async function submit(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('form').trigger('submit');
}

beforeEach(() => {
  vi.useFakeTimers({ now: new Date('2026-06-10T15:30:00'), toFake: ['Date'] });
});

afterEach(() => {
  vi.useRealTimers();
});

describe('FinishAssignmentModal', () => {
  it('shows the subject and every finish reason', () => {
    const wrapper = setup();

    expect(wrapper.findAll('p')[0].text()).toBe('John Smith · Forklift operator');
    expect(wrapper.findAll('option').map((o) => o.text())).toEqual([
      'Contract completed', 'Client reduced staff', 'Worker resigned', 'Other',
    ]);
  });

  it('limits the last day between the start date and today', () => {
    const picker = setup('2026-05-04T13:00:00').findComponent({ name: 'BDatepicker' });

    expect(picker.props('minDate')).toEqual(new Date('2026-05-04T00:00:00'));
    expect(picker.props('maxDate')).toEqual(new Date('2026-06-10T15:30:00'));
  });

  it('has no minimum date without a start date', () => {
    expect(setup().findComponent({ name: 'BDatepicker' }).props('minDate')).toBeUndefined();
  });

  it('submits today as completed contract by default', async () => {
    const wrapper = setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('submit')).toHaveLength(1); });
    expect(wrapper.emitted('submit')?.[0]).toEqual([{
      lastDay: new Date('2026-06-10T00:00:00').toISOString(),
      reason: WorkerRequestFinishReason.ContractCompleted,
      comments: undefined,
    }]);
  });

  it('submits the chosen day, reason and comments', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', new Date('2026-06-05T12:00:00'));
    await wrapper.find('select').setValue(String(WorkerRequestFinishReason.WorkerResigned));
    await wrapper.find('textarea').setValue('Moved to another city');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('submit')).toHaveLength(1); });
    expect(wrapper.emitted('submit')?.[0]).toEqual([{
      lastDay: new Date('2026-06-05T00:00:00').toISOString(),
      reason: WorkerRequestFinishReason.WorkerResigned,
      comments: 'Moved to another city',
    }]);
  });

  it('requires the last day', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', null);
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Last day is required'); });
    expect(wrapper.emitted('submit')).toBeUndefined();
  });

  it('emits close on cancel', async () => {
    const wrapper = setup();
    const cancel = wrapper.findAll('button').find((b) => b.text() === 'Cancel');
    await cancel?.trigger('click');

    expect(wrapper.emitted('close')).toHaveLength(1);
    expect(wrapper.emitted('submit')).toBeUndefined();
  });
});
