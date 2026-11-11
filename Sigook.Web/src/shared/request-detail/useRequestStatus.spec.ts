import { describe, expect, it } from 'vitest';
import { ref } from 'vue';
import { RequestStatus } from '@/shared/constants/enums';
import { requestStatusClass, useRequestStatus } from '@/shared/request-detail/useRequestStatus';
import type { RequestStatusSource } from '@/shared/request-detail/types';

function statusOf(status: RequestStatus, workersQuantityWorking = 0, workersQuantity = 3) {
  return useRequestStatus(ref<RequestStatusSource | null>({ status, workersQuantity, workersQuantityWorking }));
}

describe('requestStatusClass', () => {
  it('builds a lowercase css class from the enum name', () => {
    expect(requestStatusClass(RequestStatus.Open)).toBe('status-open');
    expect(requestStatusClass(RequestStatus.PendingReview)).toBe('status-pendingreview');
  });
});

describe('useRequestStatus', () => {
  it('returns empty values without a request', () => {
    const status = useRequestStatus(ref(null));
    expect(status.canEdit.value).toBe(false);
    expect(status.canCancel.value).toBe(false);
    expect(status.isBooking.value).toBe(false);
    expect(status.isPendingReview.value).toBe(false);
    expect(status.statusLabel.value).toBe('');
    expect(status.statusVariant.value).toBe('');
  });

  it('lets an open request without workers be edited and cancelled', () => {
    const status = statusOf(RequestStatus.Open);
    expect(status.canEdit.value).toBe(true);
    expect(status.canCancel.value).toBe(true);
    expect(status.isBooking.value).toBe(false);
    expect(status.statusLabel.value).toBe('Open');
    expect(status.statusVariant.value).toBe('is-info');
  });

  it('marks an open request with some workers as booking and blocks cancel', () => {
    const status = statusOf(RequestStatus.Open, 1);
    expect(status.isBooking.value).toBe(true);
    expect(status.canCancel.value).toBe(false);
    expect(status.statusLabel.value).toBe('Open · Booking');
    expect(status.statusVariant.value).toBe('is-warning');
  });

  it('is not booking once every spot is working', () => {
    expect(statusOf(RequestStatus.Open, 3).isBooking.value).toBe(false);
  });

  it('lets a filled request be edited but not cancelled', () => {
    const status = statusOf(RequestStatus.Filled, 3);
    expect(status.canEdit.value).toBe(true);
    expect(status.canCancel.value).toBe(false);
    expect(status.statusVariant.value).toBe('is-success');
  });

  it('locks a cancelled request', () => {
    const status = statusOf(RequestStatus.Cancelled);
    expect(status.canEdit.value).toBe(false);
    expect(status.canCancel.value).toBe(false);
    expect(status.statusVariant.value).toBe('is-danger');
  });

  it('flags pending review requests', () => {
    const status = statusOf(RequestStatus.PendingReview);
    expect(status.isPendingReview.value).toBe(true);
    expect(status.canEdit.value).toBe(true);
    expect(status.canCancel.value).toBe(true);
    expect(status.statusLabel.value).toBe('Pending Review');
    expect(status.statusVariant.value).toBe('is-warning');
  });

  it('reacts to request changes', () => {
    const open: RequestStatusSource = { status: RequestStatus.Open, workersQuantity: 2, workersQuantityWorking: 0 };
    const request = ref<RequestStatusSource | null>(open);
    const status = useRequestStatus(request);
    expect(status.canCancel.value).toBe(true);
    request.value = { ...open, status: RequestStatus.Cancelled };
    expect(status.canCancel.value).toBe(false);
  });
});
