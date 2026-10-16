import { showAlertSuccess, showAlertSummary } from '@/shared/utils/toast';
import type { SkippedItem } from '@/modules/agency/shared/request-groups/types';

function escapeHtml(value: string): string {
  const element = document.createElement('span');
  element.textContent = value;
  return element.innerHTML;
}

export function reportBulkResult(updated: number, skipped: SkippedItem[], names: Map<string, string>, noun: string): void {
  if (!skipped.length) {
    showAlertSuccess(`${updated} ${noun} updated`);
    return;
  }
  const detail = skipped
    .map((item) => `<li><strong>${escapeHtml(names.get(item.id) ?? noun)}</strong>: ${escapeHtml(item.reason)}</li>`)
    .join('');
  showAlertSummary(`${updated} updated, ${skipped.length} skipped`, `<ul class="bulk-skipped">${detail}</ul>`);
}
