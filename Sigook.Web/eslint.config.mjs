import js from '@eslint/js'
import pluginVue from 'eslint-plugin-vue'
import vueParser from 'vue-eslint-parser'
import tseslint from '@typescript-eslint/eslint-plugin'
import tsparser from '@typescript-eslint/parser'
import globals from 'globals'

export default [
  {
    ignores: [
      'dist/**',
      'node_modules/**',
      'wwwroot/**',
      'public/**',
      '*.config.js',
      '*.config.mjs'
    ]
  },

  js.configs.recommended,

  ...pluginVue.configs['flat/essential'],

  {
    files: ['**/*.vue'],
    languageOptions: {
      parser: vueParser,
      parserOptions: {
        parser: tsparser,
        ecmaVersion: 'latest',
        sourceType: 'module'
      },
      globals: {
        ...globals.browser,
        ...globals.node,
        ...globals.es2021,
        defineProps: 'readonly',
        defineEmits: 'readonly',
        defineExpose: 'readonly',
        withDefaults: 'readonly',
        defineOptions: 'readonly'
      }
    },
    plugins: {
      '@typescript-eslint': tseslint
    }
  },

  {
    files: ['**/*.{ts,tsx}'],
    languageOptions: {
      parser: tsparser,
      parserOptions: {
        ecmaVersion: 'latest',
        sourceType: 'module'
      },
      globals: {
        ...globals.browser,
        ...globals.node,
        ...globals.es2021
      }
    },
    plugins: {
      '@typescript-eslint': tseslint
    }
  },

  {
    files: ['**/*.js'],
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
        ...globals.es2021
      }
    }
  },

  {
    plugins: {
      '@typescript-eslint': tseslint
    },
    rules: {
      'no-console': 'off',
      'no-debugger': 'off',
      'no-unused-vars': 'off',
      'no-irregular-whitespace': 'warn',
      'no-undef': 'off',
      '@typescript-eslint/no-unused-vars': [
        'warn',
        {
          argsIgnorePattern: '^_',
          varsIgnorePattern: '^(_|[A-Z])'
        }
      ],
      '@typescript-eslint/no-explicit-any': 'error',
      '@typescript-eslint/no-var-requires': 'off',
      '@typescript-eslint/no-require-imports': 'off',
      '@typescript-eslint/explicit-module-boundary-types': 'off',
      '@typescript-eslint/no-non-null-assertion': 'warn',
      '@typescript-eslint/no-empty-interface': 'warn',
      'vue/require-v-for-key': 'warn',
      'vue/no-unused-vars': 'warn',
      'vue/no-parsing-error': 'warn',
      'vue/no-mutating-props': 'warn',
      'vue/multi-word-component-names': 'off',
      'vue/no-reserved-component-names': 'off',
      'vue/no-v-model-argument': 'off',
      // Catch <PascalCase> tags in templates that have no matching import or
      // registration. Without this, missing imports silently degrade to
      // unknown HTML elements (e.g. <eyebrowpillv2> rendered as plain span).
      // ignorePatterns covers globally-registered components from vue-router
      // and Buefy plus Vue built-ins.
      'vue/no-undef-components': ['error', {
        ignorePatterns: [
          // Vue Router
          'router-view',
          'router-link',
          'RouterView',
          'RouterLink',
          // Vue built-ins
          'transition',
          'transition-group',
          'Transition',
          'TransitionGroup',
          'keep-alive',
          'KeepAlive',
          'teleport',
          'Teleport',
          'suspense',
          'Suspense',
          'component',
          'slot',
          // Buefy (globally registered via buefy)
          '^[Bb]-.*',
          '^[Bb][A-Z].*',
          // Globally registered in src/main.ts
          '^[Qq]uill[Ee]ditor$',
          'default-image',
          'defaultImage',
          'DefaultImage'
        ]
      }]
    }
  },

  {
    files: ['**/*.js'],
    rules: {
      '@typescript-eslint/no-unused-vars': 'off'
    }
  },

  ...['agency', 'company', 'worker', 'landing'].map((module) => ({
    files: [`src/modules/${module}/**/*.{ts,vue}`],
    ignores: [`src/modules/${module}/routes.ts`],
    rules: {
      'no-restricted-imports': ['error', {
        patterns: [{
          group: ['agency', 'company', 'worker', 'landing'].filter((other) => other !== module).map((other) => `@/modules/${other}/*`),
          message: 'A module must not import another module. Move the shared piece to src/shared or pass it as a prop; only routes.ts may mount another module\'s page.'
        }]
      }]
    }
  })),

  {
    files: ['src/shared/**/*.{ts,vue}'],
    rules: {
      'no-restricted-imports': ['error', {
        patterns: [{
          group: ['@/modules/*'],
          message: 'src/shared must not depend on a module. Receive the function or component as a prop.'
        }]
      }]
    }
  }
]
