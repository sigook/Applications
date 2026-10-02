import { defineStore } from 'pinia';
import type { WorkerProfileDetail } from '@/types/worker';

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
