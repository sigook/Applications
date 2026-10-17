import { computed, type Ref } from 'vue';
import { datesKpi, formatBreak, rateKpi, shiftKpi } from '@/shared/request-detail/requestDetail';
import type { WorkerRequestDetail } from '@/modules/worker/requests/types';
import type { RequestFact, RequestKpi } from '@/shared/request-detail/types';

function fact(kpi: RequestKpi): RequestFact {
  return { label: kpi.label, value: kpi.hint ? `${kpi.value} ${kpi.hint}` : kpi.value };
}

export function useWorkerRequestSummary(request: Ref<WorkerRequestDetail | null>) {
  const fields = computed<RequestFact[]>(() => {
    const r = request.value;
    if (!r) return [];
    const showFinish = r.durationTerm !== 'LongTerm' || r.requestStatus === 'Filled' || r.requestStatus === 'Cancelled';
    return [
      { label: 'Role (position)', value: r.jobPosition || r.jobTitle || '—' },
      fact(rateKpi(r.workerRate, r.workerSalary, r.incentive)),
      fact(datesKpi(r.startAt, r.finishAt, showFinish)),
      fact(shiftKpi(r.displayShift, r.durationBreak, r.breakIsPaid)),
      { label: 'Term', value: r.durationTerm === 'LongTerm' ? 'Long Term' : 'Short Term' },
      { label: 'Break', value: formatBreak(r.durationBreak, r.breakIsPaid) },
      { label: 'Holiday', value: r.holidayIsPaid ? 'Paid' : 'Not paid' },
      { label: 'Spots', value: String(r.workersQuantity ?? '—') },
    ];
  });

  return { fields };
}
