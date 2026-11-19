import { defineConfig, mergeConfig } from 'vitest/config';
import viteConfig from './vite.config';

export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      environment: 'happy-dom',
      env: { TZ: 'America/Toronto' },
      include: ['src/**/*.spec.ts'],
      setupFiles: ['src/testing/setup.ts'],
      testTimeout: 15000,
      restoreMocks: true,
      unstubGlobals: true,
      coverage: {
        provider: 'v8',
        include: ['src/**/*.{ts,vue}'],
        exclude: [
          'src/**/*.spec.ts',
          'src/**/*.d.ts',
          'src/**/types.ts',
          'src/**/routes.ts',
          'src/main.ts',
          'src/testing/**',
        ],
        reporter: ['text-summary', 'cobertura', 'html'],
        reportsDirectory: 'coverage',
        thresholds: {
          statements: 93,
          branches: 94,
          functions: 86,
          lines: 94,
        },
      },
    },
  }),
);
