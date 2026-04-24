/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VUE_APP_URL_API: string;
  readonly VUE_APP_CLIENT: string;
  readonly VUE_APP_SECURITY_SERVER: string;
  readonly VUE_APP_MAXIMUM_HOURS_DAY: string;
  readonly VUE_APP_SIGOOK_NOTIFICATION: string;
  readonly VUE_APP_RE_CAPTCHA_SITE_KEY: string;
  readonly VUE_APP_ENV?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
