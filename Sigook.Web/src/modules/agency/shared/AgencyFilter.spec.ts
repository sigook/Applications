import { describe, it, expect, beforeEach } from 'vitest';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import { nextTick } from 'vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { useAgencyStore } from '@/modules/agency/store';
import AgencyFilter from '@/modules/agency/shared/AgencyFilter.vue';

const covenant = { id: 'a1', name: 'Covenant' };
const sigook = { id: 'a2', name: 'Sigook' };
const third = { id: 'a3', name: 'Third' };

let pinia: Pinia;

function mountFilter(modelValue?: string) {
  return mountWithPlugins(AgencyFilter, {
    props: { modelValue },
    global: { plugins: [pinia] },
  });
}

function switches() {
  return Array.from(document.body.querySelectorAll<HTMLInputElement>('.dropdown-content input[type="checkbox"]'));
}

async function open(wrapper: ReturnType<typeof mountFilter>) {
  await wrapper.find('.dropdown-trigger').trigger('click');
  await nextTick();
}

describe('AgencyFilter', () => {
  beforeEach(() => {
    pinia = createPinia();
    setActivePinia(pinia);
    useAgencyStore().setAgencies([covenant, sigook, third]);
  });

  it('is hidden when the user has a single agency', () => {
    useAgencyStore().setAgencies([covenant]);
    const wrapper = mountFilter();
    expect(wrapper.find('.agency-filter').exists()).toBe(false);
  });

  it('labels every selection state', async () => {
    const wrapper = mountFilter();
    expect(wrapper.text()).toContain('All agencies');
    await wrapper.setProps({ modelValue: '' });
    expect(wrapper.text()).toContain('No agencies');
    await wrapper.setProps({ modelValue: 'a2' });
    expect(wrapper.text()).toContain('Sigook');
    await wrapper.setProps({ modelValue: 'a1,a2' });
    expect(wrapper.text()).toContain('2 of 3 agencies');
  });

  it('turning one agency off sends the others and emits change', async () => {
    const wrapper = mountFilter();
    await open(wrapper);
    const [, first] = switches();
    first.click();
    await nextTick();
    expect(wrapper.emitted('update:modelValue')?.at(-1)).toEqual(['a2,a3']);
    expect(wrapper.emitted('change')).toHaveLength(1);
  });

  it('turning the last agency back on clears the filter', async () => {
    const wrapper = mountFilter('a2,a3');
    await open(wrapper);
    const [, first] = switches();
    first.click();
    await nextTick();
    expect(wrapper.emitted('update:modelValue')?.at(-1)).toEqual([undefined]);
  });

  it('the all agencies switch selects none and all', async () => {
    const wrapper = mountFilter();
    await open(wrapper);
    switches()[0].click();
    await nextTick();
    expect(wrapper.emitted('update:modelValue')?.at(-1)).toEqual(['']);
    await wrapper.setProps({ modelValue: '' });
    switches()[0].click();
    await nextTick();
    expect(wrapper.emitted('update:modelValue')?.at(-1)).toEqual([undefined]);
  });

  it('ignores ids the user no longer has', () => {
    const wrapper = mountFilter('a1,zz');
    expect(wrapper.text()).toContain('Covenant');
  });
});
