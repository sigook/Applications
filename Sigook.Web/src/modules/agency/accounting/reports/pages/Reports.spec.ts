import { describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import Reports from '@/modules/agency/accounting/reports/pages/Reports.vue';
import { createTestRouter } from '@/testing/mountWithPlugins';

const { stub } = vi.hoisted(() => ({
  stub: (name: string) => ({ name, template: `<div class="${name}" />` }),
}));

vi.mock('@/modules/agency/accounting/paystubs/components/SubcontractorsReport.vue', () => ({ default: stub('SubcontractorsReport') }));
vi.mock('@/modules/agency/accounting/reports/components/HoursWorkedReport.vue', () => ({ default: stub('HoursWorkedReport') }));
vi.mock('@/modules/agency/accounting/reports/components/T4.vue', () => ({ default: stub('T4Report') }));
vi.mock('@/modules/agency/accounting/reports/components/CRAPayroll.vue', () => ({ default: stub('CraPayrollReport') }));
vi.mock('@/modules/agency/accounting/reports/components/PaymentReport.vue', () => ({ default: stub('PaymentReport') }));
vi.mock('@/modules/agency/accounting/reports/components/TimesheetsReport.vue', () => ({ default: stub('TimesheetsReport') }));

async function setup(path = '/accounting/reports') {
  const router = createTestRouter();
  await router.push(path);
  const wrapper = mount(Reports, { attachTo: document.body, global: { plugins: [Buefy, router] } });
  await flushPromises();
  return { wrapper, router };
}

describe('Reports', () => {
  it('opens the subcontractors tab by default', async () => {
    const { wrapper } = await setup();

    expect(wrapper.find('.SubcontractorsReport').isVisible()).toBe(true);
    expect(wrapper.find('.PaymentReport').exists()).toBe(false);
  });

  it('opens the tab from the query', async () => {
    const { wrapper } = await setup('/accounting/reports?tab=4');

    expect(wrapper.find('.HoursWorkedReport').exists()).toBe(true);
    expect(wrapper.find('.SubcontractorsReport').exists()).toBe(false);
  });

  it('keeps the selected tab in the url', async () => {
    const { wrapper, router } = await setup();
    wrapper.findComponent({ name: 'BTabs' }).vm.$emit('update:modelValue', '3');
    await flushPromises();

    expect(router.currentRoute.value.fullPath).toBe('/accounting/reports?tab=3');
    expect(wrapper.find('.CraPayrollReport').exists()).toBe(true);
  });
});
