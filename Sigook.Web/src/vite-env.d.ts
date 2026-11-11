interface ImportMetaEnv {
  readonly VUE_APP_URL_API: string;
  readonly VUE_APP_CLIENT: string;
  readonly VUE_APP_SECURITY_SERVER: string;
  readonly VUE_APP_MAXIMUM_HOURS_DAY: string;
  readonly VUE_APP_RE_CAPTCHA_SITE_KEY: string;
}

declare module 'vue-cropperjs' {
  import type { DefineComponent } from 'vue';
  const VueCropper: DefineComponent;
  export default VueCropper;
}

declare module 'vue3-recaptcha2' {
  import type { DefineComponent } from 'vue';
  const Vue3Recaptcha2: DefineComponent;
  export default Vue3Recaptcha2;
}
