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
      '@typescript-eslint/no-explicit-any': 'off',
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
          // VeeValidate (auto-imported / globally registered)
          '^[Ff]ield$',
          '^[Ff]orm$',
          '^[Ee]rror[Mm]essage$',
          // Vue Quill editor
          '^[Qq]uill[Ee]ditor$',
          // Globally registered in src/main.ts
          'default-image',
          'defaultImage',
          'DefaultImage',
          // Third-party globals
          'vue-recaptcha',
          'VueRecaptcha'
        ]
      }]
    }
  },

  {
    files: ['**/*.js'],
    rules: {
      '@typescript-eslint/no-unused-vars': 'off'
    }
  }
]
