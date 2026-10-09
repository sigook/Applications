import { defineStore } from 'pinia';

interface AppState {
  isMobile: boolean;
  currentDate: Date | null;
}

export const useAppStore = defineStore('app', {
  state: (): AppState => ({
    isMobile: false,
    currentDate: null,
  }),
  actions: {
    showMobile() {
      this.isMobile = true;
    },
    getCurrentDate(): Promise<Date> {
      const now = new Date();
      this.currentDate = now;
      return Promise.resolve(now);
    },
  },
});
