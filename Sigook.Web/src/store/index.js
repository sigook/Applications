import Vue from 'vue';
import Vuex from 'vuex';
import createPersistedState from 'vuex-persistedstate';

// modules
import AgencyModule from './modules/agency';
import Catalog from './modules/catalog';
import CompanyModule from './modules/company';
import WorkerModule from './modules/worker';
import SecurityModule from './modules/security';
import Shared from "@/store/modules/shared";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    isMobile: false,
    language: 'en',
    currentDate: null,
    user: null
  },
  mutations: {
    showMobile(state) {
      state.isMobile = true;
    },
    setLanguage(state, data) {
      state.language = data;
      localStorage.setItem('language', data);
    },
    setCurrentDate(state, data) {
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
    getCurrentDate(context) {
      return new Promise((resolve, reject) => {
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