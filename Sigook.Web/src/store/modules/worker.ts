import { WorkerProfile, WorkerBasicInfo } from "@/types/worker";

interface WorkerState {
  workerBasic: WorkerBasicInfo;
  workerProfile: Partial<WorkerProfile>;
}

export default {
  namespaced: true,
  state: {
    workerProfile: {},
  } as WorkerState,
  mutations: {
    setWorkerProfile(state: WorkerState, data: Partial<WorkerProfile>) {
      state.workerProfile = data;
    },
  },
};
