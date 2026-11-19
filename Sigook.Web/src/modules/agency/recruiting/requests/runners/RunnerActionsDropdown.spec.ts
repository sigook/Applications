import { describe, expect, it } from 'vitest';
import RunnerActionsDropdown from '@/modules/agency/recruiting/requests/runners/RunnerActionsDropdown.vue';
import { RunnerStatus } from '@/modules/agency/recruiting/requests/runners/types';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(status: RunnerStatus, canEdit?: boolean) {
  return mountWithPlugins(RunnerActionsDropdown, { props: { status, canEdit } });
}

function items() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item'));
}

function labels() {
  return items().map((i) => i.textContent.trim());
}

function item(text: string) {
  const found = items().find((i) => i.textContent.trim() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

describe('RunnerActionsDropdown', () => {
  it('offers every action for an editable runner with a scheduled interview', () => {
    setup(RunnerStatus.InterviewScheduled);

    expect(labels()).toEqual(['Change status', 'Add interview', 'View history', 'Delete']);
  });

  it('hides add interview when the status does not allow it', () => {
    setup(RunnerStatus.SentToClient);

    expect(labels()).toEqual(['Change status', 'View history', 'Delete']);
  });

  it('hides change status for a hired runner', () => {
    setup(RunnerStatus.Hired);

    expect(labels()).toEqual(['View history', 'Delete']);
  });

  it('only allows viewing history when read-only', () => {
    setup(RunnerStatus.InterviewRescheduled, false);

    expect(labels()).toEqual(['View history']);
  });

  it('emits the chosen action', () => {
    const wrapper = setup(RunnerStatus.InterviewScheduled);
    item('Change status').click();
    item('Add interview').click();
    item('View history').click();
    item('Delete').click();

    expect(wrapper.emitted('open')).toEqual([['status'], ['interview'], ['history']]);
    expect(wrapper.emitted('delete')).toHaveLength(1);
  });

  it('renders a custom trigger', () => {
    const wrapper = mountWithPlugins(RunnerActionsDropdown, {
      props: { status: RunnerStatus.SentToClient },
      slots: { trigger: '<button class="custom-trigger">Actions</button>' },
    });

    expect(wrapper.find('.custom-trigger').exists()).toBe(true);
  });
});
