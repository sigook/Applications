import { describe, expect, it, vi } from 'vitest';
import { reportBulkResult } from '@/modules/agency/shared/request-groups/bulkReport';
import { showAlertSuccess, showAlertSummary } from '@/shared/utils/toast';

vi.mock('@/shared/utils/toast', () => ({
  showAlertSuccess: vi.fn(),
  showAlertSummary: vi.fn(() => Promise.resolve()),
}));

describe('reportBulkResult', () => {
  it('shows a success toast when nothing was skipped', () => {
    reportBulkResult(3, [], new Map(), 'requests');

    expect(showAlertSuccess).toHaveBeenCalledWith('3 requests updated');
    expect(showAlertSummary).not.toHaveBeenCalled();
  });

  it('lists every skipped item with its name and reason', () => {
    const names = new Map([
      ['r1', 'Forklift operator'],
      ['r2', 'Picker'],
    ]);

    reportBulkResult(1, [
      { id: 'r1', reason: 'Already closed' },
      { id: 'r2', reason: 'Has workers' },
    ], names, 'requests');

    expect(showAlertSuccess).not.toHaveBeenCalled();
    expect(showAlertSummary).toHaveBeenCalledWith(
      '1 updated, 2 skipped',
      '<ul class="bulk-skipped"><li><strong>Forklift operator</strong>: Already closed</li><li><strong>Picker</strong>: Has workers</li></ul>',
    );
  });

  it('falls back to the noun when the name is unknown', () => {
    reportBulkResult(0, [{ id: 'x', reason: 'Not found' }], new Map(), 'worker');

    expect(showAlertSummary).toHaveBeenCalledWith(
      '0 updated, 1 skipped',
      '<ul class="bulk-skipped"><li><strong>worker</strong>: Not found</li></ul>',
    );
  });

  it('escapes names and reasons', () => {
    reportBulkResult(0, [{ id: 'a', reason: 'x < y & "z"' }], new Map([['a', '<b>Bold</b>']]), 'requests');

    expect(showAlertSummary).toHaveBeenCalledWith(
      '0 updated, 1 skipped',
      '<ul class="bulk-skipped"><li><strong>&lt;b&gt;Bold&lt;/b&gt;</strong>: x &lt; y &amp; &quot;z&quot;</li></ul>',
    );
  });
});
