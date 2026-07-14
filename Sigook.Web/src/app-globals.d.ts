import { appGlobals } from '@/varaibles';

type AppGlobals = typeof appGlobals;

declare module 'vue' {
  interface ComponentCustomProperties extends AppGlobals {}
}

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties extends AppGlobals {}
}

export {};
