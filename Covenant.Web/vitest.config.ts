import { defineConfig, mergeConfig } from 'vitest/config'
import viteConfig from './vite.config'

export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      environment: 'happy-dom',
      env: { TZ: 'America/Toronto', VITE_API_BASE_URL: 'http://localhost' },
      include: ['src/**/*.spec.ts'],
      setupFiles: ['src/testing/setup.ts'],
      restoreMocks: true,
      unstubGlobals: true,
      server: {
        deps: {
          inline: ['vuetify'],
        },
      },
      coverage: {
        provider: 'v8',
        include: ['src/**/*.{ts,vue}'],
        exclude: [
          'src/**/*.spec.ts',
          'src/**/*.d.ts',
          'src/**/*.types.ts',
          'src/main.ts',
          'src/testing/**',
        ],
        reporter: ['text-summary', 'cobertura', 'html'],
        reportsDirectory: 'coverage',
        thresholds: {
          statements: 12,
          branches: 6,
          functions: 9,
          lines: 13,
        },
      },
    },
  }),
)
