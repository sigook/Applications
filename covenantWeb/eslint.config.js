// eslint.config.js
import js from '@eslint/js'
import pluginVue from 'eslint-plugin-vue'
import tseslint from '@typescript-eslint/eslint-plugin'
import tsparser from '@typescript-eslint/parser'

export default [
  // Ignores
  {
    ignores: ['dist/**', 'node_modules/**', '*.config.js']
  },

  // Base JS config
  js.configs.recommended,

  // Vue 3 essential rules
  ...pluginVue.configs['flat/essential'],

  // Custom config for TypeScript + Vue files
  {
    files: ['**/*.{ts,tsx,vue}'],
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

  // Custom rules (optional)
  {
    rules: {
      // Desactiva reglas que causan problemas comunes
      'vue/multi-word-component-names': 'off',
      // Agrega tus reglas personalizadas aquí
      // 'no-console': 'warn',
    }
  }
]

