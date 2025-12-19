// eslint.config.js
import js from '@eslint/js'
import pluginVue from 'eslint-plugin-vue'
import vueParser from 'vue-eslint-parser'
import tseslint from '@typescript-eslint/eslint-plugin'
import tsparser from '@typescript-eslint/parser'
import globals from 'globals'

export default [
  // Ignores
  {
    ignores: ['dist/**', 'node_modules/**', '*.config.js']
  },

  // Base JS config
  js.configs.recommended,

  // Vue 3 essential rules
  ...pluginVue.configs['flat/essential'],

  // Custom config for Vue files with TypeScript
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
        ...globals.es2021
      }
    },
    plugins: {
      '@typescript-eslint': tseslint
    }
  },

  // Custom config for TypeScript files
  {
    files: ['**/*.{ts,tsx}'],
    languageOptions: {
      parser: tsparser,
      parserOptions: {
        ecmaVersion: 'latest',
        sourceType: 'module'
      }
    },
    plugins: {
      '@typescript-eslint': tseslint
    }
  },

  // Custom rules
  {
    rules: {
      // Desactiva reglas que causan problemas comunes
      'vue/multi-word-component-names': 'off',
      // Agrega tus reglas personalizadas aquí
      // 'no-console': 'warn',
    }
  }
]

