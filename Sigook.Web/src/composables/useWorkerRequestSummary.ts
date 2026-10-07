import { computed, type Ref } from 'vue';
import { datesKpi, formatBreak, rateKpi, shiftKpi } from '@/utils/requestDetail';
import type { WorkerRequestDetail } from '@/types/worker';
import type { RequestChipVariant, RequestFact, RequestKpi } from '@/types/requestDetail';

const statusVariants: Record<string, RequestChipVariant> = {
  Open: 'is-info',
  Booked: 'is-success',
  Filled: 'is-success',
  InQueue: 'is-warning',
  Rejected: 'is-danger',
  Decline: 'is-danger',
  Cancelled: 'is-danger',
};

export function useWorkerRequestSummary(request: Ref<WorkerRequestDetail | null>) {
  const statusLabel = computed(() => {
    const r = request.value;
    if (!r) return '';
    return r.status && r.status !== 'None' ? r.status : r.requestStatus ?? '';
  });

  const statusVariant = computed<RequestChipVariant>(() => statusVariants[statusLabel.value] ?? '');

  const termLabel = computed(() => (request.value?.durationTerm === 'LongTerm' ? 'Long Term' : 'Short Term'));

  const kpis = computed<RequestKpi[]>(() => {
    const r = request.value;
    if (!r) return [];
    const showFinish = r.durationTerm !== 'LongTerm' || r.requestStatus === 'Filled' || r.requestStatus === 'Cancelled';
    return [
      rateKpi(r.workerRate, r.workerSalary, r.incentive),
      datesKpi(r.startAt, r.finishAt, showFinish),
      shiftKpi(r.displayShift, r.durationBreak, r.breakIsPaid),
    ];
  });

  const fields = computed<RequestFact[]>(() => {
    const r = request.value;
    if (!r) return [];
    return [
      { label: 'Role (position)', value: r.jobPosition || r.jobTitle || '—' },
      { label: 'Term', value: termLabel.value },
      { label: 'Break', value: formatBreak(r.durationBreak, r.breakIsPaid) },
      { label: 'Holiday', value: r.holidayIsPaid ? 'Paid' : 'Not paid' },
    ];
  });

  const chips = computed(() => [termLabel.value]);

  return { statusLabel, statusVariant, kpis, fields, chips };
}
