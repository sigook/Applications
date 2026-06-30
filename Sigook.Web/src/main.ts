import { createApp, defineAsyncComponent } from 'vue';
import App from './App.vue';
import router from './router';
import pinia from './stores';
import VueScrollTo from 'vue-scrollto';
import { registerAppGlobals } from './varaibles';
import Buefy from '@ntohq/buefy-next';
import { QuillEditor } from '@vueup/vue-quill';
import VueLazyload from 'vue-lazyload';
import errorImage from '@/assets/images/default/error.svg';
import loadingImage from '@/assets/images/default/loading.svg';
import { registerValidationRules } from '@/lang/validator';
import { setupBuefyProgrammatic } from '@/utils/buefyProgrammatic';
import mgr from '@/security/securityService';
import { useSecurityStore } from '@/stores/security';

// import the styles
import '@/assets/css/bootstrap-layered.css';
import '@ntohq/buefy-next/dist/buefy.css';
import '@vueup/vue-quill/dist/vue-quill.snow.css';

import statusDirective from './directives/status-directive';

registerValidationRules();

const app = createApp(App);

registerAppGlobals(app);

app.directive('status', statusDirective);

app.component('defaultImage', defineAsyncComponent(() => import('./components/DefaultImage.vue')));
app.component('QuillEditor', QuillEditor);

app.use(router);
app.use(pinia);

const securityStore = useSecurityStore(pinia);
mgr.events.addUserLoaded((user) => {
  securityStore.setUser(user as any);
});
mgr.events.addUserUnloaded(() => {
  securityStore.setUser(null);
});
mgr.events.addAccessTokenExpired(() => {
  mgr.signinSilent().catch(() => {
    securityStore.setUser(null);
    securityStore.signIn();
  });
});
mgr.events.addSilentRenewError(() => {
  securityStore.setUser(null);
});

app.use(Buefy);
setupBuefyProgrammatic(app);
app.use(VueScrollTo);
app.use(VueLazyload, {
  preLoad: 1.3,
  error: errorImage,
  loading: loadingImage,
  attempt: 1,
});

app.mount('#app');
