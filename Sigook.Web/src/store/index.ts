import Vue from 'vue';
import Vuex from 'vuex';
import createPersistedState from 'vuex-persistedstate';

// modules
import AgencyModule from './modules/agency';
import Catalog from './modules/catalog';
import CompanyModule from './modules/company';
import WorkerModule from './modules/worker';
import SecurityModule from './modules/security';
import Shared from './modules/shared';

Vue.use(Vuex);

export interface RootState {
  isMobile: boolean;
  language: string;
  currentDate: Date | null;
  user: any | null;
}

export default new Vuex.Store<RootState>({
  state: {
    isMobile: false,
    language: 'en',
    currentDate: null,
    user: null
  },
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
    catalog: Catalog,
    company: CompanyModule,
    worker: WorkerModule,
    shared: Shared
  },
  actions: {
    getCurrentDate(context: any): Promise<Date> {
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
