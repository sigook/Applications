import Vue from 'vue';
import App from './App.vue';
import router from './router';
import i18n from './lang/lang';
import store from './store';
import VueScrollTo from 'vue-scrollto';
import validator from './lang/validator';
import './varaibles';
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



import statusDirective from './directives/status-directive';
import cleaveDirective from '@/directives/cleave-directive';



Vue.directive('status', statusDirective);
Vue.directive('cleave', cleaveDirective);


Vue.component("defaultImage", () => import("./components/DefaultImage.vue"));
Vue.component("vue-editor", VueEditor);
Vue.component('vue-recaptcha', VueRecaptcha)
Vue.mixin(toastMixin);

Vue.config.productionTip = false;
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
} as any).$mount('#app');
