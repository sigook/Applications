import { defineStore } from 'pinia';
import type { WorkerProfile } from '@/types/worker';

interface WorkerStoreState {
  workerProfile: Partial<WorkerProfile>;
}

export const useWorkerStore = defineStore('worker', {
  state: (): WorkerStoreState => ({
    workerProfile: {},
  }),
  actions: {
    setWorkerProfile(data: Partial<WorkerProfile>) {
      this.workerProfile = data;
    },
  },
});
