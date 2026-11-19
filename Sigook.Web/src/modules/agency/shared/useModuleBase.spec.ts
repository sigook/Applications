import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import { defineComponent, h } from 'vue';
import { createTestRouter } from '@/testing/mountWithPlugins';
import { useModuleBase } from '@/modules/agency/shared/useModuleBase';

type ModuleBase = ReturnType<typeof useModuleBase>;

async function mountAt(path: string) {
  const router = createTestRouter();
  await router.push(path);
  await router.isReady();
  let api: ModuleBase | undefined;
  const Host = defineComponent({
    setup() {
      api = useModuleBase();
      return () => h('div');
    },
  });
  mount(Host, { global: { plugins: [router] } });
  if (!api) throw new Error('Composable did not run');
  return { api, router };
}

describe('useModuleBase', () => {
  it('points to the sales routes under /sales', async () => {
    const { api } = await mountAt('/sales/companies/1');
    expect(api.isSalesView.value).toBe(true);
    expect(api.requestBase.value).toBe('/sales/requests');
    expect(api.companyBase.value).toBe('/sales/companies');
    expect(api.moduleCrumbs.value).toEqual([{ label: 'Sales' }]);
  });

  it('points to the recruiting routes elsewhere', async () => {
    const { api } = await mountAt('/recruiting/requests/1');
    expect(api.isSalesView.value).toBe(false);
    expect(api.requestBase.value).toBe('/recruiting/requests');
    expect(api.companyBase.value).toBe('/recruiting/companies');
    expect(api.moduleCrumbs.value).toEqual([{ label: 'Recruiting' }]);
  });

  it('reacts to navigation between modules', async () => {
    const { api, router } = await mountAt('/recruiting/companies');
    await router.push('/sales/requests');
    expect(api.isSalesView.value).toBe(true);
    expect(api.requestBase.value).toBe('/sales/requests');
  });
});
