import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import { fileURLToPath, URL } from 'node:url';

export default defineConfig({
  plugins: [vue()],
  envPrefix: 'VUE_APP_',
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 3001,
    strictPort: true,
  },
  build: {
    outDir: 'wwwroot',
    emptyOutDir: true,
    sourcemap: false,
    chunkSizeWarningLimit: 1024,
    rollupOptions: {
      output: {
        manualChunks(id) {
          if (id.includes('node_modules')) {
            if (/[\\/]node_modules[\\/](vue|vue-router|pinia|@vue)[\\/]/.test(id)) return 'vue-vendor';
            if (/[\\/]node_modules[\\/](buefy|bulma|@fortawesome)[\\/]/.test(id)) return 'ui-vendor';
            if (/[\\/]node_modules[\\/]@vueup[\\/]vue-quill[\\/]/.test(id)) return 'editor-vendor';
            if (/[\\/]node_modules[\\/](cropperjs|vue-cropperjs|image-compressor\.js)[\\/]/.test(id)) return 'image-vendor';
            if (/[\\/]node_modules[\\/]google-libphonenumber[\\/]/.test(id)) return 'phone-vendor';
            if (/[\\/]node_modules[\\/]oidc-client-ts[\\/]/.test(id)) return 'auth-vendor';
            if (/[\\/]node_modules[\\/](vee-validate|yup|@vee-validate)[\\/]/.test(id)) return 'validation-vendor';
            return 'vendor';
          }
        },
      },
    },
  },
  css: {
    lightningcss: {
      errorRecovery: true,
    } as any,
    preprocessorOptions: {
      scss: {
        api: 'modern-compiler',
        silenceDeprecations: ['import', 'legacy-js-api'],
      },
      sass: {
        api: 'modern-compiler',
        silenceDeprecations: ['import', 'legacy-js-api'],
      },
    },
  },
});
