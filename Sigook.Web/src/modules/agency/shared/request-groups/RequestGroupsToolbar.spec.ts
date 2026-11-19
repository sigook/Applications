import { beforeEach, describe, expect, it, vi } from 'vitest';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import RequestGroupsToolbar from '@/modules/agency/shared/request-groups/RequestGroupsToolbar.vue';
import { useAgencyStore } from '@/modules/agency/store';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

const { exportStub } = vi.hoisted(() => ({
  exportStub: { name: 'Export', props: { url: String, params: Object, fileName: String }, emits: ['onDataLoading'], template: '<div class="export" />' },
}));

vi.mock('@/shared/ui/Export.vue', () => ({ default: exportStub }));

let pinia: Pinia;

function setup(hasGroups = true) {
  return mountWithPlugins(RequestGroupsToolbar, {
    props: { hasGroups, exportUrl: '/api/export', exportParams: { pageIndex: 1 }, exportFileName: 'Requests' },
    global: { plugins: [pinia] },
  });
}

function button(wrapper: ReturnType<typeof setup>, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  useAgencyStore().setAgencies([{ id: 'a1', name: 'Covenant' }]);
});

describe('RequestGroupsToolbar', () => {
  it('refreshes', async () => {
    const wrapper = setup();
    await wrapper.find('button[aria-label="Refresh"]').trigger('click');
    expect(wrapper.emitted('search')).toHaveLength(1);
  });

  it('expands and collapses every group', async () => {
    const wrapper = setup();
    await button(wrapper, 'Expand all requests').trigger('click');
    await button(wrapper, 'Collapse all').trigger('click');
    expect(wrapper.emitted('expandAll')).toHaveLength(1);
    expect(wrapper.emitted('collapseAll')).toHaveLength(1);
  });

  it('hides expand and collapse without groups', () => {
    const wrapper = setup(false);
    expect(wrapper.text()).not.toContain('Expand all requests');
  });

  it('searches when only mine is toggled', async () => {
    const wrapper = setup();
    await wrapper.find('input[type="checkbox"]').setValue(true);
    expect(wrapper.emitted('update:onlyMine')).toEqual([[true]]);
    expect(wrapper.emitted('search')).toHaveLength(1);
  });

  it('passes the export settings and forwards its loading', () => {
    const wrapper = setup();
    const exporter = wrapper.findComponent(exportStub);
    expect(exporter.props()).toEqual({ url: '/api/export', params: { pageIndex: 1 }, fileName: 'Requests' });
    exporter.vm.$emit('onDataLoading', true);
    expect(wrapper.emitted('loading')).toEqual([[true]]);
  });
});
