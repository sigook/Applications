// eslint.config.js
import js from '@eslint/js'
import vueParser from 'vue-eslint-parser'
import pluginVue from 'eslint-plugin-vue'
import tsParser from '@typescript-eslint/parser'
import tsPlugin from '@typescript-eslint/eslint-plugin'

export default [
  {
    ignores: ['dist/**', 'node_modules/**']
  },
  {
    files: ['**/*.{js,jsx,ts,tsx,vue}'],
    languageOptions: {
      parser: vueParser,
      parserOptions: {
        parser: tsParser,
        ecmaVersion: 2020,
        sourceType: 'module',
        extraFileExtensions: ['.vue']
      }
    },
    plugins: {
      vue: pluginVue,
      '@typescript-eslint': tsPlugin
    },
    rules: {
      // Reglas base de JS
      ...js.configs.recommended.rules,
      // Reglas base de Vue 3
      ...pluginVue.configs['vue3-essential'].rules
      // Aquí puedes agregar tus reglas personalizadas si tenías
      // 'no-console': 'warn',
    }
  }
]

