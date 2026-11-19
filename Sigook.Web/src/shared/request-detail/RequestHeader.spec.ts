import { describe, expect, it } from 'vitest';
import RequestHeader from '@/shared/request-detail/RequestHeader.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestKpi } from '@/shared/request-detail/types';

const kpis: RequestKpi[] = [{ key: 'rate', label: 'Rate', value: '$20.00' }];

interface SetupOptions {
  isAsap?: boolean;
  isDirectHiring?: boolean;
  chips?: string[];
  workersWorking?: number;
  workersTotal?: number;
  slots?: Record<string, string>;
}

function setup({ slots, ...props }: SetupOptions = {}) {
  return mountWithPlugins(RequestHeader, {
    props: {
      numberId: 1234,
      title: 'Forklift operator',
      subtitle: 'Night shift',
      orgName: 'Acme',
      statusLabel: 'Open',
      statusVariant: 'is-success',
      kpis,
      ...props,
    },
    slots,
  });
}

function kpiRows(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.detail-kpi').map((k) => {
    const value = k.find('.detail-kpi-value');
    const progress = value.find('progress');
    const text = progress.exists() ? value.text().replace(progress.text(), '').trim() : value.text();
    return [k.find('dt').text(), text];
  });
}

describe('RequestHeader', () => {
  it('shows the number, title, subtitle and status', () => {
    const wrapper = setup();

    expect(wrapper.find('.detail-header-title').text()).toBe('1234 · Forklift operator');
    expect(wrapper.find('.detail-header-meta').text()).toContain('Night shift');
    expect(wrapper.find('.detail-header-org').text()).toBe('Acme');
    expect(wrapper.find('.detail-chip.is-success').text()).toBe('Open');
    expect(kpiRows(wrapper)).toEqual([['Rate', '$20.00']]);
  });

  it('adds the ASAP and direct hiring flags', () => {
    const wrapper = setup({ isAsap: true, isDirectHiring: true });

    expect(wrapper.findAll('.detail-chip.is-icon').map((c) => c.attributes('aria-label'))).toEqual(['ASAP', 'Direct hiring']);
  });

  it('shows the extra chips as meta chips', () => {
    const wrapper = setup({ chips: ['Forklift', 'Bilingual'] });

    expect(wrapper.findAll('.detail-chip.is-meta').map((c) => c.text())).toEqual(['Forklift', 'Bilingual']);
  });

  it('shows the open spots when the working count is unknown', () => {
    const wrapper = setup({ workersTotal: 5 });

    expect(kpiRows(wrapper)).toEqual([['Spots', '5'], ['Rate', '$20.00']]);
    expect(wrapper.find('progress').exists()).toBe(false);
  });

  it('shows the staffing progress of the workers', () => {
    const wrapper = setup({ workersWorking: 3, workersTotal: 4 });

    expect(kpiRows(wrapper)[0]).toEqual(['Workers', '3 / 4']);
    expect(wrapper.find('progress').attributes('value')).toBe('75');
  });

  it('caps the progress at 100 and handles zero spots', async () => {
    const wrapper = setup({ workersWorking: 6, workersTotal: 4 });
    expect(wrapper.find('progress').attributes('value')).toBe('100');

    await wrapper.setProps({ workersWorking: 0, workersTotal: 0 });
    expect(wrapper.find('progress').attributes('value')).toBe('0');
  });

  it('forwards the slots and places the quantity actions next to the workers', () => {
    const wrapper = setup({
      workersWorking: 1,
      workersTotal: 2,
      slots: { actions: '<button class="edit">Edit</button>', 'quantity-actions': '<button class="add-spot">+</button>' },
    });

    expect(wrapper.find('.detail-header-actions .edit').exists()).toBe(true);
    expect(wrapper.find('.detail-kpi .detail-kpi-extra .add-spot').exists()).toBe(true);
  });
});
