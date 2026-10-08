import { defineStore } from 'pinia';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

interface WorkerStoreState {
  workerProfile: Partial<WorkerProfileDetail>;
}

export const useWorkerStore = defineStore('worker', {
  state: (): WorkerStoreState => ({
    workerProfile: {},
  }),
  actions: {
    setWorkerProfile(data: Partial<WorkerProfileDetail>) {
      this.workerProfile = data;
    },
  },
});
