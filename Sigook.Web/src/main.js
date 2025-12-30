import Vue from 'vue';
import App from './App.vue';
import router from './router';
import i18n from './lang/lang';
import store from './store';
import VueScrollTo from 'vue-scrollto';
import validator from './lang/validator';
import variables from './varaibles';
import Buefy from 'buefy'
import { VueEditor } from "vue2-editor";
import { VueRecaptcha } from 'vue-recaptcha';
import toastMixin from "@/mixins/toastMixin";
import VueLazyload from 'vue-lazyload';

// import the styles
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.min.js'
import 'jquery/dist/jquery.min.js'
import 'buefy/dist/buefy.css';
import 'babel-polyfill';

import currencyFilter from './filters/currencyFilter';
import currencyCadFilter from "./filters/currencyCadFilter";
import dateFilter from './filters/dateFilter';
import dateTimeFilter from './filters/dateTimeFilter';
import fileNameFilter from './filters/fileNameFilter';
import dateFromNow from './filters/dateFromNow';
import hourMinutes from './filters/hourMinutes';
import timeFilter from './filters/timeFilter';
import fixedFilter from './filters/fixedHoursFilter';
import capitalizeFilter from './filters/capitalizeFilter';
import sin from "./filters/sinFilter";
import emailName from "@/filters/emailName";
import dateMonth from "@/filters/dateMonth";
import breakWord from './filters/breakWord';
import splitCapital from "@/filters/splitCapital";
import dateHHmm from "@/filters/dateHHmm";
import dateHHmmss from "@/filters/dateHHmmss"
import avatarLetters from "@/filters/avatarLetters";
import agencyTypeFilter from "@/filters/agencyTypeFilter";

import statusDirective from './directives/status-directive';
import cleaveDirective from '@/directives/cleave-directive';
import applicationInsights from "@/utils/applicationInsights";

Vue.filter('currency', currencyFilter);
Vue.filter('currencyCad', currencyCadFilter);
Vue.filter('date', dateFilter);
Vue.filter('filename', fileNameFilter);
Vue.filter('datetime', dateTimeFilter);
Vue.filter('dateFromNow', dateFromNow);
Vue.filter('time', timeFilter);
Vue.filter('hour', fixedFilter);
Vue.filter('hourminutes', hourMinutes);
Vue.filter('lowercase', capitalizeFilter);
Vue.filter('sin', sin);
Vue.filter('emailName', emailName);
Vue.filter('dateMonth', dateMonth);
Vue.filter('breakWord', breakWord);
Vue.filter('splitCapital', splitCapital);
Vue.filter('dateHHmm', dateHHmm);
Vue.filter('dateHHmmss', dateHHmmss);
Vue.filter("avatarLetters", avatarLetters);
Vue.filter('agencyType', agencyTypeFilter);

Vue.directive('status', statusDirective);
Vue.directive('cleave', cleaveDirective);


Vue.component("defaultImage", () => import("./components/DefaultImage"));
Vue.component("vue-editor", VueEditor);
Vue.component('vue-recaptcha', VueRecaptcha)
Vue.mixin(toastMixin);

const appInstrumentationKey = process.env.VUE_APP_INSTRUMENTATION_KEY;

Vue.config.productionTip = false;
Vue.use(applicationInsights, { id: appInstrumentationKey });
Vue.use(VueScrollTo);
Vue.use(Buefy);
Vue.use(VueLazyload, {
  preLoad: 1.3,
  error: require('@/assets/images/default/error.svg'),
  loading: require('@/assets/images/default/loading.svg'),
  attempt: 1
});

new Vue({
  render: h => h(App),
  router,
  i18n,
  store,
  validator,
  variables
}).$mount('#app');
