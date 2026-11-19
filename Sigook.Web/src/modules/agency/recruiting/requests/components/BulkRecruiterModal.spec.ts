import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import BulkRecruiterModal from '@/modules/agency/recruiting/requests/components/BulkRecruiterModal.vue';
import { getAgencyPersonnel } from '@/modules/agency/profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyPersonnelListItem } from '@/modules/agency/profile/types';

vi.mock('@/modules/agency/profile/api', () => ({ getAgencyPersonnel: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

function person(id: string, name: string): AgencyPersonnelListItem {
  return { id, userId: `u-${id}`, name, email: null, role: 'Recruiter', agencyIds: [], agencies: [] };
}

const maria = person('p-1', 'Maria Diaz');
const mario = person('p-2', 'Mario Ruiz');
const john = person('p-3', 'John Smith');

async function setup(load: Promise<AgencyPersonnelListItem[]> = Promise.resolve([maria, mario, john])) {
  vi.mocked(getAgencyPersonnel).mockReturnValue(load);
  const wrapper = mountWithPlugins(BulkRecruiterModal, { props: { requestCount: 3 } });
  await load.catch(() => undefined);
  await nextTick();
  await nextTick();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function taginput(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'BTaginput' });
}

function buttons(wrapper: Wrapper) {
  return wrapper.findAll('.mt-4 button');
}

describe('BulkRecruiterModal', () => {
  it('shows the selection count and loads the personnel', async () => {
    const wrapper = await setup();

    expect(wrapper.text()).toContain('3 order(s) selected');
    expect(taginput(wrapper).props('data')).toEqual([maria, mario, john]);
    expect(buttons(wrapper).map((b) => b.text())).toEqual(['Cancel', 'Unassign all']);
  });

  it('unassigns everyone when nothing is selected', async () => {
    const wrapper = await setup();
    await buttons(wrapper)[1].trigger('click');

    expect(wrapper.emitted('submit')).toEqual([[[]]]);
  });

  it('filters by name and excludes selected recruiters', async () => {
    const wrapper = await setup();
    taginput(wrapper).vm.$emit('update:modelValue', [maria]);
    await nextTick();
    taginput(wrapper).vm.$emit('typing', 'MAR');
    await nextTick();

    expect(taginput(wrapper).props('data')).toEqual([mario]);
    taginput(wrapper).vm.$emit('typing', '');
    await nextTick();
    expect(taginput(wrapper).props('data')).toEqual([mario, john]);
  });

  it('assigns the selected recruiters', async () => {
    const wrapper = await setup();
    taginput(wrapper).vm.$emit('update:modelValue', [maria, john]);
    await nextTick();

    expect(buttons(wrapper)[1].text()).toBe('Assign');
    await buttons(wrapper)[1].trigger('click');
    expect(wrapper.emitted('submit')).toEqual([[['p-1', 'p-3']]]);
  });

  it('cancels', async () => {
    const wrapper = await setup();
    await buttons(wrapper)[0].trigger('click');

    expect(wrapper.emitted('cancel')).toHaveLength(1);
  });

  it('shows the error when the personnel fails to load', async () => {
    const error = new Error('boom');
    const wrapper = await setup(Promise.reject(error));

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(taginput(wrapper).props('data')).toEqual([]);
  });
});
