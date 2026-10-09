import { api } from '@/app/security/apiService';
import type { WorkerProfileDetail, WorkerCommentFilter, WorkerCommentList } from '@/shared/worker-profile/types';

// Comments
export function getMyComments(filter: WorkerCommentFilter): Promise<WorkerCommentList> {
  return api.get<WorkerCommentList>('/api/worker/comments', {
    params: { PageSize: filter.size, PageIndex: filter.pageIndex }
  });
}

// Profile
export function getMyProfile(): Promise<WorkerProfileDetail> {
  return api.get<WorkerProfileDetail>('/api/worker/profile/me');
}
