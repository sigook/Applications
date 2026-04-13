import Vue from 'vue';
import Vuex, { ActionContext } from 'vuex';
import createPersistedState from 'vuex-persistedstate';
import type { UserProfile } from '@/types/security';

// modules
import AgencyModule, { AgencyState } from './modules/agency';
import CompanyModule from './modules/company';
import WorkerModule from './modules/worker';
import SecurityModule, { SecurityState } from './modules/security';

Vue.use(Vuex);

export interface RootState {
  isMobile: boolean;
  language: string;
  currentDate: Date | null;
  user: UserProfile | null;
  security: SecurityState;
  agency: AgencyState;
}

export default new Vuex.Store<RootState>({
  // Module states (security, agency, etc.) are populated by Vuex at runtime from the `modules` option.
  state: {
    isMobile: false,
    language: 'en',
    currentDate: null,
    user: null
  } as RootState,
  mutations: {
    showMobile(state: RootState) {
      state.isMobile = true;
    },
    setLanguage(state: RootState, data: string) {
      state.language = data;
      localStorage.setItem('language', data);
    },
    setCurrentDate(state: RootState, data: Date) {
      state.currentDate = data;
    }
  },
  modules: {
    security: SecurityModule,
    agency: AgencyModule,
    company: CompanyModule,
    worker: WorkerModule,
  },
  actions: {
    getCurrentDate(context: ActionContext<RootState, RootState>): Promise<Date> {
      return new Promise((resolve) => {
        context.commit('setCurrentDate', new Date());
        resolve(new Date());
      });
    }
  },
  plugins: [createPersistedState({
    key: 'user',
    paths: ['security']
  })]
});
