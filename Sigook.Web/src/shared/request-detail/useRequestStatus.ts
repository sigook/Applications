import { computed, type Ref } from 'vue';
import { RequestStatus, RequestStatusLabels } from '@/shared/constants/enums';
import type { RequestChipVariant, RequestStatusSource } from '@/shared/request-detail/types';

export function requestStatusClass(status: RequestStatus): string {
  return `status-${RequestStatus[status].toLowerCase()}`;
}

export function useRequestStatus(request: Ref<RequestStatusSource | null>) {
  const isPendingReview = computed(() => !!request.value && request.value.status === RequestStatus.PendingReview);

  const canEdit = computed(() =>
    !!request.value && (request.value.status === RequestStatus.Open
      || request.value.status === RequestStatus.Filled
      || request.value.status === RequestStatus.PendingReview));

  const canCancel = computed(() =>
    !!request.value
    && (request.value.status === RequestStatus.Open || request.value.status === RequestStatus.PendingReview)
    && !request.value.workersQuantityWorking);

  const isBooking = computed(() =>
    !!request.value &&
    request.value.status === RequestStatus.Open &&
    request.value.workersQuantityWorking > 0 &&
    request.value.workersQuantityWorking < request.value.workersQuantity);

  const statusLabel = computed(() => {
    if (!request.value) return '';
    const label = RequestStatusLabels[request.value.status as RequestStatus] ?? '';
    return isBooking.value ? `${label} · Booking` : label;
  });

  const statusVariant = computed<RequestChipVariant>(() => {
    if (!request.value) return '';
    if (isBooking.value) return 'is-warning';
    switch (request.value.status) {
      case RequestStatus.Open:
        return 'is-info';
      case RequestStatus.Filled:
        return 'is-success';
      case RequestStatus.Cancelled:
        return 'is-danger';
      case RequestStatus.PendingReview:
        return 'is-warning';
      default:
        return '';
    }
  });

  return { canEdit, canCancel, isBooking, isPendingReview, statusLabel, statusVariant };
}
