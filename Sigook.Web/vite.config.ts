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
            if (/[\\/]node_modules[\\/](vue|vue-router|vuex|@vue)[\\/]/.test(id)) return 'vue-vendor';
            if (/[\\/]node_modules[\\/](buefy|bootstrap|@fortawesome)[\\/]/.test(id)) return 'ui-vendor';
            return 'vendor';
          }
          if (/[\\/]src[\\/]pages[\\/]agency[\\/]/.test(id)) return 'agency-chunk';
          if (/[\\/]src[\\/]pages[\\/]company[\\/]/.test(id)) return 'company-chunk';
          if (/[\\/]src[\\/]pages[\\/]worker[\\/]/.test(id)) return 'worker-chunk';
          if (/[\\/]src[\\/]pages[\\/]landing[\\/]/.test(id)) return 'landing-chunk';
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
