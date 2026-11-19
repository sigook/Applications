import { describe, expect, it } from 'vitest';
import SummaryCards from '@/modules/worker/activity/components/SummaryCards.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerActivitySummary, WorkerPayStubListItem } from '@/modules/worker/activity/types';

const payStub: WorkerPayStubListItem = {
  id: 'ps1',
  payStubNumber: 'PS-100',
  weekEnding: '2026-05-30T12:00:00',
  paymentDate: '2026-06-05T12:00:00',
  hours: 40,
  totalEarnings: 800,
  totalDeductions: 120,
  totalPaid: 680,
  companies: ['Acme'],
  pdfUrl: null,
};

const summary: WorkerActivitySummary = {
  lastPayStub: payStub,
  weekHours: { total: 42.5, regular: 40, overtime: 2.5, holiday: 0, pendingApproval: 8 },
};

function setup(value: WorkerActivitySummary | null, showLastPay = true) {
  return mountWithPlugins(SummaryCards, { props: { summary: value, showLastPay } });
}

function cards(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.summary-card').map((c) => c.text());
}

describe('SummaryCards', () => {
  it('shows the last pay and the week hours', () => {
    const [lastPay, hours] = cards(setup(summary));
    expect(lastPay).toContain('$680.00');
    expect(lastPay).toContain('Week ending');
    expect(hours).toContain('42.5 h');
    expect(hours).toContain('40.0 regular · 2.5 OT');
    expect(hours).toContain('8.0 pending approval');
  });

  it('downloads the last pay stub', async () => {
    const wrapper = setup(summary);
    await wrapper.find('a').trigger('click');
    expect(wrapper.emitted('download')).toEqual([[payStub]]);
  });

  it('says when there are no pay stubs and nothing is pending', () => {
    const [lastPay, hours] = cards(setup({ lastPayStub: null, weekHours: { ...summary.weekHours, pendingApproval: 0 } }));
    expect(lastPay).toContain('No pay stubs yet');
    expect(hours).not.toContain('pending approval');
  });

  it('shows zero hours while loading', () => {
    expect(cards(setup(null))[1]).toContain('0.0 h');
  });

  it('hides the last pay card when asked', () => {
    expect(cards(setup(summary, false))).toHaveLength(1);
  });
});
