import { createApp, defineAsyncComponent } from 'vue';
import App from '@/app/App.vue';
import router from '@/app/router';
import pinia from '@/app/stores';
import Buefy from 'buefy';
import { QuillEditor } from '@vueup/vue-quill';
import { registerValidationRules } from '@/lang/validator';
import { setupBuefyProgrammatic } from '@/shared/utils/buefyProgrammatic';
import mgr from '@/app/security/securityService';
import { useSecurityStore } from '@/app/stores/security';
import type { UserProfile } from '@/shared/types/security';

// import the styles
import 'buefy/dist/css/buefy.css';
import '@vueup/vue-quill/dist/vue-quill.snow.css';
import '@/assets/scss/landing-form.scss';

registerValidationRules();

const CHUNK_RELOAD_KEY = 'chunkReloadAt';
const CHUNK_RELOAD_COOLDOWN_MS = 10_000;

window.addEventListener('vite:preloadError', (event) => {
  const lastReload = Number(sessionStorage.getItem(CHUNK_RELOAD_KEY) ?? 0);
  if (Date.now() - lastReload < CHUNK_RELOAD_COOLDOWN_MS) return;
  event.preventDefault();
  sessionStorage.setItem(CHUNK_RELOAD_KEY, String(Date.now()));
  window.location.reload();
});

const app = createApp(App);

app.component('defaultImage', defineAsyncComponent(() => import('@/shared/ui/DefaultImage.vue')));
app.component('QuillEditor', QuillEditor);

app.use(router);
app.use(pinia);

const securityStore = useSecurityStore(pinia);
mgr.events.addUserLoaded((user) => {
  securityStore.setUser(user as unknown as UserProfile);
});
mgr.events.addUserUnloaded(() => {
  securityStore.setUser(null);
});
mgr.events.addAccessTokenExpired(() => {
  securityStore.silentSignin().catch(() => {
    securityStore.setUser(null);
  });
});
mgr.events.addSilentRenewError(() => {
  securityStore.setUser(null);
});

app.use(Buefy);
setupBuefyProgrammatic(app);

app.mount('#app');
