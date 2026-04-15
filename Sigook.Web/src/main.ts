import Vue from 'vue';
import App from './App.vue';
import router from './router';
import pinia from './stores';
import VueScrollTo from 'vue-scrollto';
import validator from './lang/validator';
import './varaibles';
import Buefy from 'buefy'
import { VueEditor } from "vue2-editor";
import { VueRecaptcha } from 'vue-recaptcha';
import VueLazyload from 'vue-lazyload';
import errorImage from '@/assets/images/default/error.svg';
import loadingImage from '@/assets/images/default/loading.svg';

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

Vue.config.productionTip = false;
Vue.use(VueScrollTo);
Vue.use(Buefy);
Vue.use(VueLazyload, {
  preLoad: 1.3,
  error: errorImage,
  loading: loadingImage,
  attempt: 1
});

new Vue({
  render: h => h(App),
  router,
  pinia,
  validator
} as any).$mount('#app');
